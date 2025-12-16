using Microsoft.EntityFrameworkCore;
using MedicSoft.Appointments.Api.Data;
using MedicSoft.Appointments.Api.Models;

namespace MedicSoft.Appointments.Api.Services;

public interface IAppointmentService
{
    Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto dto, string tenantId);
    Task<bool> CancelAppointmentAsync(Guid id, string cancellationReason, string tenantId);
    Task<DailyAgendaDto> GetDailyAgendaAsync(DateTime date, Guid clinicId, string tenantId);
    Task<AppointmentDto?> GetByIdAsync(Guid id, string tenantId);
    Task<IEnumerable<AvailableSlotDto>> GetAvailableSlotsAsync(DateTime date, Guid clinicId, int durationMinutes, string tenantId);
    Task<AppointmentDto?> CheckInPatientAsync(Guid appointmentId, string tenantId);
    Task<AppointmentDto?> StartConsultationAsync(Guid appointmentId, string tenantId);
    Task<AppointmentDto?> CompleteConsultationAsync(Guid appointmentId, string tenantId);
}

public class AppointmentService : IAppointmentService
{
    private readonly AppointmentsDbContext _context;
    private readonly ILogger<AppointmentService> _logger;

    public AppointmentService(AppointmentsDbContext context, ILogger<AppointmentService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto dto, string tenantId)
    {
        var endTime = dto.StartTime.Add(TimeSpan.FromMinutes(dto.DurationMinutes));

        // Check for conflicts
        var hasConflict = await _context.Appointments.AnyAsync(a =>
            a.ClinicId == dto.ClinicId &&
            a.ScheduledDate.Date == dto.ScheduledDate.Date &&
            a.Status != 5 && // Not cancelled
            ((dto.StartTime >= a.StartTime && dto.StartTime < a.EndTime) ||
             (endTime > a.StartTime && endTime <= a.EndTime) ||
             (dto.StartTime <= a.StartTime && endTime >= a.EndTime)));

        if (hasConflict)
        {
            throw new InvalidOperationException("There is already an appointment scheduled for this time slot");
        }

        var appointment = new AppointmentEntity
        {
            Id = Guid.NewGuid(),
            PatientId = dto.PatientId,
            PatientName = "Patient Name", // TODO: Fetch from Patients microservice
            ClinicId = dto.ClinicId,
            ClinicName = "Clinic Name", // TODO: Fetch from Clinics data
            DoctorId = dto.DoctorId,
            DoctorName = dto.DoctorId.HasValue ? "Doctor Name" : null, // TODO: Fetch from Users/Doctors microservice
            ScheduledDate = dto.ScheduledDate.Date,
            StartTime = dto.StartTime,
            EndTime = endTime,
            DurationMinutes = dto.DurationMinutes,
            Status = 0, // Scheduled
            Notes = dto.Notes,
            TenantId = tenantId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Appointments.Add(appointment);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created appointment: {AppointmentId}", appointment.Id);
        return MapToDto(appointment);
    }

    public async Task<bool> CancelAppointmentAsync(Guid id, string cancellationReason, string tenantId)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId);

        if (appointment == null)
            return false;

