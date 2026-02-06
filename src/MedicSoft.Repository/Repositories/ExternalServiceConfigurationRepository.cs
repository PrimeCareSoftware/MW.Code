using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class ExternalServiceConfigurationRepository : BaseRepository<ExternalServiceConfiguration>, IExternalServiceConfigurationRepository
    {
        public ExternalServiceConfigurationRepository(MedicSoftDbContext context) : base(context)
        {
        }
        
        public async Task<IEnumerable<ExternalServiceConfiguration>> GetAllByTenantAsync(string tenantId)
        {
            return await _dbSet
                .Where(x => x.TenantId == tenantId)
                .Include(x => x.Clinic)
                .OrderBy(x => x.ServiceType)
                .ThenBy(x => x.ServiceName)
                .ToListAsync();
        }
        
        public async Task<ExternalServiceConfiguration?> GetByServiceTypeAsync(
            ExternalServiceType serviceType, 
            string tenantId, 
            Guid? clinicId = null)
        {
            var query = _dbSet
                .Where(x => x.TenantId == tenantId && x.ServiceType == serviceType);
            
            if (clinicId.HasValue)
                query = query.Where(x => x.ClinicId == clinicId.Value);
            else
                query = query.Where(x => x.ClinicId == null);
            
            return await query
                .Include(x => x.Clinic)
                .FirstOrDefaultAsync();
        }
        
        public async Task<IEnumerable<ExternalServiceConfiguration>> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Where(x => x.TenantId == tenantId && x.ClinicId == clinicId)
                .Include(x => x.Clinic)
                .OrderBy(x => x.ServiceType)
                .ThenBy(x => x.ServiceName)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<ExternalServiceConfiguration>> GetActiveServicesAsync(string tenantId)
        {
            return await _dbSet
                .Where(x => x.TenantId == tenantId && x.IsActive)
                .Include(x => x.Clinic)
                .OrderBy(x => x.ServiceType)
                .ThenBy(x => x.ServiceName)
                .ToListAsync();
        }
        
        public async Task<bool> ExistsByServiceTypeAsync(
            ExternalServiceType serviceType, 
            string tenantId, 
            Guid? clinicId = null,
            Guid? excludeId = null)
        {
            var query = _dbSet
                .Where(x => x.TenantId == tenantId && x.ServiceType == serviceType);
            
            if (clinicId.HasValue)
                query = query.Where(x => x.ClinicId == clinicId.Value);
            else
                query = query.Where(x => x.ClinicId == null);
            
            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);
            
            return await query.AnyAsync();
        }
    }
}
