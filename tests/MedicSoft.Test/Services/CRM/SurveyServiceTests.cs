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
    public class SurveyServiceTests : IDisposable
    {
        private readonly MedicSoftDbContext _context;
        private readonly Mock<ILogger<SurveyService>> _mockLogger;
        private readonly Mock<IEmailService> _mockEmailService;
        private readonly Mock<ISmsService> _mockSmsService;
        private readonly Mock<IWhatsAppService> _mockWhatsAppService;
        private readonly ISurveyService _service;
        private readonly string _testTenantId = "test-tenant-123";

        public SurveyServiceTests()
        {
            var options = new DbContextOptionsBuilder<MedicSoftDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _context = new MedicSoftDbContext(options);
            _mockLogger = new Mock<ILogger<SurveyService>>();
            _mockEmailService = new Mock<IEmailService>();
            _mockSmsService = new Mock<ISmsService>();
            _mockWhatsAppService = new Mock<IWhatsAppService>();

            _service = new SurveyService(
                _context,
                _mockLogger.Object,
                _mockEmailService.Object,
                _mockSmsService.Object,
                _mockWhatsAppService.Object);
        }

        [Fact]
        public async Task CreateSurveyAsync_ShouldCreateNpsSurvey()
        {
            // Arrange
            var surveyName = "NPS Pós-Consulta";
            var description = "Pesquisa de satisfação após consulta";

            // Act
            var result = await _service.CreateSurveyAsync(
                surveyName,
                description,
                SurveyType.NPS,
                _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(surveyName, result.Name);
            Assert.Equal(description, result.Description);
            Assert.Equal(SurveyType.NPS, result.Type);
            Assert.False(result.IsActive);
        }

        [Fact]
        public async Task ActivateSurveyAsync_ShouldActivateSurvey()
        {
            // Arrange
            var survey = await _service.CreateSurveyAsync(
                "Test Survey",
                "Description",
                SurveyType.CSAT,
                _testTenantId);

            // Act
            await _service.ActivateSurveyAsync(survey.Id, _testTenantId);

            // Assert
            var updated = await _service.GetSurveyByIdAsync(survey.Id, _testTenantId);
            Assert.True(updated.IsActive);
        }

        [Fact]
        public async Task AddQuestionAsync_ShouldAddQuestionToSurvey()
        {
            // Arrange
            var survey = await _service.CreateSurveyAsync(
                "Test Survey",
                "Description",
                SurveyType.Custom,
                _testTenantId);

            // Act
            await _service.AddQuestionAsync(
                survey.Id,
                "Como você avalia nosso serviço?",
                QuestionType.Rating,
                _testTenantId,
                isRequired: true,
                order: 1);

            // Assert
            var updated = await _service.GetSurveyByIdAsync(survey.Id, _testTenantId);
            Assert.NotEmpty(updated.Questions);
            Assert.Equal("Como você avalia nosso serviço?", updated.Questions.First().Text);
            Assert.Equal(QuestionType.Rating, updated.Questions.First().Type);
        }

        [Fact]
        public async Task SubmitResponseAsync_ShouldCalculateNpsScore()
        {
            // Arrange
            var survey = await _service.CreateSurveyAsync(
                "NPS Survey",
                "Description",
                SurveyType.NPS,
                _testTenantId);

            var question = await _service.AddQuestionAsync(
                survey.Id,
                "Recomendaria nossos serviços?",
                QuestionType.NPS,
                _testTenantId,
                isRequired: true,
                order: 1);

            var pacienteId = Guid.NewGuid();

            // Create response with answers
            var questionResponses = new Dictionary<Guid, string>
            {
                { question.Id, "9" } // Promoter score
            };

            // Act
            var response = await _service.SubmitResponseAsync(
                survey.Id,
                pacienteId,
                questionResponses,
                _testTenantId);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(9, response.NpsScore);
            Assert.NotNull(response.CompletedAt);
        }

        [Fact]
        public async Task CalculateNpsAsync_ShouldCalculateCorrectNpsScore()
        {
            // Arrange
            var survey = await _service.CreateSurveyAsync(
                "NPS Survey",
                "Description",
                SurveyType.NPS,
                _testTenantId);

            var question = await _service.AddQuestionAsync(
                survey.Id,
                "Recomendaria?",
                QuestionType.NPS,
                _testTenantId,
                isRequired: true,
                order: 1);

            // Submit responses: 2 promoters (9, 10), 1 passive (7), 2 detractors (5, 6)
            var scores = new[] { 9, 10, 7, 5, 6 };
            foreach (var score in scores)
            {
                var pacienteId = Guid.NewGuid();
                var responses = new Dictionary<Guid, string> { { question.Id, score.ToString() } };
                await _service.SubmitResponseAsync(survey.Id, pacienteId, responses, _testTenantId);
            }

            // Act
            var analytics = await _service.GetSurveyAnalyticsAsync(survey.Id, _testTenantId);

            // Assert
            // NPS = (Promoters - Detractors) / Total * 100 = (2 - 2) / 5 * 100 = 0
            Assert.Equal(0, analytics.NpsScore);
            Assert.Equal(2, analytics.Promoters);
            Assert.Equal(1, analytics.Passives);
            Assert.Equal(2, analytics.Detractors);
        }

        [Fact]
        public async Task GetActiveSurveysAsync_ShouldReturnOnlyActiveSurveys()
        {
            // Arrange
            var survey1 = await _service.CreateSurveyAsync("Survey 1", "Desc", SurveyType.NPS, _testTenantId);
            var survey2 = await _service.CreateSurveyAsync("Survey 2", "Desc", SurveyType.CSAT, _testTenantId);
            var survey3 = await _service.CreateSurveyAsync("Survey 3", "Desc", SurveyType.Custom, _testTenantId);

            await _service.ActivateSurveyAsync(survey1.Id, _testTenantId);
            await _service.ActivateSurveyAsync(survey3.Id, _testTenantId);

            // Act
            var activeSurveys = await _service.GetActiveSurveysAsync(_testTenantId);

            // Assert
            Assert.Equal(2, activeSurveys.Count);
            Assert.Contains(activeSurveys, s => s.Id == survey1.Id);
            Assert.Contains(activeSurveys, s => s.Id == survey3.Id);
            Assert.DoesNotContain(activeSurveys, s => s.Id == survey2.Id);
        }

        [Fact]
        public async Task DeleteSurveyAsync_ShouldRemoveSurvey()
        {
            // Arrange
            var survey = await _service.CreateSurveyAsync("Survey to Delete", "Desc", SurveyType.NPS, _testTenantId);

            // Act
            await _service.DeleteSurveyAsync(survey.Id, _testTenantId);

            // Assert
            var allSurveys = await _service.GetAllSurveysAsync(_testTenantId);
            Assert.DoesNotContain(allSurveys, s => s.Id == survey.Id);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
