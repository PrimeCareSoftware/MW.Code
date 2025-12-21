using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service interface for in-app notifications
    /// </summary>
    public interface IInAppNotificationService
    {
        Task<NotificationDto> CreateNotificationAsync(string type, string title, string message, object? data, string tenantId);
        Task<NotificationDto> NotifyAppointmentCompletedAsync(AppointmentCompletedNotificationDto dto, string tenantId);
        Task<IEnumerable<NotificationDto>> GetNotificationsAsync(string tenantId, bool unreadOnly = false);
        Task<bool> MarkAsReadAsync(string notificationId, string tenantId);
        Task<bool> MarkAllAsReadAsync(string tenantId);
        Task<bool> DeleteNotificationAsync(string notificationId, string tenantId);
    }

    /// <summary>
    /// In-memory implementation of in-app notifications service.
    /// This is a simple implementation that stores notifications in memory.
    /// For production, consider implementing persistence to database.
    /// </summary>
    public class InAppNotificationService : IInAppNotificationService
    {
        // In-memory storage: Key is "tenantId:notificationId"
        private static readonly ConcurrentDictionary<string, InAppNotificationItem> _notifications = new();

        public Task<NotificationDto> CreateNotificationAsync(
            string type,
            string title,
            string message,
            object? data,
            string tenantId)
        {
            var id = Guid.NewGuid().ToString();
            var notification = new InAppNotificationItem
            {
                Id = id,
                Type = type,
                Title = title,
                Message = message,
                DataJson = data != null ? JsonSerializer.Serialize(data) : null,
                IsRead = false,
                TenantId = tenantId,
                CreatedAt = DateTime.UtcNow
            };

            var key = $"{tenantId}:{id}";
            _notifications.TryAdd(key, notification);

            // Limit notifications per tenant to 100 (remove oldest if exceeded)
            CleanupOldNotifications(tenantId);

            return Task.FromResult(MapToDto(notification));
        }

        public Task<NotificationDto> NotifyAppointmentCompletedAsync(
            AppointmentCompletedNotificationDto dto,
            string tenantId)
        {
            var message = $"Dr(a). {dto.DoctorName} finalizou o atendimento de {dto.PatientName}";

            if (!string.IsNullOrEmpty(dto.NextPatientName))
            {
                message += $". Próximo paciente: {dto.NextPatientName}";
            }

            return CreateNotificationAsync(
                "AppointmentCompleted",
                "Consulta Finalizada",
                message,
                dto,
                tenantId
            );
        }

        public Task<IEnumerable<NotificationDto>> GetNotificationsAsync(
            string tenantId,
            bool unreadOnly = false)
        {
            var notifications = _notifications.Values
                .Where(n => n.TenantId == tenantId)
                .OrderByDescending(n => n.CreatedAt)
                .AsEnumerable();

            if (unreadOnly)
            {
                notifications = notifications.Where(n => !n.IsRead);
            }

            var result = notifications
                .Take(50) // Limit to last 50 notifications
                .Select(MapToDto)
                .ToList();

            return Task.FromResult<IEnumerable<NotificationDto>>(result);
        }

        public Task<bool> MarkAsReadAsync(string notificationId, string tenantId)
        {
            var key = $"{tenantId}:{notificationId}";

            if (_notifications.TryGetValue(key, out var notification))
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
                return Task.FromResult(true);
            }

            return Task.FromResult(false);
        }

        public Task<bool> MarkAllAsReadAsync(string tenantId)
        {
            var notifications = _notifications.Values
                .Where(n => n.TenantId == tenantId && !n.IsRead)
                .ToList();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.ReadAt = DateTime.UtcNow;
            }

            return Task.FromResult(true);
        }

        public Task<bool> DeleteNotificationAsync(string notificationId, string tenantId)
        {
            var key = $"{tenantId}:{notificationId}";
            return Task.FromResult(_notifications.TryRemove(key, out _));
        }

        private void CleanupOldNotifications(string tenantId)
        {
            var tenantNotifications = _notifications.Values
                .Where(n => n.TenantId == tenantId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip(100)
                .ToList();

            foreach (var notification in tenantNotifications)
            {
                var key = $"{tenantId}:{notification.Id}";
                _notifications.TryRemove(key, out _);
            }
        }

        private static NotificationDto MapToDto(InAppNotificationItem item)
        {
            object? data = null;
            if (!string.IsNullOrEmpty(item.DataJson))
            {
                try
                {
                    data = JsonSerializer.Deserialize<JsonElement>(item.DataJson);
                }
                catch
                {
                    // If deserialization fails, leave data as null
                }
            }

            return new NotificationDto
            {
                Id = item.Id,
                Type = item.Type,
                Title = item.Title,
                Message = item.Message,
                Data = data,
                IsRead = item.IsRead,
                CreatedAt = item.CreatedAt
            };
        }

        private class InAppNotificationItem
        {
            public string Id { get; set; } = string.Empty;
            public string Type { get; set; } = string.Empty;
            public string Title { get; set; } = string.Empty;
            public string Message { get; set; } = string.Empty;
            public string? DataJson { get; set; }
            public bool IsRead { get; set; }
            public string TenantId { get; set; } = string.Empty;
            public DateTime CreatedAt { get; set; }
            public DateTime? ReadAt { get; set; }
        }
    }
}
