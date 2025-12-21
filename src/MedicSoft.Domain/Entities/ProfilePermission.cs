using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a permission assigned to an access profile.
    /// Permissions follow the pattern: "resource.action" (e.g., "patients.view", "appointments.create")
    /// </summary>
    public class ProfilePermission : BaseEntity
    {
        public Guid ProfileId { get; private set; }
        public string PermissionKey { get; private set; }
        public bool IsActive { get; private set; }

        // Navigation properties
        public AccessProfile Profile { get; private set; } = null!;

        private ProfilePermission()
        {
            // EF Constructor
            PermissionKey = null!;
        }

        public ProfilePermission(Guid profileId, string permissionKey, string tenantId) : base(tenantId)
        {
            if (profileId == Guid.Empty)
                throw new ArgumentException("Profile ID cannot be empty", nameof(profileId));

            if (string.IsNullOrWhiteSpace(permissionKey))
                throw new ArgumentException("Permission key cannot be empty", nameof(permissionKey));

            ProfileId = profileId;
            PermissionKey = permissionKey.Trim().ToLowerInvariant();
            IsActive = true;
        }

        public void Activate()
        {
            IsActive = true;
            UpdateTimestamp();
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdateTimestamp();
        }
    }
}
