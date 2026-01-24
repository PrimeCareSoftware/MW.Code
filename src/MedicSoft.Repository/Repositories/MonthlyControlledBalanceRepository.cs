using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class MonthlyControlledBalanceRepository : BaseRepository<MonthlyControlledBalance>, IMonthlyControlledBalanceRepository
    {
        public MonthlyControlledBalanceRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<MonthlyControlledBalance?> GetByMonthYearMedicationAsync(
            int year,
            int month,
            string medicationName,
            string tenantId)
        {
            return await _dbSet
                .Where(mcb => mcb.TenantId == tenantId &&
                             mcb.Year == year &&
                             mcb.Month == month &&
                             mcb.MedicationName == medicationName)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<MonthlyControlledBalance>> GetByMonthYearAsync(
            int year,
            int month,
            string tenantId)
        {
            return await _dbSet
                .Where(mcb => mcb.TenantId == tenantId &&
                             mcb.Year == year &&
                             mcb.Month == month)
                .OrderBy(mcb => mcb.MedicationName)
                .ToListAsync();
        }

        public async Task<IEnumerable<MonthlyControlledBalance>> GetOpenBalancesAsync(string tenantId)
        {
            return await _dbSet
                .Where(mcb => mcb.TenantId == tenantId &&
                             mcb.Status == BalanceStatus.Open)
                .OrderBy(mcb => mcb.Year)
                .ThenBy(mcb => mcb.Month)
                .ThenBy(mcb => mcb.MedicationName)
                .ToListAsync();
        }

        public async Task<IEnumerable<MonthlyControlledBalance>> GetBalancesWithDiscrepanciesAsync(string tenantId)
        {
            var balances = await _dbSet
                .Where(mcb => mcb.TenantId == tenantId &&
                             mcb.Discrepancy != null)
                .ToListAsync();

            // Filter for significant discrepancies (> 0.001)
            return balances.Where(mcb => mcb.HasDiscrepancy()).ToList();
        }

        public async Task<IEnumerable<MonthlyControlledBalance>> GetOverdueBalancesAsync(string tenantId)
        {
            var balances = await _dbSet
                .Where(mcb => mcb.TenantId == tenantId &&
                             mcb.Status == BalanceStatus.Open)
                .ToListAsync();

            // Filter for overdue balances (should be closed by 5th of following month)
            return balances.Where(mcb => mcb.IsOverdue()).ToList();
        }

        public async Task<IEnumerable<MonthlyControlledBalance>> GetBalanceHistoryAsync(
            string medicationName,
            int year,
            string tenantId)
        {
            return await _dbSet
                .Where(mcb => mcb.TenantId == tenantId &&
                             mcb.MedicationName == medicationName &&
                             mcb.Year == year)
                .OrderBy(mcb => mcb.Month)
                .ToListAsync();
        }

        public async Task<bool> BalanceExistsAsync(
            int year,
            int month,
            string medicationName,
            string tenantId)
        {
            return await _dbSet
                .AnyAsync(mcb => mcb.TenantId == tenantId &&
                                mcb.Year == year &&
                                mcb.Month == month &&
                                mcb.MedicationName == medicationName);
        }

        public async Task<MonthlyControlledBalance?> GetMostRecentClosedBalanceAsync(
            string medicationName,
            string tenantId)
        {
            return await _dbSet
                .Where(mcb => mcb.TenantId == tenantId &&
                             mcb.MedicationName == medicationName &&
                             mcb.Status == BalanceStatus.Closed)
                .OrderByDescending(mcb => mcb.Year)
                .ThenByDescending(mcb => mcb.Month)
                .FirstOrDefaultAsync();
        }
    }
}
