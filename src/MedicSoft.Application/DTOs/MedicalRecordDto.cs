using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedicSoft.Application.DTOs
{
    public class MedicalRecordDto
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "ID da consulta é obrigatório")]
        public Guid AppointmentId { get; set; }
        
        [Required(ErrorMessage = "ID do paciente é obrigatório")]
        public Guid PatientId { get; set; }
        
        public string PatientName { get; set; } = string.Empty;
        
        // CFM 1.821 - Required fields
        [Required(ErrorMessage = "Queixa principal é obrigatória")]
        [StringLength(2000, MinimumLength = 3, ErrorMessage = "Queixa principal deve ter entre 3 e 2000 caracteres")]
        public string ChiefComplaint { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "História da doença presente é obrigatória")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "História da doença presente deve ter entre 10 e 5000 caracteres")]
        public string HistoryOfPresentIllness { get; set; } = string.Empty;
        
        // CFM 1.821 - Recommended fields
        [StringLength(5000, ErrorMessage = "Histórico médico deve ter no máximo 5000 caracteres")]
        public string? PastMedicalHistory { get; set; }
        
        [StringLength(3000, ErrorMessage = "Histórico familiar deve ter no máximo 3000 caracteres")]
        public string? FamilyHistory { get; set; }
        
        [StringLength(2000, ErrorMessage = "Hábitos de vida devem ter no máximo 2000 caracteres")]
        public string? LifestyleHabits { get; set; }
        
        [StringLength(3000, ErrorMessage = "Medicamentos atuais devem ter no máximo 3000 caracteres")]
        public string? CurrentMedications { get; set; }

        [StringLength(5000, ErrorMessage = "Plano alimentar deve ter no máximo 5000 caracteres")]
        public string? NutritionalPlan { get; set; }

        [StringLength(5000, ErrorMessage = "Evolução nutricional deve ter no máximo 5000 caracteres")]
        public string? NutritionalEvolution { get; set; }

        [StringLength(5000, ErrorMessage = "Evolução terapêutica deve ter no máximo 5000 caracteres")]
        public string? TherapeuticEvolution { get; set; }
        
        // Legacy fields (DEPRECATED - use related entities)
        [StringLength(5000, ErrorMessage = "Diagnóstico deve ter no máximo 5000 caracteres")]
        public string Diagnosis { get; set; } = string.Empty;
        
        [StringLength(5000, ErrorMessage = "Prescrição deve ter no máximo 5000 caracteres")]
        public string Prescription { get; set; } = string.Empty;
        
        [StringLength(5000, ErrorMessage = "Notas devem ter no máximo 5000 caracteres")]
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
        [Required(ErrorMessage = "ID da consulta é obrigatório")]
        public Guid AppointmentId { get; set; }
        
        [Required(ErrorMessage = "ID do paciente é obrigatório")]
        public Guid PatientId { get; set; }
        
        [Required(ErrorMessage = "Horário de início da consulta é obrigatório")]
        public DateTime ConsultationStartTime { get; set; }
        
        // CFM 1.821 - Required fields
        [Required(ErrorMessage = "Queixa principal é obrigatória")]
        [StringLength(2000, MinimumLength = 3, ErrorMessage = "Queixa principal deve ter entre 3 e 2000 caracteres")]
        public string ChiefComplaint { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "História da doença presente é obrigatória")]
        [StringLength(5000, MinimumLength = 10, ErrorMessage = "História da doença presente deve ter entre 10 e 5000 caracteres")]
        public string HistoryOfPresentIllness { get; set; } = string.Empty;
        
        // CFM 1.821 - Recommended fields
        [StringLength(5000, ErrorMessage = "Histórico médico deve ter no máximo 5000 caracteres")]
        public string? PastMedicalHistory { get; set; }
        
        [StringLength(3000, ErrorMessage = "Histórico familiar deve ter no máximo 3000 caracteres")]
        public string? FamilyHistory { get; set; }
        
        [StringLength(2000, ErrorMessage = "Hábitos de vida devem ter no máximo 2000 caracteres")]
        public string? LifestyleHabits { get; set; }
        
        [StringLength(3000, ErrorMessage = "Medicamentos atuais devem ter no máximo 3000 caracteres")]
        public string? CurrentMedications { get; set; }
        
        // Legacy fields (optional for backward compatibility)
        [StringLength(5000, ErrorMessage = "Diagnóstico deve ter no máximo 5000 caracteres")]
        public string? Diagnosis { get; set; }
        
        [StringLength(5000, ErrorMessage = "Prescrição deve ter no máximo 5000 caracteres")]
        public string? Prescription { get; set; }
        
        [StringLength(5000, ErrorMessage = "Notas devem ter no máximo 5000 caracteres")]
        public string? Notes { get; set; }

        [StringLength(5000, ErrorMessage = "Plano alimentar deve ter no máximo 5000 caracteres")]
        public string? NutritionalPlan { get; set; }

        [StringLength(5000, ErrorMessage = "Evolução nutricional deve ter no máximo 5000 caracteres")]
        public string? NutritionalEvolution { get; set; }

        [StringLength(5000, ErrorMessage = "Evolução terapêutica deve ter no máximo 5000 caracteres")]
        public string? TherapeuticEvolution { get; set; }
    }

    public class UpdateMedicalRecordDto
    {
        // CFM 1.821 fields
        [StringLength(2000, ErrorMessage = "Queixa principal deve ter no máximo 2000 caracteres")]
        public string? ChiefComplaint { get; set; }
        
        [StringLength(5000, ErrorMessage = "História da doença presente deve ter no máximo 5000 caracteres")]
        public string? HistoryOfPresentIllness { get; set; }
        
        [StringLength(5000, ErrorMessage = "Histórico médico deve ter no máximo 5000 caracteres")]
        public string? PastMedicalHistory { get; set; }
        
        [StringLength(3000, ErrorMessage = "Histórico familiar deve ter no máximo 3000 caracteres")]
        public string? FamilyHistory { get; set; }
        
        [StringLength(2000, ErrorMessage = "Hábitos de vida devem ter no máximo 2000 caracteres")]
        public string? LifestyleHabits { get; set; }
        
        [StringLength(3000, ErrorMessage = "Medicamentos atuais devem ter no máximo 3000 caracteres")]
        public string? CurrentMedications { get; set; }

        [StringLength(5000, ErrorMessage = "Plano alimentar deve ter no máximo 5000 caracteres")]
        public string? NutritionalPlan { get; set; }

        [StringLength(5000, ErrorMessage = "Evolução nutricional deve ter no máximo 5000 caracteres")]
        public string? NutritionalEvolution { get; set; }

        [StringLength(5000, ErrorMessage = "Evolução terapêutica deve ter no máximo 5000 caracteres")]
        public string? TherapeuticEvolution { get; set; }
        
        // Legacy fields
        [StringLength(5000, ErrorMessage = "Diagnóstico deve ter no máximo 5000 caracteres")]
        public string? Diagnosis { get; set; }
        
        [StringLength(5000, ErrorMessage = "Prescrição deve ter no máximo 5000 caracteres")]
        public string? Prescription { get; set; }
        
        [StringLength(5000, ErrorMessage = "Notas devem ter no máximo 5000 caracteres")]
        public string? Notes { get; set; }
        
        [Range(5, 480, ErrorMessage = "Duração da consulta deve estar entre 5 e 480 minutos")]
        public int? ConsultationDurationMinutes { get; set; }
    }

    public class CompleteMedicalRecordDto
    {
        [StringLength(5000, ErrorMessage = "Diagnóstico deve ter no máximo 5000 caracteres")]
        public string? Diagnosis { get; set; }
        
        [StringLength(5000, ErrorMessage = "Prescrição deve ter no máximo 5000 caracteres")]
        public string? Prescription { get; set; }
        
        [StringLength(5000, ErrorMessage = "Notas devem ter no máximo 5000 caracteres")]
        public string? Notes { get; set; }

        [StringLength(5000, ErrorMessage = "Plano alimentar deve ter no máximo 5000 caracteres")]
        public string? NutritionalPlan { get; set; }

        [StringLength(5000, ErrorMessage = "Evolução nutricional deve ter no máximo 5000 caracteres")]
        public string? NutritionalEvolution { get; set; }

        [StringLength(5000, ErrorMessage = "Evolução terapêutica deve ter no máximo 5000 caracteres")]
        public string? TherapeuticEvolution { get; set; }
    }
}
