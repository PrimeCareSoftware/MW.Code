using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities.CRM
{
    /// <summary>
    /// Webhook subscription for CRM events
    /// </summary>
    public class WebhookSubscription : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string TargetUrl { get; private set; }
        public bool IsActive { get; private set; }
        
        // Security
        public string Secret { get; private set; } // For signature validation
        
        // Events to subscribe to
        private readonly List<WebhookEvent> _subscribedEvents = new();
        public IReadOnlyCollection<WebhookEvent> SubscribedEvents => _subscribedEvents.AsReadOnly();
        
        // Retry configuration
        public int MaxRetries { get; private set; } = 3;
        public int RetryDelaySeconds { get; private set; } = 60;
        
        // Metrics
        public int TotalDeliveries { get; private set; }
        public int SuccessfulDeliveries { get; private set; }
        public int FailedDeliveries { get; private set; }
        public DateTime? LastDeliveryAt { get; private set; }
        public DateTime? LastSuccessAt { get; private set; }
        public DateTime? LastFailureAt { get; private set; }
        
        private WebhookSubscription()
        {
            Name = string.Empty;
            Description = string.Empty;
            TargetUrl = string.Empty;
            Secret = string.Empty;
        }
        
        public WebhookSubscription(
            string name,
            string description,
            string targetUrl,
            string tenantId) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be null or empty", nameof(name));
            if (string.IsNullOrWhiteSpace(targetUrl))
                throw new ArgumentException("Target URL cannot be null or empty", nameof(targetUrl));
                
            Name = name;
            Description = description ?? string.Empty;
            TargetUrl = targetUrl;
            Secret = GenerateSecret();
            IsActive = false;
        }
        
        public void Activate()
        {
            IsActive = true;
            UpdateTimestamp();
        }
        
        public void Deactivate()
        {
            IsActive = false;
            UpdateTimestamp();
        }
        
        public void UpdateTargetUrl(string targetUrl)
        {
            if (string.IsNullOrWhiteSpace(targetUrl))
                throw new ArgumentException("Target URL cannot be null or empty", nameof(targetUrl));
                
            TargetUrl = targetUrl;
            UpdateTimestamp();
        }
        
        public void RegenerateSecret()
        {
            Secret = GenerateSecret();
            UpdateTimestamp();
        }
        
        public void SubscribeToEvent(WebhookEvent webhookEvent)
        {
            if (!_subscribedEvents.Contains(webhookEvent))
            {
                _subscribedEvents.Add(webhookEvent);
                UpdateTimestamp();
            }
        }
        
        public void UnsubscribeFromEvent(WebhookEvent webhookEvent)
        {
            if (_subscribedEvents.Contains(webhookEvent))
            {
                _subscribedEvents.Remove(webhookEvent);
                UpdateTimestamp();
            }
        }
        
        public void ConfigureRetry(int maxRetries, int retryDelaySeconds)
        {
            if (maxRetries < 0)
                throw new ArgumentException("Max retries cannot be negative", nameof(maxRetries));
            if (retryDelaySeconds < 0)
                throw new ArgumentException("Retry delay cannot be negative", nameof(retryDelaySeconds));
                
            MaxRetries = maxRetries;
            RetryDelaySeconds = retryDelaySeconds;
            UpdateTimestamp();
        }
        
        public void RecordDeliveryAttempt(bool success)
        {
            TotalDeliveries++;
            LastDeliveryAt = DateTime.UtcNow;
            
            if (success)
            {
                SuccessfulDeliveries++;
                LastSuccessAt = DateTime.UtcNow;
            }
            else
            {
                FailedDeliveries++;
                LastFailureAt = DateTime.UtcNow;
            }
            
            UpdateTimestamp();
        }
        
        private static string GenerateSecret()
        {
            return Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        }
    }
}
