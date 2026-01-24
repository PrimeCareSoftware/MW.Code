using System;
using System.Threading.Tasks;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class MedicalRecordVersioningTests
    {
        private readonly string _tenantId = "test-tenant";
        private readonly Guid _appointmentId = Guid.NewGuid();
        private readonly Guid _patientId = Guid.NewGuid();
        private readonly Guid _userId = Guid.NewGuid();

        [Fact]
        public void NewMedicalRecord_StartsAtVersion1()
        {
            // Arrange & Act
            var record = CreateValidMedicalRecord();

            // Assert
            Assert.Equal(1, record.CurrentVersion);
        }

        [Fact]
        public void CloseMedicalRecord_SetsIsClosedTrue()
        {
            // Arrange
            var record = CreateValidMedicalRecordWithExaminations();

            // Act
            record.CloseMedicalRecord(_userId);

            // Assert
            Assert.True(record.IsClosed);
            Assert.NotNull(record.ClosedAt);
            Assert.Equal(_userId, record.ClosedByUserId);
        }

        [Fact]
        public void CloseMedicalRecord_WithoutRequiredFields_ThrowsException()
        {
            // Arrange
            var record = CreateValidMedicalRecord(); // Without examinations

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => 
                record.CloseMedicalRecord(_userId));
            
            Assert.Contains("missing required fields", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void CloseMedicalRecord_WhenAlreadyClosed_ThrowsException()
        {
            // Arrange
            var record = CreateValidMedicalRecordWithExaminations();
            record.CloseMedicalRecord(_userId);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => 
                record.CloseMedicalRecord(_userId));
            
            Assert.Contains("already closed", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ReopenMedicalRecord_RequiresJustification()
        {
            // Arrange
            var record = CreateValidMedicalRecordWithExaminations();
            record.CloseMedicalRecord(_userId);

            // Act & Assert - null reason
            var exception1 = Assert.Throws<ArgumentException>(() => 
                record.ReopenMedicalRecord(_userId, null!));
            Assert.Contains("justification", exception1.Message, StringComparison.OrdinalIgnoreCase);

            // Act & Assert - empty reason
            var exception2 = Assert.Throws<ArgumentException>(() => 
                record.ReopenMedicalRecord(_userId, ""));
            Assert.Contains("justification", exception2.Message, StringComparison.OrdinalIgnoreCase);

            // Act & Assert - short reason
            var exception3 = Assert.Throws<ArgumentException>(() => 
                record.ReopenMedicalRecord(_userId, "Short"));
            Assert.Contains("20 characters", exception3.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void ReopenMedicalRecord_WithValidReason_SucceedsAndIncrementsVersion()
        {
            // Arrange
            var record = CreateValidMedicalRecordWithExaminations();
            record.CloseMedicalRecord(_userId);
            var versionBeforeReopen = record.CurrentVersion;
            var reason = "Need to add additional information that was missing";

            // Act
            record.ReopenMedicalRecord(_userId, reason);

            // Assert
            Assert.False(record.IsClosed);
            Assert.Null(record.ClosedAt);
            Assert.Null(record.ClosedByUserId);
            Assert.NotNull(record.ReopenedAt);
            Assert.Equal(_userId, record.ReopenedByUserId);
            Assert.Equal(reason, record.ReopenReason);
            Assert.Equal(versionBeforeReopen + 1, record.CurrentVersion);
        }

        [Fact]
        public void ReopenMedicalRecord_WhenNotClosed_ThrowsException()
        {
            // Arrange
            var record = CreateValidMedicalRecord();
            var reason = "Attempting to reopen an already open record";

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => 
                record.ReopenMedicalRecord(_userId, reason));
            
            Assert.Contains("not closed", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void UpdateClosedMedicalRecord_ThrowsException()
        {
            // Arrange
            var record = CreateValidMedicalRecordWithExaminations();
            record.CloseMedicalRecord(_userId);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => 
                record.UpdateChiefComplaint("New complaint after closing"));
            
            Assert.Contains("closed", exception.Message, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void UpdateAfterReopen_Succeeds()
        {
            // Arrange
            var record = CreateValidMedicalRecordWithExaminations();
            record.CloseMedicalRecord(_userId);
            var reason = "Need to add additional information that was missing";
            record.ReopenMedicalRecord(_userId, reason);
            var newComplaint = "Updated complaint after reopening the medical record";

            // Act
            record.UpdateChiefComplaint(newComplaint);

            // Assert
            Assert.Equal(newComplaint, record.ChiefComplaint);
        }

        [Fact]
        public void IncrementVersion_IncreasesVersionNumber()
        {
            // Arrange
            var record = CreateValidMedicalRecord();
            var initialVersion = record.CurrentVersion;

            // Act
            record.IncrementVersion();

            // Assert
            Assert.Equal(initialVersion + 1, record.CurrentVersion);
        }

        [Fact]
        public void FullWorkflow_CreateEditCloseReopenEdit_WorksCorrectly()
        {
            // 1. Create
            var record = CreateValidMedicalRecordWithExaminations();
            Assert.Equal(1, record.CurrentVersion);
            Assert.False(record.IsClosed);

            // 2. Edit
            record.UpdateChiefComplaint("Updated complaint during consultation");
            record.IncrementVersion();
            Assert.Equal(2, record.CurrentVersion);

            // 3. Close
            record.CloseMedicalRecord(_userId);
            Assert.True(record.IsClosed);

            // 4. Try to edit (should fail)
            Assert.Throws<InvalidOperationException>(() => 
                record.UpdateChiefComplaint("Trying to update closed record"));

            // 5. Reopen with justification
            var reason = "Patient returned with additional symptoms that need to be documented";
            record.ReopenMedicalRecord(_userId, reason);
            Assert.False(record.IsClosed);
            Assert.Equal(3, record.CurrentVersion);

            // 6. Edit after reopen
            record.UpdateNotes("Added notes after reopening");
            record.IncrementVersion();
            Assert.Equal(4, record.CurrentVersion);
        }

        private MedicalRecord CreateValidMedicalRecord()
        {
            return new MedicalRecord(
                _appointmentId, 
                _patientId, 
                _tenantId, 
                DateTime.UtcNow,
                "Patient complains of persistent cough",
                "Patient has been experiencing a persistent dry cough for the past week with no improvement.");
        }

        private MedicalRecord CreateValidMedicalRecordWithExaminations()
        {
            var record = CreateValidMedicalRecord();
            
            // Use reflection to add mock examinations, diagnoses, and plans
            // since they're read-only collections
            var examinationsField = typeof(MedicalRecord).GetField("_examinations", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var diagnosesField = typeof(MedicalRecord).GetField("_diagnoses", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var plansField = typeof(MedicalRecord).GetField("_plans", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            var examinations = examinationsField?.GetValue(record) as System.Collections.IList;
            var diagnoses = diagnosesField?.GetValue(record) as System.Collections.IList;
            var plans = plansField?.GetValue(record) as System.Collections.IList;

            // Add mock clinical examination
            var examination = new ClinicalExamination(
                record.Id,
                _tenantId,
                "Physical examination reveals clear lungs bilaterally",
                80,
                120,
                98.6m,
                16,
                98,
                "Good"
            );
            examinations?.Add(examination);

            // Add mock diagnostic hypothesis
            var diagnosis = new DiagnosticHypothesis(
                record.Id,
                _tenantId,
                "Acute bronchitis",
                "J20.9",
                DiagnosisType.Principal
            );
            diagnoses?.Add(diagnosis);

            // Add mock therapeutic plan
            var plan = new TherapeuticPlan(
                record.Id,
                _tenantId,
                "Rest and hydration. Symptomatic treatment.",
                "Cough suppressant as needed",
                null,
                null,
                "Drink plenty of fluids and rest",
                null
            );
            plans?.Add(plan);

            return record;
        }
    }
}
