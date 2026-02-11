# Migration Best Practices Guide

## Overview

This guide provides essential patterns and best practices for creating Entity Framework Core migrations in the MedicSoft system, with special focus on defensive migration patterns that prevent "relation does not exist" errors.

## Table of Contents

1. [Understanding the Problem](#understanding-the-problem)
2. [Tables Requiring Defensive Checks](#tables-requiring-defensive-checks)
3. [The Defensive Migration Pattern](#the-defensive-migration-pattern)
4. [Decision Tree](#decision-tree)
5. [Code Examples](#code-examples)
6. [Testing Checklist](#testing-checklist)
7. [Common Pitfalls](#common-pitfalls)

## Understanding the Problem

### What Causes "Relation Does Not Exist" Errors?

When a migration creates tables using `CREATE TABLE IF NOT EXISTS`, the table creation is **conditional**. This means:

- ✅ If the table doesn't exist, it will be created
- ⚠️ If the table already exists, creation is skipped silently
- ❌ Future migrations that ALTER these tables will fail if the table was skipped

### When Does This Happen?

This issue occurs in several scenarios:

1. **Fresh database** - Table creation is skipped because it already exists from a manual script
2. **Partial rollback** - Migration history is incomplete after a failed rollback
3. **Database import** - Schema was imported from another source without migration history
4. **Development environment** - Developer manually created tables for testing

### The Core Problem

```csharp
// Migration A: Creates table conditionally
migrationBuilder.Sql(@"
    CREATE TABLE IF NOT EXISTS ""DashboardWidgets"" (...)
");

// Migration B (created later): Tries to alter the table
// ❌ This ASSUMES the table exists
migrationBuilder.AlterColumn<DateTime>(
    name: "UpdatedAt",
    table: "DashboardWidgets",
    type: "timestamp with time zone");
```

If Migration A was skipped (table already existed), but Migration B runs, it will fail with:
```
42P01: relation "DashboardWidgets" does not exist
```

## Tables Requiring Defensive Checks

The following tables were created with `IF NOT EXISTS` and **MUST** use defensive checks in any future ALTER operations:

### Analytics Tables
Created in migration `20260203150000_AddAnalyticsDashboardTables.cs`:
- ✅ `CustomDashboards`
- ✅ `DashboardWidgets`
- ✅ `WidgetTemplates`
- ✅ `ReportTemplates`

### System Tables
Created in other migrations with defensive patterns:
- ✅ `SystemNotifications`
- ✅ `NotificationRules`
- ✅ `SubscriptionCredits`

### Tables with Conditional Columns
Tables where specific columns were added conditionally:
- ⚠️ `Appointments` - Payment-related columns (see `20260131181400_EnsurePaymentFieldsExist.cs`)
- ⚠️ `Clinics` - `DefaultPaymentReceiverType` column
- ⚠️ `Users` - MFA grace period columns

## The Defensive Migration Pattern

### Basic Pattern for Table Alterations

When altering a table that may not exist, always check for existence first:

```csharp
migrationBuilder.Sql(@"
    DO $$
    BEGIN
        -- Check if table exists
        IF EXISTS (
            SELECT 1 FROM information_schema.tables 
            WHERE table_name = 'DashboardWidgets' 
            AND table_schema = 'public'
        ) THEN
            -- Your ALTER statement here
            ALTER TABLE ""DashboardWidgets"" 
            ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
        END IF;
    END $$;
");
```

### Pattern for Column Alterations

When altering a specific column, check both table and column existence:

```csharp
migrationBuilder.Sql(@"
    DO $$
    BEGIN
        -- Check if both table and column exist
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
            ALTER TABLE ""DashboardWidgets"" 
            ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
        END IF;
    END $$;
");
```

### Pattern for Adding Indexes

When adding indexes to potentially missing tables:

```csharp
migrationBuilder.Sql(@"
    DO $$
    BEGIN
        -- Check if table exists and index doesn't
        IF EXISTS (
            SELECT 1 FROM information_schema.tables 
            WHERE table_name = 'DashboardWidgets' 
            AND table_schema = 'public'
        ) AND NOT EXISTS (
            SELECT 1 FROM pg_indexes 
            WHERE tablename = 'DashboardWidgets'
            AND indexname = 'IX_DashboardWidgets_NewColumn'
            AND schemaname = 'public'
        ) THEN
            CREATE INDEX ""IX_DashboardWidgets_NewColumn"" 
            ON ""DashboardWidgets"" (""NewColumn"");
        END IF;
    END $$;
");
```

### Pattern for Adding Foreign Keys

When adding foreign key constraints to potentially missing tables:

```csharp
migrationBuilder.Sql(@"
    DO $$
    BEGIN
        -- Check if both tables exist and constraint doesn't
        IF EXISTS (
            SELECT 1 FROM information_schema.tables 
            WHERE table_name = 'DashboardWidgets' 
            AND table_schema = 'public'
        ) AND EXISTS (
            SELECT 1 FROM information_schema.tables 
            WHERE table_name = 'CustomDashboards' 
            AND table_schema = 'public'
        ) AND NOT EXISTS (
            SELECT 1 FROM information_schema.table_constraints 
            WHERE constraint_name = 'FK_DashboardWidgets_CustomDashboards_DashboardId'
            AND table_schema = 'public'
        ) THEN
            ALTER TABLE ""DashboardWidgets"" 
            ADD CONSTRAINT ""FK_DashboardWidgets_CustomDashboards_DashboardId"" 
            FOREIGN KEY (""DashboardId"") 
            REFERENCES ""CustomDashboards"" (""Id"") 
            ON DELETE CASCADE;
        END IF;
    END $$;
");
```

## Decision Tree

Use this decision tree to determine if you need defensive checks:

```
Is the table in the "Tables Requiring Defensive Checks" list?
│
├─ YES → Use defensive pattern
│         └─ Are you altering an existing column?
│            ├─ YES → Check table AND column existence
│            └─ NO → Check table existence only
│
└─ NO → Are you creating a new table with IF NOT EXISTS?
          ├─ YES → Document it! Add it to the list above
          │        All future alterations will need defensive checks
          └─ NO → Regular migration pattern is safe
```

## Code Examples

### ❌ INCORRECT - Will Fail if Table Doesn't Exist

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // BAD: Assumes table exists
    migrationBuilder.AlterColumn<DateTime>(
        name: "UpdatedAt",
        table: "DashboardWidgets",
        type: "timestamp with time zone",
        nullable: true);
        
    // BAD: Assumes table and column exist
    migrationBuilder.RenameColumn(
        name: "UpdatedAt",
        table: "DashboardWidgets",
        newName: "LastModifiedAt");
        
    // BAD: Assumes table exists
    migrationBuilder.AddColumn<string>(
        name: "Notes",
        table: "DashboardWidgets",
        type: "text",
        nullable: true);
}
```

### ✅ CORRECT - Checks Existence Before Altering

```csharp
protected override void Up(MigrationBuilder migrationBuilder)
{
    // GOOD: Checks existence before altering
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
                ALTER TABLE ""DashboardWidgets"" 
                ALTER COLUMN ""UpdatedAt"" TYPE timestamp with time zone;
            END IF;
        END $$;
    ");
    
    // GOOD: Checks existence before renaming
    migrationBuilder.Sql(@"
        DO $$
        BEGIN
            IF EXISTS (
                SELECT 1 FROM information_schema.columns 
                WHERE table_name = 'DashboardWidgets' 
                AND column_name = 'UpdatedAt'
                AND table_schema = 'public'
            ) THEN
                ALTER TABLE ""DashboardWidgets"" 
                RENAME COLUMN ""UpdatedAt"" TO ""LastModifiedAt"";
            END IF;
        END $$;
    ");
    
    // GOOD: Checks existence before adding column
    migrationBuilder.Sql(@"
        DO $$
        BEGIN
            IF EXISTS (
                SELECT 1 FROM information_schema.tables 
                WHERE table_name = 'DashboardWidgets' 
                AND table_schema = 'public'
            ) AND NOT EXISTS (
                SELECT 1 FROM information_schema.columns 
                WHERE table_name = 'DashboardWidgets' 
                AND column_name = 'Notes'
                AND table_schema = 'public'
            ) THEN
                ALTER TABLE ""DashboardWidgets"" 
                ADD COLUMN ""Notes"" TEXT NULL;
            END IF;
        END $$;
    ");
}
```

## Testing Checklist

Before committing a new migration, verify:

### Pre-Migration Checks
- [ ] Does your migration alter any table from the "Tables Requiring Defensive Checks" list?
- [ ] If yes, are you using the defensive pattern?
- [ ] Have you tested on a fresh database?
- [ ] Have you tested on a database where the target table doesn't exist?

### Test Scenarios

#### Scenario 1: Fresh Database
```bash
# Create a fresh database
createdb medicsoft_test

# Run all migrations
dotnet ef database update

# Verify: Should succeed without errors
```

#### Scenario 2: Table Doesn't Exist
```bash
# Create database with incomplete migration history
createdb medicsoft_test2

# Manually apply migrations EXCEPT the one that creates the table
# Run your new migration
dotnet ef database update

# Verify: Should succeed without "relation does not exist" error
```

#### Scenario 3: Table Already Exists
```bash
# Create database with complete migration history
createdb medicsoft_test3

# Run all migrations including table creation
dotnet ef database update

# Run your new migration
dotnet ef migrations add TestMigration
dotnet ef database update

# Verify: Should succeed and make the intended changes
```

### Automated Tests

Consider adding integration tests that verify migrations work in all scenarios:

```csharp
[Fact]
public async Task Migration_ShouldSucceed_WhenTableDoesNotExist()
{
    // Arrange: Database without target table
    // Act: Run migration
    // Assert: Migration succeeds
}

[Fact]
public async Task Migration_ShouldSucceed_WhenTableExists()
{
    // Arrange: Database with target table
    // Act: Run migration
    // Assert: Migration succeeds and applies changes
}
```

## Common Pitfalls

### Pitfall 1: Using EF Core Methods Instead of Raw SQL

```csharp
// ❌ WRONG: EF methods don't support conditional checks
migrationBuilder.AlterColumn<DateTime>(
    name: "UpdatedAt",
    table: "DashboardWidgets",
    type: "timestamp with time zone");

// ✅ RIGHT: Use raw SQL with conditional checks
migrationBuilder.Sql(@"DO $$ BEGIN ... END $$;");
```

### Pitfall 2: Case Sensitivity Issues

```csharp
// ⚠️ PROBLEMATIC: Case mismatch
WHERE table_name = 'dashboardwidgets'  // lowercase

// ✅ CORRECT: Exact case match
WHERE table_name = 'DashboardWidgets'  // matches CREATE TABLE name
```

### Pitfall 3: Forgetting Schema Check

```csharp
// ⚠️ INCOMPLETE: Doesn't check schema
WHERE table_name = 'DashboardWidgets'

// ✅ COMPLETE: Checks both table and schema
WHERE table_name = 'DashboardWidgets' 
AND table_schema = 'public'
```

### Pitfall 4: Not Documenting New Conditional Tables

```csharp
// If you create a new table with IF NOT EXISTS:
migrationBuilder.Sql(@"CREATE TABLE IF NOT EXISTS ""NewTable"" ...");

// ⚠️ Don't forget to:
// 1. Add a comment in the migration file
// 2. Update the list in MIGRATION_BEST_PRACTICES.md
// 3. Inform other developers via PR description
```

### Pitfall 5: Incomplete Rollback Strategy

```csharp
protected override void Down(MigrationBuilder migrationBuilder)
{
    // ❌ WRONG: Empty or incomplete rollback
    // This makes debugging harder
    
    // ✅ RIGHT: Implement proper rollback even for defensive migrations
    migrationBuilder.Sql(@"
        DO $$
        BEGIN
            IF EXISTS (
                SELECT 1 FROM information_schema.columns 
                WHERE table_name = 'DashboardWidgets' 
                AND column_name = 'NewColumn'
            ) THEN
                ALTER TABLE ""DashboardWidgets"" 
                DROP COLUMN ""NewColumn"";
            END IF;
        END $$;
    ");
}
```

## Real-World Examples

### Example 1: EnsurePaymentFieldsExist Migration

See `src/MedicSoft.Repository/Migrations/PostgreSQL/20260131181400_EnsurePaymentFieldsExist.cs` for a complete example of defensive migration pattern.

This migration:
- ✅ Checks column existence before adding
- ✅ Checks index existence before creating
- ✅ Checks constraint existence before adding
- ✅ Uses consistent pattern throughout
- ✅ Includes helpful comments

### Example 2: RenameGlobalDocumentTemplatesTable Migration

See `src/MedicSoft.Repository/Migrations/PostgreSQL/20260208003833_RenameGlobalDocumentTemplatesTable.cs` for defensive constraint handling.

This migration:
- ✅ Checks constraint existence before dropping
- ✅ Checks table existence before altering
- ✅ Handles system tables with defensive patterns

### Example 3: AddAnalyticsDashboardTables Migration

See `src/MedicSoft.Repository/Migrations/PostgreSQL/20260203150000_AddAnalyticsDashboardTables.cs` for table creation with IF NOT EXISTS.

This migration:
- ✅ Creates tables conditionally
- ✅ Creates indexes conditionally
- ✅ Adds foreign keys with existence checks
- ⚠️ Requires future migrations to use defensive patterns

## Quick Reference Card

```sql
-- Check if table exists
IF EXISTS (SELECT 1 FROM information_schema.tables 
           WHERE table_name = 'TableName' AND table_schema = 'public')

-- Check if column exists
IF EXISTS (SELECT 1 FROM information_schema.columns 
           WHERE table_name = 'TableName' 
           AND column_name = 'ColumnName' 
           AND table_schema = 'public')

-- Check if index exists
IF EXISTS (SELECT 1 FROM pg_indexes 
           WHERE tablename = 'TableName' 
           AND indexname = 'IndexName' 
           AND schemaname = 'public')

-- Check if constraint exists
IF EXISTS (SELECT 1 FROM information_schema.table_constraints 
           WHERE constraint_name = 'ConstraintName' 
           AND table_schema = 'public')

-- Check if foreign key exists
IF EXISTS (SELECT 1 FROM pg_constraint c
           INNER JOIN pg_namespace n ON c.connamespace = n.oid
           WHERE c.conname = 'FK_Name' AND n.nspname = 'public')
```

## Additional Resources

- [TROUBLESHOOTING_MIGRATIONS.md](./TROUBLESHOOTING_MIGRATIONS.md) - Common migration issues and solutions
- [src/MedicSoft.Repository/Migrations/README.md](./src/MedicSoft.Repository/Migrations/README.md) - Quick start guide
- [EF Core Migrations Documentation](https://docs.microsoft.com/ef/core/managing-schemas/migrations/)
- [PostgreSQL System Catalogs](https://www.postgresql.org/docs/current/catalogs.html)

## Getting Help

If you encounter issues:
1. Check [TROUBLESHOOTING_MIGRATIONS.md](./TROUBLESHOOTING_MIGRATIONS.md) first
2. Review existing migrations for patterns (especially `20260131181400_EnsurePaymentFieldsExist.cs`)
3. Test in all scenarios from the Testing Checklist
4. Ask in the development team channel with specific error messages

## Summary

**Golden Rule**: If a table was created with `IF NOT EXISTS`, all future ALTER operations on that table MUST check for existence first.

Remember:
- 🔍 Always check the affected tables list
- 🛡️ Use defensive patterns for listed tables
- 📝 Document new conditional table creations
- ✅ Test in multiple scenarios
- 🔄 Implement proper rollback strategies
