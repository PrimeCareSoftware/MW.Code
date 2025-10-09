using System;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class SubscriptionPlanTests
    {
        private readonly string _tenantId = "system";

        [Fact]
        public void Constructor_WithValidData_CreatesSubscriptionPlan()
        {
            // Arrange
            var name = "Basic Plan";
            var description = "Basic plan for small clinics";
            var monthlyPrice = 99.90m;
            var trialDays = 15;
            var maxUsers = 5;
            var maxPatients = 100;
            var type = PlanType.Basic;

            // Act
            var plan = new SubscriptionPlan(name, description, monthlyPrice, trialDays,
                maxUsers, maxPatients, type, _tenantId);

            // Assert
            Assert.NotEqual(Guid.Empty, plan.Id);
            Assert.Equal(name, plan.Name);
            Assert.Equal(description, plan.Description);
            Assert.Equal(monthlyPrice, plan.MonthlyPrice);
            Assert.Equal(trialDays, plan.TrialDays);
            Assert.Equal(maxUsers, plan.MaxUsers);
            Assert.Equal(maxPatients, plan.MaxPatients);
            Assert.Equal(type, plan.Type);
            Assert.True(plan.IsActive);
        }

        [Fact]
        public void Constructor_WithAllFeatures_CreatesSubscriptionPlan()
        {
            // Arrange & Act
            var plan = new SubscriptionPlan("Premium Plan", "Premium features", 299.90m,
                15, 20, 1000, PlanType.Premium, _tenantId,
                hasReports: true, hasWhatsAppIntegration: true,
                hasSMSNotifications: true, hasTissExport: true);

            // Assert
            Assert.True(plan.HasReports);
            Assert.True(plan.HasWhatsAppIntegration);
            Assert.True(plan.HasSMSNotifications);
            Assert.True(plan.HasTissExport);
        }

        [Fact]
        public void Constructor_WithEmptyName_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new SubscriptionPlan("", "Description", 99.90m, 15, 5, 100, PlanType.Basic, _tenantId));
        }

        [Fact]
        public void Constructor_WithNegativePrice_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new SubscriptionPlan("Plan", "Description", -10m, 15, 5, 100, PlanType.Basic, _tenantId));
        }

        [Fact]
        public void Constructor_WithNegativeTrialDays_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new SubscriptionPlan("Plan", "Description", 99.90m, -5, 5, 100, PlanType.Basic, _tenantId));
        }

        [Fact]
        public void Constructor_WithZeroMaxUsers_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new SubscriptionPlan("Plan", "Description", 99.90m, 15, 0, 100, PlanType.Basic, _tenantId));
        }

        [Fact]
        public void Constructor_WithZeroMaxPatients_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new SubscriptionPlan("Plan", "Description", 99.90m, 15, 5, 0, PlanType.Basic, _tenantId));
        }

        [Fact]
        public void Update_WithValidData_UpdatesSubscriptionPlan()
        {
            // Arrange
            var plan = CreateValidPlan();
            var newName = "Updated Plan";
            var newPrice = 149.90m;
            var newMaxUsers = 10;

            // Act
            plan.Update(newName, "New description", newPrice, newMaxUsers, 200,
                true, true, false, false);

            // Assert
            Assert.Equal(newName, plan.Name);
            Assert.Equal(newPrice, plan.MonthlyPrice);
            Assert.Equal(newMaxUsers, plan.MaxUsers);
            Assert.True(plan.HasReports);
            Assert.True(plan.HasWhatsAppIntegration);
            Assert.NotNull(plan.UpdatedAt);
        }

        [Fact]
        public void Update_WithNegativePrice_ThrowsArgumentException()
        {
            // Arrange
            var plan = CreateValidPlan();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                plan.Update("Plan", "Desc", -10m, 5, 100, false, false, false, false));
        }

        [Fact]
        public void Activate_SetsIsActiveToTrue()
        {
            // Arrange
            var plan = CreateValidPlan();
            plan.Deactivate();

            // Act
            plan.Activate();

            // Assert
            Assert.True(plan.IsActive);
            Assert.NotNull(plan.UpdatedAt);
        }

        [Fact]
        public void Deactivate_SetsIsActiveToFalse()
        {
            // Arrange
            var plan = CreateValidPlan();

            // Act
            plan.Deactivate();

            // Assert
            Assert.False(plan.IsActive);
            Assert.NotNull(plan.UpdatedAt);
        }

        [Fact]
        public void PlanType_HasAllExpectedValues()
        {
            // Assert
            Assert.True(Enum.IsDefined(typeof(PlanType), PlanType.Trial));
            Assert.True(Enum.IsDefined(typeof(PlanType), PlanType.Basic));
            Assert.True(Enum.IsDefined(typeof(PlanType), PlanType.Standard));
            Assert.True(Enum.IsDefined(typeof(PlanType), PlanType.Premium));
            Assert.True(Enum.IsDefined(typeof(PlanType), PlanType.Enterprise));
        }

        [Fact]
        public void Constructor_With15DaysTrial_CreatesCorrectPlan()
        {
            // Arrange & Act
            var plan = new SubscriptionPlan("Starter", "Starter plan with 15 days trial",
                79.90m, 15, 3, 50, PlanType.Trial, _tenantId);

            // Assert
            Assert.Equal(15, plan.TrialDays);
            Assert.Equal(PlanType.Trial, plan.Type);
        }

        private SubscriptionPlan CreateValidPlan()
        {
            return new SubscriptionPlan("Basic Plan", "Description", 99.90m,
                15, 5, 100, PlanType.Basic, _tenantId);
        }
    }
}
