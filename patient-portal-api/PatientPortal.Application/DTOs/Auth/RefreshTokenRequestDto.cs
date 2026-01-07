using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Application.DTOs.Auth;

/// <summary>
/// DTO for refreshing access token
/// </summary>
public class RefreshTokenRequestDto
{
    [Required(ErrorMessage = "Refresh token é obrigatório")]
    public string RefreshToken { get; set; } = string.Empty;
}
