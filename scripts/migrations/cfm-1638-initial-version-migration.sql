-- CFM 1.638/2002 - Data Migration Script
-- This script creates version 1 for all existing medical records
-- Run this after applying the AddCfm1638VersioningAndAudit migration

-- Step 1: Update existing MedicalRecords to set CurrentVersion = 1 if it's NULL or 0
UPDATE "MedicalRecords"
SET "CurrentVersion" = 1
WHERE "CurrentVersion" IS NULL OR "CurrentVersion" = 0;

-- Step 2: Create initial version snapshots for existing medical records
-- Note: This uses a system user ID (00000000-0000-0000-0000-000000000001) for the initial migration
-- You may need to adjust this to a valid user ID in your system

WITH existing_records AS (
    SELECT 
        mr."Id" as medical_record_id,
        mr."TenantId",
        mr."CreatedAt"
    FROM "MedicalRecords" mr
    LEFT JOIN "MedicalRecordVersions" mrv ON mr."Id" = mrv."MedicalRecordId"
    WHERE mrv."Id" IS NULL  -- Only records without any versions yet
)
INSERT INTO "MedicalRecordVersions" (
    "Id",
    "MedicalRecordId",
    "Version",
    "ChangeType",
    "ChangedAt",
    "ChangedByUserId",
    "ChangeReason",
    "SnapshotJson",
    "ChangesSummary",
    "ContentHash",
    "PreviousVersionHash",
    "TenantId",
    "CreatedAt",
    "UpdatedAt"
)
SELECT 
    gen_random_uuid() as "Id",
    er.medical_record_id as "MedicalRecordId",
    1 as "Version",
    'Created' as "ChangeType",
    er."CreatedAt" as "ChangedAt",
    '00000000-0000-0000-0000-000000000001'::uuid as "ChangedByUserId",  -- System user
    'Initial version created by data migration' as "ChangeReason",
    '{}' as "SnapshotJson",  -- Empty JSON for initial migration
    'Initial version from existing record' as "ChangesSummary",
    encode(sha256('initial'::bytea), 'base64') as "ContentHash",  -- Placeholder hash
    NULL as "PreviousVersionHash",
    er."TenantId",
    er."CreatedAt" as "CreatedAt",
    er."CreatedAt" as "UpdatedAt"
FROM existing_records er;

-- Step 3: Verification - Check that all medical records now have at least one version
SELECT 
    COUNT(DISTINCT mr."Id") as total_medical_records,
    COUNT(DISTINCT mrv."MedicalRecordId") as records_with_versions,
    CASE 
        WHEN COUNT(DISTINCT mr."Id") = COUNT(DISTINCT mrv."MedicalRecordId") 
        THEN 'SUCCESS: All records have versions'
        ELSE 'WARNING: Some records are missing versions'
    END as migration_status
FROM "MedicalRecords" mr
LEFT JOIN "MedicalRecordVersions" mrv ON mr."Id" = mrv."MedicalRecordId";

-- Optional: Create audit logs for the migration
-- This creates an access log entry for the migration process
-- Uncomment if you want to track this migration in the audit logs
/*
INSERT INTO "MedicalRecordAccessLogs" (
    "Id",
    "MedicalRecordId",
    "UserId",
    "AccessType",
    "AccessedAt",
    "IpAddress",
    "UserAgent",
    "Details",
    "TenantId",
    "CreatedAt",
    "UpdatedAt"
)
SELECT 
    gen_random_uuid() as "Id",
    mr."Id" as "MedicalRecordId",
    '00000000-0000-0000-0000-000000000001'::uuid as "UserId",  -- System user
    'Migration' as "AccessType",
    NOW() as "AccessedAt",
    'system-migration' as "IpAddress",
    'CFM-1638-Data-Migration-Script' as "UserAgent",
    'Initial version created during CFM 1.638 migration' as "Details",
    mr."TenantId",
    NOW() as "CreatedAt",
    NOW() as "UpdatedAt"
FROM "MedicalRecords" mr;
*/

-- Migration complete!
-- All existing medical records should now have an initial version (version 1)
