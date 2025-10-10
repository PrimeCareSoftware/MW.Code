using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    public class NotificationRoutineRepository : BaseRepository<NotificationRoutine>, INotificationRoutineRepository
    {
        public NotificationRoutineRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<NotificationRoutine>> GetByTenantAsync(string tenantId)
        {
            return await _dbSet
                .Where(r => r.TenantId == tenantId)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<NotificationRoutine>> GetActiveRoutinesByTenantAsync(string tenantId)
        {
            return await _dbSet
                .Where(r => r.TenantId == tenantId && r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<NotificationRoutine>> GetRoutinesDueForExecutionAsync()
        {
            var now = DateTime.UtcNow;
            return await _dbSet
                .Where(r => r.IsActive && 
                           (r.NextExecutionAt == null || r.NextExecutionAt <= now))
                .OrderBy(r => r.NextExecutionAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<NotificationRoutine>> GetRoutinesByScopeAsync(RoutineScope scope, string tenantId)
        {
            var query = _dbSet.Where(r => r.Scope == scope);

            // For clinic scope, filter by tenant
            if (scope == RoutineScope.Clinic)
            {
                query = query.Where(r => r.TenantId == tenantId);
            }

            return await query
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<NotificationRoutine>> GetRoutinesByTypeAsync(NotificationType type, string tenantId)
        {
            return await _dbSet
                .Where(r => r.Type == type && r.TenantId == tenantId && r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }

        public async Task<IEnumerable<NotificationRoutine>> GetRoutinesByChannelAsync(NotificationChannel channel, string tenantId)
        {
            return await _dbSet
                .Where(r => r.Channel == channel && r.TenantId == tenantId && r.IsActive)
                .OrderBy(r => r.Name)
                .ToListAsync();
        }
    }
}
