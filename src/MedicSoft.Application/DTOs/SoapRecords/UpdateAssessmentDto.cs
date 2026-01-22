using System.Collections.Generic;

namespace MedicSoft.Application.DTOs.SoapRecords
{
    public class UpdateAssessmentDto
    {
        public string? PrimaryDiagnosis { get; set; }
        public string? PrimaryDiagnosisIcd10 { get; set; }
        public List<DifferentialDiagnosisDto>? DifferentialDiagnoses { get; set; }
        public string? ClinicalReasoning { get; set; }
        public string? Prognosis { get; set; }
        public string? Evolution { get; set; }
    }
}
