using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using MedicSoft.Api.Hubs;
using MedicSoft.Application.DTOs.SystemAdmin;
using MedicSoft.Application.Services.SystemAdmin;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Services.SystemAdmin
{
    public class SystemNotificationService : ISystemNotificationService
    {
        private readonly ISystemNotificationRepository _repository;
        private readonly IHubContext<SystemNotificationHub> _hubContext;

        public SystemNotificationService(
            ISystemNotificationRepository repository,
            IHubContext<SystemNotificationHub> hubContext)
        {
            _repository = repository;
            _hubContext = hubContext;
        }

        public async Task<SystemNotificationDto> CreateNotificationAsync(CreateSystemNotificationDto dto)
        {
            var notification = new SystemNotification(dto.Type, dto.Category, dto.Title, dto.Message)
            {
                ActionUrl = dto.ActionUrl,
                ActionLabel = dto.ActionLabel,
                Data = dto.Data
            };

            await _repository.AddAsync(notification);

            // Send real-time notification via SignalR
            await SendRealTimeNotificationAsync(notification.Id);

            return MapToDto(notification);
        }

        public async Task<List<SystemNotificationDto>> GetUnreadNotificationsAsync()
        {
            var notifications = await _repository.GetUnreadNotificationsAsync();
            return notifications.Select(MapToDto).ToList();
        }

        public async Task<List<SystemNotificationDto>> GetAllNotificationsAsync(int page, int pageSize)
        {
            var skip = (page - 1) * pageSize;
            var notifications = await _repository.GetAllNotificationsAsync(skip, pageSize);
            return notifications.Select(MapToDto).ToList();
        }

        public async Task MarkAsReadAsync(Guid notificationId)
        {
            await _repository.MarkAsReadAsync(notificationId);
        }

        public async Task MarkAllAsReadAsync()
        {
            await _repository.MarkAllAsReadAsync();
        }

        public async Task<int> GetUnreadCountAsync()
        {
            return await _repository.GetUnreadCountAsync();
        }

        public async Task SendRealTimeNotificationAsync(Guid notificationId)
        {
            var notification = await _repository.GetByIdAsync(notificationId, string.Empty);
            if (notification != null)
            {
                var dto = MapToDto(notification);
                await _hubContext.Clients.All.SendAsync("ReceiveNotification", dto);
            }
        }

        private SystemNotificationDto MapToDto(SystemNotification notification)
        {
            return new SystemNotificationDto
            {
                Id = notification.Id,
                Type = notification.Type,
                Category = notification.Category,
                Title = notification.Title,
                Message = notification.Message,
                ActionUrl = notification.ActionUrl,
                ActionLabel = notification.ActionLabel,
                IsRead = notification.IsRead,
                CreatedAt = notification.CreatedAt,
                ReadAt = notification.ReadAt,
                Data = notification.Data
            };
        }
    }
}
