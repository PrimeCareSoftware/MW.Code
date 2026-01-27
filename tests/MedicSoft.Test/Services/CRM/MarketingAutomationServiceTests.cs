using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MedicSoft.Api.Services.CRM;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Repository.Context;
using Xunit;

namespace MedicSoft.Test.Services.CRM
{
    public class MarketingAutomationServiceTests : IDisposable
    {
        private readonly MedicSoftDbContext _context;
        private readonly Mock<ILogger<MarketingAutomationService>> _mockLogger;
        private readonly Mock<IAutomationEngine> _mockAutomationEngine;
        private readonly IMarketingAutomationService _service;
        private readonly string _testTenantId = "test-tenant-123";

        public MarketingAutomationServiceTests()
        {
            // Setup in-memory database for testing
            var options = new DbContextOptionsBuilder<MedicSoftDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _context = new MedicSoftDbContext(options);
            _mockLogger = new Mock<ILogger<MarketingAutomationService>>();
            _mockAutomationEngine = new Mock<IAutomationEngine>();
            _service = new MarketingAutomationService(_context, _mockLogger.Object, _mockAutomationEngine.Object);
        }

        [Fact]
        public async Task CreateAsync_ShouldCreateNewAutomation_WithAllProperties()
        {
            // Arrange
            var createDto = new CreateMarketingAutomationDto
            {
                Name = "Welcome Email Campaign",
                Description = "Send welcome email to new patients",
                TriggerType = AutomationTriggerType.StageChange,
                TriggerStage = JourneyStageEnum.Descoberta,
                TriggerEvent = "patient_registered",
                DelayMinutes = 15,
                SegmentFilter = "{'age': {'$gte': 18}}",
                Tags = new List<string> { "welcome", "onboarding" },
                Actions = new List<CreateAutomationActionDto>
                {
                    new CreateAutomationActionDto
                    {
                        Order = 1,
                        Type = ActionType.SendEmail,
                        EmailTemplateId = Guid.NewGuid(),
                        Channel = "Email"
                    }
                }
            };

            // Act
            var result = await _service.CreateAsync(createDto, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createDto.Name, result.Name);
            Assert.Equal(createDto.Description, result.Description);
            Assert.Equal(createDto.TriggerType, result.TriggerType);
            Assert.Equal(createDto.TriggerStage, result.TriggerStage);
            Assert.Equal(createDto.TriggerEvent, result.TriggerEvent);
            Assert.Equal(createDto.DelayMinutes, result.DelayMinutes);
            Assert.Equal(createDto.SegmentFilter, result.SegmentFilter);
            Assert.NotEmpty(result.Tags);
            Assert.Contains("welcome", result.Tags);
            Assert.False(result.IsActive); // Should start inactive
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateAutomationProperties()
        {
            // Arrange
            var automation = new MarketingAutomation(
                "Old Name",
                "Old Description",
                AutomationTriggerType.StageChange,
                _testTenantId);
            _context.MarketingAutomations.Add(automation);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateMarketingAutomationDto
            {
                TriggerStage = JourneyStageEnum.Consideracao,
                TriggerEvent = "updated_event",
                DelayMinutes = 30,
                SegmentFilter = "{'new': 'filter'}",
                Tags = new List<string> { "updated", "tag" },
                Actions = new List<CreateAutomationActionDto>()
            };

            // Act
            var result = await _service.UpdateAsync(automation.Id, updateDto, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateDto.TriggerStage, result.TriggerStage);
            Assert.Equal(updateDto.TriggerEvent, result.TriggerEvent);
            Assert.Equal(updateDto.DelayMinutes, result.DelayMinutes);
            Assert.Equal(updateDto.SegmentFilter, result.SegmentFilter);
            Assert.Contains("updated", result.Tags);
        }

        [Fact]
        public async Task UpdateAsync_ShouldThrowException_WhenAutomationNotFound()
        {
            // Arrange
            var updateDto = new UpdateMarketingAutomationDto
            {
                TriggerEvent = "test"
            };
            var nonExistentId = Guid.NewGuid();

            // Act & Assert
            await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _service.UpdateAsync(nonExistentId, updateDto, _testTenantId));
        }

        [Fact]
        public async Task DeleteAsync_ShouldDeleteAutomation()
        {
            // Arrange
            var automation = new MarketingAutomation(
                "Test Automation",
                "Description",
                AutomationTriggerType.Event,
                _testTenantId);
            _context.MarketingAutomations.Add(automation);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeleteAsync(automation.Id, _testTenantId);

            // Assert
            Assert.True(result);
            var deletedAutomation = await _context.MarketingAutomations
                .FirstOrDefaultAsync(a => a.Id == automation.Id);
            Assert.Null(deletedAutomation);
        }

