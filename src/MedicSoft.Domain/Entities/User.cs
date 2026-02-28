using System;
using System.Collections.Generic;
using System.Linq;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;

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
        public Guid? ClinicId { get; private set; } // Deprecated: For backward compatibility. Use ClinicLinks instead.
        public UserRole Role { get; private set; }
        public Guid? ProfileId { get; private set; } // New: Access profile for granular permissions
        public bool IsActive { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public string? CurrentSessionId { get; private set; } // Tracks the current active session
        public string? ProfessionalId { get; private set; } // CRM, CRO, etc.
        public string? Specialty { get; private set; } // Legacy specialty field (kept for backward compatibility)
        public ProfessionalSpecialty? ProfessionalSpecialty { get; private set; } // Strongly-typed specialty from AccessProfile
        public Guid? CurrentClinicId { get; private set; } // The clinic where the user is currently working
        public DateTime? MfaGracePeriodEndsAt { get; private set; } // Grace period for MFA setup
        public DateTime? FirstLoginAt { get; private set; } // Track first login to calculate grace period
        public bool ShowInAppointmentScheduling { get; private set; } // Whether this user should appear in appointment scheduling dropdowns
        public string? CalendarColor { get; private set; } // Hex color for calendar display (e.g., "#4CAF50")
        public bool MustChangePassword { get; private set; } // Forces password change on next login

        // Navigation properties
        public Clinic? Clinic { get; private set; } // Deprecated navigation
        public AccessProfile? Profile { get; private set; }
        public Clinic? CurrentClinic { get; private set; } // The clinic where the user is currently working
        
        // New: Collection of clinics this user can access
        private readonly List<UserClinicLink> _clinicLinks = new();
        public IReadOnlyCollection<UserClinicLink> ClinicLinks => _clinicLinks.AsReadOnly();

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
            string? professionalId = null, string? specialty = null, bool showInAppointmentScheduling = true, 
            string? calendarColor = null) : base(tenantId)
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
            ShowInAppointmentScheduling = showInAppointmentScheduling;
            CalendarColor = ValidateAndFormatColor(calendarColor);
        }

        public void UpdateProfile(string email, string fullName, string phone,
            string? professionalId = null, string? specialty = null, bool? showInAppointmentScheduling = null,
            string? calendarColor = null)
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
            if (showInAppointmentScheduling.HasValue)
                ShowInAppointmentScheduling = showInAppointmentScheduling.Value;
            if (calendarColor != null)
                CalendarColor = ValidateAndFormatColor(calendarColor);
            UpdateTimestamp();
        }

        private static string? ValidateAndFormatColor(string? color)
        {
            if (string.IsNullOrWhiteSpace(color))
                return null;

            var trimmedColor = color.Trim();
            
            // Check if it matches hex color format (#RRGGBB or #RGB)
            if (System.Text.RegularExpressions.Regex.IsMatch(trimmedColor, @"^#([0-9A-Fa-f]{6}|[0-9A-Fa-f]{3})$"))
                return trimmedColor.ToUpperInvariant();

            throw new ArgumentException("Calendar color must be a valid hex color format (e.g., #FF5733 or #F57)", nameof(color));
        }

        public void UpdatePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));

            PasswordHash = newPasswordHash;
            MustChangePassword = false;
            UpdateTimestamp();
        }

        public void SetMustChangePassword(bool value)
        {
            MustChangePassword = value;
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

        /// <summary>
        /// Synchronizes the user's professional specialty with their assigned access profile
        /// This should be called after loading the Profile navigation property
        /// </summary>
        public void SyncSpecialtyFromProfile()
        {
            if (Profile?.ConsultationFormProfile != null)
            {
                ProfessionalSpecialty = Profile.ConsultationFormProfile.Specialty;
                // Also update the legacy Specialty string for backward compatibility
                Specialty = Profile.ConsultationFormProfile.Specialty.ToString();
                UpdateTimestamp();
            }
        }

        /// <summary>
        /// Sets the professional specialty directly (for users without profiles or manual override)
        /// </summary>
        public void SetProfessionalSpecialty(ProfessionalSpecialty? specialty)
        {
            ProfessionalSpecialty = specialty;
            if (specialty.HasValue)
            {
                Specialty = specialty.Value.ToString();
            }
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

        public void RecordLogin(string sessionId, int gracePeriodDays = 7)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException("Session ID cannot be empty", nameof(sessionId));
            
            LastLoginAt = DateTime.UtcNow;
            CurrentSessionId = sessionId;
            
            // Set first login time and grace period if this is the first login
            if (!FirstLoginAt.HasValue)
            {
                FirstLoginAt = DateTime.UtcNow;
                if (MfaRequiredByPolicy)
                {
                    MfaGracePeriodEndsAt = DateTime.UtcNow.AddDays(gracePeriodDays);
                }
            }
            
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
            { "company.view", Permission.ManageClinic },
            { "company.edit", Permission.ManageClinic },
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
            { "medical-records.delete", Permission.ManageMedicalRecords },
            { "attendance.view", Permission.ViewAppointments },
            { "attendance.perform", Permission.ManageMedicalRecords },
            { "procedures.view", Permission.ViewAppointments },
            { "procedures.create", Permission.ManageClinic },
            { "procedures.edit", Permission.ManageClinic },
            { "procedures.delete", Permission.ManageClinic },
            { "procedures.manage", Permission.ManageClinic },
            { "payments.view", Permission.ManagePayments },
            { "payments.manage", Permission.ManagePayments },
            { "invoices.view", Permission.ManagePayments },
            { "invoices.manage", Permission.ManagePayments },
            { "expenses.view", Permission.ManagePayments },
            { "expenses.create", Permission.ManagePayments },
            { "expenses.edit", Permission.ManagePayments },
            { "expenses.delete", Permission.ManagePayments },
            { "accounts-receivable.view", Permission.ManagePayments },
            { "accounts-receivable.manage", Permission.ManagePayments },
            { "accounts-payable.view", Permission.ManagePayments },
            { "accounts-payable.manage", Permission.ManagePayments },
            { "suppliers.view", Permission.ManagePayments },
            { "suppliers.manage", Permission.ManagePayments },
            { "cash-flow.view", Permission.ManagePayments },
            { "cash-flow.manage", Permission.ManagePayments },
            { "financial-closure.view", Permission.ManagePayments },
            { "financial-closure.manage", Permission.ManagePayments },
            { "reports.financial", Permission.ViewReports },
            { "reports.operational", Permission.ViewReports },
            { "medications.view", Permission.ViewMedicalRecords },
            { "prescriptions.create", Permission.ManageMedicalRecords },
            { "exams.view", Permission.ViewMedicalRecords },
            { "exams.request", Permission.ManageMedicalRecords },
            { "health-insurance.view", Permission.ViewPatients },
            { "health-insurance.create", Permission.ManageClinic },
            { "health-insurance.edit", Permission.ManageClinic },
            { "health-insurance.delete", Permission.ManageClinic },
            { "tiss.view", Permission.ViewMedicalRecords },
            { "tiss.create", Permission.ManageMedicalRecords },
            { "tiss.edit", Permission.ManageMedicalRecords },
            { "tiss.delete", Permission.ManageMedicalRecords },
            { "tuss.view", Permission.ViewMedicalRecords },
            { "tuss.create", Permission.ManageClinic },
            { "tuss.edit", Permission.ManageClinic },
            { "tuss.delete", Permission.ManageClinic },
            { "authorizations.view", Permission.ViewMedicalRecords },
            { "authorizations.create", Permission.ManageMedicalRecords },
            { "authorizations.edit", Permission.ManageMedicalRecords },
            { "authorizations.delete", Permission.ManageMedicalRecords },
            { "form-configuration.view", Permission.ManageClinic },
            { "form-configuration.manage", Permission.ManageClinic },
            { "complaints.view", Permission.ViewPatients },
            { "complaints.create", Permission.ManageClinic },
            { "complaints.edit", Permission.ManageClinic },
            { "complaints.delete", Permission.ManageClinic },
            { "complaints.manage", Permission.ManageClinic },
            { "surveys.view", Permission.ViewPatients },
            { "surveys.create", Permission.ManageClinic },
            { "surveys.edit", Permission.ManageClinic },
            { "surveys.delete", Permission.ManageClinic },
            { "surveys.manage", Permission.ManageClinic },
            { "patient-journey.view", Permission.ViewPatients },
            { "patient-journey.manage", Permission.ManageClinic },
            { "marketing-automation.view", Permission.ViewPatients },
            { "marketing-automation.create", Permission.ManageClinic },
            { "marketing-automation.edit", Permission.ManageClinic },
            { "marketing-automation.delete", Permission.ManageClinic },
            { "marketing-automation.manage", Permission.ManageClinic },
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
                UserRole.Psychologist => new[]
                {
                    Permission.ViewPatients,
                    Permission.ManagePatients,
                    Permission.ViewAppointments,
                    Permission.ManageAppointments,
                    Permission.ViewMedicalRecords,
                    Permission.ManageMedicalRecords
                },
                _ => Array.Empty<Permission>()
            };
        }

        /// <summary>
        /// Sets the current clinic where the user is working.
        /// The clinic must be one of the user's assigned clinics.
        /// </summary>
        public void SetCurrentClinic(Guid clinicId)
        {
            if (clinicId == Guid.Empty)
                throw new ArgumentException("Clinic ID cannot be empty", nameof(clinicId));

            // For backward compatibility, also check the legacy ClinicId
            var hasAccess = _clinicLinks.Any(l => l.ClinicId == clinicId && l.IsActive) || 
                           (ClinicId.HasValue && ClinicId.Value == clinicId);

            if (!hasAccess)
                throw new InvalidOperationException("User does not have access to this clinic");

            CurrentClinicId = clinicId;
            UpdateTimestamp();
        }

        /// <summary>
        /// Adds a clinic link for this user
        /// </summary>
        public void AddClinicLink(UserClinicLink clinicLink)
        {
            if (clinicLink == null)
                throw new ArgumentNullException(nameof(clinicLink));

            if (clinicLink.UserId != Id)
                throw new ArgumentException("Clinic link does not belong to this user", nameof(clinicLink));

            _clinicLinks.Add(clinicLink);
            UpdateTimestamp();
        }

        /// <summary>
        /// Gets all active clinic links for this user
        /// </summary>
        public IEnumerable<UserClinicLink> GetActiveClinicLinks()
        {
            return _clinicLinks.Where(l => l.IsActive);
        }

        /// <summary>
        /// Gets the preferred clinic for this user (where they authenticate by default)
        /// </summary>
        public Guid? GetPreferredClinicId()
        {
            return _clinicLinks.FirstOrDefault(l => l.IsActive && l.IsPreferredClinic)?.ClinicId 
                   ?? _clinicLinks.FirstOrDefault(l => l.IsActive)?.ClinicId
                   ?? ClinicId; // Fallback to legacy ClinicId
        }

        /// <summary>
        /// Checks if user has access to a specific clinic
        /// </summary>
        public bool HasAccessToClinic(Guid clinicId)
        {
            return _clinicLinks.Any(l => l.ClinicId == clinicId && l.IsActive) ||
                   (ClinicId.HasValue && ClinicId.Value == clinicId);
        }

        /// <summary>
        /// Checks if MFA is required for this user based on their role
        /// </summary>
        public bool MfaRequiredByPolicy => Role == UserRole.SystemAdmin || Role == UserRole.ClinicOwner;

        /// <summary>
        /// Checks if user is within the MFA setup grace period
        /// </summary>
        public bool IsInMfaGracePeriod => MfaGracePeriodEndsAt.HasValue && DateTime.UtcNow < MfaGracePeriodEndsAt.Value;

        /// <summary>
        /// Checks if MFA grace period has expired
        /// </summary>
        public bool MfaGracePeriodExpired => MfaGracePeriodEndsAt.HasValue && DateTime.UtcNow >= MfaGracePeriodEndsAt.Value;

        /// <summary>
        /// Clears the MFA grace period (called after MFA is enabled)
        /// </summary>
        public void ClearMfaGracePeriod()
        {
            MfaGracePeriodEndsAt = null;
            UpdateTimestamp();
        }

        /// <summary>
        /// Determines if this user is a healthcare professional (can perform appointments)
        /// Excludes administrative roles like SystemAdmin, ClinicOwner, Receptionist, and Secretary
        /// </summary>
        public bool IsProfessional()
        {
            return Role switch
            {
                UserRole.Doctor => true,
                UserRole.Dentist => true,
                UserRole.Nurse => true,
                UserRole.Psychologist => true,
                UserRole.SystemAdmin => false,
                UserRole.ClinicOwner => false,
                UserRole.Receptionist => false,
                UserRole.Secretary => false,
                _ => false
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
        Secretary,      // Administrative secretary
        Psychologist    // Psychologist (Psicólogo)
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
