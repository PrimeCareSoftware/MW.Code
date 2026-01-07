using PatientPortal.Domain.Entities;

namespace PatientPortal.Domain.Interfaces;

/// <summary>
/// Repository interface for PatientUser entity
/// </summary>
public interface IPatientUserRepository
{
    Task<PatientUser?> GetByIdAsync(Guid id);
    Task<PatientUser?> GetByEmailAsync(string email);
    Task<PatientUser?> GetByCPFAsync(string cpf);
    Task<PatientUser?> GetByPatientIdAsync(Guid patientId);
    Task<PatientUser> CreateAsync(PatientUser patientUser);
    Task UpdateAsync(PatientUser patientUser);
    Task<bool> ExistsAsync(string email);
    Task<bool> ExistsByCPFAsync(string cpf);
}
