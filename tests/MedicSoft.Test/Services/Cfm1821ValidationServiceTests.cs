using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using Xunit;

namespace MedicSoft.Test.Services
{
    public class Cfm1821ValidationServiceTests
    {
        private readonly Mock<IMedicalRecordRepository> _mockMedicalRecordRepository;
        private readonly Mock<IClinicalExaminationRepository> _mockClinicalExaminationRepository;
        private readonly Mock<IDiagnosticHypothesisRepository> _mockDiagnosticHypothesisRepository;
        private readonly Mock<ITherapeuticPlanRepository> _mockTherapeuticPlanRepository;
        private readonly Mock<IInformedConsentRepository> _mockInformedConsentRepository;
        private readonly ICfm1821ValidationService _validationService;

        public Cfm1821ValidationServiceTests()
        {
            _mockMedicalRecordRepository = new Mock<IMedicalRecordRepository>();
            _mockClinicalExaminationRepository = new Mock<IClinicalExaminationRepository>();
            _mockDiagnosticHypothesisRepository = new Mock<IDiagnosticHypothesisRepository>();
            _mockTherapeuticPlanRepository = new Mock<ITherapeuticPlanRepository>();
            _mockInformedConsentRepository = new Mock<IInformedConsentRepository>();

            _validationService = new Cfm1821ValidationService(
                _mockMedicalRecordRepository.Object,
                _mockClinicalExaminationRepository.Object,
                _mockDiagnosticHypothesisRepository.Object,
                _mockTherapeuticPlanRepository.Object,
                _mockInformedConsentRepository.Object
            );
        }

        [Fact]
        public async Task ValidateMedicalRecordCompleteness_WhenRecordNotFound_ShouldReturnNonCompliant()
        {
            // Arrange
            var medicalRecordId = Guid.NewGuid();
            var tenantId = "tenant123";

            _mockMedicalRecordRepository
                .Setup(r => r.GetByIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync((MedicalRecord?)null);

            // Act
            var result = await _validationService.ValidateMedicalRecordCompleteness(medicalRecordId, tenantId);

            // Assert
            Assert.False(result.IsCompliant);
            Assert.Equal(0, result.CompletenessPercentage);
            Assert.Contains("Medical record not found", result.MissingRequirements);
        }

        [Fact]
        public async Task ValidateMedicalRecordCompleteness_WhenAllRequiredFieldsPresent_ShouldReturnCompliant()
        {
            // Arrange
            var medicalRecordId = Guid.NewGuid();
            var tenantId = "tenant123";
            var appointmentId = Guid.NewGuid();
            var patientId = Guid.NewGuid();

            var medicalRecord = new MedicalRecord(
                appointmentId, patientId, tenantId, DateTime.UtcNow,
                chiefComplaint: "Patient complains of severe headache",
                historyOfPresentIllness: "Patient has been experiencing severe headaches for the past 3 days with increasing intensity"
            );

            var clinicalExamination = new ClinicalExamination(
                medicalRecordId, tenantId,
                systematicExamination: "Patient appears well, normal vital signs with no abnormalities"
            );

            var diagnosticHypothesis = new DiagnosticHypothesis(
                medicalRecordId, tenantId, "Migraine", "G43.9"
            );

            var therapeuticPlan = new TherapeuticPlan(
                medicalRecordId, tenantId, "Rest and pain medication as prescribed"
            );

            _mockMedicalRecordRepository
                .Setup(r => r.GetByIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(medicalRecord);

            _mockClinicalExaminationRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<ClinicalExamination> { clinicalExamination });

            _mockDiagnosticHypothesisRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<DiagnosticHypothesis> { diagnosticHypothesis });

            _mockTherapeuticPlanRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<TherapeuticPlan> { therapeuticPlan });

            _mockInformedConsentRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<InformedConsent>());

            // Act
            var result = await _validationService.ValidateMedicalRecordCompleteness(medicalRecordId, tenantId);

