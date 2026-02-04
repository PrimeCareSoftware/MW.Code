using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Application.Services.CRM;
using MedicSoft.Domain.Entities.CRM;
using MedicSoft.Repository.Context;

namespace MedicSoft.Api.Services.CRM
{
    public class WebhookService : IWebhookService
    {
        private readonly MedicSoftDbContext _context;
        private readonly ILogger<WebhookService> _logger;
        private readonly IHttpClientFactory _httpClientFactory;

        public WebhookService(
            MedicSoftDbContext context,
            ILogger<WebhookService> logger,
            IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _logger = logger;
            _httpClientFactory = httpClientFactory;
        }

        #region Subscription Management

        public async Task<WebhookSubscriptionDto> CreateSubscriptionAsync(
            CreateWebhookSubscriptionDto dto, 
            string tenantId)
        {
            var subscription = new WebhookSubscription(
                dto.Name,
                dto.Description,
                dto.TargetUrl,
                tenantId);

            // Subscribe to events
            foreach (var webhookEvent in dto.SubscribedEvents)
            {
                subscription.SubscribeToEvent(webhookEvent);
            }

            // Configure retry if provided
            if (dto.MaxRetries.HasValue && dto.RetryDelaySeconds.HasValue)
            {
                subscription.ConfigureRetry(dto.MaxRetries.Value, dto.RetryDelaySeconds.Value);
            }

            _context.WebhookSubscriptions.Add(subscription);
            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Created webhook subscription {SubscriptionId} for tenant {TenantId}",
                subscription.Id, tenantId);

            return MapToDto(subscription);
        }

        public async Task<WebhookSubscriptionDto?> GetSubscriptionAsync(Guid id, string tenantId)
        {
            var subscription = await _context.WebhookSubscriptions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);

