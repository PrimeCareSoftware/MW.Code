using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Application.Services.CRM
{
    /// <summary>
    /// Service for managing leads captured from registration flow
    /// </summary>
    public interface ILeadManagementService
    {
        /// <summary>
        /// Create a lead from an abandoned funnel session
        /// </summary>
        Task<Lead> CreateLeadFromFunnelAsync(string sessionId);

        /// <summary>
        /// Get all unassigned leads
        /// </summary>
        Task<IEnumerable<Lead>> GetUnassignedLeadsAsync();

        /// <summary>
        /// Get leads assigned to a specific user
        /// </summary>
        Task<IEnumerable<Lead>> GetLeadsAssignedToUserAsync(Guid userId);

        /// <summary>
        /// Get leads by status
        /// </summary>
        Task<IEnumerable<Lead>> GetLeadsByStatusAsync(LeadStatus status);

        /// <summary>
        /// Get leads that need follow-up (NextFollowUpDate <= today)
        /// </summary>
        Task<IEnumerable<Lead>> GetLeadsNeedingFollowUpAsync();

        /// <summary>
        /// Assign a lead to a user
        /// </summary>
        Task<bool> AssignLeadAsync(Guid leadId, Guid userId, Guid assignedByUserId);

        /// <summary>
        /// Update lead status
        /// </summary>
        Task<bool> UpdateLeadStatusAsync(Guid leadId, LeadStatus newStatus, string? notes = null, Guid? userId = null);

        /// <summary>
        /// Schedule a follow-up for a lead
        /// </summary>
        Task<bool> ScheduleFollowUpAsync(Guid leadId, DateTime followUpDate, Guid userId);

        /// <summary>
        /// Add an activity to a lead
        /// </summary>
        Task<LeadActivity> AddActivityAsync(
            Guid leadId, 
            ActivityType type, 
            string title, 
            string? description = null,
            Guid? userId = null,
            string? userName = null,
            int? durationMinutes = null,
            string? outcome = null);

        /// <summary>
        /// Get all activities for a lead
        /// </summary>
        Task<IEnumerable<LeadActivity>> GetLeadActivitiesAsync(Guid leadId);

        /// <summary>
        /// Get lead statistics
        /// </summary>
        Task<LeadStatistics> GetLeadStatisticsAsync();

        /// <summary>
        /// Get lead statistics by user
        /// </summary>
        Task<Dictionary<Guid, UserLeadStatistics>> GetStatisticsByUserAsync();

        /// <summary>
        /// Update lead contact information
        /// </summary>
        Task<bool> UpdateLeadContactInfoAsync(Guid leadId, string? contactName, string? email, string? phone);

        /// <summary>
        /// Add notes to a lead
        /// </summary>
        Task<bool> AddLeadNotesAsync(Guid leadId, string notes, Guid userId);

        /// <summary>
        /// Set tags for a lead
        /// </summary>
        Task<bool> SetLeadTagsAsync(Guid leadId, string? tags);

        /// <summary>
        /// Search leads by text (name, email, phone, company)
        /// </summary>
        Task<IEnumerable<Lead>> SearchLeadsAsync(string searchTerm);
    }
}
