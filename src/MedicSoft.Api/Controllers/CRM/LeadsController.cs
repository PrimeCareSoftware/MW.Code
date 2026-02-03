using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Domain.Entities.CRM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MedicSoft.Api.Controllers.CRM
{
    /// <summary>
    /// Controller for managing leads captured from registration flow
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SystemAdmin,SalesManager")]
    public class LeadsController : ControllerBase
    {
        private readonly ILeadManagementService _leadService;

        public LeadsController(ILeadManagementService leadService)
        {
            _leadService = leadService;
        }

        /// <summary>
        /// Get all unassigned leads
        /// </summary>
        [HttpGet("unassigned")]
        public async Task<ActionResult<IEnumerable<Lead>>> GetUnassignedLeads()
        {
            var leads = await _leadService.GetUnassignedLeadsAsync();
            return Ok(leads);
        }

        /// <summary>
        /// Get leads assigned to a specific user
        /// </summary>
        [HttpGet("assigned/{userId}")]
        public async Task<ActionResult<IEnumerable<Lead>>> GetLeadsAssignedToUser(Guid userId)
        {
            var leads = await _leadService.GetLeadsAssignedToUserAsync(userId);
            return Ok(leads);
        }

        /// <summary>
        /// Get leads by status
        /// </summary>
        [HttpGet("by-status/{status}")]
        public async Task<ActionResult<IEnumerable<Lead>>> GetLeadsByStatus(LeadStatus status)
        {
            var leads = await _leadService.GetLeadsByStatusAsync(status);
            return Ok(leads);
        }

        /// <summary>
        /// Get leads needing follow-up
        /// </summary>
        [HttpGet("needing-followup")]
        public async Task<ActionResult<IEnumerable<Lead>>> GetLeadsNeedingFollowUp()
        {
            var leads = await _leadService.GetLeadsNeedingFollowUpAsync();
            return Ok(leads);
        }

        /// <summary>
        /// Search leads by text
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Lead>>> SearchLeads([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return BadRequest("Search term is required");
            }

            var leads = await _leadService.SearchLeadsAsync(searchTerm);
            return Ok(leads);
        }

        /// <summary>
        /// Get lead statistics
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<LeadStatistics>> GetStatistics()
        {
            var stats = await _leadService.GetLeadStatisticsAsync();
            return Ok(stats);
        }

        /// <summary>
        /// Get statistics by user
        /// </summary>
        [HttpGet("statistics/by-user")]
        public async Task<ActionResult<Dictionary<Guid, UserLeadStatistics>>> GetStatisticsByUser()
        {
            var stats = await _leadService.GetStatisticsByUserAsync();
            return Ok(stats);
        }

        /// <summary>
        /// Create lead from abandoned funnel session
        /// </summary>
        [HttpPost("create-from-funnel/{sessionId}")]
        public async Task<ActionResult<Lead>> CreateLeadFromFunnel(string sessionId)
        {
            try
            {
                var lead = await _leadService.CreateLeadFromFunnelAsync(sessionId);
                return Ok(lead);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Assign a lead to a user
        /// </summary>
        [HttpPost("{leadId}/assign")]
        public async Task<ActionResult> AssignLead(Guid leadId, [FromBody] AssignLeadRequest request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var success = await _leadService.AssignLeadAsync(leadId, request.UserId, userId.Value);
            if (!success)
            {
                return NotFound($"Lead {leadId} not found");
            }

            return Ok();
        }

        /// <summary>
        /// Update lead status
        /// </summary>
        [HttpPut("{leadId}/status")]
        public async Task<ActionResult> UpdateLeadStatus(Guid leadId, [FromBody] UpdateLeadStatusRequest request)
        {
            var userId = GetCurrentUserId();
            var success = await _leadService.UpdateLeadStatusAsync(leadId, request.Status, request.Notes, userId);
            if (!success)
            {
                return NotFound($"Lead {leadId} not found");
            }

            return Ok();
        }

        /// <summary>
        /// Schedule a follow-up for a lead
        /// </summary>
        [HttpPost("{leadId}/followup")]
        public async Task<ActionResult> ScheduleFollowUp(Guid leadId, [FromBody] ScheduleFollowUpRequest request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var success = await _leadService.ScheduleFollowUpAsync(leadId, request.FollowUpDate, userId.Value);
            if (!success)
            {
                return NotFound($"Lead {leadId} not found");
            }

            return Ok();
        }

        /// <summary>
        /// Add an activity to a lead
        /// </summary>
        [HttpPost("{leadId}/activities")]
        public async Task<ActionResult<LeadActivity>> AddActivity(Guid leadId, [FromBody] AddActivityRequest request)
        {
            try
            {
                var userId = GetCurrentUserId();
                var userName = GetCurrentUserName();

                var activity = await _leadService.AddActivityAsync(
                    leadId,
                    request.Type,
                    request.Title,
                    request.Description,
                    userId,
                    userName,
                    request.DurationMinutes,
                    request.Outcome
                );

                return Ok(activity);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Get all activities for a lead
        /// </summary>
        [HttpGet("{leadId}/activities")]
        public async Task<ActionResult<IEnumerable<LeadActivity>>> GetLeadActivities(Guid leadId)
        {
            var activities = await _leadService.GetLeadActivitiesAsync(leadId);
            return Ok(activities);
        }

        /// <summary>
        /// Update lead contact information
        /// </summary>
        [HttpPut("{leadId}/contact-info")]
        public async Task<ActionResult> UpdateContactInfo(Guid leadId, [FromBody] UpdateContactInfoRequest request)
        {
            var success = await _leadService.UpdateLeadContactInfoAsync(
                leadId,
                request.ContactName,
                request.Email,
                request.Phone
            );

            if (!success)
            {
                return NotFound($"Lead {leadId} not found");
            }

            return Ok();
        }

        /// <summary>
        /// Add notes to a lead
        /// </summary>
        [HttpPost("{leadId}/notes")]
        public async Task<ActionResult> AddNotes(Guid leadId, [FromBody] AddNotesRequest request)
        {
            var userId = GetCurrentUserId();
            if (!userId.HasValue)
            {
                return Unauthorized();
            }

            var success = await _leadService.AddLeadNotesAsync(leadId, request.Notes, userId.Value);
            if (!success)
            {
                return NotFound($"Lead {leadId} not found");
            }

            return Ok();
        }

        /// <summary>
        /// Set tags for a lead
        /// </summary>
        [HttpPut("{leadId}/tags")]
        public async Task<ActionResult> SetTags(Guid leadId, [FromBody] SetTagsRequest request)
        {
            var success = await _leadService.SetLeadTagsAsync(leadId, request.Tags);
            if (!success)
            {
                return NotFound($"Lead {leadId} not found");
            }

            return Ok();
        }

        #region Helper Methods

        private Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
            {
                return userId;
            }
            return null;
        }

        private string? GetCurrentUserName()
        {
            return User.FindFirst(ClaimTypes.Name)?.Value;
        }

        #endregion
    }
}
