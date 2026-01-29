using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.SystemAdmin;
using MedicSoft.Application.Services.SystemAdmin;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers.SystemAdmin
{
    /// <summary>
    /// Controller for Real User Monitoring (RUM) and error tracking
    /// </summary>
    [ApiController]
    [Route("api/system-admin/monitoring")]
    public class MonitoringController : BaseController
    {
        private readonly IMonitoringService _monitoringService;

        public MonitoringController(
            ITenantContext tenantContext,
            IMonitoringService monitoringService) : base(tenantContext)
        {
            _monitoringService = monitoringService;
        }

        /// <summary>
        /// Track Real User Monitoring (RUM) metrics from frontend
        /// Note: AllowAnonymous to enable tracking from all users.
        /// Rate limiting should be configured at the API Gateway or infrastructure level.
        /// Consider implementing additional validation or API keys for production.
        /// </summary>
        [HttpPost("rum/metrics")]
        [AllowAnonymous] // Allow tracking from unauthenticated users too
        public async Task<IActionResult> TrackRumMetric([FromBody] RumMetricDto metric)
        {
            if (metric == null || string.IsNullOrWhiteSpace(metric.Metric))
            {
                return BadRequest(new { message = "Invalid metric data" });
            }

            await _monitoringService.TrackRumMetricAsync(metric);
            return Ok(new { success = true });
        }

        /// <summary>
        /// Track frontend errors
        /// Note: AllowAnonymous to enable error tracking from all users.
        /// Rate limiting should be configured at the API Gateway or infrastructure level.
        /// Consider implementing additional validation or API keys for production.
        /// </summary>
        [HttpPost("errors")]
        [AllowAnonymous] // Allow error tracking from unauthenticated users too
        public async Task<IActionResult> TrackError([FromBody] ErrorTrackingDto error)
        {
            if (error == null || string.IsNullOrWhiteSpace(error.Message))
            {
                return BadRequest(new { message = "Invalid error data" });
            }

            await _monitoringService.TrackErrorAsync(error);
            return Ok(new { success = true });
        }

        /// <summary>
        /// Get Web Vitals summary
        /// </summary>
        [HttpGet("web-vitals")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<ActionResult<WebVitalsSummaryDto>> GetWebVitalsSummary(
            [FromQuery] int days = 7)
        {
            if (days < 1 || days > 90)
            {
                return BadRequest(new { message = "Days must be between 1 and 90" });
            }

            var summary = await _monitoringService.GetWebVitalsSummaryAsync(days);
            return Ok(summary);
        }

        /// <summary>
        /// Get top slow pages
        /// </summary>
        [HttpGet("slow-pages")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<ActionResult<List<PagePerformanceDto>>> GetTopSlowPages(
            [FromQuery] int limit = 10)
        {
            if (limit < 1 || limit > 50)
            {
                return BadRequest(new { message = "Limit must be between 1 and 50" });
            }

            var pages = await _monitoringService.GetTopSlowPagesAsync(limit);
            return Ok(pages);
        }
    }
}
