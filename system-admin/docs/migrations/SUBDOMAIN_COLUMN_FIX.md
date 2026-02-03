# Subdomain Column Migration

## Issue
The API endpoint `/api/data-seeder/seed-demo` was failing with:
```
42703: column c.Subdomain does not exist
```

## Root Cause
The `Subdomain` property was added to the `Clinic` entity class but no database migration was created to add the column to the PostgreSQL database.

## Solution

### Option 1: EF Core Migration (Recommended for new databases)
An EF Core migration has been created: `20260111180146_AddSubdomainToClinic`

To apply:
```bash
dotnet ef database update --context MedicSoftDbContext
```

### Option 2: SQL Script (Recommended for existing production databases)
For existing databases, use the SQL script which is idempotent and safe to run multiple times:

```bash
psql -U postgres -d primecare < scripts/migrations/001_add_subdomain_to_clinics.sql
```

Or via Docker:
```bash
docker exec -i omnicare-postgres psql -U postgres -d primecare < scripts/migrations/001_add_subdomain_to_clinics.sql
```

## What Was Fixed
- Added `Subdomain` column to `Clinics` table (TEXT, nullable)
- Created idempotent SQL migration script
- Created EF Core migration for greenfield deployments

## Testing
Tested on PostgreSQL 16 with both methods:
- ✅ SQL script successfully adds column
- ✅ SQL script is idempotent (safe to run multiple times)
- ✅ Column allows NULL values as per entity definition
