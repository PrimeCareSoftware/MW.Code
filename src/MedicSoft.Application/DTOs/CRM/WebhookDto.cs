using System;
using System.Collections.Generic;
using MedicSoft.Domain.Entities.CRM;

namespace MedicSoft.Application.DTOs.CRM
{
    /// <summary>
    /// DTO for creating a webhook subscription
    /// </summary>
    public class CreateWebhookSubscriptionDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TargetUrl { get; set; } = string.Empty;
        public List<WebhookEvent> SubscribedEvents { get; set; } = new();
        public int? MaxRetries { get; set; }
        public int? RetryDelaySeconds { get; set; }
    }
    
    /// <summary>
    /// DTO for updating a webhook subscription
    /// </summary>
    public class UpdateWebhookSubscriptionDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? TargetUrl { get; set; }
        public bool? IsActive { get; set; }
        public List<WebhookEvent>? SubscribedEvents { get; set; }
        public int? MaxRetries { get; set; }
        public int? RetryDelaySeconds { get; set; }
    }
    
    /// <summary>
    /// DTO for webhook subscription response
    /// </summary>
    public class WebhookSubscriptionDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string TargetUrl { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string Secret { get; set; } = string.Empty;
        public List<WebhookEvent> SubscribedEvents { get; set; } = new();
        public int MaxRetries { get; set; }
        public int RetryDelaySeconds { get; set; }
        
        // Metrics
        public int TotalDeliveries { get; set; }
        public int SuccessfulDeliveries { get; set; }
        public int FailedDeliveries { get; set; }
        public DateTime? LastDeliveryAt { get; set; }
        public DateTime? LastSuccessAt { get; set; }
        public DateTime? LastFailureAt { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
    
    /// <summary>
    /// DTO for webhook delivery response
    /// </summary>
    public class WebhookDeliveryDto
    {
        public Guid Id { get; set; }
        public Guid SubscriptionId { get; set; }
        public WebhookEvent Event { get; set; }
        public string Payload { get; set; } = string.Empty;
        public WebhookDeliveryStatus Status { get; set; }
        public string TargetUrl { get; set; } = string.Empty;
        public int AttemptCount { get; set; }
        public DateTime? NextRetryAt { get; set; }
        public int? ResponseStatusCode { get; set; }
        public string? ResponseBody { get; set; }
        public string? ErrorMessage { get; set; }
        public DateTime? DeliveredAt { get; set; }
        public DateTime? FailedAt { get; set; }
        public DateTime CreatedAt { get; set; }
    }
    
    /// <summary>
    /// DTO for webhook event payload
    /// </summary>
    public class WebhookPayloadDto
    {
        public Guid Id { get; set; }
        public string Event { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; }
        public string TenantId { get; set; } = string.Empty;
        public object Data { get; set; } = new { };
    }
}
