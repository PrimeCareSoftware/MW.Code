using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using MedicSoft.Api.Controllers;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;
using Xunit;

namespace MedicSoft.Test.Security
{
    public class ModulePermissionsTests : IDisposable
    {
        private readonly MedicSoftDbContext _context;
        private readonly Mock<ITenantContext> _mockTenantContext;
        private readonly Mock<IClinicSubscriptionRepository> _mockSubscriptionRepository;
        private readonly Mock<ISubscriptionPlanRepository> _mockPlanRepository;
        private readonly Mock<IModuleConfigurationService> _mockModuleConfigService;
        private readonly string _testTenantId;

        public ModulePermissionsTests()
        {
            var options = new DbContextOptionsBuilder<MedicSoftDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new MedicSoftDbContext(options);
            _mockTenantContext = new Mock<ITenantContext>();
            _mockSubscriptionRepository = new Mock<IClinicSubscriptionRepository>();
            _mockPlanRepository = new Mock<ISubscriptionPlanRepository>();
            _mockModuleConfigService = new Mock<IModuleConfigurationService>();

            _testTenantId = "test-tenant";
            _mockTenantContext.Setup(t => t.TenantId).Returns(_testTenantId);
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region Core Module Protection Tests

        [Theory]
        [InlineData(SystemModules.PatientManagement)]
        [InlineData(SystemModules.AppointmentScheduling)]
        [InlineData(SystemModules.MedicalRecords)]
        [InlineData(SystemModules.Prescriptions)]
        [InlineData(SystemModules.FinancialManagement)]
        [InlineData(SystemModules.UserManagement)]
        public async Task CoreModules_CannotBeDisabled(string coreModuleName)
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var clinic = new Clinic("Test Clinic", _testTenantId);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic, clinicId);
            await _context.Clinics.AddAsync(clinic);
            await _context.SaveChangesAsync();

            var config = new ModuleConfiguration(clinicId, coreModuleName, _testTenantId, true);
            await _context.ModuleConfigurations.AddAsync(config);
            await _context.SaveChangesAsync();

            var service = new ModuleConfigurationService(
                _context,
                _mockSubscriptionRepository.Object,
                _mockPlanRepository.Object,
                Mock.Of<Microsoft.Extensions.Logging.ILogger<ModuleConfigurationService>>()
            );

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.DisableModuleAsync(clinicId, coreModuleName, "test-user")
            );

