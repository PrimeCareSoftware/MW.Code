using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class BusinessConfigurationRepository : Repository<BusinessConfiguration>, IBusinessConfigurationRepository
    {
        public BusinessConfigurationRepository(ApplicationDbContext context) : base(context)
        {
        }
        
        public async Task<BusinessConfiguration?> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await DbSet
                .Where(x => x.TenantId == tenantId && x.ClinicId == clinicId)
                .Include(x => x.Clinic)
                .FirstOrDefaultAsync();
        }
        
        public async Task<bool> ExistsByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await DbSet
                .AnyAsync(x => x.TenantId == tenantId && x.ClinicId == clinicId);
        }
    }
}
