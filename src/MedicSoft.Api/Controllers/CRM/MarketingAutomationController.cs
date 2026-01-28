using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers.CRM
{
    /// <summary>
    /// Marketing Automation Controller
    /// 
    /// Manages marketing automation workflows and campaigns for patient engagement. Provides endpoints
    /// for creating, configuring, and managing automated marketing workflows that can be triggered
    /// based on patient journey stages or specific events.
    /// </summary>
    /// <remarks>
    /// This controller enables:
    /// - CRUD operations for marketing automation configurations
    /// - Activation and deactivation of automated campaigns
    /// - Performance metrics and analytics for automation effectiveness
    /// - Manual trigger capability for testing and ad-hoc execution
    /// - Multi-tenant isolation for secure campaign management
    /// 
    /// Marketing automations can include email sequences, SMS campaigns, appointment reminders,
    /// follow-up communications, and other triggered marketing actions.
    /// </remarks>
    [Authorize]
    [ApiController]
    [Route("api/crm/automation")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "CRM - Marketing Automation")]
    public class MarketingAutomationController : BaseController
    {
        private readonly IMarketingAutomationService _automationService;
        private readonly ILogger<MarketingAutomationController> _logger;

        public MarketingAutomationController(
            IMarketingAutomationService automationService,
            ITenantContext tenantContext,
            ILogger<MarketingAutomationController> logger)
            : base(tenantContext)
        {
            _automationService = automationService;
            _logger = logger;
        }

        /// <summary>
        /// Get all marketing automations
        /// </summary>
        /// <remarks>
        /// Retrieves a complete list of all marketing automation configurations regardless of their
        /// active status. This includes active, paused, and archived automation workflows.
        /// Results are filtered by the current tenant context for data isolation.
        /// </remarks>
        /// <returns>A collection of all marketing automation configurations.</returns>
        /// <response code="200">Successfully retrieved all automations</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MarketingAutomationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<MarketingAutomationDto>>> GetAll()
        {
            try
            {
                var tenantId = GetTenantId();
                var automations = await _automationService.GetAllAsync(tenantId);
                return Ok(automations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all automations");
                return StatusCode(500, new { message = "Erro ao buscar automações" });
            }
        }

        /// <summary>
        /// Get active marketing automations
        /// </summary>
        /// <remarks>
        /// Retrieves only the active and enabled marketing automation workflows. These are automations
        /// that are currently running and will be triggered based on their configured conditions and schedules.
        /// Paused or disabled automations are excluded from this result set.
        /// </remarks>
        /// <returns>A collection of active marketing automation configurations.</returns>
        /// <response code="200">Successfully retrieved active automations</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<MarketingAutomationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<MarketingAutomationDto>>> GetActive()
        {
            try
            {
                var tenantId = GetTenantId();
                var automations = await _automationService.GetActiveAsync(tenantId);
                return Ok(automations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active automations");
                return StatusCode(500, new { message = "Erro ao buscar automações ativas" });
            }
        }

        /// <summary>
        /// Get a specific marketing automation by ID
        /// </summary>
        /// <remarks>
        /// Retrieves detailed information about a single marketing automation configuration including
        /// its trigger conditions, action steps, schedules, and current status. Includes metadata about
        /// creation, last modification, and activation dates.
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the marketing automation to retrieve.</param>
        /// <returns>The detailed marketing automation configuration object.</returns>
        /// <response code="200">Successfully retrieved the automation</response>
        /// <response code="404">Automation not found or does not belong to the current tenant</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MarketingAutomationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MarketingAutomationDto>> GetById(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var automation = await _automationService.GetByIdAsync(id, tenantId);
                
                if (automation == null)
                    return NotFound(new { message = "Automação não encontrada" });

                return Ok(automation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting automation {AutomationId}", id);
                return StatusCode(500, new { message = "Erro ao buscar automação" });
            }
        }

        /// <summary>
        /// Create a new marketing automation
        /// </summary>
        /// <remarks>
        /// Creates a new marketing automation workflow with specified trigger conditions, actions, and schedules.
        /// The automation will be created in an inactive state and must be explicitly activated before it begins
        /// triggering based on configured conditions.
        /// 
        /// Business Logic:
        /// - Validates trigger conditions and action configuration
        /// - Ensures unique naming within the tenant
        /// - Initializes automation with default settings and status
        /// - Returns the created automation with assigned ID
        /// - Audit trail records creation event with user information
        /// </remarks>
        /// <param name="dto">The automation creation request containing configuration, triggers, and actions.</param>
        /// <returns>The newly created marketing automation with assigned ID and metadata.</returns>
        /// <response code="201">Successfully created the automation; Location header contains resource URL</response>
        /// <response code="400">Invalid request data or configuration validation error</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpPost]
        [ProducesResponseType(typeof(MarketingAutomationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MarketingAutomationDto>> Create([FromBody] CreateMarketingAutomationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var tenantId = GetTenantId();
                var automation = await _automationService.CreateAsync(dto, tenantId);
                
                return CreatedAtAction(
                    nameof(GetById), 
                    new { id = automation.Id }, 
                    automation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating automation");
                return StatusCode(500, new { message = "Erro ao criar automação" });
            }
        }

        /// <summary>
        /// Update an existing marketing automation
        /// </summary>
        /// <remarks>
        /// Updates the configuration of an existing marketing automation. This includes modifying trigger conditions,
        /// action steps, schedules, and other automation properties. Updates cannot be performed on active automations;
        /// the automation must first be deactivated.
        /// 
        /// Business Logic:
        /// - Prevents updates to active automations without explicit deactivation
        /// - Validates all configuration changes before persistence
        /// - Maintains version history for audit purposes
        /// - Updates modification timestamp and user information
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the automation to update.</param>
        /// <param name="dto">The automation update request with modified configuration.</param>
        /// <returns>The updated marketing automation object.</returns>
        /// <response code="200">Successfully updated the automation</response>
        /// <response code="404">Automation not found or does not belong to the current tenant</response>
        /// <response code="400">Invalid request data or configuration validation error</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(MarketingAutomationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MarketingAutomationDto>> Update(
            Guid id, 
            [FromBody] UpdateMarketingAutomationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var tenantId = GetTenantId();
                var automation = await _automationService.UpdateAsync(id, dto, tenantId);
                return Ok(automation);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Automação não encontrada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating automation {AutomationId}", id);
                return StatusCode(500, new { message = "Erro ao atualizar automação" });
            }
        }

        /// <summary>
        /// Delete a marketing automation
        /// </summary>
        /// <remarks>
        /// Permanently deletes a marketing automation workflow. Deleted automations cannot be recovered
        /// and all associated execution history is archived. Active automations must be deactivated
        /// before deletion is allowed.
        /// 
        /// Business Logic:
        /// - Prevents deletion of currently active automations
        /// - Archives associated execution logs before deletion
        /// - Removes automation from any scheduled queues
        /// - Returns 404 if automation not found or already deleted
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the automation to delete.</param>
        /// <returns>No content on successful deletion.</returns>
        /// <response code="204">Successfully deleted the automation</response>
        /// <response code="404">Automation not found or does not belong to the current tenant</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var deleted = await _automationService.DeleteAsync(id, tenantId);
                
                if (!deleted)
                    return NotFound(new { message = "Automação não encontrada" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting automation {AutomationId}", id);
                return StatusCode(500, new { message = "Erro ao deletar automação" });
            }
        }

        /// <summary>
        /// Activate a marketing automation
        /// </summary>
        /// <remarks>
        /// Activates a marketing automation workflow, allowing it to begin executing based on configured
        /// triggers and conditions. Only one version of an automation can be active at a time. Activating
        /// a new version automatically deactivates the previous version.
        /// 
        /// Business Logic:
        /// - Validates automation configuration before activation
        /// - Deactivates previous version if another automation with same name exists
        /// - Records activation timestamp and user information
        /// - Triggers initialization of execution queues and schedules
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the automation to activate.</param>
        /// <returns>Success message confirming activation.</returns>
        /// <response code="200">Successfully activated the automation</response>
        /// <response code="404">Automation not found or does not belong to the current tenant</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpPost("{id}/activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Activate(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var activated = await _automationService.ActivateAsync(id, tenantId);
                
                if (!activated)
                    return NotFound(new { message = "Automação não encontrada" });

                return Ok(new { message = "Automação ativada com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating automation {AutomationId}", id);
                return StatusCode(500, new { message = "Erro ao ativar automação" });
            }
        }

        /// <summary>
        /// Deactivate a marketing automation
        /// </summary>
        /// <remarks>
        /// Deactivates a marketing automation workflow, preventing further execution based on configured triggers.
        /// Existing scheduled executions may complete before deactivation takes effect. Deactivated automations
        /// can be reactivated without reconfiguration.
        /// 
        /// Business Logic:
        /// - Removes automation from active execution queues
        /// - Cancels pending scheduled triggers
        /// - Preserves execution history for reporting and audit
        /// - Records deactivation timestamp and user information
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the automation to deactivate.</param>
        /// <returns>Success message confirming deactivation.</returns>
        /// <response code="200">Successfully deactivated the automation</response>
        /// <response code="404">Automation not found or does not belong to the current tenant</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpPost("{id}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Deactivate(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var deactivated = await _automationService.DeactivateAsync(id, tenantId);
                
                if (!deactivated)
                    return NotFound(new { message = "Automação não encontrada" });

                return Ok(new { message = "Automação desativada com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating automation {AutomationId}", id);
                return StatusCode(500, new { message = "Erro ao desativar automação" });
            }
        }

        /// <summary>
        /// Get metrics for a specific automation
        /// </summary>
        /// <remarks>
        /// Retrieves performance metrics and analytics for a specific marketing automation. Metrics provide
        /// insights into automation effectiveness including trigger counts, execution success rates, customer
        /// engagement levels, and conversion impacts.
        /// 
        /// Metrics Include:
        /// - Total triggers and execution attempts
        /// - Success and failure rates
        /// - Customer response rates and engagement metrics
        /// - Revenue or conversion impact if tracked
        /// - Average execution time and performance indicators
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the automation whose metrics will be retrieved.</param>
        /// <returns>The metrics object containing performance data for the automation.</returns>
        /// <response code="200">Successfully retrieved the automation metrics</response>
        /// <response code="404">Automation not found or does not belong to the current tenant</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpGet("{id}/metrics")]
        [ProducesResponseType(typeof(MarketingAutomationMetricsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<MarketingAutomationMetricsDto>> GetMetrics(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var metrics = await _automationService.GetMetricsAsync(id, tenantId);
                
                if (metrics == null)
                    return NotFound(new { message = "Automação não encontrada" });

                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting metrics for automation {AutomationId}", id);
                return StatusCode(500, new { message = "Erro ao buscar métricas" });
            }
        }

        /// <summary>
        /// Get metrics for all automations
        /// </summary>
        /// <remarks>
        /// Retrieves aggregated performance metrics across all marketing automations in the tenant.
        /// Provides a comprehensive view of automation portfolio performance, enabling comparison
        /// and identification of top-performing automations.
        /// </remarks>
        /// <returns>A collection of metrics objects, one for each automation.</returns>
        /// <response code="200">Successfully retrieved all automation metrics</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpGet("metrics")]
        [ProducesResponseType(typeof(IEnumerable<MarketingAutomationMetricsDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<MarketingAutomationMetricsDto>>> GetAllMetrics()
        {
            try
            {
                var tenantId = GetTenantId();
                var metrics = await _automationService.GetAllMetricsAsync(tenantId);
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all metrics");
                return StatusCode(500, new { message = "Erro ao buscar métricas" });
            }
        }

        /// <summary>
        /// Manually trigger an automation for a specific patient
        /// </summary>
        /// <remarks>
        /// Manually triggers an automation for a specific patient outside of the normal trigger conditions.
        /// Useful for testing automation configurations, re-sending failed sequences, or handling special
        /// cases where manual intervention is required.
        /// 
        /// Business Logic:
        /// - Executes automation actions immediately for the specified patient
        /// - Validation ensures patient exists and automation is valid
        /// - Execution is logged for audit trail and metrics tracking
        /// - Works regardless of automation active status (useful for testing)
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the automation to trigger.</param>
        /// <param name="patientId">The unique identifier (GUID) of the patient to trigger automation for.</param>
        /// <returns>Success message confirming trigger execution.</returns>
        /// <response code="200">Successfully triggered the automation for the patient</response>
        /// <response code="404">Automation or patient not found or does not belong to the current tenant</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpPost("{id}/trigger/{patientId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> TriggerForPatient(Guid id, Guid patientId)
        {
            try
            {
                var tenantId = GetTenantId();
                await _automationService.TriggerAutomationAsync(id, patientId, tenantId);
                
                return Ok(new { message = "Automação disparada com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error triggering automation {AutomationId} for patient {PatientId}", 
                    id, patientId);
                return StatusCode(500, new { message = "Erro ao disparar automação" });
            }
        }
    }
}
