namespace MedicSoft.Telemedicine.Application.Interfaces;

/// <summary>
/// Service for validating CRM and CPF with CFM (Conselho Federal de Medicina) API
/// API Reference: https://siem-servicos-api.cfm.org.br/swagger-ui/index.html
/// </summary>
public interface ICfmValidationService
{
    /// <summary>
    /// Validates a CRM (medical registration) with the CFM API
    /// </summary>
    /// <param name="crmNumber">CRM number</param>
    /// <param name="crmState">State code (UF)</param>
    /// <returns>Validation result with detailed information</returns>
    Task<CfmCrmValidationResult> ValidateCrmAsync(string crmNumber, string crmState);

    /// <summary>
    /// Validates a CPF (Brazilian tax ID) with the CFM API
    /// </summary>
    /// <param name="cpf">CPF number (with or without formatting)</param>
    /// <returns>Validation result</returns>
    Task<CfmCpfValidationResult> ValidateCpfAsync(string cpf);
}

/// <summary>
/// Result of CRM validation with CFM API
/// </summary>
public class CfmCrmValidationResult
{
    public bool IsValid { get; set; }
    public string? DoctorName { get; set; }
    public string? CrmNumber { get; set; }
    public string? CrmState { get; set; }
    public string? Specialty { get; set; }
    public string? Status { get; set; }
    public DateTime? RegistrationDate { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Result of CPF validation with CFM API
/// </summary>
public class CfmCpfValidationResult
{
    public bool IsValid { get; set; }
    public string? Cpf { get; set; }
    public string? ErrorMessage { get; set; }
}
