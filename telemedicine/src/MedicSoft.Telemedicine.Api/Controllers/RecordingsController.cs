using Microsoft.AspNetCore.Mvc;
using MedicSoft.Telemedicine.Application.DTOs;
using MedicSoft.Telemedicine.Application.Interfaces;

namespace MedicSoft.Telemedicine.Api.Controllers;

/// <summary>
/// Recording management endpoints
/// Optional CFM 2.314/2022 feature with patient consent
/// </summary>
[ApiController]
[Route("api/telemedicine/[controller]")]
[Produces("application/json")]
public class RecordingsController : ControllerBase
{
    private readonly ITelemedicineService _telemedicineService;
    private readonly ILogger<RecordingsController> _logger;

    public RecordingsController(
        ITelemedicineService telemedicineService,
        ILogger<RecordingsController> logger)
    {
        _telemedicineService = telemedicineService ?? throw new ArgumentNullException(nameof(telemedicineService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Creates a new recording for a session
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(RecordingResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecordingResponse>> CreateRecording(
        [FromBody] CreateRecordingRequest request,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.CreateRecordingAsync(request, tenantId);
            return CreatedAtAction(nameof(GetRecordingById), new { id = result.Id }, result);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument while creating recording");
            return BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while creating recording");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating recording");
            return StatusCode(500, "An error occurred while creating recording");
        }
    }

    /// <summary>
    /// Gets recording by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RecordingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecordingResponse>> GetRecordingById(
        Guid id,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.GetRecordingByIdAsync(id, tenantId);
            return result != null ? Ok(result) : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recording {RecordingId}", id);
            return StatusCode(500, "An error occurred while retrieving recording");
        }
    }

    /// <summary>
    /// Gets recording by session ID
    /// </summary>
    [HttpGet("session/{sessionId}")]
    [ProducesResponseType(typeof(RecordingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RecordingResponse>> GetRecordingBySessionId(
        Guid sessionId,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.GetRecordingBySessionIdAsync(sessionId, tenantId);
            return result != null ? Ok(result) : NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recording for session {SessionId}", sessionId);
            return StatusCode(500, "An error occurred while retrieving recording");
        }
    }

    /// <summary>
    /// Gets available recordings
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<RecordingResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<RecordingResponse>>> GetAvailableRecordings(
        [FromHeader(Name = "X-Tenant-Id")] string tenantId,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 50)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.GetAvailableRecordingsAsync(tenantId, skip, take);
            return Ok(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting available recordings");
            return StatusCode(500, "An error occurred while retrieving recordings");
        }
    }

    /// <summary>
    /// Starts recording
    /// </summary>
    [HttpPost("{id}/start")]
    [ProducesResponseType(typeof(RecordingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecordingResponse>> StartRecording(
        Guid id,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.StartRecordingAsync(id, tenantId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while starting recording {RecordingId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting recording {RecordingId}", id);
            return StatusCode(500, "An error occurred while starting recording");
        }
    }

    /// <summary>
    /// Completes recording
    /// </summary>
    [HttpPost("{id}/complete")]
    [ProducesResponseType(typeof(RecordingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecordingResponse>> CompleteRecording(
        Guid id,
        [FromBody] CompleteRecordingRequest request,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            var result = await _telemedicineService.CompleteRecordingAsync(id, request, tenantId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while completing recording {RecordingId}", id);
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument while completing recording");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error completing recording {RecordingId}", id);
            return StatusCode(500, "An error occurred while completing recording");
        }
    }

    /// <summary>
    /// Marks recording as failed
    /// </summary>
    [HttpPost("{id}/fail")]
    [ProducesResponseType(typeof(RecordingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecordingResponse>> FailRecording(
        Guid id,
        [FromBody] string reason,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            if (string.IsNullOrWhiteSpace(reason))
                return BadRequest("Failure reason is required");

            var result = await _telemedicineService.FailRecordingAsync(id, reason, tenantId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while failing recording {RecordingId}", id);
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error failing recording {RecordingId}", id);
            return StatusCode(500, "An error occurred while failing recording");
        }
    }

    /// <summary>
    /// Deletes recording (LGPD compliance)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(RecordingResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<RecordingResponse>> DeleteRecording(
        Guid id,
        [FromBody] DeleteRecordingRequest request,
        [FromHeader(Name = "X-Tenant-Id")] string tenantId,
        [FromHeader(Name = "X-User-Id")] Guid deletedByUserId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(tenantId))
                return BadRequest("TenantId header is required");

            if (deletedByUserId == Guid.Empty)
                return BadRequest("User ID header is required");

            var result = await _telemedicineService.DeleteRecordingAsync(id, request, deletedByUserId, tenantId);
            return Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Invalid operation while deleting recording {RecordingId}", id);
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Invalid argument while deleting recording");
            return BadRequest(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting recording {RecordingId}", id);
            return StatusCode(500, "An error occurred while deleting recording");
        }
    }
}
