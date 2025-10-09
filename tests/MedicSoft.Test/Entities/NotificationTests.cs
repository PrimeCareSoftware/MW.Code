using System;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class NotificationTests
    {
        private readonly string _tenantId = "test-tenant";
        private readonly Guid _patientId = Guid.NewGuid();
        private readonly Guid _appointmentId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidData_CreatesNotification()
        {
            // Arrange
            var type = NotificationType.AppointmentReminder;
            var channel = NotificationChannel.SMS;
            var recipient = "+5511999999999";
            var message = "Lembrete: Você tem consulta amanhã às 10h";

            // Act
            var notification = new Notification(_patientId, type, channel, recipient, message, _tenantId, _appointmentId);

            // Assert
            Assert.NotEqual(Guid.Empty, notification.Id);
            Assert.Equal(_patientId, notification.PatientId);
            Assert.Equal(_appointmentId, notification.AppointmentId);
            Assert.Equal(type, notification.Type);
            Assert.Equal(channel, notification.Channel);
            Assert.Equal(recipient, notification.Recipient);
            Assert.Equal(message, notification.Message);
            Assert.Equal(NotificationStatus.Pending, notification.Status);
            Assert.Equal(0, notification.RetryCount);
            Assert.Null(notification.SentAt);
        }

        [Fact]
        public void Constructor_WithoutAppointment_CreatesNotification()
        {
            // Arrange & Act
            var notification = new Notification(_patientId, NotificationType.General,
                NotificationChannel.Email, "patient@email.com", "Test message", _tenantId);

            // Assert
            Assert.Null(notification.AppointmentId);
        }

        [Fact]
        public void Constructor_WithEmptyPatientId_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Notification(Guid.Empty, NotificationType.General,
                    NotificationChannel.SMS, "+5511999999999", "Message", _tenantId));
        }

        [Fact]
        public void Constructor_WithEmptyRecipient_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Notification(_patientId, NotificationType.General,
                    NotificationChannel.SMS, "", "Message", _tenantId));
        }

        [Fact]
        public void Constructor_WithEmptyMessage_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Notification(_patientId, NotificationType.General,
                    NotificationChannel.SMS, "+5511999999999", "", _tenantId));
        }

        [Fact]
        public void MarkAsSent_UpdatesStatusAndSentAt()
        {
            // Arrange
            var notification = CreateValidNotification();
            var beforeSent = DateTime.UtcNow;

            // Act
            notification.MarkAsSent();

            // Assert
            Assert.Equal(NotificationStatus.Sent, notification.Status);
            Assert.NotNull(notification.SentAt);
            Assert.True(notification.SentAt.Value >= beforeSent);
            Assert.NotNull(notification.UpdatedAt);
        }

        [Fact]
        public void MarkAsDelivered_UpdatesStatusAndDeliveredAt()
        {
            // Arrange
            var notification = CreateValidNotification();
            notification.MarkAsSent();

            // Act
            notification.MarkAsDelivered();

            // Assert
            Assert.Equal(NotificationStatus.Delivered, notification.Status);
            Assert.NotNull(notification.DeliveredAt);
        }

        [Fact]
        public void MarkAsRead_UpdatesStatusAndReadAt()
        {
            // Arrange
            var notification = CreateValidNotification();
            notification.MarkAsSent();
            notification.MarkAsDelivered();

            // Act
            notification.MarkAsRead();

            // Assert
            Assert.Equal(NotificationStatus.Read, notification.Status);
            Assert.NotNull(notification.ReadAt);
        }

        [Fact]
        public void MarkAsFailed_WithErrorMessage_UpdatesStatusAndError()
        {
            // Arrange
            var notification = CreateValidNotification();
            var errorMessage = "SMS delivery failed: Invalid phone number";

            // Act
            notification.MarkAsFailed(errorMessage);

            // Assert
            Assert.Equal(NotificationStatus.Failed, notification.Status);
            Assert.Equal(errorMessage, notification.ErrorMessage);
            Assert.NotNull(notification.UpdatedAt);
        }

        [Fact]
        public void MarkAsFailed_WithEmptyErrorMessage_ThrowsArgumentException()
        {
            // Arrange
            var notification = CreateValidNotification();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => notification.MarkAsFailed(""));
        }

        [Fact]
        public void IncrementRetryCount_IncrementsCountAndResetsToPending()
        {
            // Arrange
            var notification = CreateValidNotification();
            notification.MarkAsFailed("Error");

            // Act
            notification.IncrementRetryCount();

            // Assert
            Assert.Equal(1, notification.RetryCount);
            Assert.Equal(NotificationStatus.Pending, notification.Status);
        }

        [Fact]
        public void CanRetry_WhenFailedAndLessThan3Retries_ReturnsTrue()
        {
            // Arrange
            var notification = CreateValidNotification();
            notification.MarkAsFailed("Error");

            // Act & Assert
            Assert.True(notification.CanRetry());
        }

        [Fact]
        public void CanRetry_WhenRetryCountIs3_ReturnsFalse()
        {
            // Arrange
            var notification = CreateValidNotification();
            notification.MarkAsFailed("Error");
            notification.IncrementRetryCount();
            notification.MarkAsFailed("Error");
            notification.IncrementRetryCount();
            notification.MarkAsFailed("Error");
            notification.IncrementRetryCount();

            // Act & Assert
            Assert.False(notification.CanRetry());
        }

        [Fact]
        public void CanRetry_WhenStatusIsSent_ReturnsFalse()
        {
            // Arrange
            var notification = CreateValidNotification();
            notification.MarkAsSent();

            // Act & Assert
            Assert.False(notification.CanRetry());
        }

        [Fact]
        public void NotificationType_HasAllExpectedValues()
        {
            // Assert
            Assert.True(Enum.IsDefined(typeof(NotificationType), NotificationType.AppointmentReminder));
            Assert.True(Enum.IsDefined(typeof(NotificationType), NotificationType.AppointmentConfirmation));
            Assert.True(Enum.IsDefined(typeof(NotificationType), NotificationType.AppointmentCancellation));
            Assert.True(Enum.IsDefined(typeof(NotificationType), NotificationType.AppointmentRescheduled));
            Assert.True(Enum.IsDefined(typeof(NotificationType), NotificationType.General));
        }

        [Fact]
        public void NotificationChannel_HasAllExpectedValues()
        {
            // Assert
            Assert.True(Enum.IsDefined(typeof(NotificationChannel), NotificationChannel.SMS));
            Assert.True(Enum.IsDefined(typeof(NotificationChannel), NotificationChannel.WhatsApp));
            Assert.True(Enum.IsDefined(typeof(NotificationChannel), NotificationChannel.Email));
            Assert.True(Enum.IsDefined(typeof(NotificationChannel), NotificationChannel.Push));
        }

        [Fact]
        public void NotificationStatus_HasAllExpectedValues()
        {
            // Assert
            Assert.True(Enum.IsDefined(typeof(NotificationStatus), NotificationStatus.Pending));
            Assert.True(Enum.IsDefined(typeof(NotificationStatus), NotificationStatus.Sent));
            Assert.True(Enum.IsDefined(typeof(NotificationStatus), NotificationStatus.Delivered));
            Assert.True(Enum.IsDefined(typeof(NotificationStatus), NotificationStatus.Read));
            Assert.True(Enum.IsDefined(typeof(NotificationStatus), NotificationStatus.Failed));
        }

        [Fact]
        public void Constructor_TrimsWhitespaceFromStrings()
        {
            // Arrange & Act
            var notification = new Notification(_patientId, NotificationType.General,
                NotificationChannel.SMS, "  +5511999999999  ", "  Test message  ", _tenantId);

            // Assert
            Assert.Equal("+5511999999999", notification.Recipient);
            Assert.Equal("Test message", notification.Message);
        }

        private Notification CreateValidNotification()
        {
            return new Notification(_patientId, NotificationType.AppointmentReminder,
                NotificationChannel.SMS, "+5511999999999",
                "Lembrete: Você tem consulta amanhã", _tenantId, _appointmentId);
        }
    }
}
