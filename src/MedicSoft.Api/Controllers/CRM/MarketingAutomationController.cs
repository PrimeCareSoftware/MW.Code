using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers.CRM
{
    [Authorize]
    [ApiController]
    [Route("api/crm/automation")]
    [Produces("application/json")]
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
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<MarketingAutomationDto>), StatusCodes.Status200OK)]
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
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<MarketingAutomationDto>), StatusCodes.Status200OK)]
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
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MarketingAutomationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [HttpPost]
        [ProducesResponseType(typeof(MarketingAutomationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(MarketingAutomationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [HttpPost("{id}/activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [HttpPost("{id}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [HttpGet("{id}/metrics")]
        [ProducesResponseType(typeof(MarketingAutomationMetricsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [HttpGet("metrics")]
        [ProducesResponseType(typeof(IEnumerable<MarketingAutomationMetricsDto>), StatusCodes.Status200OK)]
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
        [HttpPost("{id}/trigger/{patientId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
