-- Patient Portal Views
-- These views provide read-only access to appointments and documents from the main application
-- Note: These are placeholder views. Adjust based on actual MedicWarehouse database schema.

-- View for Patient Appointments
-- Note: This view structure should be adjusted to match the actual Appointments table structure
CREATE OR REPLACE VIEW vw_PatientAppointments AS
SELECT 
    a."Id",
    a."PatientId",
    a."DoctorId",
    COALESCE(u."Name", 'N/A') AS "DoctorName",
    COALESCE(a."Specialty", 'N/A') AS "DoctorSpecialty",
    COALESCE(c."Name", 'N/A') AS "ClinicName",
    a."AppointmentDate",
    a."StartTime",
    a."EndTime",
    a."Status",
    COALESCE(a."AppointmentType", 'Consulta') AS "AppointmentType",
    COALESCE(a."IsTelehealth", false) AS "IsTelehealth",
    a."TelehealthLink",
    a."Notes",
    CASE 
        WHEN a."AppointmentDate" > NOW() + INTERVAL '24 hours' THEN true 
        ELSE false 
    END AS "CanReschedule",
    CASE 
        WHEN a."AppointmentDate" > NOW() + INTERVAL '24 hours' THEN true 
        ELSE false 
    END AS "CanCancel",
    a."CreatedAt"
FROM "Appointments" a
LEFT JOIN "Users" u ON a."DoctorId" = u."Id"
LEFT JOIN "Clinics" c ON a."ClinicId" = c."Id"
WHERE a."IsActive" = true;

-- View for Patient Documents  
-- Note: This view structure should be adjusted to match the actual MedicalRecords table structure
CREATE OR REPLACE VIEW vw_PatientDocuments AS
SELECT 
    mr."Id",
    mr."PatientId",
    COALESCE(mr."Title", 'Documento') AS "Title",
    CASE 
        WHEN mr."RecordType" = 'Prescription' THEN 1
        WHEN mr."RecordType" = 'Exam' THEN 2
        WHEN mr."RecordType" = 'MedicalCertificate' THEN 3
        WHEN mr."RecordType" = 'Referral' THEN 4
        ELSE 0
    END AS "DocumentType",
    mr."Content" AS "Description",
    COALESCE(u."Name", 'N/A') AS "DoctorName",
    mr."RecordDate" AS "IssuedDate",
    mr."FilePath" AS "FileUrl",
    mr."FileName",
    0 AS "FileSizeBytes",
    true AS "IsAvailable",
    mr."CreatedAt"
FROM "MedicalRecords" mr
LEFT JOIN "Users" u ON mr."DoctorId" = u."Id"
WHERE mr."IsActive" = true;

-- IMPORTANT: These views are templates and should be adjusted based on:
-- 1. Actual table names and column names in the MedicWarehouse database
-- 2. Actual foreign key relationships
-- 3. Actual data types and constraints
-- Run AFTER verifying the schema matches the main application

