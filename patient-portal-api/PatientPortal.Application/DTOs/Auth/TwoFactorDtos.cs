namespace PatientPortal.Application.DTOs.Auth;

/// <summary>
/// Response DTO when 2FA is required during login
/// </summary>
public class TwoFactorRequiredDto
{
    public bool RequiresTwoFactor { get; set; } = true;
    public string TempToken { get; set; } = string.Empty;
    public string Message { get; set; } = "Código de verificação enviado para seu e-mail";
}

/// <summary>
/// Request DTO to verify 2FA code
/// </summary>
public class VerifyTwoFactorDto
{
    public string TempToken { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
}

/// <summary>
/// Request DTO to resend 2FA code
/// </summary>
public class ResendTwoFactorCodeDto
{
    public string TempToken { get; set; } = string.Empty;
}

/// <summary>
/// Response DTO for 2FA status
/// </summary>
public class TwoFactorStatusDto
{
    public bool IsEnabled { get; set; }
    public string Message { get; set; } = string.Empty;
}
