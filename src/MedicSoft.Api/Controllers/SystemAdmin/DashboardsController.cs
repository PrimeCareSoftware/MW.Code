using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.Dashboards;
using MedicSoft.Application.Services.Dashboards;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers.SystemAdmin
{
    /// <summary>
    /// Controller for custom dashboard management
    /// Phase 3: Analytics and BI
    /// </summary>
    [ApiController]
    [Route("api/system-admin/dashboards")]
    [Authorize(Roles = "SystemAdmin")]
    public class DashboardsController : BaseController
    {
        private readonly IDashboardService _dashboardService;

        public DashboardsController(
            ITenantContext tenantContext,
            IDashboardService dashboardService) : base(tenantContext)
        {
            _dashboardService = dashboardService;
        }

        /// <summary>
        /// Get all dashboards for the current user
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<CustomDashboardDto>>> GetAll()
        {
            var dashboards = await _dashboardService.GetAllDashboardsAsync();
            return Ok(dashboards);
        }

        /// <summary>
        /// Get specific dashboard by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CustomDashboardDto>> Get(Guid id)
        {
            var dashboard = await _dashboardService.GetDashboardAsync(id);
            if (dashboard == null)
                return NotFound();

            return Ok(dashboard);
        }

        /// <summary>
        /// Create a new dashboard
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<CustomDashboardDto>> Create([FromBody] CreateDashboardDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var dashboard = await _dashboardService.CreateDashboardAsync(dto, userId);
            return CreatedAtAction(nameof(Get), new { id = dashboard.Id }, dashboard);
        }

        /// <summary>
        /// Update an existing dashboard
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<CustomDashboardDto>> Update(Guid id, [FromBody] UpdateDashboardDto dto)
        {
            var dashboard = await _dashboardService.UpdateDashboardAsync(id, dto);
            if (dashboard == null)
                return NotFound();

            return Ok(dashboard);
        }

        /// <summary>
        /// Delete a dashboard
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _dashboardService.DeleteDashboardAsync(id);
            return NoContent();
        }

        /// <summary>
        /// Add a widget to a dashboard
        /// </summary>
        [HttpPost("{id}/widgets")]
        public async Task<ActionResult<DashboardWidgetDto>> AddWidget(Guid id, [FromBody] CreateWidgetDto dto)
        {
            var widget = await _dashboardService.AddWidgetAsync(id, dto);
            return CreatedAtAction(nameof(GetWidgetData), new { widgetId = widget.Id }, widget);
        }

        /// <summary>
        /// Update widget position on grid
        /// </summary>
        [HttpPut("widgets/{widgetId}/position")]
        public async Task<IActionResult> UpdateWidgetPosition(Guid widgetId, [FromBody] WidgetPositionDto dto)
        {
            await _dashboardService.UpdateWidgetPositionAsync(widgetId, dto);
            return NoContent();
        }

        /// <summary>
        /// Delete a widget from dashboard
        /// </summary>
        [HttpDelete("widgets/{widgetId}")]
        public async Task<IActionResult> DeleteWidget(Guid widgetId)
        {
            await _dashboardService.DeleteWidgetAsync(widgetId);
            return NoContent();
        }

        /// <summary>
        /// Execute widget query and get data
        /// </summary>
        [HttpGet("widgets/{widgetId}/data")]
        public async Task<ActionResult<WidgetDataDto>> GetWidgetData(Guid widgetId)
        {
            var data = await _dashboardService.ExecuteWidgetQueryAsync(widgetId);
            return Ok(data);
        }

        /// <summary>
        /// Export dashboard to specified format
        /// </summary>
        [HttpPost("{id}/export")]
        public async Task<IActionResult> Export(Guid id, [FromQuery] DashboardExportFormat format = DashboardExportFormat.Json)
        {
            var data = await _dashboardService.ExportDashboardAsync(id, format);
            
            var contentType = format switch
            {
                DashboardExportFormat.Pdf => "application/pdf",
                DashboardExportFormat.Excel => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                _ => "application/json"
            };

            var fileName = $"dashboard_{id}.{format.ToString().ToLower()}";
            return File(data, contentType, fileName);
        }

        /// <summary>
        /// Get all widget templates
        /// </summary>
        [HttpGet("templates")]
        public async Task<ActionResult<List<WidgetTemplateDto>>> GetTemplates()
        {
            var templates = await _dashboardService.GetWidgetTemplatesAsync();
            return Ok(templates);
        }

        /// <summary>
        /// Get widget templates by category
        /// </summary>
        [HttpGet("templates/category/{category}")]
        public async Task<ActionResult<List<WidgetTemplateDto>>> GetTemplatesByCategory(string category)
        {
            var templates = await _dashboardService.GetWidgetTemplatesByCategoryAsync(category);
            return Ok(templates);
        }

        // ========== Category 4.1: Dashboard Sharing Endpoints ==========

        /// <summary>
        /// Share a dashboard with a user or role
        /// </summary>
        [HttpPost("{id}/share")]
        public async Task<ActionResult<DashboardShareDto>> ShareDashboard(
            Guid id, 
            [FromBody] CreateDashboardShareDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var share = await _dashboardService.ShareDashboardAsync(id, dto, userId);
            return CreatedAtAction(nameof(GetDashboardShares), new { id }, share);
        }

        /// <summary>
        /// Get all shares for a specific dashboard
        /// </summary>
        [HttpGet("{id}/shares")]
        public async Task<ActionResult<List<DashboardShareDto>>> GetDashboardShares(Guid id)
        {
            var shares = await _dashboardService.GetDashboardSharesAsync(id);
            return Ok(shares);
        }

        /// <summary>
        /// Revoke a dashboard share
        /// </summary>
        [HttpDelete("shares/{shareId}")]
        public async Task<IActionResult> RevokeDashboardShare(Guid shareId)
        {
            await _dashboardService.RevokeDashboardShareAsync(shareId);
            return NoContent();
        }

        /// <summary>
        /// Get dashboards shared with current user
        /// </summary>
        [HttpGet("shared")]
        public async Task<ActionResult<List<CustomDashboardDto>>> GetSharedDashboards()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);
            var dashboards = await _dashboardService.GetSharedDashboardsAsync(userId, userRole);
            return Ok(dashboards);
        }

        /// <summary>
        /// Duplicate a dashboard (useful for creating templates)
        /// </summary>
        [HttpPost("{id}/duplicate")]
        public async Task<ActionResult<CustomDashboardDto>> DuplicateDashboard(
            Guid id, 
            [FromQuery] string newName = null)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var dashboard = await _dashboardService.DuplicateDashboardAsync(id, userId, newName);
            return CreatedAtAction(nameof(Get), new { id = dashboard.Id }, dashboard);
        }
    }
}
