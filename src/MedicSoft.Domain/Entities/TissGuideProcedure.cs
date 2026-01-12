using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Representa um procedimento dentro de uma guia TISS
    /// </summary>
    public class TissGuideProcedure : BaseEntity
    {
        public Guid TissGuideId { get; private set; }
        public string ProcedureCode { get; private set; } // Código TUSS
        public string ProcedureDescription { get; private set; }
        public int Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal TotalPrice { get; private set; }
        
        // Retorno da operadora (após processamento)
        public int? ApprovedQuantity { get; private set; }
        public decimal? ApprovedAmount { get; private set; }
        public decimal? GlosedAmount { get; private set; }
        public string? GlossReason { get; private set; }
        
        // Navigation property
        public TissGuide? TissGuide { get; private set; }
        
        private TissGuideProcedure() 
        { 
            // EF Constructor
            ProcedureCode = null!;
            ProcedureDescription = null!;
        }

        public TissGuideProcedure(
            Guid tissGuideId,
            string procedureCode,
            string procedureDescription,
            int quantity,
            decimal unitPrice,
            string tenantId) : base(tenantId)
        {
            if (tissGuideId == Guid.Empty)
                throw new ArgumentException("TISS Guide ID cannot be empty", nameof(tissGuideId));
            
            if (string.IsNullOrWhiteSpace(procedureCode))
                throw new ArgumentException("Procedure code cannot be empty", nameof(procedureCode));
            
            if (string.IsNullOrWhiteSpace(procedureDescription))
                throw new ArgumentException("Procedure description cannot be empty", nameof(procedureDescription));
            
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
            
            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

            TissGuideId = tissGuideId;
            ProcedureCode = procedureCode.Trim();
            ProcedureDescription = procedureDescription.Trim();
            Quantity = quantity;
            UnitPrice = unitPrice;
            TotalPrice = quantity * unitPrice;
        }

        public void UpdateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            Quantity = quantity;
            TotalPrice = quantity * UnitPrice;
            UpdateTimestamp();
        }

        public void UpdateUnitPrice(decimal unitPrice)
        {
            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

            UnitPrice = unitPrice;
            TotalPrice = Quantity * unitPrice;
            UpdateTimestamp();
        }

        public void ProcessOperatorResponse(int? approvedQuantity, decimal? approvedAmount, string? glossReason = null)
        {
            ApprovedQuantity = approvedQuantity;
            ApprovedAmount = approvedAmount;
            
            if (approvedAmount.HasValue && approvedAmount.Value < TotalPrice)
            {
                GlosedAmount = TotalPrice - approvedAmount.Value;
                GlossReason = glossReason?.Trim();
            }
            else
            {
                GlosedAmount = null;
                GlossReason = null;
            }
            
            UpdateTimestamp();
        }

        public bool IsFullyApproved()
        {
            return ApprovedAmount.HasValue && ApprovedAmount.Value == TotalPrice;
        }

        public bool IsPartiallyApproved()
        {
            return ApprovedAmount.HasValue && ApprovedAmount.Value > 0 && ApprovedAmount.Value < TotalPrice;
        }

        public bool IsRejected()
        {
            return ApprovedAmount.HasValue && ApprovedAmount.Value == 0;
        }
    }
}
