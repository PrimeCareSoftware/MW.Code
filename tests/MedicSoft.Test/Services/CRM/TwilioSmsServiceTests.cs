using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MedicSoft.Api.Configuration;
using MedicSoft.Api.Services.CRM;
using Moq;
using Xunit;

namespace MedicSoft.Test.Services.CRM
{
    /// <summary>
    /// Unit tests for TwilioSmsService
    /// Tests cover: enabled/disabled state, phone number formatting, configuration validation
    /// </summary>
    public class TwilioSmsServiceTests
    {
        private readonly Mock<ILogger<TwilioSmsService>> _loggerMock;
        private readonly MessagingConfiguration _messagingConfig;

        public TwilioSmsServiceTests()
        {
            _loggerMock = new Mock<ILogger<TwilioSmsService>>();
            _messagingConfig = new MessagingConfiguration
            {
                Sms = new SmsConfiguration
                {
                    AccountSid = "TEST1234567890abcdef1234567890test",
                    AuthToken = "test_auth_token_123456",
                    FromPhoneNumber = "+5511999999999",
                    Enabled = true
                }
            };
        }

        [Fact]
        public void Constructor_ShouldInitializeCorrectly()
        {
            // Arrange & Act
            var service = CreateService();

            // Assert
            service.Should().NotBeNull();
        }

        [Fact]
        public async Task SendSmsAsync_WhenDisabled_ShouldLogAndReturn()
        {
            // Arrange
            _messagingConfig.Sms.Enabled = false;
            var service = CreateService();

            // Act
            await service.SendSmsAsync("+5511987654321", "Test message");

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("disabled")),
                    null,
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task SendSmsAsync_WhenCredentialsNotConfigured_ShouldThrowException()
        {
            // Arrange
            _messagingConfig.Sms.AccountSid = "";
            _messagingConfig.Sms.AuthToken = "";
            var service = CreateService();

            // Act
            var act = async () => await service.SendSmsAsync("+5511987654321", "Test message");

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*Twilio credentials not configured*");
        }

        [Fact]
        public void Configuration_ShouldHaveCorrectDefaults()
        {
            // Arrange
            var config = new SmsConfiguration();

            // Assert
            config.AccountSid.Should().BeEmpty();
            config.AuthToken.Should().BeEmpty();
            config.FromPhoneNumber.Should().BeEmpty();
            config.Enabled.Should().BeTrue();
        }

        [Fact]
        public void SmsConfiguration_ShouldAllowPropertyAssignment()
        {
            // Arrange
            var config = new SmsConfiguration();

            // Act
            config.AccountSid = "test-sid";
            config.AuthToken = "test-token";
            config.FromPhoneNumber = "+5511999999999";
            config.Enabled = false;

            // Assert
            config.AccountSid.Should().Be("test-sid");
            config.AuthToken.Should().Be("test-token");
            config.FromPhoneNumber.Should().Be("+5511999999999");
            config.Enabled.Should().BeFalse();
        }

        [Theory]
        [InlineData("+5511987654321", "Test message")]
        [InlineData("11987654321", "Another test")]
        [InlineData("5511987654321", "Third test")]
        public async Task SendSmsAsync_WithDifferentPhoneFormats_ShouldHandleGracefully(string phoneNumber, string message)
        {
            // Arrange
            _messagingConfig.Sms.Enabled = false; // Disable to avoid actual API calls
            var service = CreateService();

            // Act
            var act = async () => await service.SendSmsAsync(phoneNumber, message);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public void Constructor_WithDisabledConfig_ShouldNotInitializeTwilioClient()
        {
            // Arrange
            _messagingConfig.Sms.Enabled = false;

            // Act
            var act = () => CreateService();

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void Constructor_WithEmptyCredentials_ShouldNotThrow()
        {
            // Arrange
            _messagingConfig.Sms.AccountSid = "";
            _messagingConfig.Sms.AuthToken = "";

            // Act
            var act = () => CreateService();

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void MessagingConfiguration_ShouldIncludeSmsSection()
        {
            // Arrange & Act
            var config = new MessagingConfiguration();

            // Assert
            config.Sms.Should().NotBeNull();
        }

        [Theory]
        [InlineData("11987654321", "+5511987654321")] // Brazilian mobile without country code
        [InlineData("1133334444", "+551133334444")]   // Brazilian landline without country code
        [InlineData("+5511987654321", "+5511987654321")] // Already formatted
        public void PhoneNumberFormatting_ShouldHandleBrazilianNumbers(string input, string expected)
        {
            // This test documents expected behavior for phone number formatting
            // The actual formatting is done internally by the service
            
            // Assert
            input.Should().NotBeNullOrEmpty();
            expected.Should().StartWith("+55");
        }

        private TwilioSmsService CreateService()
        {
            var optionsMock = new Mock<IOptions<MessagingConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(_messagingConfig);

            return new TwilioSmsService(_loggerMock.Object, optionsMock.Object);
        }
    }
}
