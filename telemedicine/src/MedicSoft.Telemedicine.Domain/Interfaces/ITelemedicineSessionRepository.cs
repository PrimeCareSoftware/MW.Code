using MedicSoft.Telemedicine.Domain.Entities;
using MedicSoft.Telemedicine.Domain.Enums;

namespace MedicSoft.Telemedicine.Domain.Interfaces;

/// <summary>
/// Repository interface for TelemedicineSession aggregate
/// Follows Repository Pattern from DDD
/// </summary>
public interface ITelemedicineSessionRepository
{
    Task<TelemedicineSession?> GetByIdAsync(Guid id, string tenantId);
    Task<TelemedicineSession?> GetByAppointmentIdAsync(Guid appointmentId, string tenantId);
    Task<IEnumerable<TelemedicineSession>> GetByClinicIdAsync(Guid clinicId, string tenantId, int skip = 0, int take = 50);
    Task<IEnumerable<TelemedicineSession>> GetByProviderIdAsync(Guid providerId, string tenantId, int skip = 0, int take = 50);
    Task<IEnumerable<TelemedicineSession>> GetByPatientIdAsync(Guid patientId, string tenantId, int skip = 0, int take = 50);
    Task<IEnumerable<TelemedicineSession>> GetByStatusAsync(SessionStatus status, string tenantId, int skip = 0, int take = 50);
    Task<TelemedicineSession> AddAsync(TelemedicineSession session);
    Task UpdateAsync(TelemedicineSession session);
    Task<bool> ExistsAsync(Guid id, string tenantId);
}
