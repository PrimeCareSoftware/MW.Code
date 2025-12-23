using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class ClinicCustomizationRepository : BaseRepository<ClinicCustomization>, IClinicCustomizationRepository
    {
        public ClinicCustomizationRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<ClinicCustomization?> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Where(cc => cc.ClinicId == clinicId && cc.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<ClinicCustomization?> GetBySubdomainAsync(string subdomain)
        {
            if (string.IsNullOrWhiteSpace(subdomain))
                return null;

            var normalizedSubdomain = subdomain.Trim().ToLowerInvariant();
            
            return await _dbSet
                .Include(cc => cc.Clinic)
                .Where(cc => cc.Clinic != null && cc.Clinic.Subdomain == normalizedSubdomain && cc.Clinic.IsActive)
                .FirstOrDefaultAsync();
        }
    }
}
