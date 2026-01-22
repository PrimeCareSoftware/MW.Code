namespace MedicSoft.Application.DTOs.SoapRecords
{
    public class VitalSignsDto
    {
        public int? SystolicBP { get; set; }
        public int? DiastolicBP { get; set; }
        public int? HeartRate { get; set; }
        public int? RespiratoryRate { get; set; }
        public decimal? Temperature { get; set; }
        public int? OxygenSaturation { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Height { get; set; }
        public decimal? BMI { get; set; }
        public int? Pain { get; set; }
    }
}
