using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers.CRM
{
    /// <summary>
    /// Patient Journey Controller
    /// 
    /// Manages the patient journey lifecycle including stage progression, touchpoint tracking, and metrics collection.
    /// Provides comprehensive endpoints for monitoring patient interactions throughout the healthcare journey and
    /// calculating engagement metrics based on customer touchpoints.
    /// </summary>
    /// <remarks>
    /// This controller handles:
    /// - Retrieval and creation of patient journey records
    /// - Stage advancement and validation logic
    /// - Touchpoint management for tracking patient interactions
    /// - Metrics calculation and analytics for patient engagement
    /// 
    /// All endpoints require authentication and operate within tenant context for multi-tenant isolation.
    /// </remarks>
    [Authorize]
    [ApiController]
    [Route("api/crm/journey")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "CRM - Patient Journey")]
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
        /// Get or create patient journey by patient ID
        /// </summary>
        /// <remarks>
        /// Retrieves the existing patient journey record for a specific patient. If no journey exists,
        /// creates a new one automatically with the initial stage. The journey tracks all stages, touchpoints,
        /// and metrics for the patient's interaction with the healthcare facility.
        /// 
        /// Business Logic:
        /// - Returns 404 if patient does not exist in the system
        /// - Initializes journey with default stage if not yet created
        /// - Includes all historical touchpoints and stage transitions
        /// </remarks>
        /// <param name="patientId">The unique identifier of the patient. Must be a valid GUID of an existing patient.</param>
        /// <returns>The complete patient journey object containing all stages, touchpoints, and metrics.</returns>
        /// <response code="200">Successfully retrieved the patient journey</response>
        /// <response code="404">Patient not found or journey could not be initialized</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpGet("{patientId}")]
        [ProducesResponseType(typeof(PatientJourneyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        /// <remarks>
        /// Transitions the patient from their current journey stage to the next defined stage. This action
        /// records the transition event and may trigger associated business logic (notifications, automations, etc.).
        /// 
        /// Business Logic:
        /// - Validates that the current stage allows advancement to the requested stage
        /// - Updates the journey record with the new stage and transition timestamp
        /// - Returns validation error if stage transition is not allowed or invalid
        /// - Creates audit trail of all stage changes
        /// </remarks>
        /// <param name="patientId">The unique identifier of the patient whose journey stage will be advanced.</param>
        /// <param name="dto">The advance stage request containing the target stage and transition reason.</param>
        /// <returns>The updated patient journey with the new stage information.</returns>
        /// <response code="200">Successfully advanced to the next stage</response>
        /// <response code="400">Invalid stage transition or validation error</response>
        /// <response code="404">Patient or journey not found</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpPost("{patientId}/advance")]
        [ProducesResponseType(typeof(PatientJourneyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        /// <remarks>
        /// Records a new touchpoint or interaction within the patient's journey. Touchpoints represent
        /// specific interactions or events (appointments, communications, purchases, etc.) that occur
        /// during the patient's lifecycle with the healthcare facility.
        /// 
        /// Business Logic:
        /// - Creates a new touchpoint record with timestamp and interaction details
        /// - Updates journey metrics based on touchpoint type and value
        /// - May trigger automated workflows based on touchpoint classification
        /// - Validates touchpoint data against defined business rules
        /// </remarks>
        /// <param name="patientId">The unique identifier of the patient receiving the touchpoint.</param>
        /// <param name="dto">The touchpoint details including type, channel, value, and metadata.</param>
        /// <returns>The updated patient journey with the new touchpoint included.</returns>
        /// <response code="200">Successfully added the touchpoint</response>
        /// <response code="400">Invalid touchpoint data or validation error</response>
        /// <response code="404">Patient or journey not found</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpPost("{patientId}/touchpoint")]
        [ProducesResponseType(typeof(PatientJourneyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        /// <remarks>
        /// Retrieves calculated metrics and analytics for a patient's journey. Metrics provide insights into
        /// patient engagement, stage duration, touchpoint frequency, conversion rates, and other KPIs that
        /// measure the effectiveness and progress of the patient's journey.
        /// 
        /// Metrics Include:
        /// - Total touchpoints and interaction count
        /// - Average time spent in each stage
        /// - Engagement score based on interaction types and frequency
        /// - Conversion metrics and stage progression rates
        /// - Last interaction timestamp and channel breakdown
        /// </remarks>
        /// <param name="patientId">The unique identifier of the patient whose metrics will be retrieved.</param>
        /// <returns>The calculated metrics object containing engagement data and KPIs.</returns>
        /// <response code="200">Successfully retrieved the journey metrics</response>
        /// <response code="404">Patient or journey metrics not found</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpGet("{patientId}/metrics")]
        [ProducesResponseType(typeof(PatientJourneyMetricsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        /// <remarks>
        /// Manually updates specific metrics for a patient's journey. This endpoint allows adjustments to
        /// calculated metrics if manual corrections are needed due to data import, corrections, or special circumstances.
        /// 
        /// Business Logic:
        /// - Validates metric values against expected ranges
        /// - Updates only specified metric fields
        /// - Maintains audit trail of metric changes
        /// - Triggers recalculation of dependent metrics if applicable
        /// </remarks>
        /// <param name="patientId">The unique identifier of the patient whose metrics will be updated.</param>
        /// <param name="dto">The metrics update object containing values to modify.</param>
        /// <returns>The updated patient journey with modified metrics.</returns>
        /// <response code="200">Successfully updated the metrics</response>
        /// <response code="400">Invalid metric values or validation error</response>
        /// <response code="404">Patient or journey not found</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpPatch("{patientId}/metrics")]
        [ProducesResponseType(typeof(PatientJourneyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
        /// <remarks>
        /// Triggers a complete recalculation of all metrics for a patient's journey based on current
        /// journey data (stages, touchpoints, timestamps). This operation is resource-intensive and should
        /// be used when correcting historical data or after bulk data import/updates.
        /// 
        /// Business Logic:
        /// - Aggregates all journey events and touchpoints
        /// - Recalculates engagement scores, conversion rates, and timing metrics
        /// - Updates last calculated timestamp
        /// - Returns immediately; calculation may occur asynchronously in background
        /// </remarks>
        /// <param name="patientId">The unique identifier of the patient whose metrics will be recalculated.</param>
        /// <returns>No content on successful completion.</returns>
        /// <response code="204">Metrics recalculation initiated successfully</response>
        /// <response code="404">Patient or journey not found</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpPost("{patientId}/metrics/recalculate")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
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
