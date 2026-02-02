using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class BusinessConfigurationRepository : BaseRepository<BusinessConfiguration>, IBusinessConfigurationRepository
    {
        public BusinessConfigurationRepository(MedicSoftDbContext context) : base(context)
        {
        }
        
        public async Task<BusinessConfiguration?> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Where(x => x.TenantId == tenantId && x.ClinicId == clinicId)
                .Include(x => x.Clinic)
                .FirstOrDefaultAsync();
        }
        
        public async Task<bool> ExistsByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _dbSet
                .AnyAsync(x => x.TenantId == tenantId && x.ClinicId == clinicId);
        }
    }
}
