using PatientPortal.Domain.Entities;
using PatientPortal.Domain.Enums;

namespace PatientPortal.Domain.Interfaces;

/// <summary>
/// Repository interface for AppointmentView (read-only)
/// </summary>
public interface IAppointmentViewRepository
{
    Task<AppointmentView?> GetByIdAsync(Guid id, Guid patientId);
    Task<List<AppointmentView>> GetByPatientIdAsync(Guid patientId, int skip = 0, int take = 50);
    Task<List<AppointmentView>> GetUpcomingByPatientIdAsync(Guid patientId, int take = 10);
    Task<List<AppointmentView>> GetByStatusAsync(Guid patientId, AppointmentStatus status, int skip = 0, int take = 50);
    Task<int> GetCountByPatientIdAsync(Guid patientId);
}
