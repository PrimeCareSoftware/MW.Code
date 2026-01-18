using System;
using System.Collections.Generic;

namespace MedicSoft.Application.DTOs
{
    public class AccountsReceivableDto
    {
        public Guid Id { get; set; }
        public Guid? AppointmentId { get; set; }
        public Guid? PatientId { get; set; }
        public Guid? HealthInsuranceOperatorId { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public DateTime? SettlementDate { get; set; }
        public string? CancellationReason { get; set; }
        public int? InstallmentNumber { get; set; }
        public int? TotalInstallments { get; set; }
        public decimal? InterestRate { get; set; }
        public decimal? FineRate { get; set; }
        public decimal? DiscountRate { get; set; }
        public int DaysOverdue { get; set; }
        public bool IsOverdue { get; set; }
        public List<ReceivablePaymentDto> Payments { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ReceivablePaymentDto
    {
        public Guid Id { get; set; }
        public Guid ReceivableId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? TransactionId { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CreateAccountsReceivableDto
    {
        public Guid? AppointmentId { get; set; }
        public Guid? PatientId { get; set; }
        public Guid? HealthInsuranceOperatorId { get; set; }
        public string DocumentNumber { get; set; } = string.Empty;
        public int Type { get; set; } // ReceivableType enum value
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Description { get; set; }
        public int? InstallmentNumber { get; set; }
        public int? TotalInstallments { get; set; }
    }

    public class UpdateAccountsReceivableDto
    {
        public string? Notes { get; set; }
        public decimal? InterestRate { get; set; }
        public decimal? FineRate { get; set; }
        public decimal? DiscountRate { get; set; }
    }

    public class AddReceivablePaymentDto
    {
        public Guid ReceivableId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string? TransactionId { get; set; }
        public string? Notes { get; set; }
    }

    public class CancelReceivableDto
    {
        public Guid ReceivableId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