            // Assert
            Assert.True(result.IsCompliant);
            Assert.Equal(100, result.CompletenessPercentage);
            Assert.Empty(result.MissingRequirements);
            Assert.True(result.ComponentStatus.HasChiefComplaint);
            Assert.True(result.ComponentStatus.HasHistoryOfPresentIllness);
            Assert.True(result.ComponentStatus.HasClinicalExamination);
            Assert.True(result.ComponentStatus.HasDiagnosticHypothesis);
            Assert.True(result.ComponentStatus.HasTherapeuticPlan);
        }

        [Fact]
        public async Task ValidateMedicalRecordCompleteness_WhenChiefComplaintTooShort_ShouldReturnNonCompliant()
        {
            // Arrange
            var medicalRecordId = Guid.NewGuid();
            var tenantId = "tenant123";
            var appointmentId = Guid.NewGuid();
            var patientId = Guid.NewGuid();

            var medicalRecord = new MedicalRecord(
                appointmentId, patientId, tenantId, DateTime.UtcNow,
                chiefComplaint: "Headache", // Too short - only 8 characters
                historyOfPresentIllness: "Patient has been experiencing severe headaches for the past 3 days"
            );

            _mockMedicalRecordRepository
                .Setup(r => r.GetByIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(medicalRecord);

            _mockClinicalExaminationRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<ClinicalExamination>());

            _mockDiagnosticHypothesisRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<DiagnosticHypothesis>());

            _mockTherapeuticPlanRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<TherapeuticPlan>());

            _mockInformedConsentRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<InformedConsent>());

            // Act
            var result = await _validationService.ValidateMedicalRecordCompleteness(medicalRecordId, tenantId);

            // Assert
            Assert.False(result.IsCompliant);
            Assert.Contains("Chief complaint is required (minimum 10 characters)", result.MissingRequirements);
        }

        [Fact]
        public async Task ValidateMedicalRecordCompleteness_WhenMissingClinicalExamination_ShouldReturnNonCompliant()
        {
            // Arrange
            var medicalRecordId = Guid.NewGuid();
            var tenantId = "tenant123";
            var appointmentId = Guid.NewGuid();
            var patientId = Guid.NewGuid();

            var medicalRecord = new MedicalRecord(
                appointmentId, patientId, tenantId, DateTime.UtcNow,
                chiefComplaint: "Patient complains of severe headache",
                historyOfPresentIllness: "Patient has been experiencing severe headaches for the past 3 days"
            );

            _mockMedicalRecordRepository
                .Setup(r => r.GetByIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(medicalRecord);

            _mockClinicalExaminationRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<ClinicalExamination>()); // No examination

            _mockDiagnosticHypothesisRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<DiagnosticHypothesis>());

            _mockTherapeuticPlanRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<TherapeuticPlan>());

            _mockInformedConsentRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<InformedConsent>());

            // Act
            var result = await _validationService.ValidateMedicalRecordCompleteness(medicalRecordId, tenantId);

            // Assert
            Assert.False(result.IsCompliant);
            Assert.Contains("At least one clinical examination is required", result.MissingRequirements);
            Assert.False(result.ComponentStatus.HasClinicalExamination);
        }

        [Fact]
        public async Task ValidateMedicalRecordCompleteness_WhenMissingDiagnosticHypothesis_ShouldReturnNonCompliant()
        {
            // Arrange
            var medicalRecordId = Guid.NewGuid();
            var tenantId = "tenant123";
            var appointmentId = Guid.NewGuid();
            var patientId = Guid.NewGuid();

            var medicalRecord = new MedicalRecord(
                appointmentId, patientId, tenantId, DateTime.UtcNow,
                chiefComplaint: "Patient complains of severe headache",
                historyOfPresentIllness: "Patient has been experiencing severe headaches for the past 3 days"
            );

            var clinicalExamination = new ClinicalExamination(
                medicalRecordId, tenantId,
                systematicExamination: "Patient appears well with normal vital signs"
            );

            _mockMedicalRecordRepository
                .Setup(r => r.GetByIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(medicalRecord);

            _mockClinicalExaminationRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<ClinicalExamination> { clinicalExamination });

            _mockDiagnosticHypothesisRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<DiagnosticHypothesis>()); // No diagnosis

            _mockTherapeuticPlanRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<TherapeuticPlan>());

            _mockInformedConsentRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<InformedConsent>());

            // Act
            var result = await _validationService.ValidateMedicalRecordCompleteness(medicalRecordId, tenantId);

            // Assert
            Assert.False(result.IsCompliant);
            Assert.Contains("At least one diagnostic hypothesis with ICD-10 code is required", result.MissingRequirements);
            Assert.False(result.ComponentStatus.HasDiagnosticHypothesis);
        }

        [Fact]
        public async Task IsMedicalRecordReadyForClosure_WhenCompliant_ShouldReturnTrue()
        {
            // Arrange
            var medicalRecordId = Guid.NewGuid();
            var tenantId = "tenant123";
            var appointmentId = Guid.NewGuid();
            var patientId = Guid.NewGuid();

            var medicalRecord = new MedicalRecord(
                appointmentId, patientId, tenantId, DateTime.UtcNow,
                chiefComplaint: "Patient complains of severe headache",
                historyOfPresentIllness: "Patient has been experiencing severe headaches for the past 3 days"
            );

            var clinicalExamination = new ClinicalExamination(
                medicalRecordId, tenantId,
                systematicExamination: "Patient appears well with no significant findings"
            );

            var diagnosticHypothesis = new DiagnosticHypothesis(
                medicalRecordId, tenantId, "Migraine", "G43.9"
            );

            var therapeuticPlan = new TherapeuticPlan(
                medicalRecordId, tenantId, "Rest and pain medication as prescribed"
            );

            _mockMedicalRecordRepository
                .Setup(r => r.GetByIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(medicalRecord);

            _mockClinicalExaminationRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<ClinicalExamination> { clinicalExamination });

            _mockDiagnosticHypothesisRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<DiagnosticHypothesis> { diagnosticHypothesis });

            _mockTherapeuticPlanRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<TherapeuticPlan> { therapeuticPlan });

            _mockInformedConsentRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<InformedConsent>());

            // Act
            var result = await _validationService.IsMedicalRecordReadyForClosure(medicalRecordId, tenantId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsMedicalRecordReadyForClosure_WhenNotCompliant_ShouldReturnFalse()
        {
            // Arrange
            var medicalRecordId = Guid.NewGuid();
            var tenantId = "tenant123";
            var appointmentId = Guid.NewGuid();
            var patientId = Guid.NewGuid();

            var medicalRecord = new MedicalRecord(
                appointmentId, patientId, tenantId, DateTime.UtcNow,
                chiefComplaint: "Patient complains of severe headache",
                historyOfPresentIllness: "Patient has been experiencing severe headaches for the past 3 days"
            );

            _mockMedicalRecordRepository
                .Setup(r => r.GetByIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(medicalRecord);

            _mockClinicalExaminationRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<ClinicalExamination>()); // Missing examination

            _mockDiagnosticHypothesisRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<DiagnosticHypothesis>());

            _mockTherapeuticPlanRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<TherapeuticPlan>());

            _mockInformedConsentRepository
                .Setup(r => r.GetByMedicalRecordIdAsync(medicalRecordId, tenantId))
                .ReturnsAsync(new List<InformedConsent>());

            // Act
            var result = await _validationService.IsMedicalRecordReadyForClosure(medicalRecordId, tenantId);

            // Assert
            Assert.False(result);
        }
    }
}
