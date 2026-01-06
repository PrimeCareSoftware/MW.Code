using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface ISNGPCReportRepository : IRepository<SNGPCReport>
    {
        /// <summary>
        /// Gets an SNGPC report by month and year.
        /// </summary>
        Task<SNGPCReport?> GetByMonthYearAsync(int month, int year, string tenantId);

        /// <summary>
        /// Gets all reports for a specific year.
        /// </summary>
        Task<IEnumerable<SNGPCReport>> GetByYearAsync(int year, string tenantId);

        /// <summary>
        /// Gets reports by status.
        /// </summary>
        Task<IEnumerable<SNGPCReport>> GetByStatusAsync(SNGPCReportStatus status, string tenantId);

        /// <summary>
        /// Gets reports that are overdue for transmission.
        /// </summary>
        Task<IEnumerable<SNGPCReport>> GetOverdueReportsAsync(string tenantId);

        /// <summary>
        /// Gets reports that failed transmission and can be retried.
        /// </summary>
        Task<IEnumerable<SNGPCReport>> GetFailedReportsForRetryAsync(string tenantId);

        /// <summary>
        /// Gets the most recent report (for dashboard display).
        /// </summary>
        Task<SNGPCReport?> GetMostRecentReportAsync(string tenantId);

        /// <summary>
        /// Checks if a report already exists for a specific month/year.
        /// </summary>
        Task<bool> ReportExistsAsync(int month, int year, string tenantId);

        /// <summary>
        /// Gets transmission history (last N reports).
        /// </summary>
        Task<IEnumerable<SNGPCReport>> GetTransmissionHistoryAsync(int count, string tenantId);
    }
}
