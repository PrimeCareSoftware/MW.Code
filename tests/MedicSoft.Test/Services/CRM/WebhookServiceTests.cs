using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using MedicSoft.Api.Services.CRM;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Repository.Context;
using Xunit;

namespace MedicSoft.Test.Services.CRM
{
    public class WebhookServiceTests : IDisposable
    {
        private readonly MedicSoftDbContext _context;
        private readonly Mock<ILogger<WebhookService>> _mockLogger;
        private readonly Mock<IHttpClientFactory> _mockHttpClientFactory;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;
        private readonly IWebhookService _service;
        private readonly string _testTenantId = "test-tenant-123";

        public WebhookServiceTests()
        {
            // Setup in-memory database
            var options = new DbContextOptionsBuilder<MedicSoftDbContext>()
                .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
                .Options;

            _context = new MedicSoftDbContext(options);
            _mockLogger = new Mock<ILogger<WebhookService>>();
            
            // Setup HttpClient mock
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);
            
            _mockHttpClientFactory = new Mock<IHttpClientFactory>();
            _mockHttpClientFactory.Setup(f => f.CreateClient(It.IsAny<string>()))
                .Returns(httpClient);

            _service = new WebhookService(_context, _mockLogger.Object, _mockHttpClientFactory.Object);
        }

        #region Subscription Management Tests

