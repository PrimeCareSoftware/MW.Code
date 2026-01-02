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
    /// <summary>
    /// Repository implementation for TherapeuticPlan entity (CFM 1.821 compliance)
    /// </summary>
    public class TherapeuticPlanRepository : BaseRepository<TherapeuticPlan>, ITherapeuticPlanRepository
    {
        public TherapeuticPlanRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<TherapeuticPlan>> GetByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId)
        {
            return await _dbSet
                .Where(tp => tp.MedicalRecordId == medicalRecordId && tp.TenantId == tenantId)
                .OrderBy(tp => tp.CreatedAt)
                .ToListAsync();
        }

        public async Task<TherapeuticPlan?> GetLatestByMedicalRecordIdAsync(Guid medicalRecordId, string tenantId)
        {
            return await _dbSet
                .Where(tp => tp.MedicalRecordId == medicalRecordId && tp.TenantId == tenantId)
                .OrderByDescending(tp => tp.CreatedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<TherapeuticPlan>> GetByReturnDateRangeAsync(DateTime startDate, DateTime endDate, string tenantId)
        {
            return await _dbSet
                .Where(tp => tp.ReturnDate.HasValue 
                    && tp.ReturnDate.Value >= startDate 
                    && tp.ReturnDate.Value <= endDate 
                    && tp.TenantId == tenantId)
                .OrderBy(tp => tp.ReturnDate)
                .ToListAsync();
        }
    }
}
