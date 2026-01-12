using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Representa um procedimento da tabela TUSS (Terminologia Unificada da Saúde Suplementar)
    /// </summary>
    public class TussProcedure : BaseEntity
    {
        public string Code { get; private set; } // Código TUSS (8 dígitos)
        public string Description { get; private set; } // Descrição do procedimento
        public string Category { get; private set; } // Categoria (ex: "Consultas", "Exames", "Cirurgias")
        public decimal ReferencePrice { get; private set; } // Preço de referência (AMB/CBHPM)
        public bool RequiresAuthorization { get; private set; } // Requer autorização prévia?
        public bool IsActive { get; private set; } = true;
        public DateTime? LastUpdated { get; private set; } // Data da última atualização da tabela TUSS
        
        private TussProcedure() 
        { 
            // EF Constructor
            Code = null!;
            Description = null!;
            Category = null!;
        }

        public TussProcedure(
            string code,
            string description,
            string category,
            decimal referencePrice,
            string tenantId,
            bool requiresAuthorization = false) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code cannot be empty", nameof(code));
            
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));
            
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category cannot be empty", nameof(category));
            
            if (referencePrice < 0)
                throw new ArgumentException("Reference price cannot be negative", nameof(referencePrice));

            Code = code.Trim();
            Description = description.Trim();
            Category = category.Trim();
            ReferencePrice = referencePrice;
            RequiresAuthorization = requiresAuthorization;
            LastUpdated = DateTime.UtcNow;
        }

        public void UpdateInfo(string description, string category, decimal referencePrice, bool requiresAuthorization)
        {
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));
            
            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category cannot be empty", nameof(category));
            
            if (referencePrice < 0)
                throw new ArgumentException("Reference price cannot be negative", nameof(referencePrice));

            Description = description.Trim();
            Category = category.Trim();
            ReferencePrice = referencePrice;
            RequiresAuthorization = requiresAuthorization;
            LastUpdated = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void UpdateReferencePrice(decimal referencePrice)
        {
            if (referencePrice < 0)
                throw new ArgumentException("Reference price cannot be negative", nameof(referencePrice));

            ReferencePrice = referencePrice;
            LastUpdated = DateTime.UtcNow;
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
}
