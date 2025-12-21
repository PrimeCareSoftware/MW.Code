using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;
using MedicSoft.Api.Controllers;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Test.Api
{
    public class NotificationsControllerTests
    {
        private readonly Mock<IInAppNotificationService> _mockNotificationService;
        private readonly Mock<ITenantContext> _mockTenantContext;
        private readonly NotificationsController _controller;

        public NotificationsControllerTests()
        {
            _mockNotificationService = new Mock<IInAppNotificationService>();
            _mockTenantContext = new Mock<ITenantContext>();
            
            _controller = new NotificationsController(_mockNotificationService.Object, _mockTenantContext.Object);
            
            // Setup HTTP context with tenant header
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["X-Tenant-Id"] = "test-tenant";
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task GetAll_ShouldReturnNotifications()
        {
            // Arrange
            var expectedNotifications = new List<NotificationDto>
            {
                new NotificationDto
                {
                    Id = "1",
                    Type = "AppointmentCompleted",
                    Title = "Test Notification",
                    Message = "Test Message",
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                }
            };

            _mockNotificationService
                .Setup(x => x.GetNotificationsAsync("test-tenant", false))
                .ReturnsAsync(expectedNotifications);

            // Act
            var result = await _controller.GetAll(false);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var notifications = Assert.IsAssignableFrom<IEnumerable<NotificationDto>>(okResult.Value);
            Assert.Single(notifications);
        }

        [Fact]
        public async Task NotifyAppointmentCompleted_ShouldReturnOk()
        {
            // Arrange
            var dto = new AppointmentCompletedNotificationDto
            {
                AppointmentId = "123",
                DoctorName = "Dr. Test",
                PatientName = "Patient Test",
                CompletedAt = DateTime.UtcNow
            };

            var expectedNotification = new NotificationDto
            {
                Id = "1",
                Type = "AppointmentCompleted",
                Title = "Consulta Finalizada",
                Message = "Dr(a). Dr. Test finalizou o atendimento de Patient Test",
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            _mockNotificationService
                .Setup(x => x.NotifyAppointmentCompletedAsync(dto, "test-tenant"))
                .ReturnsAsync(expectedNotification);

            // Act
            var result = await _controller.NotifyAppointmentCompleted(dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var notification = Assert.IsType<NotificationDto>(okResult.Value);
            Assert.Equal("AppointmentCompleted", notification.Type);
        }

        [Fact]
        public async Task MarkAsRead_WhenNotificationExists_ShouldReturnNoContent()
        {
            // Arrange
            _mockNotificationService
                .Setup(x => x.MarkAsReadAsync("1", "test-tenant"))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.MarkAsRead("1");

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task MarkAsRead_WhenNotificationNotFound_ShouldReturnNotFound()
        {
            // Arrange
            _mockNotificationService
                .Setup(x => x.MarkAsReadAsync("999", "test-tenant"))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.MarkAsRead("999");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public async Task MarkAllAsRead_ShouldReturnNoContent()
        {
            // Arrange
            _mockNotificationService
                .Setup(x => x.MarkAllAsReadAsync("test-tenant"))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.MarkAllAsRead();

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_WhenNotificationExists_ShouldReturnNoContent()
        {
            // Arrange
            _mockNotificationService
                .Setup(x => x.DeleteNotificationAsync("1", "test-tenant"))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.Delete("1");

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task Delete_WhenNotificationNotFound_ShouldReturnNotFound()
        {
            // Arrange
            _mockNotificationService
                .Setup(x => x.DeleteNotificationAsync("999", "test-tenant"))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.Delete("999");

            // Assert
            Assert.IsType<NotFoundObjectResult>(result);
        }

        [Fact]
        public void Health_ShouldReturnOk()
        {
            // Act
            var result = _controller.Health();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(okResult.Value);
        }
    }
}
