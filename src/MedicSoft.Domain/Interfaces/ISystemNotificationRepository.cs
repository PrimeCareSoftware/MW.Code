using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Interfaces
{
    public interface ISystemNotificationRepository : IRepository<SystemNotification>
    {
        Task<IEnumerable<SystemNotification>> GetUnreadNotificationsAsync();
        Task<IEnumerable<SystemNotification>> GetAllNotificationsAsync(int skip, int take);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync();
        Task<int> GetUnreadCountAsync();
    }

    public interface INotificationRuleRepository : IRepository<NotificationRule>
    {
        Task<IEnumerable<NotificationRule>> GetEnabledRulesAsync();
        Task<NotificationRule?> GetRuleByTriggerAsync(string trigger);
    }
}
