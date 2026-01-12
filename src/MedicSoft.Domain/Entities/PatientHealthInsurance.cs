using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Representa o vínculo entre um paciente e um plano de saúde (carteirinha)
    /// </summary>
    public class PatientHealthInsurance : BaseEntity
    {
        public Guid PatientId { get; private set; }
        public Guid HealthInsurancePlanId { get; private set; }
        public string CardNumber { get; private set; } // Número da carteirinha
        public string? CardValidationCode { get; private set; } // CVV/código de validação da carteirinha
        public DateTime ValidFrom { get; private set; }
        public DateTime? ValidUntil { get; private set; }
        public bool IsActive { get; private set; } = true;
        
        // Informações do titular (se paciente for dependente)
        public bool IsHolder { get; private set; } // Paciente é o titular?
        public string? HolderName { get; private set; }
        public string? HolderDocument { get; private set; } // CPF do titular
        
        // Navigation properties
        public Patient? Patient { get; private set; }
        public HealthInsurancePlan? HealthInsurancePlan { get; private set; }
        
        private PatientHealthInsurance() 
        { 
            // EF Constructor
            CardNumber = null!;
        }

        public PatientHealthInsurance(
            Guid patientId,
            Guid healthInsurancePlanId,
            string cardNumber,
            DateTime validFrom,
            string tenantId,
            bool isHolder = true,
            string? cardValidationCode = null,
            DateTime? validUntil = null,
            string? holderName = null,
            string? holderDocument = null) : base(tenantId)
        {
            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));
            
            if (healthInsurancePlanId == Guid.Empty)
                throw new ArgumentException("Health Insurance Plan ID cannot be empty", nameof(healthInsurancePlanId));
            
            if (string.IsNullOrWhiteSpace(cardNumber))
                throw new ArgumentException("Card number cannot be empty", nameof(cardNumber));

            if (validFrom > DateTime.UtcNow.AddYears(1))
                throw new ArgumentException("Valid from date cannot be too far in the future", nameof(validFrom));

            if (validUntil.HasValue && validUntil.Value < validFrom)
                throw new ArgumentException("Valid until date must be after valid from date", nameof(validUntil));

            // Se não for titular, deve ter nome e documento do titular
            if (!isHolder)
            {
                if (string.IsNullOrWhiteSpace(holderName))
                    throw new ArgumentException("Holder name is required when patient is not the holder", nameof(holderName));
                
                if (string.IsNullOrWhiteSpace(holderDocument))
                    throw new ArgumentException("Holder document is required when patient is not the holder", nameof(holderDocument));
            }

            PatientId = patientId;
            HealthInsurancePlanId = healthInsurancePlanId;
            CardNumber = cardNumber.Trim();
            CardValidationCode = cardValidationCode?.Trim();
            ValidFrom = validFrom;
            ValidUntil = validUntil;
            IsHolder = isHolder;
            HolderName = holderName?.Trim();
            HolderDocument = holderDocument?.Trim();
        }

        public void UpdateCardInfo(string cardNumber, string? cardValidationCode)
        {
            if (string.IsNullOrWhiteSpace(cardNumber))
                throw new ArgumentException("Card number cannot be empty", nameof(cardNumber));

            CardNumber = cardNumber.Trim();
            CardValidationCode = cardValidationCode?.Trim();
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

        public void UpdateHolderInfo(bool isHolder, string? holderName = null, string? holderDocument = null)
        {
            // Se não for titular, deve ter nome e documento do titular
            if (!isHolder)
            {
                if (string.IsNullOrWhiteSpace(holderName))
                    throw new ArgumentException("Holder name is required when patient is not the holder", nameof(holderName));
                
                if (string.IsNullOrWhiteSpace(holderDocument))
                    throw new ArgumentException("Holder document is required when patient is not the holder", nameof(holderDocument));
            }

            IsHolder = isHolder;
            HolderName = holderName?.Trim();
            HolderDocument = holderDocument?.Trim();
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
