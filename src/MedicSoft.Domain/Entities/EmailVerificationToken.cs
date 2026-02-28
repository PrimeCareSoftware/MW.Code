using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Token for email-based 2FA verification during login
    /// </summary>
    public class EmailVerificationToken : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string Code { get; private set; }
        public string Purpose { get; private set; }  // "Login", "Registration", etc.
        public DateTime CreatedAt { get; private set; }
        public DateTime ExpiresAt { get; private set; }
        public bool IsUsed { get; private set; }
        public DateTime? UsedAt { get; private set; }
        public int VerificationAttempts { get; private set; }
        public string IpAddress { get; private set; }

        private EmailVerificationToken()
        {
            // EF Constructor
            Code = null!;
            Purpose = null!;
            IpAddress = null!;
        }

        public EmailVerificationToken(
            Guid userId,
            string code,
            string purpose,
            string ipAddress,
            string tenantId,
            int expirationMinutes = 5) : base(tenantId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code cannot be empty", nameof(code));

            if (string.IsNullOrWhiteSpace(purpose))
                throw new ArgumentException("Purpose cannot be empty", nameof(purpose));

            if (string.IsNullOrWhiteSpace(ipAddress))
                throw new ArgumentException("IP address cannot be empty", nameof(ipAddress));

            UserId = userId;
            Code = code;
            Purpose = purpose;
            IpAddress = ipAddress;
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);
            IsUsed = false;
            VerificationAttempts = 0;
        }

        public bool IsExpired()
        {
            return DateTime.UtcNow > ExpiresAt;
        }

        public bool IsValid()
        {
            return !IsUsed && !IsExpired() && VerificationAttempts < 5;
        }

        public void IncrementAttempts()
        {
            VerificationAttempts++;
            UpdateTimestamp();
        }

        public void MarkAsUsed()
        {
            if (IsUsed)
                throw new InvalidOperationException("Token has already been used");

            if (IsExpired())
                throw new InvalidOperationException("Token has expired");

            IsUsed = true;
            UsedAt = DateTime.UtcNow;
            UpdateTimestamp();
        }
    }
}
