using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.Reports;
using MedicSoft.Application.Services.Reports;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers.SystemAdmin
{
    /// <summary>
    /// Controller for report template and scheduled report management
    /// Phase 3: Analytics and BI
    /// </summary>
    [ApiController]
    [Route("api/system-admin/reports")]
    [Authorize(Roles = "SystemAdmin")]
    public class ReportsController : BaseController
    {
        private readonly IReportService _reportService;

        public ReportsController(
            ITenantContext tenantContext,
            IReportService reportService) : base(tenantContext)
        {
            _reportService = reportService;
        }

        #region Report Templates

        /// <summary>
        /// Get all report templates
        /// </summary>
        [HttpGet("templates")]
        public async Task<ActionResult<List<ReportTemplateDto>>> GetAllTemplates()
        {
            var templates = await _reportService.GetAllReportTemplatesAsync();
            return Ok(templates);
        }

        /// <summary>
        /// Get report templates by category
        /// </summary>
        [HttpGet("templates/category/{category}")]
        public async Task<ActionResult<List<ReportTemplateDto>>> GetTemplatesByCategory(string category)
        {
            var templates = await _reportService.GetReportTemplatesByCategoryAsync(category);
            return Ok(templates);
        }

        /// <summary>
        /// Get specific report template by ID
        /// </summary>
        [HttpGet("templates/{id}")]
        public async Task<ActionResult<ReportTemplateDto>> GetTemplate(int id)
        {
            var template = await _reportService.GetReportTemplateAsync(id);
            if (template == null)
                return NotFound();

            return Ok(template);
        }

        /// <summary>
        /// Create a new report template
        /// </summary>
        [HttpPost("templates")]
        public async Task<ActionResult<ReportTemplateDto>> CreateTemplate([FromBody] CreateReportTemplateDto dto)
        {
            var template = await _reportService.CreateReportTemplateAsync(dto);
            return CreatedAtAction(nameof(GetTemplate), new { id = template.Id }, template);
        }

        /// <summary>
        /// Update an existing report template
        /// </summary>
        [HttpPut("templates/{id}")]
        public async Task<ActionResult<ReportTemplateDto>> UpdateTemplate(int id, [FromBody] UpdateReportTemplateDto dto)
        {
            var template = await _reportService.UpdateReportTemplateAsync(id, dto);
            if (template == null)
                return NotFound();

            return Ok(template);
        }

        /// <summary>
        /// Delete a report template
        /// </summary>
        [HttpDelete("templates/{id}")]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            await _reportService.DeleteReportTemplateAsync(id);
            return NoContent();
        }

        #endregion

        #region Report Generation

        /// <summary>
        /// Generate a report on-demand
        /// </summary>
        [HttpPost("generate")]
        public async Task<ActionResult<ReportResultDto>> GenerateReport([FromBody] GenerateReportDto dto)
        {
            var result = await _reportService.GenerateReportAsync(dto);
            
            if (!string.IsNullOrEmpty(result.Error))
            {
                return BadRequest(result);
            }

            return File(result.Data, result.ContentType, result.FileName);
        }

        #endregion

        #region Scheduled Reports

        /// <summary>
        /// Get all scheduled reports
        /// </summary>
        [HttpGet("scheduled")]
        public async Task<ActionResult<List<ScheduledReportDto>>> GetAllScheduledReports()
        {
            var scheduledReports = await _reportService.GetAllScheduledReportsAsync();
            return Ok(scheduledReports);
        }

        /// <summary>
        /// Get specific scheduled report by ID
        /// </summary>
        [HttpGet("scheduled/{id}")]
        public async Task<ActionResult<ScheduledReportDto>> GetScheduledReport(int id)
        {
            var scheduledReport = await _reportService.GetScheduledReportAsync(id);
            if (scheduledReport == null)
                return NotFound();

            return Ok(scheduledReport);
        }

        /// <summary>
        /// Create a new scheduled report
        /// </summary>
        [HttpPost("scheduled")]
        public async Task<ActionResult<ScheduledReportDto>> CreateScheduledReport([FromBody] CreateScheduledReportDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var scheduledReport = await _reportService.CreateScheduledReportAsync(dto, userId);
            return CreatedAtAction(nameof(GetScheduledReport), new { id = scheduledReport.Id }, scheduledReport);
        }

        /// <summary>
        /// Update an existing scheduled report
        /// </summary>
        [HttpPut("scheduled/{id}")]
        public async Task<ActionResult<ScheduledReportDto>> UpdateScheduledReport(int id, [FromBody] UpdateScheduledReportDto dto)
        {
            var scheduledReport = await _reportService.UpdateScheduledReportAsync(id, dto);
            if (scheduledReport == null)
                return NotFound();

            return Ok(scheduledReport);
        }

        /// <summary>
        /// Delete a scheduled report
        /// </summary>
        [HttpDelete("scheduled/{id}")]
        public async Task<IActionResult> DeleteScheduledReport(int id)
        {
            await _reportService.DeleteScheduledReportAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Execute a scheduled report immediately (admin action)
        /// </summary>
        [HttpPost("scheduled/{id}/execute")]
        public async Task<IActionResult> ExecuteScheduledReport(int id)
        {
            await _reportService.ExecuteScheduledReportAsync(id);
            return Ok(new { message = "Scheduled report execution initiated" });
        }

        #endregion
    }
}
