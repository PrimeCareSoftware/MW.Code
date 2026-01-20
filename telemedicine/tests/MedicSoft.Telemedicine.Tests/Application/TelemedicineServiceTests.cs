using FluentAssertions;
using Moq;
using MedicSoft.Telemedicine.Application.DTOs;
using MedicSoft.Telemedicine.Application.Services;
using MedicSoft.Telemedicine.Domain.Entities;
using MedicSoft.Telemedicine.Domain.Enums;
using MedicSoft.Telemedicine.Domain.Interfaces;

namespace MedicSoft.Telemedicine.Tests.Application;

/// <summary>
/// Unit tests for TelemedicineService
/// Uses Moq for mocking dependencies
/// </summary>
public class TelemedicineServiceTests
{
    private readonly Mock<ITelemedicineSessionRepository> _mockRepository;
    private readonly Mock<ITelemedicineConsentRepository> _mockConsentRepository;
    private readonly Mock<IVideoCallService> _mockVideoService;
    private readonly TelemedicineService _service;
    private const string TenantId = "tenant-123";

    public TelemedicineServiceTests()
    {
        _mockRepository = new Mock<ITelemedicineSessionRepository>();
        _mockConsentRepository = new Mock<ITelemedicineConsentRepository>();
        _mockVideoService = new Mock<IVideoCallService>();
        _service = new TelemedicineService(
            _mockRepository.Object,
            _mockConsentRepository.Object,
            _mockVideoService.Object);
    }

