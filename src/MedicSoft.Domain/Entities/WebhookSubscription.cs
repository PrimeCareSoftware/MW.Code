using System;
using System.Collections.Generic;

namespace MedicSoft.Domain.Entities
{
    public class WebhookSubscription
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Events { get; set; } // JSON array of event names
        public string Secret { get; set; } // para validação HMAC
        public bool IsEnabled { get; set; }
        public int RetryCount { get; set; } = 3;
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string UpdatedBy { get; set; }
        
        public virtual ICollection<WebhookDelivery> Deliveries { get; set; }
    }
}
