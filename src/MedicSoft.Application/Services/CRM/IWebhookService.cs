using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs.CRM;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Application.Services.CRM
{
    /// <summary>
    /// Service for managing webhook subscriptions and deliveries
    /// </summary>
    public interface IWebhookService
    {
        // Subscription Management
        Task<WebhookSubscriptionDto> CreateSubscriptionAsync(CreateWebhookSubscriptionDto dto, string tenantId);
        Task<WebhookSubscriptionDto?> GetSubscriptionAsync(Guid id, string tenantId);
        Task<List<WebhookSubscriptionDto>> GetAllSubscriptionsAsync(string tenantId);
        Task<WebhookSubscriptionDto> UpdateSubscriptionAsync(Guid id, UpdateWebhookSubscriptionDto dto, string tenantId);
        Task DeleteSubscriptionAsync(Guid id, string tenantId);
        Task<WebhookSubscriptionDto> ActivateSubscriptionAsync(Guid id, string tenantId);
        Task<WebhookSubscriptionDto> DeactivateSubscriptionAsync(Guid id, string tenantId);
        Task<WebhookSubscriptionDto> RegenerateSecretAsync(Guid id, string tenantId);
        
        // Event Publishing
        Task PublishEventAsync(WebhookEvent webhookEvent, object data, string tenantId);
        
        // Delivery Management
        Task<List<WebhookDeliveryDto>> GetDeliveriesAsync(Guid subscriptionId, string tenantId, int limit = 50);
        Task<WebhookDeliveryDto?> GetDeliveryAsync(Guid id, string tenantId);
        Task ProcessPendingDeliveriesAsync();
        Task RetryFailedDeliveryAsync(Guid deliveryId, string tenantId);
    }
}
