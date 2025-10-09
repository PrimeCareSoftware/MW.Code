using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class MedicalRecordTemplateRepository : BaseRepository<MedicalRecordTemplate>, IMedicalRecordTemplateRepository
    {
        public MedicalRecordTemplateRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<MedicalRecordTemplate>> GetByTenantAsync(string tenantId)
        {
            return await _dbSet
                .Where(t => t.TenantId == tenantId && t.IsActive)
                .OrderBy(t => t.Category)
                .ThenBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicalRecordTemplate>> GetActiveByCategoryAsync(string category, string tenantId)
        {
            return await _dbSet
                .Where(t => t.Category == category && t.TenantId == tenantId && t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<MedicalRecordTemplate>> SearchByNameAsync(string name, string tenantId)
        {
            return await _dbSet
                .Where(t => t.Name.Contains(name) && t.TenantId == tenantId && t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }
    }
}
