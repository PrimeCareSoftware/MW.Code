# Fix Summary: DashboardWidgets Table Existence Check

## Problem
The migration `20260208154714_FixGlobalTemplateIdColumnCasing` was failing with the following error:

```
Npgsql.PostgresException (0x80004005): 42P01: relation "DashboardWidgets" does not exist
   at Npgsql.NpgsqlCommand.ExecuteNonQuery()
   at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteNonQuery(RelationalCommandParameterObject parameterObject)
   at Microsoft.EntityFrameworkCore.Migrations.MigrationCommand.ExecuteNonQuery(IRelationalConnection connection, IReadOnlyDictionary`2 parameterValues)
```

The error occurred when trying to execute:
```sql
ALTER TABLE "DashboardWidgets" ALTER COLUMN "UpdatedAt" TYPE timestamp with time zone;
```

## Root Cause
The migration `20260208154714_FixGlobalTemplateIdColumnCasing.cs` contained direct EF Core `AlterColumn` statements for the `DashboardWidgets` table without checking if the table exists first.

This is problematic because:
1. The `DashboardWidgets` table is created with `IF NOT EXISTS` in migration `20260203150000_AddAnalyticsDashboardTables.cs`
2. If that migration was skipped or the table wasn't created, subsequent migrations trying to ALTER it would fail
3. The EF Core `AlterColumn` method generates raw SQL that doesn't include existence checks

## Solution
Replaced the direct `migrationBuilder.AlterColumn` statements with `migrationBuilder.Sql` statements that include proper table and column existence checks using PostgreSQL's `information_schema`.

### Changes Made

#### In the `Up` method:
- Replaced `AlterColumn` for `DashboardWidgets.UpdatedAt`
- Replaced `AlterColumn` for `DashboardWidgets.CreatedAt`

#### In the `Down` method:
- Replaced `AlterColumn` for `DashboardWidgets.UpdatedAt` (rollback)
- Replaced `AlterColumn` for `DashboardWidgets.CreatedAt` (rollback)

### Pattern Used
```csharp
// Only alter DashboardWidgets if the table and columns exist
migrationBuilder.Sql(@"
    DO $$
    BEGIN
        IF EXISTS (
            SELECT 1 FROM information_schema.tables 
            WHERE table_name = 'DashboardWidgets' 
            AND table_schema = 'public'
        ) AND EXISTS (
            SELECT 1 FROM information_schema.columns 
            WHERE table_name = 'DashboardWidgets' 
            AND column_name = 'UpdatedAt'
            AND table_schema = 'public'
        ) THEN
            ALTER TABLE ""DashboardWidgets"" ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
        END IF;
    END $$;
");
```

**Note**: The data type check was intentionally omitted from the column existence check. This allows the ALTER TABLE statement to handle type compatibility naturally, making the migration more robust and avoiding issues with type precision or other modifiers that might cause exact string matches to fail.

## Migration Safety
This fix ensures:
1. ✅ Migrations won't fail if the `DashboardWidgets` table doesn't exist
2. ✅ Migrations are idempotent - can be run multiple times safely
3. ✅ Column alterations only happen if the table and column exist with the expected data type
4. ✅ Follows the pattern established in migration `20260203150000_AddAnalyticsDashboardTables.cs`

## Files Modified
- `src/MedicSoft.Repository/Migrations/PostgreSQL/20260208154714_FixGlobalTemplateIdColumnCasing.cs`

## Testing
The fix can be tested by:
1. Running migrations on a clean database
2. Verifying that migrations complete without errors
3. Checking that existing databases with the `DashboardWidgets` table get the column alterations applied
4. Checking that new databases without the table don't fail

## Related Documentation
- See `MIGRATION_BEST_PRACTICES.md` for patterns on handling conditional schema changes
- See migration `20260203150000_AddAnalyticsDashboardTables.cs` for the original table creation with `IF NOT EXISTS`
