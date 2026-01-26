using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using PatientPortal.Application.DTOs.Appointments;
using PatientPortal.Application.Interfaces;
using PatientPortal.Application.Services;
using PatientPortal.Domain.Enums;
using PatientPortal.Domain.Interfaces;
using PatientPortal.Domain.Entities;

namespace PatientPortal.Tests.Services;

public class AppointmentServiceTests
{
    private readonly Mock<IAppointmentViewRepository> _mockAppointmentViewRepository;
    private readonly Mock<IPatientUserRepository> _mockPatientUserRepository;
    private readonly Mock<IMainDatabaseContext> _mockDatabase;
    private readonly Mock<ILogger<AppointmentService>> _mockLogger;
    private readonly AppointmentService _service;

    public AppointmentServiceTests()
    {
        _mockAppointmentViewRepository = new Mock<IAppointmentViewRepository>();
        _mockPatientUserRepository = new Mock<IPatientUserRepository>();
        _mockDatabase = new Mock<IMainDatabaseContext>();
        _mockLogger = new Mock<ILogger<AppointmentService>>();
        
        _service = new AppointmentService(
            _mockAppointmentViewRepository.Object,
            _mockPatientUserRepository.Object,
            _mockDatabase.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenPatientUserNotFound()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var patientUserId = Guid.NewGuid();

        _mockPatientUserRepository.Setup(x => x.GetByIdAsync(patientUserId))
            .ReturnsAsync((PatientUser?)null);

        // Act
        var result = await _service.GetByIdAsync(appointmentId, patientUserId);

        // Assert
        result.Should().BeNull();
        _mockPatientUserRepository.Verify(x => x.GetByIdAsync(patientUserId), Times.Once);
        _mockAppointmentViewRepository.Verify(x => x.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<Guid>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnAppointment_WhenFound()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var patientUserId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        var patientUser = new PatientUser
        {
            Id = patientUserId,
            PatientId = patientId,
            Email = "test@example.com",
            PasswordHash = "hashedpassword",
            CPF = "12345678901",
            FullName = "Test Patient",
            PhoneNumber = "1234567890",
            ClinicId = Guid.NewGuid(),
            IsActive = true
        };

        var appointmentView = new AppointmentView
        {
            Id = appointmentId,
            PatientId = patientId,
            DoctorName = "Dr. Smith",
            DoctorSpecialty = "Cardiology",
            ClinicName = "Main Clinic",
            AppointmentDate = DateTime.Today.AddDays(1),
            StartTime = new TimeSpan(10, 0, 0),
            EndTime = new TimeSpan(10, 30, 0),
            Status = AppointmentStatus.Scheduled,
            AppointmentType = "Consultation",
            IsTelehealth = false,
            CanReschedule = true,
            CanCancel = true
        };

        _mockPatientUserRepository.Setup(x => x.GetByIdAsync(patientUserId))
            .ReturnsAsync(patientUser);

        _mockAppointmentViewRepository.Setup(x => x.GetByIdAsync(appointmentId, patientId))
            .ReturnsAsync(appointmentView);

        // Act
        var result = await _service.GetByIdAsync(appointmentId, patientUserId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(appointmentId);
        result.DoctorName.Should().Be("Dr. Smith");
        result.DoctorSpecialty.Should().Be("Cardiology");
        result.Status.Should().Be("Scheduled");
    }

    [Fact]
    public async Task GetMyAppointmentsAsync_ShouldReturnEmptyList_WhenPatientUserNotFound()
    {
        // Arrange
        var patientUserId = Guid.NewGuid();

        _mockPatientUserRepository.Setup(x => x.GetByIdAsync(patientUserId))
            .ReturnsAsync((PatientUser?)null);

        // Act
        var result = await _service.GetMyAppointmentsAsync(patientUserId);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetCountAsync_ShouldReturnZero_WhenPatientUserNotFound()
    {
        // Arrange
        var patientUserId = Guid.NewGuid();

        _mockPatientUserRepository.Setup(x => x.GetByIdAsync(patientUserId))
            .ReturnsAsync((PatientUser?)null);

        // Act
        var result = await _service.GetCountAsync(patientUserId);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public async Task BookAppointmentAsync_ShouldThrowException_WhenPatientUserNotFound()
    {
        // Arrange
        var patientUserId = Guid.NewGuid();
        var request = new BookAppointmentRequestDto
        {
            DoctorId = Guid.NewGuid(),
            ClinicId = Guid.NewGuid(),
            AppointmentDate = DateTime.Today.AddDays(1),
            AppointmentTime = new TimeSpan(10, 0, 0),
            DurationMinutes = 30
        };

        _mockPatientUserRepository.Setup(x => x.GetByIdAsync(patientUserId))
            .ReturnsAsync((PatientUser?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.BookAppointmentAsync(patientUserId, request));
    }

    [Fact]
    public async Task ConfirmAppointmentAsync_ShouldThrowException_WhenAppointmentNotFound()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var patientUserId = Guid.NewGuid();
        var patientId = Guid.NewGuid();

        var patientUser = new PatientUser
        {
            Id = patientUserId,
            PatientId = patientId,
            Email = "test@example.com",
            PasswordHash = "hashedpassword",
            CPF = "12345678901",
            FullName = "Test Patient",
            PhoneNumber = "1234567890",
            ClinicId = Guid.NewGuid(),
            IsActive = true
        };

        _mockPatientUserRepository.Setup(x => x.GetByIdAsync(patientUserId))
            .ReturnsAsync(patientUser);

        _mockAppointmentViewRepository.Setup(x => x.GetByIdAsync(appointmentId, patientId))
            .ReturnsAsync((AppointmentView?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.ConfirmAppointmentAsync(appointmentId, patientUserId));
    }

    [Fact]
    public async Task CancelAppointmentAsync_ShouldThrowException_WhenPatientUserNotFound()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var patientUserId = Guid.NewGuid();
        var request = new CancelAppointmentRequestDto { Reason = "Personal reasons" };

        _mockPatientUserRepository.Setup(x => x.GetByIdAsync(patientUserId))
            .ReturnsAsync((PatientUser?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.CancelAppointmentAsync(appointmentId, patientUserId, request));
    }

    [Fact]
    public async Task RescheduleAppointmentAsync_ShouldThrowException_WhenAppointmentNotFound()
    {
        // Arrange
        var appointmentId = Guid.NewGuid();
        var patientUserId = Guid.NewGuid();
        var patientId = Guid.NewGuid();
        var request = new RescheduleAppointmentRequestDto
        {
            NewDate = DateTime.Today.AddDays(2),
            NewTime = new TimeSpan(14, 0, 0)
        };

        var patientUser = new PatientUser
        {
            Id = patientUserId,
            PatientId = patientId,
            Email = "test@example.com",
            PasswordHash = "hashedpassword",
            CPF = "12345678901",
            FullName = "Test Patient",
            PhoneNumber = "1234567890",
            ClinicId = Guid.NewGuid(),
            IsActive = true
        };

        _mockPatientUserRepository.Setup(x => x.GetByIdAsync(patientUserId))
            .ReturnsAsync(patientUser);

        _mockAppointmentViewRepository.Setup(x => x.GetByIdAsync(appointmentId, patientId))
            .ReturnsAsync((AppointmentView?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(
            async () => await _service.RescheduleAppointmentAsync(appointmentId, patientUserId, request));
    }
}

