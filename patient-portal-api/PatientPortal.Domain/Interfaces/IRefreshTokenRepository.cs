using PatientPortal.Domain.Entities;

namespace PatientPortal.Domain.Interfaces;

/// <summary>
/// Repository interface for RefreshToken entity
/// </summary>
public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<List<RefreshToken>> GetActiveTokensByUserIdAsync(Guid patientUserId);
    Task<RefreshToken> CreateAsync(RefreshToken refreshToken);
    Task UpdateAsync(RefreshToken refreshToken);
    Task RevokeAllUserTokensAsync(Guid patientUserId, string revokedByIp, string reason);
}
