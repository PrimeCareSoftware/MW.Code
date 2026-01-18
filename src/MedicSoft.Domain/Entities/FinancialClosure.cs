using System;
using System.Collections.Generic;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Status do fechamento financeiro
    /// </summary>
    public enum FinancialClosureStatus
    {
        Open = 1,              // Aberto
        PendingPayment = 2,    // Aguardando pagamento
        PartiallyPaid = 3,     // Parcialmente pago
        Closed = 4,            // Fechado
        Cancelled = 5          // Cancelado
    }

    /// <summary>
    /// Forma de pagamento do fechamento
    /// </summary>
    public enum ClosurePaymentType
    {
        OutOfPocket = 1,       // Particular
        HealthInsurance = 2,   // Convênio
        Mixed = 3              // Misto (particular + convênio)
    }

    /// <summary>
    /// Representa o fechamento financeiro de uma consulta/atendimento
    /// </summary>
    public class FinancialClosure : BaseEntity
    {
        public Guid AppointmentId { get; private set; }
        public Guid PatientId { get; private set; }
        public Guid? HealthInsuranceOperatorId { get; private set; }
        public string ClosureNumber { get; private set; }
        public FinancialClosureStatus Status { get; private set; }
        public ClosurePaymentType PaymentType { get; private set; }
        public DateTime ClosureDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public decimal PatientAmount { get; private set; }      // Valor pago pelo paciente
        public decimal InsuranceAmount { get; private set; }    // Valor cobrado do convênio
        public decimal PaidAmount { get; private set; }
        public decimal OutstandingAmount { get; private set; }
        public string? Notes { get; private set; }
        public DateTime? SettlementDate { get; private set; }
        public string? CancellationReason { get; private set; }
        
        // Informações de desconto
        public decimal? DiscountAmount { get; private set; }
        public string? DiscountReason { get; private set; }
        
        // Navigation properties
        public Appointment? Appointment { get; private set; }
        public Patient? Patient { get; private set; }
        public HealthInsuranceOperator? HealthInsuranceOperator { get; private set; }
        private readonly List<FinancialClosureItem> _items = new();
        public IReadOnlyCollection<FinancialClosureItem> Items => _items.AsReadOnly();

        private FinancialClosure()
        {
            // EF Constructor
            ClosureNumber = null!;
        }

        public FinancialClosure(
            Guid appointmentId,
            Guid patientId,
            string closureNumber,
            ClosurePaymentType paymentType,
            string tenantId,
            Guid? healthInsuranceOperatorId = null) : base(tenantId)
        {
            if (appointmentId == Guid.Empty)
                throw new ArgumentException("Appointment ID cannot be empty", nameof(appointmentId));

            if (patientId == Guid.Empty)
                throw new ArgumentException("Patient ID cannot be empty", nameof(patientId));

            if (string.IsNullOrWhiteSpace(closureNumber))
                throw new ArgumentException("Closure number cannot be empty", nameof(closureNumber));

            if (paymentType == ClosurePaymentType.HealthInsurance && healthInsuranceOperatorId == null)
                throw new ArgumentException("Health insurance operator is required for insurance payment type", nameof(healthInsuranceOperatorId));

            AppointmentId = appointmentId;
            PatientId = patientId;
            ClosureNumber = closureNumber.Trim();
            Status = FinancialClosureStatus.Open;
            PaymentType = paymentType;
            HealthInsuranceOperatorId = healthInsuranceOperatorId;
            ClosureDate = DateTime.UtcNow;
            TotalAmount = 0;
            PatientAmount = 0;
            InsuranceAmount = 0;
            PaidAmount = 0;
            OutstandingAmount = 0;
        }

        public void AddItem(string description, decimal quantity, decimal unitPrice, bool coverByInsurance = false)
        {
            if (Status != FinancialClosureStatus.Open)
                throw new InvalidOperationException($"Cannot add items to closure in status {Status}");

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

            var item = new FinancialClosureItem(
                Id,
                description,
                quantity,
                unitPrice,
                TenantId,
                coverByInsurance
            );

            _items.Add(item);
            RecalculateTotals();
            UpdateTimestamp();
        }

        public void RemoveItem(Guid itemId)
        {
            if (Status != FinancialClosureStatus.Open)
                throw new InvalidOperationException($"Cannot remove items from closure in status {Status}");

            var item = _items.FirstOrDefault(i => i.Id == itemId);
            if (item != null)
            {
                _items.Remove(item);
                RecalculateTotals();
                UpdateTimestamp();
            }
        }

        private void RecalculateTotals()
        {
            TotalAmount = _items.Sum(i => i.TotalPrice);
            InsuranceAmount = _items.Where(i => i.CoverByInsurance).Sum(i => i.TotalPrice);
            PatientAmount = TotalAmount - InsuranceAmount;
            
            // Aplicar desconto se houver
            if (DiscountAmount.HasValue && DiscountAmount.Value > 0)
            {
                var discountedTotal = TotalAmount - DiscountAmount.Value;
                if (discountedTotal < 0)
                    discountedTotal = 0;
                
                // Distribuir desconto proporcionalmente
                if (TotalAmount > 0)
                {
                    var discountRatio = DiscountAmount.Value / TotalAmount;
                    InsuranceAmount -= InsuranceAmount * discountRatio;
                    PatientAmount -= PatientAmount * discountRatio;
                    TotalAmount = discountedTotal;
                }
            }
            
            OutstandingAmount = TotalAmount - PaidAmount;
        }

        public void ApplyDiscount(decimal discountAmount, string reason)
        {
            if (Status == FinancialClosureStatus.Closed || Status == FinancialClosureStatus.Cancelled)
                throw new InvalidOperationException($"Cannot apply discount to closure in status {Status}");

            if (discountAmount < 0)
                throw new ArgumentException("Discount amount cannot be negative", nameof(discountAmount));

            if (discountAmount > TotalAmount)
                throw new ArgumentException("Discount amount cannot exceed total amount", nameof(discountAmount));

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Discount reason is required", nameof(reason));

            DiscountAmount = discountAmount;
            DiscountReason = reason.Trim();
            RecalculateTotals();
            UpdateTimestamp();
        }

        public void RecordPayment(decimal amount)
        {
            if (Status == FinancialClosureStatus.Closed)
                throw new InvalidOperationException("Closure is already closed");

            if (Status == FinancialClosureStatus.Cancelled)
                throw new InvalidOperationException("Cannot record payment on cancelled closure");

            if (amount <= 0)
                throw new ArgumentException("Payment amount must be greater than zero", nameof(amount));

            if (amount > OutstandingAmount)
                throw new ArgumentException("Payment amount exceeds outstanding amount", nameof(amount));

            PaidAmount += amount;
            OutstandingAmount -= amount;

            if (OutstandingAmount == 0)
            {
                Status = FinancialClosureStatus.Closed;
                SettlementDate = DateTime.UtcNow;
            }
            else
            {
                Status = FinancialClosureStatus.PartiallyPaid;
            }

            UpdateTimestamp();
        }

        public void MarkAsPendingPayment()
        {
            if (Status != FinancialClosureStatus.Open)
                throw new InvalidOperationException($"Cannot mark closure as pending payment in status {Status}");

            if (!_items.Any())
                throw new InvalidOperationException("Cannot mark closure as pending payment without items");

            Status = FinancialClosureStatus.PendingPayment;
            UpdateTimestamp();
        }

        public void Cancel(string reason)
        {
            if (Status == FinancialClosureStatus.Closed)
                throw new InvalidOperationException("Cannot cancel a closed closure");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Cancellation reason is required", nameof(reason));

            Status = FinancialClosureStatus.Cancelled;
            CancellationReason = reason.Trim();
            UpdateTimestamp();
        }

        public void UpdateNotes(string notes)
        {
            Notes = notes?.Trim();
            UpdateTimestamp();
        }
    }

    /// <summary>
    /// Representa um item do fechamento financeiro
    /// </summary>
    public class FinancialClosureItem : BaseEntity
    {
        public Guid ClosureId { get; private set; }
        public string Description { get; private set; }
        public decimal Quantity { get; private set; }
        public decimal UnitPrice { get; private set; }
        public decimal TotalPrice { get; private set; }
        public bool CoverByInsurance { get; private set; }  // Se é coberto por convênio

        // Navigation property
        public FinancialClosure? Closure { get; private set; }

        private FinancialClosureItem()
        {
            // EF Constructor
            Description = null!;
        }

        public FinancialClosureItem(
            Guid closureId,
            string description,
            decimal quantity,
            decimal unitPrice,
            string tenantId,
            bool coverByInsurance = false) : base(tenantId)
        {
            if (closureId == Guid.Empty)
                throw new ArgumentException("Closure ID cannot be empty", nameof(closureId));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            if (unitPrice < 0)
                throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

            ClosureId = closureId;
            Description = description.Trim();
            Quantity = quantity;
            UnitPrice = unitPrice;
            TotalPrice = quantity * unitPrice;
            CoverByInsurance = coverByInsurance;
        }

        public void UpdateQuantity(decimal quantity)
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
    }
}
