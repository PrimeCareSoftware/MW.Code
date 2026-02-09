using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public enum PaymentMethod
    {
        Cash = 1,           // Dinheiro
        CreditCard = 2,     // Cartão de Crédito
        DebitCard = 3,      // Cartão de Débito
        Pix = 4,            // PIX
        BankTransfer = 5,   // Transferência Bancária
        Check = 6           // Cheque
    }

    public enum PaymentStatus
    {
        Pending = 1,        // Pendente
        Processing = 2,     // Processando
        Paid = 3,           // Pago
        Failed = 4,         // Falhou
        Refunded = 5,       // Reembolsado
        Cancelled = 6       // Cancelado
    }

    /// <summary>
    /// Represents a payment made for an appointment or subscription.
    /// Supports multiple payment methods including cash, credit card, and PIX.
    /// </summary>
    public class Payment : BaseEntity
    {
        public Guid? AppointmentId { get; private set; }
        public Guid? ClinicSubscriptionId { get; private set; }
        public Guid? AppointmentProcedureId { get; private set; }
        public decimal Amount { get; private set; }
        public PaymentMethod Method { get; private set; }
        public PaymentStatus Status { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public DateTime? ProcessedDate { get; private set; }
        public string? TransactionId { get; private set; }
        public string? Notes { get; private set; }
        public string? CancellationReason { get; private set; }
        public DateTime? CancellationDate { get; private set; }

        // Payment method specific fields
        public string? CardLastFourDigits { get; private set; }
        public string? PixKey { get; private set; }
        public string? PixTransactionId { get; private set; }

        // Navigation properties
        public Appointment? Appointment { get; private set; }
        public ClinicSubscription? ClinicSubscription { get; private set; }
        public AppointmentProcedure? AppointmentProcedure { get; private set; }
        public Invoice? Invoice { get; private set; }

        private Payment()
        {
            // EF Constructor
        }

        public Payment(decimal amount, PaymentMethod method, string tenantId,
            Guid? appointmentId = null, Guid? clinicSubscriptionId = null,
            Guid? appointmentProcedureId = null, string? notes = null) : base(tenantId)
        {
            if (amount <= 0)
                throw new ArgumentException("Payment amount must be greater than zero", nameof(amount));

            if (appointmentId == null && clinicSubscriptionId == null && appointmentProcedureId == null)
                throw new ArgumentException("Payment must be associated with either an appointment, subscription, or procedure");

            if (appointmentId != null && appointmentId == Guid.Empty)
                throw new ArgumentException("Invalid appointment ID", nameof(appointmentId));

            if (clinicSubscriptionId != null && clinicSubscriptionId == Guid.Empty)
                throw new ArgumentException("Invalid subscription ID", nameof(clinicSubscriptionId));

            if (appointmentProcedureId != null && appointmentProcedureId == Guid.Empty)
                throw new ArgumentException("Invalid appointment procedure ID", nameof(appointmentProcedureId));

            Amount = amount;
            Method = method;
            Status = PaymentStatus.Pending;
            PaymentDate = DateTime.UtcNow;
            AppointmentId = appointmentId;
            ClinicSubscriptionId = clinicSubscriptionId;
            AppointmentProcedureId = appointmentProcedureId;
            Notes = notes?.Trim();
        }

        public void MarkAsPaid(string transactionId)
        {
            if (Status == PaymentStatus.Paid)
                throw new InvalidOperationException("Payment is already marked as paid");

            if (Status == PaymentStatus.Refunded)
                throw new InvalidOperationException("Cannot mark a refunded payment as paid");

            if (Status == PaymentStatus.Cancelled)
                throw new InvalidOperationException("Cannot mark a cancelled payment as paid");

            Status = PaymentStatus.Paid;
            ProcessedDate = DateTime.UtcNow;
            TransactionId = transactionId?.Trim();
            UpdateTimestamp();
        }

        public void MarkAsProcessing()
        {
            if (Status != PaymentStatus.Pending)
                throw new InvalidOperationException("Only pending payments can be marked as processing");

            Status = PaymentStatus.Processing;
            UpdateTimestamp();
        }

        public void MarkAsFailed(string reason)
        {
            if (Status == PaymentStatus.Paid)
                throw new InvalidOperationException("Cannot mark a paid payment as failed");

            if (Status == PaymentStatus.Refunded)
                throw new InvalidOperationException("Cannot mark a refunded payment as failed");

            Status = PaymentStatus.Failed;
            Notes = string.IsNullOrWhiteSpace(Notes) 
                ? reason?.Trim() 
                : $"{Notes}; {reason?.Trim()}";
            UpdateTimestamp();
        }

        public void Refund(string reason)
        {
            if (Status != PaymentStatus.Paid)
                throw new InvalidOperationException("Only paid payments can be refunded");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Refund reason is required", nameof(reason));

            Status = PaymentStatus.Refunded;
            CancellationReason = reason.Trim();
            CancellationDate = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void Cancel(string reason)
        {
            if (Status == PaymentStatus.Paid)
                throw new InvalidOperationException("Cannot cancel a paid payment. Use Refund instead");

            if (Status == PaymentStatus.Refunded)
                throw new InvalidOperationException("Cannot cancel a refunded payment");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Cancellation reason is required", nameof(reason));

            Status = PaymentStatus.Cancelled;
            CancellationReason = reason.Trim();
            CancellationDate = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void SetCardDetails(string lastFourDigits)
        {
            if (Method != PaymentMethod.CreditCard && Method != PaymentMethod.DebitCard)
                throw new InvalidOperationException("Card details can only be set for card payments");

            if (string.IsNullOrWhiteSpace(lastFourDigits) || lastFourDigits.Length != 4)
                throw new ArgumentException("Last four digits must be exactly 4 characters", nameof(lastFourDigits));

            CardLastFourDigits = lastFourDigits.Trim();
            UpdateTimestamp();
        }

        public void SetPixDetails(string pixKey, string pixTransactionId)
        {
            if (Method != PaymentMethod.Pix)
                throw new InvalidOperationException("PIX details can only be set for PIX payments");

            if (string.IsNullOrWhiteSpace(pixKey))
                throw new ArgumentException("PIX key is required", nameof(pixKey));

            PixKey = pixKey.Trim();
            PixTransactionId = pixTransactionId?.Trim();
            UpdateTimestamp();
        }

        public bool IsPaid()
        {
            return Status == PaymentStatus.Paid;
        }

        public bool CanBeRefunded()
        {
            return Status == PaymentStatus.Paid;
        }

        public bool CanBeCancelled()
        {
            return Status == PaymentStatus.Pending || Status == PaymentStatus.Failed;
        }
    }
}
