using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents the link between a procedure and the materials it uses.
    /// </summary>
    public class ProcedureMaterial : BaseEntity
    {
        public Guid ProcedureId { get; private set; }
        public Guid MaterialId { get; private set; }
        public int Quantity { get; private set; }

        // Navigation properties
        public Procedure? Procedure { get; private set; }
        public Material? Material { get; private set; }

        private ProcedureMaterial()
        {
            // EF Constructor
        }

        public ProcedureMaterial(Guid procedureId, Guid materialId, int quantity, string tenantId) : base(tenantId)
        {
            if (procedureId == Guid.Empty)
                throw new ArgumentException("Procedure ID cannot be empty", nameof(procedureId));

            if (materialId == Guid.Empty)
                throw new ArgumentException("Material ID cannot be empty", nameof(materialId));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            ProcedureId = procedureId;
            MaterialId = materialId;
            Quantity = quantity;
        }

        public void UpdateQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            Quantity = quantity;
            UpdateTimestamp();
        }
    }
}
