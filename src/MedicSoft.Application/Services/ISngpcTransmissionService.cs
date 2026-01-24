using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for managing SNGPC data transmission to ANVISA.
    /// Handles report submission, retry logic, and transmission tracking.
    /// </summary>
    public interface ISngpcTransmissionService
    {
        /// <summary>
        /// Transmits an SNGPC report to ANVISA.
        /// Creates a transmission record and attempts to send the report.
        /// </summary>
        /// <param name="reportId">ID of the SNGPC report to transmit.</param>
        /// <param name="tenantId">Tenant identifier for multi-tenancy.</param>
        /// <param name="userId">ID of the user initiating the transmission (optional).</param>
        /// <returns>Transmission record with status and response details.</returns>
        /// <exception cref="ArgumentException">When report ID is invalid.</exception>
        /// <exception cref="InvalidOperationException">When report is not found or not ready for transmission.</exception>
        Task<SngpcTransmission> TransmitReportAsync(
            Guid reportId, 
            string tenantId, 
            Guid? userId);

        /// <summary>
        /// Retries a failed transmission.
        /// Checks retry limits and creates a new transmission attempt.
        /// </summary>
        /// <param name="transmissionId">ID of the failed transmission to retry.</param>
        /// <param name="tenantId">Tenant identifier for multi-tenancy.</param>
        /// <returns>New transmission record for the retry attempt.</returns>
        /// <exception cref="ArgumentException">When transmission ID is invalid.</exception>
        /// <exception cref="InvalidOperationException">When transmission cannot be retried or max attempts exceeded.</exception>
        Task<SngpcTransmission> RetryTransmissionAsync(
            Guid transmissionId, 
            string tenantId);

        /// <summary>
        /// Gets the complete transmission history for a specific report.
        /// Includes all transmission attempts with their statuses and responses.
        /// </summary>
        /// <param name="reportId">ID of the SNGPC report.</param>
        /// <param name="tenantId">Tenant identifier for multi-tenancy.</param>
        /// <returns>Collection of all transmission attempts for the report.</returns>
        Task<IEnumerable<SngpcTransmission>> GetTransmissionHistoryAsync(
            Guid reportId, 
            string tenantId);

        /// <summary>
        /// Gets transmission statistics for a specified date range.
        /// Includes success rates, average response times, and attempt counts.
        /// </summary>
        /// <param name="startDate">Start date of the period (inclusive).</param>
        /// <param name="endDate">End date of the period (inclusive).</param>
        /// <param name="tenantId">Tenant identifier for multi-tenancy.</param>
        /// <returns>Aggregated transmission statistics.</returns>
        Task<TransmissionStatistics> GetStatisticsAsync(
            DateTime startDate, 
            DateTime endDate, 
            string tenantId);
    }
}
