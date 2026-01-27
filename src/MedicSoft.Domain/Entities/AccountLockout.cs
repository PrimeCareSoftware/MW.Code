using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents an account lockout due to excessive failed login attempts.
    /// </summary>
    public class AccountLockout : BaseEntity
    {
        public string UserId { get; private set; }
        public DateTime LockedAt { get; private set; }
        public DateTime UnlocksAt { get; private set; }
        public int FailedAttempts { get; private set; }
        public bool IsActive { get; private set; }
        public string? UnlockedBy { get; private set; }
        public DateTime? UnlockedAt { get; private set; }

        private AccountLockout()
        {
            // EF Constructor
            UserId = null!;
        }

        public AccountLockout(
            string userId,
            int failedAttempts,
            TimeSpan lockoutDuration,
            string tenantId) : base(tenantId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            FailedAttempts = failedAttempts;
            LockedAt = DateTime.UtcNow;
            UnlocksAt = DateTime.UtcNow.Add(lockoutDuration);
            IsActive = true;
        }

        public void Unlock(string unlockedBy)
        {
            IsActive = false;
            UnlockedBy = unlockedBy;
            UnlockedAt = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public bool IsCurrentlyLocked()
        {
            return IsActive && DateTime.UtcNow < UnlocksAt;
        }
    }
}
