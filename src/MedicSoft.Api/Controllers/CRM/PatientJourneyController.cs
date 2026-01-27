using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers.CRM
{
    [Authorize]
    [ApiController]
    [Route("api/crm/journey")]
    [Produces("application/json")]
    public class PatientJourneyController : BaseController
    {
        private readonly IPatientJourneyService _journeyService;
        private readonly ILogger<PatientJourneyController> _logger;

        public PatientJourneyController(
            IPatientJourneyService journeyService,
            ITenantContext tenantContext,
            ILogger<PatientJourneyController> logger)
            : base(tenantContext)
        {
            _journeyService = journeyService;
            _logger = logger;
        }

        /// <summary>
        /// Get patient journey by patient ID
        /// </summary>
        [HttpGet("{patientId}")]
        [ProducesResponseType(typeof(PatientJourneyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientJourneyDto>> GetByPatientId(Guid patientId)
        {
            try
            {
                var tenantId = GetTenantId();
                var journey = await _journeyService.GetOrCreateJourneyAsync(patientId, tenantId);
                return Ok(journey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting journey for patient {PatientId}", patientId);
                return StatusCode(500, new { message = "Erro ao buscar jornada do paciente" });
            }
        }

        /// <summary>
        /// Advance patient to next stage in journey
        /// </summary>
        [HttpPost("{patientId}/advance")]
        [ProducesResponseType(typeof(PatientJourneyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PatientJourneyDto>> AdvanceStage(
            Guid patientId, 
            [FromBody] AdvanceJourneyStageDto dto)
        {
            try
            {
                var tenantId = GetTenantId();
                var journey = await _journeyService.AdvanceStageAsync(patientId, dto, tenantId);
                return Ok(journey);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid stage advance for patient {PatientId}", patientId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error advancing stage for patient {PatientId}", patientId);
                return StatusCode(500, new { message = "Erro ao avançar estágio da jornada" });
            }
        }

        /// <summary>
        /// Add touchpoint to patient journey
        /// </summary>
        [HttpPost("{patientId}/touchpoint")]
        [ProducesResponseType(typeof(PatientJourneyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PatientJourneyDto>> AddTouchpoint(
            Guid patientId, 
            [FromBody] CreatePatientTouchpointDto dto)
        {
            try
            {
                var tenantId = GetTenantId();
                var journey = await _journeyService.AddTouchpointAsync(patientId, dto, tenantId);
                return Ok(journey);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid touchpoint add for patient {PatientId}", patientId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding touchpoint for patient {PatientId}", patientId);
                return StatusCode(500, new { message = "Erro ao adicionar touchpoint" });
            }
        }

        /// <summary>
        /// Get patient journey metrics
        /// </summary>
        [HttpGet("{patientId}/metrics")]
        [ProducesResponseType(typeof(PatientJourneyMetricsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientJourneyMetricsDto>> GetMetrics(Guid patientId)
        {
            try
            {
                var tenantId = GetTenantId();
                var metrics = await _journeyService.GetMetricsAsync(patientId, tenantId);
                
                if (metrics == null)
                    return NotFound(new { message = "Jornada não encontrada para o paciente" });
                
                return Ok(metrics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting metrics for patient {PatientId}", patientId);
                return StatusCode(500, new { message = "Erro ao buscar métricas" });
            }
        }

        /// <summary>
        /// Update patient journey metrics
        /// </summary>
        [HttpPatch("{patientId}/metrics")]
        [ProducesResponseType(typeof(PatientJourneyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PatientJourneyDto>> UpdateMetrics(
            Guid patientId, 
            [FromBody] UpdatePatientJourneyMetricsDto dto)
        {
            try
            {
                var tenantId = GetTenantId();
                var journey = await _journeyService.UpdateMetricsAsync(patientId, dto, tenantId);
                return Ok(journey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating metrics for patient {PatientId}", patientId);
                return StatusCode(500, new { message = "Erro ao atualizar métricas" });
            }
        }

        /// <summary>
        /// Recalculate patient journey metrics from data
        /// </summary>
        [HttpPost("{patientId}/metrics/recalculate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RecalculateMetrics(Guid patientId)
        {
            try
            {
                var tenantId = GetTenantId();
                await _journeyService.RecalculateMetricsAsync(patientId, tenantId);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recalculating metrics for patient {PatientId}", patientId);
                return StatusCode(500, new { message = "Erro ao recalcular métricas" });
            }
        }
    }
}
