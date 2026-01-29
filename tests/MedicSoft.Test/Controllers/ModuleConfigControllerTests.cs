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
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;
using Xunit;

namespace MedicSoft.Test.Controllers
{
    public class ModuleConfigControllerTests : IDisposable
    {
        private readonly Mock<ITenantContext> _mockTenantContext;
        private readonly Mock<IClinicSubscriptionRepository> _mockSubscriptionRepository;
        private readonly Mock<ISubscriptionPlanRepository> _mockPlanRepository;
        private readonly Mock<IModuleConfigurationService> _mockModuleConfigService;
        private readonly MedicSoftDbContext _context;
        private readonly ModuleConfigController _controller;
        private readonly Guid _testClinicId;
        private readonly string _testTenantId;

        public ModuleConfigControllerTests()
        {
            var options = new DbContextOptionsBuilder<MedicSoftDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new MedicSoftDbContext(options);
            _mockTenantContext = new Mock<ITenantContext>();
            _mockSubscriptionRepository = new Mock<IClinicSubscriptionRepository>();
            _mockPlanRepository = new Mock<ISubscriptionPlanRepository>();
            _mockModuleConfigService = new Mock<IModuleConfigurationService>();

            _testClinicId = Guid.NewGuid();
            _testTenantId = "test-tenant";

            _mockTenantContext.Setup(t => t.TenantId).Returns(_testTenantId);

            _controller = new ModuleConfigController(
                _mockTenantContext.Object,
                _context,
                _mockSubscriptionRepository.Object,
                _mockPlanRepository.Object,
                _mockModuleConfigService.Object
            );

            // Setup test clinic
            var clinic = new Clinic("Test Clinic", _testTenantId);
            typeof(Clinic).GetProperty(nameof(Clinic.Id))!.SetValue(clinic, _testClinicId);
            _context.Clinics.Add(clinic);
            _context.SaveChanges();

            // Setup controller user context
            var claims = new List<Claim>
            {
                new Claim("clinic_id", _testClinicId.ToString()),
                new Claim("sub", "test-user")
            };
            var identity = new ClaimsIdentity(claims, "TestAuthType");
            var claimsPrincipal = new ClaimsPrincipal(identity);
            _controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext { User = claimsPrincipal }
            };
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }

        #region GetModules Tests

