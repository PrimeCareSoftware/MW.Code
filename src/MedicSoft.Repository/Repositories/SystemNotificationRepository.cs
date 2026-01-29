using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Repository.Repositories
{
    /// <summary>
    /// Repository for system-wide admin notifications.
    /// NOTE: SystemNotifications are intentionally global (not tenant-specific) and are meant 
    /// for system administrators to monitor all tenants. Queries do not filter by tenant.
    /// </summary>
    public class SystemNotificationRepository : BaseRepository<SystemNotification>, ISystemNotificationRepository
    {
        public SystemNotificationRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SystemNotification>> GetUnreadNotificationsAsync()
        {
            return await _dbSet
                .Where(n => !n.IsRead)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<SystemNotification>> GetAllNotificationsAsync(int skip, int take)
        {
            return await _dbSet
                .OrderByDescending(n => n.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            var notification = await _dbSet.FindAsync(notificationId);
            if (notification != null)
            {
                notification.MarkAsRead();
                await _context.SaveChangesAsync();
            }
        }

        public async Task MarkAllAsReadAsync()
        {
            var now = DateTime.UtcNow;
            await _context.Database.ExecuteSqlRawAsync(
                @"UPDATE ""SystemNotifications"" 
                  SET ""IsRead"" = true, ""ReadAt"" = {0}, ""UpdatedAt"" = {0}
                  WHERE ""IsRead"" = false", 
                now);
        }

        public async Task<int> GetUnreadCountAsync()
        {
            return await _dbSet.CountAsync(n => !n.IsRead);
        }
    }

    public class NotificationRuleRepository : BaseRepository<NotificationRule>, INotificationRuleRepository
    {
        public NotificationRuleRepository(MedicSoftDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<NotificationRule>> GetEnabledRulesAsync()
        {
            return await _dbSet
                .Where(r => r.IsEnabled)
                .ToListAsync();
        }

        public async Task<NotificationRule?> GetRuleByTriggerAsync(string trigger)
        {
            return await _dbSet
                .FirstOrDefaultAsync(r => r.Trigger == trigger && r.IsEnabled);
        }
    }
}
