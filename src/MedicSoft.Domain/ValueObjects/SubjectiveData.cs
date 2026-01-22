using System;

namespace MedicSoft.Domain.ValueObjects
{
    public class SubjectiveData
    {
        public string ChiefComplaint { get; private set; }  // Queixa principal
        public string HistoryOfPresentIllness { get; private set; }  // HDA
        public string? CurrentSymptoms { get; private set; }
        public string? SymptomDuration { get; private set; }
        public string? AggravatingFactors { get; private set; }  // Fatores de piora
        public string? RelievingFactors { get; private set; }  // Fatores de melhora
        public string? ReviewOfSystems { get; private set; }  // Revisão de sistemas
        public string? Allergies { get; private set; }
        public string? CurrentMedications { get; private set; }
        public string? PastMedicalHistory { get; private set; }
        public string? FamilyHistory { get; private set; }
        public string? SocialHistory { get; private set; }  // Hábitos

        private SubjectiveData() 
        {
            ChiefComplaint = null!;
            HistoryOfPresentIllness = null!;
        }

        public SubjectiveData(
            string chiefComplaint,
            string historyOfPresentIllness,
            string? currentSymptoms = null,
            string? symptomDuration = null,
            string? aggravatingFactors = null,
            string? relievingFactors = null,
            string? reviewOfSystems = null,
            string? allergies = null,
            string? currentMedications = null,
            string? pastMedicalHistory = null,
            string? familyHistory = null,
            string? socialHistory = null)
        {
            if (string.IsNullOrWhiteSpace(chiefComplaint))
                throw new ArgumentException("Chief complaint is required", nameof(chiefComplaint));
            
            if (string.IsNullOrWhiteSpace(historyOfPresentIllness))
                throw new ArgumentException("History of present illness is required", nameof(historyOfPresentIllness));

            ChiefComplaint = chiefComplaint.Trim();
            HistoryOfPresentIllness = historyOfPresentIllness.Trim();
            CurrentSymptoms = currentSymptoms?.Trim();
            SymptomDuration = symptomDuration?.Trim();
            AggravatingFactors = aggravatingFactors?.Trim();
            RelievingFactors = relievingFactors?.Trim();
            ReviewOfSystems = reviewOfSystems?.Trim();
            Allergies = allergies?.Trim();
            CurrentMedications = currentMedications?.Trim();
            PastMedicalHistory = pastMedicalHistory?.Trim();
            FamilyHistory = familyHistory?.Trim();
            SocialHistory = socialHistory?.Trim();
        }

        public override bool Equals(object? obj)
        {
            if (obj is not SubjectiveData other)
                return false;

            return ChiefComplaint == other.ChiefComplaint &&
                   HistoryOfPresentIllness == other.HistoryOfPresentIllness &&
                   CurrentSymptoms == other.CurrentSymptoms &&
                   SymptomDuration == other.SymptomDuration &&
                   AggravatingFactors == other.AggravatingFactors &&
                   RelievingFactors == other.RelievingFactors &&
                   ReviewOfSystems == other.ReviewOfSystems &&
                   Allergies == other.Allergies &&
                   CurrentMedications == other.CurrentMedications &&
                   PastMedicalHistory == other.PastMedicalHistory &&
                   FamilyHistory == other.FamilyHistory &&
                   SocialHistory == other.SocialHistory;
        }

        public override int GetHashCode()
        {
            var hash = new HashCode();
            hash.Add(ChiefComplaint);
            hash.Add(HistoryOfPresentIllness);
            hash.Add(CurrentSymptoms);
            hash.Add(SymptomDuration);
            hash.Add(AggravatingFactors);
            hash.Add(RelievingFactors);
            hash.Add(ReviewOfSystems);
            hash.Add(Allergies);
            hash.Add(CurrentMedications);
            hash.Add(PastMedicalHistory);
            hash.Add(FamilyHistory);
            hash.Add(SocialHistory);
            return hash.ToHashCode();
        }
    }
}
