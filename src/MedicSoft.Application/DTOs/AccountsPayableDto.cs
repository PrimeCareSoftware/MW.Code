using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs
{
    public class AccountsPayableDto
    {
        public Guid Id { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public Guid? SupplierId { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? CancellationReason { get; set; }
        public int? InstallmentNumber { get; set; }
        public int? TotalInstallments { get; set; }
        public string? BankName { get; set; }
        public string? BankAccount { get; set; }
        public string? PixKey { get; set; }
        public int DaysOverdue { get; set; }
        public bool IsOverdue { get; set; }
        public SupplierDto? Supplier { get; set; }
        public List<PayablePaymentDto> Payments { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class PayablePaymentDto
    {
        public Guid Id { get; set; }
        public Guid PayableId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? TransactionId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateAccountsPayableDto
    {
        public string DocumentNumber { get; set; } = string.Empty;
        public Guid? SupplierId { get; set; }
        public int Category { get; set; } // PayableCategory enum value
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string Description { get; set; } = string.Empty;
        public int? InstallmentNumber { get; set; }
        public int? TotalInstallments { get; set; }
        public string? BankName { get; set; }
        public string? BankAccount { get; set; }
        public string? PixKey { get; set; }
    }

    public class UpdateAccountsPayableDto
    {
        public string? Notes { get; set; }
        public string? BankName { get; set; }
        public string? BankAccount { get; set; }
        public string? PixKey { get; set; }
    }

    public class AddPayablePaymentDto
    {
        public Guid PayableId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? TransactionId { get; set; }
        public string? Notes { get; set; }
    }

    public class CancelPayableDto
    {
        public Guid PayableId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
