using MedicSoft.Telemedicine.Application.DTOs;
using MedicSoft.Telemedicine.Application.Services;
using MedicSoft.Telemedicine.Domain.Entities;
using MedicSoft.Telemedicine.Domain.Enums;
using MedicSoft.Telemedicine.Domain.Interfaces;
using Moq;
using Xunit;

namespace MedicSoft.Telemedicine.Tests.Application;

/// <summary>
/// Unit tests for consent-related methods in TelemedicineService
/// Tests CFM 2.314/2022 compliance requirements
/// </summary>
public class TelemedicineConsentServiceTests
{
    private readonly Mock<ITelemedicineSessionRepository> _sessionRepositoryMock;
    private readonly Mock<ITelemedicineConsentRepository> _consentRepositoryMock;
    private readonly Mock<IVideoCallService> _videoCallServiceMock;
    private readonly TelemedicineService _service;
    private const string TestTenantId = "test-tenant";

    public TelemedicineConsentServiceTests()
    {
        _sessionRepositoryMock = new Mock<ITelemedicineSessionRepository>();
        _consentRepositoryMock = new Mock<ITelemedicineConsentRepository>();
        _videoCallServiceMock = new Mock<IVideoCallService>();
        
        _service = new TelemedicineService(
            _sessionRepositoryMock.Object,
            _consentRepositoryMock.Object,
            _videoCallServiceMock.Object);
    }

