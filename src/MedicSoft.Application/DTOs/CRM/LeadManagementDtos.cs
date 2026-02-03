using System;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Application.DTOs.CRM
{
    /// <summary>
    /// Statistics about leads
    /// </summary>
    public class LeadStatistics
    {
        public int TotalLeads { get; set; }
        public int NewLeads { get; set; }
        public int ContactedLeads { get; set; }
        public int QualifiedLeads { get; set; }
        public int ConvertedLeads { get; set; }
        public int LostLeads { get; set; }
        public int NurturingLeads { get; set; }
        public int UnassignedLeads { get; set; }
        public int NeedingFollowUp { get; set; }
        public decimal ConversionRate { get; set; }
        public int AverageScore { get; set; }
    }

    /// <summary>
    /// Statistics for a specific user
    /// </summary>
    public class UserLeadStatistics
    {
        public Guid UserId { get; set; }
        public string? UserName { get; set; }
        public int AssignedLeads { get; set; }
        public int ConvertedLeads { get; set; }
        public int NeedingFollowUp { get; set; }
        public decimal ConversionRate { get; set; }
    }

    /// <summary>
    /// Result of a sync or bulk operation
    /// </summary>
    public class SyncResult
    {
        public int TotalLeads { get; set; }
        public int SuccessfulSyncs { get; set; }
        public int FailedSyncs { get; set; }
        public List<string> Errors { get; set; } = new List<string>();
    }

    /// <summary>
    /// Request to assign a lead
    /// </summary>
    public class AssignLeadRequest
    {
        public Guid UserId { get; set; }
    }

    /// <summary>
    /// Request to update lead status
    /// </summary>
    public class UpdateLeadStatusRequest
    {
        public LeadStatus Status { get; set; }
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Request to schedule follow-up
    /// </summary>
    public class ScheduleFollowUpRequest
    {
        public DateTime FollowUpDate { get; set; }
    }

    /// <summary>
    /// Request to add activity
    /// </summary>
    public class AddActivityRequest
    {
        public ActivityType Type { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public int? DurationMinutes { get; set; }
        public string? Outcome { get; set; }
    }

    /// <summary>
    /// Request to update contact info
    /// </summary>
    public class UpdateContactInfoRequest
    {
        public string? ContactName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
    }

    /// <summary>
    /// Request to add notes
    /// </summary>
    public class AddNotesRequest
    {
        public string Notes { get; set; } = null!;
    }

    /// <summary>
    /// Request to set tags
    /// </summary>
    public class SetTagsRequest
    {
        public string? Tags { get; set; }
    }
}
