using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a material used in medical procedures.
    /// </summary>
    public class Material : BaseEntity
    {
        public string Name { get; private set; }
        public string Code { get; private set; }
        public string Description { get; private set; }
        public string Unit { get; private set; } // Unidade (caixa, frasco, unidade, etc.)
        public decimal UnitPrice { get; private set; }
        public int StockQuantity { get; private set; }
        public int MinimumStock { get; private set; }
        public bool IsActive { get; private set; }

        private Material()
        {
            // EF Constructor
            Name = null!;
            Code = null!;
            Description = null!;
            Unit = null!;
        }

        public Material(string name, string code, string description, string unit,
            decimal unitPrice, int stockQuantity, int minimumStock, string tenantId) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(code))
                throw new ArgumentException("Code cannot be empty", nameof(code));

            if (string.IsNullOrWhiteSpace(unit))
                throw new ArgumentException("Unit cannot be empty", nameof(unit));

            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

            if (stockQuantity < 0)
                throw new ArgumentException("Stock quantity cannot be negative", nameof(stockQuantity));

            if (minimumStock < 0)
                throw new ArgumentException("Minimum stock cannot be negative", nameof(minimumStock));

            Name = name.Trim();
            Code = code.Trim();
            Description = description?.Trim() ?? string.Empty;
            Unit = unit.Trim();
            UnitPrice = unitPrice;
            StockQuantity = stockQuantity;
            MinimumStock = minimumStock;
            IsActive = true;
        }

        public void Update(string name, string description, string unit,
            decimal unitPrice, int minimumStock)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(unit))
                throw new ArgumentException("Unit cannot be empty", nameof(unit));

            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

            if (minimumStock < 0)
                throw new ArgumentException("Minimum stock cannot be negative", nameof(minimumStock));

            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            Unit = unit.Trim();
            UnitPrice = unitPrice;
            MinimumStock = minimumStock;
            UpdateTimestamp();
        }

        public void AddStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            StockQuantity += quantity;
            UpdateTimestamp();
        }

        public void RemoveStock(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            if (quantity > StockQuantity)
                throw new InvalidOperationException("Insufficient stock");

            StockQuantity -= quantity;
            UpdateTimestamp();
        }

        public bool IsLowStock()
        {
            return StockQuantity <= MinimumStock;
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
