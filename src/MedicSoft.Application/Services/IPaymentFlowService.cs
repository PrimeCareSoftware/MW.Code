using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for orchestrating the complete payment flow
    /// Integrates Appointment payment tracking with Payment entity, Invoice generation, and TISS
    /// </summary>
    public interface IPaymentFlowService
    {
        /// <summary>
        /// Register payment for an appointment and create all related entities
        /// Creates: Payment entity, Invoice, and TISS guide if health insurance
        /// </summary>
        /// <param name="appointmentId">Appointment ID</param>
        /// <param name="paidByUserId">User who received the payment</param>
        /// <param name="paymentReceiverType">Type of receiver (Doctor/Secretary/Other)</param>
        /// <param name="amount">Payment amount</param>
        /// <param name="paymentMethod">Payment method</param>
        /// <param name="tenantId">Tenant ID</param>
        /// <param name="notes">Optional payment notes</param>
        /// <returns>Payment flow result with Payment and Invoice IDs</returns>
        Task<PaymentFlowResultDto> RegisterAppointmentPaymentAsync(
            Guid appointmentId,
            Guid paidByUserId,
            string paymentReceiverType,
            decimal amount,
            string paymentMethod,
            string tenantId,
            string? notes = null);

        /// <summary>
        /// Register payment during appointment completion (by doctor)
        /// </summary>
        Task<PaymentFlowResultDto> RegisterPaymentOnCompletionAsync(
            Guid appointmentId,
            Guid completedByUserId,
            decimal amount,
            string paymentMethod,
            string tenantId,
            string? notes = null);
    }
}
