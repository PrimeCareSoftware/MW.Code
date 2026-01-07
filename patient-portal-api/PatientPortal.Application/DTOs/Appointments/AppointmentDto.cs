using PatientPortal.Domain.Enums;

namespace PatientPortal.Application.DTOs.Appointments;

/// <summary>
/// DTO for appointment details
/// </summary>
public class AppointmentDto
{
    public Guid Id { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorSpecialty { get; set; } = string.Empty;
    public string ClinicName { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public string Status { get; set; } = string.Empty;
    public string AppointmentType { get; set; } = string.Empty;
    public bool IsTelehealth { get; set; }
    public string? TelehealthLink { get; set; }
    public string? Notes { get; set; }
    public bool CanReschedule { get; set; }
    public bool CanCancel { get; set; }
}
