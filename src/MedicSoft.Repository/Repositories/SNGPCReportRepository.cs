using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class SNGPCReportRepository : BaseRepository<SNGPCReport>, ISNGPCReportRepository
    {
        public SNGPCReportRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<SNGPCReport?> GetByMonthYearAsync(int month, int year, string tenantId)
        {
            return await _dbSet
                .Where(sr => sr.Month == month && sr.Year == year && sr.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<SNGPCReport>> GetByYearAsync(int year, string tenantId)
        {
            return await _dbSet
                .Where(sr => sr.Year == year && sr.TenantId == tenantId)
                .OrderBy(sr => sr.Month)
                .ToListAsync();
        }

        public async Task<IEnumerable<SNGPCReport>> GetByStatusAsync(SNGPCReportStatus status, string tenantId)
        {
            return await _dbSet
                .Where(sr => sr.Status == status && sr.TenantId == tenantId)
                .OrderByDescending(sr => sr.Year)
                .ThenByDescending(sr => sr.Month)
                .ToListAsync();
        }

        public async Task<IEnumerable<SNGPCReport>> GetOverdueReportsAsync(string tenantId)
        {
            var now = DateTime.UtcNow;
            
            return await _dbSet
                .Where(sr => sr.TenantId == tenantId &&
                            sr.Status != SNGPCReportStatus.Transmitted &&
                            sr.Status != SNGPCReportStatus.Validated)
                .ToListAsync()
                .ContinueWith(task => task.Result.Where(sr => sr.IsOverdue()).ToList());
        }

        public async Task<IEnumerable<SNGPCReport>> GetFailedReportsForRetryAsync(string tenantId)
        {
            return await _dbSet
                .Where(sr => sr.Status == SNGPCReportStatus.TransmissionFailed && 
                            sr.TenantId == tenantId)
                .ToListAsync()
                .ContinueWith(task => task.Result.Where(sr => sr.CanRetryTransmission()).ToList());
        }

        public async Task<SNGPCReport?> GetMostRecentReportAsync(string tenantId)
        {
            return await _dbSet
                .Where(sr => sr.TenantId == tenantId)
                .OrderByDescending(sr => sr.Year)
                .ThenByDescending(sr => sr.Month)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ReportExistsAsync(int month, int year, string tenantId)
        {
            return await _dbSet
                .AnyAsync(sr => sr.Month == month && sr.Year == year && sr.TenantId == tenantId);
        }

        public async Task<IEnumerable<SNGPCReport>> GetTransmissionHistoryAsync(int count, string tenantId)
        {
            return await _dbSet
                .Where(sr => sr.TenantId == tenantId)
                .OrderByDescending(sr => sr.Year)
                .ThenByDescending(sr => sr.Month)
                .Take(count)
                .ToListAsync();
        }
    }
}
