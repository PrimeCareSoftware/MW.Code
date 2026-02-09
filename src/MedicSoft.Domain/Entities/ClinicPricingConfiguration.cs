using System;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents pricing configuration for a clinic.
    /// Controls consultation costs, procedure pricing rules, and charging policies.
    /// </summary>
    public class ClinicPricingConfiguration : BaseEntity
    {
        public Guid ClinicId { get; private set; }
        
        // Consultation pricing
        public decimal DefaultConsultationPrice { get; private set; }
        public decimal? FollowUpConsultationPrice { get; private set; } // Retorno
        public decimal? TelemedicineConsultationPrice { get; private set; }
        
        // Procedure + Consultation policies
        public ProcedureConsultationPolicy DefaultProcedurePolicy { get; private set; }
        
        // When policy is DiscountOnConsultation
        public decimal? ConsultationDiscountPercentage { get; private set; }
        public decimal? ConsultationDiscountFixedAmount { get; private set; }
        
        // Navigation properties
        public Clinic? Clinic { get; private set; }
        
        private ClinicPricingConfiguration()
        {
            // EF Constructor
        }
        
        public ClinicPricingConfiguration(
            Guid clinicId,
            decimal defaultConsultationPrice,
            string tenantId,
            ProcedureConsultationPolicy defaultProcedurePolicy = ProcedureConsultationPolicy.ChargeConsultation,
            decimal? followUpConsultationPrice = null,
            decimal? telemedicineConsultationPrice = null) : base(tenantId)
        {
            if (clinicId == Guid.Empty)
                throw new ArgumentException("Clinic ID cannot be empty", nameof(clinicId));
            
            if (defaultConsultationPrice < 0)
                throw new ArgumentException("Default consultation price cannot be negative", nameof(defaultConsultationPrice));
            
            if (followUpConsultationPrice.HasValue && followUpConsultationPrice.Value < 0)
                throw new ArgumentException("Follow-up consultation price cannot be negative", nameof(followUpConsultationPrice));
            
            if (telemedicineConsultationPrice.HasValue && telemedicineConsultationPrice.Value < 0)
                throw new ArgumentException("Telemedicine consultation price cannot be negative", nameof(telemedicineConsultationPrice));
            
            ClinicId = clinicId;
            DefaultConsultationPrice = defaultConsultationPrice;
            FollowUpConsultationPrice = followUpConsultationPrice;
            TelemedicineConsultationPrice = telemedicineConsultationPrice;
            DefaultProcedurePolicy = defaultProcedurePolicy;
        }
        
        public void UpdateConsultationPrices(
            decimal defaultConsultationPrice,
            decimal? followUpConsultationPrice = null,
            decimal? telemedicineConsultationPrice = null)
        {
            if (defaultConsultationPrice < 0)
                throw new ArgumentException("Default consultation price cannot be negative", nameof(defaultConsultationPrice));
            
            if (followUpConsultationPrice.HasValue && followUpConsultationPrice.Value < 0)
                throw new ArgumentException("Follow-up consultation price cannot be negative", nameof(followUpConsultationPrice));
            
            if (telemedicineConsultationPrice.HasValue && telemedicineConsultationPrice.Value < 0)
                throw new ArgumentException("Telemedicine consultation price cannot be negative", nameof(telemedicineConsultationPrice));
            
            DefaultConsultationPrice = defaultConsultationPrice;
            FollowUpConsultationPrice = followUpConsultationPrice;
            TelemedicineConsultationPrice = telemedicineConsultationPrice;
            UpdateTimestamp();
        }
        
        public void UpdateProcedurePolicy(
            ProcedureConsultationPolicy policy,
            decimal? discountPercentage = null,
            decimal? discountFixedAmount = null)
        {
            if (policy == ProcedureConsultationPolicy.DiscountOnConsultation)
            {
                if (!discountPercentage.HasValue && !discountFixedAmount.HasValue)
                    throw new ArgumentException("Discount percentage or fixed amount is required when policy is DiscountOnConsultation");
                
                if (discountPercentage.HasValue && (discountPercentage.Value < 0 || discountPercentage.Value > 100))
                    throw new ArgumentException("Discount percentage must be between 0 and 100", nameof(discountPercentage));
                
                if (discountFixedAmount.HasValue && discountFixedAmount.Value < 0)
                    throw new ArgumentException("Discount fixed amount cannot be negative", nameof(discountFixedAmount));
            }
            
            DefaultProcedurePolicy = policy;
            ConsultationDiscountPercentage = discountPercentage;
            ConsultationDiscountFixedAmount = discountFixedAmount;
            UpdateTimestamp();
        }
        
        /// <summary>
        /// Calculates the consultation price based on appointment type and mode
        /// </summary>
        public decimal GetConsultationPrice(AppointmentType appointmentType, AppointmentMode mode)
        {
            if (mode == AppointmentMode.Telemedicine && TelemedicineConsultationPrice.HasValue)
                return TelemedicineConsultationPrice.Value;
            
            if (appointmentType == AppointmentType.FollowUp && FollowUpConsultationPrice.HasValue)
                return FollowUpConsultationPrice.Value;
            
            return DefaultConsultationPrice;
        }
        
        /// <summary>
        /// Calculates the final consultation price when a procedure is performed
        /// </summary>
        public decimal GetConsultationPriceWithProcedure(decimal baseConsultationPrice)
        {
            return DefaultProcedurePolicy switch
            {
                ProcedureConsultationPolicy.NoCharge => 0,
                ProcedureConsultationPolicy.ChargeConsultation => baseConsultationPrice,
                ProcedureConsultationPolicy.DiscountOnConsultation => CalculateDiscountedPrice(baseConsultationPrice),
                _ => baseConsultationPrice
            };
        }
        
        private decimal CalculateDiscountedPrice(decimal basePrice)
        {
            if (ConsultationDiscountFixedAmount.HasValue)
            {
                var discounted = basePrice - ConsultationDiscountFixedAmount.Value;
                return Math.Max(0, discounted); // Can't be negative
            }
            
            if (ConsultationDiscountPercentage.HasValue)
            {
                var discount = basePrice * (ConsultationDiscountPercentage.Value / 100);
                var discounted = basePrice - discount;
                return Math.Max(0, discounted); // Can't be negative
            }
            
            return basePrice;
        }
    }
    
    /// <summary>
    /// Defines how consultation is charged when a procedure is performed
    /// </summary>
    public enum ProcedureConsultationPolicy
    {
        /// <summary>
        /// Charge full consultation price + procedure price
        /// </summary>
        ChargeConsultation = 1,
        
        /// <summary>
        /// Apply discount on consultation price when procedure is performed
        /// </summary>
        DiscountOnConsultation = 2,
        
        /// <summary>
        /// No consultation charge when procedure is performed (procedure only)
        /// </summary>
        NoCharge = 3
    }
}
