using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class ConsultationFormConfigurationRepository : BaseRepository<ConsultationFormConfiguration>, IConsultationFormConfigurationRepository
    {
        public ConsultationFormConfigurationRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<ConsultationFormConfiguration?> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Include(c => c.Profile)
                .Include(c => c.Clinic)
                .Where(c => c.ClinicId == clinicId && c.TenantId == tenantId)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<ConsultationFormConfiguration>> GetByProfileIdAsync(Guid profileId, string tenantId)
        {
            return await _dbSet
                .Include(c => c.Clinic)
                .Where(c => c.ProfileId == profileId && c.TenantId == tenantId)
                .ToListAsync();
        }

        public async Task<ConsultationFormConfiguration?> GetActiveConfigurationByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Include(c => c.Profile)
                .Include(c => c.Clinic)
                .Where(c => c.ClinicId == clinicId && c.IsActive && c.TenantId == tenantId)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefaultAsync();
        }
    }
}
