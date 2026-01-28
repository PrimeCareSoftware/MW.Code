using System;

namespace MedicSoft.Domain.Events
{
    public class ClinicCreatedEvent : DomainEvent
    {
        public int ClinicId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; }

        public ClinicCreatedEvent() : base("system")
        {
        }
    }
}
