using System;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class WaitingQueueEntryTests
    {
        private readonly string _tenantId = "test-tenant";
        private readonly Guid _appointmentId = Guid.NewGuid();
        private readonly Guid _clinicId = Guid.NewGuid();
        private readonly Guid _patientId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidData_CreatesEntry()
        {
            // Arrange & Act
            var entry = new WaitingQueueEntry(
                _appointmentId,
                _clinicId,
                _patientId,
                TriagePriority.Normal,
                _tenantId,
                "Test notes"
            );

            // Assert
            Assert.NotEqual(Guid.Empty, entry.Id);
            Assert.Equal(_appointmentId, entry.AppointmentId);
            Assert.Equal(_clinicId, entry.ClinicId);
            Assert.Equal(_patientId, entry.PatientId);
            Assert.Equal(TriagePriority.Normal, entry.Priority);
            Assert.Equal(QueueStatus.Waiting, entry.Status);
            Assert.Equal("Test notes", entry.TriageNotes);
            Assert.Equal(0, entry.Position);
            Assert.Equal(0, entry.EstimatedWaitTimeMinutes);
        }

        [Fact]
        public void Constructor_WithEmptyAppointmentId_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new WaitingQueueEntry(Guid.Empty, _clinicId, _patientId, TriagePriority.Normal, _tenantId));

            Assert.Contains("agendamento", exception.Message.ToLower());
        }

        [Fact]
        public void Constructor_WithEmptyClinicId_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new WaitingQueueEntry(_appointmentId, Guid.Empty, _patientId, TriagePriority.Normal, _tenantId));

            Assert.Contains("clínica", exception.Message.ToLower());
        }

        [Fact]
        public void Constructor_WithEmptyPatientId_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new WaitingQueueEntry(_appointmentId, _clinicId, Guid.Empty, TriagePriority.Normal, _tenantId));

            Assert.Contains("paciente", exception.Message.ToLower());
        }

        [Fact]
        public void UpdatePriority_WhenWaiting_UpdatesSuccessfully()
        {
            // Arrange
            var entry = CreateValidEntry();

            // Act
            entry.UpdatePriority(TriagePriority.Urgent, "Now urgent");

            // Assert
            Assert.Equal(TriagePriority.Urgent, entry.Priority);
            Assert.Equal("Now urgent", entry.TriageNotes);
        }

        [Fact]
        public void UpdatePriority_WhenNotWaiting_ThrowsInvalidOperationException()
        {
            // Arrange
            var entry = CreateValidEntry();
            entry.Call();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                entry.UpdatePriority(TriagePriority.Urgent));

            Assert.Contains("aguardando", exception.Message.ToLower());
        }

        [Fact]
        public void UpdatePosition_WithValidPosition_UpdatesSuccessfully()
        {
            // Arrange
            var entry = CreateValidEntry();

            // Act
            entry.UpdatePosition(5);

            // Assert
            Assert.Equal(5, entry.Position);
        }

        [Fact]
        public void UpdatePosition_WithNegativePosition_ThrowsArgumentException()
        {
            // Arrange
            var entry = CreateValidEntry();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                entry.UpdatePosition(-1));

            Assert.Contains("negativa", exception.Message.ToLower());
        }

        [Fact]
        public void UpdateEstimatedWaitTime_WithValidTime_UpdatesSuccessfully()
        {
            // Arrange
            var entry = CreateValidEntry();

            // Act
            entry.UpdateEstimatedWaitTime(30);

            // Assert
            Assert.Equal(30, entry.EstimatedWaitTimeMinutes);
        }

        [Fact]
        public void UpdateEstimatedWaitTime_WithNegativeTime_ThrowsArgumentException()
        {
            // Arrange
            var entry = CreateValidEntry();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                entry.UpdateEstimatedWaitTime(-10));

            Assert.Contains("negativo", exception.Message.ToLower());
        }

        [Fact]
        public void Call_WhenWaiting_ChangesStatusToCalled()
        {
            // Arrange
            var entry = CreateValidEntry();

            // Act
            entry.Call();

            // Assert
            Assert.Equal(QueueStatus.Called, entry.Status);
            Assert.NotNull(entry.CalledTime);
        }

        [Fact]
        public void Call_WhenNotWaiting_ThrowsInvalidOperationException()
        {
            // Arrange
            var entry = CreateValidEntry();
            entry.Call();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                entry.Call());

            Assert.Contains("aguardando", exception.Message.ToLower());
        }

        [Fact]
        public void StartService_WhenCalled_ChangesStatusToInProgress()
        {
            // Arrange
            var entry = CreateValidEntry();
            entry.Call();

            // Act
            entry.StartService();

            // Assert
            Assert.Equal(QueueStatus.InProgress, entry.Status);
        }

        [Fact]
        public void StartService_WhenNotCalled_ThrowsInvalidOperationException()
        {
            // Arrange
            var entry = CreateValidEntry();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                entry.StartService());

            Assert.Contains("chamadas", exception.Message.ToLower());
        }

        [Fact]
        public void Complete_WhenInProgress_ChangesStatusToCompleted()
        {
            // Arrange
            var entry = CreateValidEntry();
            entry.Call();
            entry.StartService();

            // Act
            entry.Complete();

            // Assert
            Assert.Equal(QueueStatus.Completed, entry.Status);
            Assert.NotNull(entry.CompletedTime);
        }

        [Fact]
        public void Complete_WhenNotInProgress_ThrowsInvalidOperationException()
        {
            // Arrange
            var entry = CreateValidEntry();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                entry.Complete());

            Assert.Contains("atendimento", exception.Message.ToLower());
        }

        [Fact]
        public void Cancel_WhenWaiting_ChangesStatusToCancelled()
        {
            // Arrange
            var entry = CreateValidEntry();

            // Act
            entry.Cancel();

            // Assert
            Assert.Equal(QueueStatus.Cancelled, entry.Status);
        }

        [Fact]
        public void Cancel_WhenCompleted_ThrowsInvalidOperationException()
        {
            // Arrange
            var entry = CreateValidEntry();
            entry.Call();
            entry.StartService();
            entry.Complete();

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                entry.Cancel());

            Assert.Contains("completadas", exception.Message.ToLower());
        }

        [Fact]
        public void GetWaitingTime_ReturnsCorrectDuration()
        {
            // Arrange
            var entry = CreateValidEntry();
            
            // Simulate some wait time by setting CheckInTime in the past
            var checkInTime = DateTime.UtcNow.AddMinutes(-5);
            typeof(WaitingQueueEntry).GetProperty("CheckInTime")!.SetValue(entry, checkInTime);

            // Act
            var waitingTime = entry.GetWaitingTime();

            // Assert
            Assert.True(waitingTime.TotalMinutes >= 4.5 && waitingTime.TotalMinutes <= 5.5);
        }

        [Fact]
        public void IsActive_WhenWaiting_ReturnsTrue()
        {
            // Arrange
            var entry = CreateValidEntry();

            // Act & Assert
            Assert.True(entry.IsActive());
        }

        [Fact]
        public void IsActive_WhenCalled_ReturnsTrue()
        {
            // Arrange
            var entry = CreateValidEntry();
            entry.Call();

            // Act & Assert
            Assert.True(entry.IsActive());
        }

        [Fact]
        public void IsActive_WhenInProgress_ReturnsTrue()
        {
            // Arrange
            var entry = CreateValidEntry();
            entry.Call();
            entry.StartService();

            // Act & Assert
            Assert.True(entry.IsActive());
        }

        [Fact]
        public void IsActive_WhenCompleted_ReturnsFalse()
        {
            // Arrange
            var entry = CreateValidEntry();
            entry.Call();
            entry.StartService();
            entry.Complete();

            // Act & Assert
            Assert.False(entry.IsActive());
        }

        [Fact]
        public void IsActive_WhenCancelled_ReturnsFalse()
        {
            // Arrange
            var entry = CreateValidEntry();
            entry.Cancel();

            // Act & Assert
            Assert.False(entry.IsActive());
        }

        [Fact]
        public void FullWorkflow_FromWaitingToCompleted_WorksCorrectly()
        {
            // Arrange
            var entry = CreateValidEntry();

            // Act & Assert
            Assert.Equal(QueueStatus.Waiting, entry.Status);
            
            entry.UpdatePriority(TriagePriority.High, "Changed to high");
            Assert.Equal(TriagePriority.High, entry.Priority);
            
            entry.UpdatePosition(1);
            Assert.Equal(1, entry.Position);
            
            entry.Call();
            Assert.Equal(QueueStatus.Called, entry.Status);
            
            entry.StartService();
            Assert.Equal(QueueStatus.InProgress, entry.Status);
            
            entry.Complete();
            Assert.Equal(QueueStatus.Completed, entry.Status);
            Assert.False(entry.IsActive());
        }

        private WaitingQueueEntry CreateValidEntry()
        {
            return new WaitingQueueEntry(
                _appointmentId,
                _clinicId,
                _patientId,
                TriagePriority.Normal,
                _tenantId
            );
        }
    }
}
