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

        public async Task<Clinic?> GetByCNPJAsync(string cnpj)
        {
            // Remove formatting from CNPJ
            var cleanCnpj = new string(cnpj.Where(char.IsDigit).ToArray());
            
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Document.Replace(".", "").Replace("/", "").Replace("-", "") == cleanCnpj);
        }

        public async Task<Clinic?> GetBySubdomainAsync(string subdomain)
        {
            if (string.IsNullOrWhiteSpace(subdomain))
                return null;

            var normalizedSubdomain = subdomain.Trim().ToLowerInvariant();
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Subdomain == normalizedSubdomain && c.IsActive);
        }

        public async Task<bool> IsSubdomainUniqueAsync(string subdomain, Guid? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(subdomain))
                return true;

            var normalizedSubdomain = subdomain.Trim().ToLowerInvariant();
            var query = _dbSet.Where(c => c.Subdomain == normalizedSubdomain);
            
            if (excludeId.HasValue)
            {
                query = query.Where(c => c.Id != excludeId.Value);
            }

            return !await query.AnyAsync();
        }

        public async Task<IEnumerable<Clinic>> SearchPublicClinicsAsync(
            string? name,
            string? city,
            string? state,
            int pageNumber,
            int pageSize)
        {
            var query = _dbSet.Where(c => c.IsActive);

            // Filtros opcionais
            if (!string.IsNullOrWhiteSpace(name))
            {
                var searchTerm = name.ToLower();
                query = query.Where(c => 
                    c.Name.ToLower().Contains(searchTerm) || 
                    c.TradeName.ToLower().Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                var citySearch = city.ToLower();
                query = query.Where(c => c.Address.ToLower().Contains(citySearch));
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                var stateSearch = state.ToLower();
                query = query.Where(c => c.Address.ToLower().Contains(stateSearch));
            }

            // Aplica paginação
            return await query
                .OrderBy(c => c.Name)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> CountPublicClinicsAsync(
            string? name,
            string? city,
            string? state)
        {
            var query = _dbSet.Where(c => c.IsActive);

            // Aplica os mesmos filtros
            if (!string.IsNullOrWhiteSpace(name))
            {
                var searchTerm = name.ToLower();
                query = query.Where(c => 
                    c.Name.ToLower().Contains(searchTerm) || 
                    c.TradeName.ToLower().Contains(searchTerm));
            }

            if (!string.IsNullOrWhiteSpace(city))
            {
                var citySearch = city.ToLower();
                query = query.Where(c => c.Address.ToLower().Contains(citySearch));
            }

            if (!string.IsNullOrWhiteSpace(state))
            {
                var stateSearch = state.ToLower();
                query = query.Where(c => c.Address.ToLower().Contains(stateSearch));
            }

            return await query.CountAsync();
        }
    }
}
