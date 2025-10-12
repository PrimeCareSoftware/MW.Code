using System;
using System.Threading.Tasks;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Domain.Services
{
    /// <summary>
    /// Domain service for managing subscription logic and notifications
    /// </summary>
    public interface ISubscriptionService
    {
        /// <summary>
        /// Check if subscription payment is overdue and send notifications
        /// </summary>
        Task CheckAndNotifyOverduePayments();

        /// <summary>
        /// Process subscription renewal
        /// </summary>
        Task<bool> ProcessRenewal(Guid subscriptionId, decimal amount);

        /// <summary>
        /// Send payment overdue notification via SMS, Email, and WhatsApp
        /// </summary>
        Task SendPaymentOverdueNotification(ClinicSubscription subscription);

        /// <summary>
        /// Validate if clinic can access based on subscription status
        /// </summary>
        bool CanAccessSystem(ClinicSubscription subscription, string environment);

        /// <summary>
        /// Validate if user count is within plan limits
        /// </summary>
        bool IsWithinUserLimit(SubscriptionPlan plan, int currentUserCount);

        /// <summary>
        /// Validate if patient count is within plan limits
        /// </summary>
        bool IsWithinPatientLimit(SubscriptionPlan plan, int currentPatientCount);
    }

    public class SubscriptionService : ISubscriptionService
    {
        private readonly INotificationService _notificationService;
        private readonly string _environment;

        public SubscriptionService(INotificationService notificationService, string environment = "Production")
        {
            _notificationService = notificationService;
            _environment = environment;
        }

        public Task CheckAndNotifyOverduePayments()
        {
            // This would be called by a background job
            // Implementation would query all subscriptions with overdue payments
            // and send notifications
            return Task.CompletedTask;
        }

        public Task<bool> ProcessRenewal(Guid subscriptionId, decimal amount)
        {
            // Process payment and update subscription
            return Task.FromResult(true);
        }

        public async Task SendPaymentOverdueNotification(ClinicSubscription subscription)
        {
            if (subscription.Clinic == null)
                throw new ArgumentException("Clinic information is required", nameof(subscription));

            var message = $@"Prezado(a) {subscription.Clinic.Name},

Identificamos que o pagamento da sua assinatura está em atraso.

⚠️ ATENÇÃO: Seu acesso ao sistema MedicWarehouse ficará indisponível até a regularização do pagamento.

Valor: R$ {subscription.CurrentPrice:F2}
Data de vencimento: {subscription.NextPaymentDate:dd/MM/yyyy}

Para regularizar, acesse: https://medicwarehouse.com.br/pagamento

Após o pagamento, seu acesso será restabelecido automaticamente.

Dúvidas? Entre em contato conosco.

Atenciosamente,
Equipe MedicWarehouse";

            // Send via all channels
            await _notificationService.SendSMS(subscription.Clinic.Phone, message);
            await _notificationService.SendEmail(subscription.Clinic.Email, "Pagamento em Atraso - MedicWarehouse", message);
            await _notificationService.SendWhatsApp(subscription.Clinic.Phone, message);
        }

        public bool CanAccessSystem(ClinicSubscription subscription, string environment)
        {
            // In Development or Staging, always allow access (no payment enforcement)
            if (environment.Equals("Development", StringComparison.OrdinalIgnoreCase) ||
                environment.Equals("Staging", StringComparison.OrdinalIgnoreCase) ||
                environment.Equals("Homologacao", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            // Manual override allows access regardless of payment status
            // Used for giving free access to friends or special cases
            if (subscription.ManualOverrideActive)
                return true;

            // Regular subscription rules for Production
            if (subscription.IsFrozen)
                return false;

            if (subscription.Status == SubscriptionStatus.Cancelled)
                return false;

            if (subscription.Status == SubscriptionStatus.PaymentOverdue)
                return false;

            if (subscription.Status == SubscriptionStatus.Suspended)
                return false;

            return subscription.IsActive();
        }

        public bool IsWithinUserLimit(SubscriptionPlan plan, int currentUserCount)
        {
            return currentUserCount < plan.MaxUsers;
        }

        public bool IsWithinPatientLimit(SubscriptionPlan plan, int currentPatientCount)
        {
            return currentPatientCount < plan.MaxPatients;
        }
    }

    /// <summary>
    /// Service for sending notifications via different channels
    /// </summary>
    public interface INotificationService
    {
        Task SendSMS(string phoneNumber, string message);
        Task SendEmail(string email, string subject, string message);
        Task SendWhatsApp(string phoneNumber, string message);
    }
}
