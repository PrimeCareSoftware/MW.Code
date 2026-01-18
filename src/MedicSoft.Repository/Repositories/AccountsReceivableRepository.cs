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
    public class AccountsReceivableRepository : BaseRepository<AccountsReceivable>, IAccountsReceivableRepository
    {
        public AccountsReceivableRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<AccountsReceivable>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            return await _dbSet
                .Where(r => r.PatientId == patientId && r.TenantId == tenantId)
                .Include(r => r.Payments)
                .OrderByDescending(r => r.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<AccountsReceivable>> GetByAppointmentIdAsync(Guid appointmentId, string tenantId)
        {
            return await _dbSet
                .Where(r => r.AppointmentId == appointmentId && r.TenantId == tenantId)
                .Include(r => r.Payments)
                .OrderByDescending(r => r.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<AccountsReceivable>> GetByStatusAsync(ReceivableStatus status, string tenantId)
        {
            return await _dbSet
                .Where(r => r.Status == status && r.TenantId == tenantId)
                .Include(r => r.Payments)
                .OrderBy(r => r.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<AccountsReceivable>> GetOverdueAsync(string tenantId)
        {
            var today = DateTime.Today;
            return await _dbSet
                .Where(r => r.TenantId == tenantId && 
                           r.DueDate < today && 
                           r.OutstandingAmount > 0 &&
                           r.Status != ReceivableStatus.Cancelled &&
                           r.Status != ReceivableStatus.Paid)
                .Include(r => r.Payments)
                .OrderBy(r => r.DueDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<AccountsReceivable>> GetByDueDateRangeAsync(DateTime startDate, DateTime endDate, string tenantId)
        {
            return await _dbSet
                .Where(r => r.TenantId == tenantId && 
                           r.DueDate >= startDate && 
                           r.DueDate <= endDate)
                .Include(r => r.Payments)
                .OrderBy(r => r.DueDate)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalOutstandingAsync(string tenantId)
        {
            return await _dbSet
                .Where(r => r.TenantId == tenantId && 
                           r.Status != ReceivableStatus.Cancelled &&
                           r.Status != ReceivableStatus.Paid)
                .SumAsync(r => r.OutstandingAmount);
        }

        public async Task<decimal> GetTotalOverdueAsync(string tenantId)
        {
            var today = DateTime.Today;
            return await _dbSet
                .Where(r => r.TenantId == tenantId && 
                           r.DueDate < today && 
                           r.OutstandingAmount > 0 &&
                           r.Status != ReceivableStatus.Cancelled &&
                           r.Status != ReceivableStatus.Paid)
                .SumAsync(r => r.OutstandingAmount);
        }

        public async Task<AccountsReceivable?> GetByDocumentNumberAsync(string documentNumber, string tenantId)
        {
            return await _dbSet
                .Where(r => r.DocumentNumber == documentNumber && r.TenantId == tenantId)
                .Include(r => r.Payments)
                .FirstOrDefaultAsync();
        }

        public override async Task<AccountsReceivable?> GetByIdAsync(Guid id, string tenantId)
        {
            return await _dbSet
                .Where(r => r.Id == id && r.TenantId == tenantId)
                .Include(r => r.Payments)
                .Include(r => r.Patient)
                .Include(r => r.Appointment)
                .Include(r => r.HealthInsuranceOperator)
                .FirstOrDefaultAsync();
        }
    }
}
