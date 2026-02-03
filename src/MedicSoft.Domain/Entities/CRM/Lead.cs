using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Represents a lead captured from abandoned registration flows
    /// for internal follow-up and conversion campaigns
    /// </summary>
    public class Lead : BaseEntity
    {
        /// <summary>
        /// Session identifier from SalesFunnelMetric
        /// </summary>
        public string SessionId { get; private set; }

        /// <summary>
        /// Clinic/Company name captured during registration
        /// </summary>
        public string? CompanyName { get; private set; }

        /// <summary>
        /// Contact person's full name
        /// </summary>
        public string? ContactName { get; private set; }

        /// <summary>
        /// Contact email address
        /// </summary>
        public string? Email { get; private set; }

        /// <summary>
        /// Contact phone number
        /// </summary>
        public string? Phone { get; private set; }

        /// <summary>
        /// City where the clinic is located
        /// </summary>
        public string? City { get; private set; }

        /// <summary>
        /// State/Province code
        /// </summary>
        public string? State { get; private set; }

        /// <summary>
        /// Selected plan ID during registration
        /// </summary>
        public string? PlanId { get; private set; }

        /// <summary>
        /// Selected plan name
        /// </summary>
        public string? PlanName { get; private set; }

        /// <summary>
        /// Last step reached in registration flow (1-6)
        /// </summary>
        public int LastStepReached { get; private set; }

        /// <summary>
        /// Lead source/origin (e.g., "Website Registration", "Landing Page")
        /// </summary>
        public string LeadSource { get; private set; }

        /// <summary>
        /// Lead status: New, Contacted, Qualified, Converted, Lost, Nurturing
        /// </summary>
        public LeadStatus Status { get; private set; }

        /// <summary>
        /// Referrer URL to track traffic source
        /// </summary>
        public string? Referrer { get; private set; }

        /// <summary>
        /// UTM campaign parameters for marketing attribution
        /// </summary>
        public string? UtmCampaign { get; private set; }

        /// <summary>
        /// UTM source parameter
        /// </summary>
        public string? UtmSource { get; private set; }

        /// <summary>
        /// UTM medium parameter
        /// </summary>
        public string? UtmMedium { get; private set; }

        /// <summary>
        /// Date/time when the lead was first captured
        /// </summary>
        public DateTime CapturedAt { get; private set; }

        /// <summary>
        /// Date/time of last activity/update
        /// </summary>
        public DateTime? LastActivityAt { get; private set; }

        /// <summary>
        /// User ID of the person assigned to follow up this lead
        /// </summary>
        public Guid? AssignedToUserId { get; private set; }

        /// <summary>
        /// Date/time when assigned
        /// </summary>
        public DateTime? AssignedAt { get; private set; }

        /// <summary>
        /// Next scheduled follow-up date
        /// </summary>
        public DateTime? NextFollowUpDate { get; private set; }

        /// <summary>
        /// Lead score (0-100) based on engagement and data quality
        /// </summary>
        public int Score { get; private set; }

        /// <summary>
        /// Tags for lead categorization (comma-separated)
        /// </summary>
        public string? Tags { get; private set; }

        /// <summary>
        /// Notes about this lead
        /// </summary>
        public string? Notes { get; private set; }

        /// <summary>
        /// Additional metadata in JSON format
        /// </summary>
        public string? Metadata { get; private set; }

        /// <summary>
        /// Navigation property for activities
        /// </summary>
        public virtual ICollection<LeadActivity> Activities { get; private set; }

        private Lead()
        {
            // EF Constructor
            SessionId = null!;
            LeadSource = null!;
            Activities = new List<LeadActivity>();
        }

        public Lead(
            string sessionId,
            string leadSource,
            int lastStepReached,
            string? companyName = null,
            string? contactName = null,
            string? email = null,
            string? phone = null,
            string? city = null,
            string? state = null,
            string? planId = null,
            string? planName = null,
            string? referrer = null,
            string? utmCampaign = null,
            string? utmSource = null,
            string? utmMedium = null,
            string? metadata = null) : base(TenantConstants.SystemTenantId) // Leads are captured from public registration flow (system-wide)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException("Session ID cannot be empty", nameof(sessionId));

            if (string.IsNullOrWhiteSpace(leadSource))
                throw new ArgumentException("Lead source cannot be empty", nameof(leadSource));

            if (lastStepReached < 1 || lastStepReached > 6)
                throw new ArgumentException("Last step must be between 1 and 6", nameof(lastStepReached));

            SessionId = sessionId;
            LeadSource = leadSource;
            LastStepReached = lastStepReached;
            CompanyName = companyName;
            ContactName = contactName;
            Email = email;
            Phone = phone;
            City = city;
            State = state;
            PlanId = planId;
            PlanName = planName;
            Referrer = referrer;
            UtmCampaign = utmCampaign;
            UtmSource = utmSource;
            UtmMedium = utmMedium;
            Metadata = metadata;
            Status = LeadStatus.New;
            CapturedAt = DateTime.UtcNow;
            Score = CalculateInitialScore();
            Activities = new List<LeadActivity>();
        }

        /// <summary>
        /// Update lead contact information
        /// </summary>
        public void UpdateContactInfo(string? contactName, string? email, string? phone)
        {
            ContactName = contactName;
            Email = email;
            Phone = phone;
            LastActivityAt = DateTime.UtcNow;
            RecalculateScore();
            UpdateTimestamp();
        }

        /// <summary>
        /// Update lead status
        /// </summary>
        public void UpdateStatus(LeadStatus newStatus, string? notes = null)
        {
            Status = newStatus;
            LastActivityAt = DateTime.UtcNow;
            
            if (!string.IsNullOrWhiteSpace(notes))
            {
                Notes = string.IsNullOrWhiteSpace(Notes) 
                    ? notes 
                    : $"{Notes}\n---\n{notes}";
            }
            
            UpdateTimestamp();
        }

        /// <summary>
        /// Assign lead to a user
        /// </summary>
        public void AssignTo(Guid userId)
        {
            AssignedToUserId = userId;
            AssignedAt = DateTime.UtcNow;
            LastActivityAt = DateTime.UtcNow;
            UpdateTimestamp();
        }

        /// <summary>
        /// Unassign lead
        /// </summary>
        public void Unassign()
        {
            AssignedToUserId = null;
            AssignedAt = null;
            LastActivityAt = DateTime.UtcNow;
            UpdateTimestamp();
        }

        /// <summary>
        /// Schedule next follow-up
        /// </summary>
        public void ScheduleFollowUp(DateTime followUpDate)
        {
            if (followUpDate < DateTime.UtcNow)
                throw new ArgumentException("Follow-up date cannot be in the past", nameof(followUpDate));

            NextFollowUpDate = followUpDate;
            LastActivityAt = DateTime.UtcNow;
            UpdateTimestamp();
        }

        /// <summary>
        /// Clear follow-up date
        /// </summary>
        public void ClearFollowUp()
        {
            NextFollowUpDate = null;
            UpdateTimestamp();
        }

        /// <summary>
        /// Update lead score manually or trigger recalculation
        /// </summary>
        public void UpdateScore(int score)
        {
            if (score < 0 || score > 100)
                throw new ArgumentException("Score must be between 0 and 100", nameof(score));

            Score = score;
            UpdateTimestamp();
        }

        /// <summary>
        /// Add or update tags
        /// </summary>
        public void SetTags(string? tags)
        {
            Tags = tags;
            UpdateTimestamp();
        }

        /// <summary>
        /// Add notes
        /// </summary>
        public void AddNotes(string notes)
        {
            if (string.IsNullOrWhiteSpace(notes))
                return;

            Notes = string.IsNullOrWhiteSpace(Notes) 
                ? notes 
                : $"{Notes}\n---\n{notes}";
            
            LastActivityAt = DateTime.UtcNow;
            UpdateTimestamp();
        }

        /// <summary>
        /// Calculate initial score based on captured data quality
        /// </summary>
        private int CalculateInitialScore()
        {
            int score = 50; // Base score

            // Email presence (most important)
            if (!string.IsNullOrWhiteSpace(Email)) score += 20;

            // Phone presence
            if (!string.IsNullOrWhiteSpace(Phone)) score += 15;

            // Company name
            if (!string.IsNullOrWhiteSpace(CompanyName)) score += 10;

            // Contact name
            if (!string.IsNullOrWhiteSpace(ContactName)) score += 5;

            // Location data
            if (!string.IsNullOrWhiteSpace(City)) score += 5;
            if (!string.IsNullOrWhiteSpace(State)) score += 5;

            // Plan selection (shows serious interest)
            if (!string.IsNullOrWhiteSpace(PlanId)) score += 10;

            // Steps reached (the further, the better)
            score += LastStepReached * 2;

            // UTM tracking (shows targeted campaign)
            if (!string.IsNullOrWhiteSpace(UtmCampaign)) score += 5;

            return Math.Min(score, 100);
        }

        /// <summary>
        /// Recalculate score based on current data
        /// </summary>
        private void RecalculateScore()
        {
            Score = CalculateInitialScore();
        }
    }

    /// <summary>
    /// Lead status enumeration
    /// </summary>
    public enum LeadStatus
    {
        New = 0,
        Contacted = 1,
        Qualified = 2,
        Converted = 3,
        Lost = 4,
        Nurturing = 5
    }
}
