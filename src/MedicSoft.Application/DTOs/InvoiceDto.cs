using System;

namespace MedicSoft.Application.DTOs
{
    public class InvoiceDto
    {
        public Guid Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public Guid PaymentId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public DateTime? SentDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string? CancellationReason { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? CustomerDocument { get; set; }
        public string? CustomerAddress { get; set; }
        public int DaysUntilDue { get; set; }
        public int DaysOverdue { get; set; }
        public bool IsOverdue { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateInvoiceDto
    {
        public string InvoiceNumber { get; set; } = string.Empty;
        public Guid PaymentId { get; set; }
        public string Type { get; set; } = "Appointment";
        public decimal Amount { get; set; }
        public decimal TaxAmount { get; set; }
        public DateTime DueDate { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CustomerDocument { get; set; }
        public string? CustomerAddress { get; set; }
    }

    public class UpdateInvoiceAmountDto
    {
        public Guid InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxAmount { get; set; }
    }

    public class CancelInvoiceDto
    {
        public Guid InvoiceId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
