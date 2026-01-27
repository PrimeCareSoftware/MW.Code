using System;
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
    public class ComplaintServiceTests : IDisposable
    {
        private readonly MedicSoftDbContext _context;
        private readonly Mock<ILogger<ComplaintService>> _mockLogger;
        private readonly IComplaintService _service;
        private readonly string _testTenantId = "test-tenant-123";

        public ComplaintServiceTests()
        {
            var options = new DbContextOptionsBuilder<MedicSoftDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _context = new MedicSoftDbContext(options);
            _mockLogger = new Mock<ILogger<ComplaintService>>();
            _service = new ComplaintService(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateComplaintAsync_ShouldGenerateProtocolNumber()
        {
            // Arrange
            var pacienteId = Guid.NewGuid();
            var subject = "Demora no atendimento";
            var description = "Aguardei mais de 2 horas para ser atendido";

            // Act
            var result = await _service.CreateComplaintAsync(
                pacienteId,
                subject,
                description,
                ComplaintCategory.Atendimento,
                ComplaintPriority.Medium,
                _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.ProtocolNumber);
            Assert.StartsWith("CMP-", result.ProtocolNumber);
            Assert.Equal(subject, result.Subject);
            Assert.Equal(ComplaintStatus.Received, result.Status);
        }

        [Fact]
        public async Task GetByProtocolAsync_ShouldFindComplaint()
        {
            // Arrange
            var complaint = await _service.CreateComplaintAsync(
                Guid.NewGuid(),
                "Test",
                "Description",
                ComplaintCategory.Qualidade,
                ComplaintPriority.High,
                _testTenantId);

            // Act
            var result = await _service.GetByProtocolAsync(complaint.ProtocolNumber, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(complaint.Id, result.Id);
            Assert.Equal(complaint.ProtocolNumber, result.ProtocolNumber);
        }

        [Fact]
        public async Task UpdateStatusAsync_ShouldUpdateComplaintStatus()
        {
            // Arrange
            var complaint = await _service.CreateComplaintAsync(
                Guid.NewGuid(),
                "Test",
                "Description",
                ComplaintCategory.Instalacoes,
                ComplaintPriority.Low,
                _testTenantId);

            // Act
            await _service.UpdateStatusAsync(
                complaint.Id,
                ComplaintStatus.InProgress,
                "Iniciando análise da reclamação",
                _testTenantId);

            // Assert
            var updated = await _service.GetByIdAsync(complaint.Id, _testTenantId);
            Assert.Equal(ComplaintStatus.InProgress, updated.Status);
            Assert.NotNull(updated.FirstResponseAt);
        }

        [Fact]
        public async Task AssignComplaintAsync_ShouldAssignToUser()
        {
            // Arrange
            var complaint = await _service.CreateComplaintAsync(
                Guid.NewGuid(),
                "Test",
                "Description",
                ComplaintCategory.Financeiro,
                ComplaintPriority.Medium,
                _testTenantId);

            var assignedToId = Guid.NewGuid();

            // Act
            await _service.AssignComplaintAsync(
                complaint.Id,
                assignedToId,
                _testTenantId);

            // Assert
            var updated = await _service.GetByIdAsync(complaint.Id, _testTenantId);
            Assert.Equal(assignedToId, updated.AssignedToId);
        }

        [Fact]
        public async Task AddInteractionAsync_ShouldAddInteractionToComplaint()
        {
            // Arrange
            var complaint = await _service.CreateComplaintAsync(
                Guid.NewGuid(),
                "Test",
                "Description",
                ComplaintCategory.Atendimento,
                ComplaintPriority.High,
                _testTenantId);

            var userId = Guid.NewGuid();
            var message = "Estamos analisando sua reclamação";

            // Act
            await _service.AddInteractionAsync(
                complaint.Id,
                userId,
                message,
                isInternal: false,
                _testTenantId);

            // Assert
            var updated = await _service.GetByIdAsync(complaint.Id, _testTenantId);
            Assert.NotEmpty(updated.Interactions);
            Assert.Equal(message, updated.Interactions.First().Message);
        }

        [Fact]
        public async Task GetDashboardAsync_ShouldCalculateMetrics()
        {
            // Arrange
            // Create different complaints with different statuses
            await _service.CreateComplaintAsync(
                Guid.NewGuid(), "Test 1", "Desc", ComplaintCategory.Atendimento, ComplaintPriority.High, _testTenantId);
            await _service.CreateComplaintAsync(
                Guid.NewGuid(), "Test 2", "Desc", ComplaintCategory.Qualidade, ComplaintPriority.Medium, _testTenantId);
            await _service.CreateComplaintAsync(
                Guid.NewGuid(), "Test 3", "Desc", ComplaintCategory.Instalacoes, ComplaintPriority.Low, _testTenantId);

            // Act
            var dashboard = await _service.GetDashboardAsync(_testTenantId);

            // Assert
            Assert.Equal(3, dashboard.TotalComplaints);
            Assert.Equal(3, dashboard.OpenComplaints);
            Assert.Equal(0, dashboard.ClosedComplaints);
        }

        [Fact]
        public async Task GetByCategoryAsync_ShouldFilterByCategory()
        {
            // Arrange
            await _service.CreateComplaintAsync(
                Guid.NewGuid(), "Test 1", "Desc", ComplaintCategory.Atendimento, ComplaintPriority.High, _testTenantId);
            await _service.CreateComplaintAsync(
                Guid.NewGuid(), "Test 2", "Desc", ComplaintCategory.Atendimento, ComplaintPriority.Medium, _testTenantId);
            await _service.CreateComplaintAsync(
                Guid.NewGuid(), "Test 3", "Desc", ComplaintCategory.Qualidade, ComplaintPriority.Low, _testTenantId);

            // Act
            var atendimentoComplaints = await _service.GetByCategoryAsync(ComplaintCategory.Atendimento, _testTenantId);

            // Assert
            Assert.Equal(2, atendimentoComplaints.Count);
            Assert.All(atendimentoComplaints, c => Assert.Equal(ComplaintCategory.Atendimento, c.Category));
        }

        [Fact]
        public async Task CompleteComplaintAsync_ShouldSetResolvedAt()
        {
            // Arrange
            var complaint = await _service.CreateComplaintAsync(
                Guid.NewGuid(),
                "Test",
                "Description",
                ComplaintCategory.Atendimento,
                ComplaintPriority.Medium,
                _testTenantId);

            // Act
            await _service.UpdateStatusAsync(
                complaint.Id,
                ComplaintStatus.Resolved,
                "Problema resolvido",
                _testTenantId);

            // Assert
            var updated = await _service.GetByIdAsync(complaint.Id, _testTenantId);
            Assert.Equal(ComplaintStatus.Resolved, updated.Status);
            Assert.NotNull(updated.ResolvedAt);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
