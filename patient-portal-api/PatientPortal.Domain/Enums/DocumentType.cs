namespace PatientPortal.Domain.Enums;

/// <summary>
/// Document type enum
/// </summary>
public enum DocumentType
{
    Prescription = 1,
    MedicalCertificate = 2,
    LabResult = 3,
    ImagingResult = 4,
    MedicalReport = 5,
    Referral = 6,
    VaccinationRecord = 7,
    Other = 99
}
