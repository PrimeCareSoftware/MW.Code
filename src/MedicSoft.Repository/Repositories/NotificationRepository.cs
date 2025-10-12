using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class NotificationRepository : BaseRepository<Notification>, INotificationRepository
    {
        public NotificationRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Notification>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            return await _dbSet
                .Where(n => n.PatientId == patientId && n.TenantId == tenantId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetByAppointmentIdAsync(Guid appointmentId, string tenantId)
        {
            return await _dbSet
                .Where(n => n.AppointmentId == appointmentId && n.TenantId == tenantId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetPendingNotificationsAsync(string tenantId)
        {
            return await _dbSet
                .Where(n => n.Status == NotificationStatus.Pending && n.TenantId == tenantId)
                .OrderBy(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetFailedNotificationsForRetryAsync(string tenantId)
        {
            return await _dbSet
                .Where(n => n.Status == NotificationStatus.Failed && 
                           n.RetryCount < 3 && 
                           n.TenantId == tenantId)
                .OrderBy(n => n.CreatedAt)
                .ToListAsync();
        }
    }
}
