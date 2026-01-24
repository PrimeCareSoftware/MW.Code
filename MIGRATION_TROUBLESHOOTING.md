# Migration Troubleshooting Guide

## SNGPC Controlled Medication Tables Migration

### Current Status
The SNGPC (Sistema Nacional de Gerenciamento de Produtos Controlados) migration for controlled medication tables is **correctly implemented** in the repository.

**Migration File**: `20260124002922_AddSngpcControlledMedicationTables`
- ✅ Build Status: Successful
- ✅ No duplicate attributes or methods
- ✅ Properly configured entity relationships
- ✅ All indexes and constraints properly defined

### Common Issues and Solutions

#### Issue 1: Duplicate Migration Files
**Symptoms:**
- Build errors mentioning duplicate `DbContext` attributes
- Build errors mentioning duplicate `Migration` attributes
- Build errors about duplicate `Up`, `Down`, or `BuildTargetModel` methods

**Cause:**
Running `dotnet ef migrations add` multiple times with the same or similar migration name.

**Solution:**
```bash
# 1. List all migrations
dotnet ef migrations list --project src/MedicSoft.Repository/MedicSoft.Repository.csproj --startup-project src/MedicSoft.Api/MedicSoft.Api.csproj

# 2. Remove the duplicate migration (use the most recent timestamp if you have duplicates)
dotnet ef migrations remove --project src/MedicSoft.Repository/MedicSoft.Repository.csproj --startup-project src/MedicSoft.Api/MedicSoft.Api.csproj

# 3. If you have manually created files, delete them:
# - src/MedicSoft.Repository/Migrations/PostgreSQL/[TIMESTAMP]_MigrationName.cs
# - src/MedicSoft.Repository/Migrations/PostgreSQL/[TIMESTAMP]_MigrationName.Designer.cs

# 4. Rebuild the solution
dotnet build
```

#### Issue 2: PostgreSQL Syntax Error with "[" Character
**Symptoms:**
- Error: `syntax error at or near "["`
- Error occurs during migration execution (not compilation)
- POSITION: 81 (or another position)

**Cause:**
This typically happens when:
1. A migration file is corrupted or malformed
2. Custom SQL contains invalid syntax
3. Migration was manually edited incorrectly

**Solution:**
```bash
# 1. Verify the migration files are from the repository (not locally modified)
git diff src/MedicSoft.Repository/Migrations/PostgreSQL/

# 2. If you see differences, reset to repository version:
git checkout -- src/MedicSoft.Repository/Migrations/PostgreSQL/20260124002922_AddSngpcControlledMedicationTables.cs
git checkout -- src/MedicSoft.Repository/Migrations/PostgreSQL/20260124002922_AddSngpcControlledMedicationTables.Designer.cs

# 3. Rebuild and try again
dotnet build
```

### Verifying Migration Integrity

To ensure your local migrations match the repository:

```bash
# 1. Check the migration files exist with correct timestamp
ls -la src/MedicSoft.Repository/Migrations/PostgreSQL/ | grep "20260124002922"

# Should show:
# 20260124002922_AddSngpcControlledMedicationTables.cs (~15KB)
# 20260124002922_AddSngpcControlledMedicationTables.Designer.cs (~300KB)
# Note: File sizes are approximate and may vary slightly with updates

# 2. Verify no duplicate files exist
ls -la src/MedicSoft.Repository/Migrations/PostgreSQL/ | grep "AddSngpc"

# Should show only the two files above, not multiple versions

# 3. Verify the build succeeds
dotnet build src/MedicSoft.Repository/MedicSoft.Repository.csproj
```

### Creating New Migrations

To avoid issues when creating new migrations:

```bash
# 1. Always ensure you're on the latest code
git pull

# 2. Verify no pending migrations exist
dotnet ef migrations list --project src/MedicSoft.Repository/MedicSoft.Repository.csproj --startup-project src/MedicSoft.Api/MedicSoft.Api.csproj

# 3. Make your entity/configuration changes

# 4. Add the new migration with a descriptive name
dotnet ef migrations add YourMigrationName --project src/MedicSoft.Repository/MedicSoft.Repository.csproj --startup-project src/MedicSoft.Api/MedicSoft.Api.csproj --context MedicSoftDbContext --output-dir Migrations/PostgreSQL

# 5. Review the generated migration files before committing
```

### SNGPC Entities

The migration creates tables for the following entities:

1. **ControlledMedicationRegistry** (`ControlledMedicationRegistries` table)
   - Tracks all controlled substance movements
   - Required by ANVISA RDC 27/2007

2. **MonthlyControlledBalance** (`MonthlyControlledBalances` table)
   - Monthly balance reconciliation for controlled medications
   - Required for monthly reporting and stock control

3. **SngpcTransmission** (`SngpcTransmissions` table)
   - Tracks transmission attempts to ANVISA
   - Stores protocol numbers and response data

### Database Verification

After applying migrations, verify the tables were created:

```sql
-- Connect to your PostgreSQL database and run:

-- Check if tables exist
SELECT table_name 
FROM information_schema.tables 
WHERE table_schema = 'public' 
  AND table_name IN ('ControlledMedicationRegistries', 'MonthlyControlledBalances', 'SngpcTransmissions');

-- Verify indexes
SELECT indexname, tablename 
FROM pg_indexes 
WHERE schemaname = 'public' 
  AND tablename IN ('ControlledMedicationRegistries', 'MonthlyControlledBalances', 'SngpcTransmissions');
```

### Getting Help

If you continue to experience issues:

1. Ensure you're using .NET 8.0 SDK (check with `dotnet --version`)
2. Verify PostgreSQL version is 12 or higher
3. Check that Entity Framework Core tools are installed: `dotnet tool restore`
4. Review the full error message and stack trace
5. Compare your local migration files with the repository versions

### Related Files

- Entity Definitions:
  - `src/MedicSoft.Domain/Entities/ControlledMedicationRegistry.cs`
  - `src/MedicSoft.Domain/Entities/MonthlyControlledBalance.cs`
  - `src/MedicSoft.Domain/Entities/SngpcTransmission.cs`
  - `src/MedicSoft.Domain/Entities/SNGPCReport.cs` (Note: Uses uppercase SNGPC as per ANVISA naming)

- Entity Configurations:
  - `src/MedicSoft.Repository/Configurations/ControlledMedicationRegistryConfiguration.cs`
  - `src/MedicSoft.Repository/Configurations/MonthlyControlledBalanceConfiguration.cs`
  - `src/MedicSoft.Repository/Configurations/SngpcTransmissionConfiguration.cs`

- DbContext:
  - `src/MedicSoft.Repository/Context/MedicSoftDbContext.cs`
