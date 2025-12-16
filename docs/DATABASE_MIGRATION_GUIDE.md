# Database Migration Guide - Appointment Calendar Features

This guide explains how to apply the database changes required for the new appointment calendar and notification features.

## Overview

The following changes need to be applied to the database:

1. Add new columns to `Appointments` table
2. Create new `Notifications` table

## Option 1: Using Entity Framework Core Migrations (Recommended)

### Prerequisites
- .NET SDK installed
- Access to the appointments microservice project
- Database connection string configured

### Steps

1. Navigate to the appointments microservice directory:
```bash
cd microservices/appointments/MedicSoft.Appointments.Api
```

2. Create a new migration:
```bash
dotnet ef migrations add AddNotificationsAndAppointmentNames
```

3. Review the generated migration file to ensure it includes:
   - Add `PatientName` column to Appointments
   - Add `ClinicName` column to Appointments
   - Add `DoctorName` column to Appointments
   - Create `Notifications` table with all required columns

4. Apply the migration to the database:
```bash
dotnet ef database update
```

5. Verify the changes were applied successfully:
```bash
dotnet ef migrations list
```

## Option 2: Manual SQL Scripts

If you prefer to apply changes manually or use a different migration tool, use these SQL scripts:

### For PostgreSQL

```sql
-- Add columns to Appointments table
ALTER TABLE "Appointments"
ADD COLUMN "PatientName" VARCHAR(200),
ADD COLUMN "ClinicName" VARCHAR(200),
ADD COLUMN "DoctorName" VARCHAR(200);

-- Create Notifications table
CREATE TABLE "Notifications" (
    "Id" UUID PRIMARY KEY,
    "Type" VARCHAR(100) NOT NULL,
    "Title" VARCHAR(200) NOT NULL,
    "Message" VARCHAR(1000) NOT NULL,
    "DataJson" TEXT,
    "IsRead" BOOLEAN NOT NULL DEFAULT FALSE,
    "TenantId" VARCHAR(100) NOT NULL,
    "UserId" UUID,
    "CreatedAt" TIMESTAMP NOT NULL,
    "ReadAt" TIMESTAMP
);

-- Create indexes for better query performance
CREATE INDEX "IX_Notifications_TenantId" ON "Notifications" ("TenantId");
CREATE INDEX "IX_Notifications_IsRead" ON "Notifications" ("IsRead");
CREATE INDEX "IX_Notifications_CreatedAt" ON "Notifications" ("CreatedAt" DESC);
CREATE INDEX "IX_Notifications_UserId" ON "Notifications" ("UserId") WHERE "UserId" IS NOT NULL;
```

### For SQL Server

```sql
-- Add columns to Appointments table
ALTER TABLE [Appointments]
ADD [PatientName] NVARCHAR(200),
    [ClinicName] NVARCHAR(200),
    [DoctorName] NVARCHAR(200);

-- Create Notifications table
CREATE TABLE [Notifications] (
    [Id] UNIQUEIDENTIFIER PRIMARY KEY,
    [Type] NVARCHAR(100) NOT NULL,
    [Title] NVARCHAR(200) NOT NULL,
    [Message] NVARCHAR(1000) NOT NULL,
    [DataJson] NVARCHAR(MAX),
    [IsRead] BIT NOT NULL DEFAULT 0,
    [TenantId] NVARCHAR(100) NOT NULL,
    [UserId] UNIQUEIDENTIFIER,
    [CreatedAt] DATETIME2 NOT NULL,
    [ReadAt] DATETIME2
);

-- Create indexes for better query performance
CREATE INDEX IX_Notifications_TenantId ON [Notifications] ([TenantId]);
CREATE INDEX IX_Notifications_IsRead ON [Notifications] ([IsRead]);
CREATE INDEX IX_Notifications_CreatedAt ON [Notifications] ([CreatedAt] DESC);
CREATE INDEX IX_Notifications_UserId ON [Notifications] ([UserId]) WHERE [UserId] IS NOT NULL;
```

## Post-Migration Steps

### 1. Update Existing Appointment Records

Since the new columns are nullable, existing appointments will have NULL values for the name fields. You should update these with actual values:

```sql
-- This is a sample query - adjust based on your database structure
-- You'll need to join with your actual Patients and Clinics tables

UPDATE "Appointments" a
SET 
    "PatientName" = p."Name",
    "ClinicName" = c."Name",
    "DoctorName" = CASE WHEN a."DoctorId" IS NOT NULL THEN d."Name" ELSE NULL END
FROM "Patients" p
LEFT JOIN "Clinics" c ON a."ClinicId" = c."Id"
LEFT JOIN "Doctors" d ON a."DoctorId" = d."Id"
WHERE a."PatientId" = p."Id";
```

**Note**: Adjust the above query based on your actual schema. The tables `Patients`, `Clinics`, and `Doctors` may be in different microservices or have different names.

### 2. Verify the Migration

Run these queries to verify the changes:

```sql
-- Check Appointments table structure
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_name = 'Appointments'
AND column_name IN ('PatientName', 'ClinicName', 'DoctorName');

-- Check Notifications table was created
SELECT column_name, data_type, is_nullable
FROM information_schema.columns
WHERE table_name = 'Notifications';

-- Count existing appointments
SELECT COUNT(*) as total_appointments FROM "Appointments";

-- Check if any appointments have the new fields populated
SELECT 
    COUNT(*) as total,
    COUNT("PatientName") as with_patient_name,
    COUNT("ClinicName") as with_clinic_name,
    COUNT("DoctorName") as with_doctor_name
FROM "Appointments";
```

## Rollback Instructions

If you need to rollback the changes:

### Using EF Core Migrations

```bash
# Revert to the previous migration
dotnet ef database update PreviousMigrationName

# Or remove the migration entirely (before applying to production)
dotnet ef migrations remove
```

### Manual Rollback SQL

```sql
-- Drop Notifications table
DROP TABLE "Notifications";

-- Remove columns from Appointments table
ALTER TABLE "Appointments"
DROP COLUMN "PatientName",
DROP COLUMN "ClinicName",
DROP COLUMN "DoctorName";
```

## Troubleshooting

### Migration Fails with "Column already exists"

This means the migration was partially applied. Check the current state of your database and manually complete or rollback the changes.

```sql
-- Check if columns exist
SELECT column_name 
FROM information_schema.columns 
WHERE table_name = 'Appointments' 
AND column_name IN ('PatientName', 'ClinicName', 'DoctorName');

-- Check if Notifications table exists
SELECT table_name 
FROM information_schema.tables 
WHERE table_name = 'Notifications';
```

### Data Loss Concerns

The new columns are nullable, so no existing data will be lost. However, you should:
1. Take a database backup before applying migrations
2. Test migrations in a development environment first
3. Have a rollback plan ready

### Performance Considerations

After migration, monitor query performance. The new indexes on the Notifications table should help, but you may need to adjust based on your specific usage patterns.

## Support

If you encounter issues during migration:
1. Check the error messages in the EF Core migration output
2. Verify database permissions
3. Ensure connection strings are correct
4. Review the database logs for any constraint violations

For additional help, create an issue in the GitHub repository with:
- Database type and version
- Error messages
- Migration output
- Database state (which steps completed successfully)
