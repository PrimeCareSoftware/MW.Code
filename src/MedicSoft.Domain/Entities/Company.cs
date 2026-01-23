using System;
using System.Linq;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Services;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a company/enterprise that owns one or more clinics.
    /// The Company is the tenant entity - all clinics under a company share the same tenant.
    /// </summary>
    public class Company : BaseEntity
    {
        public string Name { get; private set; }
        public string TradeName { get; private set; }
        public string Document { get; private set; } // CPF or CNPJ
        public DocumentType DocumentType { get; private set; } // CPF or CNPJ
        public string Phone { get; private set; }
        public string Email { get; private set; }
        public bool IsActive { get; private set; } = true;
        public string? Subdomain { get; private set; } // Unique subdomain for company access

        private Company()
        {
            // EF Constructor - nullable warnings suppressed as EF Core sets these via reflection
            Name = null!;
            TradeName = null!;
            Document = null!;
            Phone = null!;
            Email = null!;
        }

        public Company(string name, string tradeName, string document, DocumentType documentType,
            string phone, string email, string tenantId) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(tradeName))
                throw new ArgumentException("Trade name cannot be empty", nameof(tradeName));

            if (string.IsNullOrWhiteSpace(document))
                throw new ArgumentException("Document cannot be empty", nameof(document));

            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone cannot be empty", nameof(phone));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            // Validate document format
            var cleanDocument = new string(document.Where(char.IsDigit).ToArray());

            // Validate based on document type
            if (documentType == DocumentType.CNPJ)
            {
                if (cleanDocument.Length != DocumentConstants.CnpjLength)
                    throw new ArgumentException($"CNPJ must have {DocumentConstants.CnpjLength} digits", nameof(document));
                if (!DocumentValidator.IsValidCnpj(document))
                    throw new ArgumentException("Invalid CNPJ format", nameof(document));
            }
            else if (documentType == DocumentType.CPF)
            {
                if (cleanDocument.Length != DocumentConstants.CpfLength)
                    throw new ArgumentException($"CPF must have {DocumentConstants.CpfLength} digits", nameof(document));
                if (!DocumentValidator.IsValidCpf(document))
                    throw new ArgumentException("Invalid CPF format", nameof(document));
            }

            Name = name.Trim();
            TradeName = tradeName.Trim();
            Document = document.Trim();
            DocumentType = documentType;
            Phone = phone.Trim();
            Email = email.Trim();
        }

        public void UpdateInfo(string name, string tradeName, string phone, string email)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(tradeName))
                throw new ArgumentException("Trade name cannot be empty", nameof(tradeName));

            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone cannot be empty", nameof(phone));

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));

            Name = name.Trim();
            TradeName = tradeName.Trim();
            Phone = phone.Trim();
            Email = email.Trim();
            UpdateTimestamp();
        }

        public void SetSubdomain(string? subdomain)
        {
            if (!string.IsNullOrWhiteSpace(subdomain))
            {
                // Validate subdomain format (lowercase alphanumeric and hyphens)
                var validSubdomain = subdomain.Trim().ToLowerInvariant();
                if (!System.Text.RegularExpressions.Regex.IsMatch(validSubdomain, "^[a-z0-9]([a-z0-9-]*[a-z0-9])?$"))
                    throw new ArgumentException("Subdomain must contain only lowercase letters, numbers, and hyphens", nameof(subdomain));

                if (validSubdomain.Length < 3 || validSubdomain.Length > 63)
                    throw new ArgumentException("Subdomain must be between 3 and 63 characters", nameof(subdomain));

                Subdomain = validSubdomain;
            }
            else
            {
                Subdomain = null;
            }
            UpdateTimestamp();
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdateTimestamp();
        }

        public void Activate()
        {
            IsActive = true;
            UpdateTimestamp();
        }

        /// <summary>
        /// Updates the company's document. Useful for upgrading from CPF to CNPJ.
        /// </summary>
        public void UpdateDocument(string document, DocumentType documentType)
        {
            if (string.IsNullOrWhiteSpace(document))
                throw new ArgumentException("Document cannot be empty", nameof(document));

            var cleanDocument = new string(document.Where(char.IsDigit).ToArray());

            if (documentType == DocumentType.CNPJ)
            {
                if (cleanDocument.Length != DocumentConstants.CnpjLength)
                    throw new ArgumentException($"CNPJ must have {DocumentConstants.CnpjLength} digits", nameof(document));
                if (!DocumentValidator.IsValidCnpj(document))
                    throw new ArgumentException("Invalid CNPJ format", nameof(document));
            }
            else if (documentType == DocumentType.CPF)
            {
                if (cleanDocument.Length != DocumentConstants.CpfLength)
                    throw new ArgumentException($"CPF must have {DocumentConstants.CpfLength} digits", nameof(document));
                if (!DocumentValidator.IsValidCpf(document))
                    throw new ArgumentException("Invalid CPF format", nameof(document));
            }

            Document = document.Trim();
            DocumentType = documentType;
            UpdateTimestamp();
        }
    }
}
