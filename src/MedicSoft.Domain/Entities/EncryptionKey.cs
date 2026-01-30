using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents an encryption key with versioning and lifecycle management.
    /// Supports key rotation and audit trail for compliance.
    /// </summary>
    public class EncryptionKey : BaseEntity
    {
        public string KeyId { get; private set; }
        public int KeyVersion { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime CreatedAt { get; private set; }
        public DateTime? ExpiresAt { get; private set; }
        public DateTime? RotatedAt { get; private set; }
        public Guid? RotatedByUserId { get; private set; }
        public string Algorithm { get; private set; }
        public string Purpose { get; private set; }
        public string? EncryptedKeyMaterial { get; private set; }
        public string? Description { get; private set; }

        private EncryptionKey()
        {
            // EF Constructor
            KeyId = null!;
            Algorithm = null!;
            Purpose = null!;
        }

        public EncryptionKey(
            string keyId,
            int keyVersion,
            string algorithm,
            string purpose,
            string tenantId,
            string? encryptedKeyMaterial = null,
            DateTime? expiresAt = null,
            string? description = null) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(keyId))
                throw new ArgumentException("Key ID cannot be empty", nameof(keyId));

            if (keyVersion < 1)
                throw new ArgumentException("Key version must be at least 1", nameof(keyVersion));

            if (string.IsNullOrWhiteSpace(algorithm))
                throw new ArgumentException("Algorithm cannot be empty", nameof(algorithm));

            if (string.IsNullOrWhiteSpace(purpose))
                throw new ArgumentException("Purpose cannot be empty", nameof(purpose));

            KeyId = keyId;
            KeyVersion = keyVersion;
            Algorithm = algorithm;
            Purpose = purpose;
            IsActive = true;
            CreatedAt = DateTime.UtcNow;
            ExpiresAt = expiresAt;
            EncryptedKeyMaterial = encryptedKeyMaterial;
            Description = description?.Trim();
        }

        public void Activate()
        {
            IsActive = true;
            UpdateTimestamp();
        }

        public void Deactivate(Guid? rotatedByUserId = null)
        {
            IsActive = false;
            RotatedAt = DateTime.UtcNow;
            RotatedByUserId = rotatedByUserId;
            UpdateTimestamp();
        }

        public void SetExpiration(DateTime expiresAt)
        {
            if (expiresAt <= DateTime.UtcNow)
                throw new ArgumentException("Expiration date must be in the future", nameof(expiresAt));

            ExpiresAt = expiresAt;
            UpdateTimestamp();
        }

        public bool IsExpired()
        {
            return ExpiresAt.HasValue && ExpiresAt.Value <= DateTime.UtcNow;
        }

        public bool IsValid()
        {
            return IsActive && !IsExpired();
        }
    }
}
