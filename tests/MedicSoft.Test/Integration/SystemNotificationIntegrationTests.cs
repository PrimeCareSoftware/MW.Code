using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using MedicSoft.Api.Services.SystemAdmin;
using MedicSoft.Application.DTOs.SystemAdmin;
using MedicSoft.Domain.Entities;
using MedicSoft.Repository.Context;
using MedicSoft.Repository.Repositories;
using Microsoft.AspNetCore.SignalR;
using MedicSoft.Api.Hubs;

namespace MedicSoft.Test.Integration
{
    /// <summary>
    /// Integration tests for System Notification functionality
    /// </summary>
    public class SystemNotificationIntegrationTests : IDisposable
    {
        private readonly MedicSoftDbContext _context;
        private readonly SystemNotificationRepository _repository;
        private readonly SystemNotificationService _service;
        private readonly Mock<IHubContext<SystemNotificationHub>> _mockHubContext;
        private readonly Mock<ILogger<SystemNotificationService>> _mockLogger;

        public SystemNotificationIntegrationTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<MedicSoftDbContext>()
                .UseInMemoryDatabase(databaseName: $"SystemNotificationTestDb_{Guid.NewGuid()}")
                .Options;

            _context = new MedicSoftDbContext(options);
            _repository = new SystemNotificationRepository(_context);
            
            // Setup mocks
            _mockHubContext = new Mock<IHubContext<SystemNotificationHub>>();
            _mockLogger = new Mock<ILogger<SystemNotificationService>>();

            // Setup mock for SignalR clients
            var mockClients = new Mock<IHubClients>();
            var mockClientProxy = new Mock<IClientProxy>();
            mockClients.Setup(clients => clients.All).Returns(mockClientProxy.Object);
            _mockHubContext.Setup(hub => hub.Clients).Returns(mockClients.Object);

