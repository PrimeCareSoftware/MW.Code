using System;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class MedicalRecordTests
    {
        private readonly string _tenantId = "test-tenant";
        private readonly Guid _appointmentId = Guid.NewGuid();
        private readonly Guid _patientId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidData_CreatesMedicalRecord()
        {
            // Arrange
            var consultationStartTime = DateTime.UtcNow;

            // Act
            var record = new MedicalRecord(_appointmentId, _patientId, _tenantId, consultationStartTime);

            // Assert
            Assert.NotEqual(Guid.Empty, record.Id);
            Assert.Equal(_appointmentId, record.AppointmentId);
            Assert.Equal(_patientId, record.PatientId);
            Assert.Equal(consultationStartTime, record.ConsultationStartTime);
            Assert.Equal(string.Empty, record.Diagnosis);
            Assert.Equal(string.Empty, record.Prescription);
            Assert.Equal(string.Empty, record.Notes);
            Assert.Equal(0, record.ConsultationDurationMinutes);
            Assert.Null(record.ConsultationEndTime);
        }

        [Fact]
        public void Constructor_WithOptionalFields_CreatesMedicalRecord()
        {
            // Arrange
            var consultationStartTime = DateTime.UtcNow;
            var diagnosis = "Common cold";
            var prescription = "Rest and fluids";
            var notes = "Patient feeling better";

            // Act
            var record = new MedicalRecord(_appointmentId, _patientId, _tenantId, 
                consultationStartTime, diagnosis, prescription, notes);

            // Assert
            Assert.Equal(diagnosis, record.Diagnosis);
            Assert.Equal(prescription, record.Prescription);
            Assert.Equal(notes, record.Notes);
        }

        [Fact]
        public void Constructor_WithEmptyAppointmentId_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new MedicalRecord(Guid.Empty, _patientId, _tenantId, DateTime.UtcNow));

            Assert.Equal("Appointment ID cannot be empty (Parameter 'appointmentId')", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyPatientId_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new MedicalRecord(_appointmentId, Guid.Empty, _tenantId, DateTime.UtcNow));

            Assert.Equal("Patient ID cannot be empty (Parameter 'patientId')", exception.Message);
        }

        [Fact]
        public void UpdateDiagnosis_UpdatesDiagnosisField()
        {
            // Arrange
            var record = CreateValidMedicalRecord();
            var newDiagnosis = "Flu with complications";

            // Act
            record.UpdateDiagnosis(newDiagnosis);

            // Assert
            Assert.Equal(newDiagnosis, record.Diagnosis);
            Assert.NotNull(record.UpdatedAt);
        }

        [Fact]
        public void UpdateDiagnosis_WithNull_SetsEmptyString()
        {
            // Arrange
            var record = CreateValidMedicalRecord();

            // Act
            record.UpdateDiagnosis(null);

            // Assert
            Assert.Equal(string.Empty, record.Diagnosis);
        }

        [Fact]
        public void UpdatePrescription_UpdatesPrescriptionField()
        {
            // Arrange
            var record = CreateValidMedicalRecord();
            var newPrescription = "Antibiotics for 7 days";

            // Act
            record.UpdatePrescription(newPrescription);

            // Assert
            Assert.Equal(newPrescription, record.Prescription);
            Assert.NotNull(record.UpdatedAt);
        }

        [Fact]
        public void UpdatePrescription_WithNull_SetsEmptyString()
        {
            // Arrange
            var record = CreateValidMedicalRecord();

            // Act
            record.UpdatePrescription(null);

            // Assert
            Assert.Equal(string.Empty, record.Prescription);
        }

        [Fact]
        public void UpdateNotes_UpdatesNotesField()
        {
            // Arrange
            var record = CreateValidMedicalRecord();
            var newNotes = "Follow-up in 2 weeks";

            // Act
            record.UpdateNotes(newNotes);

            // Assert
            Assert.Equal(newNotes, record.Notes);
            Assert.NotNull(record.UpdatedAt);
        }

        [Fact]
        public void UpdateNotes_WithNull_SetsEmptyString()
        {
            // Arrange
            var record = CreateValidMedicalRecord();

            // Act
            record.UpdateNotes(null);

            // Assert
            Assert.Equal(string.Empty, record.Notes);
        }

        [Fact]
        public void CompleteConsultation_SetsEndTimeAndCalculatesDuration()
        {
            // Arrange
            var startTime = DateTime.UtcNow.AddMinutes(-30);
            var record = new MedicalRecord(_appointmentId, _patientId, _tenantId, startTime);

            // Act
            record.CompleteConsultation("Diagnosis", "Prescription", "Notes");

            // Assert
            Assert.NotNull(record.ConsultationEndTime);
            Assert.True(record.ConsultationDurationMinutes >= 29 && record.ConsultationDurationMinutes <= 31);
            Assert.Equal("Diagnosis", record.Diagnosis);
            Assert.Equal("Prescription", record.Prescription);
            Assert.Equal("Notes", record.Notes);
            Assert.NotNull(record.UpdatedAt);
        }

        [Fact]
        public void CompleteConsultation_WithoutOptionalFields_OnlySetsEndTime()
        {
            // Arrange
            var record = CreateValidMedicalRecord();
            var originalDiagnosis = "Original diagnosis";
            record.UpdateDiagnosis(originalDiagnosis);

            // Act
            record.CompleteConsultation();

            // Assert
            Assert.NotNull(record.ConsultationEndTime);
            Assert.Equal(originalDiagnosis, record.Diagnosis);
        }

        [Fact]
        public void CompleteConsultation_WithNullValues_DoesNotUpdateFields()
        {
            // Arrange
            var record = CreateValidMedicalRecord();
            var originalDiagnosis = "Original";
            record.UpdateDiagnosis(originalDiagnosis);

            // Act
            record.CompleteConsultation(null, null, null);

            // Assert
            Assert.Equal(originalDiagnosis, record.Diagnosis);
        }

        [Fact]
        public void CompleteConsultation_WithWhitespace_DoesNotUpdateFields()
        {
            // Arrange
            var record = CreateValidMedicalRecord();
            var originalDiagnosis = "Original";
            record.UpdateDiagnosis(originalDiagnosis);

            // Act
            record.CompleteConsultation("   ", "   ", "   ");

            // Assert
            Assert.Equal(originalDiagnosis, record.Diagnosis);
        }

        [Fact]
        public void UpdateConsultationTime_UpdatesDuration()
        {
            // Arrange
            var record = CreateValidMedicalRecord();
            var duration = 45;

            // Act
            record.UpdateConsultationTime(duration);

            // Assert
            Assert.Equal(duration, record.ConsultationDurationMinutes);
            Assert.NotNull(record.UpdatedAt);
        }

        [Fact]
        public void UpdateConsultationTime_WithNegativeDuration_ThrowsArgumentException()
        {
            // Arrange
            var record = CreateValidMedicalRecord();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => record.UpdateConsultationTime(-10));
            Assert.Equal("Duration cannot be negative (Parameter 'durationMinutes')", exception.Message);
        }

        [Fact]
        public void UpdateConsultationTime_WithZero_SetsToZero()
        {
            // Arrange
            var record = CreateValidMedicalRecord();

            // Act
            record.UpdateConsultationTime(0);

            // Assert
            Assert.Equal(0, record.ConsultationDurationMinutes);
        }

        [Fact]
        public void Constructor_TrimsWhitespaceFromOptionalFields()
        {
            // Arrange
            var diagnosis = "  Diagnosis  ";
            var prescription = "  Prescription  ";
            var notes = "  Notes  ";

            // Act
            var record = new MedicalRecord(_appointmentId, _patientId, _tenantId, 
                DateTime.UtcNow, diagnosis, prescription, notes);

            // Assert
            Assert.Equal("Diagnosis", record.Diagnosis);
            Assert.Equal("Prescription", record.Prescription);
            Assert.Equal("Notes", record.Notes);
        }

        [Fact]
        public void UpdateDiagnosis_TrimsWhitespace()
        {
            // Arrange
            var record = CreateValidMedicalRecord();
            var diagnosis = "  New Diagnosis  ";

            // Act
            record.UpdateDiagnosis(diagnosis);

            // Assert
            Assert.Equal("New Diagnosis", record.Diagnosis);
        }

        [Fact]
        public void UpdatePrescription_TrimsWhitespace()
        {
            // Arrange
            var record = CreateValidMedicalRecord();
            var prescription = "  New Prescription  ";

            // Act
            record.UpdatePrescription(prescription);

            // Assert
            Assert.Equal("New Prescription", record.Prescription);
        }

        [Fact]
        public void UpdateNotes_TrimsWhitespace()
        {
            // Arrange
            var record = CreateValidMedicalRecord();
            var notes = "  New Notes  ";

            // Act
            record.UpdateNotes(notes);

            // Assert
            Assert.Equal("New Notes", record.Notes);
        }

        [Fact]
        public void CompleteConsultation_TrimsWhitespace()
        {
            // Arrange
            var record = CreateValidMedicalRecord();

            // Act
            record.CompleteConsultation("  Diagnosis  ", "  Prescription  ", "  Notes  ");

            // Assert
            Assert.Equal("Diagnosis", record.Diagnosis);
            Assert.Equal("Prescription", record.Prescription);
            Assert.Equal("Notes", record.Notes);
        }

        private MedicalRecord CreateValidMedicalRecord()
        {
            return new MedicalRecord(_appointmentId, _patientId, _tenantId, DateTime.UtcNow);
        }
    }
}
