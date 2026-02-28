using System;
using System.Linq;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Services;

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
        public string? CurrentSessionId { get; private set; } // Tracks the current active session
        public string? ProfessionalId { get; private set; } // CRM, CRO, etc. (if owner is also a professional)
        public string? Specialty { get; private set; }
        public string? Document { get; private set; } // CPF or CNPJ
        public DocumentType? DocumentType { get; private set; } // Type of document (CPF or CNPJ)
        public bool IsEmailConfirmed { get; private set; }
        public string? EmailConfirmationToken { get; private set; }
        public DateTime? EmailConfirmationTokenExpiresAt { get; private set; }
        public bool MustChangePassword { get; private set; } // Forces password change on next login

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
            string? professionalId = null, string? specialty = null, 
            string? document = null, DocumentType? documentType = null) : base(tenantId)
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

            // Validate document if provided
            if (!string.IsNullOrWhiteSpace(document))
            {
                if (!documentType.HasValue)
                    throw new ArgumentException("Document type must be specified when document is provided", nameof(documentType));

                ValidateDocument(document, documentType.Value);
            }

            Username = username.Trim().ToLowerInvariant();
            Email = email.Trim().ToLowerInvariant();
            PasswordHash = passwordHash;
            FullName = fullName.Trim();
            Phone = phone.Trim();
            ClinicId = clinicId;
            ProfessionalId = professionalId?.Trim();
            Specialty = specialty?.Trim();
            Document = document?.Trim();
            DocumentType = documentType;
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
            MustChangePassword = false;
            UpdateTimestamp();
        }

        public void SetMustChangePassword(bool value)
        {
            MustChangePassword = value;
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

        /// <summary>
        /// Updates the owner's document. Useful for upgrading from CPF to CNPJ.
        /// </summary>
        public void UpdateDocument(string document, DocumentType documentType)
        {
            if (string.IsNullOrWhiteSpace(document))
                throw new ArgumentException("Document cannot be empty", nameof(document));

            ValidateDocument(document, documentType);

            Document = document.Trim();
            DocumentType = documentType;
            UpdateTimestamp();
        }

        private static void ValidateDocument(string document, DocumentType documentType)
        {
            var cleanDocument = new string(document.Where(char.IsDigit).ToArray());
            
            switch (documentType)
            {
                case Enums.DocumentType.CPF:
                    if (cleanDocument.Length != DocumentConstants.CpfLength)
                        throw new ArgumentException($"CPF must have {DocumentConstants.CpfLength} digits", nameof(document));
                    if (!DocumentValidator.IsValidCpf(document))
                        throw new ArgumentException("Invalid CPF format", nameof(document));
                    break;

                case Enums.DocumentType.CNPJ:
                    if (cleanDocument.Length != DocumentConstants.CnpjLength)
                        throw new ArgumentException($"CNPJ must have {DocumentConstants.CnpjLength} digits", nameof(document));
                    if (!DocumentValidator.IsValidCnpj(document))
                        throw new ArgumentException("Invalid CNPJ format", nameof(document));
                    break;

                default:
                    throw new ArgumentException("Invalid document type", nameof(documentType));
            }
        }

        /// <summary>
        /// Generates a secure email confirmation token valid for 24 hours.
        /// </summary>
        public string GenerateEmailConfirmationToken()
        {
            var tokenBytes = new byte[32];
            System.Security.Cryptography.RandomNumberGenerator.Fill(tokenBytes);
            var token = Convert.ToBase64String(tokenBytes)
                .Replace('+', '-').Replace('/', '_').Replace("=", "");
            EmailConfirmationToken = token;
            EmailConfirmationTokenExpiresAt = DateTime.UtcNow.AddHours(24);
            UpdateTimestamp();
            return token;
        }

        /// <summary>
        /// Confirms the owner's email using the provided token.
        /// </summary>
        public bool ConfirmEmail(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
                return false;

            if (IsEmailConfirmed)
                return true;

            if (EmailConfirmationToken != token)
                return false;

            if (EmailConfirmationTokenExpiresAt.HasValue && DateTime.UtcNow > EmailConfirmationTokenExpiresAt.Value)
                return false;

            IsEmailConfirmed = true;
            EmailConfirmationToken = null;
            EmailConfirmationTokenExpiresAt = null;
            UpdateTimestamp();
            return true;
        }
    }
}
