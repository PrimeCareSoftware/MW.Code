using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class WaitingQueueRepository : BaseRepository<WaitingQueueEntry>, IWaitingQueueRepository
    {
        public WaitingQueueRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<WaitingQueueEntry>> GetActiveEntriesByClinicAsync(Guid clinicId, string tenantId)
        {
            return await _dbSet
                .Where(wqe => wqe.ClinicId == clinicId && 
                              wqe.TenantId == tenantId &&
                              wqe.Status != QueueStatus.Completed &&
                              wqe.Status != QueueStatus.Cancelled)
                .Include(wqe => wqe.Patient)
                .OrderBy(wqe => wqe.Position)
                .ToListAsync();
        }

        public async Task<IEnumerable<WaitingQueueEntry>> GetByStatusAsync(Guid clinicId, QueueStatus status, string tenantId)
        {
            return await _dbSet
                .Where(wqe => wqe.ClinicId == clinicId && 
                              wqe.Status == status && 
                              wqe.TenantId == tenantId)
                .Include(wqe => wqe.Patient)
                .OrderBy(wqe => wqe.Position)
                .ToListAsync();
        }

        public async Task<WaitingQueueEntry?> GetByAppointmentIdAsync(Guid appointmentId, string tenantId)
        {
            return await _dbSet
                .Include(wqe => wqe.Patient)
                .FirstOrDefaultAsync(wqe => wqe.AppointmentId == appointmentId && wqe.TenantId == tenantId);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
