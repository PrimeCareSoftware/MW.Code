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
        
        // Freeze functionality
        public bool IsFrozen { get; private set; }
        public DateTime? FrozenStartDate { get; private set; }
        public DateTime? FrozenEndDate { get; private set; }
        
        // Upgrade/Downgrade tracking
        public Guid? PendingPlanId { get; private set; }
        public decimal? PendingPlanPrice { get; private set; }
        public DateTime? PlanChangeDate { get; private set; }
        public bool IsUpgrade { get; private set; }

        // Navigation properties
        public Clinic? Clinic { get; private set; }
        public SubscriptionPlan? SubscriptionPlan { get; private set; }
        public SubscriptionPlan? PendingPlan { get; private set; }

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

        /// <summary>
        /// Freeze subscription for 1 month - suspends billing and access
        /// </summary>
        public void Freeze()
        {
            if (Status == SubscriptionStatus.Cancelled)
                throw new InvalidOperationException("Cannot freeze a cancelled subscription");

            if (IsFrozen)
                throw new InvalidOperationException("Subscription is already frozen");

            IsFrozen = true;
            FrozenStartDate = DateTime.UtcNow;
            FrozenEndDate = DateTime.UtcNow.AddMonths(1);
            Status = SubscriptionStatus.Frozen;
            
            // Extend next payment date by 1 month
            if (NextPaymentDate.HasValue)
                NextPaymentDate = NextPaymentDate.Value.AddMonths(1);
            
            UpdateTimestamp();
        }

        /// <summary>
        /// Unfreeze subscription - reactivates billing and access
        /// </summary>
        public void Unfreeze()
        {
            if (!IsFrozen)
                throw new InvalidOperationException("Subscription is not frozen");

            IsFrozen = false;
            FrozenStartDate = null;
            FrozenEndDate = null;
            Status = SubscriptionStatus.Active;
            UpdateTimestamp();
        }

        /// <summary>
        /// Schedule plan upgrade - charges difference immediately
        /// </summary>
        public void ScheduleUpgrade(Guid newPlanId, decimal newPlanPrice)
        {
            if (newPlanPrice <= CurrentPrice)
                throw new ArgumentException("New plan price must be higher for upgrade", nameof(newPlanPrice));

            if (Status == SubscriptionStatus.Cancelled)
                throw new InvalidOperationException("Cannot upgrade a cancelled subscription");

            PendingPlanId = newPlanId;
            PendingPlanPrice = newPlanPrice;
            PlanChangeDate = DateTime.UtcNow;
            IsUpgrade = true;
            UpdateTimestamp();
        }

        /// <summary>
        /// Apply upgrade immediately - used after payment of difference
        /// </summary>
        public void ApplyUpgrade()
        {
            if (!PendingPlanId.HasValue || !IsUpgrade)
                throw new InvalidOperationException("No pending upgrade found");

            SubscriptionPlanId = PendingPlanId.Value;
            CurrentPrice = PendingPlanPrice ?? CurrentPrice;
            PendingPlanId = null;
            PendingPlanPrice = null;
            PlanChangeDate = null;
            IsUpgrade = false;
            UpdateTimestamp();
        }

        /// <summary>
        /// Schedule plan downgrade - applies in next billing cycle
        /// </summary>
        public void ScheduleDowngrade(Guid newPlanId, decimal newPlanPrice)
        {
            if (newPlanPrice >= CurrentPrice)
                throw new ArgumentException("New plan price must be lower for downgrade", nameof(newPlanPrice));

            if (Status == SubscriptionStatus.Cancelled)
                throw new InvalidOperationException("Cannot downgrade a cancelled subscription");

            PendingPlanId = newPlanId;
            PendingPlanPrice = newPlanPrice;
            PlanChangeDate = NextPaymentDate ?? DateTime.UtcNow.AddMonths(1);
            IsUpgrade = false;
            UpdateTimestamp();
        }

        /// <summary>
        /// Apply downgrade in next billing cycle
        /// </summary>
        public void ApplyDowngrade()
        {
            if (!PendingPlanId.HasValue || IsUpgrade)
                throw new InvalidOperationException("No pending downgrade found");

            if (PlanChangeDate.HasValue && DateTime.UtcNow < PlanChangeDate.Value)
                throw new InvalidOperationException("Cannot apply downgrade before scheduled date");

            SubscriptionPlanId = PendingPlanId.Value;
            CurrentPrice = PendingPlanPrice ?? CurrentPrice;
            PendingPlanId = null;
            PendingPlanPrice = null;
            PlanChangeDate = null;
            IsUpgrade = false;
            UpdateTimestamp();
        }

        /// <summary>
        /// Cancel any pending plan changes
        /// </summary>
        public void CancelPendingPlanChange()
        {
            PendingPlanId = null;
            PendingPlanPrice = null;
            PlanChangeDate = null;
            IsUpgrade = false;
            UpdateTimestamp();
        }

        /// <summary>
        /// Get the amount to charge for an upgrade (difference between plans)
        /// </summary>
        public decimal GetUpgradeAmount()
        {
            if (!PendingPlanPrice.HasValue || !IsUpgrade)
                return 0;

            return PendingPlanPrice.Value - CurrentPrice;
        }
    }

    public enum SubscriptionStatus
    {
        Trial,           // In free trial period
        Active,          // Active paid subscription
        Suspended,       // Temporarily suspended
        PaymentOverdue,  // Payment is overdue
        Frozen,          // Frozen for 1 month
        Cancelled        // Cancelled subscription
    }
}
