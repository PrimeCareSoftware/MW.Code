using System.ComponentModel.DataAnnotations;

namespace PatientPortal.Application.DTOs.Auth;

/// <summary>
/// DTO for patient login
/// </summary>
public class LoginRequestDto
{
    [Required(ErrorMessage = "Email ou CPF é obrigatório")]
    public string EmailOrCPF { get; set; } = string.Empty;
    
    [Required(ErrorMessage = "Senha é obrigatória")]
    public string Password { get; set; } = string.Empty;
}
