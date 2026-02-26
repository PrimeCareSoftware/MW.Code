namespace PatientPortal.Application.DTOs.Appointments;

/// <summary>
/// DTO for doctor information
/// </summary>
public class DoctorDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Name => FullName;
    public string? Specialty { get; set; }
    public string? ProfessionalId { get; set; }
    public string? Crm => ProfessionalId;
    public bool AvailableForOnlineBooking { get; set; } = true;
    public bool IsMvpProfile { get; set; } = true;
}
