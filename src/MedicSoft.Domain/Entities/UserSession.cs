using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents an active user session in the system.
    /// Enables support for multiple concurrent sessions per user.
    /// </summary>
    public class UserSession : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string SessionId { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public DateTime LastActivityAt { get; private set; }
        public string? UserAgent { get; private set; }
        public string? IpAddress { get; private set; }

        // Navigation property
        public User? User { get; private set; }

        private UserSession()
        {
            // EF Constructor
            SessionId = null!;
        }

        public UserSession(Guid userId, string sessionId, string tenantId, int expiryHours = 24, 
            string? userAgent = null, string? ipAddress = null) : base(tenantId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("UserId cannot be empty", nameof(userId));

            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException("SessionId cannot be empty", nameof(sessionId));

            UserId = userId;
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
