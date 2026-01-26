using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PatientPortal.Application.Configuration;
using PatientPortal.Infrastructure.Services;
using Xunit;

namespace PatientPortal.Tests.Services;

public class NotificationServiceTests
{
    private readonly Mock<ILogger<NotificationService>> _mockLogger;
    private readonly NotificationService _notificationService;
    private readonly EmailSettings _emailSettings;

    public NotificationServiceTests()
    {
        _mockLogger = new Mock<ILogger<NotificationService>>();
        _emailSettings = new EmailSettings
        {
            SmtpServer = "smtp.test.com",
            SmtpPort = 587,
            UseSsl = true,
            Username = "test@test.com",
            Password = "testpassword",
            From = "noreply@test.com",
            FromName = "Test Sender"
        };

        var options = Options.Create(_emailSettings);
        _notificationService = new NotificationService(options, _mockLogger.Object);
    }

    [Fact]
    public async Task SendSmsAsync_ShouldLogMessage()
    {
        // Arrange
        var phoneNumber = "+5511999999999";
        var message = "Test SMS message";

        // Act
        await _notificationService.SendSmsAsync(phoneNumber, message);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("SMS")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SendWhatsAppAsync_ShouldLogMessage()
    {
        // Arrange
        var phoneNumber = "+5511999999999";
        var message = "Test WhatsApp message";

        // Act
        await _notificationService.SendWhatsAppAsync(phoneNumber, message);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("WhatsApp")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }

    [Fact]
    public async Task SendEmailAsync_WithEmptySmtpServer_ShouldLogWarning()
    {
        // Arrange
        var emptySettings = new EmailSettings { SmtpServer = "" };
        var options = Options.Create(emptySettings);
        var service = new NotificationService(options, _mockLogger.Object);

        // Act
        await service.SendEmailAsync("test@test.com", "Subject", "Body");

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("SMTP server not configured")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);
    }
}
