using PatientPortal.Domain.Entities;

namespace PatientPortal.Domain.Interfaces;

/// <summary>
/// Repository interface for email verification tokens
/// </summary>
public interface IEmailVerificationTokenRepository
{
    Task<EmailVerificationToken> CreateAsync(EmailVerificationToken token);
    Task<EmailVerificationToken?> GetByTokenAsync(string token);
    Task<EmailVerificationToken?> GetLatestByPatientUserIdAsync(Guid patientUserId);
    Task UpdateAsync(EmailVerificationToken token);
    Task<bool> HasValidTokenAsync(Guid patientUserId);
}
