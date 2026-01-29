using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;
using Xunit;

namespace MedicSoft.Test.Services
{
    public class ModuleConfigurationServiceTests : IDisposable
    {
        private readonly Mock<IClinicSubscriptionRepository> _mockSubscriptionRepository;
        private readonly Mock<ISubscriptionPlanRepository> _mockPlanRepository;
        private readonly Mock<ILogger<ModuleConfigurationService>> _mockLogger;
        private readonly MedicSoftDbContext _context;
        private readonly ModuleConfigurationService _service;
        private readonly Guid _testClinicId;
        private readonly string _testTenantId;
        private readonly string _testUserId;

        public ModuleConfigurationServiceTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<MedicSoftDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new MedicSoftDbContext(options);
            _mockSubscriptionRepository = new Mock<IClinicSubscriptionRepository>();
            _mockPlanRepository = new Mock<ISubscriptionPlanRepository>();
            _mockLogger = new Mock<ILogger<ModuleConfigurationService>>();

            _service = new ModuleConfigurationService(
                _context,
                _mockSubscriptionRepository.Object,
                _mockPlanRepository.Object,
                _mockLogger.Object
            );

            _testClinicId = Guid.NewGuid();
            _testTenantId = "test-tenant";
            _testUserId = "test-user";

            // Setup test clinic
            // Note: Using reflection to set Id because Clinic entity doesn't expose a constructor with Id parameter
            // This is a common pattern in testing when dealing with entities that use private setters for EF Core
            var clinic = new Clinic("Test Clinic", _testTenantId);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic, _testClinicId);
            _context.Clinics.Add(clinic);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region EnableModuleAsync Tests

        [Fact]
        public async Task EnableModuleAsync_WithValidPlan_ShouldEnableModule()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            var planId = Guid.NewGuid();
            
            var plan = new SubscriptionPlan(
                "Standard", "Standard Plan", 99.00m, 30, 10, 1000,
                SubscriptionPlanType.Standard, _testTenantId,
                hasReports: true
            );
            typeof(SubscriptionPlan).GetProperty(nameof(SubscriptionPlan.Id))!.SetValue(plan, planId);
            
