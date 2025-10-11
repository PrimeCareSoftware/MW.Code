using System;
using Xunit;
using MedicSoft.WhatsAppAgent.Entities;

namespace MedicSoft.Test.WhatsAppAgent
{
    public class WhatsAppAgentConfigurationTests
    {
        [Fact]
        public void Constructor_WithValidData_ShouldCreateConfiguration()
        {
            // Arrange
            var tenantId = "tenant-123";
            var clinicName = "Clínica ABC";
            var whatsAppNumber = "+5511999999999";
            var whatsAppApiKey = "whatsapp-key-123";
            var aiApiKey = "ai-key-456";
            var aiModel = "gpt-4";
            var systemPrompt = "You are a helpful scheduling assistant";

            // Act
            var config = new WhatsAppAgentConfiguration(
                tenantId, clinicName, whatsAppNumber, whatsAppApiKey, 
                aiApiKey, aiModel, systemPrompt);

            // Assert
            Assert.NotEqual(Guid.Empty, config.Id);
            Assert.Equal(tenantId, config.TenantId);
            Assert.Equal(clinicName, config.ClinicName);
            Assert.Equal(whatsAppNumber, config.WhatsAppNumber);
            Assert.Equal(whatsAppApiKey, config.WhatsAppApiKey);
            Assert.Equal(aiApiKey, config.AiApiKey);
            Assert.Equal(aiModel, config.AiModel);
            Assert.Equal(systemPrompt, config.SystemPrompt);
            Assert.False(config.IsActive);
            Assert.Equal(20, config.MaxMessagesPerHour);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData(" ")]
        public void Constructor_WithInvalidTenantId_ShouldThrowException(string tenantId)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new WhatsAppAgentConfiguration(
                tenantId, "Clinic", "+5511999999999", "key1", "key2", "gpt-4", "prompt"));
        }

        [Theory]
        [InlineData(0)]
        [InlineData(101)]
        [InlineData(-1)]
        public void Constructor_WithInvalidMaxMessages_ShouldThrowException(int maxMessages)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => new WhatsAppAgentConfiguration(
                "tenant", "Clinic", "+5511999999999", "key1", "key2", "gpt-4", "prompt", maxMessages));
        }

        [Fact]
        public void Activate_ShouldSetIsActiveToTrue()
        {
            // Arrange
            var config = CreateValidConfiguration();
            Assert.False(config.IsActive);

            // Act
            config.Activate();

            // Assert
            Assert.True(config.IsActive);
        }

        [Fact]
        public void Deactivate_ShouldSetIsActiveToFalse()
        {
            // Arrange
            var config = CreateValidConfiguration();
            config.Activate();
            Assert.True(config.IsActive);

            // Act
            config.Deactivate();

            // Assert
            Assert.False(config.IsActive);
        }

        [Fact]
        public void UpdateConfiguration_WithValidData_ShouldUpdateFields()
        {
            // Arrange
            var config = CreateValidConfiguration();
            var newClinicName = "Nova Clínica";
            var newSystemPrompt = "Updated prompt";
            var newMaxMessages = 30;

            // Act
            config.UpdateConfiguration(
                newClinicName, "+5511888888888", newSystemPrompt, 
                newMaxMessages, "09:00", "19:00", "Mon,Tue,Wed,Thu,Fri,Sat", "New fallback");

            // Assert
            Assert.Equal(newClinicName, config.ClinicName);
            Assert.Equal(newSystemPrompt, config.SystemPrompt);
            Assert.Equal(newMaxMessages, config.MaxMessagesPerHour);
        }

        [Fact]
        public void UpdateApiKeys_WithValidKeys_ShouldUpdateKeys()
        {
            // Arrange
            var config = CreateValidConfiguration();
            var newWhatsAppKey = "new-whatsapp-key";
            var newAiKey = "new-ai-key";

            // Act
            config.UpdateApiKeys(newWhatsAppKey, newAiKey);

            // Assert
            Assert.Equal(newWhatsAppKey, config.WhatsAppApiKey);
            Assert.Equal(newAiKey, config.AiApiKey);
        }

        [Theory]
        [InlineData("2025-10-10T10:00:00", true)]  // Friday 10 AM
        [InlineData("2025-10-10T07:00:00", false)] // Friday 7 AM (before hours)
        [InlineData("2025-10-10T19:00:00", false)] // Friday 7 PM (after hours)
        [InlineData("2025-10-11T10:00:00", false)] // Saturday (not active)
        [InlineData("2025-10-12T10:00:00", false)] // Sunday (not active)
        public void IsWithinBusinessHours_ShouldReturnCorrectValue(string dateTimeStr, bool expected)
        {
            // Arrange
            var config = CreateValidConfiguration();
            var dateTime = DateTime.Parse(dateTimeStr);

            // Act
            var result = config.IsWithinBusinessHours(dateTime);

            // Assert
            Assert.Equal(expected, result);
        }

        private WhatsAppAgentConfiguration CreateValidConfiguration()
        {
            return new WhatsAppAgentConfiguration(
                "tenant-123",
                "Clínica ABC",
                "+5511999999999",
                "whatsapp-key",
                "ai-key",
                "gpt-4",
                "You are a scheduling assistant");
        }
    }
}
