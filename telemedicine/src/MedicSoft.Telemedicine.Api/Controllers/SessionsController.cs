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
    /// CFM 2.314/2022: Validates consent and identity verification before starting
    /// </summary>
    [HttpPost("{sessionId}/start")]
    [ProducesResponseType(typeof(SessionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<SessionResponse>> StartSession(
        Guid sessionId,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            // Get session to validate participants
            var session = await _telemedicineService.GetSessionByIdAsync(sessionId, tenantId);
            if (session == null)
                return NotFound($"Session {sessionId} not found");

            // CFM 2.314 Validation: Check patient consent
            var hasPatientConsent = await _telemedicineService.HasValidConsentAsync(session.PatientId, tenantId);
            if (!hasPatientConsent)
            {
                _logger.LogWarning("Session {SessionId} cannot start: Patient {PatientId} has no valid consent", sessionId, session.PatientId);
                return BadRequest(new
                {
                    error = "CFM_2314_NO_CONSENT",
                    message = "Paciente não possui consentimento válido para teleconsulta. O consentimento é obrigatório conforme Resolução CFM 2.314/2022.",
                    patientId = session.PatientId
                });
            }

            // CFM 2.314 Validation: Check provider identity verification
            var hasProviderVerification = await _telemedicineService.HasValidIdentityVerificationAsync(session.ProviderId, "Provider", tenantId);
            if (!hasProviderVerification)
            {
                _logger.LogWarning("Session {SessionId} cannot start: Provider {ProviderId} identity not verified", sessionId, session.ProviderId);
                return BadRequest(new
                {
                    error = "CFM_2314_PROVIDER_NOT_VERIFIED",
                    message = "Identidade do médico não verificada. A verificação bidirecional é obrigatória conforme Resolução CFM 2.314/2022.",
                    providerId = session.ProviderId
                });
            }

            // CFM 2.314 Validation: Check patient identity verification
            var hasPatientVerification = await _telemedicineService.HasValidIdentityVerificationAsync(session.PatientId, "Patient", tenantId);
            if (!hasPatientVerification)
            {
                _logger.LogWarning("Session {SessionId} cannot start: Patient {PatientId} identity not verified", sessionId, session.PatientId);
                return BadRequest(new
                {
                    error = "CFM_2314_PATIENT_NOT_VERIFIED",
                    message = "Identidade do paciente não verificada. A verificação bidirecional é obrigatória conforme Resolução CFM 2.314/2022.",
                    patientId = session.PatientId
                });
            }

            // All validations passed, start the session
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
    /// Validates CFM 2.314 compliance for a session before starting
    /// Checks consent and identity verification for both participants
    /// </summary>
    [HttpGet("{sessionId}/validate-compliance")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<object>> ValidateSessionCompliance(
        Guid sessionId,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var session = await _telemedicineService.GetSessionByIdAsync(sessionId, tenantId);
            if (session == null)
                return NotFound($"Session {sessionId} not found");

            var hasPatientConsent = await _telemedicineService.HasValidConsentAsync(session.PatientId, tenantId);
            var hasProviderVerification = await _telemedicineService.HasValidIdentityVerificationAsync(session.ProviderId, "Provider", tenantId);
            var hasPatientVerification = await _telemedicineService.HasValidIdentityVerificationAsync(session.PatientId, "Patient", tenantId);

            var isCompliant = hasPatientConsent && hasProviderVerification && hasPatientVerification;

            var result = new
            {
                sessionId,
                isCompliant,
                compliance = new
                {
                    patientConsent = new
                    {
                        isValid = hasPatientConsent,
                        required = true,
                        message = hasPatientConsent 
                            ? "Consentimento válido" 
                            : "Consentimento necessário (CFM 2.314/2022)"
                    },
                    providerIdentity = new
                    {
                        isVerified = hasProviderVerification,
                        required = true,
                        message = hasProviderVerification 
                            ? "Identidade verificada" 
                            : "Verificação de identidade necessária (CFM 2.314/2022)"
                    },
                    patientIdentity = new
                    {
                        isVerified = hasPatientVerification,
                        required = true,
                        message = hasPatientVerification 
                            ? "Identidade verificada" 
                            : "Verificação de identidade necessária (CFM 2.314/2022)"
                    }
                },
                canStart = isCompliant
            };

            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating compliance for session {SessionId}", sessionId);
            return StatusCode(500, "An error occurred while validating session compliance");
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
