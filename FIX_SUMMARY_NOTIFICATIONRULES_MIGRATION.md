# Fix Summary: NotificationRules Migration Error

## Problem Statement

The application was failing to start with the following error during database migration:

```
[21:18:19 ERR] Microsoft.EntityFrameworkCore.Database.Command
Failed executing DbCommand (20ms) [Parameters=[], CommandType='Text', CommandTimeout='60']
ALTER TABLE "NotificationRules" ALTER COLUMN "UpdatedAt" TYPE timestamp with time zone;

[21:18:19 FTL] 
ERRO CRÍTICO: Tabela 'desconhecida' não existe no banco de dados. 
Isso indica que as migrações não foram aplicadas corretamente.

Npgsql.PostgresException (0x80004005): 42P01: relation "NotificationRules" does not exist
```

## Root Cause Analysis

### Migration Timeline

The `NotificationRules` table was created in migration `20260129200623_AddModuleConfigurationHistoryAndEnhancedModules`:

```csharp
migrationBuilder.CreateTable(
    name: "NotificationRules",
    columns: table => new
    {
        // ...
        CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
        UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
        // ...
    });
```

### The Issue

Migration `20260208154714_FixGlobalTemplateIdColumnCasing` attempted to alter the `NotificationRules` table columns using **unconditional** `migrationBuilder.AlterColumn()` calls:

```csharp
// BEFORE (PROBLEMATIC CODE)
migrationBuilder.AlterColumn<DateTime>(
    name: "UpdatedAt",
    table: "NotificationRules",
    type: "timestamp with time zone",
    nullable: true,
    oldClrType: typeof(DateTime),
    oldType: "timestamp without time zone",
    oldNullable: true);

migrationBuilder.AlterColumn<DateTime>(
    name: "CreatedAt",
    table: "NotificationRules",
    type: "timestamp with time zone",
    nullable: false,
    oldClrType: typeof(DateTime),
    oldType: "timestamp without time zone");
```

These unconditional ALTER statements would fail if:
1. The `NotificationRules` table didn't exist yet
2. Migrations were applied out of order
3. There was a previous migration failure that needed recovery
4. The database was in an inconsistent state

## Solution Implemented

### Changes Made

Replaced unconditional `migrationBuilder.AlterColumn()` calls with **conditional SQL** that checks for table and column existence before executing ALTER statements.

#### Up() Method Fix

```csharp
// AFTER (FIXED CODE)
// Conditionally alter NotificationRules table columns only if table and columns exist with old type
migrationBuilder.Sql(@"
    DO $$
    BEGIN
        IF EXISTS (
            SELECT 1 FROM information_schema.tables 
            WHERE table_name = 'NotificationRules'
            AND table_schema = 'public'
        ) THEN
            IF EXISTS (
                SELECT 1 FROM information_schema.columns 
                WHERE table_name = 'NotificationRules' 
                AND column_name = 'UpdatedAt'
                AND table_schema = 'public'
                AND data_type = 'timestamp without time zone'
            ) THEN
                ALTER TABLE ""NotificationRules"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
            END IF;
            
            IF EXISTS (
                SELECT 1 FROM information_schema.columns 
                WHERE table_name = 'NotificationRules' 
                AND column_name = 'CreatedAt'
                AND table_schema = 'public'
                AND data_type = 'timestamp without time zone'
            ) THEN
                ALTER TABLE ""NotificationRules"" ALTER COLUMN ""CreatedAt"" TYPE timestamp with time zone;
            END IF;
        END IF;
    END $$;
");
```

#### Down() Method Fix

Similar conditional checks were applied to the `Down()` method to ensure safe rollback:

```csharp
migrationBuilder.Sql(@"
    DO $$
    BEGIN
        IF EXISTS (
            SELECT 1 FROM information_schema.tables 
            WHERE table_name = 'NotificationRules'
            AND table_schema = 'public'
        ) THEN
            IF EXISTS (
                SELECT 1 FROM information_schema.columns 
                WHERE table_name = 'NotificationRules' 
                AND column_name = 'UpdatedAt'
                AND table_schema = 'public'
                AND data_type = 'timestamp with time zone'
            ) THEN
                ALTER TABLE ""NotificationRules"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp without time zone;
            END IF;
            
            IF EXISTS (
                SELECT 1 FROM information_schema.columns 
                WHERE table_name = 'NotificationRules' 
                AND column_name = 'CreatedAt'
                AND table_schema = 'public'
                AND data_type = 'timestamp with time zone'
            ) THEN
                ALTER TABLE ""NotificationRules"" ALTER COLUMN ""CreatedAt"" TYPE timestamp without time zone;
            END IF;
        END IF;
    END $$;
");
```

## Benefits of This Fix

1. **Idempotent**: The migration can be run multiple times safely without causing errors
2. **Resilient**: Handles cases where the table doesn't exist or is in an unexpected state
3. **Consistent**: Follows the same pattern used in other migrations (e.g., `20260203180451_AddAlertSystem`, `20260202124905_AddShowInAppointmentSchedulingToUser`)
4. **Safe Rollback**: The Down() method also uses conditional checks for safe reversibility
5. **Better Error Handling**: Prevents cryptic "relation does not exist" errors during migration

## Testing & Verification

### Build Verification
✅ Project builds successfully with no compilation errors
```
Build SUCCEEDED.
    96 Warning(s)
    0 Error(s)
Time Elapsed 00:01:12.81
```

### Code Review
✅ No review comments or issues found

### Security Scan
✅ CodeQL analysis found no security vulnerabilities

## Pattern for Future Migrations

When altering tables that might not exist or might be in different states, always use conditional SQL:

```csharp
migrationBuilder.Sql(@"
    DO $$
    BEGIN
        IF EXISTS (
            SELECT 1 FROM information_schema.tables 
            WHERE table_name = 'YourTableName'
            AND table_schema = 'public'
        ) THEN
            -- Check column data type before altering
            IF EXISTS (
                SELECT 1 FROM information_schema.columns 
                WHERE table_name = 'YourTableName' 
                AND column_name = 'YourColumnName'
                AND table_schema = 'public'
                AND data_type = 'old_type'
            ) THEN
                ALTER TABLE ""YourTableName"" ALTER COLUMN ""YourColumnName"" TYPE new_type;
            END IF;
        END IF;
    END $$;
");
```

## Files Modified

- `src/MedicSoft.Repository/Migrations/PostgreSQL/20260208154714_FixGlobalTemplateIdColumnCasing.cs`
  - Modified Up() method (lines 2075-2105)
  - Modified Down() method (lines 6814-6844)

## Impact Assessment

- **Risk Level**: Low - This is a defensive fix that makes migrations more robust
- **Breaking Changes**: None - The fix maintains the same migration behavior but with better error handling
- **Deployment**: Can be deployed immediately - no special migration steps required
- **Rollback**: Safe - Down() method also uses conditional checks

## Related Migrations

Other migrations that correctly use conditional checks for NotificationRules:
- `20260203180451_AddAlertSystem.cs`
- `20260202124905_AddShowInAppointmentSchedulingToUser.cs`
- `20260202020547_AddScheduleBlockingFeature.cs`
- `20260203135829_AddCalendarColorToUsers.cs`
- `20260206135118_AddExternalServiceConfiguration.cs`

## Security Summary

No security vulnerabilities were introduced or discovered:
- ✅ SQL injection: Protected by parameterized table/column names with double quotes
- ✅ Privilege escalation: No changes to user permissions or access control
- ✅ Data integrity: Conditional checks ensure data is only altered when appropriate
- ✅ No secrets or sensitive data in migration code
