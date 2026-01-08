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

            // Use database-level aggregation instead of in-memory processing
            var stepStats = new Dictionary<int, (int Entered, int Completed, int Abandoned)>();

            // Query for each action type efficiently
            var enteredCounts = await query
                .Where(m => m.Action == "entered")
                .GroupBy(m => m.Step)
                .Select(g => new { Step = g.Key, Count = g.Count() })
                .ToListAsync();

            var completedCounts = await query
                .Where(m => m.Action == "completed")
                .GroupBy(m => m.Step)
                .Select(g => new { Step = g.Key, Count = g.Count() })
                .ToListAsync();

            var abandonedCounts = await query
                .Where(m => m.Action == "abandoned")
                .GroupBy(m => m.Step)
                .Select(g => new { Step = g.Key, Count = g.Count() })
                .ToListAsync();

            // Combine results
            for (int step = 1; step <= 6; step++)
            {
                var entered = enteredCounts.FirstOrDefault(x => x.Step == step)?.Count ?? 0;
                var completed = completedCounts.FirstOrDefault(x => x.Step == step)?.Count ?? 0;
                var abandoned = abandonedCounts.FirstOrDefault(x => x.Step == step)?.Count ?? 0;

                stepStats[step] = (entered, completed, abandoned);
            }

            return stepStats;
        }

        public async Task<IEnumerable<SalesFunnelMetric>> GetIncompleteSessions(DateTime olderThan, int limit = 100)
        {
            // Use a single query with window functions to get the latest metric per session
            // This avoids N+1 query problem
            var latestMetrics = await _dbSet
                .Where(m => !m.IsConverted && m.CreatedAt < olderThan)
                .GroupBy(m => m.SessionId)
                .Select(g => g.OrderByDescending(m => m.CreatedAt).FirstOrDefault())
                .Take(limit)
                .ToListAsync();

            return latestMetrics.Where(m => m != null).Cast<SalesFunnelMetric>().ToList();
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

            // Use a single query with GroupBy and FirstOrDefault to avoid N+1
            var latestMetrics = await query
                .GroupBy(m => m.SessionId)
                .Select(g => g.OrderByDescending(m => m.CreatedAt).FirstOrDefault())
                .Take(limit)
                .ToListAsync();

            return latestMetrics.Where(m => m != null).Cast<SalesFunnelMetric>()
                .OrderByDescending(m => m.CreatedAt).ToList();
        }
    }
}
