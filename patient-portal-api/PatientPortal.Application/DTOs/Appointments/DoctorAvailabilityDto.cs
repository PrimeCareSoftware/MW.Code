namespace PatientPortal.Application.DTOs.Appointments;

/// <summary>
/// DTO for doctor availability information
/// </summary>
public class DoctorAvailabilityDto
{
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string? Specialty { get; set; }
    public DateTime AvailableDate { get; set; }
    public int Duration { get; set; }
}
