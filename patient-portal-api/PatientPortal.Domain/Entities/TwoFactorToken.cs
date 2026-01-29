namespace PatientPortal.Domain.Entities;

/// <summary>
/// Represents a temporary two-factor authentication token sent via email
/// </summary>
public class TwoFactorToken
{
    public Guid Id { get; set; }
    public Guid PatientUserId { get; set; }
    public string Code { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public string Purpose { get; set; } = string.Empty; // "Login" or "Verification"
    public string IpAddress { get; set; } = string.Empty;
    public DateTime? UsedAt { get; set; }
    public int VerificationAttempts { get; set; } = 0;

    /// <summary>
    /// Checks if the token is valid (not used and not expired)
    /// </summary>
    public bool IsValid => !IsUsed && DateTime.UtcNow < ExpiresAt && VerificationAttempts < 5;
}
