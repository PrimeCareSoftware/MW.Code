using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a login attempt for brute force protection and auditing.
    /// </summary>
    public class LoginAttempt : BaseEntity
    {
        public string Username { get; private set; }
        public string IpAddress { get; private set; }
        public DateTime AttemptTime { get; private set; }
        public bool WasSuccessful { get; private set; }
        public string? FailureReason { get; private set; }

        private LoginAttempt()
        {
            // EF Constructor
            Username = null!;
            IpAddress = null!;
        }

        public LoginAttempt(
            string username, 
            string ipAddress, 
            bool wasSuccessful, 
            string tenantId,
            string? failureReason = null) : base(tenantId)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            IpAddress = ipAddress ?? throw new ArgumentNullException(nameof(ipAddress));
            AttemptTime = DateTime.UtcNow;
            WasSuccessful = wasSuccessful;
            FailureReason = failureReason;
        }
    }
}
