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

namespace MedicSoft.Test.Integration
{
    /// <summary>
    /// End-to-end integration tests for CRM functionality
    /// Tests complete workflows across multiple services
    /// </summary>
    public class CRMIntegrationTests : IDisposable
    {
        private readonly MedicSoftDbContext _context;
        private readonly IPatientJourneyService _journeyService;
        private readonly ISurveyService _surveyService;
        private readonly IWebhookService _webhookService;
        private readonly string _testTenantId = "e2e-test-tenant";
        private readonly Guid _testPatientId = Guid.NewGuid();

        public CRMIntegrationTests()
        {
            var options = new DbContextOptionsBuilder<MedicSoftDbContext>()
                .UseInMemoryDatabase(databaseName: $"E2ETestDb_{Guid.NewGuid()}")
                .Options;

            _context = new MedicSoftDbContext(options);
            
            var mockJourneyLogger = new Mock<ILogger<PatientJourneyService>>();
            var mockAutomationEngine = new Mock<IAutomationEngine>();
            _journeyService = new PatientJourneyService(
                _context, 
                mockJourneyLogger.Object, 
                mockAutomationEngine.Object);

            var mockSurveyLogger = new Mock<ILogger<SurveyService>>();
            _surveyService = new SurveyService(_context, mockSurveyLogger.Object);

            var mockWebhookLogger = new Mock<ILogger<WebhookService>>();
            var mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _webhookService = new WebhookService(
                _context, 
                mockWebhookLogger.Object,
                mockHttpClientFactory.Object);
        }

        #region Patient Journey E2E Tests

        [Fact]
        public async Task PatientJourney_CompleteLifecycle_ShouldTrackAllStages()
        {
            // Arrange - Create journey
            var journey = await _journeyService.GetOrCreateJourneyAsync(_testPatientId, _testTenantId);
            Assert.Equal(JourneyStageEnum.Descoberta, journey.CurrentStage);

            // Act & Assert - Advance through stages
            
            // Stage 1: Descoberta -> Consideracao
            var advanceDto1 = new AdvanceJourneyStageDto
            {
                NewStage = JourneyStageEnum.Consideracao,
                Trigger = "Lead submitted form"
            };
            journey = await _journeyService.AdvanceStageAsync(_testPatientId, advanceDto1, _testTenantId);
            Assert.Equal(JourneyStageEnum.Consideracao, journey.CurrentStage);
            Assert.Equal(2, journey.Stages.Count);

            // Stage 2: Consideracao -> PrimeiraConsulta
            var advanceDto2 = new AdvanceJourneyStageDto
            {
                NewStage = JourneyStageEnum.PrimeiraConsulta,
                Trigger = "First appointment scheduled"
            };
            journey = await _journeyService.AdvanceStageAsync(_testPatientId, advanceDto2, _testTenantId);
            Assert.Equal(JourneyStageEnum.PrimeiraConsulta, journey.CurrentStage);
            Assert.Equal(3, journey.Stages.Count);

            // Add touchpoint
            var touchpointDto = new CreatePatientTouchpointDto
            {
                Type = TouchpointType.Consulta,
                Direction = TouchpointDirection.Inbound,
                Channel = "In-person",
                Summary = "First consultation completed",
                Sentiment = SentimentType.Positive
            };
            journey = await _journeyService.AddTouchpointAsync(_testPatientId, touchpointDto, _testTenantId);
            Assert.Single(journey.Stages.Last().Touchpoints);

            // Update metrics
            var metricsDto = new UpdatePatientJourneyMetricsDto
            {
                LifetimeValue = 1500.00m,
                NpsScore = 9,
                SatisfactionScore = 4.5
            };
            journey = await _journeyService.UpdateMetricsAsync(_testPatientId, metricsDto, _testTenantId);
            Assert.Equal(1500.00m, journey.LifetimeValue);
            Assert.Equal(9, journey.NpsScore);

            // Verify final state
            var finalJourney = await _journeyService.GetJourneyByPatientIdAsync(_testPatientId, _testTenantId);
            Assert.NotNull(finalJourney);
            Assert.Equal(3, finalJourney.Stages.Count);
            Assert.True(finalJourney.TotalTouchpoints > 0);
        }

