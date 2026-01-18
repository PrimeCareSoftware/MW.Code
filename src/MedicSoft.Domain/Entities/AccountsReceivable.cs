using System;
using System.Collections.Generic;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Status da conta a receber
    /// </summary>
    public enum ReceivableStatus
    {
        Pending = 1,        // Pendente
        PartiallyPaid = 2,  // Parcialmente pago
        Paid = 3,           // Pago
        Overdue = 4,        // Vencido
        Cancelled = 5,      // Cancelado
        InNegotiation = 6   // Em negociação
    }

    /// <summary>
    /// Tipo de recebível
    /// </summary>
    public enum ReceivableType
    {
        Consultation = 1,      // Consulta
        Procedure = 2,         // Procedimento
        Exam = 3,             // Exame
        HealthInsurance = 4,  // Convênio
        Other = 5             // Outros
    }

    /// <summary>
    /// Representa uma conta a receber do sistema
    /// </summary>
    public class AccountsReceivable : BaseEntity
    {
        public Guid? AppointmentId { get; private set; }
        public Guid? PatientId { get; private set; }
        public Guid? HealthInsuranceOperatorId { get; private set; }
        public string DocumentNumber { get; private set; }
        public ReceivableType Type { get; private set; }
        public ReceivableStatus Status { get; private set; }
        public DateTime IssueDate { get; private set; }
        public DateTime DueDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public decimal PaidAmount { get; private set; }
        public decimal OutstandingAmount { get; private set; }
        public string? Description { get; private set; }
        public string? Notes { get; private set; }
        public DateTime? SettlementDate { get; private set; }
        public string? CancellationReason { get; private set; }
        
        // Informações de parcelamento
        public int? InstallmentNumber { get; private set; }
        public int? TotalInstallments { get; private set; }
        
        // Penalidades
        public decimal? InterestRate { get; private set; }  // Taxa de juros ao mês
        public decimal? FineRate { get; private set; }      // Multa percentual
        public decimal? DiscountRate { get; private set; }  // Desconto para pagamento antecipado
        
        // Navigation properties
        public Appointment? Appointment { get; private set; }
        public Patient? Patient { get; private set; }
        public HealthInsuranceOperator? HealthInsuranceOperator { get; private set; }
        private readonly List<ReceivablePayment> _payments = new();
        public IReadOnlyCollection<ReceivablePayment> Payments => _payments.AsReadOnly();

        private AccountsReceivable()
        {
            // EF Constructor
            DocumentNumber = null!;
        }

        public AccountsReceivable(
            string documentNumber,
            ReceivableType type,
            DateTime dueDate,
            decimal totalAmount,
            string tenantId,
            Guid? appointmentId = null,
            Guid? patientId = null,
            Guid? healthInsuranceOperatorId = null,
            string? description = null,
            int? installmentNumber = null,
            int? totalInstallments = null) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                throw new ArgumentException("Document number cannot be empty", nameof(documentNumber));

            if (totalAmount <= 0)
                throw new ArgumentException("Total amount must be greater than zero", nameof(totalAmount));

            if (dueDate < DateTime.Today)
                throw new ArgumentException("Due date cannot be in the past", nameof(dueDate));

            if (installmentNumber.HasValue && totalInstallments.HasValue)
            {
                if (installmentNumber.Value <= 0 || installmentNumber.Value > totalInstallments.Value)
                    throw new ArgumentException("Invalid installment number", nameof(installmentNumber));
            }

            DocumentNumber = documentNumber.Trim();
            Type = type;
            Status = ReceivableStatus.Pending;
            IssueDate = DateTime.UtcNow;
            DueDate = dueDate;
            TotalAmount = totalAmount;
            PaidAmount = 0;
            OutstandingAmount = totalAmount;
            AppointmentId = appointmentId;
            PatientId = patientId;
            HealthInsuranceOperatorId = healthInsuranceOperatorId;
            Description = description?.Trim();
            InstallmentNumber = installmentNumber;
            TotalInstallments = totalInstallments;
        }

        public void AddPayment(decimal amount, DateTime paymentDate, string? transactionId = null, string? notes = null)
        {
            if (Status == ReceivableStatus.Cancelled)
                throw new InvalidOperationException("Cannot add payment to a cancelled receivable");

            if (Status == ReceivableStatus.Paid)
                throw new InvalidOperationException("Receivable is already fully paid");

            if (amount <= 0)
                throw new ArgumentException("Payment amount must be greater than zero", nameof(amount));

            if (amount > OutstandingAmount)
                throw new ArgumentException("Payment amount exceeds outstanding amount", nameof(amount));

            var payment = new ReceivablePayment(
                Id,
                amount,
                paymentDate,
                TenantId,
                transactionId,
                notes
            );

            _payments.Add(payment);
            PaidAmount += amount;
            OutstandingAmount -= amount;

            if (OutstandingAmount == 0)
            {
                Status = ReceivableStatus.Paid;
                SettlementDate = paymentDate;
            }
            else
            {
                Status = ReceivableStatus.PartiallyPaid;
            }

            UpdateTimestamp();
        }

        public void Cancel(string reason)
        {
            if (Status == ReceivableStatus.Paid)
                throw new InvalidOperationException("Cannot cancel a fully paid receivable");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Cancellation reason is required", nameof(reason));

            Status = ReceivableStatus.Cancelled;
            CancellationReason = reason.Trim();
            UpdateTimestamp();
        }

        public void MarkAsOverdue()
        {
            if (Status == ReceivableStatus.Paid || Status == ReceivableStatus.Cancelled)
                return;

            if (DateTime.Today > DueDate && OutstandingAmount > 0)
            {
                Status = ReceivableStatus.Overdue;
                UpdateTimestamp();
            }
        }

        public void SetInNegotiation()
        {
            if (Status == ReceivableStatus.Paid || Status == ReceivableStatus.Cancelled)
                throw new InvalidOperationException($"Cannot set receivable in status {Status} to negotiation");

            Status = ReceivableStatus.InNegotiation;
            UpdateTimestamp();
        }

        public void SetCharges(decimal? interestRate = null, decimal? fineRate = null, decimal? discountRate = null)
        {
            if (interestRate.HasValue && interestRate.Value < 0)
                throw new ArgumentException("Interest rate cannot be negative", nameof(interestRate));

            if (fineRate.HasValue && (fineRate.Value < 0 || fineRate.Value > 100))
                throw new ArgumentException("Fine rate must be between 0 and 100", nameof(fineRate));

            if (discountRate.HasValue && (discountRate.Value < 0 || discountRate.Value > 100))
                throw new ArgumentException("Discount rate must be between 0 and 100", nameof(discountRate));

            InterestRate = interestRate;
            FineRate = fineRate;
            DiscountRate = discountRate;
            UpdateTimestamp();
        }

        public decimal CalculateTotalWithCharges()
        {
            if (Status == ReceivableStatus.Paid || DateTime.Today <= DueDate)
                return OutstandingAmount;

            decimal total = OutstandingAmount;
            
            // Aplicar multa
            if (FineRate.HasValue && FineRate.Value > 0)
            {
                total += OutstandingAmount * (FineRate.Value / 100);
            }

            // Aplicar juros
            if (InterestRate.HasValue && InterestRate.Value > 0)
            {
                var monthsOverdue = (int)Math.Ceiling((DateTime.Today - DueDate).TotalDays / 30.0);
                total += OutstandingAmount * (InterestRate.Value / 100) * monthsOverdue;
            }

            return total;
        }

        public decimal CalculateTotalWithDiscount()
        {
            if (DiscountRate.HasValue && DiscountRate.Value > 0 && DateTime.Today <= DueDate)
            {
                return OutstandingAmount * (1 - (DiscountRate.Value / 100));
            }

            return OutstandingAmount;
        }

        public int GetDaysOverdue()
        {
            if (DateTime.Today <= DueDate)
                return 0;

            return (DateTime.Today - DueDate.Date).Days;
        }

        public bool IsOverdue()
        {
            return DateTime.Today > DueDate && OutstandingAmount > 0 && Status != ReceivableStatus.Cancelled;
        }

        public void UpdateNotes(string notes)
        {
            Notes = notes?.Trim();
            UpdateTimestamp();
        }
    }

    /// <summary>
    /// Representa um pagamento de uma conta a receber
    /// </summary>
    public class ReceivablePayment : BaseEntity
    {
        public Guid ReceivableId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public string? TransactionId { get; private set; }
        public string? Notes { get; private set; }

        // Navigation property
        public AccountsReceivable? Receivable { get; private set; }

        private ReceivablePayment()
        {
            // EF Constructor
        }

        public ReceivablePayment(
            Guid receivableId,
            decimal amount,
            DateTime paymentDate,
            string tenantId,
            string? transactionId = null,
            string? notes = null) : base(tenantId)
        {
            if (receivableId == Guid.Empty)
                throw new ArgumentException("Receivable ID cannot be empty", nameof(receivableId));

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero", nameof(amount));

            ReceivableId = receivableId;
            Amount = amount;
            PaymentDate = paymentDate;
            TransactionId = transactionId?.Trim();
            Notes = notes?.Trim();
        }
    }
}
