using System;

namespace MedicSoft.Application.DTOs
{
    public class ClinicalExaminationDto
    {
        public Guid Id { get; set; }
        public Guid MedicalRecordId { get; set; }
        
        // Vital signs
        public decimal? BloodPressureSystolic { get; set; }
        public decimal? BloodPressureDiastolic { get; set; }
        public int? HeartRate { get; set; }
        public int? RespiratoryRate { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? OxygenSaturation { get; set; }
        
        // Examination
        public string SystematicExamination { get; set; } = string.Empty;
        public string? GeneralState { get; set; }
        
        // Audit
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateClinicalExaminationDto
    {
        public Guid MedicalRecordId { get; set; }
        
        // Vital signs
        public decimal? BloodPressureSystolic { get; set; }
        public decimal? BloodPressureDiastolic { get; set; }
        public int? HeartRate { get; set; }
        public int? RespiratoryRate { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? OxygenSaturation { get; set; }
        
        // Examination (required - min 20 chars)
        public string SystematicExamination { get; set; } = string.Empty;
        public string? GeneralState { get; set; }
    }

    public class UpdateClinicalExaminationDto
    {
        // Vital signs
        public decimal? BloodPressureSystolic { get; set; }
        public decimal? BloodPressureDiastolic { get; set; }
        public int? HeartRate { get; set; }
        public int? RespiratoryRate { get; set; }
        public decimal? Temperature { get; set; }
        public decimal? OxygenSaturation { get; set; }
        
        // Examination
        public string? SystematicExamination { get; set; }
        public string? GeneralState { get; set; }
    }
}
