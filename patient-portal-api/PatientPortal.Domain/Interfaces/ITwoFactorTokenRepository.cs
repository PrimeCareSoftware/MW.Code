using PatientPortal.Domain.Entities;

namespace PatientPortal.Domain.Interfaces;

/// <summary>
/// Repository interface for managing two-factor authentication tokens
/// </summary>
public interface ITwoFactorTokenRepository
{
    /// <summary>
    /// Creates a new two-factor token
    /// </summary>
    Task<TwoFactorToken> CreateAsync(TwoFactorToken token);

    /// <summary>
    /// Gets a token by code and patient user ID
    /// </summary>
    Task<TwoFactorToken?> GetByCodeAsync(string code, Guid patientUserId);

    /// <summary>
    /// Gets the most recent valid token for a patient user
    /// </summary>
    Task<TwoFactorToken?> GetMostRecentValidTokenAsync(Guid patientUserId, string purpose);

    /// <summary>
    /// Updates an existing token
    /// </summary>
    Task UpdateAsync(TwoFactorToken token);

    /// <summary>
    /// Deletes expired tokens (cleanup job)
    /// </summary>
    Task DeleteExpiredTokensAsync();

    /// <summary>
    /// Counts how many tokens were created for a user in the last hour
    /// </summary>
    Task<int> CountRecentTokensAsync(Guid patientUserId, TimeSpan timeWindow);
}
