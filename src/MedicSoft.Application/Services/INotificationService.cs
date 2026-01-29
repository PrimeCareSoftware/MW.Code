using System.Threading.Tasks;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for creating and managing security-related notifications
    /// </summary>
    public interface INotificationService
    {
        /// <summary>
        /// Creates a new notification for a user
        /// </summary>
        Task CreateAsync(CreateNotificationDto dto);
        
        /// <summary>
        /// Creates a notification for multiple users
        /// </summary>
        Task CreateBulkAsync(CreateNotificationDto dto, string[] userIds);
        
        /// <summary>
        /// Marks a notification as read
        /// </summary>
        Task MarkAsReadAsync(string notificationId, string userId);
        
        /// <summary>
        /// Gets unread notifications for a user
        /// </summary>
        Task<System.Collections.Generic.List<NotificationDto>> GetUnreadAsync(string userId, string tenantId);
    }
    
    /// <summary>
    /// Basic implementation of notification service using the InAppNotificationService
    /// </summary>
    public class NotificationService : INotificationService
    {
        private readonly IInAppNotificationService _inAppNotificationService;
        
        public NotificationService(IInAppNotificationService inAppNotificationService)
        {
            _inAppNotificationService = inAppNotificationService ?? throw new System.ArgumentNullException(nameof(inAppNotificationService));
        }
        
        public async Task CreateAsync(CreateNotificationDto dto)
        {
            await _inAppNotificationService.CreateNotificationAsync(
                dto.Type,
                dto.Title,
                dto.Message,
                new { ActionUrl = dto.ActionUrl, UserId = dto.UserId },
                dto.TenantId
            );
        }
        
        public async Task CreateBulkAsync(CreateNotificationDto dto, string[] userIds)
        {
            foreach (var userId in userIds)
            {
                var userDto = new CreateNotificationDto
                {
                    UserId = userId,
                    Type = dto.Type,
                    Title = dto.Title,
                    Message = dto.Message,
                    ActionUrl = dto.ActionUrl,
                    TenantId = dto.TenantId
                };
                await CreateAsync(userDto);
            }
        }
        
        public async Task MarkAsReadAsync(string notificationId, string userId)
        {
            // For now, delegate to InAppNotificationService
            // In future, this could be extended to support database-backed notifications
            await _inAppNotificationService.MarkAsReadAsync(notificationId, userId);
        }
        
        public async Task<System.Collections.Generic.List<NotificationDto>> GetUnreadAsync(string userId, string tenantId)
        {
            var notifications = await _inAppNotificationService.GetNotificationsAsync(tenantId, unreadOnly: true);
            return new System.Collections.Generic.List<NotificationDto>(notifications);
        }
    }
}
