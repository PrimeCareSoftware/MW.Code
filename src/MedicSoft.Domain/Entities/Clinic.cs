using System;
using System.Linq;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Services;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Entities
{
    public class Clinic : BaseEntity
    {
        public string Name { get; private set; }
        public string TradeName { get; private set; }
        public string Document { get; private set; } // CNPJ
        public string Phone { get; private set; }
        public string Email { get; private set; }
        public string Address { get; private set; }
        public string? Subdomain { get; private set; } // Unique subdomain for clinic access
        public TimeSpan OpeningTime { get; private set; }
        public TimeSpan ClosingTime { get; private set; }
        public int AppointmentDurationMinutes { get; private set; } = 30;
        public bool AllowEmergencySlots { get; private set; } = true;
        public bool IsActive { get; private set; } = true;
        public bool ShowOnPublicSite { get; private set; } = false; // Owner must approve public display
        public ClinicType ClinicType { get; private set; } = ClinicType.Medical;
        public string? WhatsAppNumber { get; private set; } // Optional WhatsApp for public contact

        private Clinic() 
        { 
            // EF Constructor - nullable warnings suppressed as EF Core sets these via reflection
            Name = null!;
            TradeName = null!;
            Document = null!;
            Phone = null!;
            Email = null!;
            Address = null!;
        }

        public Clinic(string name, string tradeName, string document, string phone,
            string email, string address, TimeSpan openingTime, TimeSpan closingTime,
            string tenantId, int appointmentDurationMinutes = 30) : base(tenantId)
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
            
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address cannot be empty", nameof(address));

            if (appointmentDurationMinutes <= 0)
                throw new ArgumentException("Appointment duration must be positive", nameof(appointmentDurationMinutes));

            if (openingTime >= closingTime)
                throw new ArgumentException("Opening time must be before closing time");

            // Validate CNPJ format if document appears to be a CNPJ
            var cleanDocument = new string(document.Where(char.IsDigit).ToArray());
            if (cleanDocument.Length == DocumentConstants.CnpjLength && !DocumentValidator.IsValidCnpj(document))
                throw new ArgumentException("Invalid CNPJ format", nameof(document));

            Name = name.Trim();
            TradeName = tradeName.Trim();
            Document = document.Trim();
            Phone = phone.Trim();
            Email = email.Trim();
            Address = address.Trim();
            OpeningTime = openingTime;
            ClosingTime = closingTime;
            AppointmentDurationMinutes = appointmentDurationMinutes;
        }

        public void UpdateInfo(string name, string tradeName, string phone, string email, string address)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));
            
            if (string.IsNullOrWhiteSpace(tradeName))
                throw new ArgumentException("Trade name cannot be empty", nameof(tradeName));
            
            if (string.IsNullOrWhiteSpace(phone))
                throw new ArgumentException("Phone cannot be empty", nameof(phone));
            
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty", nameof(email));
            
            if (string.IsNullOrWhiteSpace(address))
                throw new ArgumentException("Address cannot be empty", nameof(address));

            Name = name.Trim();
            TradeName = tradeName.Trim();
            Phone = phone.Trim();
            Email = email.Trim();
            Address = address.Trim();
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

        public void UpdateScheduleSettings(TimeSpan openingTime, TimeSpan closingTime, 
            int appointmentDurationMinutes, bool allowEmergencySlots)
        {
            if (openingTime >= closingTime)
                throw new ArgumentException("Opening time must be before closing time");

            if (appointmentDurationMinutes <= 0)
                throw new ArgumentException("Appointment duration must be positive", nameof(appointmentDurationMinutes));

            OpeningTime = openingTime;
            ClosingTime = closingTime;
            AppointmentDurationMinutes = appointmentDurationMinutes;
            AllowEmergencySlots = allowEmergencySlots;
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

        public bool IsWithinWorkingHours(TimeSpan time)
        {
            return time >= OpeningTime && time < ClosingTime;
        }

        public void UpdatePublicSiteSettings(bool showOnPublicSite, ClinicType clinicType, string? whatsAppNumber = null)
        {
            ShowOnPublicSite = showOnPublicSite;
            ClinicType = clinicType;
            
            if (!string.IsNullOrWhiteSpace(whatsAppNumber))
            {
                // Validate WhatsApp number format (basic validation)
                var cleanNumber = new string(whatsAppNumber.Where(char.IsDigit).ToArray());
                if (cleanNumber.Length < 10 || cleanNumber.Length > 15)
                    throw new ArgumentException("WhatsApp number must be between 10 and 15 digits", nameof(whatsAppNumber));
                
                WhatsAppNumber = whatsAppNumber.Trim();
            }
            else
            {
                WhatsAppNumber = null;
            }
            
            UpdateTimestamp();
        }

        public void EnablePublicDisplay()
        {
            ShowOnPublicSite = true;
            UpdateTimestamp();
        }

        public void DisablePublicDisplay()
        {
            ShowOnPublicSite = false;
            UpdateTimestamp();
        }
    }
}