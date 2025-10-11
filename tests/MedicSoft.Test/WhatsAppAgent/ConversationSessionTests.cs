using System;
using Xunit;
using MedicSoft.WhatsAppAgent.Entities;

namespace MedicSoft.Test.WhatsAppAgent
{
    public class ConversationSessionTests
    {
        [Fact]
        public void Constructor_WithValidData_ShouldCreateSession()
        {
            // Arrange
            var configId = Guid.NewGuid();
            var tenantId = "tenant-123";
            var phoneNumber = "+5511999999999";
            var userName = "João Silva";

            // Act
            var session = new ConversationSession(configId, tenantId, phoneNumber, userName);

            // Assert
            Assert.NotEqual(Guid.Empty, session.Id);
            Assert.Equal(configId, session.ConfigurationId);
            Assert.Equal(tenantId, session.TenantId);
            Assert.Equal(phoneNumber, session.UserPhoneNumber);
            Assert.Equal(userName, session.UserName);
            Assert.True(session.IsActive);
            Assert.Equal(0, session.MessageCountCurrentHour);
            Assert.Equal("Initial", session.State);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_WithInvalidTenantId_ShouldThrowException(string tenantId)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new ConversationSession(Guid.NewGuid(), tenantId, "+5511999999999"));
        }

        [Fact]
        public void Constructor_WithEmptyConfigurationId_ShouldThrowException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                new ConversationSession(Guid.Empty, "tenant-123", "+5511999999999"));
        }

        [Fact]
        public void CanSendMessage_WithinLimit_ShouldReturnTrue()
        {
            // Arrange
            var session = CreateValidSession();
            var maxMessages = 20;

            // Act
            var canSend = session.CanSendMessage(maxMessages);

            // Assert
            Assert.True(canSend);
        }

        [Fact]
        public void CanSendMessage_ExceedingLimit_ShouldReturnFalse()
        {
            // Arrange
            var session = CreateValidSession();
            var maxMessages = 5;

            // Send max messages
            for (int i = 0; i < maxMessages; i++)
            {
                session.IncrementMessageCount();
            }

            // Act
            var canSend = session.CanSendMessage(maxMessages);

            // Assert
            Assert.False(canSend);
        }

        [Fact]
        public void IncrementMessageCount_ShouldIncreaseCount()
        {
            // Arrange
            var session = CreateValidSession();
            Assert.Equal(0, session.MessageCountCurrentHour);

            // Act
            session.IncrementMessageCount();

            // Assert
            Assert.Equal(1, session.MessageCountCurrentHour);
        }

        [Fact]
        public void UpdateContext_WithValidContext_ShouldUpdateContext()
        {
            // Arrange
            var session = CreateValidSession();
            var newContext = "{\"messages\": []}";

            // Act
            session.UpdateContext(newContext);

            // Assert
            Assert.Equal(newContext, session.Context);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void UpdateContext_WithInvalidContext_ShouldThrowException(string context)
        {
            // Arrange
            var session = CreateValidSession();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => session.UpdateContext(context));
        }

        [Fact]
        public void UpdateState_WithValidState_ShouldUpdateState()
        {
            // Arrange
            var session = CreateValidSession();
            var newState = "AwaitingConfirmation";

            // Act
            session.UpdateState(newState);

            // Assert
            Assert.Equal(newState, session.State);
        }

        [Fact]
        public void ExtendExpiration_ShouldUpdateExpiresAt()
        {
            // Arrange
            var session = CreateValidSession();
            var originalExpiration = session.ExpiresAt;
            System.Threading.Thread.Sleep(100);

            // Act
            session.ExtendExpiration();

            // Assert
            Assert.True(session.ExpiresAt > originalExpiration);
        }

        [Fact]
        public void EndSession_ShouldSetIsActiveToFalse()
        {
            // Arrange
            var session = CreateValidSession();
            Assert.True(session.IsActive);

            // Act
            session.EndSession();

            // Assert
            Assert.False(session.IsActive);
        }

        [Fact]
        public void IsExpired_WithFutureExpiration_ShouldReturnFalse()
        {
            // Arrange
            var session = CreateValidSession();

            // Act
            var isExpired = session.IsExpired();

            // Assert
            Assert.False(isExpired);
        }

        private ConversationSession CreateValidSession()
        {
            return new ConversationSession(
                Guid.NewGuid(),
                "tenant-123",
                "+5511999999999",
                "João Silva");
        }
    }
}
