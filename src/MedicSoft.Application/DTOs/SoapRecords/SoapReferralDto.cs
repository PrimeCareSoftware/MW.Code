namespace MedicSoft.Application.DTOs.SoapRecords
{
    public class SoapReferralDto
    {
        public string SpecialtyName { get; set; } = string.Empty;
        public string? Reason { get; set; }
        public string? Priority { get; set; }
    }
}
