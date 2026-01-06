using System;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class DigitalPrescriptionTests
    {
        private readonly string _tenantId = "test-tenant";
        private readonly Guid _medicalRecordId = Guid.NewGuid();
        private readonly Guid _patientId = Guid.NewGuid();
        private readonly Guid _doctorId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidData_CreatesDigitalPrescription()
        {
            // Arrange
            var type = PrescriptionType.Simple;
            var doctorName = "Dr. João Silva";
            var doctorCRM = "12345";
            var doctorCRMState = "SP";
            var patientName = "Maria Santos";
            var patientDocument = "12345678900";

            // Act
            var prescription = new DigitalPrescription(
                _medicalRecordId,
                _patientId,
                _doctorId,
                type,
                doctorName,
                doctorCRM,
                doctorCRMState,
                patientName,
                patientDocument,
                _tenantId
            );

            // Assert
            Assert.NotEqual(Guid.Empty, prescription.Id);
            Assert.Equal(_medicalRecordId, prescription.MedicalRecordId);
            Assert.Equal(_patientId, prescription.PatientId);
            Assert.Equal(_doctorId, prescription.DoctorId);
            Assert.Equal(type, prescription.Type);
            Assert.Equal(doctorName, prescription.DoctorName);
            Assert.Equal(doctorCRM, prescription.DoctorCRM);
            Assert.Equal(doctorCRMState, prescription.DoctorCRMState);
            Assert.Equal(patientName, prescription.PatientName);
            Assert.Equal(patientDocument, prescription.PatientDocument);
            Assert.True(prescription.IsActive);
            Assert.NotNull(prescription.VerificationCode);
            Assert.Empty(prescription.Items);
        }

        [Fact]
        public void Constructor_WithControlledPrescription_RequiresSNGPCReport()
        {
            // Arrange & Act
            var prescriptionA = new DigitalPrescription(
                _medicalRecordId, _patientId, _doctorId,
                PrescriptionType.SpecialControlA,
                "Dr. João", "12345", "SP",
                "Maria", "12345678900", _tenantId);

            var prescriptionB = new DigitalPrescription(
                _medicalRecordId, _patientId, _doctorId,
                PrescriptionType.SpecialControlB,
                "Dr. João", "12345", "SP",
                "Maria", "12345678900", _tenantId);

            var prescriptionSimple = new DigitalPrescription(
                _medicalRecordId, _patientId, _doctorId,
                PrescriptionType.Simple,
                "Dr. João", "12345", "SP",
                "Maria", "12345678900", _tenantId);

            // Assert
            Assert.True(prescriptionA.RequiresSNGPCReport);
            Assert.True(prescriptionB.RequiresSNGPCReport);
            Assert.False(prescriptionSimple.RequiresSNGPCReport);
        }

        [Fact]
        public void Constructor_WithSequenceNumber_StoresSequenceNumber()
        {
            // Arrange
            var sequenceNumber = "2024-B-0001234";

            // Act
            var prescription = new DigitalPrescription(
                _medicalRecordId, _patientId, _doctorId,
                PrescriptionType.SpecialControlB,
                "Dr. João", "12345", "SP",
                "Maria", "12345678900", _tenantId,
                sequenceNumber);

            // Assert
            Assert.Equal(sequenceNumber, prescription.SequenceNumber);
        }

        [Fact]
        public void AddItem_WithValidItem_AddsToCollection()
        {
            // Arrange
            var prescription = CreateValidPrescription();
            var item = CreateValidItem(prescription.Id);

            // Act
            prescription.AddItem(item);

            // Assert
            Assert.Single(prescription.Items);
        }

        [Fact]
        public void AddItem_WhenInactive_ThrowsInvalidOperationException()
        {
            // Arrange
            var prescription = CreateValidPrescription();
            prescription.Deactivate();
            var item = CreateValidItem(prescription.Id);

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => prescription.AddItem(item));
        }

        [Fact]
        public void SignPrescription_WithValidSignature_SignsPrescription()
        {
            // Arrange
            var prescription = CreateValidPrescription();
            var item = CreateValidItem(prescription.Id);
            prescription.AddItem(item);
            var signature = "BASE64_SIGNATURE";
            var certificate = "CERT_THUMBPRINT";

            // Act
            prescription.SignPrescription(signature, certificate);

            // Assert
            Assert.Equal(signature, prescription.DigitalSignature);
            Assert.Equal(certificate, prescription.SignatureCertificate);
            Assert.NotNull(prescription.SignedAt);
        }

        [Fact]
        public void SignPrescription_WithoutItems_ThrowsInvalidOperationException()
        {
            // Arrange
            var prescription = CreateValidPrescription();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                prescription.SignPrescription("SIG", "CERT"));
        }

        [Fact]
        public void SignPrescription_WhenAlreadySigned_ThrowsInvalidOperationException()
        {
            // Arrange
            var prescription = CreateValidPrescription();
            var item = CreateValidItem(prescription.Id);
            prescription.AddItem(item);
            prescription.SignPrescription("SIG1", "CERT1");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                prescription.SignPrescription("SIG2", "CERT2"));
        }

        [Fact]
        public void MarkAsReportedToSNGPC_WhenRequiresSNGPC_UpdatesReportedDate()
        {
            // Arrange
            var prescription = new DigitalPrescription(
                _medicalRecordId, _patientId, _doctorId,
                PrescriptionType.SpecialControlB,
                "Dr. João", "12345", "SP",
                "Maria", "12345678900", _tenantId);

            // Act
            prescription.MarkAsReportedToSNGPC();

            // Assert
            Assert.NotNull(prescription.ReportedToSNGPCAt);
        }

        [Fact]
        public void MarkAsReportedToSNGPC_WhenNotRequired_ThrowsInvalidOperationException()
        {
            // Arrange
            var prescription = CreateValidPrescription();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() =>
                prescription.MarkAsReportedToSNGPC());
        }

        [Fact]
        public void Deactivate_ChangesIsActiveToFalse()
        {
            // Arrange
            var prescription = CreateValidPrescription();

            // Act
            prescription.Deactivate();

            // Assert
            Assert.False(prescription.IsActive);
        }

        [Fact]
        public void IsValid_WithActiveNonExpiredPrescriptionAndItems_ReturnsTrue()
        {
            // Arrange
            var prescription = CreateValidPrescription();
            var item = CreateValidItem(prescription.Id);
            prescription.AddItem(item);

            // Act & Assert
            Assert.True(prescription.IsValid());
        }

        [Fact]
        public void IsValid_WhenInactive_ReturnsFalse()
        {
            // Arrange
            var prescription = CreateValidPrescription();
            var item = CreateValidItem(prescription.Id);
            prescription.AddItem(item);
            prescription.Deactivate();

            // Act & Assert
            Assert.False(prescription.IsValid());
        }

        [Fact]
        public void ExpirationDate_ForSimplePrescription_Is30Days()
        {
            // Arrange & Act
            var prescription = CreateValidPrescription();

            // Assert
            var expectedExpiration = prescription.IssuedAt.AddDays(30);
            Assert.Equal(expectedExpiration.Date, prescription.ExpiresAt.Date);
        }

        [Fact]
        public void ExpirationDate_ForAntimicrobial_Is10Days()
        {
            // Arrange & Act
            var prescription = new DigitalPrescription(
                _medicalRecordId, _patientId, _doctorId,
                PrescriptionType.Antimicrobial,
                "Dr. João", "12345", "SP",
                "Maria", "12345678900", _tenantId);

            // Assert
            var expectedExpiration = prescription.IssuedAt.AddDays(10);
            Assert.Equal(expectedExpiration.Date, prescription.ExpiresAt.Date);
        }

        // Helper methods
        private DigitalPrescription CreateValidPrescription()
        {
            return new DigitalPrescription(
                _medicalRecordId,
                _patientId,
                _doctorId,
                PrescriptionType.Simple,
                "Dr. João Silva",
                "12345",
                "SP",
                "Maria Santos",
                "12345678900",
                _tenantId
            );
        }

        private DigitalPrescriptionItem CreateValidItem(Guid prescriptionId)
        {
            return new DigitalPrescriptionItem(
                prescriptionId,
                Guid.NewGuid(),
                "Paracetamol 500mg",
                "500mg",
                "Comprimido",
                "8 em 8 horas",
                7,
                21,
                _tenantId
            );
        }
    }
}