        [Fact]
        public async Task GetModules_WithValidSubscription_ShouldReturnOk()
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
            var result = await _controller.GetModules();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var modules = okResult.Value.Should().BeAssignableTo<IEnumerable<ModuleDto>>().Subject;
            modules.Should().NotBeEmpty();
        }

        [Fact]
        public async Task GetModules_WithoutSubscription_ShouldReturnBadRequest()
        {
            // Arrange
            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(_testClinicId, _testTenantId))
                .ReturnsAsync((ClinicSubscription?)null);

            // Act
            var result = await _controller.GetModules();

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task GetModules_WithInvalidPlan_ShouldReturnBadRequest()
        {
            // Arrange
            var planId = Guid.NewGuid();
            var subscription = new ClinicSubscription(_testClinicId, planId, _testTenantId);

            _mockSubscriptionRepository
                .Setup(r => r.GetByClinicIdAsync(_testClinicId, _testTenantId))
                .ReturnsAsync(subscription);

            _mockPlanRepository
                .Setup(r => r.GetByIdAsync(planId, _testTenantId))
                .ReturnsAsync((SubscriptionPlan?)null);

            // Act
            var result = await _controller.GetModules();

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        #endregion

        #region EnableModule Tests

        [Fact]
        public async Task EnableModule_WithValidModule_ShouldReturnOk()
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
            var result = await _controller.EnableModule(moduleName);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var config = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == _testClinicId && mc.ModuleName == moduleName);
            config.Should().NotBeNull();
            config!.IsEnabled.Should().BeTrue();
        }

        [Fact]
        public async Task EnableModule_WithInvalidModuleName_ShouldReturnBadRequest()
        {
            // Arrange
            var moduleName = "InvalidModule";

            // Act
            var result = await _controller.EnableModule(moduleName);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task EnableModule_WithModuleNotInPlan_ShouldReturnBadRequest()
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
            var result = await _controller.EnableModule(moduleName);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task EnableModule_WhenAlreadyEnabled_ShouldUpdateConfig()
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
            var result = await _controller.EnableModule(moduleName);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var config = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == _testClinicId && mc.ModuleName == moduleName);
            config!.IsEnabled.Should().BeTrue();
        }

        #endregion

        #region DisableModule Tests

        [Fact]
        public async Task DisableModule_WithValidModule_ShouldReturnOk()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            var config = new ModuleConfiguration(_testClinicId, moduleName, _testTenantId, true);
            await _context.ModuleConfigurations.AddAsync(config);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.DisableModule(moduleName);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var updatedConfig = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == _testClinicId && mc.ModuleName == moduleName);
            updatedConfig!.IsEnabled.Should().BeFalse();
        }

        [Fact]
        public async Task DisableModule_WithInvalidModuleName_ShouldReturnBadRequest()
        {
            // Arrange
            var moduleName = "InvalidModule";

            // Act
            var result = await _controller.DisableModule(moduleName);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task DisableModule_WithNonExistentConfig_ShouldReturnNotFound()
        {
            // Arrange
            var moduleName = SystemModules.Reports;

            // Act
            var result = await _controller.DisableModule(moduleName);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        #endregion

        #region UpdateModuleConfig Tests

        [Fact]
        public async Task UpdateModuleConfig_WithValidRequest_ShouldReturnOk()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            var request = new UpdateConfigRequest { Configuration = "{\"setting\": \"value\"}" };
            var config = new ModuleConfiguration(_testClinicId, moduleName, _testTenantId, true);
            await _context.ModuleConfigurations.AddAsync(config);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.UpdateModuleConfig(moduleName, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var updatedConfig = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == _testClinicId && mc.ModuleName == moduleName);
            updatedConfig!.Configuration.Should().Be(request.Configuration);
        }

        [Fact]
        public async Task UpdateModuleConfig_WithInvalidModuleName_ShouldReturnBadRequest()
        {
            // Arrange
            var moduleName = "InvalidModule";
            var request = new UpdateConfigRequest { Configuration = "{\"setting\": \"value\"}" };

            // Act
            var result = await _controller.UpdateModuleConfig(moduleName, request);

            // Assert
            result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task UpdateModuleConfig_WhenConfigNotExists_ShouldCreateNew()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            var request = new UpdateConfigRequest { Configuration = "{\"setting\": \"value\"}" };

            // Act
            var result = await _controller.UpdateModuleConfig(moduleName, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            var config = await _context.ModuleConfigurations
                .FirstOrDefaultAsync(mc => mc.ClinicId == _testClinicId && mc.ModuleName == moduleName);
            config.Should().NotBeNull();
            config!.Configuration.Should().Be(request.Configuration);
        }

        #endregion

        #region GetAvailableModules Tests

        [Fact]
        public void GetAvailableModules_ShouldReturnAllModuleNames()
        {
            // Act
            var result = _controller.GetAvailableModules();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var modules = okResult.Value.Should().BeAssignableTo<IEnumerable<string>>().Subject;
            modules.Should().NotBeEmpty();
            modules.Should().Contain(SystemModules.PatientManagement);
            modules.Should().Contain(SystemModules.Reports);
        }

        #endregion

        #region GetModulesInfo Tests

        [Fact]
        public void GetModulesInfo_ShouldReturnModuleDetails()
        {
            // Act
            var result = _controller.GetModulesInfo();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var modules = okResult.Value.Should().BeAssignableTo<IEnumerable<ModuleInfoDto>>().Subject;
            var moduleList = modules.ToList();
            
            moduleList.Should().NotBeEmpty();
            moduleList.Should().HaveCount(13); // All 13 modules defined in SystemModules
            
            var patientModule = moduleList.FirstOrDefault(m => m.Name == SystemModules.PatientManagement);
            patientModule.Should().NotBeNull();
            patientModule!.IsCore.Should().BeTrue();
        }

        #endregion

        #region ValidateModuleConfig Tests

        [Fact]
        public async Task ValidateModuleConfig_WithValidModule_ShouldReturnValid()
        {
            // Arrange
            var request = new ValidateModuleRequest { ModuleName = SystemModules.Reports };
            var validationResult = new ModuleValidationResult(true);

            _mockModuleConfigService
                .Setup(s => s.ValidateModuleConfigAsync(_testClinicId, request.ModuleName))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.ValidateModuleConfig(request);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<ValidationResponseDto>().Subject;
            response.IsValid.Should().BeTrue();
        }

        [Fact]
        public async Task ValidateModuleConfig_WithInvalidModule_ShouldReturnInvalid()
        {
            // Arrange
            var request = new ValidateModuleRequest { ModuleName = "InvalidModule" };
            var validationResult = new ModuleValidationResult(false, "Module not found");

            _mockModuleConfigService
                .Setup(s => s.ValidateModuleConfigAsync(_testClinicId, request.ModuleName))
                .ReturnsAsync(validationResult);

            // Act
            var result = await _controller.ValidateModuleConfig(request);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var response = okResult.Value.Should().BeOfType<ValidationResponseDto>().Subject;
            response.IsValid.Should().BeFalse();
            response.ErrorMessage.Should().Contain("Module not found");
        }

        #endregion

        #region GetModuleHistory Tests

        [Fact]
        public async Task GetModuleHistory_ShouldReturnHistory()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            var history = new List<ModuleConfigHistoryDto>
            {
                new ModuleConfigHistoryDto
                {
                    Id = Guid.NewGuid(),
                    ModuleName = moduleName,
                    Action = "Enabled",
                    ChangedBy = "test-user",
                    ChangedAt = DateTime.UtcNow
                }
            };

            _mockModuleConfigService
                .Setup(s => s.GetModuleHistoryAsync(_testClinicId, moduleName))
                .ReturnsAsync(history);

            // Act
            var result = await _controller.GetModuleHistory(moduleName);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var historyList = okResult.Value.Should().BeAssignableTo<IEnumerable<ModuleConfigHistoryDto>>().Subject;
            historyList.Should().HaveCount(1);
        }

        #endregion

        #region EnableModuleWithReason Tests

        [Fact]
        public async Task EnableModuleWithReason_ShouldCallServiceWithReason()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            var request = new EnableModuleRequest { Reason = "Business requirement" };

            _mockModuleConfigService
                .Setup(s => s.EnableModuleAsync(_testClinicId, moduleName, "test-user", request.Reason))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _controller.EnableModuleWithReason(moduleName, request);

            // Assert
            result.Should().BeOfType<OkObjectResult>();
            _mockModuleConfigService.Verify(
                s => s.EnableModuleAsync(_testClinicId, moduleName, "test-user", request.Reason),
                Times.Once
            );
        }

        [Fact]
        public async Task EnableModuleWithReason_WhenServiceThrows_ShouldPropagateException()
        {
            // Arrange
            var moduleName = SystemModules.Reports;
            var request = new EnableModuleRequest { Reason = "Test reason" };

            _mockModuleConfigService
                .Setup(s => s.EnableModuleAsync(_testClinicId, moduleName, "test-user", request.Reason))
                .ThrowsAsync(new InvalidOperationException("Module not available"));

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _controller.EnableModuleWithReason(moduleName, request)
            );
        }

        #endregion
    }
}
