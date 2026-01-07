-- Patient Portal Views
-- These views provide read-only access to appointments and documents from the main application

-- View for Patient Appointments
CREATE OR REPLACE VIEW vw_PatientAppointments AS
SELECT 
    a.Id,
    a.PatientId,
    a.DoctorId,
    u.Name AS DoctorName,
    p.Name AS DoctorSpecialty,
    c.Name AS ClinicName,
    a.AppointmentDate,
    a.StartTime,
    a.EndTime,
    a.Status AS Status,
    a.AppointmentType,
    a.IsTelehealth,
    a.TelehealthLink,
    a.Notes,
    CASE 
        WHEN a.AppointmentDate > NOW() + INTERVAL '24 hours' THEN true 
        ELSE false 
    END AS CanReschedule,
    CASE 
        WHEN a.AppointmentDate > NOW() + INTERVAL '24 hours' THEN true 
        ELSE false 
    END AS CanCancel,
    a.CreatedAt
FROM "Appointments" a
LEFT JOIN "Users" u ON a.DoctorId = u.Id
LEFT JOIN "Procedures" p ON a.Id = p.Id
LEFT JOIN "Clinics" c ON a.ClinicId = c.Id
WHERE a.IsActive = true;

-- View for Patient Documents  
CREATE OR REPLACE VIEW vw_PatientDocuments AS
SELECT 
    mr.Id,
    mr.PatientId,
    mr.Title,
    CASE 
        WHEN mr.RecordType = 'Prescription' THEN 1
        WHEN mr.RecordType = 'Exam' THEN 2
        WHEN mr.RecordType = 'MedicalCertificate' THEN 3
        WHEN mr.RecordType = 'Referral' THEN 4
        ELSE 0
    END AS DocumentType,
    mr.Content AS Description,
    u.Name AS DoctorName,
    mr.RecordDate AS IssuedDate,
    mr.FilePath AS FileUrl,
    mr.FileName,
    0 AS FileSizeBytes,
    true AS IsAvailable,
    mr.CreatedAt
FROM "MedicalRecords" mr
LEFT JOIN "Users" u ON mr.DoctorId = u.Id
WHERE mr.IsActive = true;
