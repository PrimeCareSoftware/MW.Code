using System;

namespace MedicSoft.Application.DTOs
{
    public enum DiagnosisTypeDto
    {
        Principal = 1,
        Secondary = 2
    }

    public class DiagnosticHypothesisDto
    {
        public Guid Id { get; set; }
        public Guid MedicalRecordId { get; set; }
        
        public string Description { get; set; } = string.Empty;
        public string ICD10Code { get; set; } = string.Empty;
        public DiagnosisTypeDto Type { get; set; }
        public DateTime DiagnosedAt { get; set; }
        
        // Audit
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateDiagnosticHypothesisDto
    {
        public Guid MedicalRecordId { get; set; }
        
        public string Description { get; set; } = string.Empty;
        public string ICD10Code { get; set; } = string.Empty;
        public DiagnosisTypeDto Type { get; set; } = DiagnosisTypeDto.Principal;
    }

    public class UpdateDiagnosticHypothesisDto
    {
        public string? Description { get; set; }
        public string? ICD10Code { get; set; }
        public DiagnosisTypeDto? Type { get; set; }
    }
}
