using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.Enums;
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

        public async Task<Clinic?> GetByDocumentAsync(string document)
        {
            // Remove formatting from document
            var cleanDocument = new string(document.Where(char.IsDigit).ToArray());
            
            return await _dbSet
                .FirstOrDefaultAsync(c => c.Document.Replace(".", "").Replace("/", "").Replace("-", "") == cleanDocument);
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
            string? clinicType,
            int pageNumber,
            int pageSize)
        {
            var query = ApplyPublicClinicsFilters(name, city, state, clinicType);

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
            string? state,
            string? clinicType)
        {
            var query = ApplyPublicClinicsFilters(name, city, state, clinicType);
            return await query.CountAsync();
        }

        private IQueryable<Clinic> ApplyPublicClinicsFilters(
            string? name,
            string? city,
            string? state,
            string? clinicType)
        {
            // Only show clinics that are active AND have explicitly enabled public display
            var query = _dbSet.Where(c => c.IsActive && c.ShowOnPublicSite);

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

            if (!string.IsNullOrWhiteSpace(clinicType))
            {
                // Try to parse the clinic type enum
                if (Enum.TryParse<ClinicType>(clinicType, true, out var type))
                {
                    query = query.Where(c => c.ClinicType == type);
                }
                // Note: Invalid clinic types are ignored to allow flexible filtering
                // The frontend should validate clinic types before sending requests
            }

            return query;
        }

        public async Task<List<Clinic>> GetAllActiveAsync()
        {
            return await _dbSet
                .Where(c => c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<ClinicCustomization?> GetCustomizationAsync(Guid clinicId)
        {
            // ClinicCustomization is in a different repository, but we need to implement this for the cached repository
            // This should actually use IClinicCustomizationRepository, but for now we'll return null
            // The proper implementation would require injecting IClinicCustomizationRepository
            return null;
        }
    }
}
