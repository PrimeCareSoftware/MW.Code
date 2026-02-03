using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Application.Services.CRM
{
    /// <summary>
    /// Service for managing Salesforce lead integration
    /// </summary>
    public interface ISalesforceLeadService
    {
        /// <summary>
        /// Create a lead from abandoned registration funnel
        /// </summary>
        Task<SalesforceLead> CreateLeadFromFunnelAsync(string sessionId);

        /// <summary>
        /// Sync a specific lead to Salesforce
        /// </summary>
        Task<bool> SyncLeadToSalesforceAsync(Guid leadId);

        /// <summary>
        /// Sync all unsynced leads to Salesforce
        /// </summary>
        Task<SyncResult> SyncAllLeadsAsync();

        /// <summary>
        /// Get leads that haven't been synced yet
        /// </summary>
        Task<IEnumerable<SalesforceLead>> GetUnsyncedLeadsAsync();

        /// <summary>
        /// Get leads by status
        /// </summary>
        Task<IEnumerable<SalesforceLead>> GetLeadsByStatusAsync(LeadStatus status);

        /// <summary>
        /// Update lead status
        /// </summary>
        Task<bool> UpdateLeadStatusAsync(Guid leadId, LeadStatus newStatus);

        /// <summary>
        /// Get lead statistics
        /// </summary>
        Task<LeadStatistics> GetLeadStatisticsAsync();

        /// <summary>
        /// Check Salesforce connection status
        /// </summary>
        Task<bool> TestConnectionAsync();
    }

    /// <summary>
    /// Result of bulk sync operation
    /// </summary>
    public class SyncResult
    {
        public int TotalLeads { get; set; }
        public int SuccessfulSyncs { get; set; }
        public int FailedSyncs { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    /// <summary>
    /// Lead statistics
    /// </summary>
    public class LeadStatistics
    {
        public int TotalLeads { get; set; }
        public int NewLeads { get; set; }
        public int ContactedLeads { get; set; }
        public int QualifiedLeads { get; set; }
        public int ConvertedLeads { get; set; }
        public int LostLeads { get; set; }
        public int SyncedLeads { get; set; }
        public int UnsyncedLeads { get; set; }
        public Dictionary<int, int> LeadsByStep { get; set; } = new();
    }
}
