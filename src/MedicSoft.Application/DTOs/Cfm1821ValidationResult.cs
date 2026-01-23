namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// Result of CFM 1.821 validation for a medical record
    /// </summary>
    public class Cfm1821ValidationResult
    {
        public bool IsCompliant { get; set; }
        public List<string> MissingRequirements { get; set; } = new();
        public List<string> Warnings { get; set; } = new();
        public double CompletenessPercentage { get; set; }
        
        /// <summary>
        /// Details about each CFM 1.821 component
        /// </summary>
        public Cfm1821ComponentStatus ComponentStatus { get; set; } = new();
    }
    
    /// <summary>
    /// Status of each CFM 1.821 component
    /// </summary>
    public class Cfm1821ComponentStatus
    {
        public bool HasChiefComplaint { get; set; }
        public bool HasHistoryOfPresentIllness { get; set; }
        public bool HasClinicalExamination { get; set; }
        public bool HasDiagnosticHypothesis { get; set; }
        public bool HasTherapeuticPlan { get; set; }
        public bool HasInformedConsent { get; set; }
        
        // Optional but recommended fields
        public bool HasPastMedicalHistory { get; set; }
        public bool HasFamilyHistory { get; set; }
        public bool HasLifestyleHabits { get; set; }
        public bool HasCurrentMedications { get; set; }
    }
}
