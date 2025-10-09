using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a clinic's subscription to a plan.
    /// Manages trial period, billing status, and subscription lifecycle.
    /// </summary>
    public class ClinicSubscription : BaseEntity
    {
        public Guid ClinicId { get; private set; }
        public Guid SubscriptionPlanId { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public DateTime? TrialEndDate { get; private set; }
        public SubscriptionStatus Status { get; private set; }
        public DateTime? LastPaymentDate { get; private set; }
        public DateTime? NextPaymentDate { get; private set; }
        public decimal CurrentPrice { get; private set; }
        public string? CancellationReason { get; private set; }
        public DateTime? CancellationDate { get; private set; }

        // Navigation properties
        public Clinic? Clinic { get; private set; }
        public SubscriptionPlan? SubscriptionPlan { get; private set; }

        private ClinicSubscription()
        {
            // EF Constructor
        }

        public ClinicSubscription(Guid clinicId, Guid subscriptionPlanId,
            DateTime startDate, int trialDays, decimal price, string tenantId) : base(tenantId)
        {
            if (clinicId == Guid.Empty)
                throw new ArgumentException("Clinic ID cannot be empty", nameof(clinicId));

            if (subscriptionPlanId == Guid.Empty)
                throw new ArgumentException("Subscription plan ID cannot be empty", nameof(subscriptionPlanId));

            if (price < 0)
                throw new ArgumentException("Price cannot be negative", nameof(price));

            ClinicId = clinicId;
            SubscriptionPlanId = subscriptionPlanId;
            StartDate = startDate;
            CurrentPrice = price;

            if (trialDays > 0)
            {
                TrialEndDate = startDate.AddDays(trialDays);
                Status = SubscriptionStatus.Trial;
            }
            else
            {
                Status = SubscriptionStatus.Active;
                NextPaymentDate = startDate.AddMonths(1);
            }
        }

        public void Activate()
        {
            if (Status == SubscriptionStatus.Cancelled)
                throw new InvalidOperationException("Cannot activate a cancelled subscription");

            Status = SubscriptionStatus.Active;
            UpdateTimestamp();
        }

        public void Suspend(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Suspension reason is required", nameof(reason));

            Status = SubscriptionStatus.Suspended;
            CancellationReason = reason.Trim();
            UpdateTimestamp();
        }

        public void Cancel(string reason)
        {
            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Cancellation reason is required", nameof(reason));

            Status = SubscriptionStatus.Cancelled;
            CancellationReason = reason.Trim();
            CancellationDate = DateTime.UtcNow;
            EndDate = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void MarkPaymentOverdue()
        {
            Status = SubscriptionStatus.PaymentOverdue;
            UpdateTimestamp();
        }

        public void ProcessPayment(DateTime paymentDate, decimal amount)
        {
            if (amount < 0)
                throw new ArgumentException("Payment amount cannot be negative", nameof(amount));

            LastPaymentDate = paymentDate;
            NextPaymentDate = paymentDate.AddMonths(1);

            if (Status == SubscriptionStatus.PaymentOverdue || Status == SubscriptionStatus.Trial)
                Status = SubscriptionStatus.Active;

            UpdateTimestamp();
        }

        public void ConvertFromTrial()
        {
            if (Status != SubscriptionStatus.Trial)
                throw new InvalidOperationException("Subscription is not in trial period");

            Status = SubscriptionStatus.Active;
            NextPaymentDate = DateTime.UtcNow.AddMonths(1);
            UpdateTimestamp();
        }

        public void UpdatePrice(decimal newPrice)
        {
            if (newPrice < 0)
                throw new ArgumentException("Price cannot be negative", nameof(newPrice));

            CurrentPrice = newPrice;
            UpdateTimestamp();
        }

        public bool IsTrialActive()
        {
            return Status == SubscriptionStatus.Trial &&
                   TrialEndDate.HasValue &&
                   TrialEndDate.Value > DateTime.UtcNow;
        }

        public bool IsActive()
        {
            return Status == SubscriptionStatus.Active || IsTrialActive();
        }

        public int DaysUntilTrialEnd()
        {
            if (!TrialEndDate.HasValue || Status != SubscriptionStatus.Trial)
                return 0;

            var daysLeft = (TrialEndDate.Value - DateTime.UtcNow).Days;
            return Math.Max(0, daysLeft);
        }
    }

    public enum SubscriptionStatus
    {
        Trial,           // In free trial period
        Active,          // Active paid subscription
        Suspended,       // Temporarily suspended
        PaymentOverdue,  // Payment is overdue
        Cancelled        // Cancelled subscription
    }
}
