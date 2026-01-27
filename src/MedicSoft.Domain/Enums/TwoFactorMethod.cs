namespace MedicSoft.Domain.Enums
{
    /// <summary>
    /// Represents the method used for two-factor authentication.
    /// </summary>
    public enum TwoFactorMethod
    {
        None = 0,
        TOTP = 1,      // Time-based One-Time Password (Google Authenticator, Microsoft Authenticator)
        SMS = 2,       // SMS-based verification
        Email = 3      // Email-based verification
    }
}
