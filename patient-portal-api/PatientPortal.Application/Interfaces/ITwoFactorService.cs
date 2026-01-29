namespace PatientPortal.Application.Interfaces;

/// <summary>
/// Service for managing two-factor authentication
/// </summary>
public interface ITwoFactorService
{
    /// <summary>
    /// Enables two-factor authentication for a patient user
    /// </summary>
    Task<bool> EnableTwoFactorAsync(Guid patientUserId);

    /// <summary>
    /// Disables two-factor authentication for a patient user
    /// </summary>
    Task<bool> DisableTwoFactorAsync(Guid patientUserId);

    /// <summary>
    /// Generates a 2FA code and sends it to the user's email
    /// </summary>
    /// <returns>A temporary token to be used in the verification step</returns>
    Task<string> GenerateAndSendCodeAsync(Guid patientUserId, string purpose, string ipAddress);

    /// <summary>
    /// Verifies a 2FA code entered by the user
    /// </summary>
    Task<bool> VerifyCodeAsync(Guid patientUserId, string code, string tempToken);

    /// <summary>
    /// Checks if two-factor authentication is enabled for a user
    /// </summary>
    Task<bool> IsTwoFactorEnabledAsync(Guid patientUserId);

    /// <summary>
    /// Resends the 2FA code to the user's email
    /// </summary>
    Task<bool> ResendCodeAsync(string tempToken, string ipAddress);
}