        [Fact]
        public async Task CreateSubscriptionAsync_ShouldCreateNewSubscription()
        {
            // Arrange
            var dto = new CreateWebhookSubscriptionDto
            {
                Name = "Test Webhook",
                Description = "Test webhook subscription",
                TargetUrl = "https://example.com/webhook",
                SubscribedEvents = new List<WebhookEvent> 
                { 
                    WebhookEvent.JourneyStageChanged, 
                    WebhookEvent.SurveyCompleted 
                },
                MaxRetries = 3,
                RetryDelaySeconds = 60
            };

            // Act
            var result = await _service.CreateSubscriptionAsync(dto, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(dto.Name, result.Name);
            Assert.Equal(dto.TargetUrl, result.TargetUrl);
            Assert.False(result.IsActive); // Should start inactive
            Assert.NotEmpty(result.Secret);
            Assert.Equal(2, result.SubscribedEvents.Count);
            Assert.Equal(3, result.MaxRetries);
            Assert.Equal(60, result.RetryDelaySeconds);
        }

        [Fact]
        public async Task GetSubscriptionAsync_ShouldReturnSubscription_WhenExists()
        {
            // Arrange
            var subscription = new WebhookSubscription(
                "Test Webhook",
                "Description",
                "https://example.com/webhook",
                _testTenantId);
            subscription.SubscribeToEvent(WebhookEvent.JourneyStageChanged);
            _context.WebhookSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetSubscriptionAsync(subscription.Id, _testTenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(subscription.Id, result.Id);
            Assert.Equal(subscription.Name, result.Name);
        }

        [Fact]
        public async Task GetSubscriptionAsync_ShouldReturnNull_WhenNotExists()
        {
            // Act
            var result = await _service.GetSubscriptionAsync(Guid.NewGuid(), _testTenantId);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetAllSubscriptionsAsync_ShouldReturnAllForTenant()
        {
            // Arrange
            var sub1 = new WebhookSubscription("Sub1", "Desc1", "https://example.com/1", _testTenantId);
            var sub2 = new WebhookSubscription("Sub2", "Desc2", "https://example.com/2", _testTenantId);
            var sub3 = new WebhookSubscription("Sub3", "Desc3", "https://example.com/3", "other-tenant");
            
            _context.WebhookSubscriptions.AddRange(sub1, sub2, sub3);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetAllSubscriptionsAsync(_testTenantId);

            // Assert
            Assert.Equal(2, result.Count);
            Assert.All(result, s => Assert.Equal(_testTenantId, s.CreatedAt.ToString())); // Verify tenant filtering
        }

        [Fact]
        public async Task UpdateSubscriptionAsync_ShouldUpdateProperties()
        {
            // Arrange
            var subscription = new WebhookSubscription(
                "Original Name",
                "Original Description",
                "https://example.com/original",
                _testTenantId);
            _context.WebhookSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            var updateDto = new UpdateWebhookSubscriptionDto
            {
                TargetUrl = "https://example.com/updated",
                IsActive = true,
                SubscribedEvents = new List<WebhookEvent> { WebhookEvent.ChurnRiskCalculated }
            };

            // Act
            var result = await _service.UpdateSubscriptionAsync(subscription.Id, updateDto, _testTenantId);

            // Assert
            Assert.Equal("https://example.com/updated", result.TargetUrl);
            Assert.True(result.IsActive);
            Assert.Single(result.SubscribedEvents);
            Assert.Contains(WebhookEvent.ChurnRiskCalculated, result.SubscribedEvents);
        }

        [Fact]
        public async Task ActivateSubscriptionAsync_ShouldActivateSubscription()
        {
            // Arrange
            var subscription = new WebhookSubscription(
                "Test",
                "Test",
                "https://example.com/webhook",
                _testTenantId);
            _context.WebhookSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.ActivateSubscriptionAsync(subscription.Id, _testTenantId);

            // Assert
            Assert.True(result.IsActive);
        }

        [Fact]
        public async Task DeactivateSubscriptionAsync_ShouldDeactivateSubscription()
        {
            // Arrange
            var subscription = new WebhookSubscription(
                "Test",
                "Test",
                "https://example.com/webhook",
                _testTenantId);
            subscription.Activate();
            _context.WebhookSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.DeactivateSubscriptionAsync(subscription.Id, _testTenantId);

            // Assert
            Assert.False(result.IsActive);
        }

        [Fact]
        public async Task RegenerateSecretAsync_ShouldCreateNewSecret()
        {
            // Arrange
            var subscription = new WebhookSubscription(
                "Test",
                "Test",
                "https://example.com/webhook",
                _testTenantId);
            var originalSecret = subscription.Secret;
            _context.WebhookSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.RegenerateSecretAsync(subscription.Id, _testTenantId);

            // Assert
            Assert.NotEqual(originalSecret, result.Secret);
            Assert.NotEmpty(result.Secret);
        }

        [Fact]
        public async Task DeleteSubscriptionAsync_ShouldRemoveSubscription()
        {
            // Arrange
            var subscription = new WebhookSubscription(
                "Test",
                "Test",
                "https://example.com/webhook",
                _testTenantId);
            _context.WebhookSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            // Act
            await _service.DeleteSubscriptionAsync(subscription.Id, _testTenantId);

            // Assert
            var deleted = await _context.WebhookSubscriptions.FindAsync(subscription.Id);
            Assert.Null(deleted);
        }

        #endregion

        #region Event Publishing Tests

        [Fact]
        public async Task PublishEventAsync_ShouldCreateDeliveries_ForActiveSubscriptions()
        {
            // Arrange
            var activeSub = new WebhookSubscription("Active", "Test", "https://example.com/1", _testTenantId);
            activeSub.SubscribeToEvent(WebhookEvent.JourneyStageChanged);
            activeSub.Activate();

            var inactiveSub = new WebhookSubscription("Inactive", "Test", "https://example.com/2", _testTenantId);
            inactiveSub.SubscribeToEvent(WebhookEvent.JourneyStageChanged);

            var wrongEventSub = new WebhookSubscription("Wrong Event", "Test", "https://example.com/3", _testTenantId);
            wrongEventSub.SubscribeToEvent(WebhookEvent.SurveyCompleted);
            wrongEventSub.Activate();

            _context.WebhookSubscriptions.AddRange(activeSub, inactiveSub, wrongEventSub);
            await _context.SaveChangesAsync();

            var eventData = new { PatientId = Guid.NewGuid(), Stage = "PrimeiraConsulta" };

            // Act
            await _service.PublishEventAsync(WebhookEvent.JourneyStageChanged, eventData, _testTenantId);

            // Assert
            var deliveries = await _context.WebhookDeliveries.ToListAsync();
            Assert.Single(deliveries); // Only active subscription with matching event
            Assert.Equal(activeSub.Id, deliveries[0].SubscriptionId);
            Assert.Equal(WebhookEvent.JourneyStageChanged, deliveries[0].Event);
            Assert.Equal(WebhookDeliveryStatus.Pending, deliveries[0].Status);
        }

        [Fact]
        public async Task PublishEventAsync_ShouldNotCreateDeliveries_WhenNoActiveSubscriptions()
        {
            // Arrange
            var eventData = new { PatientId = Guid.NewGuid() };

            // Act
            await _service.PublishEventAsync(WebhookEvent.JourneyStageChanged, eventData, _testTenantId);

            // Assert
            var deliveries = await _context.WebhookDeliveries.ToListAsync();
            Assert.Empty(deliveries);
        }

        #endregion

        #region Delivery Management Tests

        [Fact]
        public async Task GetDeliveriesAsync_ShouldReturnDeliveriesForSubscription()
        {
            // Arrange
            var subscription = new WebhookSubscription("Test", "Test", "https://example.com/webhook", _testTenantId);
            _context.WebhookSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            var delivery1 = new WebhookDelivery(subscription.Id, WebhookEvent.JourneyStageChanged, "{}", "https://example.com", _testTenantId);
            var delivery2 = new WebhookDelivery(subscription.Id, WebhookEvent.SurveyCompleted, "{}", "https://example.com", _testTenantId);
            
            _context.WebhookDeliveries.AddRange(delivery1, delivery2);
            await _context.SaveChangesAsync();

            // Act
            var result = await _service.GetDeliveriesAsync(subscription.Id, _testTenantId, 50);

            // Assert
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public async Task ProcessPendingDeliveriesAsync_ShouldDeliverSuccessfully()
        {
            // Arrange
            var subscription = new WebhookSubscription("Test", "Test", "https://example.com/webhook", _testTenantId);
            subscription.Activate();
            _context.WebhookSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            var delivery = new WebhookDelivery(
                subscription.Id, 
                WebhookEvent.JourneyStageChanged, 
                "{\"test\":\"data\"}", 
                "https://example.com/webhook", 
                _testTenantId);
            _context.WebhookDeliveries.Add(delivery);
            await _context.SaveChangesAsync();

            // Setup HTTP mock to return success
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.OK,
                    Content = new StringContent("{\"received\":true}")
                });

            // Act
            await _service.ProcessPendingDeliveriesAsync();

            // Assert
            var updatedDelivery = await _context.WebhookDeliveries.FindAsync(delivery.Id);
            Assert.NotNull(updatedDelivery);
            Assert.Equal(WebhookDeliveryStatus.Delivered, updatedDelivery.Status);
            Assert.NotNull(updatedDelivery.DeliveredAt);
            Assert.Equal(200, updatedDelivery.ResponseStatusCode);
        }

        [Fact]
        public async Task ProcessPendingDeliveriesAsync_ShouldScheduleRetry_OnFailure()
        {
            // Arrange
            var subscription = new WebhookSubscription("Test", "Test", "https://example.com/webhook", _testTenantId);
            subscription.Activate();
            subscription.ConfigureRetry(3, 60);
            _context.WebhookSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            var delivery = new WebhookDelivery(
                subscription.Id, 
                WebhookEvent.JourneyStageChanged, 
                "{\"test\":\"data\"}", 
                "https://example.com/webhook", 
                _testTenantId);
            _context.WebhookDeliveries.Add(delivery);
            await _context.SaveChangesAsync();

            // Setup HTTP mock to return failure
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent("Server error")
                });

            // Act
            await _service.ProcessPendingDeliveriesAsync();

            // Assert
            var updatedDelivery = await _context.WebhookDeliveries.FindAsync(delivery.Id);
            Assert.NotNull(updatedDelivery);
            Assert.Equal(WebhookDeliveryStatus.Retrying, updatedDelivery.Status);
            Assert.Equal(1, updatedDelivery.AttemptCount);
            Assert.NotNull(updatedDelivery.NextRetryAt);
            Assert.True(updatedDelivery.NextRetryAt > DateTime.UtcNow);
        }

