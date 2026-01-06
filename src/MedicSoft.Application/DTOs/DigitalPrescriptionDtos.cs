using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs
{
    // DTO for Digital Prescription
    public class DigitalPrescriptionDto
    {
        public Guid Id { get; set; }
        public Guid MedicalRecordId { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public string Type { get; set; } = null!;
        public string? SequenceNumber { get; set; }
        public DateTime IssuedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsActive { get; set; }
        public string DoctorName { get; set; } = null!;
        public string DoctorCRM { get; set; } = null!;
        public string DoctorCRMState { get; set; } = null!;
        public string PatientName { get; set; } = null!;
        public string PatientDocument { get; set; } = null!;
        public string? DigitalSignature { get; set; }
        public DateTime? SignedAt { get; set; }
        public string? SignatureCertificate { get; set; }
        public string? VerificationCode { get; set; }
        public bool RequiresSNGPCReport { get; set; }
        public DateTime? ReportedToSNGPCAt { get; set; }
        public string? Notes { get; set; }
        public List<DigitalPrescriptionItemDto> Items { get; set; } = new();
        public int DaysUntilExpiration { get; set; }
        public bool IsExpired { get; set; }
        public bool IsValid { get; set; }
    }

    // DTO for Digital Prescription Item
    public class DigitalPrescriptionItemDto
    {
        public Guid Id { get; set; }
        public Guid DigitalPrescriptionId { get; set; }
        public Guid MedicationId { get; set; }
        public string MedicationName { get; set; } = null!;
        public string? GenericName { get; set; }
        public string? ActiveIngredient { get; set; }
        public bool IsControlledSubstance { get; set; }
        public string? ControlledList { get; set; }
        public string? AnvisaRegistration { get; set; }
        public string Dosage { get; set; } = null!;
        public string PharmaceuticalForm { get; set; } = null!;
        public string Frequency { get; set; } = null!;
        public int DurationDays { get; set; }
        public int Quantity { get; set; }
        public string? AdministrationRoute { get; set; }
        public string? Instructions { get; set; }
        public string? BatchNumber { get; set; }
        public DateTime? ManufactureDate { get; set; }
        public DateTime? ExpiryDate { get; set; }
    }

    // DTO for creating a prescription
    public class CreateDigitalPrescriptionDto
    {
        public Guid MedicalRecordId { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public string Type { get; set; } = null!;
        public string DoctorName { get; set; } = null!;
        public string DoctorCRM { get; set; } = null!;
        public string DoctorCRMState { get; set; } = null!;
        public string PatientName { get; set; } = null!;
        public string PatientDocument { get; set; } = null!;
        public string? Notes { get; set; }
        public List<CreateDigitalPrescriptionItemDto> Items { get; set; } = new();
    }

    // DTO for creating prescription item
    public class CreateDigitalPrescriptionItemDto
    {
        public Guid MedicationId { get; set; }
        public string MedicationName { get; set; } = null!;
        public string? GenericName { get; set; }
        public string? ActiveIngredient { get; set; }
        public bool IsControlledSubstance { get; set; }
        public string? ControlledList { get; set; }
        public string? AnvisaRegistration { get; set; }
        public string Dosage { get; set; } = null!;
        public string PharmaceuticalForm { get; set; } = null!;
        public string Frequency { get; set; } = null!;
        public int DurationDays { get; set; }
        public int Quantity { get; set; }
        public string? AdministrationRoute { get; set; }
        public string? Instructions { get; set; }
    }

    // DTO for SNGPC Report
    public class SNGPCReportDto
    {
        public Guid Id { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public DateTime ReportPeriodStart { get; set; }
        public DateTime ReportPeriodEnd { get; set; }
        public string Status { get; set; } = null!;
        public DateTime GeneratedAt { get; set; }
        public DateTime? TransmittedAt { get; set; }
        public string? TransmissionProtocol { get; set; }
        public int TotalPrescriptions { get; set; }
        public int TotalItems { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? LastAttemptAt { get; set; }
        public int AttemptCount { get; set; }
        public int DaysUntilDeadline { get; set; }
        public bool IsOverdue { get; set; }
    }

    // DTO for creating SNGPC Report
    public class CreateSNGPCReportDto
    {
        public int Month { get; set; }
        public int Year { get; set; }
    }
}
