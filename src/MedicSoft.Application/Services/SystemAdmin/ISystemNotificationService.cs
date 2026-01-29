using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs.SystemAdmin;

namespace MedicSoft.Application.Services.SystemAdmin
{
    /// <summary>
    /// Interface for system notification service
    /// </summary>
    public interface ISystemNotificationService
    {
        Task<SystemNotificationDto> CreateNotificationAsync(CreateSystemNotificationDto dto);
        Task<List<SystemNotificationDto>> GetUnreadNotificationsAsync();
        Task<List<SystemNotificationDto>> GetAllNotificationsAsync(int page, int pageSize);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync();
        Task<int> GetUnreadCountAsync();
        Task SendRealTimeNotificationAsync(int notificationId);
    }
}
