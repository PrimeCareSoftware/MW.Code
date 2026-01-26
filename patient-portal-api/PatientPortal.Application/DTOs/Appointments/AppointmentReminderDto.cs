namespace PatientPortal.Application.DTOs.Appointments;

/// <summary>
/// DTO containing appointment data needed for sending reminders
/// </summary>
public class AppointmentReminderDto
{
    public Guid AppointmentId { get; set; }
    public Guid PatientId { get; set; }
    public string PatientName { get; set; } = string.Empty;
    public string PatientEmail { get; set; } = string.Empty;
    public string? PatientPhone { get; set; }
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorSpecialty { get; set; } = string.Empty;
    public string ClinicName { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public TimeSpan AppointmentTime { get; set; }
    public string AppointmentType { get; set; } = string.Empty;
    public bool IsTelehealth { get; set; }
    public string? TelehealthLink { get; set; }
}