    [Fact]
    public async Task CreateSessionAsync_WithValidRequest_ShouldCreateSession()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            AppointmentId = Guid.NewGuid(),
            ClinicId = Guid.NewGuid(),
            ProviderId = Guid.NewGuid(),
            PatientId = Guid.NewGuid()
        };

        var roomInfo = new VideoRoomInfo
        {
            RoomName = "test-room",
            RoomUrl = "https://example.daily.co/test-room",
            ExpiresAt = DateTime.UtcNow.AddHours(2)
        };

        _mockRepository
            .Setup(r => r.GetByAppointmentIdAsync(request.AppointmentId, TenantId))
            .ReturnsAsync((TelemedicineSession?)null);

        _mockVideoService
            .Setup(v => v.CreateRoomAsync(It.IsAny<string>(), It.IsAny<int>()))
            .ReturnsAsync(roomInfo);

        _mockRepository
            .Setup(r => r.AddAsync(It.IsAny<TelemedicineSession>()))
            .ReturnsAsync((TelemedicineSession s) => s);

        // Act
        var result = await _service.CreateSessionAsync(request, TenantId);

        // Assert
        result.Should().NotBeNull();
        result.AppointmentId.Should().Be(request.AppointmentId);
        result.ClinicId.Should().Be(request.ClinicId);
        result.Status.Should().Be(SessionStatus.Scheduled.ToString());
        result.RoomUrl.Should().Be(roomInfo.RoomUrl);

        _mockVideoService.Verify(v => v.CreateRoomAsync(It.IsAny<string>(), 2), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<TelemedicineSession>()), Times.Once);
    }

    [Fact]
    public async Task CreateSessionAsync_WhenSessionExists_ShouldThrowException()
    {
        // Arrange
        var request = new CreateSessionRequest
        {
            AppointmentId = Guid.NewGuid(),
            ClinicId = Guid.NewGuid(),
            ProviderId = Guid.NewGuid(),
            PatientId = Guid.NewGuid()
        };

        var existingSession = new TelemedicineSession(
            TenantId,
            request.AppointmentId,
            request.ClinicId,
            request.ProviderId,
            request.PatientId,
            "existing-room",
            "http://example.com"
        );

        _mockRepository
            .Setup(r => r.GetByAppointmentIdAsync(request.AppointmentId, TenantId))
            .ReturnsAsync(existingSession);

        // Act
        Func<Task> act = async () => await _service.CreateSessionAsync(request, TenantId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*already exists*");
    }

    [Fact]
    public async Task JoinSessionAsync_WithValidRequest_ShouldReturnJoinInfo()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var userName = "Dr. Smith";

        var session = new TelemedicineSession(
            TenantId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "test-room",
            "https://example.daily.co/test-room"
        );

        var request = new JoinSessionRequest
        {
            SessionId = sessionId,
            UserId = userId,
            UserName = userName,
            Role = "provider"
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(sessionId, TenantId))
            .ReturnsAsync(session);

        _mockVideoService
            .Setup(v => v.GenerateTokenAsync(
                session.RoomId,
                userId.ToString(),
                userName,
                120))
            .ReturnsAsync("test-token");

        // Act
        var result = await _service.JoinSessionAsync(request, TenantId);

        // Assert
        result.Should().NotBeNull();
        result.RoomUrl.Should().Be(session.RoomUrl);
        result.AccessToken.Should().Be("test-token");
        result.ExpiresAt.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(120), TimeSpan.FromSeconds(5));

        _mockVideoService.Verify(v => v.GenerateTokenAsync(
            session.RoomId,
            userId.ToString(),
            userName,
            120), Times.Once);
    }

    [Fact]
    public async Task StartSessionAsync_ShouldUpdateStatusToInProgress()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = new TelemedicineSession(
            TenantId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "test-room",
            "https://example.daily.co/test-room"
        );

        _mockRepository
            .Setup(r => r.GetByIdAsync(sessionId, TenantId))
            .ReturnsAsync(session);

        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<TelemedicineSession>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.StartSessionAsync(sessionId, TenantId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(SessionStatus.InProgress.ToString());

        _mockRepository.Verify(r => r.UpdateAsync(It.Is<TelemedicineSession>(
            s => s.Status == SessionStatus.InProgress)), Times.Once);
    }

    [Fact]
    public async Task CompleteSessionAsync_ShouldUpdateStatusToCompleted()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var session = new TelemedicineSession(
            TenantId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "test-room",
            "https://example.daily.co/test-room"
        );
        session.StartSession();

        var request = new CompleteSessionRequest
        {
            SessionId = sessionId,
            Notes = "Session went well"
        };

        _mockRepository
            .Setup(r => r.GetByIdAsync(sessionId, TenantId))
            .ReturnsAsync(session);

        _mockVideoService
            .Setup(v => v.GetRecordingUrlAsync(session.RoomId))
            .ReturnsAsync("https://cdn.daily.co/recording.mp4");

        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<TelemedicineSession>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CompleteSessionAsync(request, TenantId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(SessionStatus.Completed.ToString());
        result.RecordingUrl.Should().Be("https://cdn.daily.co/recording.mp4");

        _mockRepository.Verify(r => r.UpdateAsync(It.Is<TelemedicineSession>(
            s => s.Status == SessionStatus.Completed && s.RecordingUrl != null)), Times.Once);
    }

    [Fact]
    public async Task CancelSessionAsync_ShouldUpdateStatusToCancelled()
    {
        // Arrange
        var sessionId = Guid.NewGuid();
        var reason = "Patient unavailable";
        var session = new TelemedicineSession(
            TenantId,
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "test-room",
            "https://example.daily.co/test-room"
        );

        _mockRepository
            .Setup(r => r.GetByIdAsync(sessionId, TenantId))
            .ReturnsAsync(session);

        _mockVideoService
            .Setup(v => v.DeleteRoomAsync(session.RoomId))
            .Returns(Task.CompletedTask);

        _mockRepository
            .Setup(r => r.UpdateAsync(It.IsAny<TelemedicineSession>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.CancelSessionAsync(sessionId, reason, TenantId);

        // Assert
        result.Should().NotBeNull();
        result.Status.Should().Be(SessionStatus.Cancelled.ToString());

        _mockVideoService.Verify(v => v.DeleteRoomAsync(session.RoomId), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.Is<TelemedicineSession>(
            s => s.Status == SessionStatus.Cancelled)), Times.Once);
    }
}
