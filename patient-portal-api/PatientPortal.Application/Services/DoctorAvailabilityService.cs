using Microsoft.Extensions.Logging;
using PatientPortal.Application.DTOs.Appointments;
using PatientPortal.Application.Interfaces;
using PatientPortal.Domain.Enums;

namespace PatientPortal.Application.Services;

/// <summary>
/// Service for managing doctor availability and scheduling
/// Queries the main MedicSoft database for doctor schedules and appointments
/// </summary>
public class DoctorAvailabilityService : IDoctorAvailabilityService
{
    private readonly IMainDatabaseContext _mainDatabase;
    private readonly ILogger<DoctorAvailabilityService> _logger;

    // Default working hours (can be overridden by doctor schedule)
    private readonly TimeSpan _defaultStartTime = new TimeSpan(8, 0, 0);  // 8 AM
    private readonly TimeSpan _defaultEndTime = new TimeSpan(18, 0, 0);   // 6 PM
    private const int DefaultAppointmentDuration = 30; // 30 minutes

    public DoctorAvailabilityService(
        IMainDatabaseContext mainDatabase,
        ILogger<DoctorAvailabilityService> logger)
    {
        _mainDatabase = mainDatabase;
        _logger = logger;
    }

    public async Task<List<DoctorAvailabilityDto>> GetAvailableSlotsAsync(
        Guid? doctorId,
        DateTime date,
        string? specialty,
        Guid clinicId,
        string tenantId)
    {
        try
        {
            // Check if online appointment scheduling is enabled for this clinic
            var clinicSettingsQuery = @"
                SELECT ""EnableOnlineAppointmentScheduling""
                FROM ""Clinics""
                WHERE ""Id"" = {0} AND ""TenantId"" = {1}";
            
            var clinicSettings = await _mainDatabase.ExecuteQueryAsync<ClinicSchedulingSettings>(
                clinicSettingsQuery,
                clinicId,
                tenantId
            );
            
            var settings = clinicSettings.FirstOrDefault();
            if (settings == null || !settings.EnableOnlineAppointmentScheduling)
            {
                _logger.LogWarning("Online appointment scheduling is disabled for clinic {ClinicId}", clinicId);
                return new List<DoctorAvailabilityDto>();
            }

            var availableSlots = new List<DoctorAvailabilityDto>();

            // Query doctors from the main Users table (doctors have Role = Doctor)
            var doctorsQuery = @"
                SELECT u.""Id"", u.""FullName"", u.""Specialty""
                FROM ""Users"" u
                INNER JOIN ""UserClinicLinks"" ucl ON u.""Id"" = ucl.""UserId""
                WHERE ucl.""ClinicId"" = {0}
                AND u.""TenantId"" = {1}
                AND u.""IsActive"" = true
                AND u.""Role"" = 1";  // 1 = Doctor role

            var parameters = new List<object> { clinicId, tenantId };

            if (doctorId.HasValue)
            {
                doctorsQuery += " AND u.\"Id\" = {2}";
                parameters.Add(doctorId.Value);
            }

            if (!string.IsNullOrWhiteSpace(specialty))
            {
                doctorsQuery += $" AND u.\"Specialty\" = {{{parameters.Count}}}";
                parameters.Add(specialty);
            }

            // Execute query to get doctors
            var doctors = await _mainDatabase.ExecuteQueryAsync<DoctorInfo>(
                doctorsQuery,
                parameters.ToArray()
            );

            // For each doctor, find available slots
            foreach (var doctor in doctors)
            {
                var slots = await GetDoctorAvailableSlotsAsync(doctor.Id, date, clinicId, tenantId);

                foreach (var slot in slots)
                {
                    availableSlots.Add(new DoctorAvailabilityDto
                    {
                        DoctorId = doctor.Id,
                        DoctorName = doctor.FullName,
                        Specialty = doctor.Specialty,
                        AvailableDate = slot,
                        Duration = DefaultAppointmentDuration
                    });
                }
            }

            return availableSlots.OrderBy(s => s.AvailableDate).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available slots for date {Date}, clinic {ClinicId}", date, clinicId);
            throw;
        }
    }

    public async Task<bool> IsSlotAvailableAsync(Guid doctorId, DateTime dateTime, int durationMinutes, string tenantId)
    {
        try
        {
            var date = dateTime.Date;
            var time = dateTime.TimeOfDay;
            var endTime = time.Add(TimeSpan.FromMinutes(durationMinutes));

            // Check if there are any conflicting appointments
            var conflictQuery = @"
                SELECT COUNT(*)::int as ""Count""
                FROM ""Appointments"" a
                WHERE a.""ProfessionalId"" = {0}
                AND a.""ScheduledDate"" = {1}
                AND a.""TenantId"" = {2}
                AND a.""Status"" NOT IN ({5}, {6})
                AND (
                    (a.""ScheduledTime"" < {4} AND (a.""ScheduledTime"" + (a.""DurationMinutes"" || ' minutes')::interval) > {3})
                    OR (a.""ScheduledTime"" >= {3} AND a.""ScheduledTime"" < {4})
                )";

            var result = await _mainDatabase.ExecuteQueryAsync<ConflictCount>(
                conflictQuery,
                doctorId,
                date,
                tenantId,
                time,
                endTime,
                (int)AppointmentStatus.Cancelled,
                (int)AppointmentStatus.NoShow
            );

            var count = result.FirstOrDefault()?.Count ?? 0;
            return count == 0 && dateTime > DateTime.Now;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking slot availability for doctor {DoctorId} at {DateTime}", doctorId, dateTime);
            throw;
        }
    }

