using System;
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
    /// Controller for advanced clinic management and CRM features
    /// </summary>
    [ApiController]
    [Route("api/system-admin/clinic-management")]
    [Authorize(Roles = "SystemAdmin")]
    public class ClinicManagementController : BaseController
    {
        private readonly IClinicManagementService _clinicService;

        public ClinicManagementController(
            ITenantContext tenantContext,
            IClinicManagementService clinicService) : base(tenantContext)
        {
            _clinicService = clinicService;
        }

        /// <summary>
        /// Get detailed clinic information with related data
        /// </summary>
        [HttpGet("{id:guid}/detail")]
        public async Task<ActionResult<ClinicDetailDto>> GetDetail(Guid id)
        {
            var detail = await _clinicService.GetClinicDetail(id);
            
            if (detail == null)
                return NotFound(new { message = $"Clinic with ID {id} not found" });

            return Ok(detail);
        }

        /// <summary>
        /// Calculate and get health score for a clinic
        /// </summary>
        [HttpGet("{id:guid}/health-score")]
        public async Task<ActionResult<ClinicHealthScoreDto>> GetHealthScore(Guid id)
        {
            try
            {
                var healthScore = await _clinicService.CalculateHealthScore(id);
                return Ok(healthScore);
            }
            catch (ArgumentException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get timeline events for a clinic
        /// </summary>
        [HttpGet("{id:guid}/timeline")]
        public async Task<ActionResult<List<ClinicTimelineEventDto>>> GetTimeline(
            Guid id,
            [FromQuery] int limit = 50)
        {
            var timeline = await _clinicService.GetTimeline(id, limit);
            return Ok(timeline);
        }

        /// <summary>
        /// Get usage metrics for a clinic
        /// </summary>
        [HttpGet("{id:guid}/usage-metrics")]
        public async Task<ActionResult<ClinicUsageMetricsDto>> GetUsageMetrics(
            Guid id,
            [FromQuery] DateTime? periodStart = null,
            [FromQuery] DateTime? periodEnd = null)
        {
            var metrics = await _clinicService.GetUsageMetrics(id, periodStart, periodEnd);
            return Ok(metrics);
        }

        /// <summary>
        /// Filter clinics with advanced criteria
        /// </summary>
        [HttpPost("filter")]
        public async Task<ActionResult> FilterClinics([FromBody] ClinicFilterDto filters)
        {
            var (clinics, totalCount) = await _clinicService.GetClinicsWithFilters(filters);
            
            return Ok(new
            {
                data = clinics,
                totalCount,
                page = filters.Page,
                pageSize = filters.PageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)filters.PageSize)
            });
        }

        /// <summary>
        /// Get clinics by segment (quick filters)
        /// </summary>
        [HttpGet("segment/{segment}")]
        public async Task<ActionResult> GetBySegment(string segment)
        {
            var filters = new ClinicFilterDto();

            // Apply segment-specific filters
            switch (segment.ToLower())
            {
                case "new":
                    filters.CreatedAfter = DateTime.UtcNow.AddDays(-30);
                    break;
                case "trial":
                    filters.SubscriptionStatus = "Trial";
                    break;
                case "at-risk":
                    filters.HealthStatus = HealthStatus.AtRisk;
                    break;
                case "needs-attention":
                    filters.HealthStatus = HealthStatus.NeedsAttention;
                    break;
                case "healthy":
                    filters.HealthStatus = HealthStatus.Healthy;
                    break;
                case "inactive":
                    filters.IsActive = false;
                    break;
                default:
                    return BadRequest(new { message = $"Unknown segment: {segment}" });
            }

            var (clinics, totalCount) = await _clinicService.GetClinicsWithFilters(filters);
            
            return Ok(new
            {
                segment,
                data = clinics,
                totalCount
            });
        }

        /// <summary>
        /// Perform bulk actions on multiple clinics
        /// </summary>
        [HttpPost("bulk-action")]
        public async Task<ActionResult> BulkAction([FromBody] BulkActionDto actionDto)
        {
            if (actionDto.ClinicIds == null || actionDto.ClinicIds.Count == 0)
                return BadRequest(new { message = "At least one clinic must be selected" });

            try
            {
                var result = await _clinicService.ExecuteBulkAction(actionDto);
                return Ok(result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Export clinics data to various formats (CSV, Excel, PDF)
        /// </summary>
        [HttpPost("export")]
        public async Task<ActionResult> ExportClinics([FromBody] ExportClinicsDto exportDto)
        {
            if (exportDto.ClinicIds == null || exportDto.ClinicIds.Count == 0)
                return BadRequest(new { message = "At least one clinic must be selected for export" });

            try
            {
                var (fileBytes, fileName, contentType) = await _clinicService.ExportClinics(exportDto);
                return File(fileBytes, contentType, fileName);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
