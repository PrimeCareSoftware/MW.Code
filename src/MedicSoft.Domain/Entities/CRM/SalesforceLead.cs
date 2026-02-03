using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Represents a lead captured from abandoned registration flows
    /// to be synced with Salesforce for follow-up campaigns
    /// </summary>
    public class SalesforceLead : BaseEntity
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
        /// Lead status: New, Contacted, Qualified, Converted, Lost
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
        /// Salesforce Lead ID after sync (null if not yet synced)
        /// </summary>
        public string? SalesforceLeadId { get; private set; }

        /// <summary>
        /// Whether this lead has been synced to Salesforce
        /// </summary>
        public bool IsSyncedToSalesforce { get; private set; }

        /// <summary>
        /// Date/time when synced to Salesforce
        /// </summary>
        public DateTime? SyncedAt { get; private set; }

        /// <summary>
        /// Number of sync attempts (for retry logic)
        /// </summary>
        public int SyncAttempts { get; private set; }

        /// <summary>
        /// Last sync error message if any
        /// </summary>
        public string? LastSyncError { get; private set; }

        /// <summary>
        /// Additional metadata in JSON format
        /// </summary>
        public string? Metadata { get; private set; }

        private SalesforceLead()
        {
            // EF Constructor
            SessionId = null!;
            LeadSource = null!;
        }

        public SalesforceLead(
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
            string? metadata = null) : base(TenantConstants.SystemTenantId)
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
            IsSyncedToSalesforce = false;
            SyncAttempts = 0;
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
            UpdateTimestamp();
        }

        /// <summary>
        /// Update lead status
        /// </summary>
        public void UpdateStatus(LeadStatus newStatus)
        {
            Status = newStatus;
            LastActivityAt = DateTime.UtcNow;
            UpdateTimestamp();
        }

        /// <summary>
        /// Mark lead as synced with Salesforce
        /// </summary>
        public void MarkAsSynced(string salesforceLeadId)
        {
            if (string.IsNullOrWhiteSpace(salesforceLeadId))
                throw new ArgumentException("Salesforce Lead ID cannot be empty", nameof(salesforceLeadId));

            SalesforceLeadId = salesforceLeadId;
            IsSyncedToSalesforce = true;
            SyncedAt = DateTime.UtcNow;
            LastSyncError = null;
            UpdateTimestamp();
        }

        /// <summary>
        /// Record a failed sync attempt
        /// </summary>
        public void RecordSyncFailure(string errorMessage)
        {
            SyncAttempts++;
            LastSyncError = errorMessage;
            UpdateTimestamp();
        }

        /// <summary>
        /// Reset sync attempts counter
        /// </summary>
        public void ResetSyncAttempts()
        {
            SyncAttempts = 0;
            LastSyncError = null;
            UpdateTimestamp();
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
