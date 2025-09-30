using System;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Domain.Entities
{
    public class Patient : BaseEntity
    {
        public string Name { get; private set; }
        public string Document { get; private set; } // CPF/RG/Passport
        public DateTime DateOfBirth { get; private set; }
        public string Gender { get; private set; }
        public Email Email { get; private set; }
        public Phone Phone { get; private set; }
        public Address Address { get; private set; }
        public string? MedicalHistory { get; private set; }
        public string? Allergies { get; private set; }
        public bool IsActive { get; private set; } = true;

        private Patient() { } // EF Constructor

        public Patient(string name, string document, DateTime dateOfBirth, string gender,
            Email email, Phone phone, Address address, string tenantId,
            string? medicalHistory = null, string? allergies = null) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));
            
            if (string.IsNullOrWhiteSpace(document))
                throw new ArgumentException("Document cannot be empty", nameof(document));
            
            if (string.IsNullOrWhiteSpace(gender))
                throw new ArgumentException("Gender cannot be empty", nameof(gender));

            if (dateOfBirth >= DateTime.Now)
                throw new ArgumentException("Date of birth must be in the past", nameof(dateOfBirth));

            Name = name.Trim();
            Document = document.Trim();
            DateOfBirth = dateOfBirth;
            Gender = gender.Trim();
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            MedicalHistory = medicalHistory?.Trim();
            Allergies = allergies?.Trim();
        }

        public void UpdatePersonalInfo(string name, Email email, Phone phone, Address address)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            Name = name.Trim();
            Email = email ?? throw new ArgumentNullException(nameof(email));
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Address = address ?? throw new ArgumentNullException(nameof(address));
            UpdateTimestamp();
        }

        public void UpdateMedicalInfo(string? medicalHistory, string? allergies)
        {
            MedicalHistory = medicalHistory?.Trim();
            Allergies = allergies?.Trim();
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

        public int GetAge()
        {
            var today = DateTime.Today;
            var age = today.Year - DateOfBirth.Year;
            if (DateOfBirth.Date > today.AddYears(-age)) age--;
            return age;
        }
    }
}