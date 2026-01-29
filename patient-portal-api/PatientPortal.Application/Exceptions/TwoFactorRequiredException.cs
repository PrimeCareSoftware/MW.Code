namespace PatientPortal.Application.Exceptions;

/// <summary>
/// Exception thrown when 2FA is required during login
/// </summary>
public class TwoFactorRequiredException : Exception
{
    public string TempToken { get; set; } = string.Empty;
    
    public TwoFactorRequiredException() : base("Two-factor authentication required")
    {
    }
}
