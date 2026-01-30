using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for collecting user feedback and NPS surveys (Phase 2 Validation)
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class FeedbackController : BaseController
    {
        private readonly IFeedbackService _feedbackService;
        private readonly INpsSurveyService _npsSurveyService;
        private readonly ILogger<FeedbackController> _logger;

        public FeedbackController(
            ITenantContext tenantContext,
            IFeedbackService feedbackService,
            INpsSurveyService npsSurveyService,
            ILogger<FeedbackController> logger) : base(tenantContext)
        {
            _feedbackService = feedbackService;
            _npsSurveyService = npsSurveyService;
            _logger = logger;
        }

        #region User Feedback

        /// <summary>
        /// Submit user feedback (bug, feature request, UX issue, etc.)
        /// </summary>
        /// <param name="dto">Feedback data</param>
        /// <returns>Created feedback</returns>
        [HttpPost]
        [ProducesResponseType(typeof(UserFeedbackDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateFeedback([FromBody] CreateUserFeedbackDto dto)
        {
            try
            {
                var userId = GetUserId().ToString();
                var tenantId = GetTenantId();

                _logger.LogInformation("Creating feedback - User: {UserId}, Type: {Type}, Tenant: {TenantId}",
                    userId, dto.Type, tenantId);

                var feedback = await _feedbackService.CreateFeedbackAsync(dto, userId, tenantId);
                
                _logger.LogInformation("Feedback created successfully - ID: {FeedbackId}", feedback.Id);
                
                return CreatedAtAction(nameof(GetFeedbackById), new { id = feedback.Id }, feedback);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating feedback");
                return StatusCode(500, new { message = "Error submitting feedback" });
            }
        }

        /// <summary>
        /// Get feedback by ID
        /// </summary>
        /// <param name="id">Feedback ID</param>
        /// <returns>Feedback details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(UserFeedbackDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetFeedbackById(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var feedback = await _feedbackService.GetFeedbackByIdAsync(id, tenantId);

                if (feedback == null)
                    return NotFound(new { message = "Feedback not found" });

                return Ok(feedback);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting feedback by ID: {FeedbackId}", id);
                return StatusCode(500, new { message = "Error retrieving feedback" });
            }
        }

        /// <summary>
        /// Get all feedback for the tenant
        /// </summary>
        /// <returns>List of all feedback</returns>
        [HttpGet]
        [Authorize(Roles = "Admin,Owner")]
        [ProducesResponseType(typeof(UserFeedbackDto[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllFeedback()
        {
            try
            {
                var tenantId = GetTenantId();
                var feedbacks = await _feedbackService.GetAllFeedbackAsync(tenantId);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all feedback");
                return StatusCode(500, new { message = "Error retrieving feedback" });
            }
        }

        /// <summary>
        /// Get feedback by status
        /// </summary>
        /// <param name="status">Feedback status</param>
        /// <returns>List of feedback with the specified status</returns>
        [HttpGet("status/{status}")]
        [Authorize(Roles = "Admin,Owner")]
        [ProducesResponseType(typeof(UserFeedbackDto[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFeedbackByStatus(FeedbackStatus status)
        {
            try
            {
                var tenantId = GetTenantId();
                var feedbacks = await _feedbackService.GetFeedbackByStatusAsync(status, tenantId);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting feedback by status: {Status}", status);
                return StatusCode(500, new { message = "Error retrieving feedback" });
            }
        }

        /// <summary>
        /// Get feedback submitted by current user
        /// </summary>
        /// <returns>List of user's feedback</returns>
        [HttpGet("my-feedback")]
        [ProducesResponseType(typeof(UserFeedbackDto[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyFeedback()
        {
            try
            {
                var userId = GetUserId().ToString();
                var tenantId = GetTenantId();
                var feedbacks = await _feedbackService.GetFeedbackByUserAsync(userId, tenantId);
                return Ok(feedbacks);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user feedback");
                return StatusCode(500, new { message = "Error retrieving feedback" });
            }
        }

        /// <summary>
        /// Update feedback status (Admin/Owner only)
        /// </summary>
        /// <param name="id">Feedback ID</param>
        /// <param name="dto">Status update data</param>
        /// <returns>Updated feedback</returns>
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Admin,Owner")]
        [ProducesResponseType(typeof(UserFeedbackDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateFeedbackStatus(Guid id, [FromBody] UpdateFeedbackStatusDto dto)
        {
            try
            {
                var userId = GetUserId().ToString();
                var tenantId = GetTenantId();

                _logger.LogInformation("Updating feedback status - ID: {FeedbackId}, NewStatus: {Status}, User: {UserId}",
                    id, dto.Status, userId);

                var feedback = await _feedbackService.UpdateFeedbackStatusAsync(id, dto, userId, tenantId);
                return Ok(feedback);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Feedback not found: {FeedbackId}", id);
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating feedback status");
                return StatusCode(500, new { message = "Error updating feedback status" });
            }
        }

        /// <summary>
        /// Get feedback statistics
        /// </summary>
        /// <returns>Feedback statistics</returns>
        [HttpGet("statistics")]
        [Authorize(Roles = "Admin,Owner")]
        [ProducesResponseType(typeof(FeedbackStatisticsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetFeedbackStatistics()
        {
            try
            {
                var tenantId = GetTenantId();
                var statistics = await _feedbackService.GetStatisticsAsync(tenantId);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting feedback statistics");
                return StatusCode(500, new { message = "Error retrieving statistics" });
            }
        }

        #endregion

        #region NPS Survey

        /// <summary>
        /// Submit NPS survey response
        /// </summary>
        /// <param name="dto">Survey response data</param>
        /// <returns>Created survey response</returns>
        [HttpPost("nps")]
        [ProducesResponseType(typeof(NpsSurveyDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateNpsSurveyResponse([FromBody] CreateNpsSurveyDto dto)
        {
            try
            {
                var userId = GetUserId().ToString();
                var tenantId = GetTenantId();

                _logger.LogInformation("Creating NPS survey response - User: {UserId}, Score: {Score}, Tenant: {TenantId}",
                    userId, dto.Score, tenantId);

                var survey = await _npsSurveyService.CreateSurveyResponseAsync(dto, userId, tenantId);
                
                _logger.LogInformation("NPS survey response created successfully - ID: {SurveyId}", survey.Id);
                
                return CreatedAtAction(nameof(GetNpsSurveyById), new { id = survey.Id }, survey);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "User already responded to NPS survey: {UserId}", GetUserId());
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating NPS survey response");
                return StatusCode(500, new { message = "Error submitting survey response" });
            }
        }

        /// <summary>
        /// Get NPS survey by ID
        /// </summary>
        /// <param name="id">Survey ID</param>
        /// <returns>Survey details</returns>
        [HttpGet("nps/{id}")]
        [Authorize(Roles = "Admin,Owner")]
        [ProducesResponseType(typeof(NpsSurveyDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetNpsSurveyById(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var survey = await _npsSurveyService.GetSurveyByIdAsync(id, tenantId);

                if (survey == null)
                    return NotFound(new { message = "Survey not found" });

                return Ok(survey);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting NPS survey by ID: {SurveyId}", id);
                return StatusCode(500, new { message = "Error retrieving survey" });
            }
        }

        /// <summary>
        /// Get all NPS surveys
        /// </summary>
        /// <returns>List of all surveys</returns>
        [HttpGet("nps")]
        [Authorize(Roles = "Admin,Owner")]
        [ProducesResponseType(typeof(NpsSurveyDto[]), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllNpsSurveys()
        {
            try
            {
                var tenantId = GetTenantId();
                var surveys = await _npsSurveyService.GetAllSurveysAsync(tenantId);
                return Ok(surveys);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting all NPS surveys");
                return StatusCode(500, new { message = "Error retrieving surveys" });
            }
        }

        /// <summary>
        /// Check if current user has responded to NPS survey
        /// </summary>
        /// <returns>Boolean indicating if user has responded</returns>
        [HttpGet("nps/has-responded")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> HasUserRespondedToNps()
        {
            try
            {
                var userId = GetUserId().ToString();
                var tenantId = GetTenantId();
                var hasResponded = await _npsSurveyService.HasUserRespondedAsync(userId, tenantId);
                return Ok(new { hasResponded });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking NPS response status");
                return StatusCode(500, new { message = "Error checking survey status" });
            }
        }

        /// <summary>
        /// Get NPS statistics
        /// </summary>
        /// <returns>NPS statistics including score and category breakdown</returns>
        [HttpGet("nps/statistics")]
        [Authorize(Roles = "Admin,Owner")]
        [ProducesResponseType(typeof(NpsStatisticsDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetNpsStatistics()
        {
            try
            {
                var tenantId = GetTenantId();
                var statistics = await _npsSurveyService.GetStatisticsAsync(tenantId);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting NPS statistics");
                return StatusCode(500, new { message = "Error retrieving statistics" });
            }
        }

        #endregion
    }
}