    [Fact]
    public async Task RecordConsentAsync_WithValidRequest_CreatesConsent()
    {
        // Arrange
        var request = new CreateConsentRequest
        {
            PatientId = Guid.NewGuid(),
            AppointmentId = null,
            AcceptsRecording = true,
            AcceptsDataSharing = true
        };
        var ipAddress = "192.168.1.1";
        var userAgent = "Mozilla/5.0";

        _consentRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TelemedicineConsent>()))
            .ReturnsAsync((TelemedicineConsent c) => c);

        // Act
        var result = await _service.RecordConsentAsync(request, ipAddress, userAgent, TestTenantId);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(request.PatientId, result.PatientId);
        Assert.True(result.AcceptsRecording);
        Assert.True(result.AcceptsDataSharing);
        Assert.True(result.IsActive);
        
        _consentRepositoryMock.Verify(
            r => r.AddAsync(It.Is<TelemedicineConsent>(c => 
                c.PatientId == request.PatientId &&
                c.AcceptsRecording == request.AcceptsRecording &&
                c.AcceptsDataSharing == request.AcceptsDataSharing)),
            Times.Once);
    }

    [Fact]
    public async Task RecordConsentAsync_WithAppointmentId_LinksToSession()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var sessionId = Guid.NewGuid();
        var request = new CreateConsentRequest
        {
            PatientId = Guid.NewGuid(),
            AppointmentId = appointmentId,
            AcceptsRecording = false,
            AcceptsDataSharing = true
        };

        var session = new TelemedicineSession(
            TestTenantId,
            appointmentId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            request.PatientId,
            "room-1",
            "https://room.url");

        _consentRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TelemedicineConsent>()))
            .ReturnsAsync((TelemedicineConsent c) => c);

        _sessionRepositoryMock
            .Setup(r => r.GetByAppointmentIdAsync(appointmentId, TestTenantId))
            .ReturnsAsync(session);

        // Act
        var result = await _service.RecordConsentAsync(request, "192.168.1.1", "user-agent", TestTenantId);

        // Assert
        Assert.Equal(appointmentId, result.AppointmentId);
        
        _sessionRepositoryMock.Verify(
            r => r.UpdateAsync(It.Is<TelemedicineSession>(s => 
                s.PatientConsented &&
                s.AppointmentId == appointmentId)),
            Times.Once);
    }

    [Fact]
    public async Task RecordConsentAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.RecordConsentAsync(null!, "ip", "agent", TestTenantId));
    }

    [Fact]
    public async Task RecordConsentAsync_WithEmptyIpAddress_ThrowsArgumentException()
    {
        // Arrange
        var request = new CreateConsentRequest
        {
            PatientId = Guid.NewGuid(),
            AcceptsRecording = true,
            AcceptsDataSharing = true
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() =>
            _service.RecordConsentAsync(request, "", "user-agent", TestTenantId));
    }

    [Fact]
    public async Task RevokeConsentAsync_WithValidConsent_RevokesConsent()
    {
        // Arrange
        var consentId = Guid.NewGuid();
        var consent = new TelemedicineConsent(
            TestTenantId,
            Guid.NewGuid(),
            "consent text",
            "192.168.1.1",
            "user agent",
            true,
            true);
        var reason = "Patient requested";

        _consentRepositoryMock
            .Setup(r => r.GetByIdAsync(consentId, TestTenantId))
            .ReturnsAsync(consent);

        // Act
        var result = await _service.RevokeConsentAsync(consentId, reason, TestTenantId);

        // Assert
        Assert.False(result.IsActive);
        Assert.NotNull(result.RevokedAt);
        Assert.Equal(reason, result.RevocationReason);
        
        _consentRepositoryMock.Verify(
            r => r.UpdateAsync(It.Is<TelemedicineConsent>(c => 
                !c.IsActive &&
                c.RevocationReason == reason)),
            Times.Once);
    }

    [Fact]
    public async Task RevokeConsentAsync_WithNonExistentConsent_ThrowsInvalidOperationException()
    {
        // Arrange
        var consentId = Guid.NewGuid();
        
        _consentRepositoryMock
            .Setup(r => r.GetByIdAsync(consentId, TestTenantId))
            .ReturnsAsync((TelemedicineConsent?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _service.RevokeConsentAsync(consentId, "reason", TestTenantId));
    }

    [Fact]
    public async Task HasValidConsentAsync_WithValidConsent_ReturnsTrue()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        
        _consentRepositoryMock
            .Setup(r => r.HasValidConsentAsync(patientId, TestTenantId))
            .ReturnsAsync(true);

        // Act
        var result = await _service.HasValidConsentAsync(patientId, TestTenantId);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public async Task HasValidConsentAsync_WithNoConsent_ReturnsFalse()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        
        _consentRepositoryMock
            .Setup(r => r.HasValidConsentAsync(patientId, TestTenantId))
            .ReturnsAsync(false);

        // Act
        var result = await _service.HasValidConsentAsync(patientId, TestTenantId);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public async Task ValidateFirstAppointmentAsync_WithNoHistory_ReturnsFirstAppointmentTrue()
    {
        // Arrange
        var request = new ValidateFirstAppointmentRequest
        {
            PatientId = Guid.NewGuid(),
            ProviderId = Guid.NewGuid(),
            Justification = null
        };

        _sessionRepositoryMock
            .Setup(r => r.GetByPatientIdAsync(request.PatientId, TestTenantId, 0, 1))
            .ReturnsAsync(new List<TelemedicineSession>());

        // Act
        var result = await _service.ValidateFirstAppointmentAsync(request, TestTenantId);

        // Assert
        Assert.True(result.IsFirstAppointment);
        Assert.False(result.CanProceedWithTelemedicine);
        Assert.Contains("CFM 2.314", result.ValidationMessage);
    }

    [Fact]
    public async Task ValidateFirstAppointmentAsync_WithHistory_ReturnsFirstAppointmentFalse()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var providerId = Guid.NewGuid();
        var request = new ValidateFirstAppointmentRequest
        {
            PatientId = patientId,
            ProviderId = providerId,
            Justification = null
        };

        var previousSession = new TelemedicineSession(
            TestTenantId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            providerId,
            patientId,
            "room-1",
            "https://room.url");
        previousSession.StartSession();
        previousSession.CompleteSession();

        _sessionRepositoryMock
            .Setup(r => r.GetByPatientIdAsync(patientId, TestTenantId, 0, 1))
            .ReturnsAsync(new List<TelemedicineSession> { previousSession });

        // Act
        var result = await _service.ValidateFirstAppointmentAsync(request, TestTenantId);

        // Assert
        Assert.False(result.IsFirstAppointment);
        Assert.True(result.CanProceedWithTelemedicine);
    }

    [Fact]
    public async Task ValidateFirstAppointmentAsync_WithJustification_AllowsTelemedicine()
    {
        // Arrange
        var request = new ValidateFirstAppointmentRequest
        {
            PatientId = Guid.NewGuid(),
            ProviderId = Guid.NewGuid(),
            Justification = "Patient lives in remote area with no access to clinic"
        };

        _sessionRepositoryMock
            .Setup(r => r.GetByPatientIdAsync(request.PatientId, TestTenantId, 0, 1))
            .ReturnsAsync(new List<TelemedicineSession>());

        // Act
        var result = await _service.ValidateFirstAppointmentAsync(request, TestTenantId);

        // Assert
        Assert.True(result.IsFirstAppointment);
        Assert.True(result.CanProceedWithTelemedicine);
        Assert.Contains("justificativa", result.ValidationMessage);
    }

    [Fact]
    public async Task ValidateFirstAppointmentAsync_WithNullRequest_ThrowsArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() =>
            _service.ValidateFirstAppointmentAsync(null!, TestTenantId));
    }

    [Fact]
    public async Task GetPatientConsentsAsync_ReturnsConsentList()
    {
        // Arrange
        var patientId = Guid.NewGuid();
        var consents = new List<TelemedicineConsent>
        {
            new TelemedicineConsent(
                TestTenantId,
                patientId,
                "consent 1",
                "192.168.1.1",
                "agent",
                true,
                true),
            new TelemedicineConsent(
                TestTenantId,
                patientId,
                "consent 2",
                "192.168.1.2",
                "agent",
                false,
                true)
        };

        _consentRepositoryMock
            .Setup(r => r.GetByPatientIdAsync(patientId, TestTenantId, true))
            .ReturnsAsync(consents);

        // Act
        var result = await _service.GetPatientConsentsAsync(patientId, TestTenantId, true);

        // Assert
        Assert.Equal(2, result.Count());
        Assert.All(result, c => Assert.Equal(patientId, c.PatientId));
    }
}
