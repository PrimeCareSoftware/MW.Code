using System;

namespace MedicSoft.Application.DTOs
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid? AppointmentId { get; set; }
        public Guid? ClinicSubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public DateTime? ProcessedDate { get; set; }
        public string? TransactionId { get; set; }
        public string? Notes { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime? CancellationDate { get; set; }
        public string? CardLastFourDigits { get; set; }
        public string? PixKey { get; set; }
        public string? PixTransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreatePaymentDto
    {
        public Guid? AppointmentId { get; set; }
        public Guid? ClinicSubscriptionId { get; set; }
        public decimal Amount { get; set; }
        public string Method { get; set; } = "Cash";
        public string? Notes { get; set; }
        public string? CardLastFourDigits { get; set; }
        public string? PixKey { get; set; }
    }

    public class ProcessPaymentDto
    {
        public Guid PaymentId { get; set; }
        public string TransactionId { get; set; } = string.Empty;
    }

    public class RefundPaymentDto
    {
        public Guid PaymentId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }

    public class CancelPaymentDto
    {
        public Guid PaymentId { get; set; }
        public string Reason { get; set; } = string.Empty;
    }
}
