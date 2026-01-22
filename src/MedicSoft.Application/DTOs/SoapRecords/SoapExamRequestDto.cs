namespace MedicSoft.Application.DTOs.SoapRecords
{
    public class SoapExamRequestDto
    {
        public string ExamName { get; set; } = string.Empty;
        public string? ExamType { get; set; }
        public string? ClinicalIndication { get; set; }
        public bool IsUrgent { get; set; }
    }
}
