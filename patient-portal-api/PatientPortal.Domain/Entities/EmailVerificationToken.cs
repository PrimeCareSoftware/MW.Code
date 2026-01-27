namespace PatientPortal.Domain.Entities;

/// <summary>
/// Represents an email verification token for validating user email addresses
/// </summary>
public class EmailVerificationToken
{
    public Guid Id { get; set; }
    public Guid PatientUserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime? UsedAt { get; set; }
    
    /// <summary>
    /// Gets whether the token is valid (not expired and not used)
    /// </summary>
    public bool IsValid => !IsUsed && DateTime.UtcNow < ExpiresAt;
}
