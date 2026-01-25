using MedicSoft.Telemedicine.Domain.Entities;

namespace MedicSoft.Telemedicine.Domain.Interfaces;

/// <summary>
/// Repository interface for identity verification
/// </summary>
public interface IIdentityVerificationRepository
{
    Task<IdentityVerification?> GetByIdAsync(Guid id, string tenantId);
    Task<IdentityVerification?> GetLatestByUserIdAsync(Guid userId, string userType, string tenantId);
    Task<IEnumerable<IdentityVerification>> GetByUserIdAsync(Guid userId, string userType, string tenantId);
    Task<IEnumerable<IdentityVerification>> GetBySessionIdAsync(Guid sessionId, string tenantId);
    Task<IEnumerable<IdentityVerification>> GetPendingVerificationsAsync(string tenantId, int skip = 0, int take = 50);
    Task<IEnumerable<IdentityVerification>> GetExpiredVerificationsAsync(string tenantId, int skip = 0, int take = 50);
    Task<bool> HasValidVerificationAsync(Guid userId, string userType, string tenantId);
    Task AddAsync(IdentityVerification verification);
    Task UpdateAsync(IdentityVerification verification);
    Task DeleteAsync(Guid id, string tenantId);
}
