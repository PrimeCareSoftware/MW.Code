using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class ClinicPricingConfigurationRepository : BaseRepository<ClinicPricingConfiguration>, IClinicPricingConfigurationRepository
    {
        public ClinicPricingConfigurationRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<ClinicPricingConfiguration?> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _context.ClinicPricingConfigurations
                .FirstOrDefaultAsync(c => c.ClinicId == clinicId && c.TenantId == tenantId);
        }
    }
}