            return subscription == null ? null : MapToDto(subscription);
        }

        public async Task<List<WebhookSubscriptionDto>> GetAllSubscriptionsAsync(string tenantId)
        {
            var subscriptions = await _context.WebhookSubscriptions
                .AsNoTracking()
                .Where(s => s.TenantId == tenantId)
                .ToListAsync();

            return subscriptions.Select(MapToDto).ToList();
        }

        public async Task<WebhookSubscriptionDto> UpdateSubscriptionAsync(
            Guid id, 
            UpdateWebhookSubscriptionDto dto, 
            string tenantId)
        {
            var subscription = await _context.WebhookSubscriptions
                .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);

            if (subscription == null)
                throw new InvalidOperationException($"Webhook subscription {id} not found");

            if (!string.IsNullOrWhiteSpace(dto.TargetUrl))
                subscription.UpdateTargetUrl(dto.TargetUrl);

            if (dto.IsActive.HasValue)
            {
                if (dto.IsActive.Value)
                    subscription.Activate();
                else
                    subscription.Deactivate();
            }

            if (dto.SubscribedEvents != null)
            {
                // Clear existing and add new
                var currentEvents = subscription.SubscribedEvents.ToList();
                foreach (var evt in currentEvents)
                {
                    subscription.UnsubscribeFromEvent(evt);
                }
                foreach (var evt in dto.SubscribedEvents)
                {
                    subscription.SubscribeToEvent(evt);
                }
            }

            if (dto.MaxRetries.HasValue && dto.RetryDelaySeconds.HasValue)
            {
                subscription.ConfigureRetry(dto.MaxRetries.Value, dto.RetryDelaySeconds.Value);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Updated webhook subscription {SubscriptionId}", id);

            return MapToDto(subscription);
        }

        public async Task DeleteSubscriptionAsync(Guid id, string tenantId)
        {
            var subscription = await _context.WebhookSubscriptions
                .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);

            if (subscription == null)
                throw new InvalidOperationException($"Webhook subscription {id} not found");

            _context.WebhookSubscriptions.Remove(subscription);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deleted webhook subscription {SubscriptionId}", id);
        }

        public async Task<WebhookSubscriptionDto> ActivateSubscriptionAsync(Guid id, string tenantId)
        {
            var subscription = await _context.WebhookSubscriptions
                .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);

            if (subscription == null)
                throw new InvalidOperationException($"Webhook subscription {id} not found");

            subscription.Activate();
            await _context.SaveChangesAsync();

            _logger.LogInformation("Activated webhook subscription {SubscriptionId}", id);

            return MapToDto(subscription);
        }

        public async Task<WebhookSubscriptionDto> DeactivateSubscriptionAsync(Guid id, string tenantId)
        {
            var subscription = await _context.WebhookSubscriptions
                .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);

            if (subscription == null)
                throw new InvalidOperationException($"Webhook subscription {id} not found");

            subscription.Deactivate();
            await _context.SaveChangesAsync();

            _logger.LogInformation("Deactivated webhook subscription {SubscriptionId}", id);

            return MapToDto(subscription);
        }

        public async Task<WebhookSubscriptionDto> RegenerateSecretAsync(Guid id, string tenantId)
        {
            var subscription = await _context.WebhookSubscriptions
                .FirstOrDefaultAsync(s => s.Id == id && s.TenantId == tenantId);

            if (subscription == null)
                throw new InvalidOperationException($"Webhook subscription {id} not found");

            subscription.RegenerateSecret();
            await _context.SaveChangesAsync();

            _logger.LogInformation("Regenerated secret for webhook subscription {SubscriptionId}", id);

            return MapToDto(subscription);
        }

        #endregion

        #region Event Publishing

        public async Task PublishEventAsync(WebhookEvent webhookEvent, object data, string tenantId)
        {
            var subscriptions = await _context.WebhookSubscriptions
                .Where(s => s.TenantId == tenantId && 
                           s.IsActive && 
                           s.SubscribedEvents.Contains(webhookEvent))
                .ToListAsync();

            if (!subscriptions.Any())
            {
                _logger.LogDebug("No active subscriptions found for event {Event}", webhookEvent);
                return;
            }

            var payload = new WebhookPayloadDto
            {
                Id = Guid.NewGuid(),
                Event = webhookEvent.ToString(),
                Timestamp = DateTime.UtcNow,
                TenantId = tenantId,
                Data = data
            };

            var payloadJson = JsonSerializer.Serialize(payload, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            foreach (var subscription in subscriptions)
            {
                var delivery = new WebhookDelivery(
                    subscription.Id,
                    webhookEvent,
                    payloadJson,
                    subscription.TargetUrl,
                    tenantId);

                _context.WebhookDeliveries.Add(delivery);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation(
                "Published event {Event} to {Count} subscriptions",
                webhookEvent, subscriptions.Count);
        }

        #endregion

        #region Delivery Management

        public async Task<List<WebhookDeliveryDto>> GetDeliveriesAsync(
            Guid subscriptionId, 
            string tenantId, 
            int limit = 50)
        {
            var deliveries = await _context.WebhookDeliveries
                .Where(d => d.SubscriptionId == subscriptionId && d.TenantId == tenantId)
                .OrderByDescending(d => d.CreatedAt)
                .Take(limit)
                .ToListAsync();

            return deliveries.Select(MapToDeliveryDto).ToList();
        }

        public async Task<WebhookDeliveryDto?> GetDeliveryAsync(Guid id, string tenantId)
        {
            var delivery = await _context.WebhookDeliveries
                .FirstOrDefaultAsync(d => d.Id == id && d.TenantId == tenantId);

            return delivery == null ? null : MapToDeliveryDto(delivery);
        }

        public async Task ProcessPendingDeliveriesAsync()
        {
            var pendingDeliveries = await _context.WebhookDeliveries
                .Where(d => (d.Status == WebhookDeliveryStatus.Pending || 
                            d.Status == WebhookDeliveryStatus.Retrying) &&
                           (d.NextRetryAt == null || d.NextRetryAt <= DateTime.UtcNow))
                .Include(d => d.TenantId)
                .Take(100) // Process in batches
                .ToListAsync();

            foreach (var delivery in pendingDeliveries)
            {
                await DeliverWebhookAsync(delivery);
            }

            await _context.SaveChangesAsync();
        }

        public async Task RetryFailedDeliveryAsync(Guid deliveryId, string tenantId)
        {
            var delivery = await _context.WebhookDeliveries
                .FirstOrDefaultAsync(d => d.Id == deliveryId && d.TenantId == tenantId);

            if (delivery == null)
                throw new InvalidOperationException($"Webhook delivery {deliveryId} not found");

            if (delivery.Status == WebhookDeliveryStatus.Delivered)
                throw new InvalidOperationException("Cannot retry a delivered webhook");

            await DeliverWebhookAsync(delivery);
            await _context.SaveChangesAsync();
        }

        private async Task DeliverWebhookAsync(WebhookDelivery delivery)
        {
            var subscription = await _context.WebhookSubscriptions
                .FirstOrDefaultAsync(s => s.Id == delivery.SubscriptionId);

            if (subscription == null)
            {
                _logger.LogWarning(
                    "Subscription {SubscriptionId} not found for delivery {DeliveryId}",
                    delivery.SubscriptionId, delivery.Id);
                delivery.MarkAsFailed("Subscription not found");
                return;
            }

            if (!subscription.IsActive)
            {
                _logger.LogDebug(
                    "Subscription {SubscriptionId} is inactive, skipping delivery {DeliveryId}",
                    subscription.Id, delivery.Id);
                delivery.MarkAsFailed("Subscription is inactive");
                return;
            }

            try
            {
                var client = _httpClientFactory.CreateClient();
                client.Timeout = TimeSpan.FromSeconds(30);

                var request = new HttpRequestMessage(HttpMethod.Post, delivery.TargetUrl)
                {
                    Content = new StringContent(delivery.Payload, Encoding.UTF8, "application/json")
                };

                // Add signature header for security
                var signature = GenerateSignature(delivery.Payload, subscription.Secret);
                request.Headers.Add("X-Webhook-Signature", signature);
                request.Headers.Add("X-Webhook-Event", delivery.Event.ToString());
                request.Headers.Add("X-Webhook-Delivery-Id", delivery.Id.ToString());

                var response = await client.SendAsync(request);
                var responseBody = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    delivery.MarkAsDelivered((int)response.StatusCode, responseBody);
                    subscription.RecordDeliveryAttempt(true);

                    _logger.LogInformation(
                        "Successfully delivered webhook {DeliveryId} to {Url}",
                        delivery.Id, delivery.TargetUrl);
                }
                else
                {
                    HandleFailedDelivery(delivery, subscription, 
                        $"HTTP {response.StatusCode}", 
                        (int)response.StatusCode, 
                        responseBody);
                }
            }
            catch (Exception ex)
            {
                HandleFailedDelivery(delivery, subscription, ex.Message);
            }
        }

        private void HandleFailedDelivery(
            WebhookDelivery delivery, 
            WebhookSubscription subscription,
            string errorMessage,
            int? statusCode = null,
            string? responseBody = null)
        {
            if (delivery.CanRetry(subscription.MaxRetries))
            {
                delivery.ScheduleRetry(subscription.RetryDelaySeconds);
                
                _logger.LogWarning(
                    "Webhook delivery {DeliveryId} failed (attempt {Attempt}/{Max}), " +
                    "scheduled for retry at {RetryAt}: {Error}",
                    delivery.Id, delivery.AttemptCount, subscription.MaxRetries,
                    delivery.NextRetryAt, errorMessage);
            }
            else
            {
                delivery.MarkAsFailed(errorMessage, statusCode, responseBody);
                subscription.RecordDeliveryAttempt(false);

                _logger.LogError(
                    "Webhook delivery {DeliveryId} failed permanently after {Attempts} attempts: {Error}",
                    delivery.Id, delivery.AttemptCount, errorMessage);
            }
        }

        private static string GenerateSignature(string payload, string secret)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secret));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
            return Convert.ToBase64String(hash);
        }

        #endregion

        #region Mapping

        private static WebhookSubscriptionDto MapToDto(WebhookSubscription subscription)
        {
            return new WebhookSubscriptionDto
            {
                Id = subscription.Id,
                Name = subscription.Name,
                Description = subscription.Description,
                TargetUrl = subscription.TargetUrl,
                IsActive = subscription.IsActive,
                Secret = subscription.Secret,
                SubscribedEvents = subscription.SubscribedEvents.ToList(),
                MaxRetries = subscription.MaxRetries,
                RetryDelaySeconds = subscription.RetryDelaySeconds,
                TotalDeliveries = subscription.TotalDeliveries,
                SuccessfulDeliveries = subscription.SuccessfulDeliveries,
                FailedDeliveries = subscription.FailedDeliveries,
        LastDeliveryAt = subscription.LastDeliveryAt,
                LastSuccessAt = subscription.LastSuccessAt,
                LastFailureAt = subscription.LastFailureAt,
                CreatedAt = subscription.CreatedAt,
                UpdatedAt = subscription.UpdatedAt ?? subscription.CreatedAt
            };
        }

        private static WebhookDeliveryDto MapToDeliveryDto(WebhookDelivery delivery)
        {
            return new WebhookDeliveryDto
            {
                Id = delivery.Id,
                SubscriptionId = delivery.SubscriptionId,
                Event = delivery.Event,
                Payload = delivery.Payload,
                Status = delivery.Status,
                TargetUrl = delivery.TargetUrl,
                AttemptCount = delivery.AttemptCount,
                NextRetryAt = delivery.NextRetryAt,
                ResponseStatusCode = delivery.ResponseStatusCode,
                ResponseBody = delivery.ResponseBody,
                ErrorMessage = delivery.ErrorMessage,
                DeliveredAt = delivery.DeliveredAt,
                FailedAt = delivery.FailedAt,
                CreatedAt = delivery.CreatedAt
            };
        }

        #endregion
    }
}
