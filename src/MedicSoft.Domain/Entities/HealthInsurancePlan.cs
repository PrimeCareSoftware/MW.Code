using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public class HealthInsurancePlan : BaseEntity
    {
        public Guid OperatorId { get; private set; } // Operadora de saúde
        public string PlanName { get; private set; } // Nome do plano
        public string PlanCode { get; private set; } // Código do plano na operadora
        public string? RegisterNumber { get; private set; } // Registro ANS do plano
        public PlanType Type { get; private set; } // Individual, Empresarial, Coletivo
        public bool IsActive { get; private set; } = true;
        
        // Cobertura
        public bool CoversConsultations { get; private set; }
        public bool CoversExams { get; private set; }
        public bool CoversProcedures { get; private set; }
        public bool RequiresPriorAuthorization { get; private set; }
        
        // Navigation properties
        public HealthInsuranceOperator? Operator { get; private set; }
        
        // DEPRECATED: Manter para retrocompatibilidade, será removido em versão futura
        [Obsolete("Use PatientHealthInsurance entity instead")]
        public Guid PatientId { get; private set; }
        [Obsolete("Use OperatorId and PlanName instead")]
        public string? InsuranceName { get; private set; }
        [Obsolete("Use PlanCode instead")]
        public string? PlanNumber { get; private set; }
        [Obsolete("Use Type instead")]
        public string? OldPlanType { get; private set; }
        [Obsolete("Use PatientHealthInsurance entity instead")]
        public DateTime ValidFrom { get; private set; }
        [Obsolete("Use PatientHealthInsurance entity instead")]
        public DateTime? ValidUntil { get; private set; }
        [Obsolete("Use PatientHealthInsurance entity instead")]
        public string? HolderName { get; private set; }
        
        [Obsolete("Use PatientHealthInsurance entity instead")]
        public Patient? Patient { get; private set; }

        private HealthInsurancePlan() 
        { 
            // EF Constructor - nullable warnings suppressed as EF Core sets these via reflection
            PlanName = null!;
            PlanCode = null!;
        }

        // NOVO CONSTRUTOR - TISS Phase 1
        public HealthInsurancePlan(
            Guid operatorId,
            string planName,
            string planCode,
            PlanType type,
            string tenantId,
            string? registerNumber = null,
            bool coversConsultations = true,
            bool coversExams = true,
            bool coversProcedures = true,
            bool requiresPriorAuthorization = false) : base(tenantId)
        {
            if (operatorId == Guid.Empty)
                throw new ArgumentException("Operator ID cannot be empty", nameof(operatorId));
            
            if (string.IsNullOrWhiteSpace(planName))
                throw new ArgumentException("Plan name cannot be empty", nameof(planName));
            
            if (string.IsNullOrWhiteSpace(planCode))
                throw new ArgumentException("Plan code cannot be empty", nameof(planCode));

            OperatorId = operatorId;
            PlanName = planName.Trim();
            PlanCode = planCode.Trim();
            RegisterNumber = registerNumber?.Trim();
            Type = type;
            CoversConsultations = coversConsultations;
            CoversExams = coversExams;
            CoversProcedures = coversProcedures;
            RequiresPriorAuthorization = requiresPriorAuthorization;
        }

        // CONSTRUTOR LEGADO - Manter para retrocompatibilidade
        [Obsolete("Use new constructor with OperatorId and PlanType instead")]
        public HealthInsurancePlan(Guid patientId, string insuranceName, string planNumber, 
            DateTime validFrom, string tenantId, string? planType = null, 
            DateTime? validUntil = null, string? holderName = null) : base(tenantId)
        {
            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));
            
            if (string.IsNullOrWhiteSpace(insuranceName))
                throw new ArgumentException("Insurance name cannot be empty", nameof(insuranceName));
            
            if (string.IsNullOrWhiteSpace(planNumber))
                throw new ArgumentException("Plan number cannot be empty", nameof(planNumber));

            if (validFrom > DateTime.UtcNow.AddYears(1))
                throw new ArgumentException("Valid from date cannot be too far in the future", nameof(validFrom));

            if (validUntil.HasValue && validUntil.Value < validFrom)
                throw new ArgumentException("Valid until date must be after valid from date", nameof(validUntil));

            // Legacy fields
            PatientId = patientId;
            InsuranceName = insuranceName.Trim();
            PlanNumber = planNumber.Trim();
            OldPlanType = planType?.Trim();
            ValidFrom = validFrom;
            ValidUntil = validUntil;
            HolderName = holderName?.Trim();
            
            // New fields with defaults
            OperatorId = Guid.Empty; // Will need manual migration
            PlanName = insuranceName.Trim();
            PlanCode = planNumber.Trim();
            Type = PlanType.Individual;
            CoversConsultations = true;
            CoversExams = true;
            CoversProcedures = true;
            RequiresPriorAuthorization = false;
        }

        // MÉTODOS NOVOS - TISS Phase 1
        public void UpdatePlanInfo(string planName, string planCode, string? registerNumber, PlanType type)
        {
            if (string.IsNullOrWhiteSpace(planName))
                throw new ArgumentException("Plan name cannot be empty", nameof(planName));
            
            if (string.IsNullOrWhiteSpace(planCode))
                throw new ArgumentException("Plan code cannot be empty", nameof(planCode));

            PlanName = planName.Trim();
            PlanCode = planCode.Trim();
            RegisterNumber = registerNumber?.Trim();
            Type = type;
            UpdateTimestamp();
        }

        public void UpdateCoverage(bool coversConsultations, bool coversExams, bool coversProcedures, bool requiresPriorAuthorization)
        {
            CoversConsultations = coversConsultations;
            CoversExams = coversExams;
            CoversProcedures = coversProcedures;
            RequiresPriorAuthorization = requiresPriorAuthorization;
            UpdateTimestamp();
        }

        // MÉTODOS LEGADOS - Manter para retrocompatibilidade
        [Obsolete("Use new UpdatePlanInfo method instead")]
        public void UpdatePlanInfo(string insuranceName, string planNumber, string? planType, string? holderName)
        {
            if (string.IsNullOrWhiteSpace(insuranceName))
                throw new ArgumentException("Insurance name cannot be empty", nameof(insuranceName));
            
            if (string.IsNullOrWhiteSpace(planNumber))
                throw new ArgumentException("Plan number cannot be empty", nameof(planNumber));

            InsuranceName = insuranceName.Trim();
            PlanNumber = planNumber.Trim();
            OldPlanType = planType?.Trim();
            HolderName = holderName?.Trim();
            UpdateTimestamp();
        }

        [Obsolete("Use PatientHealthInsurance entity instead")]
        public void UpdateValidityPeriod(DateTime validFrom, DateTime? validUntil)
        {
            if (validFrom > DateTime.UtcNow.AddYears(1))
                throw new ArgumentException("Valid from date cannot be too far in the future", nameof(validFrom));

            if (validUntil.HasValue && validUntil.Value < validFrom)
                throw new ArgumentException("Valid until date must be after valid from date", nameof(validUntil));

            ValidFrom = validFrom;
            ValidUntil = validUntil;
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

        [Obsolete("Use PatientHealthInsurance entity instead")]
        public bool IsValid(DateTime? asOfDate = null)
        {
            var checkDate = asOfDate ?? DateTime.UtcNow;
            
            if (!IsActive)
                return false;

            if (checkDate < ValidFrom)
                return false;

            if (ValidUntil.HasValue && checkDate > ValidUntil.Value)
                return false;

            return true;
        }
    }

    /// <summary>
    /// Tipo de plano de saúde
    /// </summary>
    public enum PlanType
    {
        Individual = 1,   // Plano individual/familiar
        Enterprise = 2,   // Plano empresarial
        Collective = 3    // Plano coletivo por adesão
    }
}
