namespace PatientPortal.Application.DTOs.Appointments;

/// <summary>
/// DTO for available time slot
/// </summary>
public class TimeSlotDto
{
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public int DurationMinutes { get; set; }
    public bool IsAvailable { get; set; }
}
