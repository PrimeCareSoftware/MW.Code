using System;

namespace MedicSoft.Domain.Events
{
    public abstract class DomainEvent
    {
        public Guid Id { get; } = Guid.NewGuid();
        public DateTime OccurredOn { get; } = DateTime.UtcNow;
        public string TenantId { get; }

        protected DomainEvent(string tenantId)
        {
            TenantId = tenantId;
        }
    }
}