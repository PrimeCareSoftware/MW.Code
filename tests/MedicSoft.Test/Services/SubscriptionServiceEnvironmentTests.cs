using System;
using Xunit;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Services;
using System.Threading.Tasks;

namespace MedicSoft.Test.Services
{
    public class SubscriptionServiceEnvironmentTests
    {
        private readonly TestNotificationService _notificationService;

        public SubscriptionServiceEnvironmentTests()
        {
            _notificationService = new TestNotificationService();
        }

        [Fact]
        public void CanAccessSystem_InDevelopment_AlwaysReturnsTrue()
        {
            // Arrange
            var service = new SubscriptionService(_notificationService, "Development");
            var subscription = CreateOverdueSubscription();

            // Act
            var canAccess = service.CanAccessSystem(subscription, "Development");

            // Assert
            Assert.True(canAccess);
        }

        [Fact]
        public void CanAccessSystem_InStaging_AlwaysReturnsTrue()
        {
            // Arrange
            var service = new SubscriptionService(_notificationService, "Staging");
            var subscription = CreateOverdueSubscription();

            // Act
            var canAccess = service.CanAccessSystem(subscription, "Staging");

            // Assert
            Assert.True(canAccess);
        }

        [Fact]
        public void CanAccessSystem_InHomologacao_AlwaysReturnsTrue()
        {
            // Arrange
            var service = new SubscriptionService(_notificationService, "Homologacao");
            var subscription = CreateOverdueSubscription();

            // Act
            var canAccess = service.CanAccessSystem(subscription, "Homologacao");

            // Assert
            Assert.True(canAccess);
        }

        [Fact]
        public void CanAccessSystem_InProduction_WithOverduePayment_ReturnsFalse()
        {
            // Arrange
            var service = new SubscriptionService(_notificationService, "Production");
            var subscription = CreateOverdueSubscription();

            // Act
            var canAccess = service.CanAccessSystem(subscription, "Production");

            // Assert
            Assert.False(canAccess);
        }

        [Fact]
        public void CanAccessSystem_InProduction_WithManualOverride_ReturnsTrue()
        {
            // Arrange
            var service = new SubscriptionService(_notificationService, "Production");
            var subscription = CreateOverdueSubscription();
            subscription.EnableManualOverride("Free access for friend", "admin");

            // Act
            var canAccess = service.CanAccessSystem(subscription, "Production");

            // Assert
            Assert.True(canAccess);
        }

        [Fact]
        public void CanAccessSystem_InProduction_WithActiveSubscription_ReturnsTrue()
        {
            // Arrange
            var service = new SubscriptionService(_notificationService, "Production");
            var subscription = CreateActiveSubscription();

            // Act
            var canAccess = service.CanAccessSystem(subscription, "Production");

            // Assert
            Assert.True(canAccess);
        }

        [Fact]
        public void CanAccessSystem_InProduction_WithTrialActive_ReturnsTrue()
        {
            // Arrange
            var service = new SubscriptionService(_notificationService, "Production");
            var subscription = CreateTrialSubscription();

            // Act
            var canAccess = service.CanAccessSystem(subscription, "Production");

            // Assert
            Assert.True(canAccess);
        }

        [Fact]
        public void CanAccessSystem_InProduction_WithFrozenSubscription_ReturnsFalse()
        {
            // Arrange
            var service = new SubscriptionService(_notificationService, "Production");
            var subscription = CreateActiveSubscription();
            subscription.Freeze();

            // Act
            var canAccess = service.CanAccessSystem(subscription, "Production");

            // Assert
            Assert.False(canAccess);
        }

        [Fact]
        public void CanAccessSystem_InProduction_WithCancelledSubscription_ReturnsFalse()
        {
            // Arrange
            var service = new SubscriptionService(_notificationService, "Production");
            var subscription = CreateActiveSubscription();
            subscription.Cancel("Test cancellation");

            // Act
            var canAccess = service.CanAccessSystem(subscription, "Production");

            // Assert
            Assert.False(canAccess);
        }

        [Fact]
        public void CanAccessSystem_InProduction_WithSuspendedSubscription_ReturnsFalse()
        {
            // Arrange
            var service = new SubscriptionService(_notificationService, "Production");
            var subscription = CreateActiveSubscription();
            subscription.Suspend("Test suspension");

            // Act
            var canAccess = service.CanAccessSystem(subscription, "Production");

            // Assert
            Assert.False(canAccess);
        }

        [Fact]
        public void CanAccessSystem_EnvironmentNameCaseInsensitive_WorksCorrectly()
        {
            // Arrange
            var service = new SubscriptionService(_notificationService, "development");
            var subscription = CreateOverdueSubscription();

            // Act
            var canAccessLower = service.CanAccessSystem(subscription, "development");
            var canAccessUpper = service.CanAccessSystem(subscription, "DEVELOPMENT");
            var canAccessMixed = service.CanAccessSystem(subscription, "DevelopMent");

            // Assert
            Assert.True(canAccessLower);
            Assert.True(canAccessUpper);
            Assert.True(canAccessMixed);
        }

        private ClinicSubscription CreateOverdueSubscription()
        {
            var clinicId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var startDate = DateTime.UtcNow.AddMonths(-1);
            var trialDays = 0;
            var price = 100.00m;
            var tenantId = "test-tenant";

            var subscription = new ClinicSubscription(clinicId, planId, startDate, trialDays, price, tenantId);
            subscription.MarkPaymentOverdue();
            return subscription;
        }

        private ClinicSubscription CreateActiveSubscription()
        {
            var clinicId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var startDate = DateTime.UtcNow;
            var trialDays = 0;
            var price = 100.00m;
            var tenantId = "test-tenant";

            var subscription = new ClinicSubscription(clinicId, planId, startDate, trialDays, price, tenantId);
            subscription.Activate();
            return subscription;
        }

        private ClinicSubscription CreateTrialSubscription()
        {
            var clinicId = Guid.NewGuid();
            var planId = Guid.NewGuid();
            var startDate = DateTime.UtcNow;
            var trialDays = 15;
            var price = 100.00m;
            var tenantId = "test-tenant";

            return new ClinicSubscription(clinicId, planId, startDate, trialDays, price, tenantId);
        }

        // Simple test implementation of INotificationService
        private class TestNotificationService : INotificationService
        {
            public Task SendSMS(string phoneNumber, string message) => Task.CompletedTask;
            public Task SendEmail(string email, string subject, string message) => Task.CompletedTask;
            public Task SendWhatsApp(string phoneNumber, string message) => Task.CompletedTask;
        }
    }
}