        [Fact]
        public async Task DeleteAsync_ShouldReturnFalse_WhenAutomationNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _service.DeleteAsync(nonExistentId, _testTenantId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnAutomation_WhenExists()
        {
            // Arrange
            var automation = new MarketingAutomation(
                "Test Automation",
                "Description",
                AutomationTriggerType.StageChange,
                _testTenantId);
            automation.AddTag("important");
            _context.MarketingAutomations.Add(automation);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetByIdAsync(automation.Id, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(automation.Id, result.Id);
            Assert.Equal(automation.Name, result.Name);
            Assert.Contains("important", result.Tags);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnNull_WhenNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _service.GetByIdAsync(nonExistentId, _testTenantId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllAutomationsForTenant()
        {
            // Arrange
            var automation1 = new MarketingAutomation("Campaign 1", "Desc 1", AutomationTriggerType.Event, _testTenantId);
            var automation2 = new MarketingAutomation("Campaign 2", "Desc 2", AutomationTriggerType.Scheduled, _testTenantId);
            var automation3 = new MarketingAutomation("Campaign 3", "Desc 3", AutomationTriggerType.Event, "other-tenant");

            _context.MarketingAutomations.AddRange(automation1, automation2, automation3);
            await _context.SaveChangesAsync();

            // Act
            var results = await _service.GetAllAsync(_testTenantId);

            // Assert
            Assert.NotNull(results);
            var resultList = results.ToList();
            Assert.Equal(2, resultList.Count);
            // Verify all results belong to the test tenant (not other-tenant)
            Assert.DoesNotContain(resultList, a => a.Name == "Campaign 3");
        }

        [Fact]
        public async Task GetActiveAsync_ShouldReturnOnlyActiveAutomations()
        {
            // Arrange
            var activeAutomation1 = new MarketingAutomation("Active 1", "Desc", AutomationTriggerType.Event, _testTenantId);
            activeAutomation1.Activate();
            var activeAutomation2 = new MarketingAutomation("Active 2", "Desc", AutomationTriggerType.Scheduled, _testTenantId);
            activeAutomation2.Activate();
            var inactiveAutomation = new MarketingAutomation("Inactive", "Desc", AutomationTriggerType.Event, _testTenantId);

            _context.MarketingAutomations.AddRange(activeAutomation1, activeAutomation2, inactiveAutomation);
            await _context.SaveChangesAsync();

            // Act
            var results = await _service.GetActiveAsync(_testTenantId);

            // Assert
            Assert.NotNull(results);
            var resultList = results.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.All(resultList, a => Assert.True(a.IsActive));
        }

        [Fact]
        public async Task ActivateAsync_ShouldActivateAutomation()
        {
            // Arrange
            var automation = new MarketingAutomation(
                "Test Automation",
                "Description",
                AutomationTriggerType.Event,
                _testTenantId);
            _context.MarketingAutomations.Add(automation);
            await _context.SaveChangesAsync();
            Assert.False(automation.IsActive);

            // Act
            var result = await _service.ActivateAsync(automation.Id, _testTenantId);

            // Assert
            Assert.True(result);
            var updated = await _context.MarketingAutomations.FirstOrDefaultAsync(a => a.Id == automation.Id);
            Assert.NotNull(updated);
            Assert.True(updated.IsActive);
        }

        [Fact]
        public async Task DeactivateAsync_ShouldDeactivateAutomation()
        {
            // Arrange
            var automation = new MarketingAutomation(
                "Test Automation",
                "Description",
                AutomationTriggerType.Event,
                _testTenantId);
            automation.Activate();
            _context.MarketingAutomations.Add(automation);
            await _context.SaveChangesAsync();
            Assert.True(automation.IsActive);

            // Act
            var result = await _service.DeactivateAsync(automation.Id, _testTenantId);

            // Assert
            Assert.True(result);
            var updated = await _context.MarketingAutomations.FirstOrDefaultAsync(a => a.Id == automation.Id);
            Assert.NotNull(updated);
            Assert.False(updated.IsActive);
        }

        [Fact]
        public async Task ActivateAsync_ShouldReturnFalse_WhenAutomationNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _service.ActivateAsync(nonExistentId, _testTenantId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task DeactivateAsync_ShouldReturnFalse_WhenAutomationNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _service.DeactivateAsync(nonExistentId, _testTenantId);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public async Task GetMetricsAsync_ShouldReturnAutomationMetrics()
        {
            // Arrange
            var automation = new MarketingAutomation(
                "Test Automation",
                "Description",
                AutomationTriggerType.Event,
                _testTenantId);
            automation.Activate();
            _context.MarketingAutomations.Add(automation);
            await _context.SaveChangesAsync();

            // Act
            var metrics = await _service.GetMetricsAsync(automation.Id, _testTenantId);

            // Assert
            Assert.NotNull(metrics);
            Assert.Equal(automation.Id, metrics.AutomationId);
            Assert.Equal(automation.Name, metrics.Name);
            Assert.Equal(automation.TimesExecuted, metrics.TimesExecuted);
            Assert.Equal(automation.SuccessRate, metrics.SuccessRate);
        }

        [Fact]
        public async Task GetMetricsAsync_ShouldReturnNull_WhenAutomationNotFound()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid();

            // Act
            var result = await _service.GetMetricsAsync(nonExistentId, _testTenantId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllMetricsAsync_ShouldReturnAllAutomationMetrics()
        {
            // Arrange
            var automation1 = new MarketingAutomation("Campaign 1", "Desc 1", AutomationTriggerType.Event, _testTenantId);
            var automation2 = new MarketingAutomation("Campaign 2", "Desc 2", AutomationTriggerType.Scheduled, _testTenantId);
            var automation3 = new MarketingAutomation("Campaign 3", "Desc 3", AutomationTriggerType.Event, "other-tenant");

            _context.MarketingAutomations.AddRange(automation1, automation2, automation3);
            await _context.SaveChangesAsync();

            // Act
            var results = await _service.GetAllMetricsAsync(_testTenantId);

            // Assert
            Assert.NotNull(results);
            var resultList = results.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.All(resultList, m => Assert.NotNull(m));
        }

        [Fact]
        public async Task TriggerAutomationAsync_ShouldCallAutomationEngine_WhenActive()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var automation = new MarketingAutomation(
                "Test Automation",
                "Description",
                AutomationTriggerType.Event,
                _testTenantId);
            automation.Activate();
            _context.MarketingAutomations.Add(automation);
            await _context.SaveChangesAsync();

            _mockAutomationEngine
                .Setup(x => x.ExecuteAutomationAsync(It.IsAny<MarketingAutomation>(), patientId, _testTenantId))
                .Returns(Task.CompletedTask);

            // Act
            await _service.TriggerAutomationAsync(automation.Id, patientId, _testTenantId);

            // Assert
            _mockAutomationEngine.Verify(
                x => x.ExecuteAutomationAsync(It.IsAny<MarketingAutomation>(), patientId, _testTenantId),
                Times.Once);
        }

        [Fact]
        public async Task TriggerAutomationAsync_ShouldNotCallEngine_WhenInactive()
        {
            // Arrange
            var patientId = Guid.NewGuid();
            var automation = new MarketingAutomation(
                "Test Automation",
                "Description",
                AutomationTriggerType.Event,
                _testTenantId);
            // Automation is inactive by default
            _context.MarketingAutomations.Add(automation);
            await _context.SaveChangesAsync();

            // Act
            await _service.TriggerAutomationAsync(automation.Id, patientId, _testTenantId);

            // Assert
            _mockAutomationEngine.Verify(
                x => x.ExecuteAutomationAsync(It.IsAny<MarketingAutomation>(), It.IsAny<Guid>(), It.IsAny<string>()),
                Times.Never);
        }

        [Fact]
        public async Task CreateAsync_ShouldHandleEmptyActionsList()
        {
            // Arrange
            var createDto = new CreateMarketingAutomationDto
            {
                Name = "Empty Actions Campaign",
                Description = "Campaign with no actions",
                TriggerType = AutomationTriggerType.StageChange,
                TriggerStage = JourneyStageEnum.Descoberta,
                Tags = new List<string>(),
                Actions = new List<CreateAutomationActionDto>()
            };

            // Act
            var result = await _service.CreateAsync(createDto, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(createDto.Name, result.Name);
            Assert.Empty(result.Actions);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnEmptyList_WhenNoAutomationsForTenant()
        {
            // Arrange
            var otherTenant = "other-tenant-456";

            // Act
            var results = await _service.GetAllAsync(otherTenant);

            // Assert
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
