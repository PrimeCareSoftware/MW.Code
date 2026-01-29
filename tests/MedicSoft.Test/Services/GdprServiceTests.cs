using System;
using System.Threading.Tasks;
using Moq;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using Xunit;

namespace MedicSoft.Test.Services
{
    public class GdprServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IClinicRepository> _mockClinicRepository;
        private readonly Mock<IAuditService> _mockAuditService;
        private readonly Mock<IAuditRepository> _mockAuditRepository;
        private readonly IGdprService _service;
        private const string TestUserId = "550e8400-e29b-41d4-a716-446655440000";
        private const string TestTenantId = "tenant123";

        public GdprServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockClinicRepository = new Mock<IClinicRepository>();
            _mockAuditService = new Mock<IAuditService>();
            _mockAuditRepository = new Mock<IAuditRepository>();
            
            _service = new GdprService(
                _mockUserRepository.Object,
                _mockClinicRepository.Object,
                _mockAuditService.Object,
                _mockAuditRepository.Object
            );
        }

        [Fact]
        public async Task ExportUserDataAsync_ValidUser_ReturnsJsonData()
        {
            // Arrange
            var user = CreateTestUser();
            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), TestTenantId))
                .ReturnsAsync(user);
            
            _mockAuditRepository.Setup(r => r.GetByUserIdAsync(
                TestUserId, TestTenantId, null, null))
                .ReturnsAsync(new System.Collections.Generic.List<AuditLog>());
            
            _mockAuditService.Setup(a => a.LogAsync(It.IsAny<Application.DTOs.CreateAuditLogDto>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.ExportUserDataAsync(TestUserId, TestTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            _mockAuditService.Verify(a => a.LogAsync(It.IsAny<Application.DTOs.CreateAuditLogDto>()), Times.Once);
        }

        [Fact]
        public async Task ExportUserDataAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), TestTenantId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.ExportUserDataAsync(TestUserId, TestTenantId));
        }

        [Fact]
        public async Task AnonymizeUserDataAsync_ValidUser_AnonymizesData()
        {
            // Arrange
            var user = CreateTestUser();
            var requestedByUserId = "admin-user-id";
            
            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), TestTenantId))
                .ReturnsAsync(user);
            
            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);
            
            _mockAuditService.Setup(a => a.LogAsync(It.IsAny<Application.DTOs.CreateAuditLogDto>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.AnonymizeUserDataAsync(TestUserId, TestTenantId, requestedByUserId);

            // Assert
            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
            _mockAuditService.Verify(a => a.LogAsync(It.Is<Application.DTOs.CreateAuditLogDto>(
                dto => dto.Severity == Domain.Enums.AuditSeverity.CRITICAL)), Times.Once);
            
            // Verify that sensitive data was anonymized
            Assert.DoesNotContain("test@example.com", user.Email);
            Assert.DoesNotContain("Test User", user.Name);
        }

        [Fact]
        public async Task AnonymizeUserDataAsync_UserNotFound_ThrowsException()
        {
            // Arrange
            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), TestTenantId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(
                () => _service.AnonymizeUserDataAsync(TestUserId, TestTenantId, "admin-id"));
        }

        [Fact]
        public async Task ExportClinicDataAsync_ValidClinic_ReturnsJsonData()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var clinic = CreateTestClinic(clinicId);
            
            _mockClinicRepository.Setup(r => r.GetByIdAsync(clinicId, TestTenantId))
                .ReturnsAsync(clinic);
            
            _mockAuditRepository.Setup(r => r.GetByEntityAsync(
                "Clinic", clinicId.ToString(), TestTenantId, null, null))
                .ReturnsAsync(new System.Collections.Generic.List<AuditLog>());
            
            _mockAuditService.Setup(a => a.LogAsync(It.IsAny<Application.DTOs.CreateAuditLogDto>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.ExportClinicDataAsync(clinicId, TestTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            _mockAuditService.Verify(a => a.LogAsync(It.IsAny<Application.DTOs.CreateAuditLogDto>()), Times.Once);
        }

        [Fact]
        public async Task AnonymizeClinicAsync_ValidClinic_AnonymizesData()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var userId = "admin-user-id";
            var clinic = CreateTestClinic(clinicId);
            
            _mockClinicRepository.Setup(r => r.GetByIdAsync(clinicId, TestTenantId))
                .ReturnsAsync(clinic);
            
            _mockClinicRepository.Setup(r => r.UpdateAsync(It.IsAny<Clinic>()))
                .Returns(Task.CompletedTask);
            
            _mockAuditService.Setup(a => a.LogAsync(It.IsAny<Application.DTOs.CreateAuditLogDto>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.AnonymizeClinicAsync(clinicId, TestTenantId, userId);

            // Assert
            _mockClinicRepository.Verify(r => r.UpdateAsync(It.IsAny<Clinic>()), Times.Once);
            _mockAuditService.Verify(a => a.LogAsync(It.Is<Application.DTOs.CreateAuditLogDto>(
                dto => dto.Severity == Domain.Enums.AuditSeverity.CRITICAL)), Times.Once);
            
            // Verify that sensitive data was anonymized
            Assert.DoesNotContain("Test Clinic", clinic.Name);
            Assert.DoesNotContain("clinic@example.com", clinic.Email ?? "");
        }

        [Fact]
        public async Task GenerateLgpdReportAsync_ValidUser_ReturnsReport()
        {
            // Arrange
            var user = CreateTestUser();
            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), TestTenantId))
                .ReturnsAsync(user);
            
            var auditLogs = new System.Collections.Generic.List<AuditLog>
            {
                CreateAuditLog(Domain.Enums.AuditAction.READ),
                CreateAuditLog(Domain.Enums.AuditAction.UPDATE),
                CreateAuditLog(Domain.Enums.AuditAction.EXPORT)
            };
            
            _mockAuditRepository.Setup(r => r.GetByUserIdAsync(
                TestUserId, TestTenantId, null, null))
                .ReturnsAsync(auditLogs);

            // Act
            var report = await _service.GenerateLgpdReportAsync(TestUserId, TestTenantId);

            // Assert
            Assert.NotNull(report);
            Assert.Equal(TestUserId, report.UserId);
            Assert.True(report.TotalAccesses > 0);
        }

        [Fact]
        public async Task GetDataRetentionPolicyAsync_ReturnsPolicy()
        {
            // Act
            var policy = await _service.GetDataRetentionPolicyAsync(TestTenantId);

            // Assert
            Assert.NotNull(policy);
            Assert.True(policy.RetentionPeriodYears > 0);
        }

        [Fact]
        public async Task RequestDataDeletionAsync_CreatesRequest()
        {
            // Arrange
            var user = CreateTestUser();
            _mockUserRepository.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), TestTenantId))
                .ReturnsAsync(user);
            
            _mockAuditService.Setup(a => a.LogAsync(It.IsAny<Application.DTOs.CreateAuditLogDto>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.RequestDataDeletionAsync(TestUserId, TestTenantId);

            // Assert
            _mockAuditService.Verify(a => a.LogAsync(It.Is<Application.DTOs.CreateAuditLogDto>(
                dto => dto.Action == Domain.Enums.AuditAction.DELETE)), Times.Once);
        }

        private User CreateTestUser()
        {
            return new User
            {
                Id = Guid.Parse(TestUserId),
                Email = "test@example.com",
                Name = "Test User",
                TenantId = TestTenantId,
                Cpf = "12345678901",
                Phone = "+55 11 99999-9999"
            };
        }

        private Clinic CreateTestClinic(Guid clinicId)
        {
            return new Clinic
            {
                Id = clinicId,
                Name = "Test Clinic",
                Email = "clinic@example.com",
                TenantId = TestTenantId,
                Cnpj = "12345678000190",
                Phone = "+55 11 88888-8888"
            };
        }

        private AuditLog CreateAuditLog(Domain.Enums.AuditAction action)
        {
            return new AuditLog(
                TestUserId,
                "Test User",
                "test@example.com",
                action,
                "Test action",
                "Patient",
                Guid.NewGuid().ToString(),
                "192.168.1.1",
                "Mozilla/5.0",
                "/api/test",
                "GET",
                Domain.Enums.OperationResult.SUCCESS,
                Domain.Enums.DataCategory.PERSONAL,
                Domain.Enums.LgpdPurpose.HEALTHCARE,
                Domain.Enums.AuditSeverity.INFO,
                TestTenantId
            );
        }
    }
}
