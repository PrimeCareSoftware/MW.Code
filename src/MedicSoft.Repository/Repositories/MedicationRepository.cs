using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class MedicationRepository : BaseRepository<Medication>, IMedicationRepository
    {
        public MedicationRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<Medication?> GetByNameAsync(string name, string tenantId)
        {
            return await _dbSet
                .Where(m => m.Name == name && m.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Medication>> SearchByNameAsync(string searchTerm, string tenantId)
        {
            return await _dbSet
                .Where(m => m.Name.Contains(searchTerm) && m.TenantId == tenantId && m.IsActive)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Medication>> GetByCategoryAsync(MedicationCategory category, string tenantId)
        {
            return await _dbSet
                .Where(m => m.Category == category && m.TenantId == tenantId && m.IsActive)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Medication>> GetActiveAsync(string tenantId)
        {
            return await _dbSet
                .Where(m => m.IsActive && m.TenantId == tenantId)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<Medication>> GetByActiveIngredientAsync(string activeIngredient, string tenantId)
        {
            return await _dbSet
                .Where(m => m.ActiveIngredient != null && 
                           m.ActiveIngredient.Contains(activeIngredient) && 
                           m.TenantId == tenantId && 
                           m.IsActive)
                .OrderBy(m => m.Name)
                .ToListAsync();
        }

        public async Task<bool> IsNameUniqueAsync(string name, string tenantId, Guid? excludeId = null)
        {
            var query = _dbSet.Where(m => m.Name == name && m.TenantId == tenantId);
            
            if (excludeId.HasValue)
                query = query.Where(m => m.Id != excludeId.Value);
            
            return !await query.AnyAsync();
        }
    }
}
