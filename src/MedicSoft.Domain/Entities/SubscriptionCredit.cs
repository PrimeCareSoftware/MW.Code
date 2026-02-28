using System;

namespace MedicSoft.Domain.Entities
{
    public class SubscriptionCredit
    {
        public int Id { get; set; }
        public Guid SubscriptionId { get; set; }
        public virtual ClinicSubscription Subscription { get; set; }
        
        public int Days { get; set; }
        public string Reason { get; set; }
        public DateTime GrantedAt { get; set; }
        public Guid GrantedBy { get; set; }
        public virtual User GrantedByUser { get; set; }
        
        public string TenantId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
