using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents two-factor authentication configuration for a user.
    /// </summary>
    public class TwoFactorAuth : BaseEntity
    {
        public string UserId { get; private set; }
        public bool IsEnabled { get; private set; }
        public TwoFactorMethod Method { get; private set; }
        public string? SecretKey { get; private set; }  // Encrypted for TOTP
        public DateTime? EnabledAt { get; private set; }
        public string? EnabledByIp { get; private set; }
        
        private readonly List<BackupCode> _backupCodes = new();
        public IReadOnlyCollection<BackupCode> BackupCodes => _backupCodes.AsReadOnly();

        private TwoFactorAuth()
        {
            // EF Constructor
            UserId = null!;
        }

        public TwoFactorAuth(
            string userId,
            TwoFactorMethod method,
            string tenantId,
            string? secretKey = null) : base(tenantId)
        {
            UserId = userId ?? throw new ArgumentNullException(nameof(userId));
            Method = method;
            SecretKey = secretKey;
            IsEnabled = false;
        }

        public void Enable(string ipAddress)
        {
            IsEnabled = true;
            EnabledAt = DateTime.UtcNow;
            EnabledByIp = ipAddress;
            UpdateTimestamp();
        }

        public void Disable()
        {
            IsEnabled = false;
            UpdateTimestamp();
        }

        public void AddBackupCode(string code, string hashedCode)
        {
            _backupCodes.Add(new BackupCode(code, hashedCode));
            UpdateTimestamp();
        }

        public void MarkBackupCodeAsUsed(string code)
        {
            var backupCode = _backupCodes.Find(bc => bc.Code == code);
            if (backupCode != null)
            {
                backupCode.MarkAsUsed();
                UpdateTimestamp();
            }
        }
    }

    /// <summary>
    /// Represents a backup code for two-factor authentication recovery.
    /// </summary>
    public class BackupCode
    {
        public string Code { get; private set; }  // Display code (partial, e.g., "XXXX-1234")
        public string HashedCode { get; private set; }  // Full hashed code for verification
        public bool IsUsed { get; private set; }
        public DateTime? UsedAt { get; private set; }

        private BackupCode()
        {
            // EF Constructor
            Code = null!;
            HashedCode = null!;
        }

        public BackupCode(string code, string hashedCode)
        {
            Code = code ?? throw new ArgumentNullException(nameof(code));
            HashedCode = hashedCode ?? throw new ArgumentNullException(nameof(hashedCode));
            IsUsed = false;
        }

        public void MarkAsUsed()
        {
            IsUsed = true;
            UsedAt = DateTime.UtcNow;
        }
    }
}