        appointment.Status = 5; // Cancelled
        appointment.CancellationReason = cancellationReason;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Cancelled appointment: {AppointmentId}", id);
        return true;
    }

    public async Task<DailyAgendaDto> GetDailyAgendaAsync(DateTime date, Guid clinicId, string tenantId)
    {
        var appointments = await _context.Appointments
            .Where(a => a.ClinicId == clinicId &&
                       a.ScheduledDate.Date == date.Date &&
                       a.TenantId == tenantId &&
                       a.Status != 5) // Not cancelled
            .OrderBy(a => a.StartTime)
            .ToListAsync();

        return new DailyAgendaDto
        {
            Date = date.Date,
            ClinicId = clinicId,
            Appointments = appointments.Select(MapToDto)
        };
    }

    public async Task<AppointmentDto?> GetByIdAsync(Guid id, string tenantId)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == id && a.TenantId == tenantId);

        return appointment != null ? MapToDto(appointment) : null;
    }

    public async Task<IEnumerable<AvailableSlotDto>> GetAvailableSlotsAsync(DateTime date, Guid clinicId, int durationMinutes, string tenantId)
    {
        var existingAppointments = await _context.Appointments
            .Where(a => a.ClinicId == clinicId &&
                       a.ScheduledDate.Date == date.Date &&
                       a.TenantId == tenantId &&
                       a.Status != 5)
            .OrderBy(a => a.StartTime)
            .ToListAsync();

        var slots = new List<AvailableSlotDto>();
        var workStartTime = new TimeSpan(8, 0, 0); // 8:00 AM
        var workEndTime = new TimeSpan(18, 0, 0);  // 6:00 PM
        var slotDuration = TimeSpan.FromMinutes(durationMinutes);

        var currentTime = workStartTime;

        while (currentTime.Add(slotDuration) <= workEndTime)
        {
            var slotEnd = currentTime.Add(slotDuration);

            var isOccupied = existingAppointments.Any(a =>
                (currentTime >= a.StartTime && currentTime < a.EndTime) ||
                (slotEnd > a.StartTime && slotEnd <= a.EndTime) ||
                (currentTime <= a.StartTime && slotEnd >= a.EndTime));

            if (!isOccupied)
            {
                slots.Add(new AvailableSlotDto
                {
                    StartTime = currentTime,
                    EndTime = slotEnd,
                    DurationMinutes = durationMinutes
                });
            }

            currentTime = currentTime.Add(slotDuration);
        }

        return slots;
    }

    public async Task<AppointmentDto?> CheckInPatientAsync(Guid appointmentId, string tenantId)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == appointmentId && a.TenantId == tenantId);

        if (appointment == null)
            return null;

        appointment.Status = 2; // Arrived
        appointment.CheckInTime = DateTime.UtcNow;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Patient checked in for appointment: {AppointmentId}", appointmentId);
        return MapToDto(appointment);
    }

    public async Task<AppointmentDto?> StartConsultationAsync(Guid appointmentId, string tenantId)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == appointmentId && a.TenantId == tenantId);

        if (appointment == null)
            return null;

        appointment.Status = 3; // InProgress
        appointment.StartedAt = DateTime.UtcNow;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Consultation started for appointment: {AppointmentId}", appointmentId);
        return MapToDto(appointment);
    }

    public async Task<AppointmentDto?> CompleteConsultationAsync(Guid appointmentId, string tenantId)
    {
        var appointment = await _context.Appointments
            .FirstOrDefaultAsync(a => a.Id == appointmentId && a.TenantId == tenantId);

        if (appointment == null)
            return null;

        appointment.Status = 4; // Completed
        appointment.CompletedAt = DateTime.UtcNow;
        appointment.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Consultation completed for appointment: {AppointmentId}", appointmentId);
        return MapToDto(appointment);
    }

    private static AppointmentDto MapToDto(AppointmentEntity entity)
    {
        return new AppointmentDto
        {
            Id = entity.Id,
            PatientId = entity.PatientId,
            PatientName = entity.PatientName,
            ClinicId = entity.ClinicId,
            ClinicName = entity.ClinicName,
            DoctorId = entity.DoctorId,
            DoctorName = entity.DoctorName,
            ScheduledDate = entity.ScheduledDate,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            DurationMinutes = entity.DurationMinutes,
            Status = GetStatusName(entity.Status),
            Notes = entity.Notes,
            CancellationReason = entity.CancellationReason,
            CheckInTime = entity.CheckInTime,
            StartedAt = entity.StartedAt,
            CompletedAt = entity.CompletedAt,
            CreatedAt = entity.CreatedAt
        };
    }

    private static string GetStatusName(int status)
    {
        return status switch
        {
            0 => "Scheduled",
            1 => "Confirmed",
            2 => "Arrived",
            3 => "InProgress",
            4 => "Completed",
            5 => "Cancelled",
            6 => "NoShow",
            _ => "Unknown"
        };
    }
}
