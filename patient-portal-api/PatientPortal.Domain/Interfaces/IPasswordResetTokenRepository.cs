using PatientPortal.Domain.Entities;

namespace PatientPortal.Domain.Interfaces;

/// <summary>
/// Repository interface for password reset tokens
/// </summary>
public interface IPasswordResetTokenRepository
{
    Task<PasswordResetToken> CreateAsync(PasswordResetToken token);
    Task<PasswordResetToken?> GetByTokenAsync(string token);
    Task UpdateAsync(PasswordResetToken token);
    Task<bool> HasValidTokenAsync(Guid patientUserId);
    Task RevokeAllActiveTokensAsync(Guid patientUserId);
}
