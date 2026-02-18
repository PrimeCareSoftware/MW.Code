using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// Result of syncing default profile permissions with latest definitions
    /// </summary>
    public class SyncProfilePermissionsResult
    {
        public int ProfilesUpdated { get; set; }
        public int ProfilesSkipped { get; set; }
        public List<ProfileSyncDetail> ProfileDetails { get; set; } = new();
    }

    /// <summary>
    /// Details of permission sync for a specific profile
    /// </summary>
    public class ProfileSyncDetail
    {
        public Guid ProfileId { get; set; }
        public string ProfileName { get; set; } = string.Empty;
        public Guid? ClinicId { get; set; }
        public List<string> PermissionsAdded { get; set; } = new();
        public bool Skipped { get; set; }
        public string? SkipReason { get; set; }
    }
}
