using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents an active owner session in the system.
    /// Enables support for multiple concurrent sessions per owner.
    /// </summary>
    public class OwnerSession : BaseEntity
    {
        public Guid OwnerId { get; private set; }
        public string SessionId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime LastActivityAt { get; private set; }
        public string? UserAgent { get; private set; }
        public string? IpAddress { get; private set; }

        // Navigation property
        public Owner? Owner { get; private set; }

        private OwnerSession()
        {
            // EF Constructor
            SessionId = null!;
        }

        public OwnerSession(Guid ownerId, string sessionId, string tenantId, int expiryHours = 24,
            string? userAgent = null, string? ipAddress = null) : base(tenantId)
        {
            if (ownerId == Guid.Empty)
                throw new ArgumentException("OwnerId cannot be empty", nameof(ownerId));

            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException("SessionId cannot be empty", nameof(sessionId));

            OwnerId = ownerId;
            SessionId = sessionId;
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = DateTime.UtcNow.AddHours(expiryHours);
            LastActivityAt = DateTime.UtcNow;
            UserAgent = userAgent;
            IpAddress = ipAddress;
        }

        /// <summary>
        /// Updates the last activity timestamp and extends the session expiration.
        /// </summary>
        public void UpdateActivity(int expiryHours = 24)
        {
            LastActivityAt = DateTime.UtcNow;
            ExpiresAt = DateTime.UtcNow.AddHours(expiryHours);
            UpdateTimestamp();
        }

        /// <summary>
        /// Checks if the session is still valid (not expired).
        /// </summary>
        public bool IsValid()
        {
            return ExpiresAt > DateTime.UtcNow;
        }
    }
}
