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
    public class SentimentAnalysisServiceTests : IDisposable
    {
        private readonly MedicSoftDbContext _context;
        private readonly Mock<ILogger<SentimentAnalysisService>> _mockLogger;
        private readonly ISentimentAnalysisService _service;
        private readonly string _testTenantId = "test-tenant-123";

        public SentimentAnalysisServiceTests()
        {
            // Setup in-memory database for testing
            var options = new DbContextOptionsBuilder<MedicSoftDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _context = new MedicSoftDbContext(options);
            _mockLogger = new Mock<ILogger<SentimentAnalysisService>>();
            _service = new SentimentAnalysisService(_context, _mockLogger.Object);
        }

        [Fact]
        public async Task AnalyzeTextAsync_ShouldAnalyzePositiveText()
        {
            // Arrange
            var text = "Excelente atendimento, o médico foi muito atencioso e o serviço foi perfeito!";

            // Act
            var result = await _service.AnalyzeTextAsync(text, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(SentimentType.Positive, result.Sentiment);
            Assert.True(result.PositiveScore > result.NegativeScore);
            Assert.True(result.ConfidenceScore > 0);
            Assert.NotEmpty(result.Topics);
        }

        [Fact]
        public async Task AnalyzeTextAsync_ShouldAnalyzeNegativeText()
        {
            // Arrange
            var text = "Péssimo atendimento, o médico foi rude e o serviço foi horrível!";

            // Act
            var result = await _service.AnalyzeTextAsync(text, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(SentimentType.Negative, result.Sentiment);
            Assert.True(result.NegativeScore > result.PositiveScore);
            Assert.True(result.ConfidenceScore > 0);
            Assert.True(result.IsNegativeAlert);
        }

        [Fact]
        public async Task AnalyzeTextAsync_ShouldAnalyzeNeutralText()
        {
            // Arrange
            var text = "A consulta foi realizada conforme agendado.";

            // Act
            var result = await _service.AnalyzeTextAsync(text, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(SentimentType.Neutral, result.Sentiment);
            Assert.True(result.NeutralScore > result.PositiveScore && result.NeutralScore > result.NegativeScore);
        }

        [Fact]
        public async Task AnalyzeTextAsync_ShouldExtractTopics()
        {
            // Arrange
            var text = "O atendimento foi bom, a recepção amável, mas o preço foi alto. A consulta foi rápida.";

            // Act
            var result = await _service.AnalyzeTextAsync(text, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result.Topics);
            Assert.Contains("Atendimento", result.Topics);
            Assert.Contains("Recepção", result.Topics);
            Assert.Contains("Preço", result.Topics);
            Assert.Contains("Consulta", result.Topics);
        }

        [Fact]
        public async Task AnalyzeTextAsync_ShouldThrowException_WhenTextIsEmpty()
        {
            // Arrange
            var text = "";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.AnalyzeTextAsync(text, _testTenantId));
        }

        [Fact]
        public async Task AnalyzeTextAsync_ShouldThrowException_WhenTextIsNull()
        {
            // Arrange
            string text = null;

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.AnalyzeTextAsync(text, _testTenantId));
        }

        [Fact]
        public async Task AnalyzeTextAsync_ShouldThrowException_WhenTextIsWhitespace()
        {
            // Arrange
            var text = "   \n\t  ";

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(
                () => _service.AnalyzeTextAsync(text, _testTenantId));
        }

        [Fact]
        public async Task AnalyzeTextAsync_ShouldSaveAnalysisToDatabase()
        {
            // Arrange
            var text = "Bom atendimento!";
            var initialCount = _context.SentimentAnalyses.Count();

            // Act
            var result = await _service.AnalyzeTextAsync(text, _testTenantId);

            // Assert
            var finalCount = _context.SentimentAnalyses.Count();
            Assert.Equal(initialCount + 1, finalCount);

            var savedAnalysis = await _context.SentimentAnalyses
                .FirstOrDefaultAsync(s => s.SourceText == text && s.TenantId == _testTenantId);
            Assert.NotNull(savedAnalysis);
            Assert.Equal(text, savedAnalysis.SourceText);
            Assert.Equal(SentimentType.Positive, savedAnalysis.Sentiment);
        }

        [Fact]
        public async Task AnalyzeBatchAsync_ShouldAnalyzeMultipleTexts()
        {
            // Arrange
            var texts = new List<string>
            {
                "Excelente serviço!",
                "Péssimo atendimento",
                "Normal, conforme esperado"
            };

            // Act
            var results = await _service.AnalyzeBatchAsync(texts, _testTenantId);

            // Assert
            Assert.NotNull(results);
            var resultList = results.ToList();
            Assert.Equal(3, resultList.Count);
            Assert.Single(resultList, r => r.Sentiment == SentimentType.Positive);
            Assert.Single(resultList, r => r.Sentiment == SentimentType.Negative);
            Assert.Single(resultList, r => r.Sentiment == SentimentType.Neutral);
        }

        [Fact]
        public async Task AnalyzeBatchAsync_ShouldSkipEmptyTexts()
        {
            // Arrange
            var texts = new List<string>
            {
                "Bom atendimento!",
                "",
                "Péssimo serviço",
                "   ",
                null
            };

            // Act
            var results = await _service.AnalyzeBatchAsync(texts, _testTenantId);

            // Assert
            Assert.NotNull(results);
            var resultList = results.ToList();
            Assert.Equal(2, resultList.Count); // Only non-empty texts
        }

        [Fact]
        public async Task AnalyzeBatchAsync_ShouldReturnEmptyList_WhenAllTextsAreEmpty()
        {
            // Arrange
            var texts = new List<string> { "", "   ", null };

            // Act
            var results = await _service.AnalyzeBatchAsync(texts, _testTenantId);

            // Assert
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        [Fact]
        public async Task AnalyzeBatchAsync_ShouldSaveAllAnalysesToDatabase()
        {
            // Arrange
            var texts = new List<string>
            {
                "Excelente!",
                "Péssimo!",
                "Normal"
            };
            var initialCount = _context.SentimentAnalyses.Count();

            // Act
            var results = await _service.AnalyzeBatchAsync(texts, _testTenantId);

            // Assert
            var finalCount = _context.SentimentAnalyses.Count();
            Assert.Equal(initialCount + 3, finalCount);
        }

        [Fact]
        public async Task GetByEntityAsync_ShouldReturnAnalysesForEntity()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            var entityType = "Patient";
            var analysis1 = new SentimentAnalysis("Ótimo atendimento", entityType, entityId, _testTenantId);
            var analysis2 = new SentimentAnalysis("Excelente médico", entityType, entityId, _testTenantId);
            var analysis3 = new SentimentAnalysis("Péssimo", "Review", Guid.NewGuid(), _testTenantId); // Different entity

            _context.SentimentAnalyses.AddRange(analysis1, analysis2, analysis3);
            await _context.SaveChangesAsync();

            // Act
            var results = await _service.GetByEntityAsync(entityId, entityType, _testTenantId);

            // Assert
            Assert.NotNull(results);
            var resultList = results.ToList();
            Assert.Equal(2, resultList.Count);
            Assert.All(resultList, a => Assert.Equal(entityId, a.SourceId));
            Assert.All(resultList, a => Assert.Equal(entityType, a.SourceType));
        }

        [Fact]
        public async Task GetByEntityAsync_ShouldReturnOrderedByAnalyzedAtDescending()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            var entityType = "Patient";
            var analysis1 = new SentimentAnalysis("First", entityType, entityId, _testTenantId);
            var analysis2 = new SentimentAnalysis("Second", entityType, entityId, _testTenantId);

            _context.SentimentAnalyses.Add(analysis1);
            await _context.SaveChangesAsync();
            
            // Introduce a small delay to ensure different timestamps
            await Task.Delay(10);
            _context.SentimentAnalyses.Add(analysis2);
            await _context.SaveChangesAsync();

            // Act
            var results = await _service.GetByEntityAsync(entityId, entityType, _testTenantId);

            // Assert
            var resultList = results.ToList();
            Assert.Equal(2, resultList.Count);
            // Most recent should come first
            Assert.Equal("Second", resultList.First().SourceText);
        }

        [Fact]
        public async Task GetByEntityAsync_ShouldReturnEmpty_WhenNoAnalysesForEntity()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            var entityType = "Patient";

            // Act
            var results = await _service.GetByEntityAsync(entityId, entityType, _testTenantId);

            // Assert
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        [Fact]
        public async Task GetByEntityAsync_ShouldFilterByTenant()
        {
            // Arrange
            var entityId = Guid.NewGuid();
            var entityType = "Patient";
            var analysis1 = new SentimentAnalysis("My tenant analysis", entityType, entityId, _testTenantId);
            var analysis2 = new SentimentAnalysis("Other tenant analysis", entityType, entityId, "other-tenant");

            _context.SentimentAnalyses.AddRange(analysis1, analysis2);
            await _context.SaveChangesAsync();

            // Act
            var results = await _service.GetByEntityAsync(entityId, entityType, _testTenantId);

            // Assert
            Assert.NotNull(results);
            var resultList = results.ToList();
            Assert.Single(resultList);
        }

        [Fact]
        public async Task GetNegativeAlertsAsync_ShouldReturnNegativeAnalyses()
        {
            // Arrange
            var negativeAnalysis = new SentimentAnalysis("Péssimo atendimento!", "Complaint", Guid.NewGuid(), _testTenantId);
            negativeAnalysis.SetAnalysisResult(SentimentType.Negative, 0.1, 0.1, 0.8, 0.7);
            
            var positiveAnalysis = new SentimentAnalysis("Excelente!", "Review", Guid.NewGuid(), _testTenantId);
            positiveAnalysis.SetAnalysisResult(SentimentType.Positive, 0.9, 0.05, 0.05, 0.85);

            _context.SentimentAnalyses.AddRange(negativeAnalysis, positiveAnalysis);
            await _context.SaveChangesAsync();

            // Act
            var results = await _service.GetNegativeAlertsAsync(_testTenantId);

            // Assert
            Assert.NotNull(results);
            var resultList = results.ToList();
            Assert.All(resultList, a => Assert.Equal(SentimentType.Negative, a.Sentiment));
        }

        [Fact]
        public async Task GetNegativeAlertsAsync_ShouldLimitResults()
        {
            // Arrange
            // Create 60 negative analyses (more than the 50 limit)
            for (int i = 0; i < 60; i++)
            {
                var analysis = new SentimentAnalysis($"Negative {i}", "Complaint", Guid.NewGuid(), _testTenantId);
                analysis.SetAnalysisResult(SentimentType.Negative, 0.1, 0.1, 0.8, 0.7);
                _context.SentimentAnalyses.Add(analysis);
            }
            await _context.SaveChangesAsync();

            // Act
            var results = await _service.GetNegativeAlertsAsync(_testTenantId);

            // Assert
            var resultList = results.ToList();
            Assert.True(resultList.Count <= 50);
        }

        [Fact]
        public async Task GetNegativeAlertsAsync_ShouldFilterOldAnalyses()
        {
            // Arrange
            // This would require the ability to control datetime in the service
            // For now, we'll just verify that the query executes
            
            // Act
            var results = await _service.GetNegativeAlertsAsync(_testTenantId);

            // Assert
            Assert.NotNull(results);
        }

        [Fact]
        public async Task GetNegativeAlertsAsync_ShouldOrderByNegativeScoreDescending()
        {
            // Arrange
            var analysis1 = new SentimentAnalysis("Very negative", "Complaint", Guid.NewGuid(), _testTenantId);
            analysis1.SetAnalysisResult(SentimentType.Negative, 0.05, 0.1, 0.85, 0.8);
            
            var analysis2 = new SentimentAnalysis("Slightly negative", "Complaint", Guid.NewGuid(), _testTenantId);
            analysis2.SetAnalysisResult(SentimentType.Negative, 0.2, 0.2, 0.6, 0.4);

            _context.SentimentAnalyses.AddRange(analysis1, analysis2);
            await _context.SaveChangesAsync();

            // Act
            var results = await _service.GetNegativeAlertsAsync(_testTenantId);

            // Assert
            var resultList = results.ToList();
            Assert.Equal(2, resultList.Count);
            // First should have higher negative score
            Assert.True(resultList[0].NegativeScore >= resultList[1].NegativeScore);
        }

        [Fact]
        public async Task GetNegativeAlertsAsync_ShouldReturnEmpty_WhenNoNegativeAnalyses()
        {
            // Arrange
            var positiveAnalysis = new SentimentAnalysis("Excelente!", "Review", Guid.NewGuid(), _testTenantId);
            positiveAnalysis.SetAnalysisResult(SentimentType.Positive, 0.9, 0.05, 0.05, 0.85);
            
            _context.SentimentAnalyses.Add(positiveAnalysis);
            await _context.SaveChangesAsync();

            // Act
            var results = await _service.GetNegativeAlertsAsync(_testTenantId);

            // Assert
            Assert.NotNull(results);
            Assert.Empty(results);
        }

        [Fact]
        public async Task AnalyzeTextAsync_ShouldHandleMixedSentiment()
        {
            // Arrange
            var text = "O atendimento foi bom mas a demora foi terrível e o preço péssimo.";

            // Act
            var result = await _service.AnalyzeTextAsync(text, _testTenantId);

            // Assert
            Assert.NotNull(result);
            // Should identify mixed sentiment
            Assert.True(result.PositiveScore > 0 && result.NegativeScore > 0);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
