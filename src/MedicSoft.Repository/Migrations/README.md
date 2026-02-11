# MedicSoft Migrations - Quick Reference Guide

## Overview

This directory contains Entity Framework Core migrations for the MedicSoft PostgreSQL database. This README provides quick access to migration best practices and troubleshooting resources.

## Table of Contents

1. [Quick Start](#quick-start)
2. [Creating New Migrations](#creating-new-migrations)
3. [Migration Checklist](#migration-checklist)
4. [Common Issues](#common-issues)
5. [Important Resources](#important-resources)

## Quick Start

### Applying Migrations

```bash
# From the repository root
cd src/MedicSoft.Api

# Apply all pending migrations
dotnet ef database update

# Apply to a specific migration
dotnet ef database update [MigrationName]

# List all migrations
dotnet ef migrations list
```

### Connection String

Default development connection string:
```
Host=localhost;Database=medicsoft;Username=postgres;Password=postgres;Port=5432
```

## Creating New Migrations

### Standard Migration

```bash
# From repository root
cd src/MedicSoft.Api

# Create a new migration
dotnet ef migrations add YourMigrationName \
  --context MedicSoftDbContext \
  --output-dir ../MedicSoft.Repository/Migrations/PostgreSQL

# Review generated files before committing
# Apply to test database
dotnet ef database update
```

### ⚠️ IMPORTANT: Tables Requiring Defensive Checks

The following tables were created with `CREATE TABLE IF NOT EXISTS` and **require defensive checks** in ALL future ALTER operations:

#### Analytics Dashboard Tables
- `CustomDashboards`
- `DashboardWidgets`
- `WidgetTemplates`
- `ReportTemplates`

#### System Configuration Tables
- `SystemNotifications`
- `NotificationRules`
- `SubscriptionCredits`

#### Tables with Conditional Columns
- `Appointments` (payment-related columns)
- `Clinics` (DefaultPaymentReceiverType column)
- `Users` (MFA grace period columns)

### Defensive Migration Pattern

When altering any table from the list above, use this pattern:

```csharp
// ❌ WRONG - Will fail if table doesn't exist
migrationBuilder.AlterColumn<string>(
    name: "SomeColumn",
    table: "DashboardWidgets",
    type: "text");

// ✅ CORRECT - Always check existence first
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
            AND column_name = 'SomeColumn'
            AND table_schema = 'public'
        ) THEN
            ALTER TABLE ""DashboardWidgets"" 
            ALTER COLUMN ""SomeColumn"" TYPE text;
        END IF;
    END $$;
");
```

See [MIGRATION_BEST_PRACTICES.md](../../../MIGRATION_BEST_PRACTICES.md) for complete patterns and examples.

## Migration Checklist

Before committing a new migration, verify:

### Pre-Commit Checks
- [ ] Does your migration alter any table from the "Tables Requiring Defensive Checks" list?
- [ ] If yes, are you using the defensive pattern?
- [ ] Have you tested on a fresh database?
- [ ] Have you reviewed the generated SQL?
- [ ] Have you implemented a proper Down() method?

### Testing Scenarios

#### Test 1: Fresh Database
```bash
# Create test database
createdb medicsoft_test

# Apply all migrations
dotnet ef database update --connection "Host=localhost;Database=medicsoft_test;..."

# Verify success
dropdb medicsoft_test
```

#### Test 2: Existing Database
```bash
# Test on database with existing data
dotnet ef database update --connection "Host=localhost;Database=medicsoft_dev;..."
```

#### Test 3: Rollback
```bash
# Test rollback works
dotnet ef database update [PreviousMigrationName]
```

## Common Issues

### "Relation Does Not Exist" Error (42P01)

**Error:**
```
42P01: relation "DashboardWidgets" does not exist
```

**Cause:** Migration tries to ALTER a table that was created with `IF NOT EXISTS` and doesn't exist in the current database.

**Solution:** Use defensive checks. See [TROUBLESHOOTING_MIGRATIONS.md](../../../TROUBLESHOOTING_MIGRATIONS.md) for details.

### Table Already Exists Error (42P07)

**Error:**
```
42P07: relation "TableName" already exists
```

**Solution:** Use `CREATE TABLE IF NOT EXISTS` or check if table exists before creating. Remember to document it if using IF NOT EXISTS!

### Migration Build Errors

**Error:** Duplicate definitions, syntax errors

**Solution:**
```bash
# Remove last migration
dotnet ef migrations remove

# Clean and rebuild
dotnet clean
dotnet build

# Create migration again
dotnet ef migrations add YourMigrationName
```

## Important Resources

### Documentation
- **[MIGRATION_BEST_PRACTICES.md](../../../MIGRATION_BEST_PRACTICES.md)** - Complete guide to defensive migration patterns
- **[TROUBLESHOOTING_MIGRATIONS.md](../../../TROUBLESHOOTING_MIGRATIONS.md)** - Common issues and solutions
- **[system-admin/guias/MIGRATIONS_GUIDE.md](../../../system-admin/guias/MIGRATIONS_GUIDE.md)** - Overview of all migrations in the system

### Example Migrations
- `20260131181400_EnsurePaymentFieldsExist.cs` - Excellent example of defensive pattern
- `20260203150000_AddAnalyticsDashboardTables.cs` - Creates tables with IF NOT EXISTS (requires defensive checks in future)
- `20260208003833_RenameGlobalDocumentTemplatesTable.cs` - Defensive constraint handling

### External Resources
- [EF Core Migrations Documentation](https://docs.microsoft.com/ef/core/managing-schemas/migrations/)
- [PostgreSQL System Catalogs](https://www.postgresql.org/docs/current/catalogs.html)
- [PostgreSQL Error Codes](https://www.postgresql.org/docs/current/errcodes-appendix.html)

## Quick Commands Reference

```bash
# List all migrations and their status
dotnet ef migrations list

# Generate SQL script for review
dotnet ef migrations script

# Generate SQL for specific migration
dotnet ef migrations script [FromMigration] [ToMigration]

# Generate idempotent script (safe to run multiple times)
dotnet ef migrations script --idempotent --output migrations.sql

# Remove last migration (if not applied)
dotnet ef migrations remove

# Rollback to specific migration
dotnet ef database update [MigrationName]

# Drop database (DESTRUCTIVE - dev only)
dotnet ef database drop --force

# Check EF Core tools version
dotnet ef --version
```

## Migration Naming Conventions

Use clear, descriptive names that indicate what the migration does:

✅ **Good Names:**
- `AddPaymentTrackingToAppointments`
- `UpdateDashboardWidgetTimestamps`
- `AddIndexToUserEmail`
- `RemoveDeprecatedColumns`

❌ **Bad Names:**
- `UpdateDatabase`
- `FixStuff`
- `Changes`
- `NewMigration`

## Security Considerations

1. **Never commit connection strings** with real passwords to version control
2. **Always use parameterized queries** in custom SQL (EF does this automatically)
3. **Review generated SQL** before applying to production
4. **Backup before migrations** on production databases
5. **Test rollback** before deploying to production

## Best Practices Summary

### Golden Rules

1. 🔍 **Check the affected tables list** before altering any table
2. 🛡️ **Use defensive patterns** for tables created with IF NOT EXISTS
3. 📝 **Document new conditional tables** immediately
4. ✅ **Test in multiple scenarios** before committing
5. 🔄 **Implement proper rollback** in Down() method
6. 💾 **Backup before applying** to production
7. 👥 **Review migration SQL** in pull requests

### When to Use Defensive Patterns

Use defensive patterns when:
- Altering tables from the "Tables Requiring Defensive Checks" list
- Adding/removing columns to/from conditional tables
- Adding/removing indexes on conditional tables
- Adding/removing foreign keys involving conditional tables
- Renaming conditional tables or their columns

Don't use defensive patterns when:
- Creating new regular tables (not with IF NOT EXISTS)
- Altering tables NOT in the affected list
- Initial table creation (unless using IF NOT EXISTS)

## Getting Help

1. **Check documentation first:**
   - [TROUBLESHOOTING_MIGRATIONS.md](../../../TROUBLESHOOTING_MIGRATIONS.md)
   - [MIGRATION_BEST_PRACTICES.md](../../../MIGRATION_BEST_PRACTICES.md)

2. **Review example migrations:**
   - Look at `20260131181400_EnsurePaymentFieldsExist.cs`
   - Study the defensive patterns used

3. **Ask the team:**
   - Include error messages (full text)
   - Show what you tried
   - Provide database state information

## Deployment Workflow

### Development
```bash
# 1. Pull latest changes
git pull origin main

# 2. Create migration
dotnet ef migrations add YourMigration

# 3. Test on fresh database
createdb test_db
dotnet ef database update --connection "Host=localhost;Database=test_db;..."

# 4. Test on existing database
dotnet ef database update --connection "Host=localhost;Database=medicsoft_dev;..."

# 5. Commit
git add .
git commit -m "Add migration: YourMigration"
```

### Production
```bash
# 1. Backup database
pg_dump -U postgres medicsoft > backup_$(date +%Y%m%d_%H%M%S).sql

# 2. Stop application (if necessary)
systemctl stop medicsoft

# 3. Apply migrations
dotnet ef database update --connection "$PROD_CONNECTION_STRING"

# 4. Verify success
dotnet ef migrations list --connection "$PROD_CONNECTION_STRING"

# 5. Start application
systemctl start medicsoft

# 6. Monitor logs
journalctl -u medicsoft -f
```

## Summary

- ⚠️ Always check if tables are in the "defensive checks required" list
- 🛡️ Use defensive patterns when altering conditional tables
- 📚 Refer to MIGRATION_BEST_PRACTICES.md for complete guidance
- 🧪 Test migrations thoroughly before committing
- 💾 Always backup production databases before migrations
- 📝 Document your migrations clearly

---

**For detailed patterns and examples, see [MIGRATION_BEST_PRACTICES.md](../../../MIGRATION_BEST_PRACTICES.md)**

**For troubleshooting help, see [TROUBLESHOOTING_MIGRATIONS.md](../../../TROUBLESHOOTING_MIGRATIONS.md)**

**Last Updated:** 2026-02-11  
**Maintained By:** MedicSoft Development Team
