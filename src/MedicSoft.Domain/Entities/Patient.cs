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
        public string Document { get; private set; } // CPF/RG/Passaporte
        public DateTime DateOfBirth { get; private set; }
        public string Gender { get; private set; }
        public Email Email { get; private set; }
        public Phone Phone { get; private set; }
        public Address Address { get; private set; }
        public string? MedicalHistory { get; private set; }
        public string? Allergies { get; private set; }
        public bool IsActive { get; private set; } = true;

        // Propriedades de relacionamento Responsável-Criança
        public Guid? GuardianId { get; private set; }
        public Patient? Guardian { get; private set; }
        private readonly List<Patient> _children = new();
        public IReadOnlyCollection<Patient> Children => _children.AsReadOnly();

        // Propriedade de navegação para planos de saúde (relacionamento 0..N)
        private readonly List<HealthInsurancePlan> _healthInsurancePlans = new();
        public IReadOnlyCollection<HealthInsurancePlan> HealthInsurancePlans => _healthInsurancePlans.AsReadOnly();

        // Propriedade de navegação para vínculos de clínica (relacionamento N:N)
        private readonly List<PatientClinicLink> _clinicLinks = new();
        public IReadOnlyCollection<PatientClinicLink> ClinicLinks => _clinicLinks.AsReadOnly();

        private Patient() 
        { 
            // Construtor do EF - avisos de nulabilidade suprimidos pois o EF Core define via reflection
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
                throw new ArgumentException("O nome não pode estar vazio", nameof(name));
            
            if (string.IsNullOrWhiteSpace(document))
                throw new ArgumentException("O documento não pode estar vazio", nameof(document));
            
            if (string.IsNullOrWhiteSpace(gender))
                throw new ArgumentException("O gênero não pode estar vazio", nameof(gender));

            if (dateOfBirth >= DateTime.Now)
                throw new ArgumentException("A data de nascimento deve estar no passado", nameof(dateOfBirth));

            // Valida o formato do CPF se o documento parecer ser um CPF
            var cleanDocument = new string(document.Where(char.IsDigit).ToArray());
            if (cleanDocument.Length == DocumentConstants.CpfLength && !DocumentValidator.IsValidCpf(document))
                throw new ArgumentException("Formato de CPF inválido", nameof(document));

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
                throw new ArgumentException("O nome não pode estar vazio", nameof(name));

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
                throw new ArgumentException("O plano de saúde não pertence a este paciente", nameof(plan));

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
                throw new ArgumentException("O vínculo da clínica não pertence a este paciente", nameof(clinicLink));

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

        public bool IsChild()
        {
            return GetAge() < 18;
        }

        public void SetGuardian(Guid guardianId)
        {
            if (guardianId == Guid.Empty)
                throw new ArgumentException("O ID do responsável não pode estar vazio", nameof(guardianId));

            if (guardianId == Id)
                throw new ArgumentException("O paciente não pode ser seu próprio responsável", nameof(guardianId));

            if (!IsChild())
                throw new InvalidOperationException("Apenas crianças (menores de 18 anos) podem ter um responsável");

            GuardianId = guardianId;
            UpdateTimestamp();
        }

        public void RemoveGuardian()
        {
            GuardianId = null;
            UpdateTimestamp();
        }

        public void AddChild(Patient child)
        {
            if (child == null)
                throw new ArgumentNullException(nameof(child));

            if (child.Id == Id)
                throw new ArgumentException("O paciente não pode ser seu próprio filho", nameof(child));

            if (!child.IsChild())
                throw new ArgumentException("Apenas crianças (menores de 18 anos) podem ser adicionadas como dependentes", nameof(child));

            if (child.GuardianId.HasValue && child.GuardianId != Id)
                throw new InvalidOperationException("A criança já possui um responsável diferente");

            _children.Add(child);
            UpdateTimestamp();
        }

        public void RemoveChild(Guid childId)
        {
            var child = _children.FirstOrDefault(c => c.Id == childId);
            if (child != null)
            {
                _children.Remove(child);
                UpdateTimestamp();
            }
        }

        public IEnumerable<Patient> GetChildren()
        {
            return _children.Where(c => c.IsActive);
        }
    }
}