using MedicSoft.Domain.Enums;

namespace MedicSoft.Application.DTOs
{
    public class RecurringAppointmentPatternDto
    {
        public Guid Id { get; set; }
        public Guid ClinicId { get; set; }
        public Guid? ProfessionalId { get; set; }
        public Guid? PatientId { get; set; }
        public RecurrenceFrequency Frequency { get; set; }
        public int Interval { get; set; }
        public RecurrenceDays? DaysOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? OccurrencesCount { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? DurationMinutes { get; set; }
        public AppointmentType? AppointmentType { get; set; }
        public BlockedTimeSlotType? BlockedSlotType { get; set; }
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Navigation properties
        public string? ClinicName { get; set; }
        public string? ProfessionalName { get; set; }
        public string? PatientName { get; set; }
    }

    public class CreateRecurringAppointmentPatternDto
    {
        public Guid ClinicId { get; set; }
        public Guid? ProfessionalId { get; set; }
        public Guid? PatientId { get; set; }
        public RecurrenceFrequency Frequency { get; set; }
        public int Interval { get; set; } = 1;
        public RecurrenceDays? DaysOfWeek { get; set; }
        public int? DayOfMonth { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? OccurrencesCount { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int? DurationMinutes { get; set; }
        public AppointmentType? AppointmentType { get; set; }
        public BlockedTimeSlotType? BlockedSlotType { get; set; }
        public string? Notes { get; set; }
    }

    public class CreateRecurringAppointmentsDto
    {
        public Guid PatientId { get; set; }
        public Guid ClinicId { get; set; }
        public Guid? ProfessionalId { get; set; }
        public RecurrenceFrequency Frequency { get; set; }
        public RecurrenceDays? DaysOfWeek { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? OccurrencesCount { get; set; }
        public TimeSpan StartTime { get; set; }
        public int DurationMinutes { get; set; }
        public AppointmentType AppointmentType { get; set; }
        public string? Notes { get; set; }
    }
}
