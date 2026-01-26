using PatientPortal.Application.DTOs.Appointments;

namespace PatientPortal.Application.Interfaces;

/// <summary>
/// Service for managing doctor availability and time slots
/// </summary>
public interface IDoctorAvailabilityService
{
    /// <summary>
    /// Gets available time slots for doctors
    /// </summary>
    /// <param name="doctorId">Optional specific doctor ID</param>
    /// <param name="date">Date to check availability</param>
    /// <param name="specialty">Optional specialty filter</param>
    /// <param name="clinicId">Clinic ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>List of available slots</returns>
    Task<List<DoctorAvailabilityDto>> GetAvailableSlotsAsync(
        Guid? doctorId, 
        DateTime date, 
        string? specialty,
        Guid clinicId,
        string tenantId);
    
    /// <summary>
    /// Checks if a specific time slot is available for a doctor
    /// </summary>
    /// <param name="doctorId">Doctor ID</param>
    /// <param name="dateTime">Date and time to check</param>
    /// <param name="durationMinutes">Duration in minutes</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>True if slot is available</returns>
    Task<bool> IsSlotAvailableAsync(Guid doctorId, DateTime dateTime, int durationMinutes, string tenantId);

    /// <summary>
    /// Gets list of doctors, optionally filtered by specialty
    /// </summary>
    /// <param name="specialty">Optional specialty filter</param>
    /// <param name="clinicId">Clinic ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>List of doctors</returns>
    Task<List<DoctorDto>> GetDoctorsAsync(string? specialty, Guid clinicId, string tenantId);

    /// <summary>
    /// Gets list of available specialties with doctor counts
    /// </summary>
    /// <param name="clinicId">Clinic ID</param>
    /// <param name="tenantId">Tenant ID</param>
    /// <returns>List of specialties</returns>
    Task<List<SpecialtyDto>> GetSpecialtiesAsync(Guid clinicId, string tenantId);
}
