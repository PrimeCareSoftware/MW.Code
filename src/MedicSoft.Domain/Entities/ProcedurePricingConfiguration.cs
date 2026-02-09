using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents procedure-specific pricing configuration.
    /// Allows overriding the clinic's default consultation policy for specific procedures.
    /// </summary>
    public class ProcedurePricingConfiguration : BaseEntity
    {
        public Guid ProcedureId { get; private set; }
        public Guid ClinicId { get; private set; }
        
        // Override clinic default policy for this specific procedure
        public ProcedureConsultationPolicy? ConsultationPolicy { get; private set; }
        
        // Procedure-specific discount configuration
        public decimal? ConsultationDiscountPercentage { get; private set; }
        public decimal? ConsultationDiscountFixedAmount { get; private set; }
        
        // Custom price for this procedure at this clinic (overrides Procedure.Price)
        public decimal? CustomPrice { get; private set; }
        
        // Navigation properties
        public Procedure? Procedure { get; private set; }
        public Clinic? Clinic { get; private set; }
        
        private ProcedurePricingConfiguration()
        {
            // EF Constructor
        }
        
        public ProcedurePricingConfiguration(
            Guid procedureId,
            Guid clinicId,
            string tenantId,
            ProcedureConsultationPolicy? consultationPolicy = null,
            decimal? customPrice = null) : base(tenantId)
        {
            if (procedureId == Guid.Empty)
                throw new ArgumentException("Procedure ID cannot be empty", nameof(procedureId));
            
            if (clinicId == Guid.Empty)
                throw new ArgumentException("Clinic ID cannot be empty", nameof(clinicId));
            
            if (customPrice.HasValue && customPrice.Value < 0)
                throw new ArgumentException("Custom price cannot be negative", nameof(customPrice));
            
            ProcedureId = procedureId;
            ClinicId = clinicId;
            ConsultationPolicy = consultationPolicy;
            CustomPrice = customPrice;
        }
        
        public void UpdateConsultationPolicy(
            ProcedureConsultationPolicy? policy,
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
            
            ConsultationPolicy = policy;
            ConsultationDiscountPercentage = discountPercentage;
            ConsultationDiscountFixedAmount = discountFixedAmount;
            UpdateTimestamp();
        }
        
        public void UpdateCustomPrice(decimal? customPrice)
        {
            if (customPrice.HasValue && customPrice.Value < 0)
                throw new ArgumentException("Custom price cannot be negative", nameof(customPrice));
            
            CustomPrice = customPrice;
            UpdateTimestamp();
        }
        
        /// <summary>
        /// Gets the effective procedure price (custom price if set, otherwise default)
        /// </summary>
        public decimal GetEffectivePrice(decimal defaultPrice)
        {
            return CustomPrice ?? defaultPrice;
        }
    }
}
