using Microsoft.AspNetCore.Mvc;
using MedicSoft.Appointments.Api.Models;
using MedicSoft.Appointments.Api.Services;
using MedicSoft.Shared.Authentication;

namespace MedicSoft.Appointments.Api.Controllers;

[Route("api/[controller]")]
public class NotificationsController : MicroserviceBaseController
{
    private readonly INotificationService _notificationService;

    public NotificationsController(INotificationService notificationService)
    {
        _notificationService = notificationService;
    }

    /// <summary>
    /// Get all notifications for the tenant
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<NotificationDto>>> GetAll([FromQuery] bool unreadOnly = false)
    {
        try
        {
            var notifications = await _notificationService.GetNotificationsAsync(
                GetTenantId(), 
                unreadOnly: unreadOnly
            );
            return Ok(notifications);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Notify that an appointment was completed
    /// </summary>
    [HttpPost("appointment-completed")]
    public async Task<ActionResult> NotifyAppointmentCompleted(
        [FromBody] AppointmentCompletedNotificationDto dto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        try
        {
            var notification = await _notificationService.NotifyAppointmentCompletedAsync(
                dto, 
                GetTenantId()
            );
            return Ok(notification);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Mark a notification as read
    /// </summary>
    [HttpPut("{id}/read")]
    public async Task<ActionResult> MarkAsRead(Guid id)
    {
        try
        {
            var result = await _notificationService.MarkAsReadAsync(id, GetTenantId());
            
            if (!result)
                return NotFound($"Notification with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Mark all notifications as read
    /// </summary>
    [HttpPut("read-all")]
    public async Task<ActionResult> MarkAllAsRead()
    {
        try
        {
            await _notificationService.MarkAllAsReadAsync(GetTenantId());
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Delete a notification
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            var result = await _notificationService.DeleteNotificationAsync(id, GetTenantId());
            
            if (!result)
                return NotFound($"Notification with ID {id} not found");

            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "Notifications" });
    }
}
