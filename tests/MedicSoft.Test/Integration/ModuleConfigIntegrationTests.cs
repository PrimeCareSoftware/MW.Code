using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;
using Xunit;

namespace MedicSoft.Test.Integration
{
    /// <summary>
    /// Integration tests for Module Configuration System
    /// Tests the full flow from service to database
    /// </summary>
    public class ModuleConfigIntegrationTests : IDisposable
    {
        private readonly MedicSoftDbContext _context;
        private readonly Mock<IClinicSubscriptionRepository> _mockSubscriptionRepository;
        private readonly Mock<ISubscriptionPlanRepository> _mockPlanRepository;
        private readonly ModuleConfigurationService _service;
        private readonly string _testTenantId;

        public ModuleConfigIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<MedicSoftDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new MedicSoftDbContext(options);
            _mockSubscriptionRepository = new Mock<IClinicSubscriptionRepository>();
            _mockPlanRepository = new Mock<ISubscriptionPlanRepository>();

            _service = new ModuleConfigurationService(
                _context,
                _mockSubscriptionRepository.Object,
                _mockPlanRepository.Object,
                Mock.Of<ILogger<ModuleConfigurationService>>()
            );

            _testTenantId = "integration-tenant";
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region End-to-End Module Lifecycle Tests

        [Fact]
        public async Task CompleteModuleLifecycle_EnableConfigureDisable_ShouldWork()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var userId = "integration-user";
            var moduleName = SystemModules.Reports;

            // Setup clinic
            var clinic = new Clinic("Integration Test Clinic", _testTenantId);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic, clinicId);
            await _context.Clinics.AddAsync(clinic);
            await _context.SaveChangesAsync();

            // Setup plan with Reports
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

            // Act 1: Enable module
            await _service.EnableModuleAsync(clinicId, moduleName, userId, "Initial setup");

