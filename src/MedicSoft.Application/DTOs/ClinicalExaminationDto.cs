using System;
using System.ComponentModel.DataAnnotations;

namespace MedicSoft.Application.DTOs
{
    public class ClinicalExaminationDto
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "ID do prontuário médico é obrigatório")]
        public Guid MedicalRecordId { get; set; }
        
        // Vital signs
        [Range(50, 300, ErrorMessage = "Pressão sistólica deve estar entre 50 e 300 mmHg")]
        public decimal? BloodPressureSystolic { get; set; }
        
        [Range(30, 200, ErrorMessage = "Pressão diastólica deve estar entre 30 e 200 mmHg")]
        public decimal? BloodPressureDiastolic { get; set; }
        
        [Range(30, 300, ErrorMessage = "Frequência cardíaca deve estar entre 30 e 300 bpm")]
        public int? HeartRate { get; set; }
        
        [Range(8, 60, ErrorMessage = "Frequência respiratória deve estar entre 8 e 60 rpm")]
        public int? RespiratoryRate { get; set; }
        
        [Range(32.0, 43.0, ErrorMessage = "Temperatura deve estar entre 32 e 43°C")]
        public decimal? Temperature { get; set; }
        
        [Range(70, 100, ErrorMessage = "Saturação de oxigênio deve estar entre 70 e 100%")]
        public decimal? OxygenSaturation { get; set; }
        
        // Anthropometric data
        [Range(0.5, 500, ErrorMessage = "Peso deve estar entre 0.5 e 500 kg")]
        public decimal? Weight { get; set; }
        
        [Range(0.3, 3.0, ErrorMessage = "Altura deve estar entre 0.3 e 3.0 metros")]
        public decimal? Height { get; set; }
        
        // Calculated BMI (read-only)
        public decimal? BMI { get; set; }
        
        // Examination
        [Required(ErrorMessage = "Exame sistemático é obrigatório")]
        [StringLength(5000, MinimumLength = 20, ErrorMessage = "Exame sistemático deve ter entre 20 e 5000 caracteres")]
        public string SystematicExamination { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Estado geral deve ter no máximo 500 caracteres")]
        public string? GeneralState { get; set; }
        
        // Audit
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateClinicalExaminationDto
    {
        [Required(ErrorMessage = "ID do prontuário médico é obrigatório")]
        public Guid MedicalRecordId { get; set; }
        
        // Vital signs
        [Range(50, 300, ErrorMessage = "Pressão sistólica deve estar entre 50 e 300 mmHg")]
        public decimal? BloodPressureSystolic { get; set; }
        
        [Range(30, 200, ErrorMessage = "Pressão diastólica deve estar entre 30 e 200 mmHg")]
        public decimal? BloodPressureDiastolic { get; set; }
        
        [Range(30, 300, ErrorMessage = "Frequência cardíaca deve estar entre 30 e 300 bpm")]
        public int? HeartRate { get; set; }
        
        [Range(8, 60, ErrorMessage = "Frequência respiratória deve estar entre 8 e 60 rpm")]
        public int? RespiratoryRate { get; set; }
        
        [Range(32.0, 43.0, ErrorMessage = "Temperatura deve estar entre 32 e 43°C")]
        public decimal? Temperature { get; set; }
        
        [Range(70, 100, ErrorMessage = "Saturação de oxigênio deve estar entre 70 e 100%")]
        public decimal? OxygenSaturation { get; set; }
        
        // Anthropometric data
        [Range(0.5, 500, ErrorMessage = "Peso deve estar entre 0.5 e 500 kg")]
        public decimal? Weight { get; set; }
        
        [Range(0.3, 3.0, ErrorMessage = "Altura deve estar entre 0.3 e 3.0 metros")]
        public decimal? Height { get; set; }
        
        // Examination (required - min 20 chars)
        [Required(ErrorMessage = "Exame sistemático é obrigatório")]
        [StringLength(5000, MinimumLength = 20, ErrorMessage = "Exame sistemático deve ter entre 20 e 5000 caracteres")]
        public string SystematicExamination { get; set; } = string.Empty;
        
        [StringLength(500, ErrorMessage = "Estado geral deve ter no máximo 500 caracteres")]
        public string? GeneralState { get; set; }
    }

    public class UpdateClinicalExaminationDto
    {
        // Vital signs
        [Range(50, 300, ErrorMessage = "Pressão sistólica deve estar entre 50 e 300 mmHg")]
        public decimal? BloodPressureSystolic { get; set; }
        
        [Range(30, 200, ErrorMessage = "Pressão diastólica deve estar entre 30 e 200 mmHg")]
        public decimal? BloodPressureDiastolic { get; set; }
        
        [Range(30, 300, ErrorMessage = "Frequência cardíaca deve estar entre 30 e 300 bpm")]
        public int? HeartRate { get; set; }
        
        [Range(8, 60, ErrorMessage = "Frequência respiratória deve estar entre 8 e 60 rpm")]
        public int? RespiratoryRate { get; set; }
        
        [Range(32.0, 43.0, ErrorMessage = "Temperatura deve estar entre 32 e 43°C")]
        public decimal? Temperature { get; set; }
        
        [Range(70, 100, ErrorMessage = "Saturação de oxigênio deve estar entre 70 e 100%")]
        public decimal? OxygenSaturation { get; set; }
        
        // Anthropometric data
        [Range(0.5, 500, ErrorMessage = "Peso deve estar entre 0.5 e 500 kg")]
        public decimal? Weight { get; set; }
        
        [Range(0.3, 3.0, ErrorMessage = "Altura deve estar entre 0.3 e 3.0 metros")]
        public decimal? Height { get; set; }
        
        // Examination
        [StringLength(5000, ErrorMessage = "Exame sistemático deve ter no máximo 5000 caracteres")]
        public string? SystematicExamination { get; set; }
        
        [StringLength(500, ErrorMessage = "Estado geral deve ter no máximo 500 caracteres")]
        public string? GeneralState { get; set; }
    }
}