            var subscription = new ClinicSubscription(_testClinicId, planId, _testTenantId);
            
            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(_testClinicId, _testTenantId))
                .ReturnsAsync(subscription);
                
            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(planId, _testTenantId))
                .ReturnsAsync(plan);

            // Act
            await _service.EnableModuleAsync(_testClinicId, moduleName, _testUserId);

            // Assert
            var config = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == _testClinicId && mc.ModuleName == moduleName);
            
            config.Should().NotBeNull();
            config!.IsEnabled.Should().BeTrue();
            
            // Verify history was created
            var history = await _context.ModuleConfigurationHistories
                .FirstOrDefaultAsync(h => h.ClinicId == _testClinicId && h.ModuleName == moduleName);
            history.Should().NotBeNull();
            history!.Action.Should().Be("Enabled");
        }

        [Fact]
        public async Task EnableModuleAsync_WithoutValidPlan_ShouldThrowException()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            var planId = Guid.NewGuid();
            
            var plan = new SubscriptionPlan(
                "Basic", "Basic Plan", 49.00m, 30, 5, 500,
                SubscriptionPlanType.Basic, _testTenantId,
                hasReports: false
            );
            typeof(SubscriptionPlan).GetProperty(nameof(SubscriptionPlan.Id))!.SetValue(plan, planId);
            
            var subscription = new ClinicSubscription(_testClinicId, planId, _testTenantId);
            
            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(_testClinicId, _testTenantId))
                .ReturnsAsync(subscription);
                
            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(planId, _testTenantId))
                .ReturnsAsync(plan);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.EnableModuleAsync(_testClinicId, moduleName, _testUserId)
            );
            
            exception.Message.Should().Contain("not available in current plan");
        }

        [Fact]
        public async Task EnableModuleAsync_WithInvalidModuleName_ShouldThrowException()
        {
            // Arrange
            var moduleName = "InvalidModule";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() =>
                _service.EnableModuleAsync(_testClinicId, moduleName, _testUserId)
            );
        }

        [Fact]
        public async Task EnableModuleAsync_WithoutRequiredModules_ShouldThrowException()
        {
            // Arrange
            var moduleName = SystemModules.WhatsAppIntegration;
            var planId = Guid.NewGuid();
            
            var plan = new SubscriptionPlan(
                "Standard", "Standard Plan", 99.00m, 30, 10, 1000,
                SubscriptionPlanType.Standard, _testTenantId,
                hasWhatsAppIntegration: true
            );
            typeof(SubscriptionPlan).GetProperty(nameof(SubscriptionPlan.Id))!.SetValue(plan, planId);
            
            var subscription = new ClinicSubscription(_testClinicId, planId, _testTenantId);
            
            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(_testClinicId, _testTenantId))
                .ReturnsAsync(subscription);
                
            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(planId, _testTenantId))
                .ReturnsAsync(plan);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.EnableModuleAsync(_testClinicId, moduleName, _testUserId)
            );
            
            exception.Message.Should().Contain("Required modules");
        }

        [Fact]
        public async Task EnableModuleAsync_WhenAlreadyExists_ShouldUpdateExistingConfig()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            var planId = Guid.NewGuid();
            
            // Create existing disabled config
            var existingConfig = new ModuleConfiguration(_testClinicId, moduleName, _testTenantId, false);
            await _context.ModuleConfigurations.AddAsync(existingConfig);
            await _context.SaveChangesAsync();
            
            var plan = new SubscriptionPlan(
                "Standard", "Standard Plan", 99.00m, 30, 10, 1000,
                SubscriptionPlanType.Standard, _testTenantId,
                hasReports: true
            );
            typeof(SubscriptionPlan).GetProperty(nameof(SubscriptionPlan.Id))!.SetValue(plan, planId);
            
            var subscription = new ClinicSubscription(_testClinicId, planId, _testTenantId);
            
            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(_testClinicId, _testTenantId))
                .ReturnsAsync(subscription);
                
            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(planId, _testTenantId))
                .ReturnsAsync(plan);

            // Act
            await _service.EnableModuleAsync(_testClinicId, moduleName, _testUserId);

            // Assert
            var configs = await _context.ModuleConfigurations
                .Where(mc => mc.ClinicId == _testClinicId && mc.ModuleName == moduleName)
                .ToListAsync();
            
            configs.Should().HaveCount(1);
            configs[0].IsEnabled.Should().BeTrue();
        }

        #endregion

        #region DisableModuleAsync Tests

        [Fact]
        public async Task DisableModuleAsync_WithNonCoreModule_ShouldDisableModule()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            var config = new ModuleConfiguration(_testClinicId, moduleName, _testTenantId, true);
            await _context.ModuleConfigurations.AddAsync(config);
            await _context.SaveChangesAsync();

            // Act
            await _service.DisableModuleAsync(_testClinicId, moduleName, _testUserId);

            // Assert
            var updatedConfig = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == _testClinicId && mc.ModuleName == moduleName);
            
            updatedConfig.Should().NotBeNull();
            updatedConfig!.IsEnabled.Should().BeFalse();
            
            // Verify history
            var history = await _context.ModuleConfigurationHistories
                .FirstOrDefaultAsync(h => h.ClinicId == _testClinicId && h.ModuleName == moduleName);
            history.Should().NotBeNull();
            history!.Action.Should().Be("Disabled");
        }

        [Fact]
        public async Task DisableModuleAsync_WithCoreModule_ShouldThrowException()
        {
            // Arrange
            var moduleName = SystemModules.PatientManagement;
            var config = new ModuleConfiguration(_testClinicId, moduleName, _testTenantId, true);
            await _context.ModuleConfigurations.AddAsync(config);
            await _context.SaveChangesAsync();

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.DisableModuleAsync(_testClinicId, moduleName, _testUserId)
            );
            
            exception.Message.Should().Contain("Core modules cannot be disabled");
        }

        [Fact]
        public async Task DisableModuleAsync_WithNonExistentConfig_ShouldThrowException()
        {
            // Arrange
            var moduleName = SystemModules.Reports;

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.DisableModuleAsync(_testClinicId, moduleName, _testUserId)
            );
        }

        [Fact]
        public async Task DisableModuleAsync_WithReason_ShouldRecordReason()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            var reason = "Not needed anymore";
            var config = new ModuleConfiguration(_testClinicId, moduleName, _testTenantId, true);
            await _context.ModuleConfigurations.AddAsync(config);
            await _context.SaveChangesAsync();

            // Act
            await _service.DisableModuleAsync(_testClinicId, moduleName, _testUserId, reason);

            // Assert
            var history = await _context.ModuleConfigurationHistories
                .FirstOrDefaultAsync(h => h.ClinicId == _testClinicId && h.ModuleName == moduleName);
            history.Should().NotBeNull();
            history!.Reason.Should().Be(reason);
        }

        #endregion

        #region UpdateModuleConfigAsync Tests

        [Fact]
        public async Task UpdateModuleConfigAsync_WithValidConfig_ShouldUpdateConfiguration()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            var configuration = "{\"setting\": \"value\"}";
            var config = new ModuleConfiguration(_testClinicId, moduleName, _testTenantId, true);
            await _context.ModuleConfigurations.AddAsync(config);
            await _context.SaveChangesAsync();

            // Act
            await _service.UpdateModuleConfigAsync(_testClinicId, moduleName, configuration, _testUserId);

            // Assert
            var updatedConfig = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == _testClinicId && mc.ModuleName == moduleName);
            
            updatedConfig.Should().NotBeNull();
            updatedConfig!.Configuration.Should().Be(configuration);
            
            // Verify history
            var history = await _context.ModuleConfigurationHistories
                .FirstOrDefaultAsync(h => h.ClinicId == _testClinicId && h.ModuleName == moduleName);
            history.Should().NotBeNull();
            history!.Action.Should().Be("ConfigUpdated");
            history.NewConfiguration.Should().Be(configuration);
        }

        [Fact]
        public async Task UpdateModuleConfigAsync_WhenConfigNotExists_ShouldCreateNew()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            var configuration = "{\"setting\": \"value\"}";

            // Act
            await _service.UpdateModuleConfigAsync(_testClinicId, moduleName, configuration, _testUserId);

            // Assert
            var config = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == _testClinicId && mc.ModuleName == moduleName);
            
            config.Should().NotBeNull();
            config!.Configuration.Should().Be(configuration);
            config.IsEnabled.Should().BeTrue();
        }

        #endregion

        #region GetModuleConfigAsync Tests

        [Fact]
        public async Task GetModuleConfigAsync_WithEnabledModule_ShouldReturnCorrectDto()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            var planId = Guid.NewGuid();
            var config = new ModuleConfiguration(_testClinicId, moduleName, _testTenantId, true, "{\"test\":\"value\"}");
            await _context.ModuleConfigurations.AddAsync(config);
            await _context.SaveChangesAsync();
            
            var plan = new SubscriptionPlan(
                "Standard", "Standard Plan", 99.00m, 30, 10, 1000,
                SubscriptionPlanType.Standard, _testTenantId,
                hasReports: true
            );
            typeof(SubscriptionPlan).GetProperty(nameof(SubscriptionPlan.Id))!.SetValue(plan, planId);
            
            var subscription = new ClinicSubscription(_testClinicId, planId, _testTenantId);
            
            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(_testClinicId, _testTenantId))
                .ReturnsAsync(subscription);
                
            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(planId, _testTenantId))
                .ReturnsAsync(plan);

            // Act
            var result = await _service.GetModuleConfigAsync(_testClinicId, moduleName);

            // Assert
            result.Should().NotBeNull();
            result.ModuleName.Should().Be(moduleName);
            result.IsEnabled.Should().BeTrue();
            result.IsAvailableInPlan.Should().BeTrue();
            result.Configuration.Should().Be("{\"test\":\"value\"}");
        }

        [Fact]
        public async Task GetModuleConfigAsync_WithNonExistentConfig_ShouldReturnDefaultDto()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            var planId = Guid.NewGuid();
            
            var plan = new SubscriptionPlan(
                "Basic", "Basic Plan", 49.00m, 30, 5, 500,
                SubscriptionPlanType.Basic, _testTenantId,
                hasReports: false
            );
            typeof(SubscriptionPlan).GetProperty(nameof(SubscriptionPlan.Id))!.SetValue(plan, planId);
            
            var subscription = new ClinicSubscription(_testClinicId, planId, _testTenantId);
            
            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(_testClinicId, _testTenantId))
                .ReturnsAsync(subscription);
                
            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(planId, _testTenantId))
                .ReturnsAsync(plan);

            // Act
            var result = await _service.GetModuleConfigAsync(_testClinicId, moduleName);

            // Assert
            result.Should().NotBeNull();
            result.IsEnabled.Should().BeFalse();
            result.IsAvailableInPlan.Should().BeFalse();
        }

        #endregion

        #region GetAllModuleConfigsAsync Tests

        [Fact]
        public async Task GetAllModuleConfigsAsync_ShouldReturnAllModules()
        {
            // Arrange
            var planId = Guid.NewGuid();
            var plan = new SubscriptionPlan(
                "Standard", "Standard Plan", 99.00m, 30, 10, 1000,
                SubscriptionPlanType.Standard, _testTenantId,
                hasReports: true
            );
            typeof(SubscriptionPlan).GetProperty(nameof(SubscriptionPlan.Id))!.SetValue(plan, planId);
            
            var subscription = new ClinicSubscription(_testClinicId, planId, _testTenantId);
            
            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(_testClinicId, _testTenantId))
                .ReturnsAsync(subscription);
                
            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(planId, _testTenantId))
                .ReturnsAsync(plan);

            // Act
            var result = await _service.GetAllModuleConfigsAsync(_testClinicId);

            // Assert
            var moduleList = result.ToList();
            moduleList.Should().HaveCount(SystemModules.GetAllModules().Length);
            moduleList.Should().Contain(m => m.ModuleName == SystemModules.PatientManagement);
            moduleList.Should().Contain(m => m.ModuleName == SystemModules.Reports);
        }

        #endregion

        #region ValidateModuleConfigAsync Tests

        [Fact]
        public async Task ValidateModuleConfigAsync_WithValidModule_ShouldReturnValid()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            var planId = Guid.NewGuid();
            
            var plan = new SubscriptionPlan(
                "Standard", "Standard Plan", 99.00m, 30, 10, 1000,
                SubscriptionPlanType.Standard, _testTenantId,
                hasReports: true
            );
            typeof(SubscriptionPlan).GetProperty(nameof(SubscriptionPlan.Id))!.SetValue(plan, planId);
            
            var subscription = new ClinicSubscription(_testClinicId, planId, _testTenantId);
            
            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(_testClinicId, _testTenantId))
                .ReturnsAsync(subscription);
                
            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(planId, _testTenantId))
                .ReturnsAsync(plan);

            // Act
            var result = await _service.ValidateModuleConfigAsync(_testClinicId, moduleName);

            // Assert
            result.IsValid.Should().BeTrue();
            result.ErrorMessage.Should().BeEmpty();
        }

        [Fact]
        public async Task ValidateModuleConfigAsync_WithInvalidModule_ShouldReturnInvalid()
        {
            // Arrange
            var moduleName = "InvalidModule";

            // Act
            var result = await _service.ValidateModuleConfigAsync(_testClinicId, moduleName);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("Module not found");
        }

        [Fact]
        public async Task ValidateModuleConfigAsync_WithoutSubscription_ShouldReturnInvalid()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            
            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(_testClinicId, _testTenantId))
                .ReturnsAsync((ClinicSubscription?)null);

            // Act
            var result = await _service.ValidateModuleConfigAsync(_testClinicId, moduleName);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("no active subscription");
        }

        [Fact]
        public async Task ValidateModuleConfigAsync_WithModuleNotInPlan_ShouldReturnInvalid()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            var planId = Guid.NewGuid();
            
            var plan = new SubscriptionPlan(
                "Basic", "Basic Plan", 49.00m, 30, 5, 500,
                SubscriptionPlanType.Basic, _testTenantId,
                hasReports: false
            );
            typeof(SubscriptionPlan).GetProperty(nameof(SubscriptionPlan.Id))!.SetValue(plan, planId);
            
            var subscription = new ClinicSubscription(_testClinicId, planId, _testTenantId);
            
            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(_testClinicId, _testTenantId))
                .ReturnsAsync(subscription);
                
            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(planId, _testTenantId))
                .ReturnsAsync(plan);

            // Act
            var result = await _service.ValidateModuleConfigAsync(_testClinicId, moduleName);

            // Assert
            result.IsValid.Should().BeFalse();
            result.ErrorMessage.Should().Contain("not available in current plan");
        }

        #endregion

        #region HasRequiredModulesAsync Tests

        [Fact]
        public async Task HasRequiredModulesAsync_WithNoRequiredModules_ShouldReturnTrue()
        {
            // Arrange
            var moduleName = SystemModules.Reports;

            // Act
            var result = await _service.HasRequiredModulesAsync(_testClinicId, moduleName);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task HasRequiredModulesAsync_WithEnabledRequiredModules_ShouldReturnTrue()
        {
            // Arrange
            var moduleName = SystemModules.WhatsAppIntegration;
            
            // Enable required module (PatientManagement)
            var requiredConfig = new ModuleConfiguration(
                _testClinicId, 
                SystemModules.PatientManagement, 
                _testTenantId, 
                true
            );
            await _context.ModuleConfigurations.AddAsync(requiredConfig);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.HasRequiredModulesAsync(_testClinicId, moduleName);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task HasRequiredModulesAsync_WithDisabledRequiredModules_ShouldReturnFalse()
        {
            // Arrange
            var moduleName = SystemModules.WhatsAppIntegration;
            
            // Add required module but disabled
            var requiredConfig = new ModuleConfiguration(
                _testClinicId, 
                SystemModules.PatientManagement, 
                _testTenantId, 
                false
            );
            await _context.ModuleConfigurations.AddAsync(requiredConfig);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.HasRequiredModulesAsync(_testClinicId, moduleName);

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public async Task HasRequiredModulesAsync_WithMissingRequiredModules_ShouldReturnFalse()
        {
            // Arrange
            var moduleName = SystemModules.WhatsAppIntegration;

            // Act (no required modules configured)
            var result = await _service.HasRequiredModulesAsync(_testClinicId, moduleName);

            // Assert
            result.Should().BeFalse();
        }

        #endregion

        #region GetGlobalModuleUsageAsync Tests

        [Fact]
        public async Task GetGlobalModuleUsageAsync_ShouldReturnCorrectStatistics()
        {
            // Arrange
            var clinic2Id = Guid.NewGuid();
            var clinic2 = new Clinic("Test Clinic 2", _testTenantId);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic2, clinic2Id);
            await _context.Clinics.AddAsync(clinic2);
            
            // Add configurations for different clinics
            await _context.ModuleConfigurations.AddAsync(
                new ModuleConfiguration(_testClinicId, SystemModules.Reports, _testTenantId, true)
            );
            await _context.ModuleConfigurations.AddAsync(
                new ModuleConfiguration(clinic2Id, SystemModules.Reports, _testTenantId, true)
            );
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetGlobalModuleUsageAsync();

            // Assert
            var usageList = result.ToList();
            usageList.Should().NotBeEmpty();
            
            var reportsUsage = usageList.FirstOrDefault(m => m.ModuleName == SystemModules.Reports);
            reportsUsage.Should().NotBeNull();
            reportsUsage!.ClinicsWithModuleEnabled.Should().Be(2);
            reportsUsage.TotalClinics.Should().Be(2);
            reportsUsage.AdoptionRate.Should().Be(100m);
        }

        #endregion

        #region GetModuleHistoryAsync Tests

        [Fact]
        public async Task GetModuleHistoryAsync_ShouldReturnHistoryOrderedByDate()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            var configId = Guid.NewGuid();
            
            var history1 = new ModuleConfigurationHistory(
                configId, _testClinicId, moduleName, "Enabled", 
                _testUserId, _testTenantId
            );
            var history2 = new ModuleConfigurationHistory(
                configId, _testClinicId, moduleName, "ConfigUpdated", 
                _testUserId, _testTenantId
            );
            
            await _context.ModuleConfigurationHistories.AddRangeAsync(history1, history2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetModuleHistoryAsync(_testClinicId, moduleName);

            // Assert
            var historyList = result.ToList();
            historyList.Should().HaveCount(2);
            historyList[0].ChangedAt.Should().BeAfter(historyList[1].ChangedAt);
        }

        #endregion

        #region CanEnableModuleAsync Tests

        [Fact]
        public async Task CanEnableModuleAsync_WithValidConditions_ShouldReturnTrue()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            var planId = Guid.NewGuid();
            
            var plan = new SubscriptionPlan(
                "Standard", "Standard Plan", 99.00m, 30, 10, 1000,
                SubscriptionPlanType.Standard, _testTenantId,
                hasReports: true
            );
            typeof(SubscriptionPlan).GetProperty(nameof(SubscriptionPlan.Id))!.SetValue(plan, planId);
            
            var subscription = new ClinicSubscription(_testClinicId, planId, _testTenantId);
            
            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(_testClinicId, _testTenantId))
                .ReturnsAsync(subscription);
                
            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(planId, _testTenantId))
                .ReturnsAsync(plan);

            // Act
            var result = await _service.CanEnableModuleAsync(_testClinicId, moduleName);

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public async Task CanEnableModuleAsync_WithInvalidConditions_ShouldReturnFalse()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            
            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(_testClinicId, _testTenantId))
                .ReturnsAsync((ClinicSubscription?)null);

            // Act
            var result = await _service.CanEnableModuleAsync(_testClinicId, moduleName);

            // Assert
            result.Should().BeFalse();
        }

        #endregion
    }
}
