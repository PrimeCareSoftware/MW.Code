using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents an owner/proprietor in the system.
    /// Clinic owners have administrative privileges for their clinic.
    /// System owners (ClinicId = null) have system-wide administrative privileges.
    /// </summary>
    public class Owner : BaseEntity
    {
        public string Username { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public string FullName { get; private set; }
        public string Phone { get; private set; }
        public Guid? ClinicId { get; private set; }
        public bool IsActive { get; private set; }
        public DateTime? LastLoginAt { get; private set; }
        public string? ProfessionalId { get; private set; } // CRM, CRO, etc. (if owner is also a professional)
        public string? Specialty { get; private set; }

        // Navigation properties
        public Clinic? Clinic { get; private set; }

        /// <summary>
        /// Indicates whether this owner is a system-level owner (not tied to a specific clinic)
        /// </summary>
        public bool IsSystemOwner => !ClinicId.HasValue;

        private Owner()
        {
            // EF Constructor
            Username = null!;
            Email = null!;
            PasswordHash = null!;
            FullName = null!;
            Phone = null!;
        }

        public Owner(string username, string email, string passwordHash, string fullName,
            string phone, string tenantId, Guid? clinicId = null,
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

            if (clinicId.HasValue && clinicId.Value == Guid.Empty)
                throw new ArgumentException("ClinicId cannot be empty Guid", nameof(clinicId));

            Username = username.Trim().ToLowerInvariant();
            Email = email.Trim().ToLowerInvariant();
            PasswordHash = passwordHash;
            FullName = fullName.Trim();
            Phone = phone.Trim();
            ClinicId = clinicId;
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

        public void RecordLogin()
        {
            LastLoginAt = DateTime.UtcNow;
            UpdateTimestamp();
        }
    }
}
