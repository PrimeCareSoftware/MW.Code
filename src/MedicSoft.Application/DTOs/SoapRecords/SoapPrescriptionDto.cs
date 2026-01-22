namespace MedicSoft.Application.DTOs.SoapRecords
{
    public class SoapPrescriptionDto
    {
        public string MedicationName { get; set; } = string.Empty;
        public string? Dosage { get; set; }
        public string? Frequency { get; set; }
        public string? Duration { get; set; }
        public string? Instructions { get; set; }
    }
}
