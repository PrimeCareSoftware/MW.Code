using Microsoft.Extensions.Logging;
using PatientPortal.Application.DTOs.Appointments;
using PatientPortal.Application.Interfaces;
using PatientPortal.Domain.Enums;
using PatientPortal.Domain.Interfaces;

namespace PatientPortal.Application.Services;

public class AppointmentService : IAppointmentService
{
    private readonly IAppointmentViewRepository _appointmentViewRepository;
    private readonly IPatientUserRepository _patientUserRepository;
    private readonly IMainDatabaseContext _mainDatabase;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(
        IAppointmentViewRepository appointmentViewRepository,
        IPatientUserRepository patientUserRepository,
        IMainDatabaseContext mainDatabase,
        ILogger<AppointmentService> logger)
    {
        _appointmentViewRepository = appointmentViewRepository;
        _patientUserRepository = patientUserRepository;
        _mainDatabase = mainDatabase;
        _logger = logger;
    }

    public async Task<AppointmentDto?> GetByIdAsync(Guid appointmentId, Guid patientUserId)
    {
        var patientUser = await _patientUserRepository.GetByIdAsync(patientUserId);
        if (patientUser == null)
            return null;

        var appointment = await _appointmentViewRepository.GetByIdAsync(appointmentId, patientUser.PatientId);
        
        return appointment == null ? null : MapToDto(appointment);
    }

    public async Task<List<AppointmentDto>> GetMyAppointmentsAsync(Guid patientUserId, int skip = 0, int take = 50)
    {
        var patientUser = await _patientUserRepository.GetByIdAsync(patientUserId);
        if (patientUser == null)
            return new List<AppointmentDto>();

        var appointments = await _appointmentViewRepository.GetByPatientIdAsync(patientUser.PatientId, skip, take);
        
        return appointments.Select(MapToDto).ToList();
    }

    public async Task<List<AppointmentDto>> GetUpcomingAppointmentsAsync(Guid patientUserId, int take = 10)
    {
        var patientUser = await _patientUserRepository.GetByIdAsync(patientUserId);
        if (patientUser == null)
            return new List<AppointmentDto>();

        var appointments = await _appointmentViewRepository.GetUpcomingByPatientIdAsync(patientUser.PatientId, take);
        
        return appointments.Select(MapToDto).ToList();
    }

    public async Task<List<AppointmentDto>> GetByStatusAsync(Guid patientUserId, AppointmentStatus status, int skip = 0, int take = 50)
    {
        var patientUser = await _patientUserRepository.GetByIdAsync(patientUserId);
        if (patientUser == null)
            return new List<AppointmentDto>();

        var appointments = await _appointmentViewRepository.GetByStatusAsync(patientUser.PatientId, status, skip, take);
        
        return appointments.Select(MapToDto).ToList();
    }

    public async Task<int> GetCountAsync(Guid patientUserId)
    {
        var patientUser = await _patientUserRepository.GetByIdAsync(patientUserId);
        if (patientUser == null)
            return 0;

        return await _appointmentViewRepository.GetCountByPatientIdAsync(patientUser.PatientId);
    }

    public async Task<AppointmentDto> BookAppointmentAsync(Guid patientUserId, BookAppointmentRequestDto request)
    {
        try
        {
            var patientUser = await _patientUserRepository.GetByIdAsync(patientUserId);
            if (patientUser == null)
                throw new InvalidOperationException("Patient user not found");

            // Get patient and clinic tenant info
            var patientQuery = await _mainDatabase.ExecuteQueryAsync<PatientInfo>(
                @"SELECT p.""Id"", p.""TenantId"", p.""ClinicId""
                  FROM ""Patients"" p
                  WHERE p.""Id"" = {0}",
                patientUser.PatientId
            );

            var patientInfo = patientQuery.FirstOrDefault();
            if (patientInfo == null)
                throw new InvalidOperationException("Patient not found");

            // Create new appointment using raw SQL insert
            var appointmentId = Guid.NewGuid();
            var now = DateTime.UtcNow;

            var insertQuery = @"
                INSERT INTO ""Appointments"" 
                (""Id"", ""PatientId"", ""ClinicId"", ""ProfessionalId"", ""ScheduledDate"", ""ScheduledTime"", 
                 ""DurationMinutes"", ""Type"", ""Mode"", ""PaymentType"", ""HealthInsurancePlanId"", 
                 ""Status"", ""Notes"", ""TenantId"", ""CreatedAt"", ""UpdatedAt"", ""IsPaid"")
                VALUES 
                ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16})";

            await _mainDatabase.ExecuteCommandAsync(
                insertQuery,
                appointmentId,
                patientUser.PatientId,
                request.ClinicId,
                request.DoctorId,
                request.AppointmentDate,
                request.AppointmentTime,
                request.DurationMinutes,
                request.AppointmentType,
                request.AppointmentMode,
                request.PaymentType,
                request.HealthInsurancePlanId.HasValue ? (object)request.HealthInsurancePlanId.Value : DBNull.Value,
                (int)AppointmentStatus.Scheduled, // Status = Scheduled
                request.Notes ?? (object)DBNull.Value,
                patientInfo.TenantId,
                now,
                now,
                false
            );

            _logger.LogInformation("Appointment {AppointmentId} booked for patient {PatientId}", appointmentId, patientUser.PatientId);

            // Retrieve the created appointment
            var appointment = await _appointmentViewRepository.GetByIdAsync(appointmentId, patientUser.PatientId);
            if (appointment == null)
                throw new InvalidOperationException("Failed to retrieve created appointment");

