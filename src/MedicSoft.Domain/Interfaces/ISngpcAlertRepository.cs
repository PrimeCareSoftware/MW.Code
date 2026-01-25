using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for managing SNGPC alerts.
    /// Provides persistence and querying capabilities for SNGPC compliance alerts.
    /// </summary>
    public interface ISngpcAlertRepository
    {
        /// <summary>
        /// Adds a new alert to the repository
        /// </summary>
        Task<SngpcAlert> AddAsync(SngpcAlert alert);

        /// <summary>
        /// Updates an existing alert
        /// </summary>
        Task<SngpcAlert> UpdateAsync(SngpcAlert alert);

        /// <summary>
        /// Gets an alert by its ID
        /// </summary>
        Task<SngpcAlert?> GetByIdAsync(Guid id);

        /// <summary>
        /// Gets all active alerts for a tenant
        /// </summary>
        /// <param name="tenantId">The tenant to query</param>
        /// <param name="severity">Optional severity filter</param>
        /// <returns>List of active alerts</returns>
        Task<IEnumerable<SngpcAlert>> GetActiveAlertsAsync(string tenantId, AlertSeverity? severity = null);

        /// <summary>
        /// Gets alerts by type for a tenant
        /// </summary>
        Task<IEnumerable<SngpcAlert>> GetByTypeAsync(string tenantId, AlertType type);

        /// <summary>
        /// Gets all alerts for a specific report
        /// </summary>
        Task<IEnumerable<SngpcAlert>> GetByReportIdAsync(Guid reportId);

        /// <summary>
        /// Gets all alerts for a specific registry entry
        /// </summary>
        Task<IEnumerable<SngpcAlert>> GetByRegistryIdAsync(Guid registryId);

        /// <summary>
        /// Gets all alerts for a specific balance
        /// </summary>
        Task<IEnumerable<SngpcAlert>> GetByBalanceIdAsync(Guid balanceId);

        /// <summary>
        /// Gets unacknowledged alerts for a tenant
        /// </summary>
        Task<IEnumerable<SngpcAlert>> GetUnacknowledgedAlertsAsync(string tenantId);

        /// <summary>
        /// Gets resolved alerts within a date range
        /// </summary>
        Task<IEnumerable<SngpcAlert>> GetResolvedAlertsAsync(string tenantId, DateTime startDate, DateTime endDate);

        /// <summary>
        /// Gets count of active alerts by severity
        /// </summary>
        Task<Dictionary<AlertSeverity, int>> GetActiveAlertCountBySeverityAsync(string tenantId);

        /// <summary>
        /// Deletes old resolved alerts (for cleanup)
        /// </summary>
        /// <param name="olderThan">Delete alerts resolved before this date</param>
        Task<int> DeleteOldResolvedAlertsAsync(DateTime olderThan);
    }
}
