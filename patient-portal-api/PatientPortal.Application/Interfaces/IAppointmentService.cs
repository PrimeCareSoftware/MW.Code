using PatientPortal.Application.DTOs.Appointments;
using PatientPortal.Domain.Enums;

namespace PatientPortal.Application.Interfaces;

/// <summary>
/// Appointment service interface
/// </summary>
public interface IAppointmentService
{
    Task<AppointmentDto?> GetByIdAsync(Guid appointmentId, Guid patientUserId);
    Task<List<AppointmentDto>> GetMyAppointmentsAsync(Guid patientUserId, int skip = 0, int take = 50);
    Task<List<AppointmentDto>> GetUpcomingAppointmentsAsync(Guid patientUserId, int take = 10);
    Task<List<AppointmentDto>> GetByStatusAsync(Guid patientUserId, AppointmentStatus status, int skip = 0, int take = 50);
    Task<int> GetCountAsync(Guid patientUserId);
}
