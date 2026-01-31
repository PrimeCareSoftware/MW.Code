namespace MedicSoft.Domain.Common
{
    /// <summary>
    /// Constants for user roles used throughout the application
    /// </summary>
    public static class RoleNames
    {
        public const string SystemAdmin = "SystemAdmin";
        public const string ClinicOwner = "ClinicOwner";
        public const string Doctor = "Doctor";
        public const string Dentist = "Dentist";
        public const string Nurse = "Nurse";
        public const string Receptionist = "Receptionist";
        public const string Secretary = "Secretary";
        
        /// <summary>
        /// Legacy role name for clinic owners. Use ClinicOwner instead.
        /// Kept for backward compatibility with existing tokens.
        /// </summary>
        [System.Obsolete("Use ClinicOwner instead. This constant is kept for backward compatibility.")]
        public const string Owner = "Owner";
    }
}
