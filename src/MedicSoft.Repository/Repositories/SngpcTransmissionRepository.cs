using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class SngpcTransmissionRepository : BaseRepository<SngpcTransmission>, ISngpcTransmissionRepository
    {
        public SngpcTransmissionRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SngpcTransmission>> GetByReportIdAsync(
            Guid reportId,
            string tenantId)
        {
            return await _dbSet
                .Where(st => st.TenantId == tenantId &&
                            st.SNGPCReportId == reportId)
                .OrderBy(st => st.AttemptNumber)
                .ToListAsync();
        }

        public async Task<SngpcTransmission?> GetMostRecentTransmissionAsync(
            Guid reportId,
            string tenantId)
        {
            return await _dbSet
                .Where(st => st.TenantId == tenantId &&
                            st.SNGPCReportId == reportId)
                .OrderByDescending(st => st.AttemptNumber)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SngpcTransmission>> GetPendingTransmissionsAsync(string tenantId)
        {
            return await _dbSet
                .Where(st => st.TenantId == tenantId &&
                            (st.Status == TransmissionStatus.Pending ||
                             st.Status == TransmissionStatus.InProgress))
                .OrderBy(st => st.AttemptedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SngpcTransmission>> GetFailedTransmissionsForRetryAsync(string tenantId)
        {
            var failedTransmissions = await _dbSet
                .Where(st => st.TenantId == tenantId &&
                            (st.Status == TransmissionStatus.Failed ||
                             st.Status == TransmissionStatus.TimedOut))
                .ToListAsync();

            // Filter for transmissions that can retry
            return failedTransmissions.Where(st => st.CanRetry()).ToList();
        }

        public async Task<IEnumerable<SngpcTransmission>> GetByStatusAsync(
            TransmissionStatus status,
            string tenantId)
        {
            return await _dbSet
                .Where(st => st.TenantId == tenantId &&
                            st.Status == status)
                .OrderByDescending(st => st.AttemptedAt)
                .ToListAsync();
        }

        public async Task<TransmissionStatistics> GetTransmissionStatisticsAsync(
            DateTime startDate,
            DateTime endDate,
            string tenantId)
        {
            var transmissions = await _dbSet
                .Where(st => st.TenantId == tenantId &&
                            st.AttemptedAt >= startDate &&
                            st.AttemptedAt <= endDate)
                .ToListAsync();

            var totalAttempts = transmissions.Count;
            var successfulAttempts = transmissions.Count(st => st.Status == TransmissionStatus.Successful);
            var failedAttempts = transmissions.Count(st => st.Status == TransmissionStatus.Failed ||
                                                            st.Status == TransmissionStatus.TimedOut);
            var pendingAttempts = transmissions.Count(st => st.Status == TransmissionStatus.Pending ||
                                                            st.Status == TransmissionStatus.InProgress);

            var successRate = totalAttempts > 0 ? (decimal)successfulAttempts / totalAttempts * 100 : 0;

            var successfulWithResponseTime = transmissions
                .Where(st => st.Status == TransmissionStatus.Successful && st.ResponseTimeMs.HasValue)
                .ToList();

            var averageResponseTime = successfulWithResponseTime.Any()
                ? (long)successfulWithResponseTime.Average(st => st.ResponseTimeMs!.Value)
                : 0;

            return new TransmissionStatistics
            {
                TotalAttempts = totalAttempts,
                SuccessfulAttempts = successfulAttempts,
                FailedAttempts = failedAttempts,
                PendingAttempts = pendingAttempts,
                SuccessRate = successRate,
                AverageResponseTimeMs = averageResponseTime
            };
        }

        public async Task<int> GetAttemptCountAsync(Guid reportId, string tenantId)
        {
            return await _dbSet
                .Where(st => st.TenantId == tenantId &&
                            st.SNGPCReportId == reportId)
                .CountAsync();
        }

        public async Task<decimal> GetSuccessRateAsync(string tenantId)
        {
            var transmissions = await _dbSet
                .Where(st => st.TenantId == tenantId)
                .ToListAsync();

            if (!transmissions.Any())
                return 0;

            var successfulCount = transmissions.Count(st => st.Status == TransmissionStatus.Successful);
            return (decimal)successfulCount / transmissions.Count * 100;
        }
    }
}
