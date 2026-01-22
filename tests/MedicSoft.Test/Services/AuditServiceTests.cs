using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using Xunit;

namespace MedicSoft.Test.Services
{
    public class AuditServiceTests
    {
        private readonly Mock<IAuditRepository> _mockAuditRepository;
        private readonly IAuditService _auditService;

        public AuditServiceTests()
        {
            _mockAuditRepository = new Mock<IAuditRepository>();
            _auditService = new AuditService(_mockAuditRepository.Object);
        }

        [Fact]
        public async Task LogAsync_ShouldCreateAuditLogSuccessfully()
        {
            // Arrange
            var dto = new CreateAuditLogDto(
                UserId: "user123",
                UserName: "John Doe",
                UserEmail: "john@example.com",
                Action: AuditAction.CREATE,
                ActionDescription: "Created new patient",
                EntityType: "Patient",
                EntityId: Guid.NewGuid().ToString(),
                EntityDisplayName: "Jane Smith",
                IpAddress: "192.168.1.1",
                UserAgent: "Mozilla/5.0",
                RequestPath: "/api/patients",
                HttpMethod: "POST",
                Result: OperationResult.SUCCESS,
                DataCategory: DataCategory.SENSITIVE,
                Purpose: LgpdPurpose.HEALTHCARE,
                Severity: AuditSeverity.INFO,
                TenantId: "tenant123"
            );

            AuditLog? capturedLog = null;
            _mockAuditRepository.Setup(r => r.AddAsync(It.IsAny<AuditLog>()))
                .Callback<AuditLog>(log => capturedLog = log)
                .Returns(Task.CompletedTask);

            // Act
            await _auditService.LogAsync(dto);

            // Assert
            _mockAuditRepository.Verify(r => r.AddAsync(It.IsAny<AuditLog>()), Times.Once);
            Assert.NotNull(capturedLog);
            Assert.Equal(dto.UserId, capturedLog.UserId);
            Assert.Equal(dto.Action, capturedLog.Action);
        }

        [Fact]
        public async Task LogAuthenticationAsync_Success_ShouldCreateSuccessLog()
        {
            // Arrange
            var userId = "user123";
            var userName = "John Doe";
            var userEmail = "john@example.com";
            var action = AuditAction.LOGIN;
            var ipAddress = "192.168.1.1";
            var userAgent = "Mozilla/5.0";
            var tenantId = "tenant123";

            AuditLog? capturedLog = null;
            _mockAuditRepository.Setup(r => r.AddAsync(It.IsAny<AuditLog>()))
                .Callback<AuditLog>(log => capturedLog = log)
                .Returns(Task.CompletedTask);

            // Act
            await _auditService.LogAuthenticationAsync(
                userId, userName, userEmail, action, true, ipAddress, userAgent, tenantId);

            // Assert
            _mockAuditRepository.Verify(r => r.AddAsync(It.IsAny<AuditLog>()), Times.Once);
            Assert.NotNull(capturedLog);
            Assert.Equal(OperationResult.SUCCESS, capturedLog.Result);
            Assert.Equal(AuditSeverity.INFO, capturedLog.Severity);
        }

        [Fact]
        public async Task LogAuthenticationAsync_Failure_ShouldCreateFailureLog()
        {
            // Arrange
            var userId = "user123";
            var userName = "John Doe";
            var userEmail = "john@example.com";
            var action = AuditAction.LOGIN_FAILED;
            var ipAddress = "192.168.1.1";
            var userAgent = "Mozilla/5.0";
            var tenantId = "tenant123";
            var reason = "Invalid password";

            AuditLog? capturedLog = null;
            _mockAuditRepository.Setup(r => r.AddAsync(It.IsAny<AuditLog>()))
                .Callback<AuditLog>(log => capturedLog = log)
                .Returns(Task.CompletedTask);

            // Act
            await _auditService.LogAuthenticationAsync(
                userId, userName, userEmail, action, false, ipAddress, userAgent, tenantId, reason);

            // Assert
            _mockAuditRepository.Verify(r => r.AddAsync(It.IsAny<AuditLog>()), Times.Once);
            Assert.NotNull(capturedLog);
            Assert.Equal(OperationResult.FAILED, capturedLog.Result);
            Assert.Equal(AuditSeverity.WARNING, capturedLog.Severity);
            Assert.Equal(reason, capturedLog.FailureReason);
        }

        [Fact]
        public async Task LogDataAccessAsync_ShouldCreateAccessLog()
        {
            // Arrange
            var userId = "user123";
            var userName = "John Doe";
            var userEmail = "john@example.com";
            var entityType = "Patient";
            var entityId = Guid.NewGuid().ToString();
            var entityDisplayName = "Jane Smith";
            var ipAddress = "192.168.1.1";
            var userAgent = "Mozilla/5.0";
            var requestPath = "/api/patients/123";
            var httpMethod = "GET";
            var tenantId = "tenant123";

            AuditLog? capturedLog = null;
            _mockAuditRepository.Setup(r => r.AddAsync(It.IsAny<AuditLog>()))
                .Callback<AuditLog>(log => capturedLog = log)
                .Returns(Task.CompletedTask);

            // Act
            await _auditService.LogDataAccessAsync(
                userId, userName, userEmail, entityType, entityId, entityDisplayName,
                ipAddress, userAgent, requestPath, httpMethod, tenantId);

            // Assert
            _mockAuditRepository.Verify(r => r.AddAsync(It.IsAny<AuditLog>()), Times.Once);
            Assert.NotNull(capturedLog);
            Assert.Equal(AuditAction.READ, capturedLog.Action);
            Assert.Equal(entityType, capturedLog.EntityType);
            Assert.Equal(entityId, capturedLog.EntityId);
        }

