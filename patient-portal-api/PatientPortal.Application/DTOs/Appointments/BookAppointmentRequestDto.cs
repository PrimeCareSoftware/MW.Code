using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Application.DTOs.Appointments;

/// <summary>
/// DTO for booking a new appointment
/// </summary>
public class BookAppointmentRequestDto
{
    [Required]
    public Guid DoctorId { get; set; }

    [Required]
    public Guid ClinicId { get; set; }

    [Required]
    public DateTime AppointmentDate { get; set; }

    [Required]
    public TimeSpan AppointmentTime { get; set; }

    [Required]
    [Range(15, 240)]
    public int DurationMinutes { get; set; } = 30;

    public int AppointmentType { get; set; } = 1; // Regular = 1

    public int AppointmentMode { get; set; } = 1; // InPerson = 1

    public int PaymentType { get; set; } = 1; // Private = 1

    public Guid? HealthInsurancePlanId { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }
}
