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
    public class FinancialClosureRepository : BaseRepository<FinancialClosure>, IFinancialClosureRepository
    {
        public FinancialClosureRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<FinancialClosure?> GetByAppointmentIdAsync(Guid appointmentId, string tenantId)
        {
            return await _dbSet
                .Where(c => c.AppointmentId == appointmentId && c.TenantId == tenantId)
                .Include(c => c.Items)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<FinancialClosure>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            return await _dbSet
                .Where(c => c.PatientId == patientId && c.TenantId == tenantId)
                .Include(c => c.Items)
                .OrderByDescending(c => c.ClosureDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<FinancialClosure>> GetByStatusAsync(FinancialClosureStatus status, string tenantId)
        {
            return await _dbSet
                .Where(c => c.Status == status && c.TenantId == tenantId)
                .Include(c => c.Items)
                .OrderBy(c => c.ClosureDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<FinancialClosure>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, string tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && 
                           c.ClosureDate >= startDate && 
                           c.ClosureDate <= endDate)
                .Include(c => c.Items)
                .OrderBy(c => c.ClosureDate)
                .ToListAsync();
        }

        public async Task<FinancialClosure?> GetByClosureNumberAsync(string closureNumber, string tenantId)
        {
            return await _dbSet
                .Where(c => c.ClosureNumber == closureNumber && c.TenantId == tenantId)
                .Include(c => c.Items)
                .FirstOrDefaultAsync();
        }

        public async Task<decimal> GetTotalOutstandingAsync(string tenantId)
        {
            return await _dbSet
                .Where(c => c.TenantId == tenantId && 
                           c.Status != FinancialClosureStatus.Closed &&
                           c.Status != FinancialClosureStatus.Cancelled)
                .SumAsync(c => c.OutstandingAmount);
        }

        public override async Task<FinancialClosure?> GetByIdAsync(Guid id, string tenantId)
        {
            return await _dbSet
                .Where(c => c.Id == id && c.TenantId == tenantId)
                .Include(c => c.Items)
                .Include(c => c.Patient)
                .Include(c => c.Appointment)
                .Include(c => c.HealthInsuranceOperator)
                .FirstOrDefaultAsync();
        }
    }
}
