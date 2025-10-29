using FluentAssertions;
using MedicSoft.Telemedicine.Domain.Entities;
using MedicSoft.Telemedicine.Domain.Enums;

namespace MedicSoft.Telemedicine.Tests.Domain;

/// <summary>
/// Unit tests for TelemedicineSession entity
/// Following AAA pattern (Arrange, Act, Assert)
/// </summary>
public class TelemedicineSessionTests
{
    [Fact]
    public void Constructor_WithValidParameters_ShouldCreateSession()
    {
        // Arrange
        var tenantId = "tenant-123";
        var appointmentId = Guid.NewGuid();
        var clinicId = Guid.NewGuid();
        var providerId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var roomId = "test-room";
        var roomUrl = "https://example.daily.co/test-room";

        // Act
        var session = new TelemedicineSession(
            tenantId,
            appointmentId,
            clinicId,
            providerId,
            patientId,
            roomId,
            roomUrl
        );

        // Assert
        session.Should().NotBeNull();
        session.Id.Should().NotBeEmpty();
        session.TenantId.Should().Be(tenantId);
        session.AppointmentId.Should().Be(appointmentId);
        session.ClinicId.Should().Be(clinicId);
        session.ProviderId.Should().Be(providerId);
        session.PatientId.Should().Be(patientId);
        session.RoomId.Should().Be(roomId);
        session.RoomUrl.Should().Be(roomUrl);
        session.Status.Should().Be(SessionStatus.Scheduled);
        session.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void Constructor_WithInvalidTenantId_ShouldThrowException(string invalidTenantId)
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var clinicId = Guid.NewGuid();
        var providerId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        // Act
        Action act = () => new TelemedicineSession(
            invalidTenantId,
            appointmentId,
            clinicId,
            providerId,
            patientId,
            "room-id",
            "http://example.com"
        );

        // Assert
        act.Should().Throw<ArgumentException>()
            .WithMessage("*TenantId*");
    }

    [Fact]
    public void StartSession_FromScheduledStatus_ShouldStartSuccessfully()
    {
        // Arrange
        var session = CreateValidSession();

        // Act
        session.StartSession();

        // Assert
        session.Status.Should().Be(SessionStatus.InProgress);
        session.Duration.Should().NotBeNull();
        session.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void StartSession_FromNonScheduledStatus_ShouldThrowException()
    {
        // Arrange
        var session = CreateValidSession();
        session.StartSession(); // Now in progress

        // Act
        Action act = () => session.StartSession();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*start*");
    }

    [Fact]
    public void CompleteSession_FromInProgressStatus_ShouldCompleteSuccessfully()
    {
        // Arrange
        var session = CreateValidSession();
        session.StartSession();
        var notes = "Session completed successfully";

        // Act
        session.CompleteSession(notes);

        // Assert
        session.Status.Should().Be(SessionStatus.Completed);
        session.SessionNotes.Should().Be(notes);
        session.Duration.GetDuration().Should().NotBeNull();
    }

    [Fact]
    public void CompleteSession_FromScheduledStatus_ShouldThrowException()
    {
        // Arrange
        var session = CreateValidSession();

        // Act
        Action act = () => session.CompleteSession();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*complete*");
    }

    [Fact]
    public void CancelSession_FromScheduledOrInProgress_ShouldCancelSuccessfully()
    {
        // Arrange
        var session = CreateValidSession();
        var reason = "Patient requested cancellation";

        // Act
        session.CancelSession(reason);

        // Assert
        session.Status.Should().Be(SessionStatus.Cancelled);
        session.SessionNotes.Should().Contain(reason);
    }

    [Fact]
    public void CancelSession_FromCompletedStatus_ShouldThrowException()
    {
        // Arrange
        var session = CreateValidSession();
        session.StartSession();
        session.CompleteSession();

        // Act
        Action act = () => session.CancelSession("Test");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*cancel*");
    }

    [Fact]
    public void MarkAsFailed_ShouldSetStatusToFailed()
    {
        // Arrange
        var session = CreateValidSession();
        session.StartSession();
        var errorMessage = "Connection lost";

        // Act
        session.MarkAsFailed(errorMessage);

        // Assert
        session.Status.Should().Be(SessionStatus.Failed);
        session.SessionNotes.Should().Contain(errorMessage);
    }

    [Fact]
    public void SetRecordingUrl_WithValidUrl_ShouldSetUrl()
    {
        // Arrange
        var session = CreateValidSession();
        var recordingUrl = "https://cdn.daily.co/recording.mp4";

        // Act
        session.SetRecordingUrl(recordingUrl);

        // Assert
        session.RecordingUrl.Should().Be(recordingUrl);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("   ")]
    public void SetRecordingUrl_WithInvalidUrl_ShouldThrowException(string invalidUrl)
    {
        // Arrange
        var session = CreateValidSession();

        // Act
        Action act = () => session.SetRecordingUrl(invalidUrl);

        // Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void AddNotes_ShouldAppendNotesToExisting()
    {
        // Arrange
        var session = CreateValidSession();
        var notes1 = "First note";
        var notes2 = "Second note";

        // Act
        session.AddNotes(notes1);
        session.AddNotes(notes2);

        // Assert
        session.SessionNotes.Should().Contain(notes1);
        session.SessionNotes.Should().Contain(notes2);
    }

    private TelemedicineSession CreateValidSession()
    {
        return new TelemedicineSession(
            "tenant-123",
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            Guid.NewGuid(),
            "test-room",
            "https://example.daily.co/test-room"
        );
    }
}
