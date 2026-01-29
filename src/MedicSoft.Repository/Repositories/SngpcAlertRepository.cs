using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    /// <summary>
    /// Repository implementation for managing SNGPC alerts.
    /// Provides persistence and querying capabilities for SNGPC compliance alerts.
    /// </summary>
    public class SngpcAlertRepository : BaseRepository<SngpcAlert>, ISngpcAlertRepository
    {
        public SngpcAlertRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public override async Task<SngpcAlert> AddAsync(SngpcAlert alert)
        {
            await _dbSet.AddAsync(alert);
            await _context.SaveChangesAsync();
            return alert;
        }

        public new async Task<SngpcAlert> UpdateAsync(SngpcAlert alert)
        {
            _dbSet.Update(alert);
            await _context.SaveChangesAsync();
            return alert;
        }

        public async Task<SngpcAlert?> GetByIdAsync(Guid id)
        {
            return await _dbSet
                .Include(a => a.RelatedReport)
                .Include(a => a.RelatedRegistry)
                .Include(a => a.RelatedBalance)
                .Include(a => a.AcknowledgedBy)
                .Include(a => a.ResolvedBy)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<IEnumerable<SngpcAlert>> GetActiveAlertsAsync(string tenantId, AlertSeverity? severity = null)
        {
            var query = _dbSet
                .Where(a => a.TenantId == tenantId && a.ResolvedAt == null);

            if (severity.HasValue)
            {
                query = query.Where(a => a.Severity == severity.Value);
            }

            return await query
                .OrderByDescending(a => a.Severity)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SngpcAlert>> GetByTypeAsync(string tenantId, AlertType type)
        {
            return await _dbSet
                .Where(a => a.TenantId == tenantId && a.Type == type)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SngpcAlert>> GetByReportIdAsync(Guid reportId)
        {
            return await _dbSet
                .Where(a => a.RelatedReportId == reportId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SngpcAlert>> GetByRegistryIdAsync(Guid registryId)
        {
            return await _dbSet
                .Where(a => a.RelatedRegistryId == registryId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SngpcAlert>> GetByBalanceIdAsync(Guid balanceId)
        {
            return await _dbSet
                .Where(a => a.RelatedBalanceId == balanceId)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SngpcAlert>> GetUnacknowledgedAlertsAsync(string tenantId)
        {
            return await _dbSet
                .Where(a => a.TenantId == tenantId && 
                           a.AcknowledgedAt == null &&
                           a.ResolvedAt == null)
                .OrderByDescending(a => a.Severity)
                .ThenByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SngpcAlert>> GetResolvedAlertsAsync(string tenantId, DateTime startDate, DateTime endDate)
        {
            return await _dbSet
                .Where(a => a.TenantId == tenantId &&
                           a.ResolvedAt != null &&
                           a.ResolvedAt >= startDate &&
                           a.ResolvedAt <= endDate)
                .OrderByDescending(a => a.ResolvedAt)
                .ToListAsync();
        }

        public async Task<Dictionary<AlertSeverity, int>> GetActiveAlertCountBySeverityAsync(string tenantId)
        {
            var counts = await _dbSet
                .Where(a => a.TenantId == tenantId && a.ResolvedAt == null)
                .GroupBy(a => a.Severity)
                .Select(g => new { Severity = g.Key, Count = g.Count() })
                .ToListAsync();

            var result = new Dictionary<AlertSeverity, int>();
            
            // Initialize all severities with 0
            foreach (AlertSeverity severity in Enum.GetValues(typeof(AlertSeverity)))
            {
                result[severity] = 0;
            }

            // Update with actual counts
            foreach (var item in counts)
            {
                result[item.Severity] = item.Count;
            }

            return result;
        }

        public async Task<int> DeleteOldResolvedAlertsAsync(DateTime olderThan)
        {
            var alertsToDelete = await _dbSet
                .Where(a => a.ResolvedAt != null && a.ResolvedAt < olderThan)
                .ToListAsync();

            _dbSet.RemoveRange(alertsToDelete);
            await _context.SaveChangesAsync();

            return alertsToDelete.Count;
        }
    }
}
