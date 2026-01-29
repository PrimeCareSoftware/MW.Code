using System;

namespace MedicSoft.Domain.Events
{
    public class TrialExpiringEvent : DomainEvent
    {
        public Guid ClinicId { get; set; }
        public Guid SubscriptionId { get; set; }
        public int DaysRemaining { get; set; }
        public DateTime TrialEndsAt { get; set; }
        public string ClinicName { get; set; }
        public string Email { get; set; }

        public TrialExpiringEvent() : base("system")
        {
        }
    }
}
