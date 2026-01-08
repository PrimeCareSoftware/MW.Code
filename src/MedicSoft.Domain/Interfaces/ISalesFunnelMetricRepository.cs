using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Repository interface for sales funnel metrics tracking
    /// </summary>
    public interface ISalesFunnelMetricRepository : IRepository<SalesFunnelMetric>
    {
        /// <summary>
        /// Get all metrics for a specific session
        /// </summary>
        Task<IEnumerable<SalesFunnelMetric>> GetBySessionIdAsync(string sessionId);

        /// <summary>
        /// Get metrics within a date range
        /// </summary>
        Task<IEnumerable<SalesFunnelMetric>> GetByDateRangeAsync(DateTime startDate, DateTime endDate);

        /// <summary>
        /// Get metrics for a specific step
        /// </summary>
        Task<IEnumerable<SalesFunnelMetric>> GetByStepAsync(int step, DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Get conversion rate statistics
        /// </summary>
        Task<(int TotalSessions, int Conversions, double ConversionRate)> GetConversionStatsAsync(DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Get step completion statistics
        /// </summary>
        Task<Dictionary<int, (int Entered, int Completed, int Abandoned)>> GetStepStatsAsync(DateTime? startDate = null, DateTime? endDate = null);

        /// <summary>
        /// Get incomplete sessions (not converted and last activity older than threshold)
        /// </summary>
        Task<IEnumerable<SalesFunnelMetric>> GetIncompleteSessions(DateTime olderThan, int limit = 100);

        /// <summary>
        /// Get most recent metric for each session
        /// </summary>
        Task<IEnumerable<SalesFunnelMetric>> GetLatestMetricPerSessionAsync(DateTime? startDate = null, DateTime? endDate = null, int limit = 100);
    }
}