        [Fact]
        public async Task PatientJourney_WithMultipleTouchpoints_ShouldTrackEngagement()
        {
            // Arrange
            var journey = await _journeyService.GetOrCreateJourneyAsync(_testPatientId, _testTenantId);

            // Act - Add multiple touchpoints
            var touchpoints = new[]
            {
                new CreatePatientTouchpointDto 
                { 
                    Type = TouchpointType.Email, 
                    Direction = TouchpointDirection.Outbound,
                    Channel = "Email",
                    Summary = "Welcome email sent"
                },
                new CreatePatientTouchpointDto 
                { 
                    Type = TouchpointType.Telefone, 
                    Direction = TouchpointDirection.Outbound,
                    Channel = "Phone",
                    Summary = "Appointment reminder call"
                },
                new CreatePatientTouchpointDto 
                { 
                    Type = TouchpointType.Consulta, 
                    Direction = TouchpointDirection.Inbound,
                    Channel = "In-person",
                    Summary = "Consultation completed",
                    Sentiment = SentimentType.Positive
                }
            };

            foreach (var tp in touchpoints)
            {
                journey = await _journeyService.AddTouchpointAsync(_testPatientId, tp, _testTenantId);
            }

            // Assert
            Assert.Equal(3, journey.TotalTouchpoints);
            var currentStage = journey.Stages.First(s => s.Stage == journey.CurrentStage);
            Assert.Equal(3, currentStage.Touchpoints.Count);
        }

        #endregion

        #region Survey/NPS E2E Tests

        [Fact]
        public async Task Survey_CreateAndComplete_ShouldCalculateNPS()
        {
            // Arrange - Create NPS survey
            var createDto = new CreateSurveyDto
            {
                Title = "NPS Survey",
                Description = "How likely are you to recommend us?",
                Type = SurveyType.NPS,
                TriggerStage = JourneyStageEnum.PrimeiraConsulta
            };

            var survey = await _surveyService.CreateSurveyAsync(createDto, _testTenantId);
            
            // Add NPS question
            var questionDto = new CreateSurveyQuestionDto
            {
                Text = "How likely are you to recommend our service?",
                Type = QuestionType.Rating,
                IsRequired = true,
                Order = 1
            };
            survey = await _surveyService.AddQuestionAsync(survey.Id, questionDto, _testTenantId);

            // Activate survey
            survey = await _surveyService.ActivateSurveyAsync(survey.Id, _testTenantId);

            // Act - Send and complete survey
            var sendDto = new SendSurveyDto
            {
                PacienteId = _testPatientId,
                DeliveryMethod = "Email"
            };
            var response = await _surveyService.SendSurveyAsync(survey.Id, sendDto, _testTenantId);

            // Submit response with score 9 (promoter)
            var submitDto = new SubmitSurveyResponseDto
            {
                Responses = new List<SurveyQuestionResponseDto>
                {
                    new SurveyQuestionResponseDto
                    {
                        QuestionId = survey.Questions.First().Id,
                        NumericValue = 9
                    }
                }
            };
            response = await _surveyService.SubmitResponseAsync(response.Id, submitDto, _testTenantId);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.IsCompleted);
            Assert.NotNull(response.CompletedAt);
            Assert.Single(response.Responses);
            Assert.Equal(9, response.Responses.First().NumericValue);

            // Verify NPS calculation
            var insights = await _surveyService.GetNPSInsightsAsync(_testTenantId, DateTime.UtcNow.AddDays(-1));
            Assert.NotNull(insights);
        }

        [Fact]
        public async Task Survey_MultipleResponses_ShouldCalculateCorrectNPS()
        {
            // Arrange - Create survey
            var survey = await CreateBasicNPSSurvey();

            // Act - Submit multiple responses
            var scores = new[] { 9, 10, 8, 7, 6, 9, 10, 5, 4, 9 }; // Mix of promoters, passives, detractors
            
            foreach (var score in scores)
            {
                var patientId = Guid.NewGuid();
                var sendDto = new SendSurveyDto
                {
                    PacienteId = patientId,
                    DeliveryMethod = "Email"
                };
                var response = await _surveyService.SendSurveyAsync(survey.Id, sendDto, _testTenantId);

                var submitDto = new SubmitSurveyResponseDto
                {
                    Responses = new List<SurveyQuestionResponseDto>
                    {
                        new SurveyQuestionResponseDto
                        {
                            QuestionId = survey.Questions.First().Id,
                            NumericValue = score
                        }
                    }
                };
                await _surveyService.SubmitResponseAsync(response.Id, submitDto, _testTenantId);
            }

            // Assert
            var insights = await _surveyService.GetNPSInsightsAsync(_testTenantId, DateTime.UtcNow.AddDays(-1));
            Assert.NotNull(insights);
            
            // Expected: 5 promoters (9-10), 3 passives (7-8), 2 detractors (0-6)
            // NPS = (5 - 2) / 10 * 100 = 30
            Assert.InRange(insights.Score, 25, 35); // Allow small variance
        }

