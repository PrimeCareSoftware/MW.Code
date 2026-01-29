using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using Xunit;

namespace MedicSoft.Test.Services
{
    public class LoginAnomalyDetectionServiceTests
    {
        private readonly Mock<IUserSessionRepository> _mockSessionRepository;
        private readonly Mock<IAuditService> _mockAuditService;
        private readonly Mock<INotificationService> _mockNotificationService;
        private readonly ILoginAnomalyDetectionService _service;
        private const string TestUserId = "550e8400-e29b-41d4-a716-446655440000";
        private const string TestTenantId = "tenant123";

        public LoginAnomalyDetectionServiceTests()
        {
            _mockSessionRepository = new Mock<IUserSessionRepository>();
            _mockAuditService = new Mock<IAuditService>();
            _mockNotificationService = new Mock<INotificationService>();
            
            _service = new LoginAnomalyDetectionService(
                _mockSessionRepository.Object,
                _mockAuditService.Object,
                _mockNotificationService.Object
            );
        }

        [Fact]
        public async Task IsLoginSuspicious_FirstLogin_ReturnsNotSuspicious()
        {
            // Arrange
            var attempt = CreateLoginAttempt("192.168.1.1", "Mozilla/5.0", "BR");
            _mockSessionRepository.Setup(r => r.GetRecentSessionsByUserIdAsync(
                It.IsAny<Guid>(), TestTenantId, 10))
                .ReturnsAsync(new List<UserSession>());

            // Act
            var result = await _service.IsLoginSuspicious(TestUserId, attempt, TestTenantId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsLoginSuspicious_NewIP_ReturnsNotSuspicious()
        {
            // Arrange (only 1 flag - not enough for suspicion)
            var attempt = CreateLoginAttempt("192.168.1.2", "Mozilla/5.0", "BR");
            var recentSessions = new List<UserSession>
            {
                CreateUserSession("192.168.1.1", "Mozilla/5.0", "BR")
            };
            
            _mockSessionRepository.Setup(r => r.GetRecentSessionsByUserIdAsync(
                It.IsAny<Guid>(), TestTenantId, 10))
                .ReturnsAsync(recentSessions);

            // Act
            var result = await _service.IsLoginSuspicious(TestUserId, attempt, TestTenantId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsLoginSuspicious_NewIPAndNewDevice_ReturnsSuspicious()
        {
            // Arrange (2 flags - suspicious!)
            var attempt = CreateLoginAttempt("192.168.1.2", "Safari/17.0", "BR");
            var recentSessions = new List<UserSession>
            {
                CreateUserSession("192.168.1.1", "Mozilla/5.0 Firefox", "BR")
            };
            
            _mockSessionRepository.Setup(r => r.GetRecentSessionsByUserIdAsync(
                It.IsAny<Guid>(), TestTenantId, 10))
                .ReturnsAsync(recentSessions);

            _mockAuditService.Setup(a => a.LogAsync(It.IsAny<Application.DTOs.CreateAuditLogDto>()))
                .Returns(Task.CompletedTask);

            _mockNotificationService.Setup(n => n.CreateNotificationAsync(It.IsAny<Application.DTOs.CreateNotificationDto>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.IsLoginSuspicious(TestUserId, attempt, TestTenantId);

            // Assert
            Assert.True(result);
            _mockAuditService.Verify(a => a.LogAsync(It.IsAny<Application.DTOs.CreateAuditLogDto>()), Times.Once);
            _mockNotificationService.Verify(n => n.CreateNotificationAsync(It.IsAny<Application.DTOs.CreateNotificationDto>()), Times.Once);
        }

        [Fact]
        public async Task IsLoginSuspicious_NewCountry_ReturnsNotSuspicious()
        {
            // Arrange (only 1 flag)
            var attempt = CreateLoginAttempt("192.168.1.1", "Mozilla/5.0", "US");
            var recentSessions = new List<UserSession>
            {
                CreateUserSession("192.168.1.1", "Mozilla/5.0", "BR")
            };
            
            _mockSessionRepository.Setup(r => r.GetRecentSessionsByUserIdAsync(
                It.IsAny<Guid>(), TestTenantId, 10))
                .ReturnsAsync(recentSessions);

            // Act
            var result = await _service.IsLoginSuspicious(TestUserId, attempt, TestTenantId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsLoginSuspicious_NewCountryAndNewDevice_ReturnsSuspicious()
        {
            // Arrange (2 flags)
            var attempt = CreateLoginAttempt("192.168.1.1", "Safari/17.0", "US");
            var recentSessions = new List<UserSession>
            {
                CreateUserSession("192.168.1.1", "Mozilla/5.0 Firefox", "BR")
            };
            
            _mockSessionRepository.Setup(r => r.GetRecentSessionsByUserIdAsync(
                It.IsAny<Guid>(), TestTenantId, 10))
                .ReturnsAsync(recentSessions);

            _mockAuditService.Setup(a => a.LogAsync(It.IsAny<Application.DTOs.CreateAuditLogDto>()))
                .Returns(Task.CompletedTask);

            _mockNotificationService.Setup(n => n.CreateNotificationAsync(It.IsAny<Application.DTOs.CreateNotificationDto>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.IsLoginSuspicious(TestUserId, attempt, TestTenantId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsLoginSuspicious_ImpossibleTravel_ReturnsSuspicious()
        {
            // Arrange (impossible travel flag + another flag)
            var attempt = CreateLoginAttempt("10.0.0.1", "Safari/17.0", "US");
            var recentSessions = new List<UserSession>
            {
                CreateUserSession("192.168.1.1", "Mozilla/5.0 Firefox", "BR", DateTime.UtcNow.AddMinutes(-30))
            };
            
            _mockSessionRepository.Setup(r => r.GetRecentSessionsByUserIdAsync(
                It.IsAny<Guid>(), TestTenantId, 10))
                .ReturnsAsync(recentSessions);

            _mockAuditService.Setup(a => a.LogAsync(It.IsAny<Application.DTOs.CreateAuditLogDto>()))
                .Returns(Task.CompletedTask);

            _mockNotificationService.Setup(n => n.CreateNotificationAsync(It.IsAny<Application.DTOs.CreateNotificationDto>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.IsLoginSuspicious(TestUserId, attempt, TestTenantId);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public async Task IsLoginSuspicious_InvalidUserId_ReturnsNotSuspicious()
        {
            // Arrange
            var attempt = CreateLoginAttempt("192.168.1.1", "Mozilla/5.0", "BR");

            // Act
            var result = await _service.IsLoginSuspicious("invalid-guid", attempt, TestTenantId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task IsLoginSuspicious_NullAttempt_ReturnsNotSuspicious()
        {
            // Act
            var result = await _service.IsLoginSuspicious(TestUserId, null!, TestTenantId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task RecordLoginAttempt_Success_CreatesSession()
        {
            // Arrange
            var attempt = CreateLoginAttempt("192.168.1.1", "Mozilla/5.0", "BR");
            
            _mockSessionRepository.Setup(r => r.CreateSessionAsync(It.IsAny<UserSession>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.RecordLoginAttempt(TestUserId, attempt, true, TestTenantId);

            // Assert
            _mockSessionRepository.Verify(r => r.CreateSessionAsync(It.IsAny<UserSession>()), Times.Once);
        }

        [Fact]
        public async Task RecordLoginAttempt_Failure_DoesNotCreateSession()
        {
            // Arrange
            var attempt = CreateLoginAttempt("192.168.1.1", "Mozilla/5.0", "BR");

            // Act
            await _service.RecordLoginAttempt(TestUserId, attempt, false, TestTenantId);

            // Assert
            _mockSessionRepository.Verify(r => r.CreateSessionAsync(It.IsAny<UserSession>()), Times.Never);
        }

        private LoginAttemptDto CreateLoginAttempt(string ip, string userAgent, string country)
        {
            return new LoginAttemptDto
            {
                IpAddress = ip,
                UserAgent = userAgent,
                Country = country
            };
        }

        private UserSession CreateUserSession(string ip, string userAgent, string country, DateTime? startedAt = null)
        {
            var userId = Guid.Parse(TestUserId);
            var session = new UserSession(userId, TestTenantId)
            {
                IpAddress = ip,
                UserAgent = userAgent,
                Country = country
            };
            
            // Use reflection to set StartedAt if provided
            if (startedAt.HasValue)
            {
                var property = typeof(UserSession).GetProperty("StartedAt");
                property?.SetValue(session, startedAt.Value);
            }
            
            return session;
        }
    }
}