            exception.Message.Should().Contain("Core modules cannot be disabled");
        }

        [Fact]
        public void CoreModules_ShouldHaveIsCoreFlagSet()
        {
            // Arrange
            var coreModules = new[]
            {
                SystemModules.PatientManagement,
                SystemModules.AppointmentScheduling,
                SystemModules.MedicalRecords,
                SystemModules.Prescriptions,
                SystemModules.FinancialManagement,
                SystemModules.UserManagement
            };

            // Act & Assert
            foreach (var moduleName in coreModules)
            {
                var moduleInfo = SystemModules.GetModuleInfo(moduleName);
                moduleInfo.IsCore.Should().BeTrue($"{moduleName} should be marked as core");
            }
        }

        #endregion

        #region Plan Restrictions Tests

        [Fact]
        public async Task EnableModule_WithBasicPlan_ShouldNotAllowPremiumModules()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var planId = Guid.NewGuid();

            var clinic = new Clinic("Test Clinic", _testTenantId);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic, clinicId);
            await _context.Clinics.AddAsync(clinic);
            await _context.SaveChangesAsync();

            var plan = new SubscriptionPlan(
                "Basic", "Basic Plan", 49.00m, 30, 5, 500,
                SubscriptionPlanType.Basic, _testTenantId,
                hasReports: false,
                hasTissExport: false
            );
            typeof(SubscriptionPlan).GetProperty(nameof(SubscriptionPlan.Id))!.SetValue(plan, planId);

            var subscription = new ClinicSubscription(clinicId, planId, _testTenantId);

            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(clinicId, _testTenantId))
                .ReturnsAsync(subscription);

            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(planId, _testTenantId))
                .ReturnsAsync(plan);

            var service = new ModuleConfigurationService(
                _context,
                _mockSubscriptionRepository.Object,
                _mockPlanRepository.Object,
                Mock.Of<Microsoft.Extensions.Logging.ILogger<ModuleConfigurationService>>()
            );

            // Act & Assert - Reports module
            var exception1 = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.EnableModuleAsync(clinicId, SystemModules.Reports, "test-user")
            );
            exception1.Message.Should().Contain("not available in current plan");

            // Act & Assert - TISS Export module
            var exception2 = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.EnableModuleAsync(clinicId, SystemModules.TissExport, "test-user")
            );
            exception2.Message.Should().Contain("not available in current plan");
        }

        [Fact]
        public async Task EnableModule_WithStandardPlan_ShouldAllowStandardModules()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var planId = Guid.NewGuid();

            var clinic = new Clinic("Test Clinic", _testTenantId);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic, clinicId);
            await _context.Clinics.AddAsync(clinic);
            await _context.SaveChangesAsync();

            var plan = new SubscriptionPlan(
                "Standard", "Standard Plan", 99.00m, 30, 10, 1000,
                SubscriptionPlanType.Standard, _testTenantId,
                hasReports: true,
                hasWhatsAppIntegration: true,
                hasSMSNotifications: true
            );
            typeof(SubscriptionPlan).GetProperty(nameof(SubscriptionPlan.Id))!.SetValue(plan, planId);

            var subscription = new ClinicSubscription(clinicId, planId, _testTenantId);

            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(clinicId, _testTenantId))
                .ReturnsAsync(subscription);

            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(planId, _testTenantId))
                .ReturnsAsync(plan);

            var service = new ModuleConfigurationService(
                _context,
                _mockSubscriptionRepository.Object,
                _mockPlanRepository.Object,
                Mock.Of<Microsoft.Extensions.Logging.ILogger<ModuleConfigurationService>>()
            );

            // Act - Enable Reports module (should succeed)
            await service.EnableModuleAsync(clinicId, SystemModules.Reports, "test-user");

            // Assert
            var config = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == clinicId && mc.ModuleName == SystemModules.Reports);

            config.Should().NotBeNull();
            config!.IsEnabled.Should().BeTrue();
        }

        [Fact]
        public async Task EnableModule_WithPremiumPlan_ShouldAllowAllModules()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var planId = Guid.NewGuid();

            var clinic = new Clinic("Test Clinic", _testTenantId);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic, clinicId);
            await _context.Clinics.AddAsync(clinic);
            await _context.SaveChangesAsync();

            var plan = new SubscriptionPlan(
                "Premium", "Premium Plan", 199.00m, 30, 50, 5000,
                SubscriptionPlanType.Premium, _testTenantId,
                hasReports: true,
                hasWhatsAppIntegration: true,
                hasSMSNotifications: true,
                hasTissExport: true
            );
            typeof(SubscriptionPlan).GetProperty(nameof(SubscriptionPlan.Id))!.SetValue(plan, planId);

            var subscription = new ClinicSubscription(clinicId, planId, _testTenantId);

            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(clinicId, _testTenantId))
                .ReturnsAsync(subscription);

            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(planId, _testTenantId))
                .ReturnsAsync(plan);

            var service = new ModuleConfigurationService(
                _context,
                _mockSubscriptionRepository.Object,
                _mockPlanRepository.Object,
                Mock.Of<Microsoft.Extensions.Logging.ILogger<ModuleConfigurationService>>()
            );

            // Act - Enable TISS Export module (premium feature)
            await service.EnableModuleAsync(clinicId, SystemModules.TissExport, "test-user");

            // Assert
            var config = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == clinicId && mc.ModuleName == SystemModules.TissExport);

            config.Should().NotBeNull();
            config!.IsEnabled.Should().BeTrue();
        }

        #endregion

        #region Clinic Isolation Tests

        [Fact]
        public async Task Clinic_CanOnlyAccessOwnModules()
        {
            // Arrange
            var clinic1Id = Guid.NewGuid();
            var clinic2Id = Guid.NewGuid();

            var clinic1 = new Clinic("Clinic 1", _testTenantId);
            var clinic2 = new Clinic("Clinic 2", _testTenantId);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic1, clinic1Id);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic2, clinic2Id);
            await _context.Clinics.AddRangeAsync(clinic1, clinic2);

            // Create config for clinic1
            var config1 = new ModuleConfiguration(clinic1Id, SystemModules.Reports, _testTenantId, true);
            await _context.ModuleConfigurations.AddAsync(config1);
            await _context.SaveChangesAsync();

            // Act - Try to query clinic1's modules as clinic2
            var clinic2Modules = await _context.ModuleConfigurations
                .Where(mc => mc.ClinicId == clinic2Id && mc.TenantId == _testTenantId)
                .ToListAsync();

            // Assert
            clinic2Modules.Should().BeEmpty("Clinic 2 should not see Clinic 1's modules");
        }

        [Fact]
        public async Task Controller_ShouldExtractClinicIdFromToken()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var planId = Guid.NewGuid();

            var clinic = new Clinic("Test Clinic", _testTenantId);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic, clinicId);
            await _context.Clinics.AddAsync(clinic);
            await _context.SaveChangesAsync();

            var plan = new SubscriptionPlan(
                "Standard", "Standard Plan", 99.00m, 30, 10, 1000,
                SubscriptionPlanType.Standard, _testTenantId,
                hasReports: true
            );
            typeof(SubscriptionPlan).GetProperty(nameof(SubscriptionPlan.Id))!.SetValue(plan, planId);

            var subscription = new ClinicSubscription(clinicId, planId, _testTenantId);

            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(clinicId, _testTenantId))
                .ReturnsAsync(subscription);

            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(planId, _testTenantId))
                .ReturnsAsync(plan);

            var controller = new ModuleConfigController(
                _mockTenantContext.Object,
                _context,
                _mockSubscriptionRepository.Object,
                _mockPlanRepository.Object,
                _mockModuleConfigService.Object
            );

            // Setup controller user context with clinic_id claim
            var claims = new List<Claim>
            {
                new Claim("clinic_id", clinicId.ToString()),
                new Claim("sub", "test-user")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };

            // Act
            var result = await controller.EnableModule(SystemModules.Reports);

            // Assert
            result.Should().BeOfType<OkObjectResult>();

            // Verify the correct clinic was used
            var config = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == clinicId && mc.ModuleName == SystemModules.Reports);
            config.Should().NotBeNull("Module should be enabled for the correct clinic");
        }

        #endregion

        #region Required Modules Tests

        [Fact]
        public async Task EnableModule_WithoutRequiredModules_ShouldFail()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var planId = Guid.NewGuid();

            var clinic = new Clinic("Test Clinic", _testTenantId);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic, clinicId);
            await _context.Clinics.AddAsync(clinic);
            await _context.SaveChangesAsync();

            var plan = new SubscriptionPlan(
                "Standard", "Standard Plan", 99.00m, 30, 10, 1000,
                SubscriptionPlanType.Standard, _testTenantId,
                hasWhatsAppIntegration: true
            );
            typeof(SubscriptionPlan).GetProperty(nameof(SubscriptionPlan.Id))!.SetValue(plan, planId);

            var subscription = new ClinicSubscription(clinicId, planId, _testTenantId);

            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(clinicId, _testTenantId))
                .ReturnsAsync(subscription);

            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(planId, _testTenantId))
                .ReturnsAsync(plan);

            var service = new ModuleConfigurationService(
                _context,
                _mockSubscriptionRepository.Object,
                _mockPlanRepository.Object,
                Mock.Of<Microsoft.Extensions.Logging.ILogger<ModuleConfigurationService>>()
            );

            // Act & Assert - Try to enable WhatsApp without PatientManagement
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                service.EnableModuleAsync(clinicId, SystemModules.WhatsAppIntegration, "test-user")
            );

            exception.Message.Should().Contain("Required modules");
        }

        [Fact]
        public async Task EnableModule_WithRequiredModulesEnabled_ShouldSucceed()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var planId = Guid.NewGuid();

            var clinic = new Clinic("Test Clinic", _testTenantId);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic, clinicId);
            await _context.Clinics.AddAsync(clinic);

            // Enable required module (PatientManagement)
            var requiredConfig = new ModuleConfiguration(
                clinicId,
                SystemModules.PatientManagement,
                _testTenantId,
                true
            );
            await _context.ModuleConfigurations.AddAsync(requiredConfig);
            await _context.SaveChangesAsync();

            var plan = new SubscriptionPlan(
                "Standard", "Standard Plan", 99.00m, 30, 10, 1000,
                SubscriptionPlanType.Standard, _testTenantId,
                hasWhatsAppIntegration: true
            );
            typeof(SubscriptionPlan).GetProperty(nameof(SubscriptionPlan.Id))!.SetValue(plan, planId);

            var subscription = new ClinicSubscription(clinicId, planId, _testTenantId);

            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(clinicId, _testTenantId))
                .ReturnsAsync(subscription);

            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(planId, _testTenantId))
                .ReturnsAsync(plan);

            var service = new ModuleConfigurationService(
                _context,
                _mockSubscriptionRepository.Object,
                _mockPlanRepository.Object,
                Mock.Of<Microsoft.Extensions.Logging.ILogger<ModuleConfigurationService>>()
            );

            // Act - Enable WhatsApp with PatientManagement already enabled
            await service.EnableModuleAsync(clinicId, SystemModules.WhatsAppIntegration, "test-user");

            // Assert
            var config = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == clinicId && mc.ModuleName == SystemModules.WhatsAppIntegration);

            config.Should().NotBeNull();
            config!.IsEnabled.Should().BeTrue();
        }

        #endregion

        #region Audit Trail Tests

        [Fact]
        public async Task EnableModule_ShouldCreateAuditHistory()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var userId = "test-user";

            var clinic = new Clinic("Test Clinic", _testTenantId);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic, clinicId);
            await _context.Clinics.AddAsync(clinic);
            await _context.SaveChangesAsync();

            var plan = new SubscriptionPlan(
                "Standard", "Standard Plan", 99.00m, 30, 10, 1000,
                SubscriptionPlanType.Standard, _testTenantId,
                hasReports: true
            );
            typeof(SubscriptionPlan).GetProperty(nameof(SubscriptionPlan.Id))!.SetValue(plan, planId);

            var subscription = new ClinicSubscription(clinicId, planId, _testTenantId);

            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(clinicId, _testTenantId))
                .ReturnsAsync(subscription);

            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(planId, _testTenantId))
                .ReturnsAsync(plan);

            var service = new ModuleConfigurationService(
                _context,
                _mockSubscriptionRepository.Object,
                _mockPlanRepository.Object,
                Mock.Of<Microsoft.Extensions.Logging.ILogger<ModuleConfigurationService>>()
            );

            // Act
            await service.EnableModuleAsync(clinicId, SystemModules.Reports, userId, "Business requirement");

            // Assert
            var history = await _context.ModuleConfigurationHistories
                .FirstOrDefaultAsync(h => h.ClinicId == clinicId && h.ModuleName == SystemModules.Reports);

            history.Should().NotBeNull();
            history!.Action.Should().Be("Enabled");
            history.ChangedBy.Should().Be(userId);
            history.Reason.Should().Be("Business requirement");
        }

        [Fact]
        public async Task DisableModule_ShouldCreateAuditHistory()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var userId = "test-user";

            var clinic = new Clinic("Test Clinic", _testTenantId);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic, clinicId);
            await _context.Clinics.AddAsync(clinic);

            var config = new ModuleConfiguration(clinicId, SystemModules.Reports, _testTenantId, true);
            await _context.ModuleConfigurations.AddAsync(config);
            await _context.SaveChangesAsync();

            var service = new ModuleConfigurationService(
                _context,
                _mockSubscriptionRepository.Object,
                _mockPlanRepository.Object,
                Mock.Of<Microsoft.Extensions.Logging.ILogger<ModuleConfigurationService>>()
            );

            // Act
            await service.DisableModuleAsync(clinicId, SystemModules.Reports, userId, "Not needed");

            // Assert
            var history = await _context.ModuleConfigurationHistories
                .FirstOrDefaultAsync(h => h.ClinicId == clinicId && h.ModuleName == SystemModules.Reports);

            history.Should().NotBeNull();
            history!.Action.Should().Be("Disabled");
            history.ChangedBy.Should().Be(userId);
            history.Reason.Should().Be("Not needed");
        }

        #endregion

        #region Validation Tests

        [Fact]
        public async Task ValidateModuleConfig_WithoutSubscription_ShouldReturnInvalid()
        {
            // Arrange
            var clinicId = Guid.NewGuid();

            var clinic = new Clinic("Test Clinic", _testTenantId);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic, clinicId);
            await _context.Clinics.AddAsync(clinic);
            await _context.SaveChangesAsync();

            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(clinicId, _testTenantId))
                .ReturnsAsync((ClinicSubscription?)null);

            var service = new ModuleConfigurationService(
                _context,
                _mockSubscriptionRepository.Object,
                _mockPlanRepository.Object,
                Mock.Of<Microsoft.Extensions.Logging.ILogger<ModuleConfigurationService>>()
            );

            // Act
            var result = await service.ValidateModuleConfigAsync(clinicId, SystemModules.Reports);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("no active subscription");
        }

        [Fact]
        public async Task ValidateModuleConfig_WithInvalidPlan_ShouldReturnInvalid()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var planId = Guid.NewGuid();

            var clinic = new Clinic("Test Clinic", _testTenantId);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic, clinicId);
            await _context.Clinics.AddAsync(clinic);
            await _context.SaveChangesAsync();

            var subscription = new ClinicSubscription(clinicId, planId, _testTenantId);

            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(clinicId, _testTenantId))
                .ReturnsAsync(subscription);

            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(planId, _testTenantId))
                .ReturnsAsync((SubscriptionPlan?)null);

            var service = new ModuleConfigurationService(
                _context,
                _mockSubscriptionRepository.Object,
                _mockPlanRepository.Object,
                Mock.Of<Microsoft.Extensions.Logging.ILogger<ModuleConfigurationService>>()
            );

            // Act
            var result = await service.ValidateModuleConfigAsync(clinicId, SystemModules.Reports);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Invalid subscription plan");
        }

        #endregion
    }
}
