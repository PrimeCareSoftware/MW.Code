using System;
using System.Collections.Generic;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Application.DTOs
{
    public class CustomFieldDto
    {
        public string FieldKey { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public CustomFieldType FieldType { get; set; }
        public bool IsRequired { get; set; }
        public int DisplayOrder { get; set; }
        public string? Placeholder { get; set; }
        public string? DefaultValue { get; set; }
        public string? HelpText { get; set; }
        public List<string>? Options { get; set; }
    }

    public class ConsultationFormProfileDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ProfessionalSpecialty Specialty { get; set; }
        public bool IsActive { get; set; }
        public bool IsSystemDefault { get; set; }
        
        // Field visibility
        public bool ShowChiefComplaint { get; set; }
        public bool ShowHistoryOfPresentIllness { get; set; }
        public bool ShowPastMedicalHistory { get; set; }
        public bool ShowFamilyHistory { get; set; }
        public bool ShowLifestyleHabits { get; set; }
        public bool ShowCurrentMedications { get; set; }
        
        public List<CustomFieldDto> CustomFields { get; set; } = new();
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateConsultationFormProfileDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ProfessionalSpecialty Specialty { get; set; }
        
        // Field visibility
        public bool ShowChiefComplaint { get; set; } = true;
        public bool ShowHistoryOfPresentIllness { get; set; } = true;
        public bool ShowPastMedicalHistory { get; set; } = true;
        public bool ShowFamilyHistory { get; set; } = true;
        public bool ShowLifestyleHabits { get; set; } = true;
        public bool ShowCurrentMedications { get; set; } = true;
        
        public List<CustomFieldDto>? CustomFields { get; set; }
    }

    public class UpdateConsultationFormProfileDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        // Field visibility
        public bool ShowChiefComplaint { get; set; }
        public bool ShowHistoryOfPresentIllness { get; set; }
        public bool ShowPastMedicalHistory { get; set; }
        public bool ShowFamilyHistory { get; set; }
        public bool ShowLifestyleHabits { get; set; }
        public bool ShowCurrentMedications { get; set; }
        
        public List<CustomFieldDto>? CustomFields { get; set; }
    }

    public class ConsultationFormConfigurationDto
    {
        public Guid Id { get; set; }
        public Guid ClinicId { get; set; }
        public Guid? ProfileId { get; set; }
        public string ConfigurationName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        
        // Field visibility
        public bool ShowChiefComplaint { get; set; }
        public bool ShowHistoryOfPresentIllness { get; set; }
        public bool ShowPastMedicalHistory { get; set; }
        public bool ShowFamilyHistory { get; set; }
        public bool ShowLifestyleHabits { get; set; }
        public bool ShowCurrentMedications { get; set; }
        
        public List<CustomFieldDto> CustomFields { get; set; } = new();
        
        // Profile information if based on a profile
        public ConsultationFormProfileDto? Profile { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateConsultationFormConfigurationDto
    {
        public Guid ClinicId { get; set; }
        public Guid? ProfileId { get; set; }
        public string ConfigurationName { get; set; } = string.Empty;
        
        // Field visibility
        public bool ShowChiefComplaint { get; set; } = true;
        public bool ShowHistoryOfPresentIllness { get; set; } = true;
        public bool ShowPastMedicalHistory { get; set; } = true;
        public bool ShowFamilyHistory { get; set; } = true;
        public bool ShowLifestyleHabits { get; set; } = true;
        public bool ShowCurrentMedications { get; set; } = true;
        
        public List<CustomFieldDto>? CustomFields { get; set; }
    }

    public class UpdateConsultationFormConfigurationDto
    {
        public string ConfigurationName { get; set; } = string.Empty;
        
        // Field visibility
        public bool ShowChiefComplaint { get; set; }
        public bool ShowHistoryOfPresentIllness { get; set; }
        public bool ShowPastMedicalHistory { get; set; }
        public bool ShowFamilyHistory { get; set; }
        public bool ShowLifestyleHabits { get; set; }
        public bool ShowCurrentMedications { get; set; }
        
        public List<CustomFieldDto>? CustomFields { get; set; }
    }
}
