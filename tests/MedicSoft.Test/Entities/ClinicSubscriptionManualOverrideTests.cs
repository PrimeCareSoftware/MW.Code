using System;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class ClinicSubscriptionManualOverrideTests
    {
        private const string TenantId = "test-tenant";

        [Fact]
        public void EnableManualOverride_WithValidData_EnablesOverride()
        {
            // Arrange
            var subscription = CreateTestSubscription();
            var reason = "Free access for friend doctor";
            var username = "admin";

            // Act
            subscription.EnableManualOverride(reason, username);

            // Assert
            Assert.True(subscription.ManualOverrideActive);
            Assert.Equal(reason, subscription.ManualOverrideReason);
            Assert.Equal(username, subscription.ManualOverrideSetBy);
            Assert.NotNull(subscription.ManualOverrideSetAt);
            Assert.True(subscription.ManualOverrideSetAt <= DateTime.UtcNow);
        }

        [Fact]
        public void EnableManualOverride_WithEmptyReason_ThrowsArgumentException()
        {
            // Arrange
            var subscription = CreateTestSubscription();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                subscription.EnableManualOverride("", "admin"));
        }

        [Fact]
        public void EnableManualOverride_WithNullReason_ThrowsArgumentException()
        {
            // Arrange
            var subscription = CreateTestSubscription();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                subscription.EnableManualOverride(null!, "admin"));
        }

        [Fact]
        public void EnableManualOverride_WithEmptyUsername_ThrowsArgumentException()
        {
            // Arrange
            var subscription = CreateTestSubscription();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                subscription.EnableManualOverride("Test reason", ""));
        }

        [Fact]
        public void DisableManualOverride_ClearsAllOverrideFields()
        {
            // Arrange
            var subscription = CreateTestSubscription();
            subscription.EnableManualOverride("Test reason", "admin");

            // Act
            subscription.DisableManualOverride();

            // Assert
            Assert.False(subscription.ManualOverrideActive);
            Assert.Null(subscription.ManualOverrideReason);
            Assert.Null(subscription.ManualOverrideSetBy);
            Assert.Null(subscription.ManualOverrideSetAt);
        }

        [Fact]
        public void CanAccessWithOverride_WhenOverrideActive_ReturnsTrue()
        {
            // Arrange
            var subscription = CreateTestSubscription();
            subscription.MarkPaymentOverdue(); // Would normally block access
            subscription.EnableManualOverride("Testing access", "admin");

            // Act
            var canAccess = subscription.CanAccessWithOverride();

            // Assert
            Assert.True(canAccess);
        }

        [Fact]
        public void CanAccessWithOverride_WhenOverrideInactive_UsesNormalRules()
        {
            // Arrange
            var subscription = CreateTestSubscription();
            subscription.MarkPaymentOverdue(); // Blocks access

            // Act
            var canAccess = subscription.CanAccessWithOverride();

            // Assert
            Assert.False(canAccess);
        }

        [Fact]
        public void CanAccessWithOverride_WhenActiveAndNoOverride_ReturnsTrue()
        {
            // Arrange
            var subscription = CreateTestSubscription();
            subscription.Activate();

            // Act
            var canAccess = subscription.CanAccessWithOverride();

            // Assert
            Assert.True(canAccess);
        }

        [Fact]
        public void ManualOverride_AllowsAccessEvenWhenCancelled()
        {
            // Arrange
            var subscription = CreateTestSubscription();
            subscription.Cancel("Test cancellation");
            subscription.EnableManualOverride("Special case", "admin");

            // Act
            var canAccess = subscription.CanAccessWithOverride();

            // Assert
            Assert.True(canAccess);
            Assert.Equal(SubscriptionStatus.Cancelled, subscription.Status);
        }

        [Fact]
        public void ManualOverride_AllowsAccessEvenWhenSuspended()
        {
            // Arrange
            var subscription = CreateTestSubscription();
            subscription.Suspend("Test suspension");
            subscription.EnableManualOverride("Testing", "admin");

            // Act
            var canAccess = subscription.CanAccessWithOverride();

            // Assert
            Assert.True(canAccess);
        }

        [Fact]
        public void ManualOverride_AllowsAccessEvenWhenFrozen()
        {
            // Arrange
            var subscription = CreateTestSubscription();
            subscription.Freeze();
            subscription.EnableManualOverride("Special access", "admin");

            // Act
            var canAccess = subscription.CanAccessWithOverride();

            // Assert
            Assert.True(canAccess);
        }

        private ClinicSubscription CreateTestSubscription()
        {
            var clinicId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var startDate = DateTime.UtcNow;
            var trialDays = 15;
            var price = 100.00m;

            return new ClinicSubscription(clinicId, planId, startDate, trialDays, price, TenantId);
        }
    }
}
