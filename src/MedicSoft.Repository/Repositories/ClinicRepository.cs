using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class ClinicRepository : BaseRepository<Clinic>, IClinicRepository
    {
        public ClinicRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<Clinic?> GetByDocumentAsync(string document, string tenantId)
        {
            return await _dbSet
                .Where(c => c.Document == document && c.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsDocumentUniqueAsync(string document, string tenantId, Guid? excludeId = null)
        {
            var query = _dbSet.Where(c => c.Document == document && c.TenantId == tenantId);
            
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }
    }
}