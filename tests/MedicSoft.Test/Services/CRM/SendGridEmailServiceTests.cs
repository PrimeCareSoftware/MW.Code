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
    /// Unit tests for SendGridEmailService
    /// Tests cover: enabled/disabled state, error handling, configuration validation
    /// </summary>
    public class SendGridEmailServiceTests
    {
        private readonly Mock<ILogger<SendGridEmailService>> _loggerMock;
        private readonly MessagingConfiguration _messagingConfig;

        public SendGridEmailServiceTests()
        {
            _loggerMock = new Mock<ILogger<SendGridEmailService>>();
            _messagingConfig = new MessagingConfiguration
            {
                Email = new EmailConfiguration
                {
                    ApiKey = "SG.test_api_key_123456789",
                    FromEmail = "test@primecare.com.br",
                    FromName = "PrimeCare Test",
                    UseSandbox = true,
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
        public async Task SendEmailAsync_WhenDisabled_ShouldLogAndReturn()
        {
            // Arrange
            _messagingConfig.Email.Enabled = false;
            var service = CreateService();

            // Act
            await service.SendEmailAsync("recipient@test.com", "Test Subject", "Test Body");

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
        public async Task SendEmailWithTemplateAsync_WhenDisabled_ShouldLogAndReturn()
        {
            // Arrange
            _messagingConfig.Email.Enabled = false;
            var service = CreateService();
            var templateId = Guid.NewGuid();
            var variables = new Dictionary<string, string>
            {
                { "name", "John" },
                { "date", "2026-01-28" }
            };

            // Act
            await service.SendEmailWithTemplateAsync("recipient@test.com", templateId, variables);

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
        public void SendEmailAsync_WithValidConfiguration_ShouldNotThrowDuringConstruction()
        {
            // Arrange & Act
            var act = () => CreateService();

            // Assert
            act.Should().NotThrow();
        }

        [Fact]
        public void SendEmailAsync_WithSandboxMode_ShouldBeConfiguredCorrectly()
        {
            // Arrange
            _messagingConfig.Email.UseSandbox = true;
            
            // Act
            var service = CreateService();

            // Assert
            service.Should().NotBeNull();
            _messagingConfig.Email.UseSandbox.Should().BeTrue();
        }

        [Fact]
        public void Configuration_ShouldHaveCorrectDefaults()
        {
            // Arrange
            var config = new EmailConfiguration();

            // Assert
            config.ApiKey.Should().BeEmpty();
            config.FromEmail.Should().BeEmpty();
            config.FromName.Should().BeEmpty();
            config.UseSandbox.Should().BeFalse();
            config.Enabled.Should().BeTrue();
        }

        [Fact]
        public void MessagingConfiguration_ShouldInitializeEmailConfig()
        {
            // Arrange & Act
            var config = new MessagingConfiguration();

            // Assert
            config.Email.Should().NotBeNull();
            config.Sms.Should().NotBeNull();
            config.WhatsApp.Should().NotBeNull();
        }

        [Theory]
        [InlineData("test@example.com", "Subject", "Body")]
        [InlineData("user@domain.com", "Another Subject", "Another Body")]
        public async Task SendEmailAsync_WithDifferentInputs_ShouldHandleGracefully(string to, string subject, string body)
        {
            // Arrange
            _messagingConfig.Email.Enabled = false; // Disable to avoid actual API calls
            var service = CreateService();

            // Act
            var act = async () => await service.SendEmailAsync(to, subject, body);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public void EmailConfiguration_ShouldAllowPropertyAssignment()
        {
            // Arrange
            var config = new EmailConfiguration();

            // Act
            config.ApiKey = "test-key";
            config.FromEmail = "from@test.com";
            config.FromName = "Test Sender";
            config.UseSandbox = true;
            config.Enabled = false;

            // Assert
            config.ApiKey.Should().Be("test-key");
            config.FromEmail.Should().Be("from@test.com");
            config.FromName.Should().Be("Test Sender");
            config.UseSandbox.Should().BeTrue();
            config.Enabled.Should().BeFalse();
        }

        private SendGridEmailService CreateService()
        {
            var optionsMock = new Mock<IOptions<MessagingConfiguration>>();
            optionsMock.Setup(o => o.Value).Returns(_messagingConfig);

            return new SendGridEmailService(_loggerMock.Object, optionsMock.Object);
        }
    }
}
