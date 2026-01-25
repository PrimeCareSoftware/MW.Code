# Phase 2: Database Migration Guide

## Overview
This guide documents the database migration created for Phase 2 of the multi-clinic refactoring. The migration transforms the system from a 1:1 model (one owner, one clinic) to a 1:N model (one company/owner can have multiple clinics).

## Migration File
**File:** `src/MedicSoft.Repository/Migrations/PostgreSQL/20260123150022_AddCompanyAndMultiClinicSupport.cs`

## What This Migration Does

### 1. Schema Changes

#### New Tables Created:

**Companies Table:**
- Stores company/enterprise information
- The Company becomes the tenant entity (replaces Clinic as tenant)
- Fields: Id, Name, TradeName, Document, DocumentType, Phone, Email, IsActive, Subdomain, CreatedAt, UpdatedAt, TenantId
- Unique indexes on: Document, Subdomain (when not null)

**UserClinicLinks Table:**
- Implements N:N relationship between Users and Clinics
- Allows users to work at multiple clinics within the same company
- Fields: Id, UserId, ClinicId, LinkedDate, IsActive, IsPreferredClinic, InactivatedDate, InactivationReason, CreatedAt, UpdatedAt, TenantId
- Unique composite index on: (UserId, ClinicId, TenantId)

#### Columns Added:

**Clinics Table:**
- `CompanyId` (Guid, nullable) - Foreign key to Companies table

**Users Table:**
- `CurrentClinicId` (Guid, nullable) - Foreign key to Clinics table, indicates which clinic the user is currently working in

### 2. Data Migration

The migration includes SQL commands to migrate existing data automatically:

#### Step 1: Create Companies from existing Clinics
```sql
INSERT INTO "Companies" (...)
SELECT 
    gen_random_uuid() as "Id",
    "Name", "TradeName", "Document", "DocumentType", ...
FROM "Clinics"
GROUP BY "Document", "DocumentType", ... -- Groups by unique document to avoid duplicates
```

This creates one Company record for each unique Document (CPF/CNPJ) in the Clinics table.

#### Step 2: Link Clinics to Companies
```sql
UPDATE "Clinics" c
SET "CompanyId" = comp."Id"
FROM "Companies" comp
WHERE c."Document" = comp."Document" AND c."TenantId" = comp."TenantId"
```

This establishes the relationship between existing Clinics and their newly created Company records.

#### Step 3: Create UserClinicLink records
```sql
INSERT INTO "UserClinicLinks" (...)
SELECT 
    gen_random_uuid() as "Id",
    u."Id" as "UserId",
    u."ClinicId" as "ClinicId",
    ...
FROM "Users" u
WHERE u."ClinicId" IS NOT NULL
```

This creates UserClinicLink records for all existing User-Clinic relationships, marking them as the preferred clinic.

#### Step 4: Set CurrentClinicId for existing users
```sql
UPDATE "Users"
SET "CurrentClinicId" = "ClinicId"
WHERE "ClinicId" IS NOT NULL
```

This sets the current working clinic for all existing users based on their original ClinicId.

### 3. Indexes Created

The migration creates optimized indexes for:
- Company lookups by Document (unique)
- Company lookups by Subdomain (unique, filtered)
- Company filtering by IsActive
- Company filtering by TenantId
- UserClinicLink lookups by UserId
- UserClinicLink lookups by ClinicId
- UserClinicLink composite unique constraint
- UserClinicLink filtering by UserId and IsActive
- Foreign key relationships

## Backward Compatibility

The migration preserves backward compatibility:
- **User.ClinicId** is NOT removed (marked as deprecated in code but still in database)
- **Clinic.Document** is NOT removed (data is copied to Company, not moved)
- All existing relationships are preserved
- All existing data remains accessible

## Rollback

The migration includes a `Down()` method that:
1. Removes foreign keys
2. Drops UserClinicLinks table
3. Drops Companies table
4. Removes CompanyId from Clinics
5. Removes CurrentClinicId from Users

**Note:** The rollback does NOT restore the original state completely - it only removes the new structures. The original data remains unchanged.

## Testing the Migration

### Before Running:
1. **Backup your database** - Always backup before running migrations in production
2. Verify the current database state
3. Check that all existing Clinics have valid Document values

### To Apply the Migration:

```bash
cd /home/runner/work/MW.Code/MW.Code/src/MedicSoft.Repository
dotnet ef database update --context MedicSoftDbContext
```

### Verification Steps:

1. **Verify Companies were created:**
   ```sql
   SELECT COUNT(*) FROM "Companies";
   -- Should match the number of unique Documents in Clinics
   ```

2. **Verify Clinics are linked to Companies:**
   ```sql
   SELECT COUNT(*) FROM "Clinics" WHERE "CompanyId" IS NOT NULL;
   -- Should match total number of Clinics
   ```

3. **Verify UserClinicLinks were created:**
   ```sql
   SELECT COUNT(*) FROM "UserClinicLinks";
   -- Should match number of Users with ClinicId set
   ```

4. **Verify Users have CurrentClinicId set:**
   ```sql
   SELECT COUNT(*) FROM "Users" WHERE "CurrentClinicId" IS NOT NULL;
   -- Should match number of Users with ClinicId set
   ```

## Known Considerations

1. **Multi-tenant data**: The migration groups Companies by Document AND TenantId, ensuring multi-tenant isolation is preserved.

2. **Duplicate Documents**: If multiple Clinics share the same Document within the same tenant, they will be linked to the same Company (which is the desired behavior).

3. **NULL Documents**: If any Clinic has a NULL Document value, it will not get a Company created. This should be validated before migration.

4. **Subdomain conflicts**: If two Clinics with different Documents have the same Subdomain, the migration will fail due to the unique constraint. This should be validated before migration.

## Post-Migration Steps

After the migration is successful:

1. **Phase 3**: Update backend services (RegistrationService, AuthenticationService, etc.)
2. **Phase 4**: Update API endpoints
3. **Phase 5**: Update frontend - Site (registration forms)
4. **Phase 6**: Update frontend - System (clinic selector, user management)
5. **Phase 7**: Add comprehensive tests

## Support

For issues or questions about this migration:
1. Review the REFACTORING_SUMMARY.md file
2. Check the entity definitions in `src/MedicSoft.Domain/Entities/`
3. Review the EF Core configurations in `src/MedicSoft.Repository/Configurations/`
