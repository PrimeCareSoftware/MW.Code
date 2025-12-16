using Microsoft.EntityFrameworkCore;
using MedicSoft.Appointments.Api.Data;
using MedicSoft.Appointments.Api.Models;
using System.Text.Json;

namespace MedicSoft.Appointments.Api.Services;

public interface INotificationService
{
    Task<NotificationDto> CreateNotificationAsync(string type, string title, string message, object? data, string tenantId, Guid? userId = null);
    Task<NotificationDto> NotifyAppointmentCompletedAsync(AppointmentCompletedNotificationDto dto, string tenantId);
    Task<IEnumerable<NotificationDto>> GetNotificationsAsync(string tenantId, Guid? userId = null, bool unreadOnly = false);
    Task<bool> MarkAsReadAsync(Guid notificationId, string tenantId);
    Task<bool> MarkAllAsReadAsync(string tenantId, Guid? userId = null);
    Task<bool> DeleteNotificationAsync(Guid notificationId, string tenantId);
}

public class NotificationService : INotificationService
{
    private readonly AppointmentsDbContext _context;
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(AppointmentsDbContext context, ILogger<NotificationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<NotificationDto> CreateNotificationAsync(
        string type, 
        string title, 
        string message, 
        object? data, 
        string tenantId,
        Guid? userId = null)
    {
        var notification = new NotificationEntity
        {
            Id = Guid.NewGuid(),
            Type = type,
            Title = title,
            Message = message,
            DataJson = data != null ? JsonSerializer.Serialize(data) : null,
            IsRead = false,
            TenantId = tenantId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Created notification: {NotificationId} of type {Type}", notification.Id, type);

        return MapToDto(notification);
    }

    public async Task<NotificationDto> NotifyAppointmentCompletedAsync(
        AppointmentCompletedNotificationDto dto, 
        string tenantId)
    {
        var message = $"Dr(a). {dto.DoctorName} finalizou o atendimento de {dto.PatientName}";
        
        if (!string.IsNullOrEmpty(dto.NextPatientName))
        {
            message += $". Próximo paciente: {dto.NextPatientName}";
        }

        return await CreateNotificationAsync(
            "AppointmentCompleted",
            "Consulta Finalizada",
            message,
            dto,
            tenantId
        );
    }

    public async Task<IEnumerable<NotificationDto>> GetNotificationsAsync(
        string tenantId, 
        Guid? userId = null, 
        bool unreadOnly = false)
    {
        var query = _context.Notifications
            .Where(n => n.TenantId == tenantId);

        if (userId.HasValue)
        {
            query = query.Where(n => n.UserId == null || n.UserId == userId.Value);
        }

        if (unreadOnly)
        {
            query = query.Where(n => !n.IsRead);
        }

        var notifications = await query
            .OrderByDescending(n => n.CreatedAt)
            .Take(50) // Limit to last 50 notifications
            .ToListAsync();

        return notifications.Select(MapToDto);
    }

    public async Task<bool> MarkAsReadAsync(Guid notificationId, string tenantId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.TenantId == tenantId);

        if (notification == null)
            return false;

        notification.IsRead = true;
        notification.ReadAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Marked notification as read: {NotificationId}", notificationId);
        return true;
    }

    public async Task<bool> MarkAllAsReadAsync(string tenantId, Guid? userId = null)
    {
        var query = _context.Notifications
            .Where(n => n.TenantId == tenantId && !n.IsRead);

        if (userId.HasValue)
        {
            query = query.Where(n => n.UserId == null || n.UserId == userId.Value);
        }

        var notifications = await query.ToListAsync();

        foreach (var notification in notifications)
        {
            notification.IsRead = true;
            notification.ReadAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Marked {Count} notifications as read for tenant {TenantId}", 
            notifications.Count, tenantId);
        return true;
    }

    public async Task<bool> DeleteNotificationAsync(Guid notificationId, string tenantId)
    {
        var notification = await _context.Notifications
            .FirstOrDefaultAsync(n => n.Id == notificationId && n.TenantId == tenantId);

        if (notification == null)
            return false;

        _context.Notifications.Remove(notification);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Deleted notification: {NotificationId}", notificationId);
        return true;
    }

    private static NotificationDto MapToDto(NotificationEntity entity)
    {
        object? data = null;
        if (!string.IsNullOrEmpty(entity.DataJson))
        {
            try
            {
                data = JsonSerializer.Deserialize<object>(entity.DataJson);
            }
            catch
            {
                // If deserialization fails, leave data as null
            }
        }

        return new NotificationDto
        {
            Id = entity.Id,
            Type = entity.Type,
            Title = entity.Title,
            Message = entity.Message,
            Data = data,
            IsRead = entity.IsRead,
            CreatedAt = entity.CreatedAt
        };
    }
}
