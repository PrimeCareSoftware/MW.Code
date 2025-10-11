using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Token for password reset with 2FA verification
    /// </summary>
    public class PasswordResetToken : BaseEntity
    {
        public Guid UserId { get; private set; }
        public string Token { get; private set; }
        public string VerificationCode { get; private set; }
        public VerificationMethod Method { get; private set; }
        public string Destination { get; private set; } // Email or Phone
        public DateTime ExpiresAt { get; private set; }
        public bool IsUsed { get; private set; }
        public bool IsVerified { get; private set; }
        public DateTime? VerifiedAt { get; private set; }
        public DateTime? UsedAt { get; private set; }
        public int VerificationAttempts { get; private set; }

        // Navigation properties
        public User? User { get; private set; }

        private PasswordResetToken()
        {
            // EF Constructor
            Token = null!;
            VerificationCode = null!;
            Destination = null!;
        }

        public PasswordResetToken(
            Guid userId,
            string token,
            string verificationCode,
            VerificationMethod method,
            string destination,
            string tenantId,
            int expirationMinutes = 15) : base(tenantId)
        {
            if (userId == Guid.Empty)
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentException("Token cannot be empty", nameof(token));

            if (string.IsNullOrWhiteSpace(verificationCode))
                throw new ArgumentException("Verification code cannot be empty", nameof(verificationCode));

            if (string.IsNullOrWhiteSpace(destination))
                throw new ArgumentException("Destination cannot be empty", nameof(destination));

            UserId = userId;
            Token = token;
            VerificationCode = verificationCode;
            Method = method;
            Destination = destination.Trim();
            ExpiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);
            IsUsed = false;
            IsVerified = false;
            VerificationAttempts = 0;
        }

        public bool IsExpired()
        {
            return DateTime.UtcNow > ExpiresAt;
        }

        public bool IsValid()
        {
            return !IsUsed && !IsExpired() && IsVerified;
        }

        public void Verify()
        {
            if (IsExpired())
                throw new InvalidOperationException("Token has expired");

            if (IsUsed)
                throw new InvalidOperationException("Token has already been used");

            IsVerified = true;
            VerifiedAt = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void IncrementVerificationAttempts()
        {
            VerificationAttempts++;
            UpdateTimestamp();
        }

        public void MarkAsUsed()
        {
            if (!IsVerified)
                throw new InvalidOperationException("Token must be verified before use");

            if (IsUsed)
                throw new InvalidOperationException("Token has already been used");

            if (IsExpired())
                throw new InvalidOperationException("Token has expired");

            IsUsed = true;
            UsedAt = DateTime.UtcNow;
            UpdateTimestamp();
        }
    }

    public enum VerificationMethod
    {
        Email,
        SMS
    }
}
