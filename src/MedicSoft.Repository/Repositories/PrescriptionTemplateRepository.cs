using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class PrescriptionTemplateRepository : BaseRepository<PrescriptionTemplate>, IPrescriptionTemplateRepository
    {
        public PrescriptionTemplateRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<PrescriptionTemplate>> GetByTenantAsync(string tenantId)
        {
            return await _dbSet
                .Where(t => t.TenantId == tenantId && t.IsActive)
                .OrderBy(t => t.Category)
                .ThenBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<PrescriptionTemplate>> GetActiveByCategoryAsync(string category, string tenantId)
        {
            return await _dbSet
                .Where(t => t.Category == category && t.TenantId == tenantId && t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<PrescriptionTemplate>> SearchByNameAsync(string name, string tenantId)
        {
            return await _dbSet
                .Where(t => t.Name.Contains(name) && t.TenantId == tenantId && t.IsActive)
                .OrderBy(t => t.Name)
                .ToListAsync();
        }
    }
}
