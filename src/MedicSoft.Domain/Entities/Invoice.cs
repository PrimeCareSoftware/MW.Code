using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public enum InvoiceStatus
    {
        Draft = 1,          // Rascunho
        Issued = 2,         // Emitida
        Sent = 3,           // Enviada
        Paid = 4,           // Paga
        Cancelled = 5,      // Cancelada
        Overdue = 6         // Vencida
    }

    public enum InvoiceType
    {
        Appointment = 1,    // Consulta
        Subscription = 2,   // Assinatura
        Service = 3         // Serviço
    }

    /// <summary>
    /// Represents an invoice (nota fiscal) for a payment.
    /// Supports appointment and subscription invoices.
    /// </summary>
    public class Invoice : BaseEntity
    {
        public string InvoiceNumber { get; private set; } = null!;
        public Guid PaymentId { get; private set; }
        public InvoiceType Type { get; private set; }
        public InvoiceStatus Status { get; private set; }
        public DateTime IssueDate { get; private set; }
        public DateTime DueDate { get; private set; }
        public decimal Amount { get; private set; }
        public decimal TaxAmount { get; private set; }
        public decimal TotalAmount { get; private set; }
        public string? Description { get; private set; }
        public string? Notes { get; private set; }
        public DateTime? SentDate { get; private set; }
        public DateTime? PaidDate { get; private set; }
        public DateTime? CancellationDate { get; private set; }
        public string? CancellationReason { get; private set; }

        // Customer details (denormalized for invoice history)
        public string CustomerName { get; private set; } = null!;
        public string? CustomerDocument { get; private set; }
        public string? CustomerAddress { get; private set; }

        // Navigation properties
        public Payment Payment { get; private set; } = null!;

        private Invoice()
        {
            // EF Constructor
        }

        public Invoice(string invoiceNumber, Guid paymentId, InvoiceType type,
            decimal amount, decimal taxAmount, DateTime dueDate,
            string customerName, string tenantId,
            string? description = null, string? customerDocument = null,
            string? customerAddress = null) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(invoiceNumber))
                throw new ArgumentException("Invoice number is required", nameof(invoiceNumber));

            if (paymentId == Guid.Empty)
                throw new ArgumentException("Payment ID cannot be empty", nameof(paymentId));

            if (amount <= 0)
                throw new ArgumentException("Invoice amount must be greater than zero", nameof(amount));

            if (taxAmount < 0)
                throw new ArgumentException("Tax amount cannot be negative", nameof(taxAmount));

            if (string.IsNullOrWhiteSpace(customerName))
                throw new ArgumentException("Customer name is required", nameof(customerName));

            InvoiceNumber = invoiceNumber.Trim();
            PaymentId = paymentId;
            Type = type;
            Status = InvoiceStatus.Draft;
            IssueDate = DateTime.UtcNow;
            DueDate = dueDate;
            Amount = amount;
            TaxAmount = taxAmount;
            TotalAmount = amount + taxAmount;
            Description = description?.Trim();
            CustomerName = customerName.Trim();
            CustomerDocument = customerDocument?.Trim();
            CustomerAddress = customerAddress?.Trim();
        }

        public void Issue()
        {
            if (Status != InvoiceStatus.Draft)
                throw new InvalidOperationException("Only draft invoices can be issued");

            Status = InvoiceStatus.Issued;
            IssueDate = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void MarkAsSent()
        {
            if (Status != InvoiceStatus.Issued && Status != InvoiceStatus.Overdue)
                throw new InvalidOperationException("Only issued or overdue invoices can be marked as sent");

            Status = InvoiceStatus.Sent;
            SentDate = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void MarkAsPaid(DateTime paidDate)
        {
            if (Status == InvoiceStatus.Cancelled)
                throw new InvalidOperationException("Cannot mark a cancelled invoice as paid");

            if (Status == InvoiceStatus.Draft)
                throw new InvalidOperationException("Cannot mark a draft invoice as paid");

            Status = InvoiceStatus.Paid;
            PaidDate = paidDate;
            UpdateTimestamp();
        }

        public void MarkAsOverdue()
        {
            if (Status == InvoiceStatus.Paid || Status == InvoiceStatus.Cancelled)
                throw new InvalidOperationException("Cannot mark paid or cancelled invoices as overdue");

            if (DateTime.UtcNow <= DueDate)
                throw new InvalidOperationException("Cannot mark invoice as overdue before due date");

            Status = InvoiceStatus.Overdue;
            UpdateTimestamp();
        }

        public void Cancel(string reason)
        {
            if (Status == InvoiceStatus.Paid)
                throw new InvalidOperationException("Cannot cancel a paid invoice");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Cancellation reason is required", nameof(reason));

            Status = InvoiceStatus.Cancelled;
            CancellationReason = reason.Trim();
            CancellationDate = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void UpdateAmount(decimal amount, decimal taxAmount)
        {
            if (Status != InvoiceStatus.Draft)
                throw new InvalidOperationException("Can only update amount for draft invoices");

            if (amount <= 0)
                throw new ArgumentException("Invoice amount must be greater than zero", nameof(amount));

            if (taxAmount < 0)
                throw new ArgumentException("Tax amount cannot be negative", nameof(taxAmount));

            Amount = amount;
            TaxAmount = taxAmount;
            TotalAmount = amount + taxAmount;
            UpdateTimestamp();
        }

        public void UpdateDescription(string description)
        {
            if (Status != InvoiceStatus.Draft)
                throw new InvalidOperationException("Can only update description for draft invoices");

            Description = description?.Trim();
            UpdateTimestamp();
        }

        public bool IsOverdue()
        {
            return Status != InvoiceStatus.Paid &&
                   Status != InvoiceStatus.Cancelled &&
                   DateTime.UtcNow > DueDate;
        }

        public int DaysUntilDue()
        {
            if (Status == InvoiceStatus.Paid || Status == InvoiceStatus.Cancelled)
                return 0;

            var days = (DueDate - DateTime.UtcNow).Days;
            return Math.Max(0, days);
        }

        public int DaysOverdue()
        {
            if (!IsOverdue())
                return 0;

            return (DateTime.UtcNow - DueDate).Days;
        }
    }
}
