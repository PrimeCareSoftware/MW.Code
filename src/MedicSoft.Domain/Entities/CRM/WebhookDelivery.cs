using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Status da entrega do webhook
    /// </summary>
    public enum WebhookDeliveryStatus
    {
        Pending = 1,
        Delivered = 2,
        Failed = 3,
        Retrying = 4
    }
    
    /// <summary>
    /// Record of a webhook delivery attempt
    /// </summary>
    public class WebhookDelivery : BaseEntity
    {
        public Guid SubscriptionId { get; private set; }
        public WebhookEvent Event { get; private set; }
        public string Payload { get; private set; }
        public WebhookDeliveryStatus Status { get; private set; }
        
        // Delivery details
        public string TargetUrl { get; private set; }
        public int AttemptCount { get; private set; }
        public DateTime? NextRetryAt { get; private set; }
        
        // Response details
        public int? ResponseStatusCode { get; private set; }
        public string? ResponseBody { get; private set; }
        public string? ErrorMessage { get; private set; }
        
        // Timestamps
        public DateTime? DeliveredAt { get; private set; }
        public DateTime? FailedAt { get; private set; }
        
        private WebhookDelivery()
        {
            Payload = string.Empty;
            TargetUrl = string.Empty;
        }
        
        public WebhookDelivery(
            Guid subscriptionId,
            WebhookEvent webhookEvent,
            string payload,
            string targetUrl,
            string tenantId) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(payload))
                throw new ArgumentException("Payload cannot be null or empty", nameof(payload));
            if (string.IsNullOrWhiteSpace(targetUrl))
                throw new ArgumentException("Target URL cannot be null or empty", nameof(targetUrl));
                
            SubscriptionId = subscriptionId;
            Event = webhookEvent;
            Payload = payload;
            TargetUrl = targetUrl;
            Status = WebhookDeliveryStatus.Pending;
            AttemptCount = 0;
        }
        
        public void MarkAsDelivered(int statusCode, string responseBody)
        {
            Status = WebhookDeliveryStatus.Delivered;
            DeliveredAt = DateTime.UtcNow;
            ResponseStatusCode = statusCode;
            ResponseBody = responseBody;
            UpdateTimestamp();
        }
        
        public void MarkAsFailed(string errorMessage, int? statusCode = null, string? responseBody = null)
        {
            Status = WebhookDeliveryStatus.Failed;
            FailedAt = DateTime.UtcNow;
            ErrorMessage = errorMessage;
            ResponseStatusCode = statusCode;
            ResponseBody = responseBody;
            UpdateTimestamp();
        }
        
        public void ScheduleRetry(int delaySeconds)
        {
            Status = WebhookDeliveryStatus.Retrying;
            AttemptCount++;
            NextRetryAt = DateTime.UtcNow.AddSeconds(delaySeconds * Math.Pow(2, AttemptCount - 1)); // Exponential backoff
            UpdateTimestamp();
        }
        
        public bool CanRetry(int maxRetries)
        {
            return AttemptCount < maxRetries && Status != WebhookDeliveryStatus.Delivered;
        }
    }
}
