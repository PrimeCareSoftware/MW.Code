using System;

namespace MedicSoft.Domain.Events
{
    public class InactivityDetectedEvent : DomainEvent
    {
        public Guid ClinicId { get; set; }
        public int DaysSinceLastActivity { get; set; }
        public DateTime LastActivityAt { get; set; }
        public string ClinicName { get; set; }
        public string Email { get; set; }

        public InactivityDetectedEvent() : base("system")
        {
        }
    }
}
