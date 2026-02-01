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
            var type = SubscriptionPlanType.Basic;

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
                15, 20, 1000, SubscriptionPlanType.Premium, _tenantId,
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
                new SubscriptionPlan("", "Description", 99.90m, 15, 5, 100, SubscriptionPlanType.Basic, _tenantId));
        }

        [Fact]
        public void Constructor_WithNegativePrice_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new SubscriptionPlan("Plan", "Description", -10m, 15, 5, 100, SubscriptionPlanType.Basic, _tenantId));
        }

        [Fact]
        public void Constructor_WithNegativeTrialDays_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new SubscriptionPlan("Plan", "Description", 99.90m, -5, 5, 100, SubscriptionPlanType.Basic, _tenantId));
        }

        [Fact]
        public void Constructor_WithZeroMaxUsers_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new SubscriptionPlan("Plan", "Description", 99.90m, 15, 0, 100, SubscriptionPlanType.Basic, _tenantId));
        }

        [Fact]
        public void Constructor_WithZeroMaxPatients_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new SubscriptionPlan("Plan", "Description", 99.90m, 15, 5, 0, SubscriptionPlanType.Basic, _tenantId));
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
            Assert.True(Enum.IsDefined(typeof(SubscriptionPlanType), SubscriptionPlanType.Trial));
            Assert.True(Enum.IsDefined(typeof(SubscriptionPlanType), SubscriptionPlanType.Basic));
            Assert.True(Enum.IsDefined(typeof(SubscriptionPlanType), SubscriptionPlanType.Standard));
            Assert.True(Enum.IsDefined(typeof(SubscriptionPlanType), SubscriptionPlanType.Premium));
            Assert.True(Enum.IsDefined(typeof(SubscriptionPlanType), SubscriptionPlanType.Enterprise));
        }

        [Fact]
        public void Constructor_With15DaysTrial_CreatesCorrectPlan()
        {
            // Arrange & Act
            var plan = new SubscriptionPlan("Starter", "Starter plan with 15 days trial",
                79.90m, 15, 3, 50, SubscriptionPlanType.Trial, _tenantId);

            // Assert
            Assert.Equal(15, plan.TrialDays);
            Assert.Equal(SubscriptionPlanType.Trial, plan.Type);
        }

        private SubscriptionPlan CreateValidPlan()
        {
            return new SubscriptionPlan("Basic Plan", "Description", 99.90m,
                15, 5, 100, SubscriptionPlanType.Basic, _tenantId);
        }

        #region Campaign Tests

        [Fact]
        public void SetCampaignPricing_WithValidData_SetsCampaign()
        {
            // Arrange
            var plan = CreateValidPlan();
            var originalPrice = 199.90m;
            var campaignPrice = 99.90m;

            // Act
            plan.SetCampaignPricing("Early Adopter", "Special launch pricing", 
                originalPrice, campaignPrice, DateTime.UtcNow, null, 100);

            // Assert
            Assert.Equal("Early Adopter", plan.CampaignName);
            Assert.Equal(originalPrice, plan.OriginalPrice);
            Assert.Equal(campaignPrice, plan.CampaignPrice);
            Assert.Equal(100, plan.MaxEarlyAdopters);
            Assert.Equal(0, plan.CurrentEarlyAdopters);
            Assert.True(plan.IsCampaignActive());
        }

        [Fact]
        public void IsCampaignActive_WhenSlotsAreFull_ReturnsFalse()
        {
            // Arrange
            var plan = CreateValidPlan();
            plan.SetCampaignPricing("Early Adopter", "Limited slots", 
                199.90m, 99.90m, DateTime.UtcNow, null, 2);

            // Act - Fill all slots
            plan.IncrementEarlyAdopters();
            plan.IncrementEarlyAdopters();

            // Assert
            Assert.Equal(2, plan.CurrentEarlyAdopters);
            Assert.False(plan.IsCampaignActive());
            Assert.False(plan.CanJoinCampaign());
        }

        [Fact]
        public void IncrementEarlyAdopters_WhenCampaignIsFull_ThrowsInvalidOperationException()
        {
            // Arrange
            var plan = CreateValidPlan();
            plan.SetCampaignPricing("Early Adopter", "Limited slots", 
                199.90m, 99.90m, DateTime.UtcNow, null, 1);
            plan.IncrementEarlyAdopters(); // Fill the only slot

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => 
                plan.IncrementEarlyAdopters());
            Assert.Contains("campaign", exception.Message.ToLower());
        }

        [Fact]
        public void CanJoinCampaign_WithAvailableSlots_ReturnsTrue()
        {
            // Arrange
            var plan = CreateValidPlan();
            plan.SetCampaignPricing("Early Adopter", "Limited slots", 
                199.90m, 99.90m, DateTime.UtcNow, null, 100);
            plan.IncrementEarlyAdopters(); // Use 1 out of 100 slots

            // Act & Assert
            Assert.True(plan.CanJoinCampaign());
            Assert.Equal(1, plan.CurrentEarlyAdopters);
            Assert.Equal(100, plan.MaxEarlyAdopters);
        }

        [Fact]
        public void IsCampaignActive_WhenExpired_ReturnsFalse()
        {
            // Arrange
            var plan = CreateValidPlan();
            var startDate = DateTime.UtcNow.AddDays(-10);
            var endDate = DateTime.UtcNow.AddDays(-1); // Expired yesterday
            plan.SetCampaignPricing("Early Adopter", "Expired campaign", 
                199.90m, 99.90m, startDate, endDate, 100);

            // Act & Assert
            Assert.False(plan.IsCampaignActive());
        }

        [Fact]
        public void IsCampaignActive_WhenNotStarted_ReturnsFalse()
        {
            // Arrange
            var plan = CreateValidPlan();
            var startDate = DateTime.UtcNow.AddDays(1); // Starts tomorrow
            var endDate = DateTime.UtcNow.AddDays(30);
            plan.SetCampaignPricing("Early Adopter", "Future campaign", 
                199.90m, 99.90m, startDate, endDate, 100);

            // Act & Assert
            Assert.False(plan.IsCampaignActive());
        }

        [Fact]
        public void GetEffectivePrice_WhenCampaignActive_ReturnsCampaignPrice()
        {
            // Arrange
            var plan = CreateValidPlan();
            plan.SetCampaignPricing("Early Adopter", "Active campaign", 
                199.90m, 49.90m, DateTime.UtcNow, null, 100);

            // Act
            var effectivePrice = plan.GetEffectivePrice();

            // Assert
            Assert.Equal(49.90m, effectivePrice);
        }

        [Fact]
        public void GetEffectivePrice_WhenCampaignInactive_ReturnsRegularPrice()
        {
            // Arrange
            var plan = CreateValidPlan();
            var regularPrice = 99.90m;
            // Set an expired campaign
            plan.SetCampaignPricing("Early Adopter", "Expired campaign", 
                199.90m, 49.90m, DateTime.UtcNow.AddDays(-10), DateTime.UtcNow.AddDays(-1), 100);

            // Act
            var effectivePrice = plan.GetEffectivePrice();

            // Assert
            Assert.Equal(regularPrice, effectivePrice); // Should return the MonthlyPrice
        }

        [Fact]
        public void GetSavingsPercentage_WhenCampaignActive_ReturnsCorrectPercentage()
        {
            // Arrange
            var plan = CreateValidPlan();
            plan.SetCampaignPricing("Early Adopter", "67% off", 
                149.90m, 49.90m, DateTime.UtcNow, null, 100);

            // Act
            var savings = plan.GetSavingsPercentage();

            // Assert - 100 off 150 is approximately 67%
            Assert.InRange(savings, 66, 67);
        }

        [Fact]
        public void SetCampaignPricing_WhenCampaignPriceHigherThanOriginal_ThrowsArgumentException()
        {
            // Arrange
            var plan = CreateValidPlan();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => 
                plan.SetCampaignPricing("Invalid", "Invalid pricing", 
                    99.90m, 199.90m, DateTime.UtcNow, null, 100));
        }

        [Fact]
        public void ClearCampaignPricing_RestoresOriginalPrice()
        {
            // Arrange
            var plan = CreateValidPlan();
            var originalMonthlyPrice = plan.MonthlyPrice;
            plan.SetCampaignPricing("Early Adopter", "Campaign", 
                199.90m, 49.90m, DateTime.UtcNow, null, 100);
            plan.IncrementEarlyAdopters();

            // Act
            plan.ClearCampaignPricing();

            // Assert
            Assert.Null(plan.CampaignName);
            Assert.Null(plan.CampaignPrice);
            Assert.Null(plan.OriginalPrice);
            Assert.Equal(0, plan.CurrentEarlyAdopters);
            Assert.False(plan.IsCampaignActive());
        }

        #endregion
    }
}