            return MapToDto(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error booking appointment for patient user {PatientUserId}", patientUserId);
            throw;
        }
    }

    public async Task<AppointmentDto> ConfirmAppointmentAsync(Guid appointmentId, Guid patientUserId)
    {
        try
        {
            var patientUser = await _patientUserRepository.GetByIdAsync(patientUserId);
            if (patientUser == null)
                throw new InvalidOperationException("Patient user not found");

            // Verify appointment belongs to patient
            var appointment = await _appointmentViewRepository.GetByIdAsync(appointmentId, patientUser.PatientId);
            if (appointment == null)
                throw new InvalidOperationException("Appointment not found");

            // Update appointment status
            await _mainDatabase.ExecuteCommandAsync(
                @"UPDATE ""Appointments"" 
                  SET ""Status"" = {0}, ""UpdatedAt"" = {1}
                  WHERE ""Id"" = {2} AND ""PatientId"" = {3}",
                (int)AppointmentStatus.Confirmed, // Status = Confirmed
                DateTime.UtcNow,
                appointmentId,
                patientUser.PatientId
            );

            _logger.LogInformation("Appointment {AppointmentId} confirmed by patient {PatientId}", appointmentId, patientUser.PatientId);

            // Retrieve updated appointment
            var updatedAppointment = await _appointmentViewRepository.GetByIdAsync(appointmentId, patientUser.PatientId);
            if (updatedAppointment == null)
                throw new InvalidOperationException("Failed to retrieve updated appointment");

            return MapToDto(updatedAppointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error confirming appointment {AppointmentId}", appointmentId);
            throw;
        }
    }

    public async Task<AppointmentDto> CancelAppointmentAsync(Guid appointmentId, Guid patientUserId, CancelAppointmentRequestDto request)
    {
        try
        {
            var patientUser = await _patientUserRepository.GetByIdAsync(patientUserId);
            if (patientUser == null)
                throw new InvalidOperationException("Patient user not found");

            // Verify appointment belongs to patient
            var appointment = await _appointmentViewRepository.GetByIdAsync(appointmentId, patientUser.PatientId);
            if (appointment == null)
                throw new InvalidOperationException("Appointment not found");

            // Update appointment status
            await _mainDatabase.ExecuteCommandAsync(
                @"UPDATE ""Appointments"" 
                  SET ""Status"" = {0}, ""CancellationReason"" = {1}, ""UpdatedAt"" = {2}
                  WHERE ""Id"" = {3} AND ""PatientId"" = {4}",
                (int)AppointmentStatus.Cancelled, // Status = Cancelled
                request.Reason,
                DateTime.UtcNow,
                appointmentId,
                patientUser.PatientId
            );

            _logger.LogInformation("Appointment {AppointmentId} cancelled by patient {PatientId}", appointmentId, patientUser.PatientId);

            // Retrieve updated appointment
            var updatedAppointment = await _appointmentViewRepository.GetByIdAsync(appointmentId, patientUser.PatientId);
            if (updatedAppointment == null)
                throw new InvalidOperationException("Failed to retrieve updated appointment");

            return MapToDto(updatedAppointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling appointment {AppointmentId}", appointmentId);
            throw;
        }
    }

    public async Task<AppointmentDto> RescheduleAppointmentAsync(Guid appointmentId, Guid patientUserId, RescheduleAppointmentRequestDto request)
    {
        try
        {
            var patientUser = await _patientUserRepository.GetByIdAsync(patientUserId);
            if (patientUser == null)
                throw new InvalidOperationException("Patient user not found");

            // Verify appointment belongs to patient
            var appointment = await _appointmentViewRepository.GetByIdAsync(appointmentId, patientUser.PatientId);
            if (appointment == null)
                throw new InvalidOperationException("Appointment not found");

            // Update appointment date and time
            await _mainDatabase.ExecuteCommandAsync(
                @"UPDATE ""Appointments"" 
                  SET ""ScheduledDate"" = {0}, ""ScheduledTime"" = {1}, ""Status"" = {2}, ""UpdatedAt"" = {3}
                  WHERE ""Id"" = {4} AND ""PatientId"" = {5}",
                request.NewDate,
                request.NewTime,
                (int)AppointmentStatus.Scheduled, // Status = Scheduled (reset to scheduled after rescheduling)
                DateTime.UtcNow,
                appointmentId,
                patientUser.PatientId
            );

            _logger.LogInformation("Appointment {AppointmentId} rescheduled by patient {PatientId} to {NewDate} {NewTime}", 
                appointmentId, patientUser.PatientId, request.NewDate, request.NewTime);

            // Retrieve updated appointment
            var updatedAppointment = await _appointmentViewRepository.GetByIdAsync(appointmentId, patientUser.PatientId);
            if (updatedAppointment == null)
                throw new InvalidOperationException("Failed to retrieve updated appointment");

            return MapToDto(updatedAppointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rescheduling appointment {AppointmentId}", appointmentId);
            throw;
        }
    }

    private static AppointmentDto MapToDto(Domain.Entities.AppointmentView appointment)
    {
        return new AppointmentDto
        {
            Id = appointment.Id,
            DoctorName = appointment.DoctorName,
            DoctorSpecialty = appointment.DoctorSpecialty,
            ClinicName = appointment.ClinicName,
            AppointmentDate = appointment.AppointmentDate,
            StartTime = appointment.StartTime,
            EndTime = appointment.EndTime,
            Status = appointment.Status.ToString(),
            AppointmentType = appointment.AppointmentType,
            IsTelehealth = appointment.IsTelehealth,
            TelehealthLink = appointment.TelehealthLink,
            Notes = appointment.Notes,
            CanReschedule = appointment.CanReschedule,
            CanCancel = appointment.CanCancel
        };
    }

    // Helper class for patient info
    private class PatientInfo
    {
        public Guid Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public Guid ClinicId { get; set; }
    }
}
