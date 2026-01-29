using System;

namespace MedicSoft.Domain.Events
{
    public class SubscriptionExpiredEvent : DomainEvent
    {
        public Guid ClinicId { get; set; }
        public Guid SubscriptionId { get; set; }
        public DateTime ExpiredAt { get; set; }
        public string ClinicName { get; set; }
        public string Email { get; set; }

        public SubscriptionExpiredEvent() : base("system")
        {
        }
    }
}