        [Fact]
        public async Task LogDataModificationAsync_ShouldCreateModificationLogWithChanges()
        {
            // Arrange
            var userId = "user123";
            var userName = "John Doe";
            var userEmail = "john@example.com";
            var entityType = "Patient";
            var entityId = Guid.NewGuid().ToString();
            var entityDisplayName = "Jane Smith";
            var oldValues = new { Name = "Jane Smith", Age = 30 };
            var newValues = new { Name = "Jane Doe", Age = 31 };
            var ipAddress = "192.168.1.1";
            var userAgent = "Mozilla/5.0";
            var requestPath = "/api/patients/123";
            var httpMethod = "PUT";
            var tenantId = "tenant123";

            AuditLog? capturedLog = null;
            _mockAuditRepository.Setup(r => r.AddAsync(It.IsAny<AuditLog>()))
                .Callback<AuditLog>(log => capturedLog = log)
                .Returns(Task.CompletedTask);

            // Act
            await _auditService.LogDataModificationAsync(
                userId, userName, userEmail, entityType, entityId, entityDisplayName,
                oldValues, newValues, ipAddress, userAgent, requestPath, httpMethod, tenantId);

            // Assert
            _mockAuditRepository.Verify(r => r.AddAsync(It.IsAny<AuditLog>()), Times.Once);
            Assert.NotNull(capturedLog);
            Assert.Equal(AuditAction.UPDATE, capturedLog.Action);
            Assert.NotNull(capturedLog.OldValues);
            Assert.NotNull(capturedLog.NewValues);
            Assert.NotNull(capturedLog.ChangedFields);
            Assert.Contains("Name", capturedLog.ChangedFields);
            Assert.Contains("Age", capturedLog.ChangedFields);
        }

        [Fact]
        public async Task GetUserActivityAsync_ShouldReturnUserLogs()
        {
            // Arrange
            var userId = "user123";
            var tenantId = "tenant123";
            var startDate = DateTime.UtcNow.AddDays(-7);
            var endDate = DateTime.UtcNow;

            var logs = new List<AuditLog>
            {
                CreateSampleAuditLog(userId, AuditAction.LOGIN),
                CreateSampleAuditLog(userId, AuditAction.READ)
            };

            _mockAuditRepository.Setup(r => r.GetByUserIdAsync(userId, tenantId, startDate, endDate))
                .ReturnsAsync(logs);

            // Act
            var result = await _auditService.GetUserActivityAsync(userId, startDate, endDate, tenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            _mockAuditRepository.Verify(r => r.GetByUserIdAsync(userId, tenantId, startDate, endDate), Times.Once);
        }

        [Fact]
        public async Task GenerateLgpdReportAsync_ShouldGenerateReportWithCorrectCounts()
        {
            // Arrange
            var userId = "user123";
            var tenantId = "tenant123";

            var logs = new List<AuditLog>
            {
                CreateSampleAuditLog(userId, AuditAction.READ),
                CreateSampleAuditLog(userId, AuditAction.READ),
                CreateSampleAuditLog(userId, AuditAction.UPDATE),
                CreateSampleAuditLog(userId, AuditAction.CREATE),
                CreateSampleAuditLog(userId, AuditAction.EXPORT),
                CreateSampleAuditLog(userId, AuditAction.DOWNLOAD)
            };

            _mockAuditRepository.Setup(r => r.GetByUserIdAsync(userId, tenantId, null, null))
                .ReturnsAsync(logs);

            // Act
            var report = await _auditService.GenerateLgpdReportAsync(userId, tenantId);

            // Assert
            Assert.NotNull(report);
            Assert.Equal(userId, report.UserId);
            Assert.Equal(2, report.TotalAccesses);  // 2 READ actions
            Assert.Equal(2, report.DataModifications);  // 1 UPDATE + 1 CREATE
            Assert.Equal(2, report.DataExports);  // 1 EXPORT + 1 DOWNLOAD
            Assert.NotEmpty(report.RecentActivity);
        }

        private AuditLog CreateSampleAuditLog(string userId, AuditAction action)
        {
            return new AuditLog(
                userId,
                "John Doe",
                "john@example.com",
                action,
                "Sample action",
                "Patient",
                Guid.NewGuid().ToString(),
                "192.168.1.1",
                "Mozilla/5.0",
                "/api/patients",
                "GET",
                OperationResult.SUCCESS,
                DataCategory.SENSITIVE,
                LgpdPurpose.HEALTHCARE,
                AuditSeverity.INFO,
                "tenant123"
            );
        }
    }
}
