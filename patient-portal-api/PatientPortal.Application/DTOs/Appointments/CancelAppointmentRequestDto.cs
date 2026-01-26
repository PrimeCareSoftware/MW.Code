using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Application.DTOs.Appointments;

/// <summary>
/// DTO for cancelling an appointment
/// </summary>
public class CancelAppointmentRequestDto
{
    [Required]
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
}
