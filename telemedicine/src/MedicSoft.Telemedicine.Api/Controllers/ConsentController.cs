using Microsoft.AspNetCore.Mvc;
using MedicSoft.Telemedicine.Application.DTOs;
using MedicSoft.Telemedicine.Application.Interfaces;

namespace MedicSoft.Telemedicine.Api.Controllers;

/// <summary>
/// Telemedicine consent management endpoints
/// Required for CFM 2.314/2022 compliance
/// </summary>
[ApiController]
[Route("api/telemedicine/[controller]")]
[Produces("application/json")]
public class ConsentController : ControllerBase
{
    private readonly ITelemedicineService _telemedicineService;
    private readonly ILogger<ConsentController> _logger;

    public ConsentController(
        ITelemedicineService telemedicineService,
        ILogger<ConsentController> logger)
    {
        _telemedicineService = telemedicineService ?? throw new ArgumentNullException(nameof(telemedicineService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Records patient consent for telemedicine (CFM 2.314 requirement)
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ConsentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ConsentResponse>> RecordConsent(
        [FromBody] CreateConsentRequest request,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            // Get IP address and user agent from request
            var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
            var userAgent = HttpContext.Request.Headers.UserAgent.ToString();

            var result = await _telemedicineService.RecordConsentAsync(request, ipAddress, userAgent, tenantId);
            return CreatedAtAction(nameof(GetConsentById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument while recording consent");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error recording consent");
            return StatusCode(500, "An error occurred while recording consent");
        }
    }

    /// <summary>
    /// Gets consent by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ConsentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConsentResponse>> GetConsentById(
        Guid id,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.GetConsentByIdAsync(id, tenantId);
            return result != null ? Ok(result) : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting consent {ConsentId}", id);
            return StatusCode(500, "An error occurred while retrieving consent");
        }
    }

    /// <summary>
    /// Gets all consents for a patient
    /// </summary>
    [HttpGet("patient/{patientId}")]
    [ProducesResponseType(typeof(IEnumerable<ConsentResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<ConsentResponse>>> GetPatientConsents(
        Guid patientId,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId,
        [FromQuery] bool activeOnly = true)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.GetPatientConsentsAsync(patientId, tenantId, activeOnly);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting consents for patient {PatientId}", patientId);
            return StatusCode(500, "An error occurred while retrieving consents");
        }
    }

    /// <summary>
    /// Checks if patient has valid active consent
    /// </summary>
    [HttpGet("patient/{patientId}/has-valid-consent")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    public async Task<ActionResult<bool>> HasValidConsent(
        Guid patientId,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.HasValidConsentAsync(patientId, tenantId);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking consent for patient {PatientId}", patientId);
            return StatusCode(500, "An error occurred while checking consent");
        }
    }

    /// <summary>
    /// Gets most recent consent for a patient
    /// </summary>
    [HttpGet("patient/{patientId}/most-recent")]
    [ProducesResponseType(typeof(ConsentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConsentResponse>> GetMostRecentConsent(
        Guid patientId,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.GetMostRecentConsentAsync(patientId, tenantId);
            return result != null ? Ok(result) : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting most recent consent for patient {PatientId}", patientId);
            return StatusCode(500, "An error occurred while retrieving consent");
        }
    }

    /// <summary>
    /// Revokes an existing consent
    /// </summary>
    [HttpPost("{id}/revoke")]
    [ProducesResponseType(typeof(ConsentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ConsentResponse>> RevokeConsent(
        Guid id,
        [FromBody] RevokeConsentRequest request,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.RevokeConsentAsync(id, request.Reason, tenantId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while revoking consent {ConsentId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking consent {ConsentId}", id);
            return StatusCode(500, "An error occurred while revoking consent");
        }
    }

    /// <summary>
    /// Validates first appointment rule (CFM 2.314 requirement)
    /// </summary>
    [HttpPost("validate-first-appointment")]
    [ProducesResponseType(typeof(FirstAppointmentValidationResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<FirstAppointmentValidationResponse>> ValidateFirstAppointment(
        [FromBody] ValidateFirstAppointmentRequest request,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.ValidateFirstAppointmentAsync(request, tenantId);
            return Ok(result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument while validating first appointment");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating first appointment");
            return StatusCode(500, "An error occurred while validating first appointment");
        }
    }

    /// <summary>
    /// Gets the CFM 2.314 consent text
    /// </summary>
    [HttpGet("consent-text")]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public ActionResult<object> GetConsentText([FromQuery] bool includeRecording = false)
    {
        try
        {
            var consentText = ConsentTexts.TelemedicineConsentText;
            if (includeRecording)
            {
                consentText += "\n\n" + ConsentTexts.RecordingConsentText;
            }

            return Ok(new { consentText });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting consent text");
            return StatusCode(500, "An error occurred while retrieving consent text");
        }
    }
}
