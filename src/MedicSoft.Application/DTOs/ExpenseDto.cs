using System;

namespace MedicSoft.Application.DTOs
{
    public class ExpenseDto
    {
        public Guid Id { get; set; }
        public Guid ClinicId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? PaidDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? PaymentMethod { get; set; }
        public string? PaymentReference { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierDocument { get; set; }
        public string? Notes { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? DaysOverdue { get; set; }
    }

    public class CreateExpenseDto
    {
        public Guid ClinicId { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = "Other";
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierDocument { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateExpenseDto
    {
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = "Other";
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string? SupplierName { get; set; }
        public string? SupplierDocument { get; set; }
        public string? Notes { get; set; }
    }

    public class PayExpenseDto
    {
        public string PaymentMethod { get; set; } = "Cash";
        public string? PaymentReference { get; set; }
    }

    public class CancelExpenseDto
    {
        public string Reason { get; set; } = string.Empty;
    }
}
