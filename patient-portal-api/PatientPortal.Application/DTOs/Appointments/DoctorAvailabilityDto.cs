using System.Text.Json.Serialization;
using PatientPortal.Application.JsonConverters;

namespace PatientPortal.Application.DTOs.Appointments;

/// <summary>
/// DTO for doctor availability information
/// </summary>
public class DoctorAvailabilityDto
{
    public Guid DoctorId { get; set; }
    public string DoctorName { get; set; } = string.Empty;
    public string? Specialty { get; set; }
    [JsonConverter(typeof(DateOnlyJsonConverter))]
    public DateTime AvailableDate { get; set; }
    public int Duration { get; set; }
}
