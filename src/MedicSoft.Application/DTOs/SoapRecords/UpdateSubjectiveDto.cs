namespace MedicSoft.Application.DTOs.SoapRecords
{
    public class UpdateSubjectiveDto
    {
        public string ChiefComplaint { get; set; } = string.Empty;
        public string HistoryOfPresentIllness { get; set; } = string.Empty;
        public string? CurrentSymptoms { get; set; }
        public string? SymptomDuration { get; set; }
        public string? AggravatingFactors { get; set; }
        public string? RelievingFactors { get; set; }
        public string? ReviewOfSystems { get; set; }
        public string? Allergies { get; set; }
        public string? CurrentMedications { get; set; }
        public string? PastMedicalHistory { get; set; }
        public string? FamilyHistory { get; set; }
        public string? SocialHistory { get; set; }
    }
}
