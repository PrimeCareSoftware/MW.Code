using System;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class NotificationRoutineTests
    {
        private readonly string _tenantId = "test-tenant";

        [Fact]
        public void Constructor_WithValidData_CreatesNotificationRoutine()
        {
            // Arrange
            var name = "Daily Appointment Reminder";
            var description = "Send reminders for next-day appointments";
            var channel = NotificationChannel.WhatsApp;
            var type = NotificationType.AppointmentReminder;
            var messageTemplate = "Hello {patientName}, appointment at {time}";
            var scheduleType = RoutineScheduleType.Daily;
            var scheduleConfig = "{\"time\":\"18:00\"}";
            var scope = RoutineScope.Clinic;

            // Act
            var routine = new NotificationRoutine(
                name, description, channel, type, messageTemplate,
                scheduleType, scheduleConfig, scope, _tenantId
            );

            // Assert
            Assert.NotEqual(Guid.Empty, routine.Id);
            Assert.Equal(name, routine.Name);
            Assert.Equal(description, routine.Description);
            Assert.Equal(channel, routine.Channel);
            Assert.Equal(type, routine.Type);
            Assert.Equal(messageTemplate, routine.MessageTemplate);
            Assert.Equal(scheduleType, routine.ScheduleType);
            Assert.Equal(scheduleConfig, routine.ScheduleConfiguration);
            Assert.Equal(scope, routine.Scope);
            Assert.True(routine.IsActive);
            Assert.Equal(3, routine.MaxRetries);
            Assert.Equal(_tenantId, routine.TenantId);
        }

        [Fact]
        public void Constructor_WithCustomMaxRetries_CreatesNotificationRoutine()
        {
            // Arrange
            var maxRetries = 5;

            // Act
            var routine = CreateValidRoutine(maxRetries: maxRetries);

            // Assert
            Assert.Equal(maxRetries, routine.MaxRetries);
        }

        [Fact]
        public void Constructor_WithRecipientFilter_CreatesNotificationRoutine()
        {
            // Arrange
            var recipientFilter = "{\"hasAppointmentNextDay\":true}";

            // Act
            var routine = CreateValidRoutine(recipientFilter: recipientFilter);

            // Assert
            Assert.Equal(recipientFilter, routine.RecipientFilter);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_WithEmptyName_ThrowsArgumentException(string name)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new NotificationRoutine(
                    name, "description", NotificationChannel.SMS, NotificationType.General,
                    "template", RoutineScheduleType.Daily, "{}", RoutineScope.Clinic, _tenantId
                )
            );
            Assert.Equal("Name cannot be empty (Parameter 'name')", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_WithEmptyMessageTemplate_ThrowsArgumentException(string template)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new NotificationRoutine(
                    "name", "description", NotificationChannel.SMS, NotificationType.General,
                    template, RoutineScheduleType.Daily, "{}", RoutineScope.Clinic, _tenantId
                )
            );
            Assert.Equal("Message template cannot be empty (Parameter 'messageTemplate')", exception.Message);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_WithEmptyScheduleConfiguration_ThrowsArgumentException(string config)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new NotificationRoutine(
                    "name", "description", NotificationChannel.SMS, NotificationType.General,
                    "template", RoutineScheduleType.Daily, config, RoutineScope.Clinic, _tenantId
                )
            );
            Assert.Equal("Schedule configuration cannot be empty (Parameter 'scheduleConfiguration')", exception.Message);
        }

        [Theory]
        [InlineData(-1)]
        [InlineData(11)]
        public void Constructor_WithInvalidMaxRetries_ThrowsArgumentException(int maxRetries)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new NotificationRoutine(
                    "name", "description", NotificationChannel.SMS, NotificationType.General,
                    "template", RoutineScheduleType.Daily, "{}", RoutineScope.Clinic, 
                    _tenantId, maxRetries
                )
            );
            Assert.Equal("Max retries must be between 0 and 10 (Parameter 'maxRetries')", exception.Message);
        }

        [Fact]
        public void Update_WithValidData_UpdatesRoutine()
        {
            // Arrange
            var routine = CreateValidRoutine();
            var newName = "Updated Routine";
            var newDescription = "Updated Description";
            var newChannel = NotificationChannel.Email;
            var newType = NotificationType.PaymentReminder;
            var newTemplate = "New template";
            var newScheduleType = RoutineScheduleType.Weekly;
            var newConfig = "{\"day\":\"monday\"}";
            var newMaxRetries = 5;

            // Act
            routine.Update(newName, newDescription, newChannel, newType, 
                newTemplate, newScheduleType, newConfig, newMaxRetries);

            // Assert
            Assert.Equal(newName, routine.Name);
            Assert.Equal(newDescription, routine.Description);
            Assert.Equal(newChannel, routine.Channel);
            Assert.Equal(newType, routine.Type);
            Assert.Equal(newTemplate, routine.MessageTemplate);
            Assert.Equal(newScheduleType, routine.ScheduleType);
            Assert.Equal(newConfig, routine.ScheduleConfiguration);
            Assert.Equal(newMaxRetries, routine.MaxRetries);
            Assert.NotNull(routine.UpdatedAt);
        }

        [Fact]
        public void Activate_SetsIsActiveToTrue()
        {
            // Arrange
            var routine = CreateValidRoutine();
            routine.Deactivate();

            // Act
            routine.Activate();

            // Assert
            Assert.True(routine.IsActive);
            Assert.NotNull(routine.UpdatedAt);
        }

        [Fact]
        public void Deactivate_SetsIsActiveToFalse()
        {
            // Arrange
            var routine = CreateValidRoutine();

            // Act
            routine.Deactivate();

            // Assert
            Assert.False(routine.IsActive);
            Assert.NotNull(routine.UpdatedAt);
        }

        [Fact]
        public void MarkAsExecuted_UpdatesLastExecutedAt()
        {
            // Arrange
            var routine = CreateValidRoutine();
            var nextExecution = DateTime.UtcNow.AddDays(1);

            // Act
            routine.MarkAsExecuted(nextExecution);

            // Assert
            Assert.NotNull(routine.LastExecutedAt);
            Assert.Equal(nextExecution, routine.NextExecutionAt);
            Assert.NotNull(routine.UpdatedAt);
        }

        [Fact]
        public void SetNextExecution_WithFutureDate_UpdatesNextExecutionAt()
        {
            // Arrange
            var routine = CreateValidRoutine();
            var nextExecution = DateTime.UtcNow.AddDays(1);

            // Act
            routine.SetNextExecution(nextExecution);

            // Assert
            Assert.Equal(nextExecution, routine.NextExecutionAt);
            Assert.NotNull(routine.UpdatedAt);
        }

        [Fact]
        public void SetNextExecution_WithPastDate_ThrowsArgumentException()
        {
            // Arrange
            var routine = CreateValidRoutine();
            var pastDate = DateTime.UtcNow.AddDays(-1);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                routine.SetNextExecution(pastDate)
            );
            Assert.Equal("Next execution must be in the future (Parameter 'nextExecution')", exception.Message);
        }

        [Fact]
        public void ShouldExecute_WhenActiveAndNextExecutionIsNull_ReturnsTrue()
        {
            // Arrange
            var routine = CreateValidRoutine();

            // Act
            var result = routine.ShouldExecute();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ShouldExecute_WhenActiveAndNextExecutionIsPast_ReturnsTrue()
        {
            // Arrange
            var routine = CreateValidRoutine();
            routine.MarkAsExecuted(DateTime.UtcNow.AddSeconds(-1));

            // Act
            var result = routine.ShouldExecute();

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void ShouldExecute_WhenActiveAndNextExecutionIsFuture_ReturnsFalse()
        {
            // Arrange
            var routine = CreateValidRoutine();
            routine.SetNextExecution(DateTime.UtcNow.AddDays(1));

            // Act
            var result = routine.ShouldExecute();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ShouldExecute_WhenInactive_ReturnsFalse()
        {
            // Arrange
            var routine = CreateValidRoutine();
            routine.Deactivate();

            // Act
            var result = routine.ShouldExecute();

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void Constructor_WithSystemScope_CreatesRoutine()
        {
            // Arrange
            var scope = RoutineScope.System;

            // Act
            var routine = new NotificationRoutine(
                "System Routine", "System-wide routine", NotificationChannel.SMS,
                NotificationType.General, "template", RoutineScheduleType.Daily,
                "{}", scope, "system-admin"
            );

            // Assert
            Assert.Equal(scope, routine.Scope);
        }

        // Helper methods
        private NotificationRoutine CreateValidRoutine(
            int maxRetries = 3, 
            string? recipientFilter = null)
        {
            return new NotificationRoutine(
                "Test Routine",
                "Test Description",
                NotificationChannel.SMS,
                NotificationType.AppointmentReminder,
                "Test message template",
                RoutineScheduleType.Daily,
                "{\"time\":\"10:00\"}",
                RoutineScope.Clinic,
                _tenantId,
                maxRetries,
                recipientFilter
            );
        }
    }
}
