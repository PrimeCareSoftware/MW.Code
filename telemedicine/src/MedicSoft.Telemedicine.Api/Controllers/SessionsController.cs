using Microsoft.AspNetCore.Mvc;
using MedicSoft.Telemedicine.Application.DTOs;
using MedicSoft.Telemedicine.Application.Interfaces;

namespace MedicSoft.Telemedicine.Api.Controllers;

/// <summary>
/// Telemedicine session management endpoints
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class SessionsController : ControllerBase
{
    private readonly ITelemedicineService _telemedicineService;
    private readonly ILogger<SessionsController> _logger;

    public SessionsController(
        ITelemedicineService telemedicineService,
        ILogger<SessionsController> logger)
    {
        _telemedicineService = telemedicineService ?? throw new ArgumentNullException(nameof(telemedicineService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a new telemedicine session for an appointment
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SessionResponse>> CreateSession(
        [FromBody] CreateSessionRequest request,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.CreateSessionAsync(request, tenantId);
            return CreatedAtAction(nameof(GetSessionById), new { id = result.Id }, result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while creating session");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating session");
            return StatusCode(500, "An error occurred while creating the session");
        }
    }

    /// <summary>
    /// Generates access token to join a session
    /// </summary>
    [HttpPost("{sessionId}/join")]
    [ProducesResponseType(typeof(JoinSessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<JoinSessionResponse>> JoinSession(
        Guid sessionId,
        [FromBody] JoinSessionRequest request,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            request.SessionId = sessionId;
            var result = await _telemedicineService.JoinSessionAsync(request, tenantId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while joining session {SessionId}", sessionId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error joining session {SessionId}", sessionId);
            return StatusCode(500, "An error occurred while joining the session");
        }
    }

    /// <summary>
    /// Starts a scheduled session
    /// </summary>
    [HttpPost("{sessionId}/start")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SessionResponse>> StartSession(
        Guid sessionId,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.StartSessionAsync(sessionId, tenantId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while starting session {SessionId}", sessionId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting session {SessionId}", sessionId);
            return StatusCode(500, "An error occurred while starting the session");
        }
    }

    /// <summary>
    /// Completes an active session
    /// </summary>
    [HttpPost("{sessionId}/complete")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SessionResponse>> CompleteSession(
        Guid sessionId,
        [FromBody] CompleteSessionRequest request,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            request.SessionId = sessionId;
            var result = await _telemedicineService.CompleteSessionAsync(request, tenantId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while completing session {SessionId}", sessionId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing session {SessionId}", sessionId);
            return StatusCode(500, "An error occurred while completing the session");
        }
    }

    /// <summary>
    /// Cancels a session
    /// </summary>
    [HttpPost("{sessionId}/cancel")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SessionResponse>> CancelSession(
        Guid sessionId,
        [FromBody] string reason,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.CancelSessionAsync(sessionId, reason, tenantId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while cancelling session {SessionId}", sessionId);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling session {SessionId}", sessionId);
            return StatusCode(500, "An error occurred while cancelling the session");
        }
    }

    /// <summary>
    /// Gets session by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SessionResponse>> GetSessionById(
        Guid id,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.GetSessionByIdAsync(id, tenantId);
            return result != null ? Ok(result) : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting session {SessionId}", id);
            return StatusCode(500, "An error occurred while retrieving the session");
        }
    }

    /// <summary>
    /// Gets session by appointment ID
    /// </summary>
    [HttpGet("appointment/{appointmentId}")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SessionResponse>> GetSessionByAppointmentId(
        Guid appointmentId,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.GetSessionByAppointmentIdAsync(appointmentId, tenantId);
            return result != null ? Ok(result) : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting session for appointment {AppointmentId}", appointmentId);
            return StatusCode(500, "An error occurred while retrieving the session");
        }
    }

    /// <summary>
    /// Gets all sessions for a clinic
    /// </summary>
    [HttpGet("clinic/{clinicId}")]
    [ProducesResponseType(typeof(IEnumerable<SessionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SessionResponse>>> GetClinicSessions(
        Guid clinicId,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.GetClinicSessionsAsync(clinicId, tenantId, skip, take);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sessions for clinic {ClinicId}", clinicId);
            return StatusCode(500, "An error occurred while retrieving sessions");
        }
    }

    /// <summary>
    /// Gets all sessions for a provider
    /// </summary>
    [HttpGet("provider/{providerId}")]
    [ProducesResponseType(typeof(IEnumerable<SessionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SessionResponse>>> GetProviderSessions(
        Guid providerId,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.GetProviderSessionsAsync(providerId, tenantId, skip, take);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sessions for provider {ProviderId}", providerId);
            return StatusCode(500, "An error occurred while retrieving sessions");
        }
    }

    /// <summary>
    /// Gets all sessions for a patient
    /// </summary>
    [HttpGet("patient/{patientId}")]
    [ProducesResponseType(typeof(IEnumerable<SessionResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<SessionResponse>>> GetPatientSessions(
        Guid patientId,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.GetPatientSessionsAsync(patientId, tenantId, skip, take);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sessions for patient {PatientId}", patientId);
            return StatusCode(500, "An error occurred while retrieving sessions");
        }
    }
}
