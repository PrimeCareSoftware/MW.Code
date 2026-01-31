# Migration Instructions: Add DocumentHash Column to Patients Table

## Problem Summary
The application was failing with the error:
```
column "DocumentHash" of relation "Patients" does not exist
```

This occurred because the `Patient` entity model included a `DocumentHash` property, but the database schema was missing this column.

## Solution
Created migration `20260131025933_AddDocumentHashToPatients` that:
1. Adds the `DocumentHash` column (nullable, varchar(100)) to the `Patients` table
2. Creates an index `IX_Patients_DocumentHash` for efficient lookups
3. Cleans up duplicate/incorrect indexes and foreign keys detected in the schema

## How to Apply the Migration

### Option 1: Using the Migration Script (Recommended)
```bash
# Navigate to the repository root
cd /path/to/MW.Code

# Run the migration script with your connection string
./run-all-migrations.sh "Host=localhost;Database=primecare;Username=postgres;Password=YourPassword"

# Or use environment variable
export DATABASE_CONNECTION_STRING="Host=localhost;Database=primecare;Username=postgres;Password=YourPassword"
./run-all-migrations.sh
```

### Option 2: Using dotnet ef directly
```bash
# Navigate to the API project
cd src/MedicSoft.Api

# Apply the migration
dotnet ef database update --context MedicSoftDbContext --connection "Host=localhost;Database=primecare;Username=postgres;Password=YourPassword"
```

### Option 3: Using the run-all-migrations script from repository root
```bash
# This will apply all pending migrations
./run-all-migrations.sh
```

## Verification

After applying the migration, verify it worked:

### 1. Check the column exists
```sql
SELECT column_name, data_type, character_maximum_length, is_nullable
FROM information_schema.columns
WHERE table_name = 'Patients' AND column_name = 'DocumentHash';
```

Expected result:
- column_name: DocumentHash
- data_type: character varying
- character_maximum_length: 100
- is_nullable: YES

### 2. Check the index exists
```sql
SELECT indexname, indexdef
FROM pg_indexes
WHERE tablename = 'Patients' AND indexname = 'IX_Patients_DocumentHash';
```

### 3. Test seed-demo
```bash
# The seed-demo command should now work without errors
POST /api/DataSeeder/seed-demo
```

## What DocumentHash Is Used For

The `DocumentHash` field is a hash of the encrypted `Document` field (CPF/RG/Passport). It enables:
- Fast lookups without decrypting the document
- Searchable encryption for LGPD compliance
- Efficient indexing for patient searches by document number

## Rollback (if needed)

If you need to rollback this migration:

```bash
# Navigate to the API project
cd src/MedicSoft.Api

# Rollback to the previous migration
dotnet ef database update 20260130000000_AddMfaGracePeriodToUsers --context MedicSoftDbContext
```

## Additional Changes in This Migration

Besides adding the DocumentHash column, this migration also:
- Cleans up duplicate foreign key constraints in CRM tables
- Removes duplicate indexes in AuditLogs table
- Fixes timestamp data types for consistency (UTC vs non-UTC)
- Recreates proper indexes with correct names

These are all positive changes that improve the database schema consistency.

## Migration Files
- Designer: `src/MedicSoft.Repository/Migrations/PostgreSQL/20260131025933_AddDocumentHashToPatients.Designer.cs`
- Migration: `src/MedicSoft.Repository/Migrations/PostgreSQL/20260131025933_AddDocumentHashToPatients.cs`
- Model Snapshot: Updated in `MedicSoftDbContextModelSnapshot.cs`

## Related Code
- Entity: `src/MedicSoft.Domain/Entities/Patient.cs` (line 19)
- Configuration: `src/MedicSoft.Repository/Configurations/PatientConfiguration.cs` (lines 26-27, 117-118)
