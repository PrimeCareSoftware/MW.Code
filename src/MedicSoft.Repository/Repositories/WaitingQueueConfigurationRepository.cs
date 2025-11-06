using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class WaitingQueueConfigurationRepository : BaseRepository<WaitingQueueConfiguration>, IWaitingQueueConfigurationRepository
    {
        public WaitingQueueConfigurationRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<WaitingQueueConfiguration?> GetByClinicIdAsync(Guid clinicId, string tenantId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(wqc => wqc.ClinicId == clinicId && wqc.TenantId == tenantId);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
