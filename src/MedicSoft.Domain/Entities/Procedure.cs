using System;
using System.Collections.Generic;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a medical procedure or service offered by the clinic.
    /// Can be linked to appointments and patients.
    /// </summary>
    public class Procedure : BaseEntity
    {
        public string Name { get; private set; }
        public string Code { get; private set; }
        public string Description { get; private set; }
        public ProcedureCategory Category { get; private set; }
        public decimal Price { get; private set; }
        public int DurationMinutes { get; private set; }
        public bool RequiresMaterials { get; private set; }
        public bool IsActive { get; private set; }
        
        // New fields for enhanced procedure management
        public Guid? ClinicId { get; private set; }
        public string? AcceptedHealthInsurances { get; private set; }
        public bool AllowInMedicalAttendance { get; private set; }
        public bool AllowInExclusiveProcedureAttendance { get; private set; }

        // Navigation property for materials
        private readonly List<ProcedureMaterial> _materials = new();
        public IReadOnlyCollection<ProcedureMaterial> Materials => _materials.AsReadOnly();

        private Procedure()
        {
            // EF Constructor
            Name = null!;
            Code = null!;
            Description = null!;
        }

        public Procedure(string name, string code, string description,
            ProcedureCategory category, decimal price, int durationMinutes,
            string tenantId, bool requiresMaterials = false, Guid? clinicId = null,
            string? acceptedHealthInsurances = null, bool allowInMedicalAttendance = true,
            bool allowInExclusiveProcedureAttendance = false) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code cannot be empty", nameof(code));

            if (price < 0)
                throw new ArgumentException("Price cannot be negative", nameof(price));

            if (durationMinutes <= 0)
                throw new ArgumentException("Duration must be greater than zero", nameof(durationMinutes));

            Name = name.Trim();
            Code = code.Trim();
            Description = description?.Trim() ?? string.Empty;
            Category = category;
            Price = price;
            DurationMinutes = durationMinutes;
            RequiresMaterials = requiresMaterials;
            IsActive = true;
            ClinicId = clinicId;
            AcceptedHealthInsurances = string.IsNullOrWhiteSpace(acceptedHealthInsurances) ? null : acceptedHealthInsurances.Trim();
            AllowInMedicalAttendance = allowInMedicalAttendance;
            AllowInExclusiveProcedureAttendance = allowInExclusiveProcedureAttendance;
        }

        public void Update(string name, string description, ProcedureCategory category,
            decimal price, int durationMinutes, bool requiresMaterials, Guid? clinicId = null,
            string? acceptedHealthInsurances = null, bool allowInMedicalAttendance = true,
            bool allowInExclusiveProcedureAttendance = false)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            if (price < 0)
                throw new ArgumentException("Price cannot be negative", nameof(price));

            if (durationMinutes <= 0)
                throw new ArgumentException("Duration must be greater than zero", nameof(durationMinutes));

            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            Category = category;
            Price = price;
            DurationMinutes = durationMinutes;
            RequiresMaterials = requiresMaterials;
            ClinicId = clinicId;
            AcceptedHealthInsurances = string.IsNullOrWhiteSpace(acceptedHealthInsurances) ? null : acceptedHealthInsurances.Trim();
            AllowInMedicalAttendance = allowInMedicalAttendance;
            AllowInExclusiveProcedureAttendance = allowInExclusiveProcedureAttendance;
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
    }

    public enum ProcedureCategory
    {
        Consultation,       // Consulta
        Exam,              // Exame
        Surgery,           // Cirurgia
        Therapy,           // Terapia
        Vaccination,       // Vacinação
        Diagnostic,        // Diagnóstico
        Treatment,         // Tratamento
        Emergency,         // Emergência
        Prevention,        // Prevenção
        Aesthetic,         // Estética
        FollowUp,          // Retorno
        Other              // Outros
    }
}
