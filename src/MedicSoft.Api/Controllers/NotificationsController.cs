using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing in-app notifications
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class NotificationsController : BaseController
    {
        private readonly IInAppNotificationService _notificationService;

        public NotificationsController(
            IInAppNotificationService notificationService,
            ITenantContext tenantContext)
            : base(tenantContext)
        {
            _notificationService = notificationService;
        }

        /// <summary>
        /// Get all notifications for the current tenant
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NotificationDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<NotificationDto>>> GetAll([FromQuery] bool unreadOnly = false)
        {
            var tenantId = GetTenantId();
            var notifications = await _notificationService.GetNotificationsAsync(tenantId, unreadOnly);
            return Ok(notifications);
        }

        /// <summary>
        /// Notify that an appointment was completed
        /// </summary>
        [HttpPost("appointment-completed")]
        [ProducesResponseType(typeof(NotificationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<NotificationDto>> NotifyAppointmentCompleted(
            [FromBody] AppointmentCompletedNotificationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var tenantId = GetTenantId();
            var notification = await _notificationService.NotifyAppointmentCompletedAsync(dto, tenantId);
            return Ok(notification);
        }

        /// <summary>
        /// Mark a notification as read
        /// </summary>
        [HttpPut("{id}/read")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> MarkAsRead(string id)
        {
            var tenantId = GetTenantId();
            var result = await _notificationService.MarkAsReadAsync(id, tenantId);

            if (!result)
                return NotFound(new { message = $"Notification with ID {id} not found" });

            return NoContent();
        }

        /// <summary>
        /// Mark all notifications as read
        /// </summary>
        [HttpPut("read-all")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> MarkAllAsRead()
        {
            var tenantId = GetTenantId();
            await _notificationService.MarkAllAsReadAsync(tenantId);
            return NoContent();
        }

        /// <summary>
        /// Delete a notification
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(string id)
        {
            var tenantId = GetTenantId();
            var result = await _notificationService.DeleteNotificationAsync(id, tenantId);

            if (!result)
                return NotFound(new { message = $"Notification with ID {id} not found" });

            return NoContent();
        }

        /// <summary>
        /// Health check endpoint
        /// </summary>
        [HttpGet("health")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public IActionResult Health()
        {
            return Ok(new { status = "healthy", service = "Notifications" });
        }
    }
}
