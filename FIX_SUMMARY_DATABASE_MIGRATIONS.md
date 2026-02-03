# Fix Summary: Database Migration Issues

## Issue Description

The Omni Care Software application was experiencing runtime errors when CRM jobs attempted to access database tables that didn't exist:

```
Npgsql.PostgresException (0x80004005): 42P01: relation "crm.SentimentAnalyses" does not exist
Npgsql.PostgresException (0x80004005): 42P01: relation "crm.Complaints" does not exist  
Npgsql.PostgresException (0x80004005): 42P01: relation "crm.MarketingAutomations" does not exist
```

These errors occurred in:
- `SentimentAnalysisJob.GenerateNegativeSentimentAlertsAsync()` (line 230)
- `SentimentAnalysisJob.AnalyzeComplaintsAsync()` (line 116)
- `AutomationExecutorJob.UpdateMetricsAsync()` (line 119)

## Root Cause

The Entity Framework Core migrations that create these CRM tables exist in the codebase (migration `20260127205215_AddCRMEntities` from 27/01/2026), but they had not been applied to the database.

The application did have code to apply migrations on startup in `Program.cs`, but when migrations failed:
1. It only logged the error
2. It continued to start the application
3. The CRM background jobs were scheduled and executed
4. These jobs failed because the database tables didn't exist

## Solution Implemented

### 1. Enhanced Migration Failure Handling

**File**: `src/MedicSoft.Api/Program.cs`

**Changes**:
- Added `throw;` statement after logging migration errors to halt application startup
- Added detailed console output to guide users:
  - Check if connection string is correct
  - Verify PostgreSQL is running
  - Confirm user has proper permissions
  - Provide command to manually apply migrations

**Before**:
```csharp
catch (Exception ex)
{
    Log.Fatal(ex, "Falha ao aplicar migrações do banco de dados: {Message}", ex.Message);
    Console.WriteLine($"Database migration failed: {ex.Message}");
    // Application continues to start!
}
```

**After**:
```csharp
catch (Exception ex)
{
    Log.Fatal(ex, "Falha ao aplicar migrações do banco de dados: {Message}", ex.Message);
    Console.WriteLine($"Database migration failed: {ex.Message}");
    Console.WriteLine("A aplicação não pode iniciar sem as migrações do banco de dados.");
    Console.WriteLine("Por favor, verifique:");
    Console.WriteLine("1. A string de conexão está correta?");
    Console.WriteLine("2. O banco de dados PostgreSQL está rodando?");
    Console.WriteLine("3. O usuário tem permissões para criar schemas e tabelas?");
    Console.WriteLine("4. Execute: dotnet ef database update --project src/MedicSoft.Api");
    throw; // Halt application startup if migrations fail
}
```

### 2. Comprehensive Migration Guide

**File**: `MIGRATIONS_GUIDE.md` (new file)

**Contents**:
- Quick start instructions for applying migrations
- Detailed explanation of the "relation does not exist" error
- Step-by-step solution guide
- Troubleshooting common issues:
  - PostgreSQL not running
  - Permission errors
  - Connection timeouts
  - Schema creation issues
- Entity Framework Core commands reference
- Configuration examples for development and production
- Explanation of automatic migration application
- Best practices for production deployments

The guide is already referenced in the main `README.md` file.

## Benefits

### 1. Fail Fast
The application now fails immediately on startup if migrations cannot be applied, preventing the application from running in a broken state.

### 2. Clear Error Messages
Users receive detailed, actionable error messages that guide them to the solution.

### 3. Better Documentation
Comprehensive guide helps both developers and DevOps teams understand and resolve migration issues.

### 4. Production Safety
In production environments, the application won't start if the database schema is incorrect, preventing data corruption or cascading failures.

## Verification

### Build Status
✅ Project builds successfully with no errors (335 warnings are pre-existing)

### Code Review
✅ Passed automated code review with no issues

### Security Scan
✅ No security vulnerabilities found

### Test Coverage
✅ Existing CRM integration tests continue to work (they use in-memory database)

## How to Apply the Fix

### For End Users

1. **Ensure PostgreSQL is running**:
   ```bash
   podman-compose up postgres -d
   # or
   docker-compose up postgres -d
   ```

2. **Apply all migrations**:
   ```bash
   ./run-all-migrations.sh
   ```

3. **Start the application**:
   ```bash
   cd src/MedicSoft.Api
   dotnet run
   ```

If migrations succeed, you'll see:
```
[INF] Aplicando migrações do banco de dados...
[INF] Migrações do banco de dados aplicadas com sucesso
```

If migrations fail, the application will halt with detailed error messages.

### For Developers

The fix is already in the codebase. When you pull the latest changes:

1. The application will automatically apply migrations on startup
2. If there are issues, you'll get clear error messages
3. Refer to `MIGRATIONS_GUIDE.md` for detailed troubleshooting

## Migration Details

The CRM tables are created by migration: `20260127205215_AddCRMEntities`

This migration:
- Creates the `crm` schema in PostgreSQL
- Creates tables: `SentimentAnalyses`, `Complaints`, `MarketingAutomations`, `AutomationActions`, and others
- Sets up proper foreign keys and indexes
- Configures JSONB columns for flexible data storage

## Related Files

- `src/MedicSoft.Api/Program.cs` - Application startup and migration logic
- `MIGRATIONS_GUIDE.md` - Comprehensive migration documentation
- `run-all-migrations.sh` - Script to apply all migrations
- `src/MedicSoft.Repository/Migrations/PostgreSQL/20260127205215_AddCRMEntities.cs` - CRM tables migration
- `src/MedicSoft.Repository/Configurations/CRM/` - Entity configurations for CRM tables

## Future Considerations

### Recommended Improvements

1. **Migration Smoke Tests**: Add automated tests that verify critical tables exist after migrations
2. **Health Check Endpoint**: Create an API endpoint that checks database schema status
3. **Migration Rollback Strategy**: Document rollback procedures for production
4. **Schema Version Tracking**: Add monitoring for schema version in production

### Production Deployment Checklist

Before deploying to production:

1. ✅ Backup the database
2. ✅ Test migrations in staging environment
3. ✅ Verify migration scripts in version control
4. ✅ Plan rollback strategy
5. ✅ Schedule maintenance window if needed
6. ✅ Monitor application logs during deployment
7. ✅ Verify CRM functionality after deployment

## Contact

For issues or questions:
- Check `MIGRATIONS_GUIDE.md` for detailed troubleshooting
- Review application logs in `logs/` directory
- Open an issue on GitHub with error logs

---

**Fixed**: 2026-01-31  
**Author**: GitHub Copilot  
**Reviewed**: ✅  
**Security Scan**: ✅ No vulnerabilities
