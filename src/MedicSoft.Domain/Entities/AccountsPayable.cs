using System;
using System.Collections.Generic;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Status da conta a pagar
    /// </summary>
    public enum PayableStatus
    {
        Pending = 1,        // Pendente
        PartiallyPaid = 2,  // Parcialmente pago
        Paid = 3,           // Pago
        Overdue = 4,        // Vencido
        Cancelled = 5       // Cancelado
    }

    /// <summary>
    /// Categoria de despesa
    /// </summary>
    public enum ExpenseCategory
    {
        Rent = 1,              // Aluguel
        Salaries = 2,          // Salários
        Supplies = 3,          // Materiais e suprimentos
        Equipment = 4,         // Equipamentos
        Maintenance = 5,       // Manutenção
        Utilities = 6,         // Utilidades (água, luz, etc.)
        Marketing = 7,         // Marketing
        Insurance = 8,         // Seguros
        Taxes = 9,            // Impostos
        ProfessionalServices = 10, // Serviços profissionais
        Laboratory = 11,       // Laboratório
        Pharmacy = 12,         // Farmácia
        Other = 99            // Outros
    }

    /// <summary>
    /// Representa uma conta a pagar do sistema
    /// </summary>
    public class AccountsPayable : BaseEntity
    {
        public string DocumentNumber { get; private set; }
        public Guid? SupplierId { get; private set; }
        public ExpenseCategory Category { get; private set; }
        public PayableStatus Status { get; private set; }
        public DateTime IssueDate { get; private set; }
        public DateTime DueDate { get; private set; }
        public decimal TotalAmount { get; private set; }
        public decimal PaidAmount { get; private set; }
        public decimal OutstandingAmount { get; private set; }
        public string Description { get; private set; }
        public string? Notes { get; private set; }
        public DateTime? PaymentDate { get; private set; }
        public string? CancellationReason { get; private set; }
        
        // Informações de parcelamento
        public int? InstallmentNumber { get; private set; }
        public int? TotalInstallments { get; private set; }
        
        // Informações bancárias
        public string? BankName { get; private set; }
        public string? BankAccount { get; private set; }
        public string? PixKey { get; private set; }
        
        // Navigation properties
        public Supplier? Supplier { get; private set; }
        private readonly List<PayablePayment> _payments = new();
        public IReadOnlyCollection<PayablePayment> Payments => _payments.AsReadOnly();

        private AccountsPayable()
        {
            // EF Constructor
            DocumentNumber = null!;
            Description = null!;
        }

        public AccountsPayable(
            string documentNumber,
            ExpenseCategory category,
            DateTime dueDate,
            decimal totalAmount,
            string description,
            string tenantId,
            Guid? supplierId = null,
            int? installmentNumber = null,
            int? totalInstallments = null) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(documentNumber))
                throw new ArgumentException("Document number cannot be empty", nameof(documentNumber));

            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));

            if (totalAmount <= 0)
                throw new ArgumentException("Total amount must be greater than zero", nameof(totalAmount));

            if (installmentNumber.HasValue && totalInstallments.HasValue)
            {
                if (installmentNumber.Value <= 0 || installmentNumber.Value > totalInstallments.Value)
                    throw new ArgumentException("Invalid installment number", nameof(installmentNumber));
            }

            DocumentNumber = documentNumber.Trim();
            Category = category;
            Status = PayableStatus.Pending;
            IssueDate = DateTime.UtcNow;
            DueDate = dueDate;
            TotalAmount = totalAmount;
            PaidAmount = 0;
            OutstandingAmount = totalAmount;
            Description = description.Trim();
            SupplierId = supplierId;
            InstallmentNumber = installmentNumber;
            TotalInstallments = totalInstallments;
        }

        public void AddPayment(decimal amount, DateTime paymentDate, string? transactionId = null, string? notes = null)
        {
            if (Status == PayableStatus.Cancelled)
                throw new InvalidOperationException("Cannot add payment to a cancelled payable");

            if (Status == PayableStatus.Paid)
                throw new InvalidOperationException("Payable is already fully paid");

            if (amount <= 0)
                throw new ArgumentException("Payment amount must be greater than zero", nameof(amount));

            if (amount > OutstandingAmount)
                throw new ArgumentException("Payment amount exceeds outstanding amount", nameof(amount));

            var payment = new PayablePayment(
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
                Status = PayableStatus.Paid;
                PaymentDate = paymentDate;
            }
            else
            {
                Status = PayableStatus.PartiallyPaid;
            }

            UpdateTimestamp();
        }

        public void Cancel(string reason)
        {
            if (Status == PayableStatus.Paid)
                throw new InvalidOperationException("Cannot cancel a fully paid payable");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Cancellation reason is required", nameof(reason));

            Status = PayableStatus.Cancelled;
            CancellationReason = reason.Trim();
            UpdateTimestamp();
        }

        public void MarkAsOverdue()
        {
            if (Status == PayableStatus.Paid || Status == PayableStatus.Cancelled)
                return;

            if (DateTime.Today > DueDate && OutstandingAmount > 0)
            {
                Status = PayableStatus.Overdue;
                UpdateTimestamp();
            }
        }

        public void SetBankingInfo(string? bankName = null, string? bankAccount = null, string? pixKey = null)
        {
            BankName = bankName?.Trim();
            BankAccount = bankAccount?.Trim();
            PixKey = pixKey?.Trim();
            UpdateTimestamp();
        }

        public int GetDaysOverdue()
        {
            if (DateTime.Today <= DueDate)
                return 0;

            return (DateTime.Today - DueDate.Date).Days;
        }

        public bool IsOverdue()
        {
            return DateTime.Today > DueDate && OutstandingAmount > 0 && Status != PayableStatus.Cancelled;
        }

        public void UpdateNotes(string notes)
        {
            Notes = notes?.Trim();
            UpdateTimestamp();
        }
    }

    /// <summary>
    /// Representa um pagamento de uma conta a pagar
    /// </summary>
    public class PayablePayment : BaseEntity
    {
        public Guid PayableId { get; private set; }
        public decimal Amount { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public string? TransactionId { get; private set; }
        public string? Notes { get; private set; }

        // Navigation property
        public AccountsPayable? Payable { get; private set; }

        private PayablePayment()
        {
            // EF Constructor
        }

        public PayablePayment(
            Guid payableId,
            decimal amount,
            DateTime paymentDate,
            string tenantId,
            string? transactionId = null,
            string? notes = null) : base(tenantId)
        {
            if (payableId == Guid.Empty)
                throw new ArgumentException("Payable ID cannot be empty", nameof(payableId));

            if (amount <= 0)
                throw new ArgumentException("Amount must be greater than zero", nameof(amount));

            PayableId = payableId;
            Amount = amount;
            PaymentDate = paymentDate;
            TransactionId = transactionId?.Trim();
            Notes = notes?.Trim();
        }
    }
}
