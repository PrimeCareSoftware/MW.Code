using System;

namespace MedicSoft.Application.DTOs
{
    public class TherapeuticPlanDto
    {
        public Guid Id { get; set; }
        public Guid MedicalRecordId { get; set; }
        
        public string Treatment { get; set; } = string.Empty;
        public string? MedicationPrescription { get; set; }
        public string? ExamRequests { get; set; }
        public string? Referrals { get; set; }
        public string? PatientGuidance { get; set; }
        public DateTime? ReturnDate { get; set; }
        
        // Audit
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateTherapeuticPlanDto
    {
        public Guid MedicalRecordId { get; set; }
        
        public string Treatment { get; set; } = string.Empty;
        public string? MedicationPrescription { get; set; }
        public string? ExamRequests { get; set; }
        public string? Referrals { get; set; }
        public string? PatientGuidance { get; set; }
        public DateTime? ReturnDate { get; set; }
    }

    public class UpdateTherapeuticPlanDto
    {
        public string? Treatment { get; set; }
        public string? MedicationPrescription { get; set; }
        public string? ExamRequests { get; set; }
        public string? Referrals { get; set; }
        public string? PatientGuidance { get; set; }
        public DateTime? ReturnDate { get; set; }
    }
}
