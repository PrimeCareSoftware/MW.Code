namespace MedicSoft.Application.DTOs.SoapRecords
{
    public class DifferentialDiagnosisDto
    {
        public string Diagnosis { get; set; } = string.Empty;
        public string? Icd10Code { get; set; }
        public string? Justification { get; set; }
        public int Priority { get; set; } = 1;
    }
}