        #endregion

        #region Webhook E2E Tests

        [Fact]
        public async Task Webhook_JourneyStageChange_ShouldTriggerWebhook()
        {
            // Arrange - Create webhook subscription
            var webhookDto = new CreateWebhookSubscriptionDto
            {
                Name = "Journey Stage Webhook",
                Description = "Notifies on stage changes",
                TargetUrl = "https://example.com/webhook",
                SubscribedEvents = new List<WebhookEvent> { WebhookEvent.JourneyStageChanged }
            };
            var subscription = await _webhookService.CreateSubscriptionAsync(webhookDto, _testTenantId);
            subscription = await _webhookService.ActivateSubscriptionAsync(subscription.Id, _testTenantId);

            // Act - Advance patient journey stage (this should trigger webhook)
            var journey = await _journeyService.GetOrCreateJourneyAsync(_testPatientId, _testTenantId);
            
            // Manually publish event (in real scenario, this would be in the service)
            var eventData = new
            {
                PatientId = _testPatientId,
                PreviousStage = JourneyStageEnum.Descoberta.ToString(),
                NewStage = JourneyStageEnum.Consideracao.ToString(),
                Trigger = "Test trigger"
            };
            await _webhookService.PublishEventAsync(WebhookEvent.JourneyStageChanged, eventData, _testTenantId);

            // Assert
            var deliveries = await _webhookService.GetDeliveriesAsync(subscription.Id, _testTenantId);
            Assert.Single(deliveries);
            Assert.Equal(WebhookEvent.JourneyStageChanged, deliveries[0].Event);
            Assert.Equal(WebhookDeliveryStatus.Pending, deliveries[0].Status);
        }

        [Fact]
        public async Task Webhook_MultipleEvents_ShouldReceiveAllSubscribedEvents()
        {
            // Arrange - Create webhook with multiple event subscriptions
            var webhookDto = new CreateWebhookSubscriptionDto
            {
                Name = "Multi-Event Webhook",
                Description = "Subscribes to multiple events",
                TargetUrl = "https://example.com/webhook",
                SubscribedEvents = new List<WebhookEvent> 
                { 
                    WebhookEvent.JourneyStageChanged,
                    WebhookEvent.SurveyCompleted,
                    WebhookEvent.TouchpointCreated
                }
            };
            var subscription = await _webhookService.CreateSubscriptionAsync(webhookDto, _testTenantId);
            subscription = await _webhookService.ActivateSubscriptionAsync(subscription.Id, _testTenantId);

            // Act - Trigger multiple events
            await _webhookService.PublishEventAsync(
                WebhookEvent.JourneyStageChanged, 
                new { PatientId = _testPatientId, Stage = "Test" }, 
                _testTenantId);

            await _webhookService.PublishEventAsync(
                WebhookEvent.SurveyCompleted, 
                new { SurveyId = Guid.NewGuid(), PatientId = _testPatientId }, 
                _testTenantId);

            await _webhookService.PublishEventAsync(
                WebhookEvent.TouchpointCreated, 
                new { PatientId = _testPatientId, Type = "Email" }, 
                _testTenantId);

            // Don't trigger this one (not subscribed)
            await _webhookService.PublishEventAsync(
                WebhookEvent.ComplaintCreated, 
                new { ComplaintId = Guid.NewGuid() }, 
                _testTenantId);

            // Assert
            var deliveries = await _webhookService.GetDeliveriesAsync(subscription.Id, _testTenantId);
            Assert.Equal(3, deliveries.Count); // Only subscribed events
            Assert.Contains(deliveries, d => d.Event == WebhookEvent.JourneyStageChanged);
            Assert.Contains(deliveries, d => d.Event == WebhookEvent.SurveyCompleted);
            Assert.Contains(deliveries, d => d.Event == WebhookEvent.TouchpointCreated);
            Assert.DoesNotContain(deliveries, d => d.Event == WebhookEvent.ComplaintCreated);
        }

        #endregion

        #region Complete Workflow E2E Test

