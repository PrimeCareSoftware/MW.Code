namespace MedicSoft.Appointments.Api.Models;

public class AppointmentDto
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public Guid ClinicId { get; set; }
    public string ClinicName { get; set; } = string.Empty;
    public Guid? DoctorId { get; set; }
    public string? DoctorName { get; set; }
    public DateTime ScheduledDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CheckInTime { get; set; }
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateAppointmentDto
{
    public Guid PatientId { get; set; }
    public Guid ClinicId { get; set; }
    public Guid? DoctorId { get; set; }
    public DateTime ScheduledDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public int DurationMinutes { get; set; } = 30;
    public string? Notes { get; set; }
}

public class DailyAgendaDto
{
    public DateTime Date { get; set; }
    public Guid ClinicId { get; set; }
    public IEnumerable<AppointmentDto> Appointments { get; set; } = new List<AppointmentDto>();
}

public class AvailableSlotDto
{
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int DurationMinutes { get; set; }
}

public class CancelAppointmentRequest
{
    public string CancellationReason { get; set; } = string.Empty;
}

public class WaitingQueueEntryDto
{
    public Guid Id { get; set; }
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public int Position { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime CheckInTime { get; set; }
    public DateTime? CalledAt { get; set; }
    public DateTime? ServiceStartedAt { get; set; }
}
