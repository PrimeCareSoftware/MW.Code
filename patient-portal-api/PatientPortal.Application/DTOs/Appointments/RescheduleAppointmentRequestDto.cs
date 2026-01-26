using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Application.DTOs.Appointments;

/// <summary>
/// DTO for rescheduling an appointment
/// </summary>
public class RescheduleAppointmentRequestDto
{
    [Required]
    public DateTime NewDate { get; set; }

    [Required]
    public TimeSpan NewTime { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }
}