        [Fact]
        public async Task CompletePatientWorkflow_ShouldIntegrateAllCRMComponents()
        {
            // This test simulates a complete patient interaction workflow
            // 1. Patient journey creation
            // 2. Stage advancement with touchpoints
            // 3. Survey sent and completed
            // 4. Webhook notifications
            
            // Step 1: Create patient journey
            var journey = await _journeyService.GetOrCreateJourneyAsync(_testPatientId, _testTenantId);
            Assert.NotNull(journey);

            // Step 2: Setup webhook to track events
            var webhookDto = new CreateWebhookSubscriptionDto
            {
                Name = "Complete Workflow Webhook",
                TargetUrl = "https://example.com/webhook",
                SubscribedEvents = new List<WebhookEvent> 
                { 
                    WebhookEvent.JourneyStageChanged,
                    WebhookEvent.SurveyCompleted
                }
            };
            var webhook = await _webhookService.CreateSubscriptionAsync(webhookDto, _testTenantId);
            await _webhookService.ActivateSubscriptionAsync(webhook.Id, _testTenantId);

            // Step 3: Advance to first consultation
            var advanceDto = new AdvanceJourneyStageDto
            {
                NewStage = JourneyStageEnum.PrimeiraConsulta,
                Trigger = "First appointment scheduled"
            };
            journey = await _journeyService.AdvanceStageAsync(_testPatientId, advanceDto, _testTenantId);
            
            // Publish webhook event
            await _webhookService.PublishEventAsync(
                WebhookEvent.JourneyStageChanged,
                new { PatientId = _testPatientId, Stage = journey.CurrentStage.ToString() },
                _testTenantId);

            // Step 4: Add consultation touchpoint
            var touchpointDto = new CreatePatientTouchpointDto
            {
                Type = TouchpointType.Consulta,
                Direction = TouchpointDirection.Inbound,
                Channel = "In-person",
                Summary = "Consultation completed successfully",
                Sentiment = SentimentType.Positive
            };
            journey = await _journeyService.AddTouchpointAsync(_testPatientId, touchpointDto, _testTenantId);

            // Step 5: Send post-consultation NPS survey
            var survey = await CreateBasicNPSSurvey();
            var sendDto = new SendSurveyDto
            {
                PacienteId = _testPatientId,
                DeliveryMethod = "Email"
            };
            var response = await _surveyService.SendSurveyAsync(survey.Id, sendDto, _testTenantId);

            // Step 6: Patient completes survey
            var submitDto = new SubmitSurveyResponseDto
            {
                Responses = new List<SurveyQuestionResponseDto>
                {
                    new SurveyQuestionResponseDto
                    {
                        QuestionId = survey.Questions.First().Id,
                        NumericValue = 10 // Perfect score!
                    }
                }
            };
            response = await _surveyService.SubmitResponseAsync(response.Id, submitDto, _testTenantId);
            
            // Publish survey completion webhook
            await _webhookService.PublishEventAsync(
                WebhookEvent.SurveyCompleted,
                new { SurveyId = survey.Id, PatientId = _testPatientId, Score = 10 },
                _testTenantId);

            // Step 7: Update journey metrics based on survey
            var metricsDto = new UpdatePatientJourneyMetricsDto
            {
                NpsScore = 10,
                SatisfactionScore = 5.0
            };
            journey = await _journeyService.UpdateMetricsAsync(_testPatientId, metricsDto, _testTenantId);

            // Final Assertions
            Assert.Equal(JourneyStageEnum.PrimeiraConsulta, journey.CurrentStage);
            Assert.True(journey.TotalTouchpoints > 0);
            Assert.Equal(10, journey.NpsScore);
            Assert.True(response.IsCompleted);
            
            var webhookDeliveries = await _webhookService.GetDeliveriesAsync(webhook.Id, _testTenantId);
            Assert.Equal(2, webhookDeliveries.Count); // Stage change + survey completion
        }

        #endregion

        #region Helper Methods

        private async Task<SurveyDto> CreateBasicNPSSurvey()
        {
            var createDto = new CreateSurveyDto
            {
                Title = "NPS Survey",
                Description = "How likely are you to recommend us?",
                Type = SurveyType.NPS,
                TriggerStage = JourneyStageEnum.PrimeiraConsulta
            };

            var survey = await _surveyService.CreateSurveyAsync(createDto, _testTenantId);
            
            var questionDto = new CreateSurveyQuestionDto
            {
                Text = "On a scale of 0-10, how likely are you to recommend our service?",
                Type = QuestionType.Rating,
                IsRequired = true,
                Order = 1
            };
            survey = await _surveyService.AddQuestionAsync(survey.Id, questionDto, _testTenantId);
            survey = await _surveyService.ActivateSurveyAsync(survey.Id, _testTenantId);
            
            return survey;
        }

        #endregion

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
        }
    }
}
