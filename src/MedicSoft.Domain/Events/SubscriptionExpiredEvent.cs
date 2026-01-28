using System;

namespace MedicSoft.Domain.Events
{
    public class SubscriptionExpiredEvent : DomainEvent
    {
        public int ClinicId { get; set; }
        public int SubscriptionId { get; set; }
        public DateTime ExpiredAt { get; set; }
        public string ClinicName { get; set; }
        public string Email { get; set; }

        public SubscriptionExpiredEvent() : base("system")
        {
        }
    }
}
