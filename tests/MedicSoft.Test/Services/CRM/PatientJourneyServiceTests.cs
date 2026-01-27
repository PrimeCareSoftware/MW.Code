using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using MedicSoft.Api.Services.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Repository.Context;
using Xunit;

namespace MedicSoft.Test.Services.CRM
{
    public class PatientJourneyServiceTests : IDisposable
    {
        private readonly MedicSoftDbContext _context;
        private readonly Mock<ILogger<PatientJourneyService>> _mockLogger;
        private readonly IPatientJourneyService _service;
        private readonly string _testTenantId = "test-tenant-123";

        public PatientJourneyServiceTests()
        {
            // Setup in-memory database for testing
            var options = new DbContextOptionsBuilder<MedicSoftDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _context = new MedicSoftDbContext(options);
            _mockLogger = new Mock<ILogger<PatientJourneyService>>();
            _service = new PatientJourneyService(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task GetOrCreateJourneyAsync_ShouldCreateNewJourney_WhenNotExists()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();

            // Act
            var result = await _service.GetOrCreateJourneyAsync(pacienteId, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(pacienteId, result.PacienteId);
            Assert.Equal(JourneyStageEnum.Descoberta, result.CurrentStage);
            Assert.NotEmpty(result.Stages);
            Assert.Equal(JourneyStageEnum.Descoberta, result.Stages.First().Stage);
        }

        [Fact]
        public async Task GetOrCreateJourneyAsync_ShouldReturnExisting_WhenExists()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var existingJourney = new PatientJourney(pacienteId, _testTenantId);
            _context.PatientJourneys.Add(existingJourney);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetOrCreateJourneyAsync(pacienteId, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(existingJourney.Id, result.Id);
            Assert.Equal(pacienteId, result.PacienteId);
        }

        [Fact]
        public async Task AdvanceStageAsync_ShouldMoveToNextStage()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var journey = await _service.GetOrCreateJourneyAsync(pacienteId, _testTenantId);
            Assert.Equal(JourneyStageEnum.Descoberta, journey.CurrentStage);

            // Act
            await _service.AdvanceStageAsync(
                pacienteId,
                JourneyStageEnum.Consideracao,
                "Paciente pesquisou serviços",
                _testTenantId);

            // Assert
            var updated = await _service.GetOrCreateJourneyAsync(pacienteId, _testTenantId);
            Assert.Equal(JourneyStageEnum.Consideracao, updated.CurrentStage);
            Assert.Equal(2, updated.Stages.Count);
        }

        [Fact]
        public async Task AddTouchpointAsync_ShouldAddTouchpointToCurrentStage()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var journey = await _service.GetOrCreateJourneyAsync(pacienteId, _testTenantId);

            // Act
            await _service.AddTouchpointAsync(
                pacienteId,
                TouchpointType.EmailInteraction,
                "Email",
                "Email de boas-vindas enviado",
                TouchpointDirection.Outbound,
                _testTenantId);

            // Assert
            var updated = await _service.GetOrCreateJourneyAsync(pacienteId, _testTenantId);
            Assert.True(updated.TotalTouchpoints > 0);
            var currentStage = updated.Stages.First(s => s.Stage == updated.CurrentStage);
            Assert.NotEmpty(currentStage.Touchpoints);
        }

        [Fact]
        public async Task RecalculateMetricsAsync_ShouldUpdateJourneyMetrics()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var journey = await _service.GetOrCreateJourneyAsync(pacienteId, _testTenantId);

            // Add some touchpoints
            await _service.AddTouchpointAsync(
                pacienteId, TouchpointType.EmailInteraction, "Email", "Test", TouchpointDirection.Outbound, _testTenantId);
            await _service.AddTouchpointAsync(
                pacienteId, TouchpointType.PhoneCall, "Phone", "Test", TouchpointDirection.Inbound, _testTenantId);

            // Act
            await _service.RecalculateMetricsAsync(pacienteId, _testTenantId);

            // Assert
            var updated = await _service.GetOrCreateJourneyAsync(pacienteId, _testTenantId);
            Assert.Equal(2, updated.TotalTouchpoints);
        }

        [Fact]
        public async Task UpdateMetricsAsync_ShouldUpdateMetricsManually()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var journey = await _service.GetOrCreateJourneyAsync(pacienteId, _testTenantId);

            // Act
            await _service.UpdateMetricsAsync(
                pacienteId,
                lifetimeValue: 1500.00m,
                npsScore: 9,
                satisfactionScore: 4.5,
                churnRisk: ChurnRiskLevel.Low,
                _testTenantId);

            // Assert
            var updated = await _service.GetOrCreateJourneyAsync(pacienteId, _testTenantId);
            Assert.Equal(1500.00m, updated.LifetimeValue);
            Assert.Equal(9, updated.NpsScore);
            Assert.Equal(4.5, updated.SatisfactionScore);
            Assert.Equal(ChurnRiskLevel.Low, updated.ChurnRisk);
        }

        [Fact]
        public async Task GetMetricsAsync_ShouldReturnJourneyMetrics()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var journey = await _service.GetOrCreateJourneyAsync(pacienteId, _testTenantId);
            
            await _service.UpdateMetricsAsync(
                pacienteId, 2000.00m, 10, 5.0, ChurnRiskLevel.Low, _testTenantId);

            // Act
            var metrics = await _service.GetMetricsAsync(pacienteId, _testTenantId);

            // Assert
            Assert.NotNull(metrics);
            Assert.Equal(2000.00m, metrics.LifetimeValue);
            Assert.Equal(10, metrics.NpsScore);
            Assert.Equal(5.0, metrics.SatisfactionScore);
            Assert.Equal(ChurnRiskLevel.Low, metrics.ChurnRisk);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
