namespace MedicSoft.CrossCutting.Authorization
{
    /// <summary>
    /// Custom claim types used throughout the application
    /// </summary>
    public static class CustomClaimTypes
    {
        /// <summary>
        /// Claim that indicates whether the user is a System Owner (platform administrator)
        /// Value should be "true" for System Owners, absent or any other value for regular users
        /// </summary>
        public const string IsSystemOwner = "is_system_owner";
        
        /// <summary>
        /// Claim for the tenant ID
        /// </summary>
        public const string TenantId = "tenant_id";
        
        /// <summary>
        /// Claim for the clinic ID
        /// </summary>
        public const string ClinicId = "clinic_id";
        
        /// <summary>
        /// Claim for the session ID
        /// </summary>
        public const string SessionId = "session_id";
        
        /// <summary>
        /// Claim for the owner ID (used when authenticating as an owner)
        /// </summary>
        public const string OwnerId = "owner_id";
    }
}