            // Assert 1: Module is enabled
            var config1 = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == clinicId && mc.ModuleName == moduleName);
            config1.Should().NotBeNull();
            config1!.IsEnabled.Should().BeTrue();

            // Act 2: Update configuration
            var configuration = "{\"reportType\": \"monthly\", \"format\": \"pdf\"}";
            await _service.UpdateModuleConfigAsync(clinicId, moduleName, configuration, userId);

            // Assert 2: Configuration is updated
            var config2 = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == clinicId && mc.ModuleName == moduleName);
            config2!.Configuration.Should().Be(configuration);

            // Act 3: Disable module
            await _service.DisableModuleAsync(clinicId, moduleName, userId, "No longer needed");

            // Assert 3: Module is disabled
            var config3 = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == clinicId && mc.ModuleName == moduleName);
            config3!.IsEnabled.Should().BeFalse();

            // Assert 4: History is complete
            var history = await _context.ModuleConfigurationHistories
                .Where(h => h.ClinicId == clinicId && h.ModuleName == moduleName)
                .OrderBy(h => h.ChangedAt)
                .ToListAsync();

            history.Should().HaveCount(3);
            history[0].Action.Should().Be("Enabled");
            history[1].Action.Should().Be("ConfigUpdated");
            history[2].Action.Should().Be("Disabled");
        }

        #endregion

        #region Multiple Clinics Tests

        [Fact]
        public async Task MultipleClinics_ShouldHaveIsolatedConfigurations()
        {
            // Arrange
            var clinic1Id = Guid.NewGuid();
            var clinic2Id = Guid.NewGuid();
            var planId = Guid.NewGuid();

            // Setup clinics
            var clinic1 = new Clinic("Clinic 1", _testTenantId);
            var clinic2 = new Clinic("Clinic 2", _testTenantId);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic1, clinic1Id);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic2, clinic2Id);
            await _context.Clinics.AddRangeAsync(clinic1, clinic2);
            await _context.SaveChangesAsync();

            // Setup plan
            var plan = new SubscriptionPlan(
                "Standard", "Standard Plan", 99.00m, 30, 10, 1000,
                SubscriptionPlanType.Standard, _testTenantId,
                hasReports: true
            );
            typeof(SubscriptionPlan).GetProperty(nameof(SubscriptionPlan.Id))!.SetValue(plan, planId);

            var subscription1 = new ClinicSubscription(clinic1Id, planId, _testTenantId);
            var subscription2 = new ClinicSubscription(clinic2Id, planId, _testTenantId);

            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(It.IsAny<Guid>(), _testTenantId))
                .ReturnsAsync((Guid cId, string _) => cId == clinic1Id ? subscription1 : subscription2);

            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(planId, _testTenantId))
                .ReturnsAsync(plan);

            // Act: Enable Reports for clinic1, disable for clinic2
            await _service.EnableModuleAsync(clinic1Id, SystemModules.Reports, "user1");

            // Add and disable for clinic2
            var clinic2Config = new ModuleConfiguration(clinic2Id, SystemModules.Reports, _testTenantId, true);
            await _context.ModuleConfigurations.AddAsync(clinic2Config);
            await _context.SaveChangesAsync();
            await _service.DisableModuleAsync(clinic2Id, SystemModules.Reports, "user2");

            // Assert: Each clinic has its own configuration
            var clinic1Modules = await _service.GetAllModuleConfigsAsync(clinic1Id);
            var clinic2Modules = await _service.GetAllModuleConfigsAsync(clinic2Id);

            var clinic1Reports = clinic1Modules.FirstOrDefault(m => m.ModuleName == SystemModules.Reports);
            var clinic2Reports = clinic2Modules.FirstOrDefault(m => m.ModuleName == SystemModules.Reports);

            clinic1Reports.Should().NotBeNull();
            clinic1Reports!.IsEnabled.Should().BeTrue();

            clinic2Reports.Should().NotBeNull();
            clinic2Reports!.IsEnabled.Should().BeFalse();
        }

        #endregion

        #region Module Dependencies Tests

        [Fact]
        public async Task EnableModuleChain_WithDependencies_ShouldWorkInOrder()
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

            // Act 1: Enable required module first (PatientManagement)
            await _service.EnableModuleAsync(clinicId, SystemModules.PatientManagement, "user");

            // Act 2: Enable dependent module (WhatsAppIntegration)
            await _service.EnableModuleAsync(clinicId, SystemModules.WhatsAppIntegration, "user");

            // Assert: Both modules are enabled
            var configs = await _context.ModuleConfigurations
                .Where(mc => mc.ClinicId == clinicId)
                .ToListAsync();

            configs.Should().HaveCount(2);
            configs.Should().Contain(c => c.ModuleName == SystemModules.PatientManagement && c.IsEnabled);
            configs.Should().Contain(c => c.ModuleName == SystemModules.WhatsAppIntegration && c.IsEnabled);
        }

        [Fact]
        public async Task EnableModule_WithMultipleDependencies_ShouldValidateAll()
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

            // Enable FinancialManagement (required for TissExport)
            await _service.EnableModuleAsync(clinicId, SystemModules.FinancialManagement, "user");

            // Act: Enable TissExport
            await _service.EnableModuleAsync(clinicId, SystemModules.TissExport, "user");

            // Assert
            var tissConfig = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == clinicId && mc.ModuleName == SystemModules.TissExport);

            tissConfig.Should().NotBeNull();
            tissConfig!.IsEnabled.Should().BeTrue();
        }

        #endregion

        #region Plan Upgrade/Downgrade Scenarios

        [Fact]
        public async Task PlanDowngrade_ShouldPreventEnablingPremiumModules()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var basicPlanId = Guid.NewGuid();

            var clinic = new Clinic("Test Clinic", _testTenantId);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic, clinicId);
            await _context.Clinics.AddAsync(clinic);
            await _context.SaveChangesAsync();

            // Simulate downgrade to Basic plan
            var basicPlan = new SubscriptionPlan(
                "Basic", "Basic Plan", 49.00m, 30, 5, 500,
                SubscriptionPlanType.Basic, _testTenantId,
                hasReports: false
            );
            typeof(SubscriptionPlan).GetProperty(nameof(SubscriptionPlan.Id))!.SetValue(basicPlan, basicPlanId);

            var subscription = new ClinicSubscription(clinicId, basicPlanId, _testTenantId);

            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(clinicId, _testTenantId))
                .ReturnsAsync(subscription);

            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(basicPlanId, _testTenantId))
                .ReturnsAsync(basicPlan);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _service.EnableModuleAsync(clinicId, SystemModules.Reports, "user")
            );

            exception.Message.Should().Contain("not available in current plan");
        }

        [Fact]
        public async Task PlanUpgrade_ShouldAllowEnablingPremiumModules()
        {
            // Arrange
            var clinicId = Guid.NewGuid();
            var premiumPlanId = Guid.NewGuid();

            var clinic = new Clinic("Test Clinic", _testTenantId);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic, clinicId);
            await _context.Clinics.AddAsync(clinic);
            await _context.SaveChangesAsync();

            // Simulate upgrade to Premium plan
            var premiumPlan = new SubscriptionPlan(
                "Premium", "Premium Plan", 199.00m, 30, 50, 5000,
                SubscriptionPlanType.Premium, _testTenantId,
                hasReports: true,
                hasTissExport: true
            );
            typeof(SubscriptionPlan).GetProperty(nameof(SubscriptionPlan.Id))!.SetValue(premiumPlan, premiumPlanId);

            var subscription = new ClinicSubscription(clinicId, premiumPlanId, _testTenantId);

            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(clinicId, _testTenantId))
                .ReturnsAsync(subscription);

            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(premiumPlanId, _testTenantId))
                .ReturnsAsync(premiumPlan);

            // Enable FinancialManagement first (required for TissExport)
            await _service.EnableModuleAsync(clinicId, SystemModules.FinancialManagement, "user");

            // Act: Enable premium module
            await _service.EnableModuleAsync(clinicId, SystemModules.TissExport, "user");

            // Assert
            var config = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == clinicId && mc.ModuleName == SystemModules.TissExport);

            config.Should().NotBeNull();
            config!.IsEnabled.Should().BeTrue();
        }

        #endregion

        #region Global Usage Statistics Tests

        [Fact]
        public async Task GetGlobalModuleUsage_WithMultipleClinics_ShouldCalculateCorrectly()
        {
            // Arrange
            var clinic1Id = Guid.NewGuid();
            var clinic2Id = Guid.NewGuid();
            var clinic3Id = Guid.NewGuid();

            var clinic1 = new Clinic("Clinic 1", _testTenantId);
            var clinic2 = new Clinic("Clinic 2", _testTenantId);
            var clinic3 = new Clinic("Clinic 3", _testTenantId);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic1, clinic1Id);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic2, clinic2Id);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic3, clinic3Id);
            await _context.Clinics.AddRangeAsync(clinic1, clinic2, clinic3);

            // Clinic 1 and 2 have Reports enabled
            await _context.ModuleConfigurations.AddAsync(
                new ModuleConfiguration(clinic1Id, SystemModules.Reports, _testTenantId, true)
            );
            await _context.ModuleConfigurations.AddAsync(
                new ModuleConfiguration(clinic2Id, SystemModules.Reports, _testTenantId, true)
            );
            // Clinic 3 has Reports disabled
            await _context.ModuleConfigurations.AddAsync(
                new ModuleConfiguration(clinic3Id, SystemModules.Reports, _testTenantId, false)
            );

            await _context.SaveChangesAsync();

            // Act
            var usage = await _service.GetGlobalModuleUsageAsync();

            // Assert
            var reportsUsage = usage.FirstOrDefault(u => u.ModuleName == SystemModules.Reports);
            reportsUsage.Should().NotBeNull();
            reportsUsage!.TotalClinics.Should().Be(3);
            reportsUsage.ClinicsWithModuleEnabled.Should().Be(2);
            reportsUsage.AdoptionRate.Should().BeApproximately(66.67m, 0.01m);
        }

        #endregion

        #region History and Audit Tests

        [Fact]
        public async Task ModuleOperations_ShouldMaintainCompleteHistory()
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

            // Act: Perform multiple operations
            await _service.EnableModuleAsync(clinicId, SystemModules.Reports, "user1", "Initial enable");
            await _service.UpdateModuleConfigAsync(clinicId, SystemModules.Reports, "{\"test\":1}", "user1");
            await _service.UpdateModuleConfigAsync(clinicId, SystemModules.Reports, "{\"test\":2}", "user2");
            await _service.DisableModuleAsync(clinicId, SystemModules.Reports, "user2", "Temporary disable");
            
            // Re-enable
            await _service.EnableModuleAsync(clinicId, SystemModules.Reports, "user3", "Re-enable");

            // Assert: History is complete and ordered
            var history = await _service.GetModuleHistoryAsync(clinicId, SystemModules.Reports);
            var historyList = history.ToList();

            historyList.Should().HaveCount(5);
            historyList[0].Action.Should().Be("Enabled"); // Most recent
            historyList[1].Action.Should().Be("Disabled");
            historyList[2].Action.Should().Be("ConfigUpdated");
            historyList[3].Action.Should().Be("ConfigUpdated");
            historyList[4].Action.Should().Be("Enabled"); // Oldest
        }

        #endregion

        #region Concurrency Tests

        [Fact]
        public async Task ConcurrentModuleOperations_ShouldHandleCorrectly()
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

            // Act: Enable multiple modules concurrently
            var tasks = new[]
            {
                _service.EnableModuleAsync(clinicId, SystemModules.Reports, "user"),
                _service.EnableModuleAsync(clinicId, SystemModules.InventoryManagement, "user")
            };

            await Task.WhenAll(tasks);

            // Assert: All modules are enabled
            var configs = await _context.ModuleConfigurations
                .Where(mc => mc.ClinicId == clinicId)
                .ToListAsync();

            configs.Should().Contain(c => c.ModuleName == SystemModules.Reports && c.IsEnabled);
            configs.Should().Contain(c => c.ModuleName == SystemModules.InventoryManagement && c.IsEnabled);
        }

        #endregion

        #region Configuration Persistence Tests

        [Fact]
        public async Task ModuleConfiguration_ShouldPersistCorrectly()
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

            var complexConfig = @"{
                ""emailSettings"": {
                    ""enabled"": true,
                    ""recipients"": [""admin@clinic.com""]
                },
                ""schedule"": ""0 0 * * *"",
                ""format"": ""pdf""
            }";

            // Act
            await _service.EnableModuleAsync(clinicId, SystemModules.Reports, "user");
            await _service.UpdateModuleConfigAsync(clinicId, SystemModules.Reports, complexConfig, "user");

            // Create new service instance to simulate app restart
            var newService = new ModuleConfigurationService(
                _context,
                _mockSubscriptionRepository.Object,
                _mockPlanRepository.Object,
                Mock.Of<ILogger<ModuleConfigurationService>>()
            );

            // Assert: Configuration persists across service instances
            var retrievedConfig = await newService.GetModuleConfigAsync(clinicId, SystemModules.Reports);
            retrievedConfig.Configuration.Should().Be(complexConfig);
            retrievedConfig.IsEnabled.Should().BeTrue();
        }

        #endregion
    }
}
