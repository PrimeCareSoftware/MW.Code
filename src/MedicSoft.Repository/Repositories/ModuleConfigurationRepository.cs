using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class ModuleConfigurationRepository : BaseRepository<ModuleConfiguration>, IModuleConfigurationRepository
    {
        public ModuleConfigurationRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<ModuleConfiguration?> GetByClinicAndModuleAsync(Guid clinicId, string moduleName, string tenantId)
        {
            return await _dbSet
                .Where(mc => mc.ClinicId == clinicId && mc.ModuleName == moduleName && mc.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ModuleConfiguration>> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Where(mc => mc.ClinicId == clinicId && mc.TenantId == tenantId)
                .ToListAsync();
        }
    }
}
