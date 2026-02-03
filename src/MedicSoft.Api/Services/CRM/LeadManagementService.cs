using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Api.Services.CRM
{
    /// <summary>
    /// Implementation of lead management service
    /// </summary>
    public class LeadManagementService : ILeadManagementService
    {
        private readonly IRepository<Lead> _leadRepository;
        private readonly IRepository<LeadActivity> _activityRepository;
        private readonly IRepository<SalesFunnelMetric> _funnelRepository;
        private readonly ILogger<LeadManagementService> _logger;

        public LeadManagementService(
            IRepository<Lead> leadRepository,
            IRepository<LeadActivity> activityRepository,
            IRepository<SalesFunnelMetric> funnelRepository,
            ILogger<LeadManagementService> logger)
        {
            _leadRepository = leadRepository;
            _activityRepository = activityRepository;
            _funnelRepository = funnelRepository;
            _logger = logger;
        }

        public async Task<Lead> CreateLeadFromFunnelAsync(string sessionId)
        {
            try
            {
                // Get all funnel metrics for this session
                var funnelMetrics = await _funnelRepository
                    .FindAsync(m => m.SessionId == sessionId);

                if (!funnelMetrics.Any())
                {
                    throw new InvalidOperationException($"No funnel metrics found for session {sessionId}");
                }

                // Check if lead already exists
                var existingLead = await _leadRepository
                    .FindFirstAsync(l => l.SessionId == sessionId);

                if (existingLead != null)
                {
                    _logger.LogInformation("Lead already exists for session {SessionId}", sessionId);
                    return existingLead;
                }

                // Get the last step reached
                var lastStep = funnelMetrics.Max(m => m.Step);
                var lastMetric = funnelMetrics.First(m => m.Step == lastStep);

                // Parse captured data to extract contact information
                var capturedData = ParseCapturedData(funnelMetrics);

                // Create lead entity
                var lead = new Lead(
                    sessionId: sessionId,
                    leadSource: "Website Registration",
                    lastStepReached: lastStep,
                    companyName: capturedData.GetValueOrDefault("companyName"),
                    contactName: capturedData.GetValueOrDefault("contactName"),
                    email: capturedData.GetValueOrDefault("email"),
                    phone: capturedData.GetValueOrDefault("phone"),
                    city: capturedData.GetValueOrDefault("city"),
                    state: capturedData.GetValueOrDefault("state"),
                    planId: lastMetric.PlanId,
                    planName: capturedData.GetValueOrDefault("planName"),
                    referrer: lastMetric.Referrer,
                    utmCampaign: ParseUtmParameter(lastMetric.Metadata, "utm_campaign"),
                    utmSource: ParseUtmParameter(lastMetric.Metadata, "utm_source"),
                    utmMedium: ParseUtmParameter(lastMetric.Metadata, "utm_medium"),
                    metadata: lastMetric.Metadata
                );

                await _leadRepository.AddAsync(lead);
                _logger.LogInformation("Created lead {LeadId} from session {SessionId}", lead.Id, sessionId);

                // Create initial activity
                var activity = new LeadActivity(
                    leadId: lead.Id,
                    type: ActivityType.Note,
                    title: "Lead Captured",
                    description: $"Lead captured from registration flow at step {lastStep}",
                    activityDate: lead.CapturedAt
                );
                await _activityRepository.AddAsync(activity);

                return lead;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lead from funnel session {SessionId}", sessionId);
                throw;
            }
        }

        public async Task<IEnumerable<Lead>> GetUnassignedLeadsAsync()
        {
            return await _leadRepository.FindAsync(l => l.AssignedToUserId == null && !l.IsDeleted);
        }

        public async Task<IEnumerable<Lead>> GetLeadsAssignedToUserAsync(Guid userId)
        {
            return await _leadRepository.FindAsync(l => l.AssignedToUserId == userId && !l.IsDeleted);
        }

        public async Task<IEnumerable<Lead>> GetLeadsByStatusAsync(LeadStatus status)
        {
            return await _leadRepository.FindAsync(l => l.Status == status && !l.IsDeleted);
        }

        public async Task<IEnumerable<Lead>> GetLeadsNeedingFollowUpAsync()
        {
            var today = DateTime.UtcNow.Date;
            return await _leadRepository.FindAsync(l => 
                l.NextFollowUpDate.HasValue && 
                l.NextFollowUpDate.Value.Date <= today &&
                l.Status != LeadStatus.Converted &&
                l.Status != LeadStatus.Lost &&
                !l.IsDeleted);
        }

        public async Task<bool> AssignLeadAsync(Guid leadId, Guid userId, Guid assignedByUserId)
        {
            try
            {
                var lead = await _leadRepository.GetByIdAsync(leadId);
                if (lead == null || lead.IsDeleted)
                {
                    _logger.LogWarning("Lead {LeadId} not found", leadId);
                    return false;
                }

                lead.AssignTo(userId);
                await _leadRepository.UpdateAsync(lead);

                // Create activity
                var activity = new LeadActivity(
                    leadId: leadId,
                    type: ActivityType.Assignment,
                    title: "Lead Assigned",
                    description: $"Lead assigned to user {userId}",
                    performedByUserId: assignedByUserId
                );
                await _activityRepository.AddAsync(activity);

                _logger.LogInformation("Assigned lead {LeadId} to user {UserId}", leadId, userId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error assigning lead {LeadId} to user {UserId}", leadId, userId);
                return false;
            }
        }

        public async Task<bool> UpdateLeadStatusAsync(Guid leadId, LeadStatus newStatus, string? notes = null, Guid? userId = null)
        {
            try
            {
                var lead = await _leadRepository.GetByIdAsync(leadId);
                if (lead == null || lead.IsDeleted)
                {
                    _logger.LogWarning("Lead {LeadId} not found", leadId);
                    return false;
                }

                var oldStatus = lead.Status;
                lead.UpdateStatus(newStatus, notes);
                await _leadRepository.UpdateAsync(lead);

                // Create activity
                var activity = new LeadActivity(
                    leadId: leadId,
                    type: ActivityType.StatusChange,
                    title: "Status Changed",
                    description: $"Status changed from {oldStatus} to {newStatus}",
                    performedByUserId: userId
                );
                await _activityRepository.AddAsync(activity);

                _logger.LogInformation("Updated lead {LeadId} status from {OldStatus} to {NewStatus}", 
                    leadId, oldStatus, newStatus);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating lead {LeadId} status", leadId);
                return false;
            }
        }

        public async Task<bool> ScheduleFollowUpAsync(Guid leadId, DateTime followUpDate, Guid userId)
        {
            try
            {
                var lead = await _leadRepository.GetByIdAsync(leadId);
                if (lead == null || lead.IsDeleted)
                {
                    _logger.LogWarning("Lead {LeadId} not found", leadId);
                    return false;
                }

                lead.ScheduleFollowUp(followUpDate);
                await _leadRepository.UpdateAsync(lead);

                // Create activity
                var activity = new LeadActivity(
                    leadId: leadId,
                    type: ActivityType.FollowUpScheduled,
                    title: "Follow-up Scheduled",
                    description: $"Follow-up scheduled for {followUpDate:yyyy-MM-dd HH:mm}",
                    performedByUserId: userId
                );
                await _activityRepository.AddAsync(activity);

                _logger.LogInformation("Scheduled follow-up for lead {LeadId} at {FollowUpDate}", 
                    leadId, followUpDate);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling follow-up for lead {LeadId}", leadId);
                return false;
            }
        }

        public async Task<LeadActivity> AddActivityAsync(
            Guid leadId,
            ActivityType type,
            string title,
            string? description = null,
            Guid? userId = null,
            string? userName = null,
            int? durationMinutes = null,
            string? outcome = null)
        {
            try
            {
                var lead = await _leadRepository.GetByIdAsync(leadId);
                if (lead == null || lead.IsDeleted)
                {
                    throw new InvalidOperationException($"Lead {leadId} not found");
                }

                var activity = new LeadActivity(
                    leadId: leadId,
                    type: type,
                    title: title,
                    description: description,
                    performedByUserId: userId,
                    performedByUserName: userName,
                    durationMinutes: durationMinutes,
                    outcome: outcome
                );

                await _activityRepository.AddAsync(activity);
                _logger.LogInformation("Added {Type} activity to lead {LeadId}", type, leadId);

                return activity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding activity to lead {LeadId}", leadId);
                throw;
            }
        }

        public async Task<IEnumerable<LeadActivity>> GetLeadActivitiesAsync(Guid leadId)
        {
            var activities = await _activityRepository.FindAsync(a => a.LeadId == leadId && !a.IsDeleted);
            return activities.OrderByDescending(a => a.ActivityDate);
        }

        public async Task<LeadStatistics> GetLeadStatisticsAsync()
        {
            var allLeads = await _leadRepository.FindAsync(l => !l.IsDeleted);
            var today = DateTime.UtcNow.Date;

            var stats = new LeadStatistics
            {
                TotalLeads = allLeads.Count(),
                NewLeads = allLeads.Count(l => l.Status == LeadStatus.New),
                ContactedLeads = allLeads.Count(l => l.Status == LeadStatus.Contacted),
                QualifiedLeads = allLeads.Count(l => l.Status == LeadStatus.Qualified),
                ConvertedLeads = allLeads.Count(l => l.Status == LeadStatus.Converted),
                LostLeads = allLeads.Count(l => l.Status == LeadStatus.Lost),
                NurturingLeads = allLeads.Count(l => l.Status == LeadStatus.Nurturing),
                UnassignedLeads = allLeads.Count(l => l.AssignedToUserId == null),
                NeedingFollowUp = allLeads.Count(l => 
                    l.NextFollowUpDate.HasValue && 
                    l.NextFollowUpDate.Value.Date <= today &&
                    l.Status != LeadStatus.Converted &&
                    l.Status != LeadStatus.Lost),
                AverageScore = allLeads.Any() ? (int)allLeads.Average(l => l.Score) : 0
            };

            if (stats.TotalLeads > 0)
            {
                stats.ConversionRate = (decimal)stats.ConvertedLeads / stats.TotalLeads * 100;
            }

            return stats;
        }

        public async Task<Dictionary<Guid, UserLeadStatistics>> GetStatisticsByUserAsync()
        {
            var allLeads = await _leadRepository.FindAsync(l => !l.IsDeleted && l.AssignedToUserId.HasValue);
            var today = DateTime.UtcNow.Date;

            var statsByUser = allLeads
                .GroupBy(l => l.AssignedToUserId!.Value)
                .ToDictionary(
                    g => g.Key,
                    g =>
                    {
                        var userLeads = g.ToList();
                        var converted = userLeads.Count(l => l.Status == LeadStatus.Converted);
                        return new UserLeadStatistics
                        {
                            UserId = g.Key,
                            AssignedLeads = userLeads.Count,
                            ConvertedLeads = converted,
                            NeedingFollowUp = userLeads.Count(l =>
                                l.NextFollowUpDate.HasValue &&
                                l.NextFollowUpDate.Value.Date <= today &&
                                l.Status != LeadStatus.Converted &&
                                l.Status != LeadStatus.Lost),
                            ConversionRate = userLeads.Count > 0 ? (decimal)converted / userLeads.Count * 100 : 0
                        };
                    });

            return statsByUser;
        }

        public async Task<bool> UpdateLeadContactInfoAsync(Guid leadId, string? contactName, string? email, string? phone)
        {
            try
            {
                var lead = await _leadRepository.GetByIdAsync(leadId);
                if (lead == null || lead.IsDeleted)
                {
                    _logger.LogWarning("Lead {LeadId} not found", leadId);
                    return false;
                }

                lead.UpdateContactInfo(contactName, email, phone);
                await _leadRepository.UpdateAsync(lead);

                _logger.LogInformation("Updated contact info for lead {LeadId}", leadId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contact info for lead {LeadId}", leadId);
                return false;
            }
        }

        public async Task<bool> AddLeadNotesAsync(Guid leadId, string notes, Guid userId)
        {
            try
            {
                var lead = await _leadRepository.GetByIdAsync(leadId);
                if (lead == null || lead.IsDeleted)
                {
                    _logger.LogWarning("Lead {LeadId} not found", leadId);
                    return false;
                }

                lead.AddNotes(notes);
                await _leadRepository.UpdateAsync(lead);

                // Create activity
                var activity = new LeadActivity(
                    leadId: leadId,
                    type: ActivityType.Note,
                    title: "Note Added",
                    description: notes,
                    performedByUserId: userId
                );
                await _activityRepository.AddAsync(activity);

                _logger.LogInformation("Added notes to lead {LeadId}", leadId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding notes to lead {LeadId}", leadId);
                return false;
            }
        }

        public async Task<bool> SetLeadTagsAsync(Guid leadId, string? tags)
        {
            try
            {
                var lead = await _leadRepository.GetByIdAsync(leadId);
                if (lead == null || lead.IsDeleted)
                {
                    _logger.LogWarning("Lead {LeadId} not found", leadId);
                    return false;
                }

                lead.SetTags(tags);
                await _leadRepository.UpdateAsync(lead);

                _logger.LogInformation("Updated tags for lead {LeadId}", leadId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error setting tags for lead {LeadId}", leadId);
                return false;
            }
        }

        public async Task<IEnumerable<Lead>> SearchLeadsAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return Enumerable.Empty<Lead>();
            }

            var searchLower = searchTerm.ToLower();
            return await _leadRepository.FindAsync(l =>
                !l.IsDeleted &&
                (l.ContactName != null && l.ContactName.ToLower().Contains(searchLower) ||
                 l.Email != null && l.Email.ToLower().Contains(searchLower) ||
                 l.Phone != null && l.Phone.Contains(searchTerm) ||
                 l.CompanyName != null && l.CompanyName.ToLower().Contains(searchLower)));
        }

        #region Helper Methods

        private Dictionary<string, string> ParseCapturedData(IEnumerable<SalesFunnelMetric> funnelMetrics)
        {
            var result = new Dictionary<string, string>();

            foreach (var metric in funnelMetrics)
            {
                if (string.IsNullOrWhiteSpace(metric.CapturedData))
                    continue;

                try
                {
                    var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(metric.CapturedData);
                    if (data != null)
                    {
                        foreach (var kvp in data)
                        {
                            if (kvp.Value.ValueKind == JsonValueKind.String)
                            {
                                result[kvp.Key] = kvp.Value.GetString() ?? string.Empty;
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Error parsing captured data for session {SessionId}", metric.SessionId);
                }
            }

            return result;
        }

        private string? ParseUtmParameter(string? metadata, string paramName)
        {
            if (string.IsNullOrWhiteSpace(metadata))
                return null;

            try
            {
                var data = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(metadata);
                if (data != null && data.ContainsKey(paramName))
                {
                    return data[paramName].GetString();
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error parsing UTM parameter {ParamName}", paramName);
            }

            return null;
        }

        #endregion
    }
}
