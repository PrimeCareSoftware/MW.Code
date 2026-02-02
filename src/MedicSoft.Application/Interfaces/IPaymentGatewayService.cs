using System;
using System.Threading;
using System.Threading.Tasks;

namespace MedicSoft.Application.Interfaces
{
    /// <summary>
    /// Interface for payment gateway operations
    /// </summary>
    public interface IPaymentGatewayService
    {
        /// <summary>
        /// Create a payment for a subscription
        /// </summary>
        Task<PaymentGatewayResult> CreateSubscriptionPaymentAsync(
            string customerId,
            string customerEmail,
            decimal amount,
            string planName,
            string tenantId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Create a payment for an appointment
        /// </summary>
        Task<PaymentGatewayResult> CreateAppointmentPaymentAsync(
            string customerId,
            string customerEmail,
            decimal amount,
            string description,
            string tenantId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Process a webhook notification from the payment gateway
        /// </summary>
        Task<bool> ProcessWebhookNotificationAsync(
            string payload,
            string signature,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Get payment status from the gateway
        /// </summary>
        Task<PaymentGatewayStatus> GetPaymentStatusAsync(
            string transactionId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Refund a payment
        /// </summary>
        Task<PaymentGatewayResult> RefundPaymentAsync(
            string transactionId,
            decimal? amount,
            string reason,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Check if the payment gateway is configured and available
        /// </summary>
        bool IsConfigured();
    }

    /// <summary>
    /// Result of a payment gateway operation
    /// </summary>
    public class PaymentGatewayResult
    {
        public bool Success { get; set; }
        public string? TransactionId { get; set; }
        public string? PaymentUrl { get; set; }
        public string? QrCode { get; set; }
        public PaymentGatewayStatus Status { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ErrorCode { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }

    /// <summary>
    /// Payment status from the gateway
    /// </summary>
    public enum PaymentGatewayStatus
    {
        Pending,
        Processing,
        Approved,
        Rejected,
        Cancelled,
        Refunded,
        ChargedBack
    }
}
