using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class CashFlowEntryRepository : BaseRepository<CashFlowEntry>, ICashFlowEntryRepository
    {
        public CashFlowEntryRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<CashFlowEntry>> GetByTypeAsync(CashFlowType type, string tenantId)
        {
            return await _dbSet
                .Where(e => e.Type == type && e.TenantId == tenantId)
                .OrderByDescending(e => e.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<CashFlowEntry>> GetByCategoryAsync(CashFlowCategory category, string tenantId)
        {
            return await _dbSet
                .Where(e => e.Category == category && e.TenantId == tenantId)
                .OrderByDescending(e => e.TransactionDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<CashFlowEntry>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, string tenantId)
        {
            return await _dbSet
                .Where(e => e.TenantId == tenantId && 
                           e.TransactionDate >= startDate && 
                           e.TransactionDate <= endDate)
                .OrderBy(e => e.TransactionDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalIncomeAsync(DateTime startDate, DateTime endDate, string tenantId)
        {
            return await _dbSet
                .Where(e => e.TenantId == tenantId && 
                           e.Type == CashFlowType.Income &&
                           e.TransactionDate >= startDate && 
                           e.TransactionDate <= endDate)
                .SumAsync(e => e.Amount);
        }

        public async Task<decimal> GetTotalExpenseAsync(DateTime startDate, DateTime endDate, string tenantId)
        {
            return await _dbSet
                .Where(e => e.TenantId == tenantId && 
                           e.Type == CashFlowType.Expense &&
                           e.TransactionDate >= startDate && 
                           e.TransactionDate <= endDate)
                .SumAsync(e => e.Amount);
        }

        public async Task<decimal> GetBalanceAsync(DateTime startDate, DateTime endDate, string tenantId)
        {
            var income = await GetTotalIncomeAsync(startDate, endDate, tenantId);
            var expense = await GetTotalExpenseAsync(startDate, endDate, tenantId);
            return income - expense;
        }

        public async Task<IEnumerable<CashFlowEntry>> GetByAppointmentIdAsync(Guid appointmentId, string tenantId)
        {
            return await _dbSet
                .Where(e => e.AppointmentId == appointmentId && e.TenantId == tenantId)
                .OrderByDescending(e => e.TransactionDate)
                .ToListAsync();
        }

        public override async Task<CashFlowEntry?> GetByIdAsync(Guid id, string tenantId)
        {
            return await _dbSet
                .Where(e => e.Id == id && e.TenantId == tenantId)
                .Include(e => e.Payment)
                .Include(e => e.Receivable)
                .Include(e => e.Payable)
                .Include(e => e.Appointment)
                .FirstOrDefaultAsync();
        }
    }
}
