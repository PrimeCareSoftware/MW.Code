using Microsoft.AspNetCore.Mvc;
using MedicSoft.Telemedicine.Application.Interfaces;

namespace MedicSoft.Telemedicine.Api.Controllers;

/// <summary>
/// CFM (Conselho Federal de Medicina) validation endpoints
/// Validates CRM and CPF against the official CFM database
/// </summary>
[ApiController]
[Route("api/telemedicine/[controller]")]
[Produces("application/json")]
public class CfmValidationController : ControllerBase
{
    private readonly ICfmValidationService _cfmValidationService;
    private readonly ILogger<CfmValidationController> _logger;

    public CfmValidationController(
        ICfmValidationService cfmValidationService,
        ILogger<CfmValidationController> logger)
    {
        _cfmValidationService = cfmValidationService ?? throw new ArgumentNullException(nameof(cfmValidationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Validates a CRM (medical registration) with the CFM API
    /// </summary>
    /// <param name="crmNumber">CRM number (digits only)</param>
    /// <param name="crmState">State code (e.g., SP, RJ, MG)</param>
    /// <returns>Validation result with doctor information if found</returns>
    [HttpGet("crm/{crmNumber}/{crmState}")]
    [ProducesResponseType(typeof(CfmCrmValidationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CfmCrmValidationResult>> ValidateCrm(
        [FromRoute] string crmNumber,
        [FromRoute] string crmState)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(crmNumber))
                return BadRequest("CRM number is required");

            if (string.IsNullOrWhiteSpace(crmState))
                return BadRequest("CRM state is required");

            var result = await _cfmValidationService.ValidateCrmAsync(crmNumber, crmState);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating CRM {CrmNumber}-{CrmState}", crmNumber, crmState);
            return StatusCode(500, new { error = "An error occurred while validating CRM" });
        }
    }

    /// <summary>
    /// Validates a CPF (Brazilian tax ID) with the CFM API
    /// </summary>
    /// <param name="cpf">CPF number (with or without formatting)</param>
    /// <returns>Validation result</returns>
    [HttpGet("cpf/{cpf}")]
    [ProducesResponseType(typeof(CfmCpfValidationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CfmCpfValidationResult>> ValidateCpf([FromRoute] string cpf)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(cpf))
                return BadRequest("CPF is required");

            var result = await _cfmValidationService.ValidateCpfAsync(cpf);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating CPF");
            return StatusCode(500, new { error = "An error occurred while validating CPF" });
        }
    }

    /// <summary>
    /// Validates CRM and CPF together for identity verification
    /// </summary>
    /// <param name="request">Validation request with CRM and CPF</param>
    /// <returns>Combined validation result</returns>
    [HttpPost("validate-identity")]
    [ProducesResponseType(typeof(CfmIdentityValidationResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CfmIdentityValidationResult>> ValidateIdentity(
        [FromBody] ValidateIdentityRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.CrmNumber))
                return BadRequest("CRM number is required");

            if (string.IsNullOrWhiteSpace(request.CrmState))
                return BadRequest("CRM state is required");

            if (string.IsNullOrWhiteSpace(request.Cpf))
                return BadRequest("CPF is required");

            // Validate both in parallel for better performance
            var crmTask = _cfmValidationService.ValidateCrmAsync(request.CrmNumber, request.CrmState);
            var cpfTask = _cfmValidationService.ValidateCpfAsync(request.Cpf);

            await Task.WhenAll(crmTask, cpfTask);

            var crmResult = crmTask.Result;
            var cpfResult = cpfTask.Result;

            var result = new CfmIdentityValidationResult
            {
                IsValid = crmResult.IsValid && cpfResult.IsValid,
                CrmValidation = crmResult,
                CpfValidation = cpfResult
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating identity");
            return StatusCode(500, new { error = "An error occurred while validating identity" });
        }
    }
}

/// <summary>
/// Request for identity validation
/// </summary>
public class ValidateIdentityRequest
{
    public string CrmNumber { get; set; } = string.Empty;
    public string CrmState { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
}

/// <summary>
/// Combined validation result for CRM and CPF
/// </summary>
public class CfmIdentityValidationResult
{
    public bool IsValid { get; set; }
    public CfmCrmValidationResult CrmValidation { get; set; } = new();
    public CfmCpfValidationResult CpfValidation { get; set; } = new();
}
