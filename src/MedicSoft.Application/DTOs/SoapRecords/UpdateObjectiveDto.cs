namespace MedicSoft.Application.DTOs.SoapRecords
{
    public class UpdateObjectiveDto
    {
        public VitalSignsDto? VitalSigns { get; set; }
        public PhysicalExaminationDto? PhysicalExam { get; set; }
        public string? LabResults { get; set; }
        public string? ImagingResults { get; set; }
        public string? OtherExamResults { get; set; }
    }
}