        [Fact]
        public async Task ProcessPendingDeliveriesAsync_ShouldMarkFailed_AfterMaxRetries()
        {
            // Arrange
            var subscription = new WebhookSubscription("Test", "Test", "https://example.com/webhook", _testTenantId);
            subscription.Activate();
            subscription.ConfigureRetry(2, 60); // Max 2 retries
            _context.WebhookSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            var delivery = new WebhookDelivery(
                subscription.Id, 
                WebhookEvent.JourneyStageChanged, 
                "{\"test\":\"data\"}", 
                "https://example.com/webhook", 
                _testTenantId);
            
            // Simulate already failed twice
            delivery.ScheduleRetry(60);
            delivery.ScheduleRetry(60);
            
            _context.WebhookDeliveries.Add(delivery);
            await _context.SaveChangesAsync();

            // Setup HTTP mock to return failure
            _mockHttpMessageHandler.Protected()
                .Setup<Task<HttpResponseMessage>>(
                    "SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Content = new StringContent("Server error")
                });

            // Act
            await _service.ProcessPendingDeliveriesAsync();

            // Assert
            var updatedDelivery = await _context.WebhookDeliveries.FindAsync(delivery.Id);
            Assert.NotNull(updatedDelivery);
            Assert.Equal(WebhookDeliveryStatus.Failed, updatedDelivery.Status);
            Assert.NotNull(updatedDelivery.FailedAt);
            Assert.NotEmpty(updatedDelivery.ErrorMessage);
        }

        [Fact]
        public async Task ProcessPendingDeliveriesAsync_ShouldSkip_InactiveSubscriptions()
        {
            // Arrange
            var subscription = new WebhookSubscription("Test", "Test", "https://example.com/webhook", _testTenantId);
            // Not activated
            _context.WebhookSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            var delivery = new WebhookDelivery(
                subscription.Id, 
                WebhookEvent.JourneyStageChanged, 
                "{\"test\":\"data\"}", 
                "https://example.com/webhook", 
                _testTenantId);
            _context.WebhookDeliveries.Add(delivery);
            await _context.SaveChangesAsync();

            // Act
            await _service.ProcessPendingDeliveriesAsync();

            // Assert
            var updatedDelivery = await _context.WebhookDeliveries.FindAsync(delivery.Id);
            Assert.NotNull(updatedDelivery);
            Assert.Equal(WebhookDeliveryStatus.Failed, updatedDelivery.Status);
            Assert.Contains("inactive", updatedDelivery.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        public void Dispose()
        {
            _context?.Database.EnsureDeleted();
            _context?.Dispose();
        }
    }
}
