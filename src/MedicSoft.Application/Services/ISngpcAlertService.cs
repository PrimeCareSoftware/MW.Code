using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Interface for SNGPC monitoring and alerting service.
    /// Provides deadline monitoring, compliance validation, and anomaly detection.
    /// Reference: ANVISA RDC 27/2007
    /// </summary>
    public interface ISngpcAlertService
    {
        /// <summary>
        /// Checks for reports approaching their submission deadline.
        /// ANVISA requires monthly submission by the 15th of the following month.
        /// </summary>
        /// <param name="tenantId">The tenant to check</param>
        /// <param name="daysBeforeDeadline">How many days before deadline to alert (default: 5)</param>
        /// <returns>List of reports approaching deadline</returns>
        Task<IEnumerable<SngpcAlertDto>> CheckApproachingDeadlinesAsync(string tenantId, int daysBeforeDeadline = 5);

        /// <summary>
        /// Checks for overdue reports that should have been submitted.
        /// </summary>
        /// <param name="tenantId">The tenant to check</param>
        /// <returns>List of overdue reports</returns>
        Task<IEnumerable<SngpcAlertDto>> CheckOverdueReportsAsync(string tenantId);

        /// <summary>
        /// Validates compliance of controlled medication registry.
        /// Checks for missing entries, incorrect balances, etc.
        /// </summary>
        /// <param name="tenantId">The tenant to validate</param>
        /// <returns>List of compliance issues found</returns>
        Task<IEnumerable<SngpcAlertDto>> ValidateComplianceAsync(string tenantId);

        /// <summary>
        /// Detects anomalies in controlled medication movements.
        /// Unusual patterns, negative balances, excessive dispensing, etc.
        /// </summary>
        /// <param name="tenantId">The tenant to check</param>
        /// <param name="startDate">Start of period to analyze</param>
        /// <param name="endDate">End of period to analyze</param>
        /// <returns>List of detected anomalies</returns>
        Task<IEnumerable<SngpcAlertDto>> DetectAnomaliesAsync(string tenantId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets all active alerts for a tenant.
        /// </summary>
        /// <param name="tenantId">The tenant to check</param>
        /// <param name="severity">Optional severity filter</param>
        /// <returns>List of active alerts</returns>
        Task<IEnumerable<SngpcAlertDto>> GetActiveAlertsAsync(string tenantId, AlertSeverity? severity = null);

        /// <summary>
        /// Marks an alert as acknowledged.
        /// </summary>
        /// <param name="alertId">The alert to acknowledge</param>
        /// <param name="userId">User acknowledging the alert</param>
        /// <param name="notes">Optional notes about the acknowledgment</param>
        Task AcknowledgeAlertAsync(Guid alertId, Guid userId, string? notes = null);

        /// <summary>
        /// Marks an alert as resolved.
        /// </summary>
        /// <param name="alertId">The alert to resolve</param>
        /// <param name="userId">User resolving the alert</param>
        /// <param name="resolution">Description of the resolution</param>
        Task ResolveAlertAsync(Guid alertId, Guid userId, string resolution);
    }

    /// <summary>
    /// Represents an SNGPC-related alert DTO
    /// </summary>
    public class SngpcAlertDto
    {
        public Guid Id { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public AlertType Type { get; set; }
        public AlertSeverity Severity { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? AcknowledgedAt { get; set; }
        public Guid? AcknowledgedByUserId { get; set; }
        public string? AcknowledgmentNotes { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public Guid? ResolvedByUserId { get; set; }
        public string? Resolution { get; set; }
        public bool IsActive => ResolvedAt == null;
        
        // Related entity information
        public Guid? RelatedReportId { get; set; }
        public Guid? RelatedRegistryId { get; set; }
        public string? RelatedMedication { get; set; }
        
        // Additional data (JSON serialized)
        public string? AdditionalData { get; set; }
    }
}

