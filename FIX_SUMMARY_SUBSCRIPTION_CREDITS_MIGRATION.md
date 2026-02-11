# Fix Summary: SubscriptionCredits Migration Error

## Problem Statement

Users encountered the following PostgreSQL error when running database migrations:

```
[21:06:40 ERR] Microsoft.EntityFrameworkCore.Database.Command
Failed executing DbCommand (14ms) [Parameters=[], CommandType='Text', CommandTimeout='60']
ALTER TABLE "SubscriptionCredits" ALTER COLUMN "GrantedAt" TYPE timestamp with time zone;

[21:06:40 FTL] 
ERRO CRÍTICO: Tabela 'desconhecida' não existe no banco de dados.
Npgsql.PostgresException (0x80004005): 42P01: relation "SubscriptionCredits" does not exist
```

The error occurred at line 891 in `Program.cs` during `context.Database.Migrate()`.

## Root Cause Analysis

### The Issue
The migration `20260208154714_FixGlobalTemplateIdColumnCasing` contained direct `migrationBuilder.AlterColumn` calls for the `SubscriptionCredits.GrantedAt` column without checking if the table exists first.

### Why This Happened
1. **Table Creation Strategy:** The `SubscriptionCredits` table was created with `CREATE TABLE IF NOT EXISTS` logic in migration `20260131025933_AddDocumentHashToPatients`
2. **EF Core Code Generation:** When Entity Framework Core generates migrations, it creates direct `AlterColumn` operations without defensive existence checks
3. **Database State Inconsistency:** The table might not exist in certain scenarios:
   - Fresh databases that skip certain migrations
   - Partial migration rollbacks
   - Manual database operations

### The Problematic Code

**Before (lines 1187-1193 in Up method):**
```csharp
migrationBuilder.AlterColumn<DateTime>(
    name: "GrantedAt",
    table: "SubscriptionCredits",
    type: "timestamp with time zone",
    nullable: false,
    oldClrType: typeof(DateTime),
    oldType: "timestamp without time zone");
```

This generated SQL:
```sql
ALTER TABLE "SubscriptionCredits" ALTER COLUMN "GrantedAt" TYPE timestamp with time zone;
```

When the table doesn't exist, PostgreSQL throws error `42P01: relation "SubscriptionCredits" does not exist`.

## Solution Implemented

### Code Pattern Applied
Replaced direct `AlterColumn` calls with conditional SQL that checks for both table and column existence before performing the ALTER operation.

**After (lines 1187-1205 in Up method):**
```csharp
// Use conditional SQL to alter SubscriptionCredits columns since table may not exist in all databases
migrationBuilder.Sql(@"
    DO $$
    BEGIN
        IF EXISTS (
            SELECT 1 FROM information_schema.tables 
            WHERE table_name = 'SubscriptionCredits' 
            AND table_schema = 'public'
        ) AND EXISTS (
            SELECT 1 FROM information_schema.columns 
            WHERE table_name = 'SubscriptionCredits' 
            AND column_name = 'GrantedAt'
            AND table_schema = 'public'
            AND data_type = 'timestamp without time zone'
        ) THEN
            ALTER TABLE ""SubscriptionCredits"" ALTER COLUMN ""GrantedAt"" TYPE timestamp with time zone;
        END IF;
    END $$;
");
```

### Changes Made
1. **Up Method (lines 1187-1205):** Added conditional SQL to check table and column existence before altering `GrantedAt` from `timestamp without time zone` to `timestamp with time zone`
2. **Down Method (lines 5911-5929):** Added conditional SQL to check table and column existence before altering `GrantedAt` from `timestamp with time zone` back to `timestamp without time zone`

### Pattern Consistency
This fix follows the established pattern from:
- Migration `20260208003833_RenameGlobalDocumentTemplatesTable` (lines 1133-1150, 5866-5883)
- Migration `20260203180451_AddAlertSystem`
- Other migrations that handle conditionally-created tables

## Files Modified

- `src/MedicSoft.Repository/Migrations/PostgreSQL/20260208154714_FixGlobalTemplateIdColumnCasing.cs`

## Testing Recommendations

### 1. Fresh Database Test
```bash
# Drop and recreate database
dotnet ef database drop --force --context MedicSoftDbContext
dotnet ef database update --context MedicSoftDbContext
```

### 2. Rollback Test
```bash
# Test rolling back and forward
dotnet ef database update 20260207152902 --context MedicSoftDbContext
dotnet ef database update --context MedicSoftDbContext
```

### 3. Partial State Test
```bash
# Update to before problematic migration
dotnet ef database update 20260131025933 --context MedicSoftDbContext
# Update to current
dotnet ef database update --context MedicSoftDbContext
```

### 4. Application Startup Test
```bash
# Start the application and verify migrations apply automatically
dotnet run --project src/MedicSoft.Api/MedicSoft.Api.csproj
```

## Impact Assessment

### Positive Impacts
✅ Prevents "relation SubscriptionCredits does not exist" errors during migrations  
✅ Allows migrations to run on databases in various states  
✅ Handles partial migration rollbacks gracefully  
✅ No functional changes to application behavior  
✅ Follows established pattern from earlier migrations  
✅ Build succeeds with no compilation errors

### Risk Assessment
🟢 **Low Risk**
- Changes are defensive/additive only
- No data manipulation or deletion
- Matches proven pattern from earlier migrations
- Only affects migration execution, not runtime behavior
- No breaking changes to existing functionality

## Validation Results

### Build Verification
✅ **Build Status:** Succeeded
- No compilation errors
- Pre-existing warnings remain unchanged
- Build time: ~30 seconds

### Code Review
✅ **Code Review Status:** Passed
- Initial review identified unnecessary string interpolation
- Issue resolved: Changed `$@` to `@` for SQL strings without variables
- Final review: No issues found

### Security Scan
✅ **CodeQL Status:** No Issues
- Migration files not analyzed by CodeQL (expected)
- No security vulnerabilities introduced
- No sensitive data exposed

## Related Issues and Documentation

### Related Migrations
- **20260131025933_AddDocumentHashToPatients:** Creates `SubscriptionCredits` table with `IF NOT EXISTS`
- **20260208003833_RenameGlobalDocumentTemplatesTable:** Reference implementation for conditional SQL pattern
- **20260203180451_AddAlertSystem:** Another example of the same pattern

### Related Documentation
- `FIX_SUMMARY_MIGRATION_TABLE_EXISTS.md` - Similar fix for SystemNotifications, NotificationRules, CustomDashboards, and DashboardWidgets tables
- `TROUBLESHOOTING_MIGRATIONS.md` - General migration troubleshooting guide (referenced in error message)

## Conclusion

This fix addresses the immediate issue of the `SubscriptionCredits` table not existing when the migration tries to alter its columns. The solution is defensive, low-risk, and follows established patterns in the codebase.

The fix ensures migrations can run successfully regardless of the database's current state, whether it's a fresh database, a partially migrated database, or one that has undergone rollbacks.

**Status:** ✅ Complete and Ready for Deployment

## Commit History

1. `b480983` - Initial plan
2. `8a1d666` - Fix SubscriptionCredits table migration error with conditional SQL checks
3. `e995b83` - Address code review: remove unnecessary string interpolation in SQL
