using System;

namespace MedicSoft.Domain.Common
{
    public abstract class BaseEntity
    {
        public Guid Id { get; protected set; } = Guid.NewGuid();
        public DateTime CreatedAt { get; protected set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; protected set; }
        public string TenantId { get; protected set; } = string.Empty;

        protected BaseEntity() { }

        protected BaseEntity(string tenantId)
        {
            TenantId = tenantId;
        }

        public void UpdateTimestamp()
        {
            UpdatedAt = DateTime.UtcNow;
        }
    }
}