            _service = new SystemNotificationService(_repository, _mockHubContext.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateNotification_ShouldCreateAndReturn()
        {
            // Arrange
            var dto = new CreateSystemNotificationDto
            {
                Type = "info",
                Category = "system",
                Title = "Test Notification",
                Message = "This is a test notification",
                ActionUrl = "/test",
                ActionLabel = "View"
            };

            // Act
            var result = await _service.CreateNotificationAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(Guid.Empty, result.Id);
            Assert.Equal(dto.Type, result.Type);
            Assert.Equal(dto.Category, result.Category);
            Assert.Equal(dto.Title, result.Title);
            Assert.Equal(dto.Message, result.Message);
            Assert.False(result.IsRead);
            Assert.Null(result.ReadAt);
        }

        [Fact]
        public async Task GetUnreadNotifications_ShouldReturnOnlyUnread()
        {
            // Arrange
            await _service.CreateNotificationAsync(new CreateSystemNotificationDto
            {
                Type = "info",
                Category = "system",
                Title = "Unread 1",
                Message = "Unread message 1"
            });

            var readNotification = await _service.CreateNotificationAsync(new CreateSystemNotificationDto
            {
                Type = "warning",
                Category = "subscription",
                Title = "Read Notification",
                Message = "This will be marked as read"
            });

            await _service.MarkAsReadAsync(readNotification.Id);

            await _service.CreateNotificationAsync(new CreateSystemNotificationDto
            {
                Type = "success",
                Category = "customer",
                Title = "Unread 2",
                Message = "Unread message 2"
            });

            // Act
            var unreadNotifications = await _service.GetUnreadNotificationsAsync();

            // Assert
            Assert.Equal(2, unreadNotifications.Count);
            Assert.All(unreadNotifications, n => Assert.False(n.IsRead));
        }

        [Fact]
        public async Task GetUnreadCount_ShouldReturnCorrectCount()
        {
            // Arrange
            for (int i = 0; i < 5; i++)
            {
                await _service.CreateNotificationAsync(new CreateSystemNotificationDto
                {
                    Type = "info",
                    Category = "system",
                    Title = $"Test {i}",
                    Message = $"Message {i}"
                });
            }

            var toRead = await _service.GetUnreadNotificationsAsync();
            await _service.MarkAsReadAsync(toRead.First().Id);
            await _service.MarkAsReadAsync(toRead.Skip(1).First().Id);

            // Act
            var count = await _service.GetUnreadCountAsync();

            // Assert
            Assert.Equal(3, count);
        }

        [Fact]
        public async Task MarkAsRead_ShouldUpdateNotification()
        {
            // Arrange
            var created = await _service.CreateNotificationAsync(new CreateSystemNotificationDto
            {
                Type = "critical",
                Category = "system",
                Title = "Critical Alert",
                Message = "System requires attention"
            });

            Assert.False(created.IsRead);

            // Act
            await _service.MarkAsReadAsync(created.Id);

            // Assert
            var notifications = await _service.GetAllNotificationsAsync(1, 10);
            var updated = notifications.First(n => n.Id == created.Id);
            Assert.True(updated.IsRead);
            Assert.NotNull(updated.ReadAt);
        }

        [Fact]
        public async Task MarkAllAsRead_ShouldUpdateAllNotifications()
        {
            // Arrange
            for (int i = 0; i < 3; i++)
            {
                await _service.CreateNotificationAsync(new CreateSystemNotificationDto
                {
                    Type = "info",
                    Category = "system",
                    Title = $"Notification {i}",
                    Message = $"Message {i}"
                });
            }

            // Act
            await _service.MarkAllAsReadAsync();

            // Assert
            var unreadCount = await _service.GetUnreadCountAsync();
            Assert.Equal(0, unreadCount);
        }

        [Fact]
        public async Task GetAllNotifications_ShouldRespectPagination()
        {
            // Arrange
            for (int i = 0; i < 15; i++)
            {
                await _service.CreateNotificationAsync(new CreateSystemNotificationDto
                {
                    Type = "info",
                    Category = "system",
                    Title = $"Notification {i}",
                    Message = $"Message {i}"
                });
            }

            // Act
            var page1 = await _service.GetAllNotificationsAsync(1, 10);
            var page2 = await _service.GetAllNotificationsAsync(2, 10);

            // Assert
            Assert.Equal(10, page1.Count);
            Assert.Equal(5, page2.Count);
        }

        [Fact]
        public async Task CreateNotification_WithDifferentTypes_ShouldWork()
        {
            // Arrange & Act
            var critical = await _service.CreateNotificationAsync(new CreateSystemNotificationDto
            {
                Type = "critical",
                Category = "subscription",
                Title = "Critical Issue",
                Message = "Immediate action required"
            });

            var warning = await _service.CreateNotificationAsync(new CreateSystemNotificationDto
            {
                Type = "warning",
                Category = "customer",
                Title = "Warning",
                Message = "Please review"
            });

            var info = await _service.CreateNotificationAsync(new CreateSystemNotificationDto
            {
                Type = "info",
                Category = "system",
                Title = "Information",
                Message = "For your information"
            });

            var success = await _service.CreateNotificationAsync(new CreateSystemNotificationDto
            {
                Type = "success",
                Category = "ticket",
                Title = "Success",
                Message = "Operation completed"
            });

            // Assert
            Assert.Equal("critical", critical.Type);
            Assert.Equal("warning", warning.Type);
            Assert.Equal("info", info.Type);
            Assert.Equal("success", success.Type);
        }

        [Fact]
        public async Task CreateNotification_WithAdditionalData_ShouldStoreJson()
        {
            // Arrange
            var jsonData = "{\"userId\":\"123\",\"clinicId\":\"456\"}";

            // Act
            var result = await _service.CreateNotificationAsync(new CreateSystemNotificationDto
            {
                Type = "info",
                Category = "system",
                Title = "Test with Data",
                Message = "Has additional data",
                Data = jsonData
            });

            // Assert
            Assert.Equal(jsonData, result.Data);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
