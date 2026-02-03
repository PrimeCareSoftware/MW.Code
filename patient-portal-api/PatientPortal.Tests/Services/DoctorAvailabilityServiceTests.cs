using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PatientPortal.Application.Interfaces;
using PatientPortal.Application.Services;

namespace PatientPortal.Tests.Services;

public class DoctorAvailabilityServiceTests
{
    private readonly Mock<IMainDatabaseContext> _mockDatabase;
    private readonly Mock<ClinicSettingsService> _mockClinicSettings;
    private readonly Mock<ILogger<DoctorAvailabilityService>> _mockLogger;
    private readonly DoctorAvailabilityService _service;

    public DoctorAvailabilityServiceTests()
    {
        _mockDatabase = new Mock<IMainDatabaseContext>();
        var mockClinicSettingsLogger = new Mock<ILogger<ClinicSettingsService>>();
        _mockClinicSettings = new Mock<ClinicSettingsService>(_mockDatabase.Object, mockClinicSettingsLogger.Object);
        _mockLogger = new Mock<ILogger<DoctorAvailabilityService>>();
        _service = new DoctorAvailabilityService(_mockDatabase.Object, _mockClinicSettings.Object, _mockLogger.Object);
    }

    [Fact]
    public void DoctorAvailabilityService_ShouldBeConstructed_Successfully()
    {
        // Assert
        _service.Should().NotBeNull();
        Assert.IsType<DoctorAvailabilityService>(_service);
    }

    // Note: Testing raw SQL services with complex queries is challenging with unit tests.
    // These services are better tested with integration tests against a real database.
    // The main value of the current tests is verifying null/error handling in AppointmentService.
}

