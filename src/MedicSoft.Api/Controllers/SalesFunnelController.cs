using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.SalesFunnel;
using MedicSoft.Application.Services;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for tracking sales funnel metrics
    /// All endpoints are unauthenticated to track anonymous user journeys
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]
    public class SalesFunnelController : ControllerBase
    {
        private readonly ISalesFunnelService _salesFunnelService;
        private readonly ILogger<SalesFunnelController> _logger;

        public SalesFunnelController(
            ISalesFunnelService salesFunnelService,
            ILogger<SalesFunnelController> logger)
        {
            _salesFunnelService = salesFunnelService;
            _logger = logger;
        }

        /// <summary>
        /// Track a sales funnel event (unauthenticated)
        /// </summary>
        /// <param name="eventDto">Event tracking data</param>
        /// <returns>Success or failure response</returns>
        [HttpPost("track")]
        [ProducesResponseType(typeof(TrackEventResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(TrackEventResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TrackEventResponseDto>> TrackEvent([FromBody] TrackSalesFunnelEventDto eventDto)
        {
            try
            {
                // Get IP address and user agent from request
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                var result = await _salesFunnelService.TrackEventAsync(eventDto, ipAddress, userAgent);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error tracking sales funnel event");
                return BadRequest(new TrackEventResponseDto
                {
                    Success = false,
                    Message = "Failed to track event"
                });
            }
        }

        /// <summary>
        /// Mark a session as converted after successful registration
        /// This should be called internally after registration completes
        /// </summary>
        /// <param name="conversionDto">Conversion data</param>
        /// <returns>Success or failure response</returns>
        [HttpPost("convert")]
        [ProducesResponseType(typeof(TrackEventResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(TrackEventResponseDto), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TrackEventResponseDto>> MarkConversion([FromBody] MarkConversionDto conversionDto)
        {
            try
            {
                var result = await _salesFunnelService.MarkConversionAsync(conversionDto);

                if (result.Success)
                {
                    return Ok(result);
                }

                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking conversion");
                return BadRequest(new TrackEventResponseDto
                {
                    Success = false,
                    Message = "Failed to mark conversion"
                });
            }
        }

        /// <summary>
        /// Get funnel statistics (authenticated - for system admin)
        /// </summary>
        /// <param name="startDate">Optional start date for filtering</param>
        /// <param name="endDate">Optional end date for filtering</param>
        /// <returns>Funnel statistics</returns>
        [HttpGet("stats")]
        [Authorize] // Require authentication for viewing stats
        [ProducesResponseType(typeof(FunnelStatsDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<FunnelStatsDto>> GetStats(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var stats = await _salesFunnelService.GetFunnelStatsAsync(startDate, endDate);
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting funnel stats");
                return BadRequest(new { message = "Failed to retrieve statistics" });
            }
        }

        /// <summary>
        /// Get incomplete sessions (authenticated - for system admin)
        /// </summary>
        /// <param name="hoursOld">Get sessions older than this many hours</param>
        /// <param name="limit">Maximum number of sessions to return</param>
        /// <returns>List of incomplete sessions</returns>
        [HttpGet("incomplete")]
        [Authorize] // Require authentication for viewing incomplete sessions
        [ProducesResponseType(typeof(IEnumerable<IncompleteSessionDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<IncompleteSessionDto>>> GetIncompleteSessions(
            [FromQuery] int hoursOld = 24,
            [FromQuery] int limit = 100)
        {
            try
            {
                var sessions = await _salesFunnelService.GetIncompleteSessions(hoursOld, limit);
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting incomplete sessions");
                return BadRequest(new { message = "Failed to retrieve incomplete sessions" });
            }
        }

        /// <summary>
        /// Get all metrics for a specific session (authenticated - for system admin)
        /// </summary>
        /// <param name="sessionId">Session identifier</param>
        /// <returns>List of metrics for the session</returns>
        [HttpGet("session/{sessionId}")]
        [Authorize] // Require authentication for viewing session details
        [ProducesResponseType(typeof(IEnumerable<SalesFunnelMetricDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SalesFunnelMetricDto>>> GetSessionMetrics(string sessionId)
        {
            try
            {
                var metrics = await _salesFunnelService.GetSessionMetricsAsync(sessionId);
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting session metrics for {SessionId}", sessionId);
                return BadRequest(new { message = "Failed to retrieve session metrics" });
            }
        }

        /// <summary>
        /// Get recent sessions (authenticated - for system admin)
        /// </summary>
        /// <param name="limit">Maximum number of sessions to return</param>
        /// <returns>List of recent sessions</returns>
        [HttpGet("recent")]
        [Authorize] // Require authentication for viewing recent sessions
        [ProducesResponseType(typeof(IEnumerable<SalesFunnelMetricDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<SalesFunnelMetricDto>>> GetRecentSessions([FromQuery] int limit = 100)
        {
            try
            {
                var sessions = await _salesFunnelService.GetRecentSessions(limit);
                return Ok(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting recent sessions");
                return BadRequest(new { message = "Failed to retrieve recent sessions" });
            }
        }
    }
}
