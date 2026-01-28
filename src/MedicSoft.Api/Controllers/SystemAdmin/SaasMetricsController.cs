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
    /// Controller for SaaS metrics and analytics
    /// </summary>
    [ApiController]
    [Route("api/system-admin/saas-metrics")]
    [Authorize(Roles = "SystemAdmin")]
    public class SaasMetricsController : BaseController
    {
        private readonly ISaasMetricsService _metricsService;

        public SaasMetricsController(
            ITenantContext tenantContext,
            ISaasMetricsService metricsService) : base(tenantContext)
        {
            _metricsService = metricsService;
        }

        /// <summary>
        /// Get comprehensive SaaS dashboard metrics
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<ActionResult<SaasDashboardDto>> GetDashboard()
        {
            var metrics = await _metricsService.GetSaasDashboardMetricsAsync();
            return Ok(metrics);
        }

        /// <summary>
        /// Get detailed MRR breakdown
        /// </summary>
        [HttpGet("mrr-breakdown")]
        public async Task<ActionResult<MrrBreakdownDto>> GetMrrBreakdown()
        {
            var breakdown = await _metricsService.GetMrrBreakdownAsync();
            return Ok(breakdown);
        }

        /// <summary>
        /// Get churn analysis
        /// </summary>
        [HttpGet("churn-analysis")]
        public async Task<ActionResult<ChurnAnalysisDto>> GetChurnAnalysis()
        {
            var analysis = await _metricsService.GetChurnAnalysisAsync();
            return Ok(analysis);
        }

        /// <summary>
        /// Get growth metrics
        /// </summary>
        [HttpGet("growth")]
        public async Task<ActionResult<GrowthMetricsDto>> GetGrowthMetrics()
        {
            var metrics = await _metricsService.GetGrowthMetricsAsync();
            return Ok(metrics);
        }

        /// <summary>
        /// Get revenue timeline
        /// </summary>
        [HttpGet("revenue-timeline")]
        public async Task<ActionResult<List<RevenueTimelineDto>>> GetRevenueTimeline(
            [FromQuery] int months = 12)
        {
            if (months < 1 || months > 36)
            {
                return BadRequest(new { message = "Months must be between 1 and 36" });
            }
            
            var timeline = await _metricsService.GetRevenueTimelineAsync(months);
            return Ok(timeline);
        }

        /// <summary>
        /// Get customer breakdown by plan and status
        /// </summary>
        [HttpGet("customer-breakdown")]
        public async Task<ActionResult<CustomerBreakdownDto>> GetCustomerBreakdown()
        {
            var breakdown = await _metricsService.GetCustomerBreakdownAsync();
            return Ok(breakdown);
        }
    }
}
