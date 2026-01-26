using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using PatientPortal.Application.Configuration;
using PatientPortal.Application.Interfaces;
using PatientPortal.Application.Services;
using PatientPortal.Domain.Enums;
using PatientPortal.Infrastructure.Services;
using Xunit;

namespace PatientPortal.Tests.Services;

public class AppointmentReminderServiceTests
{
    private readonly Mock<IServiceProvider> _mockServiceProvider;
    private readonly Mock<IServiceScopeFactory> _mockScopeFactory;
    private readonly Mock<IServiceScope> _mockScope;
    private readonly Mock<ILogger<AppointmentReminderService>> _mockLogger;
    private readonly Mock<IMainDatabaseContext> _mockDatabase;
    private readonly Mock<INotificationService> _mockNotificationService;
    private readonly AppointmentReminderSettings _settings;
    private readonly PortalSettings _portalSettings;

    public AppointmentReminderServiceTests()
    {
        _mockServiceProvider = new Mock<IServiceProvider>();
        _mockScopeFactory = new Mock<IServiceScopeFactory>();
        _mockScope = new Mock<IServiceScope>();
        _mockLogger = new Mock<ILogger<AppointmentReminderService>>();
        _mockDatabase = new Mock<IMainDatabaseContext>();
        _mockNotificationService = new Mock<INotificationService>();

        _settings = new AppointmentReminderSettings
        {
            Enabled = true,
            CheckIntervalMinutes = 1,
            AdvanceNoticeHours = 24
        };

        _portalSettings = new PortalSettings
        {
            BaseUrl = "https://portal.test.com"
        };

        // Setup service provider chain
        var scopeServiceProvider = new Mock<IServiceProvider>();
        scopeServiceProvider
            .Setup(x => x.GetService(typeof(IMainDatabaseContext)))
            .Returns(_mockDatabase.Object);
        scopeServiceProvider
            .Setup(x => x.GetService(typeof(INotificationService)))
            .Returns(_mockNotificationService.Object);

        _mockScope.Setup(x => x.ServiceProvider).Returns(scopeServiceProvider.Object);
        _mockScopeFactory.Setup(x => x.CreateScope()).Returns(_mockScope.Object);
        _mockServiceProvider
            .Setup(x => x.GetService(typeof(IServiceScopeFactory)))
            .Returns(_mockScopeFactory.Object);
    }

    [Fact]
    public void Constructor_ShouldCreateInstance()
    {
        // Arrange & Act
        var service = new AppointmentReminderService(
            _mockServiceProvider.Object,
            _mockLogger.Object,
            Options.Create(_settings),
            Options.Create(_portalSettings)
        );

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task ExecuteAsync_WhenDisabled_ShouldNotProcessReminders()
    {
        // Arrange
        _settings.Enabled = false;
        var service = new AppointmentReminderService(
            _mockServiceProvider.Object,
            _mockLogger.Object,
            Options.Create(_settings),
            Options.Create(_portalSettings)
        );

        var cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(TimeSpan.FromSeconds(1));

        // Act
        await service.StartAsync(cancellationTokenSource.Token);
        await Task.Delay(500); // Wait a bit
        await service.StopAsync(cancellationTokenSource.Token);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString()!.Contains("disabled")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
            Times.Once);

        _mockDatabase.Verify(x => x.ExecuteQueryAsync<It.IsAnyType>(
            It.IsAny<string>(),
            It.IsAny<object[]>()),
            Times.Never);
    }
}
