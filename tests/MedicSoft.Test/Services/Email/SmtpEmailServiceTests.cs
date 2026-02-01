using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MedicSoft.Application.Services.Email;
using Moq;
using Xunit;

namespace MedicSoft.Test.Services.Email
{
    /// <summary>
    /// Unit tests for SmtpEmailService
    /// Tests cover: enabled/disabled state, error handling, configuration validation
    /// </summary>
    public class SmtpEmailServiceTests
    {
        private readonly Mock<ILogger<SmtpEmailService>> _loggerMock;
        private readonly SmtpEmailSettings _emailSettings;

        public SmtpEmailServiceTests()
        {
            _loggerMock = new Mock<ILogger<SmtpEmailService>>();
            _emailSettings = new SmtpEmailSettings
            {
                SmtpServer = "smtp.test.com",
                SmtpPort = 587,
                UseSsl = true,
                Username = "test@test.com",
                Password = "test123",
                From = "test@test.com",
                FromName = "Test Service",
                Enabled = true,
                TimeoutSeconds = 30
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
            _emailSettings.Enabled = false;
            var service = CreateService();

            // Act
            await service.SendEmailAsync("recipient@test.com", "Test Subject", "<p>Test Body</p>");

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("Email sending is disabled")),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }

        [Fact]
        public async Task SendEmailAsync_WithNullTo_ShouldThrowArgumentException()
        {
            // Arrange
            var service = CreateService();

            // Act
            Func<Task> act = async () => await service.SendEmailAsync(null!, "Subject", "Body");

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Email recipient (to) cannot be null or empty*");
        }

        [Fact]
        public async Task SendEmailAsync_WithEmptyTo_ShouldThrowArgumentException()
        {
            // Arrange
            var service = CreateService();

            // Act
            Func<Task> act = async () => await service.SendEmailAsync("", "Subject", "Body");

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Email recipient (to) cannot be null or empty*");
        }

        [Fact]
        public async Task SendEmailAsync_WithNullSubject_ShouldThrowArgumentException()
        {
            // Arrange
            var service = CreateService();

            // Act
            Func<Task> act = async () => await service.SendEmailAsync("test@test.com", null!, "Body");

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Email subject cannot be null or empty*");
        }

        [Fact]
        public async Task SendEmailAsync_WithNullBody_ShouldThrowArgumentException()
        {
            // Arrange
            var service = CreateService();

            // Act
            Func<Task> act = async () => await service.SendEmailAsync("test@test.com", "Subject", null!);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>()
                .WithMessage("*Email body cannot be null or empty*");
        }

        [Fact]
        public async Task SendEmailAsync_WithoutSmtpServer_ShouldThrowInvalidOperationException()
        {
            // Arrange
            _emailSettings.SmtpServer = "";
            var service = CreateService();

            // Act
            Func<Task> act = async () => await service.SendEmailAsync("test@test.com", "Subject", "Body");

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("SMTP server not configured");
        }

        [Fact]
        public void SendEmailWithTemplateAsync_ShouldThrowNotImplementedException()
        {
            // Arrange
            var service = CreateService();
            var templateId = Guid.NewGuid();
            var variables = new Dictionary<string, string> { { "name", "Test" } };

            // Act
            Func<Task> act = async () => await service.SendEmailWithTemplateAsync(
                "test@test.com", templateId, variables, "tenant-123");

            // Assert
            act.Should().ThrowAsync<NotImplementedException>()
                .WithMessage("*Template-based email is not supported in SmtpEmailService*");
        }

        [Fact]
        public void SmtpEmailSettings_ShouldHaveCorrectDefaults()
        {
            // Arrange & Act
            var settings = new SmtpEmailSettings();

            // Assert
            settings.SmtpPort.Should().Be(587);
            settings.UseSsl.Should().BeTrue();
            settings.Enabled.Should().BeTrue();
            settings.TimeoutSeconds.Should().Be(30);
        }

        [Fact]
        public void SmtpEmailSettings_SectionName_ShouldBeEmail()
        {
            // Assert
            SmtpEmailSettings.SectionName.Should().Be("Email");
        }

        private SmtpEmailService CreateService()
        {
            var options = Options.Create(_emailSettings);
            return new SmtpEmailService(_loggerMock.Object, options);
        }
    }
}
