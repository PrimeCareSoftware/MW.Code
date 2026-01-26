namespace PatientPortal.Application.DTOs.Appointments;

/// <summary>
/// DTO for specialty information
/// </summary>
public class SpecialtyDto
{
    public string Name { get; set; } = string.Empty;
    public int AvailableDoctors { get; set; }
}
