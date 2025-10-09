using System;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class ClinicSubscriptionTests
    {
        private readonly string _tenantId = "test-tenant";
        private readonly Guid _clinicId = Guid.NewGuid();
        private readonly Guid _planId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithTrialPeriod_CreatesTrialSubscription()
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var trialDays = 15;
            var price = 99.90m;

            // Act
            var subscription = new ClinicSubscription(_clinicId, _planId, startDate, trialDays, price, _tenantId);

            // Assert
            Assert.NotEqual(Guid.Empty, subscription.Id);
            Assert.Equal(_clinicId, subscription.ClinicId);
            Assert.Equal(_planId, subscription.SubscriptionPlanId);
            Assert.Equal(startDate, subscription.StartDate);
            Assert.Equal(price, subscription.CurrentPrice);
            Assert.Equal(SubscriptionStatus.Trial, subscription.Status);
            Assert.NotNull(subscription.TrialEndDate);
            Assert.Equal(startDate.AddDays(trialDays), subscription.TrialEndDate.Value);
        }

        [Fact]
        public void Constructor_WithoutTrialPeriod_CreatesActiveSubscription()
        {
            // Arrange
            var startDate = DateTime.UtcNow;
            var trialDays = 0;
            var price = 99.90m;

            // Act
            var subscription = new ClinicSubscription(_clinicId, _planId, startDate, trialDays, price, _tenantId);

            // Assert
            Assert.Equal(SubscriptionStatus.Active, subscription.Status);
            Assert.Null(subscription.TrialEndDate);
            Assert.NotNull(subscription.NextPaymentDate);
            Assert.Equal(startDate.AddMonths(1), subscription.NextPaymentDate.Value);
        }

        [Fact]
        public void Constructor_WithEmptyClinicId_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new ClinicSubscription(Guid.Empty, _planId, DateTime.UtcNow, 15, 99.90m, _tenantId));
        }

        [Fact]
        public void Constructor_WithEmptyPlanId_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new ClinicSubscription(_clinicId, Guid.Empty, DateTime.UtcNow, 15, 99.90m, _tenantId));
        }

        [Fact]
        public void Constructor_WithNegativePrice_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new ClinicSubscription(_clinicId, _planId, DateTime.UtcNow, 15, -10m, _tenantId));
        }

        [Fact]
        public void Activate_ChangesStatusToActive()
        {
            // Arrange
            var subscription = CreateTrialSubscription();
            subscription.Suspend("Test");

            // Act
            subscription.Activate();

            // Assert
            Assert.Equal(SubscriptionStatus.Active, subscription.Status);
            Assert.NotNull(subscription.UpdatedAt);
        }

        [Fact]
        public void Activate_WhenCancelled_ThrowsInvalidOperationException()
        {
            // Arrange
            var subscription = CreateTrialSubscription();
            subscription.Cancel("Test cancellation");

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => subscription.Activate());
        }

        [Fact]
        public void Suspend_WithReason_SuspendsSubscription()
        {
            // Arrange
            var subscription = CreateActiveSubscription();
            var reason = "Payment issue";

            // Act
            subscription.Suspend(reason);

            // Assert
            Assert.Equal(SubscriptionStatus.Suspended, subscription.Status);
            Assert.Equal(reason, subscription.CancellationReason);
            Assert.NotNull(subscription.UpdatedAt);
        }

        [Fact]
        public void Suspend_WithEmptyReason_ThrowsArgumentException()
        {
            // Arrange
            var subscription = CreateActiveSubscription();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => subscription.Suspend(""));
        }

        [Fact]
        public void Cancel_WithReason_CancelsSubscription()
        {
            // Arrange
            var subscription = CreateActiveSubscription();
            var reason = "Customer request";

            // Act
            subscription.Cancel(reason);

            // Assert
            Assert.Equal(SubscriptionStatus.Cancelled, subscription.Status);
            Assert.Equal(reason, subscription.CancellationReason);
            Assert.NotNull(subscription.CancellationDate);
            Assert.NotNull(subscription.EndDate);
            Assert.NotNull(subscription.UpdatedAt);
        }

        [Fact]
        public void MarkPaymentOverdue_ChangesStatusToPaymentOverdue()
        {
            // Arrange
            var subscription = CreateActiveSubscription();

            // Act
            subscription.MarkPaymentOverdue();

            // Assert
            Assert.Equal(SubscriptionStatus.PaymentOverdue, subscription.Status);
        }

        [Fact]
        public void ProcessPayment_UpdatesPaymentDates()
        {
            // Arrange
            var subscription = CreateActiveSubscription();
            var paymentDate = DateTime.UtcNow;
            var amount = 99.90m;

            // Act
            subscription.ProcessPayment(paymentDate, amount);

            // Assert
            Assert.Equal(paymentDate, subscription.LastPaymentDate);
            Assert.NotNull(subscription.NextPaymentDate);
            Assert.Equal(paymentDate.AddMonths(1), subscription.NextPaymentDate.Value);
            Assert.Equal(SubscriptionStatus.Active, subscription.Status);
        }

        [Fact]
        public void ProcessPayment_WhenOverdue_ActivatesSubscription()
        {
            // Arrange
            var subscription = CreateActiveSubscription();
            subscription.MarkPaymentOverdue();

            // Act
            subscription.ProcessPayment(DateTime.UtcNow, 99.90m);

            // Assert
            Assert.Equal(SubscriptionStatus.Active, subscription.Status);
        }

        [Fact]
        public void ConvertFromTrial_WhenInTrial_ConvertsToActive()
        {
            // Arrange
            var subscription = CreateTrialSubscription();

            // Act
            subscription.ConvertFromTrial();

            // Assert
            Assert.Equal(SubscriptionStatus.Active, subscription.Status);
            Assert.NotNull(subscription.NextPaymentDate);
        }

        [Fact]
        public void ConvertFromTrial_WhenNotInTrial_ThrowsInvalidOperationException()
        {
            // Arrange
            var subscription = CreateActiveSubscription();

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => subscription.ConvertFromTrial());
        }

        [Fact]
        public void UpdatePrice_UpdatesCurrentPrice()
        {
            // Arrange
            var subscription = CreateActiveSubscription();
            var newPrice = 149.90m;

            // Act
            subscription.UpdatePrice(newPrice);

            // Assert
            Assert.Equal(newPrice, subscription.CurrentPrice);
            Assert.NotNull(subscription.UpdatedAt);
        }

        [Fact]
        public void IsTrialActive_WhenInTrial_ReturnsTrue()
        {
            // Arrange
            var subscription = CreateTrialSubscription();

            // Act & Assert
            Assert.True(subscription.IsTrialActive());
        }

        [Fact]
        public void IsTrialActive_WhenNotInTrial_ReturnsFalse()
        {
            // Arrange
            var subscription = CreateActiveSubscription();

            // Act & Assert
            Assert.False(subscription.IsTrialActive());
        }

        [Fact]
        public void IsActive_WhenInTrial_ReturnsTrue()
        {
            // Arrange
            var subscription = CreateTrialSubscription();

            // Act & Assert
            Assert.True(subscription.IsActive());
        }

        [Fact]
        public void IsActive_WhenActive_ReturnsTrue()
        {
            // Arrange
            var subscription = CreateActiveSubscription();

            // Act & Assert
            Assert.True(subscription.IsActive());
        }

        [Fact]
        public void IsActive_WhenCancelled_ReturnsFalse()
        {
            // Arrange
            var subscription = CreateActiveSubscription();
            subscription.Cancel("Test");

            // Act & Assert
            Assert.False(subscription.IsActive());
        }

        [Fact]
        public void DaysUntilTrialEnd_WhenInTrial_ReturnsCorrectDays()
        {
            // Arrange
            var subscription = CreateTrialSubscription();

            // Act
            var daysLeft = subscription.DaysUntilTrialEnd();

            // Assert
            Assert.True(daysLeft >= 0 && daysLeft <= 15);
        }

        [Fact]
        public void DaysUntilTrialEnd_WhenNotInTrial_ReturnsZero()
        {
            // Arrange
            var subscription = CreateActiveSubscription();

            // Act
            var daysLeft = subscription.DaysUntilTrialEnd();

            // Assert
            Assert.Equal(0, daysLeft);
        }

        [Fact]
        public void SubscriptionStatus_HasAllExpectedValues()
        {
            // Assert
            Assert.True(Enum.IsDefined(typeof(SubscriptionStatus), SubscriptionStatus.Trial));
            Assert.True(Enum.IsDefined(typeof(SubscriptionStatus), SubscriptionStatus.Active));
            Assert.True(Enum.IsDefined(typeof(SubscriptionStatus), SubscriptionStatus.Suspended));
            Assert.True(Enum.IsDefined(typeof(SubscriptionStatus), SubscriptionStatus.PaymentOverdue));
            Assert.True(Enum.IsDefined(typeof(SubscriptionStatus), SubscriptionStatus.Cancelled));
        }

        private ClinicSubscription CreateTrialSubscription()
        {
            return new ClinicSubscription(_clinicId, _planId, DateTime.UtcNow, 15, 99.90m, _tenantId);
        }

        private ClinicSubscription CreateActiveSubscription()
        {
            return new ClinicSubscription(_clinicId, _planId, DateTime.UtcNow, 0, 99.90m, _tenantId);
        }
    }
}
