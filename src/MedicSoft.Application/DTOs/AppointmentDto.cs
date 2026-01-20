using System;

namespace MedicSoft.Application.DTOs
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public TimeSpan ScheduledTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateAppointmentDto
    {
        public Guid PatientId { get; set; }
        public Guid ClinicId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public TimeSpan ScheduledTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Type { get; set; } = "Regular";
        public string? Notes { get; set; }
    }

    public class UpdateAppointmentDto
    {
        public DateTime ScheduledDate { get; set; }
        public TimeSpan ScheduledTime { get; set; }
        public int DurationMinutes { get; set; }
        public string Type { get; set; } = "Regular";
        public string? Notes { get; set; }
    }

    public class DailyAgendaDto
    {
        public DateTime Date { get; set; }
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public List<AppointmentDto> Appointments { get; set; } = new();
        public List<TimeSpan> AvailableSlots { get; set; } = new();
    }

    public class AvailableSlotDto
    {
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsAvailable { get; set; }
    }
}