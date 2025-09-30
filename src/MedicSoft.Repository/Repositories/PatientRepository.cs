using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class PatientRepository : BaseRepository<Patient>, IPatientRepository
    {
        public PatientRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<Patient?> GetByDocumentAsync(string document, string tenantId)
        {
            return await _dbSet
                .Where(p => p.Document == document && p.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<Patient?> GetByEmailAsync(string email, string tenantId)
        {
            return await _dbSet
                .Where(p => p.Email.Value == email && p.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Patient>> SearchByNameAsync(string name, string tenantId)
        {
            return await _dbSet
                .Where(p => p.Name.Contains(name) && p.TenantId == tenantId)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<bool> IsDocumentUniqueAsync(string document, string tenantId, Guid? excludeId = null)
        {
            var query = _dbSet.Where(p => p.Document == document && p.TenantId == tenantId);
            
            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email, string tenantId, Guid? excludeId = null)
        {
            var query = _dbSet.Where(p => p.Email.Value == email && p.TenantId == tenantId);
            
            if (excludeId.HasValue)
            {
                query = query.Where(p => p.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }
    }
}