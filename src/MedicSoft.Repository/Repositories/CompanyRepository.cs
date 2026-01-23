using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class CompanyRepository : BaseRepository<Company>, ICompanyRepository
    {
        public CompanyRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<Company?> GetByDocumentAsync(string document)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Document == document);
        }

        public async Task<Company?> GetBySubdomainAsync(string subdomain)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Subdomain == subdomain);
        }

        public async Task<bool> IsSubdomainUniqueAsync(string subdomain, Guid? excludeId = null)
        {
            var query = _dbSet.Where(c => c.Subdomain == subdomain);

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<bool> IsDocumentUniqueAsync(string document, Guid? excludeId = null)
        {
            var query = _dbSet.Where(c => c.Document == document);

            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Clinic>> GetCompayClinicsAsync(Guid companyId)
        {
            return await _context.Clinics
                .Where(c => c.CompanyId == companyId && c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }
    }
}
