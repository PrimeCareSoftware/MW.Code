using System;

namespace MedicSoft.Domain.Entities
{
    public class WebhookDelivery
    {
        public int Id { get; set; }
        public int WebhookSubscriptionId { get; set; }
        public virtual WebhookSubscription Subscription { get; set; }
        
        public string Event { get; set; }
        public string Payload { get; set; }
        public string Status { get; set; } // pending, delivered, failed
        public int AttemptCount { get; set; }
        public string ResponseCode { get; set; }
        public string ResponseBody { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
}
