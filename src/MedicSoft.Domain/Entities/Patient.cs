using System;
using System.Collections.Generic;
using System.Linq;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.ValueObjects;
using MedicSoft.Domain.Services;

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

        // Navigation property for health insurance plans (0..N relationship)
        private readonly List<HealthInsurancePlan> _healthInsurancePlans = new();
        public IReadOnlyCollection<HealthInsurancePlan> HealthInsurancePlans => _healthInsurancePlans.AsReadOnly();

        // Navigation property for clinic links (N:N relationship)
        private readonly List<PatientClinicLink> _clinicLinks = new();
        public IReadOnlyCollection<PatientClinicLink> ClinicLinks => _clinicLinks.AsReadOnly();

        private Patient() 
        { 
            // EF Constructor - nullable warnings suppressed as EF Core sets these via reflection
            Name = null!;
            Document = null!;
            Gender = null!;
            Email = null!;
            Phone = null!;
            Address = null!;
        }

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

            // Validate CPF format if document appears to be a CPF (11 digits)
            var cleanDocument = new string(document.Where(char.IsDigit).ToArray());
            if (cleanDocument.Length == 11 && !DocumentValidator.IsValidCpf(document))
                throw new ArgumentException("Invalid CPF format", nameof(document));

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

        public void AddHealthInsurancePlan(HealthInsurancePlan plan)
        {
            if (plan == null)
                throw new ArgumentNullException(nameof(plan));

            if (plan.PatientId != Id)
                throw new ArgumentException("Health insurance plan does not belong to this patient", nameof(plan));

            _healthInsurancePlans.Add(plan);
            UpdateTimestamp();
        }

        public void RemoveHealthInsurancePlan(Guid planId)
        {
            var plan = _healthInsurancePlans.FirstOrDefault(p => p.Id == planId);
            if (plan != null)
            {
                _healthInsurancePlans.Remove(plan);
                UpdateTimestamp();
            }
        }

        public IEnumerable<HealthInsurancePlan> GetActiveHealthInsurancePlans()
        {
            return _healthInsurancePlans.Where(p => p.IsValid());
        }

        public void AddClinicLink(PatientClinicLink clinicLink)
        {
            if (clinicLink == null)
                throw new ArgumentNullException(nameof(clinicLink));

            if (clinicLink.PatientId != Id)
                throw new ArgumentException("Clinic link does not belong to this patient", nameof(clinicLink));

            _clinicLinks.Add(clinicLink);
            UpdateTimestamp();
        }

        public void RemoveClinicLink(Guid clinicId)
        {
            var link = _clinicLinks.FirstOrDefault(l => l.ClinicId == clinicId);
            if (link != null)
            {
                link.Deactivate();
                UpdateTimestamp();
            }
        }

        public IEnumerable<PatientClinicLink> GetActiveClinicLinks()
        {
            return _clinicLinks.Where(l => l.IsActive);
        }

        public bool IsLinkedToClinic(Guid clinicId)
        {
            return _clinicLinks.Any(l => l.ClinicId == clinicId && l.IsActive);
        }
    }
}