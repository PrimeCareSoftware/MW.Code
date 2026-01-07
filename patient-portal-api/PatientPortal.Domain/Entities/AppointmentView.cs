using PatientPortal.Domain.Enums;

namespace PatientPortal.Domain.Entities;

/// <summary>
/// Read-only view of appointments for patients
/// </summary>
public class AppointmentView
{
    public Guid Id { get; set; }
    public Guid PatientId { get; set; }
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string DoctorSpecialty { get; set; } = string.Empty;
    public string ClinicName { get; set; } = string.Empty;
    public DateTime AppointmentDate { get; set; }
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public AppointmentStatus Status { get; set; }
    public string AppointmentType { get; set; } = string.Empty;
    public bool IsTelehealth { get; set; }
    public string? TelehealthLink { get; set; }
    public string? Notes { get; set; }
    public bool CanReschedule { get; set; }
    public bool CanCancel { get; set; }
    public DateTime CreatedAt { get; set; }
}
