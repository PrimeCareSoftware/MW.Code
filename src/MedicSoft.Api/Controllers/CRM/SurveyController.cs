using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers.CRM
{
    [Authorize]
    [ApiController]
    [Route("api/crm/survey")]
    [Produces("application/json")]
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
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<SurveyDto>), StatusCodes.Status200OK)]
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
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<SurveyDto>), StatusCodes.Status200OK)]
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
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SurveyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [HttpPost]
        [ProducesResponseType(typeof(SurveyDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(SurveyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [HttpPost("{id}/activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [HttpPost("{id}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [HttpPost("response")]
        [ProducesResponseType(typeof(SurveyResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
        [HttpGet("{id}/responses")]
        [ProducesResponseType(typeof(IEnumerable<SurveyResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [HttpGet("{id}/analytics")]
        [ProducesResponseType(typeof(SurveyAnalyticsDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [HttpPost("{id}/send/{patientId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
