using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class SalesFunnelMetricRepository : BaseRepository<SalesFunnelMetric>, ISalesFunnelMetricRepository
    {
        public SalesFunnelMetricRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SalesFunnelMetric>> GetBySessionIdAsync(string sessionId)
        {
            return await _dbSet
                .Where(m => m.SessionId == sessionId)
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SalesFunnelMetric>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(m => m.CreatedAt >= startDate && m.CreatedAt <= endDate)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SalesFunnelMetric>> GetByStepAsync(int step, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _dbSet.Where(m => m.Step == step);

            if (startDate.HasValue)
                query = query.Where(m => m.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(m => m.CreatedAt <= endDate.Value);

            return await query
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
        }

        public async Task<(int TotalSessions, int Conversions, double ConversionRate)> GetConversionStatsAsync(
            DateTime? startDate = null, 
            DateTime? endDate = null)
        {
            var query = _dbSet.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(m => m.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(m => m.CreatedAt <= endDate.Value);

            // Get unique sessions
            var totalSessions = await query
                .Select(m => m.SessionId)
                .Distinct()
                .CountAsync();

            // Get converted sessions
            var conversions = await query
                .Where(m => m.IsConverted)
                .Select(m => m.SessionId)
                .Distinct()
                .CountAsync();

            var conversionRate = totalSessions > 0 ? (double)conversions / totalSessions * 100 : 0;

            return (totalSessions, conversions, conversionRate);
        }

        public async Task<Dictionary<int, (int Entered, int Completed, int Abandoned)>> GetStepStatsAsync(
            DateTime? startDate = null, 
            DateTime? endDate = null)
        {
            var query = _dbSet.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(m => m.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(m => m.CreatedAt <= endDate.Value);

            var metrics = await query.ToListAsync();

            var stepStats = new Dictionary<int, (int Entered, int Completed, int Abandoned)>();

            for (int step = 1; step <= 6; step++)
            {
                var stepMetrics = metrics.Where(m => m.Step == step).ToList();
                
                var entered = stepMetrics.Count(m => m.Action == "entered");
                var completed = stepMetrics.Count(m => m.Action == "completed");
                var abandoned = stepMetrics.Count(m => m.Action == "abandoned");

                stepStats[step] = (entered, completed, abandoned);
            }

            return stepStats;
        }

        public async Task<IEnumerable<SalesFunnelMetric>> GetIncompleteSessions(DateTime olderThan, int limit = 100)
        {
            // Get the latest metric for each session that is not converted and older than threshold
            var sessionIds = await _dbSet
                .Where(m => !m.IsConverted && m.CreatedAt < olderThan)
                .Select(m => m.SessionId)
                .Distinct()
                .Take(limit)
                .ToListAsync();

            var latestMetrics = new List<SalesFunnelMetric>();

            foreach (var sessionId in sessionIds)
            {
                var latestMetric = await _dbSet
                    .Where(m => m.SessionId == sessionId)
                    .OrderByDescending(m => m.CreatedAt)
                    .FirstOrDefaultAsync();

                if (latestMetric != null)
                    latestMetrics.Add(latestMetric);
            }

            return latestMetrics;
        }

        public async Task<IEnumerable<SalesFunnelMetric>> GetLatestMetricPerSessionAsync(
            DateTime? startDate = null, 
            DateTime? endDate = null, 
            int limit = 100)
        {
            var query = _dbSet.AsQueryable();

            if (startDate.HasValue)
                query = query.Where(m => m.CreatedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(m => m.CreatedAt <= endDate.Value);

            // Get unique session IDs
            var sessionIds = await query
                .Select(m => m.SessionId)
                .Distinct()
                .Take(limit)
                .ToListAsync();

            var latestMetrics = new List<SalesFunnelMetric>();

            foreach (var sessionId in sessionIds)
            {
                var latestMetric = await _dbSet
                    .Where(m => m.SessionId == sessionId)
                    .OrderByDescending(m => m.CreatedAt)
                    .FirstOrDefaultAsync();

                if (latestMetric != null)
                    latestMetrics.Add(latestMetric);
            }

            return latestMetrics.OrderByDescending(m => m.CreatedAt).ToList();
        }
    }
}
