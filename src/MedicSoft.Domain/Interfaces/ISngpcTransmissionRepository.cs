using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface ISngpcTransmissionRepository : IRepository<SngpcTransmission>
    {
        /// <summary>
        /// Gets all transmission attempts for a specific SNGPC report.
        /// </summary>
        Task<IEnumerable<SngpcTransmission>> GetByReportIdAsync(
            Guid reportId,
            string tenantId);

        /// <summary>
        /// Gets the most recent transmission for a report.
        /// </summary>
        Task<SngpcTransmission?> GetMostRecentTransmissionAsync(
            Guid reportId,
            string tenantId);

        /// <summary>
        /// Gets all pending transmissions (queued for sending).
        /// </summary>
        Task<IEnumerable<SngpcTransmission>> GetPendingTransmissionsAsync(string tenantId);

        /// <summary>
        /// Gets failed transmissions that can be retried.
        /// </summary>
        Task<IEnumerable<SngpcTransmission>> GetFailedTransmissionsForRetryAsync(string tenantId);

        /// <summary>
        /// Gets transmissions by status.
        /// </summary>
        Task<IEnumerable<SngpcTransmission>> GetByStatusAsync(
            TransmissionStatus status,
            string tenantId);

        /// <summary>
        /// Gets transmission statistics for a date range.
        /// </summary>
        Task<TransmissionStatistics> GetTransmissionStatisticsAsync(
            DateTime startDate,
            DateTime endDate,
            string tenantId);

        /// <summary>
        /// Gets the count of transmission attempts for a report.
        /// </summary>
        Task<int> GetAttemptCountAsync(Guid reportId, string tenantId);

        /// <summary>
        /// Gets the success rate for transmissions.
        /// </summary>
        Task<decimal> GetSuccessRateAsync(string tenantId);
    }

    /// <summary>
    /// Statistics for SNGPC transmissions
    /// </summary>
    public class TransmissionStatistics
    {
        public int TotalAttempts { get; set; }
        public int SuccessfulAttempts { get; set; }
        public int FailedAttempts { get; set; }
        public int PendingAttempts { get; set; }
        public decimal SuccessRate { get; set; }
        public long AverageResponseTimeMs { get; set; }
    }
}
