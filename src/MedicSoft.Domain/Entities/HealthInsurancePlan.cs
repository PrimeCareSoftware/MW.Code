using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public class HealthInsurancePlan : BaseEntity
    {
        public Guid PatientId { get; private set; }
        public string InsuranceName { get; private set; }
        public string PlanNumber { get; private set; }
        public string? PlanType { get; private set; }
        public DateTime ValidFrom { get; private set; }
        public DateTime? ValidUntil { get; private set; }
        public string? HolderName { get; private set; }
        public bool IsActive { get; private set; } = true;

        // Navigation property
        public Patient Patient { get; private set; } = null!;

        private HealthInsurancePlan() 
        { 
            // EF Constructor - nullable warnings suppressed as EF Core sets these via reflection
            InsuranceName = null!;
            PlanNumber = null!;
        }

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

            PatientId = patientId;
            InsuranceName = insuranceName.Trim();
            PlanNumber = planNumber.Trim();
            PlanType = planType?.Trim();
            ValidFrom = validFrom;
            ValidUntil = validUntil;
            HolderName = holderName?.Trim();
        }

        public void UpdatePlanInfo(string insuranceName, string planNumber, string? planType, string? holderName)
        {
            if (string.IsNullOrWhiteSpace(insuranceName))
                throw new ArgumentException("Insurance name cannot be empty", nameof(insuranceName));
            
            if (string.IsNullOrWhiteSpace(planNumber))
                throw new ArgumentException("Plan number cannot be empty", nameof(planNumber));

            InsuranceName = insuranceName.Trim();
            PlanNumber = planNumber.Trim();
            PlanType = planType?.Trim();
            HolderName = holderName?.Trim();
            UpdateTimestamp();
        }

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
}
