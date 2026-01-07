namespace PatientPortal.Domain.Enums;

/// <summary>
/// Appointment status enum
/// </summary>
public enum AppointmentStatus
{
    Scheduled = 1,
    Confirmed = 2,
    InProgress = 3,
    Completed = 4,
    Cancelled = 5,
    NoShow = 6,
    Rescheduled = 7
}
