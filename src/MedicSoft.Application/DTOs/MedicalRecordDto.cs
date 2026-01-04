using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs
{
    public class MedicalRecordDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        
        // CFM 1.821 - Required fields
        public string ChiefComplaint { get; set; } = string.Empty;
        public string HistoryOfPresentIllness { get; set; } = string.Empty;
        
        // CFM 1.821 - Recommended fields
        public string? PastMedicalHistory { get; set; }
        public string? FamilyHistory { get; set; }
        public string? LifestyleHabits { get; set; }
        public string? CurrentMedications { get; set; }
        
        // Legacy fields (DEPRECATED - use related entities)
        public string Diagnosis { get; set; } = string.Empty;
        public string Prescription { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        
        public int ConsultationDurationMinutes { get; set; }
        public DateTime ConsultationStartTime { get; set; }
        public DateTime? ConsultationEndTime { get; set; }
        
        // Closure control (CFM 1.821 - Audit)
        public bool IsClosed { get; set; }
        public DateTime? ClosedAt { get; set; }
        public Guid? ClosedByUserId { get; set; }
        
        // CFM 1.821 - Related entities
        public List<ClinicalExaminationDto> Examinations { get; set; } = new();
        public List<DiagnosticHypothesisDto> Diagnoses { get; set; } = new();
        public List<TherapeuticPlanDto> Plans { get; set; } = new();
        public List<InformedConsentDto> Consents { get; set; } = new();
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateMedicalRecordDto
    {
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public DateTime ConsultationStartTime { get; set; }
        
        // CFM 1.821 - Required fields
        public string ChiefComplaint { get; set; } = string.Empty;
        public string HistoryOfPresentIllness { get; set; } = string.Empty;
        
        // CFM 1.821 - Recommended fields
        public string? PastMedicalHistory { get; set; }
        public string? FamilyHistory { get; set; }
        public string? LifestyleHabits { get; set; }
        public string? CurrentMedications { get; set; }
        
        // Legacy fields (optional for backward compatibility)
        public string? Diagnosis { get; set; }
        public string? Prescription { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateMedicalRecordDto
    {
        // CFM 1.821 fields
        public string? ChiefComplaint { get; set; }
        public string? HistoryOfPresentIllness { get; set; }
        public string? PastMedicalHistory { get; set; }
        public string? FamilyHistory { get; set; }
        public string? LifestyleHabits { get; set; }
        public string? CurrentMedications { get; set; }
        
        // Legacy fields
        public string? Diagnosis { get; set; }
        public string? Prescription { get; set; }
        public string? Notes { get; set; }
        public int? ConsultationDurationMinutes { get; set; }
    }

    public class CompleteMedicalRecordDto
    {
        public string? Diagnosis { get; set; }
        public string? Prescription { get; set; }
        public string? Notes { get; set; }
    }
}
