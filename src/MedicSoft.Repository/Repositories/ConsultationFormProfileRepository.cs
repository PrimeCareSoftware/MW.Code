using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class ConsultationFormProfileRepository : BaseRepository<ConsultationFormProfile>, IConsultationFormProfileRepository
    {
        public ConsultationFormProfileRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<ConsultationFormProfile>> GetBySpecialtyAsync(ProfessionalSpecialty specialty, string tenantId)
        {
            return await _dbSet
                .Where(p => p.Specialty == specialty && p.TenantId == tenantId)
                .OrderBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<ConsultationFormProfile>> GetActiveProfilesAsync(string tenantId)
        {
            return await _dbSet
                .Where(p => p.IsActive && p.TenantId == tenantId)
                .OrderBy(p => p.Specialty)
                .ThenBy(p => p.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<ConsultationFormProfile>> GetSystemDefaultProfilesAsync(string tenantId)
        {
            return await _dbSet
                .Where(p => p.IsSystemDefault && p.TenantId == tenantId)
                .OrderBy(p => p.Specialty)
                .ToListAsync();
        }
    }
}