    public async Task<List<DoctorDto>> GetDoctorsAsync(string? specialty, Guid clinicId, string tenantId)
    {
        try
        {
            var query = @"
                SELECT u.""Id"", u.""FullName"", u.""Specialty"", u.""ProfessionalId""
                FROM ""Users"" u
                INNER JOIN ""UserClinicLinks"" ucl ON u.""Id"" = ucl.""UserId""
                WHERE ucl.""ClinicId"" = {0}
                AND u.""TenantId"" = {1}
                AND u.""IsActive"" = true
                AND u.""Role"" = 1";  // 1 = Doctor role

            var parameters = new List<object> { clinicId, tenantId };

            if (!string.IsNullOrWhiteSpace(specialty))
            {
                query += " AND u.\"Specialty\" = {2}";
                parameters.Add(specialty);
            }

            query += " ORDER BY u.\"FullName\"";

            var doctors = await _mainDatabase.ExecuteQueryAsync<DoctorInfo>(
                query,
                parameters.ToArray()
            );

            return doctors.Select(d => new DoctorDto
            {
                Id = d.Id,
                FullName = d.FullName,
                Specialty = d.Specialty,
                ProfessionalId = d.ProfessionalId
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting doctors for clinic {ClinicId}", clinicId);
            throw;
        }
    }

    public async Task<List<SpecialtyDto>> GetSpecialtiesAsync(Guid clinicId, string tenantId)
    {
        try
        {
            var query = @"
                SELECT u.""Specialty"" as Name, COUNT(*)::int as AvailableDoctors
                FROM ""Users"" u
                INNER JOIN ""UserClinicLinks"" ucl ON u.""Id"" = ucl.""UserId""
                WHERE ucl.""ClinicId"" = {0}
                AND u.""TenantId"" = {1}
                AND u.""IsActive"" = true
                AND u.""Role"" = 1
                AND u.""Specialty"" IS NOT NULL
                GROUP BY u.""Specialty""
                ORDER BY u.""Specialty""";

            var specialties = await _mainDatabase.ExecuteQueryAsync<SpecialtyInfo>(
                query,
                clinicId,
                tenantId
            );

            return specialties.Select(s => new SpecialtyDto
            {
                Name = s.Name,
                AvailableDoctors = s.AvailableDoctors
            }).ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting specialties for clinic {ClinicId}", clinicId);
            throw;
        }
    }

    private async Task<List<DateTime>> GetDoctorAvailableSlotsAsync(Guid doctorId, DateTime date, Guid clinicId, string tenantId)
    {
        var availableSlots = new List<DateTime>();

        // Skip past dates
        if (date.Date < DateTime.Today)
            return availableSlots;

        // Get existing appointments for the doctor on this date
        var appointmentsQuery = @"
            SELECT a.""ScheduledDate"", a.""ScheduledTime"", a.""DurationMinutes""
            FROM ""Appointments"" a
            WHERE a.""ProfessionalId"" = {0}
            AND a.""ScheduledDate"" = {1}
            AND a.""TenantId"" = {2}
            AND a.""Status"" NOT IN ({3}, {4})";  // Exclude Cancelled and NoShow

        var appointments = await _mainDatabase.ExecuteQueryAsync<AppointmentInfo>(
            appointmentsQuery,
            doctorId,
            date,
            tenantId,
            (int)AppointmentStatus.Cancelled,
            (int)AppointmentStatus.NoShow
        );

        // Generate time slots for the day
        var slots = GenerateTimeSlots(_defaultStartTime, _defaultEndTime, DefaultAppointmentDuration);

        foreach (var slot in slots)
        {
            var slotDateTime = date.Date.Add(slot);

            // Skip slots in the past
            if (slotDateTime <= DateTime.Now)
                continue;

            // Check if slot conflicts with existing appointments
            var isOccupied = appointments.Any(a =>
            {
                var appointmentStart = a.ScheduledDate.Add(a.ScheduledTime);
                var appointmentEnd = appointmentStart.AddMinutes(a.DurationMinutes);
                var slotEnd = slotDateTime.AddMinutes(DefaultAppointmentDuration);

                return slotDateTime < appointmentEnd && slotEnd > appointmentStart;
            });

            if (!isOccupied)
            {
                availableSlots.Add(slotDateTime);
            }
        }

        return availableSlots;
    }

    private List<TimeSpan> GenerateTimeSlots(TimeSpan startTime, TimeSpan endTime, int durationMinutes)
    {
        var slots = new List<TimeSpan>();
        var current = startTime;

        while (current.Add(TimeSpan.FromMinutes(durationMinutes)) <= endTime)
        {
            slots.Add(current);
            current = current.Add(TimeSpan.FromMinutes(durationMinutes));
        }

        return slots;
    }

    // Helper classes for raw SQL queries
    private class ClinicSchedulingSettings
    {
        public bool EnableOnlineAppointmentScheduling { get; set; }
    }

    private class DoctorInfo
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? Specialty { get; set; }
        public string? ProfessionalId { get; set; }
    }

    private class AppointmentInfo
    {
        public DateTime ScheduledDate { get; set; }
        public TimeSpan ScheduledTime { get; set; }
        public int DurationMinutes { get; set; }
    }

    private class SpecialtyInfo
    {
        public string Name { get; set; } = string.Empty;
        public int AvailableDoctors { get; set; }
    }

    private class ConflictCount
    {
        public int Count { get; set; }
    }
}
