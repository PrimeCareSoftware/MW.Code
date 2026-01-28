# Database Migration Fix Summary

## Issue Description
The application was experiencing two critical database migration errors:

1. **Error 42P07**: `relation "FilasEspera" already exists`
   - Migration was attempting to CREATE a table that already existed
   - Caused the migration process to fail

2. **Error 42P01**: `relation "crm.SentimentAnalyses" does not exist`
   - Table was never created because previous migration failure blocked all subsequent migrations
   - Caused runtime errors in sentiment analysis jobs

## Root Cause

### Duplicate Migration Files
Migration files were split across two locations with different namespaces:
- **Root folder**: `src/MedicSoft.Repository/Migrations/` with namespace `MedicSoft.Repository.Migrations`
- **PostgreSQL subfolder**: `src/MedicSoft.Repository/Migrations/PostgreSQL/` with namespace `MedicSoft.Repository.Migrations.PostgreSQL`

### Duplicate Table Creation
Migration `20260127145640_AddConsultaDiariaTable.cs` was incorrectly attempting to:
- CREATE TABLE `FilasEspera` (already created by earlier migration `20260127142157_AddRequiredFieldsToConsultationFormConfiguration`)
- CREATE TABLE `SenhasFila` (already created by same earlier migration)

This caused Entity Framework Core to fail when applying migrations, which prevented subsequent migrations from running, including the CRM entities migration that creates `crm.SentimentAnalyses`.

## Solution

### Changes Made
1. **Consolidated Migrations**: Moved 4 files from root folder to PostgreSQL subfolder
   - `20260127145640_AddConsultaDiariaTable.cs` + `.Designer.cs`
   - `20260127182135_AddDigitalSignatureTables.cs` + `.Designer.cs`

2. **Fixed Namespaces**: Updated namespace from `MedicSoft.Repository.Migrations` to `MedicSoft.Repository.Migrations.PostgreSQL`

3. **Removed Duplicates**: Eliminated 156 lines of duplicate code:
   - Removed CREATE TABLE statements for `FilasEspera` and `SenhasFila`
   - Removed CREATE INDEX statements for these tables
   - Removed corresponding DROP TABLE statements from Down() method

### Correct Migration Order
After the fix, migrations now run in the correct order:

1. `20260127142157_AddRequiredFieldsToConsultationFormConfiguration` → Creates FilasEspera
2. `20260127145640_AddConsultaDiariaTable` → Creates only ConsultasDiarias (fixed)
3. `20260127182135_AddDigitalSignatureTables` → Adds digital signature tables
4. `20260127205215_AddCRMEntities` → Creates crm schema and SentimentAnalyses table
5. `20260127211405_AddPatientJourneyTagsAndEngagement` → Adds patient journey tables

## Expected Results

After applying this fix:
- ✅ All migrations run successfully without conflicts
- ✅ FilasEspera table exists with correct schema
- ✅ CRM schema and SentimentAnalyses table are created
- ✅ Sentiment analysis jobs can query the database without errors
- ✅ All migrations are in a single location with consistent namespace

## Testing Recommendations

1. **Fresh Database**: Test against a fresh database to ensure all migrations apply cleanly
2. **Existing Database**: For databases where earlier migrations already ran, Entity Framework will skip already-applied migrations
3. **Verify Tables**: Check that both `FilasEspera` and `crm.SentimentAnalyses` tables exist after migration

## Commands to Test

```bash
# Check migration status
dotnet ef migrations list --project src/MedicSoft.Repository --startup-project src/MedicSoft.Api

# Apply migrations
dotnet ef database update --project src/MedicSoft.Repository --startup-project src/MedicSoft.Api

# Verify tables exist (PostgreSQL)
psql -d your_database -c "\dt FilasEspera"
psql -d your_database -c "\dt crm.SentimentAnalyses"
```

## Files Modified
- `src/MedicSoft.Repository/Migrations/PostgreSQL/20260127145640_AddConsultaDiariaTable.cs`
- `src/MedicSoft.Repository/Migrations/PostgreSQL/20260127145640_AddConsultaDiariaTable.Designer.cs`
- `src/MedicSoft.Repository/Migrations/PostgreSQL/20260127182135_AddDigitalSignatureTables.cs`
- `src/MedicSoft.Repository/Migrations/PostgreSQL/20260127182135_AddDigitalSignatureTables.Designer.cs`

## Security Impact
No security vulnerabilities introduced. Changes are limited to:
- File relocation
- Namespace updates
- Removal of duplicate schema definitions

The fix maintains all existing functionality while eliminating migration conflicts.
