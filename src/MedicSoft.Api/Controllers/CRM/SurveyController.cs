using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers.CRM
{
    /// <summary>
    /// Survey Controller
    /// 
    /// Manages patient satisfaction surveys and feedback collection. Provides comprehensive endpoints
    /// for creating survey templates, distributing surveys to patients, collecting responses, and
    /// analyzing survey data for insights into patient satisfaction and experience.
    /// </summary>
    /// <remarks>
    /// This controller enables:
    /// - Survey template creation and management
    /// - Survey distribution to specific patients or groups
    /// - Response collection and validation
    /// - Analytics and reporting on survey results
    /// - Survey lifecycle management (activation, deactivation, archival)
    /// 
    /// Surveys are valuable tools for understanding patient satisfaction, gathering feedback on
    /// specific services or encounters, and driving continuous improvement initiatives.
    /// </remarks>
    [Authorize]
    [ApiController]
    [Route("api/crm/survey")]
    [Produces("application/json")]
    [ApiExplorerSettings(GroupName = "CRM - Survey Management")]
    public class SurveyController : BaseController
    {
        private readonly ISurveyService _surveyService;
        private readonly ILogger<SurveyController> _logger;

        public SurveyController(
            ISurveyService surveyService,
            ITenantContext tenantContext,
            ILogger<SurveyController> logger)
            : base(tenantContext)
        {
            _surveyService = surveyService;
            _logger = logger;
        }

        /// <summary>
        /// Get all surveys
        /// </summary>
        /// <remarks>
        /// Retrieves a complete list of all survey templates in the system regardless of their
        /// active status. Results include archived, active, and draft surveys filtered by the
        /// current tenant context.
        /// </remarks>
        /// <returns>A collection of all survey templates and configurations.</returns>
        /// <response code="200">Successfully retrieved all surveys</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SurveyDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<SurveyDto>>> GetAll()
        {
            try
            {
                var tenantId = GetTenantId();
                var surveys = await _surveyService.GetAllAsync(tenantId);
                return Ok(surveys);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all surveys");
                return StatusCode(500, new { message = "Erro ao buscar pesquisas" });
            }
        }

        /// <summary>
        /// Get active surveys
        /// </summary>
        /// <remarks>
        /// Retrieves only the currently active survey templates that are available for distribution
        /// to patients. Excluded surveys that are draft, paused, or archived.
        /// </remarks>
        /// <returns>A collection of active survey templates.</returns>
        /// <response code="200">Successfully retrieved active surveys</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<SurveyDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<SurveyDto>>> GetActive()
        {
            try
            {
                var tenantId = GetTenantId();
                var surveys = await _surveyService.GetActiveAsync(tenantId);
                return Ok(surveys);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active surveys");
                return StatusCode(500, new { message = "Erro ao buscar pesquisas ativas" });
            }
        }

        /// <summary>
        /// Get a specific survey by ID
        /// </summary>
        /// <remarks>
        /// Retrieves detailed information about a single survey template including questions, answer options,
        /// scoring rules, and distribution settings. Includes metadata about creation, activation dates,
        /// and current response counts.
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the survey to retrieve.</param>
        /// <returns>The detailed survey configuration and metadata.</returns>
        /// <response code="200">Successfully retrieved the survey</response>
        /// <response code="404">Survey not found or does not belong to the current tenant</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SurveyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SurveyDto>> GetById(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var survey = await _surveyService.GetByIdAsync(id, tenantId);
                
                if (survey == null)
                    return NotFound(new { message = "Pesquisa não encontrada" });

                return Ok(survey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting survey {SurveyId}", id);
                return StatusCode(500, new { message = "Erro ao buscar pesquisa" });
            }
        }

        /// <summary>
        /// Create a new survey
        /// </summary>
        /// <remarks>
        /// Creates a new survey template with specified questions, answer options, and distribution rules.
        /// Surveys are created in draft state and must be activated before they can be sent to patients.
        /// 
        /// Business Logic:
        /// - Validates survey structure (minimum required questions, valid answer options)
        /// - Ensures unique survey names within the tenant
        /// - Initializes survey with default scoring and analysis settings
        /// - Records creation metadata and user information
        /// - Returns created survey with assigned ID for subsequent operations
        /// </remarks>
        /// <param name="dto">The survey creation request containing template configuration and questions.</param>
        /// <returns>The newly created survey template with assigned ID and default settings.</returns>
        /// <response code="201">Successfully created the survey; Location header contains resource URL</response>
        /// <response code="400">Invalid request data or survey structure validation error</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpPost]
        [ProducesResponseType(typeof(SurveyDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SurveyDto>> Create([FromBody] CreateSurveyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var tenantId = GetTenantId();
                var survey = await _surveyService.CreateAsync(dto, tenantId);
                
                return CreatedAtAction(
                    nameof(GetById), 
                    new { id = survey.Id }, 
                    survey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating survey");
                return StatusCode(500, new { message = "Erro ao criar pesquisa" });
            }
        }

        /// <summary>
        /// Update an existing survey
        /// </summary>
        /// <remarks>
        /// Updates the configuration of an existing survey template. Changes can include question modifications,
        /// answer options, scoring rules, and distribution settings. Active surveys must be deactivated
        /// before major structural changes are allowed.
        /// 
        /// Business Logic:
        /// - Prevents changes to questions if responses have been collected (to maintain data integrity)
        /// - Allows metadata and distribution setting updates on active surveys
        /// - Validates all configuration changes before persistence
        /// - Maintains version history for audit trail
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the survey to update.</param>
        /// <param name="dto">The survey update request with modified configuration.</param>
        /// <returns>The updated survey template.</returns>
        /// <response code="200">Successfully updated the survey</response>
        /// <response code="404">Survey not found or does not belong to the current tenant</response>
        /// <response code="400">Invalid request data or configuration validation error</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SurveyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SurveyDto>> Update(
            Guid id, 
            [FromBody] UpdateSurveyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var tenantId = GetTenantId();
                var survey = await _surveyService.UpdateAsync(id, dto, tenantId);
                return Ok(survey);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Pesquisa não encontrada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating survey {SurveyId}", id);
                return StatusCode(500, new { message = "Erro ao atualizar pesquisa" });
            }
        }

        /// <summary>
        /// Delete a survey
        /// </summary>
        /// <remarks>
        /// Permanently deletes a survey template from the system. Deleted surveys cannot be recovered.
        /// Response data for deleted surveys is retained for historical and compliance purposes.
        /// Active surveys must be deactivated before deletion.
        /// 
        /// Business Logic:
        /// - Prevents deletion of currently active surveys
        /// - Archives all response data before deletion
        /// - Removes survey from active distribution queues
        /// - Returns 404 if survey not found or already deleted
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the survey to delete.</param>
        /// <returns>No content on successful deletion.</returns>
        /// <response code="204">Successfully deleted the survey</response>
        /// <response code="404">Survey not found or does not belong to the current tenant</response>
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
                var deleted = await _surveyService.DeleteAsync(id, tenantId);
                
                if (!deleted)
                    return NotFound(new { message = "Pesquisa não encontrada" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting survey {SurveyId}", id);
                return StatusCode(500, new { message = "Erro ao deletar pesquisa" });
            }
        }

        /// <summary>
        /// Activate a survey
        /// </summary>
        /// <remarks>
        /// Activates a survey template, making it available for distribution to patients. Only active
        /// surveys can be sent out. A survey can be deactivated and reactivated multiple times without
        /// losing collected response data.
        /// 
        /// Business Logic:
        /// - Validates survey configuration before activation
        /// - Records activation timestamp and user information
        /// - Updates survey status in distribution system
        /// - Enables automated distribution workflows if configured
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the survey to activate.</param>
        /// <returns>Success message confirming activation.</returns>
        /// <response code="200">Successfully activated the survey</response>
        /// <response code="404">Survey not found or does not belong to the current tenant</response>
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
                var activated = await _surveyService.ActivateAsync(id, tenantId);
                
                if (!activated)
                    return NotFound(new { message = "Pesquisa não encontrada" });

                return Ok(new { message = "Pesquisa ativada com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating survey {SurveyId}", id);
                return StatusCode(500, new { message = "Erro ao ativar pesquisa" });
            }
        }

        /// <summary>
        /// Deactivate a survey
        /// </summary>
        /// <remarks>
        /// Deactivates a survey template, preventing further distribution to patients. Existing
        /// distributed surveys may still be completed by patients. Deactivated surveys can be
        /// reactivated without losing any response data.
        /// 
        /// Business Logic:
        /// - Removes survey from active distribution queues
        /// - Stops automated distribution workflows
        /// - Preserves all collected response data
        /// - Records deactivation timestamp and user information
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the survey to deactivate.</param>
        /// <returns>Success message confirming deactivation.</returns>
        /// <response code="200">Successfully deactivated the survey</response>
        /// <response code="404">Survey not found or does not belong to the current tenant</response>
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
                var deactivated = await _surveyService.DeactivateAsync(id, tenantId);
                
                if (!deactivated)
                    return NotFound(new { message = "Pesquisa não encontrada" });

                return Ok(new { message = "Pesquisa desativada com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deactivating survey {SurveyId}", id);
                return StatusCode(500, new { message = "Erro ao desativar pesquisa" });
            }
        }

        /// <summary>
        /// Submit survey response
        /// </summary>
        /// <remarks>
        /// Submits a patient's survey responses for a specific survey. Responses are validated against
        /// survey configuration (answer options, required fields, etc.) before acceptance. Once submitted,
        /// responses are immediately available for analytics and are reflected in survey metrics.
        /// 
        /// Business Logic:
        /// - Validates response data against survey structure
        /// - Ensures all required questions are answered
        /// - Calculates scoring based on survey rules
        /// - Records submission timestamp and patient identifier
        /// - Triggers any post-submission automations or notifications
        /// </remarks>
        /// <param name="dto">The survey response submission containing answers to survey questions.</param>
        /// <returns>The recorded survey response with confirmation and timestamps.</returns>
        /// <response code="201">Successfully submitted the survey response</response>
        /// <response code="400">Invalid response data or validation error (incomplete answers, invalid options, etc.)</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpPost("response")]
        [ProducesResponseType(typeof(SurveyResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SurveyResponseDto>> SubmitResponse([FromBody] SubmitSurveyResponseDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var tenantId = GetTenantId();
                var response = await _surveyService.SubmitResponseAsync(dto, tenantId);
                
                return CreatedAtAction(
                    nameof(GetById), 
                    new { id = dto.SurveyId }, 
                    response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error submitting survey response");
                return StatusCode(500, new { message = "Erro ao enviar resposta da pesquisa" });
            }
        }

        /// <summary>
        /// Get survey responses
        /// </summary>
        /// <remarks>
        /// Retrieves all responses submitted for a specific survey. Results include detailed response
        /// data, submission timestamps, and calculated scores. Useful for analyzing individual responses
        /// or exporting data for external analysis.
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the survey whose responses will be retrieved.</param>
        /// <returns>A collection of survey responses with details and scores.</returns>
        /// <response code="200">Successfully retrieved survey responses</response>
        /// <response code="404">Survey not found or does not belong to the current tenant</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpGet("{id}/responses")]
        [ProducesResponseType(typeof(IEnumerable<SurveyResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<SurveyResponseDto>>> GetResponses(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var responses = await _surveyService.GetSurveyResponsesAsync(id, tenantId);
                return Ok(responses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting responses for survey {SurveyId}", id);
                return StatusCode(500, new { message = "Erro ao buscar respostas da pesquisa" });
            }
        }

        /// <summary>
        /// Get survey analytics
        /// </summary>
        /// <remarks>
        /// Retrieves comprehensive analytics and insights for a specific survey. Analytics include
        /// response rate statistics, score distributions, answer frequency breakdowns, and identified
        /// trends or patterns in the data.
        /// 
        /// Analytics Include:
        /// - Total responses and response rate percentage
        /// - Average scores and score distribution
        /// - Per-question answer frequency and percentages
        /// - Response timeline and completion patterns
        /// - Key insights and identified trends
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the survey whose analytics will be retrieved.</param>
        /// <returns>The analytics object containing comprehensive survey insights and statistics.</returns>
        /// <response code="200">Successfully retrieved survey analytics</response>
        /// <response code="404">Survey not found or does not belong to the current tenant</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpGet("{id}/analytics")]
        [ProducesResponseType(typeof(SurveyAnalyticsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<SurveyAnalyticsDto>> GetAnalytics(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var analytics = await _surveyService.GetAnalyticsAsync(id, tenantId);
                
                if (analytics == null)
                    return NotFound(new { message = "Pesquisa não encontrada" });

                return Ok(analytics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting analytics for survey {SurveyId}", id);
                return StatusCode(500, new { message = "Erro ao buscar análises da pesquisa" });
            }
        }

        /// <summary>
        /// Send survey to patient
        /// </summary>
        /// <remarks>
        /// Sends an active survey to a specific patient via their preferred communication channel
        /// (email, SMS, in-portal notification, etc.). The survey is recorded as sent and a completion
        /// reminder can be automatically scheduled.
        /// 
        /// Business Logic:
        /// - Validates survey is active and patient exists
        /// - Records survey distribution timestamp
        /// - Determines and uses patient's preferred communication channel
        /// - Generates unique survey link for patient
        /// - Schedules reminder notifications if configured
        /// </remarks>
        /// <param name="id">The unique identifier (GUID) of the survey to send.</param>
        /// <param name="patientId">The unique identifier (GUID) of the patient to receive the survey.</param>
        /// <returns>Success message confirming survey distribution.</returns>
        /// <response code="200">Successfully sent survey to patient</response>
        /// <response code="404">Survey or patient not found or does not belong to the current tenant</response>
        /// <response code="500">An unexpected error occurred while processing the request</response>
        [HttpPost("{id}/send/{patientId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendToPatient(Guid id, Guid patientId)
        {
            try
            {
                var tenantId = GetTenantId();
                await _surveyService.SendSurveyToPatientAsync(id, patientId, tenantId);
                
                return Ok(new { message = "Pesquisa enviada com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending survey {SurveyId} to patient {PatientId}", 
                    id, patientId);
                return StatusCode(500, new { message = "Erro ao enviar pesquisa" });
            }
        }
    }
}
