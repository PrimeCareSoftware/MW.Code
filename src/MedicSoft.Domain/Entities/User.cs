using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a user in the system with role-based access control.
    /// </summary>
    public class User : BaseEntity
    {
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string FullName { get; private set; }
        public string Phone { get; private set; }
        public Guid? ClinicId { get; private set; }
        public UserRole Role { get; private set; }
        public Guid? ProfileId { get; private set; } // New: Access profile for granular permissions
        public bool IsActive { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public string? CurrentSessionId { get; private set; } // Tracks the current active session
        public string? ProfessionalId { get; private set; } // CRM, CRO, etc.
        public string? Specialty { get; private set; }

        // Navigation properties
        public Clinic? Clinic { get; private set; }
        public AccessProfile? Profile { get; private set; }

        private User()
        {
            // EF Constructor
            Username = null!;
            Email = null!;
            PasswordHash = null!;
            FullName = null!;
            Phone = null!;
        }

        public User(string username, string email, string passwordHash, string fullName,
            string phone, UserRole role, string tenantId, Guid? clinicId = null,
            string? professionalId = null, string? specialty = null) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty", nameof(username));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name cannot be empty", nameof(fullName));

            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone cannot be empty", nameof(phone));

            Username = username.Trim().ToLowerInvariant();
            Email = email.Trim().ToLowerInvariant();
            PasswordHash = passwordHash;
            FullName = fullName.Trim();
            Phone = phone.Trim();
            ClinicId = clinicId;
            Role = role;
            ProfessionalId = professionalId?.Trim();
            Specialty = specialty?.Trim();
            IsActive = true;
        }

        public void UpdateProfile(string email, string fullName, string phone,
            string? professionalId = null, string? specialty = null)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            if (string.IsNullOrWhiteSpace(fullName))
                throw new ArgumentException("Full name cannot be empty", nameof(fullName));

            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone cannot be empty", nameof(phone));

            Email = email.Trim().ToLowerInvariant();
            FullName = fullName.Trim();
            Phone = phone.Trim();
            ProfessionalId = professionalId?.Trim();
            Specialty = specialty?.Trim();
            UpdateTimestamp();
        }

        public void UpdatePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));

            PasswordHash = newPasswordHash;
            UpdateTimestamp();
        }

        public void ChangeRole(UserRole newRole)
        {
            Role = newRole;
            UpdateTimestamp();
        }

        public void AssignProfile(Guid? profileId)
        {
            ProfileId = profileId;
            UpdateTimestamp();
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

        public void RecordLogin(string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException("Session ID cannot be empty", nameof(sessionId));
            
            LastLoginAt = DateTime.UtcNow;
            CurrentSessionId = sessionId;
            UpdateTimestamp();
        }
        
        public bool IsSessionValid(string sessionId)
        {
            return !string.IsNullOrWhiteSpace(CurrentSessionId) && 
                   CurrentSessionId == sessionId;
        }

        public bool HasPermission(Permission permission)
        {
            return GetRolePermissions(Role).Contains(permission);
        }

        /// <summary>
        /// Checks if user has a specific permission key (new profile-based system)
        /// </summary>
        public bool HasPermissionKey(string permissionKey)
        {
            // If user has a profile, use profile-based permissions
            if (Profile != null && Profile.IsActive)
            {
                return Profile.HasPermission(permissionKey);
            }

            // Fallback to role-based permissions (for backward compatibility)
            return HasLegacyPermission(permissionKey);
        }

        private static readonly Dictionary<string, Permission> LegacyPermissionMapping = new()
        {
            { "clinic.view", Permission.ManageClinic },
            { "clinic.manage", Permission.ManageClinic },
            { "users.view", Permission.ManageUsers },
            { "users.create", Permission.ManageUsers },
            { "users.edit", Permission.ManageUsers },
            { "users.delete", Permission.ManageUsers },
            { "profiles.view", Permission.ManageUsers },
            { "profiles.create", Permission.ManageUsers },
            { "profiles.edit", Permission.ManageUsers },
            { "profiles.delete", Permission.ManageUsers },
            { "patients.view", Permission.ViewPatients },
            { "patients.create", Permission.ManagePatients },
            { "patients.edit", Permission.ManagePatients },
            { "patients.delete", Permission.ManagePatients },
            { "appointments.view", Permission.ViewAppointments },
            { "appointments.create", Permission.ManageAppointments },
            { "appointments.edit", Permission.ManageAppointments },
            { "appointments.delete", Permission.ManageAppointments },
            { "medical-records.view", Permission.ViewMedicalRecords },
            { "medical-records.create", Permission.ManageMedicalRecords },
            { "medical-records.edit", Permission.ManageMedicalRecords },
            { "attendance.view", Permission.ViewAppointments },
            { "attendance.perform", Permission.ManageMedicalRecords },
            { "procedures.view", Permission.ViewAppointments },
            { "procedures.create", Permission.ManageClinic },
            { "procedures.edit", Permission.ManageClinic },
            { "procedures.delete", Permission.ManageClinic },
            { "payments.view", Permission.ManagePayments },
            { "payments.manage", Permission.ManagePayments },
            { "invoices.view", Permission.ManagePayments },
            { "invoices.manage", Permission.ManagePayments },
            { "expenses.view", Permission.ManagePayments },
            { "expenses.create", Permission.ManagePayments },
            { "expenses.edit", Permission.ManagePayments },
            { "expenses.delete", Permission.ManagePayments },
            { "reports.financial", Permission.ViewReports },
            { "reports.operational", Permission.ViewReports },
            { "medications.view", Permission.ViewMedicalRecords },
            { "prescriptions.create", Permission.ManageMedicalRecords },
            { "exams.view", Permission.ViewMedicalRecords },
            { "exams.request", Permission.ManageMedicalRecords },
            { "notifications.view", Permission.ViewAppointments },
            { "notifications.manage", Permission.ManageAppointments },
            { "waiting-queue.view", Permission.ViewAppointments },
            { "waiting-queue.manage", Permission.ManageAppointments }
        };

        private bool HasLegacyPermission(string permissionKey)
        {
            // Map new permission keys to old Permission enum
            if (LegacyPermissionMapping.TryGetValue(permissionKey, out var permission))
            {
                return HasPermission(permission);
            }

            return false;
        }

        private static Permission[] GetRolePermissions(UserRole role)
        {
            return role switch
            {
                UserRole.SystemAdmin => new[]
                {
                    Permission.ViewAllClinics,
                    Permission.ManageSubscriptions,
                    Permission.ViewSystemAnalytics,
                    Permission.ManagePlans,
                    Permission.CrossTenantAccess,
                    Permission.ManageUsers,
                    Permission.ManageClinic,
                    Permission.ViewPatients,
                    Permission.ManagePatients,
                    Permission.ViewAppointments,
                    Permission.ManageAppointments,
                    Permission.ViewMedicalRecords,
                    Permission.ManageMedicalRecords,
                    Permission.ViewReports,
                    Permission.ManagePayments
                },
                UserRole.ClinicOwner => new[]
                {
                    Permission.ManageUsers,
                    Permission.ManageClinic,
                    Permission.ViewPatients,
                    Permission.ManagePatients,
                    Permission.ViewAppointments,
                    Permission.ManageAppointments,
                    Permission.ViewMedicalRecords,
                    Permission.ManageMedicalRecords,
                    Permission.ViewReports,
                    Permission.ManagePayments,
                    Permission.ManageSubscription
                },
                UserRole.Doctor => new[]
                {
                    Permission.ViewPatients,
                    Permission.ManagePatients,
                    Permission.ViewAppointments,
                    Permission.ManageAppointments,
                    Permission.ViewMedicalRecords,
                    Permission.ManageMedicalRecords
                },
                UserRole.Dentist => new[]
                {
                    Permission.ViewPatients,
                    Permission.ManagePatients,
                    Permission.ViewAppointments,
                    Permission.ManageAppointments,
                    Permission.ViewMedicalRecords,
                    Permission.ManageMedicalRecords
                },
                UserRole.Nurse => new[]
                {
                    Permission.ViewPatients,
                    Permission.ViewAppointments,
                    Permission.ViewMedicalRecords,
                    Permission.ManageMedicalRecords
                },
                UserRole.Receptionist => new[]
                {
                    Permission.ViewPatients,
                    Permission.ManagePatients,
                    Permission.ViewAppointments,
                    Permission.ManageAppointments
                },
                UserRole.Secretary => new[]
                {
                    Permission.ViewPatients,
                    Permission.ManagePatients,
                    Permission.ViewAppointments,
                    Permission.ManageAppointments,
                    Permission.ManagePayments
                },
                _ => Array.Empty<Permission>()
            };
        }
    }

    public enum UserRole
    {
        SystemAdmin,    // Full system access
        ClinicOwner,    // Owner of a clinic
        Doctor,         // Medical doctor
        Dentist,        // Dentist
        Nurse,          // Nurse
        Receptionist,   // Front desk / reception
        Secretary       // Administrative secretary
    }

    public enum Permission
    {
        // System-level permissions
        ViewAllClinics,
        ManageSubscriptions,
        ViewSystemAnalytics,
        ManagePlans,
        CrossTenantAccess,

        // Clinic-level permissions
        ManageClinic,
        ManageUsers,
        ManageSubscription,

        // Patient permissions
        ViewPatients,
        ManagePatients,

        // Appointment permissions
        ViewAppointments,
        ManageAppointments,

        // Medical record permissions
        ViewMedicalRecords,
        ManageMedicalRecords,

        // Reporting permissions
        ViewReports,

        // Payment permissions
        ManagePayments
    }
}
