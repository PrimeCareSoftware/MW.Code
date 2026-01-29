using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.SystemAdmin;
using MedicSoft.Application.Services.SystemAdmin;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers.SystemAdmin
{
    /// <summary>
    /// Controller for system admin notifications
    /// </summary>
    [ApiController]
    [Route("api/system-admin/notifications")]
    [Authorize(Roles = "SystemAdmin")]
    public class SystemNotificationsController : BaseController
    {
        private readonly ISystemNotificationService _notificationService;

        public SystemNotificationsController(
            ITenantContext tenantContext,
            ISystemNotificationService notificationService) : base(tenantContext)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Get unread notifications
        /// </summary>
        [HttpGet("unread")]
        public async Task<ActionResult<List<SystemNotificationDto>>> GetUnreadNotifications()
        {
            var notifications = await _notificationService.GetUnreadNotificationsAsync();
            return Ok(notifications);
        }

        /// <summary>
        /// Get all notifications with pagination
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<SystemNotificationDto>>> GetAllNotifications(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            if (page < 1)
            {
                return BadRequest(new { message = "page must be >= 1" });
            }
            
            if (pageSize < 1 || pageSize > 100)
            {
                return BadRequest(new { message = "pageSize must be between 1 and 100" });
            }
            
            var notifications = await _notificationService.GetAllNotificationsAsync(page, pageSize);
            return Ok(notifications);
        }

        /// <summary>
        /// Get unread count
        /// </summary>
        [HttpGet("unread/count")]
        public async Task<ActionResult<int>> GetUnreadCount()
        {
            var count = await _notificationService.GetUnreadCountAsync();
            return Ok(new { count });
        }

        /// <summary>
        /// Mark notification as read
        /// </summary>
        [HttpPost("{id}/read")]
        public async Task<ActionResult> MarkAsRead(Guid id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return Ok(new { message = "Notification marked as read" });
        }

        /// <summary>
        /// Mark all notifications as read
        /// </summary>
        [HttpPost("read-all")]
        public async Task<ActionResult> MarkAllAsRead()
        {
            await _notificationService.MarkAllAsReadAsync();
            return Ok(new { message = "All notifications marked as read" });
        }

        /// <summary>
        /// Create a new notification (for system use only - automated jobs)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SystemAdmin,BackgroundJob")]
        public async Task<ActionResult<SystemNotificationDto>> CreateNotification(
            [FromBody] CreateSystemNotificationDto dto)
        {
            var notification = await _notificationService.CreateNotificationAsync(dto);
            return CreatedAtAction(nameof(GetAllNotifications), new { id = notification.Id }, notification);
        }
    }
}
