using System;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Implementation of CFM 1.821 validation service
    /// </summary>
    public class Cfm1821ValidationService : ICfm1821ValidationService
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IClinicalExaminationRepository _clinicalExaminationRepository;
        private readonly IDiagnosticHypothesisRepository _diagnosticHypothesisRepository;
        private readonly ITherapeuticPlanRepository _therapeuticPlanRepository;
        private readonly IInformedConsentRepository _informedConsentRepository;

        // CFM 1.821 validation constants
        private const int MIN_CHIEF_COMPLAINT_LENGTH = 10;
        private const int MIN_HISTORY_LENGTH = 50;
        private const int TOTAL_REQUIRED_FIELDS = 5; // ChiefComplaint, HDA, Examination, Diagnosis, Plan
        private const double PERCENTAGE_MULTIPLIER = 100.0;

        public Cfm1821ValidationService(
            IMedicalRecordRepository medicalRecordRepository,
            IClinicalExaminationRepository clinicalExaminationRepository,
            IDiagnosticHypothesisRepository diagnosticHypothesisRepository,
            ITherapeuticPlanRepository therapeuticPlanRepository,
            IInformedConsentRepository informedConsentRepository)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _clinicalExaminationRepository = clinicalExaminationRepository;
            _diagnosticHypothesisRepository = diagnosticHypothesisRepository;
            _therapeuticPlanRepository = therapeuticPlanRepository;
            _informedConsentRepository = informedConsentRepository;
        }

        public async Task<Cfm1821ValidationResult> ValidateMedicalRecordCompleteness(Guid medicalRecordId, string tenantId)
        {
            var result = new Cfm1821ValidationResult
            {
                ComponentStatus = new Cfm1821ComponentStatus()
            };

            // Get medical record
            var medicalRecord = await _medicalRecordRepository.GetByIdAsync(medicalRecordId, tenantId);
            if (medicalRecord == null)
            {
                result.MissingRequirements.Add("Medical record not found");
                result.IsCompliant = false;
                result.CompletenessPercentage = 0;
                return result;
            }

            // Check required anamnesis fields
            result.ComponentStatus.HasChiefComplaint = !string.IsNullOrWhiteSpace(medicalRecord.ChiefComplaint) 
                && medicalRecord.ChiefComplaint.Length >= MIN_CHIEF_COMPLAINT_LENGTH;
            
            result.ComponentStatus.HasHistoryOfPresentIllness = !string.IsNullOrWhiteSpace(medicalRecord.HistoryOfPresentIllness) 
                && medicalRecord.HistoryOfPresentIllness.Length >= MIN_HISTORY_LENGTH;

            if (!result.ComponentStatus.HasChiefComplaint)
                result.MissingRequirements.Add($"Chief complaint is required (minimum {MIN_CHIEF_COMPLAINT_LENGTH} characters)");
            
            if (!result.ComponentStatus.HasHistoryOfPresentIllness)
                result.MissingRequirements.Add($"History of present illness is required (minimum {MIN_HISTORY_LENGTH} characters)");

            // Check optional but recommended fields
            result.ComponentStatus.HasPastMedicalHistory = !string.IsNullOrWhiteSpace(medicalRecord.PastMedicalHistory);
            result.ComponentStatus.HasFamilyHistory = !string.IsNullOrWhiteSpace(medicalRecord.FamilyHistory);
            result.ComponentStatus.HasLifestyleHabits = !string.IsNullOrWhiteSpace(medicalRecord.LifestyleHabits);
            result.ComponentStatus.HasCurrentMedications = !string.IsNullOrWhiteSpace(medicalRecord.CurrentMedications);

            if (!result.ComponentStatus.HasPastMedicalHistory)
                result.Warnings.Add("Past medical history is recommended for complete records");
            
            if (!result.ComponentStatus.HasFamilyHistory)
                result.Warnings.Add("Family history is recommended for complete records");

            // Check clinical examination
            var examinations = await _clinicalExaminationRepository.GetByMedicalRecordIdAsync(medicalRecordId, tenantId);
            result.ComponentStatus.HasClinicalExamination = examinations?.Any() == true;
            
            if (!result.ComponentStatus.HasClinicalExamination)
                result.MissingRequirements.Add("At least one clinical examination is required");

            // Check diagnostic hypothesis
            var diagnoses = await _diagnosticHypothesisRepository.GetByMedicalRecordIdAsync(medicalRecordId, tenantId);
            result.ComponentStatus.HasDiagnosticHypothesis = diagnoses?.Any() == true;
            
            if (!result.ComponentStatus.HasDiagnosticHypothesis)
                result.MissingRequirements.Add("At least one diagnostic hypothesis with ICD-10 code is required");

            // Check therapeutic plan
            var plans = await _therapeuticPlanRepository.GetByMedicalRecordIdAsync(medicalRecordId, tenantId);
            result.ComponentStatus.HasTherapeuticPlan = plans?.Any() == true;
            
            if (!result.ComponentStatus.HasTherapeuticPlan)
                result.MissingRequirements.Add("At least one therapeutic plan is required");

            // Check informed consent (recommended but not always mandatory)
            var consents = await _informedConsentRepository.GetByMedicalRecordIdAsync(medicalRecordId, tenantId);
            result.ComponentStatus.HasInformedConsent = consents?.Any(c => c.IsAccepted) == true;
            
            if (!result.ComponentStatus.HasInformedConsent)
                result.Warnings.Add("Informed consent is recommended, especially for invasive procedures");

            // Calculate completeness
            int requiredFieldsComplete = 0;

            if (result.ComponentStatus.HasChiefComplaint) requiredFieldsComplete++;
            if (result.ComponentStatus.HasHistoryOfPresentIllness) requiredFieldsComplete++;
            if (result.ComponentStatus.HasClinicalExamination) requiredFieldsComplete++;
            if (result.ComponentStatus.HasDiagnosticHypothesis) requiredFieldsComplete++;
            if (result.ComponentStatus.HasTherapeuticPlan) requiredFieldsComplete++;

            result.CompletenessPercentage = (requiredFieldsComplete / (double)TOTAL_REQUIRED_FIELDS) * PERCENTAGE_MULTIPLIER;
            result.IsCompliant = result.MissingRequirements.Count == 0;

            return result;
        }

        public async Task<bool> IsMedicalRecordReadyForClosure(Guid medicalRecordId, string tenantId)
        {
            var validationResult = await ValidateMedicalRecordCompleteness(medicalRecordId, tenantId);
            return validationResult.IsCompliant;
        }
    }
}
