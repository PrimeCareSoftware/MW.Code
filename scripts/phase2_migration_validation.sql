-- Phase 2: Data Migration Validation Script
-- This script can be used to validate the migration or to manually migrate data if needed
-- Run this AFTER the EF migration has created the new tables and columns

-- =====================================================
-- VALIDATION QUERIES (Run these BEFORE migration)
-- =====================================================

-- Check current state of Clinics
SELECT 
    COUNT(*) as TotalClinics,
    COUNT(DISTINCT "Document") as UniqueDocuments,
    COUNT(CASE WHEN "Document" IS NULL THEN 1 END) as ClinicsWithoutDocument
FROM "Clinics";

-- Check for potential Subdomain conflicts
SELECT "Subdomain", COUNT(*) as ClinicCount
FROM "Clinics"
WHERE "Subdomain" IS NOT NULL
GROUP BY "Subdomain"
HAVING COUNT(*) > 1;

-- Check Users with Clinic associations
SELECT 
    COUNT(*) as TotalUsers,
    COUNT(CASE WHEN "ClinicId" IS NOT NULL THEN 1 END) as UsersWithClinic
FROM "Users";

-- =====================================================
-- DATA MIGRATION QUERIES (These are included in the EF migration)
-- =====================================================

-- 1. Create Company for each existing Clinic (grouped by Document)
-- INSERT INTO "Companies" (...)
-- SELECT ... FROM "Clinics" GROUP BY "Document", ...

-- 2. Link Clinics to their respective Companies
-- UPDATE "Clinics" c SET "CompanyId" = comp."Id" FROM "Companies" comp ...

-- 3. Create UserClinicLink for each existing User.ClinicId relationship
-- INSERT INTO "UserClinicLinks" (...) SELECT ... FROM "Users" u WHERE u."ClinicId" IS NOT NULL

-- 4. Set CurrentClinicId for existing users
-- UPDATE "Users" SET "CurrentClinicId" = "ClinicId" WHERE "ClinicId" IS NOT NULL

-- =====================================================
-- POST-MIGRATION VALIDATION QUERIES
-- =====================================================

-- Verify Companies were created
SELECT COUNT(*) as TotalCompanies FROM "Companies";

-- Verify all Clinics are linked to Companies
SELECT 
    COUNT(*) as TotalClinics,
    COUNT(CASE WHEN "CompanyId" IS NOT NULL THEN 1 END) as ClinicsLinked,
    COUNT(CASE WHEN "CompanyId" IS NULL THEN 1 END) as ClinicsNotLinked
FROM "Clinics";

-- Verify UserClinicLinks were created
SELECT COUNT(*) as TotalLinks FROM "UserClinicLinks";

-- Verify Users have CurrentClinicId set
SELECT 
    COUNT(*) as TotalUsers,
    COUNT(CASE WHEN "CurrentClinicId" IS NOT NULL THEN 1 END) as UsersWithCurrentClinic,
    COUNT(CASE WHEN "ClinicId" IS NOT NULL AND "CurrentClinicId" IS NULL THEN 1 END) as UsersNeedingCurrentClinic
FROM "Users";

-- Verify data consistency: Users.CurrentClinicId matches Users.ClinicId
SELECT COUNT(*) as MismatchedUsers
FROM "Users"
WHERE "ClinicId" IS NOT NULL 
  AND "CurrentClinicId" IS NOT NULL 
  AND "ClinicId" != "CurrentClinicId";

-- Verify data consistency: UserClinicLinks match Users.ClinicId
SELECT u."Id" as UserId, u."FullName", u."ClinicId"
FROM "Users" u
LEFT JOIN "UserClinicLinks" ucl ON u."Id" = ucl."UserId" AND u."ClinicId" = ucl."ClinicId"
WHERE u."ClinicId" IS NOT NULL AND ucl."Id" IS NULL;

-- =====================================================
-- DETAILED VALIDATION REPORT
-- =====================================================

-- Companies created from Clinics
SELECT 
    c."Document",
    c."DocumentType",
    c."Name",
    c."TradeName",
    COUNT(DISTINCT cl."Id") as NumberOfClinics
FROM "Companies" c
LEFT JOIN "Clinics" cl ON cl."CompanyId" = c."Id"
GROUP BY c."Id", c."Document", c."DocumentType", c."Name", c."TradeName"
ORDER BY NumberOfClinics DESC, c."Name";

-- UserClinicLinks distribution
SELECT 
    u."FullName",
    u."Email",
    COUNT(ucl."Id") as NumberOfClinics,
    STRING_AGG(c."Name", ', ') as ClinicNames
FROM "Users" u
LEFT JOIN "UserClinicLinks" ucl ON u."Id" = ucl."UserId" AND ucl."IsActive" = true
LEFT JOIN "Clinics" c ON ucl."ClinicId" = c."Id"
GROUP BY u."Id", u."FullName", u."Email"
HAVING COUNT(ucl."Id") > 0
ORDER BY NumberOfClinics DESC, u."FullName";

-- Users with preferred clinic set
SELECT 
    u."FullName",
    u."Email",
    c."Name" as PreferredClinic
FROM "Users" u
INNER JOIN "UserClinicLinks" ucl ON u."Id" = ucl."UserId" AND ucl."IsPreferredClinic" = true
INNER JOIN "Clinics" c ON ucl."ClinicId" = c."Id"
ORDER BY u."FullName";

-- =====================================================
-- CLEANUP QUERIES (Use with caution - for testing only)
-- =====================================================

-- WARNING: These queries will delete data. Only use in test environments!

-- Reset migration (equivalent to running Down() migration)
-- DO NOT run these in production without a backup!

/*
-- Remove foreign keys
ALTER TABLE "Clinics" DROP CONSTRAINT IF EXISTS "FK_Clinics_Companies_CompanyId";
ALTER TABLE "Users" DROP CONSTRAINT IF EXISTS "FK_Users_Clinics_CurrentClinicId";

-- Drop tables
DROP TABLE IF EXISTS "UserClinicLinks";
DROP TABLE IF EXISTS "Companies";

-- Remove columns
ALTER TABLE "Clinics" DROP COLUMN IF EXISTS "CompanyId";
ALTER TABLE "Users" DROP COLUMN IF EXISTS "CurrentClinicId";
*/
