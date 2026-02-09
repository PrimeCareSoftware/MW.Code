using System;
using FluentAssertions;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using Xunit;

namespace MedicSoft.Test.Entities
{
    public class ClinicPricingConfigurationTests
    {
        private const string TenantId = "test-tenant";
        private readonly Guid _clinicId = Guid.NewGuid();

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidData_CreatesInstance()
        {
            // Arrange & Act
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId
            );

            // Assert
            config.Should().NotBeNull();
            config.ClinicId.Should().Be(_clinicId);
            config.DefaultConsultationPrice.Should().Be(150.00m);
            config.DefaultProcedurePolicy.Should().Be(ProcedureConsultationPolicy.ChargeConsultation);
            config.TenantId.Should().Be(TenantId);
        }

        [Fact]
        public void Constructor_WithAllPrices_CreatesInstance()
        {
            // Arrange & Act
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId,
                defaultProcedurePolicy: ProcedureConsultationPolicy.DiscountOnConsultation,
                followUpConsultationPrice: 80.00m,
                telemedicineConsultationPrice: 120.00m
            );

            // Assert
            config.FollowUpConsultationPrice.Should().Be(80.00m);
            config.TelemedicineConsultationPrice.Should().Be(120.00m);
            config.DefaultProcedurePolicy.Should().Be(ProcedureConsultationPolicy.DiscountOnConsultation);
        }

        [Fact]
        public void Constructor_WithEmptyClinicId_ThrowsException()
        {
            // Arrange & Act
            Action act = () => new ClinicPricingConfiguration(
                clinicId: Guid.Empty,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId
            );

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Clinic ID cannot be empty*");
        }

        [Fact]
        public void Constructor_WithNegativeDefaultPrice_ThrowsException()
        {
            // Arrange & Act
            Action act = () => new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: -10.00m,
                tenantId: TenantId
            );

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Default consultation price cannot be negative*");
        }

        [Fact]
        public void Constructor_WithNegativeFollowUpPrice_ThrowsException()
        {
            // Arrange & Act
            Action act = () => new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId,
                followUpConsultationPrice: -10.00m
            );

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Follow-up consultation price cannot be negative*");
        }

        [Fact]
        public void Constructor_WithNegativeTelemedicinePrice_ThrowsException()
        {
            // Arrange & Act
            Action act = () => new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId,
                telemedicineConsultationPrice: -10.00m
            );

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Telemedicine consultation price cannot be negative*");
        }

        #endregion

        #region UpdateConsultationPrices Tests

        [Fact]
        public void UpdateConsultationPrices_WithValidData_UpdatesPrices()
        {
            // Arrange
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId
            );

            // Act
            config.UpdateConsultationPrices(
                defaultConsultationPrice: 200.00m,
                followUpConsultationPrice: 100.00m,
                telemedicineConsultationPrice: 150.00m
            );

            // Assert
            config.DefaultConsultationPrice.Should().Be(200.00m);
            config.FollowUpConsultationPrice.Should().Be(100.00m);
            config.TelemedicineConsultationPrice.Should().Be(150.00m);
        }

        [Fact]
        public void UpdateConsultationPrices_WithNegativePrice_ThrowsException()
        {
            // Arrange
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId
            );

            // Act
            Action act = () => config.UpdateConsultationPrices(-10.00m);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Default consultation price cannot be negative*");
        }

        #endregion

        #region UpdateProcedurePolicy Tests

        [Fact]
        public void UpdateProcedurePolicy_WithChargeConsultation_UpdatesPolicy()
        {
            // Arrange
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId
            );

            // Act
            config.UpdateProcedurePolicy(ProcedureConsultationPolicy.ChargeConsultation);

            // Assert
            config.DefaultProcedurePolicy.Should().Be(ProcedureConsultationPolicy.ChargeConsultation);
        }

        [Fact]
        public void UpdateProcedurePolicy_WithNoCharge_UpdatesPolicy()
        {
            // Arrange
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId
            );

            // Act
            config.UpdateProcedurePolicy(ProcedureConsultationPolicy.NoCharge);

            // Assert
            config.DefaultProcedurePolicy.Should().Be(ProcedureConsultationPolicy.NoCharge);
        }

        [Fact]
        public void UpdateProcedurePolicy_WithDiscountPercentage_UpdatesPolicy()
        {
            // Arrange
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId
            );

            // Act
            config.UpdateProcedurePolicy(
                ProcedureConsultationPolicy.DiscountOnConsultation,
                discountPercentage: 50
            );

            // Assert
            config.DefaultProcedurePolicy.Should().Be(ProcedureConsultationPolicy.DiscountOnConsultation);
            config.ConsultationDiscountPercentage.Should().Be(50);
        }

        [Fact]
        public void UpdateProcedurePolicy_WithDiscountFixedAmount_UpdatesPolicy()
        {
            // Arrange
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId
            );

            // Act
            config.UpdateProcedurePolicy(
                ProcedureConsultationPolicy.DiscountOnConsultation,
                discountFixedAmount: 50.00m
            );

            // Assert
            config.DefaultProcedurePolicy.Should().Be(ProcedureConsultationPolicy.DiscountOnConsultation);
            config.ConsultationDiscountFixedAmount.Should().Be(50.00m);
        }

        [Fact]
        public void UpdateProcedurePolicy_WithDiscountPolicyButNoDiscount_ThrowsException()
        {
            // Arrange
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId
            );

            // Act
            Action act = () => config.UpdateProcedurePolicy(
                ProcedureConsultationPolicy.DiscountOnConsultation
            );

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Discount percentage or fixed amount is required*");
        }

        [Fact]
        public void UpdateProcedurePolicy_WithInvalidDiscountPercentage_ThrowsException()
        {
            // Arrange
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId
            );

            // Act
            Action act = () => config.UpdateProcedurePolicy(
                ProcedureConsultationPolicy.DiscountOnConsultation,
                discountPercentage: 150
            );

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Discount percentage must be between 0 and 100*");
        }

        [Fact]
        public void UpdateProcedurePolicy_WithNegativeDiscountAmount_ThrowsException()
        {
            // Arrange
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId
            );

            // Act
            Action act = () => config.UpdateProcedurePolicy(
                ProcedureConsultationPolicy.DiscountOnConsultation,
                discountFixedAmount: -10.00m
            );

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Discount fixed amount cannot be negative*");
        }

        #endregion

        #region GetConsultationPrice Tests

        [Fact]
        public void GetConsultationPrice_ForRegularInPerson_ReturnsDefaultPrice()
        {
            // Arrange
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId,
                followUpConsultationPrice: 80.00m,
                telemedicineConsultationPrice: 120.00m
            );

            // Act
            var price = config.GetConsultationPrice(AppointmentType.Regular, AppointmentMode.InPerson);

            // Assert
            price.Should().Be(150.00m);
        }

        [Fact]
        public void GetConsultationPrice_ForFollowUp_ReturnsFollowUpPrice()
        {
            // Arrange
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId,
                followUpConsultationPrice: 80.00m,
                telemedicineConsultationPrice: 120.00m
            );

            // Act
            var price = config.GetConsultationPrice(AppointmentType.FollowUp, AppointmentMode.InPerson);

            // Assert
            price.Should().Be(80.00m);
        }

        [Fact]
        public void GetConsultationPrice_ForTelemedicine_ReturnsTelemedicinePrice()
        {
            // Arrange
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId,
                followUpConsultationPrice: 80.00m,
                telemedicineConsultationPrice: 120.00m
            );

            // Act
            var price = config.GetConsultationPrice(AppointmentType.Regular, AppointmentMode.Telemedicine);

            // Assert
            price.Should().Be(120.00m);
        }

        [Fact]
        public void GetConsultationPrice_ForFollowUpWithoutSpecificPrice_ReturnsDefaultPrice()
        {
            // Arrange
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId
            );

            // Act
            var price = config.GetConsultationPrice(AppointmentType.FollowUp, AppointmentMode.InPerson);

            // Assert
            price.Should().Be(150.00m);
        }

        [Fact]
        public void GetConsultationPrice_ForTelemedicineWithoutSpecificPrice_ReturnsDefaultPrice()
        {
            // Arrange
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId
            );

            // Act
            var price = config.GetConsultationPrice(AppointmentType.Regular, AppointmentMode.Telemedicine);

            // Assert
            price.Should().Be(150.00m);
        }

        #endregion

        #region GetConsultationPriceWithProcedure Tests

        [Fact]
        public void GetConsultationPriceWithProcedure_WithChargePolicy_ReturnsFullPrice()
        {
            // Arrange
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId,
                defaultProcedurePolicy: ProcedureConsultationPolicy.ChargeConsultation
            );

            // Act
            var price = config.GetConsultationPriceWithProcedure(150.00m);

            // Assert
            price.Should().Be(150.00m);
        }

        [Fact]
        public void GetConsultationPriceWithProcedure_WithNoChargePolicy_ReturnsZero()
        {
            // Arrange
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId,
                defaultProcedurePolicy: ProcedureConsultationPolicy.NoCharge
            );

            // Act
            var price = config.GetConsultationPriceWithProcedure(150.00m);

            // Assert
            price.Should().Be(0);
        }

        [Fact]
        public void GetConsultationPriceWithProcedure_WithPercentageDiscount_ReturnsDiscountedPrice()
        {
            // Arrange
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId,
                defaultProcedurePolicy: ProcedureConsultationPolicy.DiscountOnConsultation
            );
            config.UpdateProcedurePolicy(
                ProcedureConsultationPolicy.DiscountOnConsultation,
                discountPercentage: 50
            );

            // Act
            var price = config.GetConsultationPriceWithProcedure(150.00m);

            // Assert
            price.Should().Be(75.00m);
        }

        [Fact]
        public void GetConsultationPriceWithProcedure_WithFixedDiscount_ReturnsDiscountedPrice()
        {
            // Arrange
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId,
                defaultProcedurePolicy: ProcedureConsultationPolicy.DiscountOnConsultation
            );
            config.UpdateProcedurePolicy(
                ProcedureConsultationPolicy.DiscountOnConsultation,
                discountFixedAmount: 50.00m
            );

            // Act
            var price = config.GetConsultationPriceWithProcedure(150.00m);

            // Assert
            price.Should().Be(100.00m);
        }

        [Fact]
        public void GetConsultationPriceWithProcedure_WithDiscountExceedingPrice_ReturnsZero()
        {
            // Arrange
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId,
                defaultProcedurePolicy: ProcedureConsultationPolicy.DiscountOnConsultation
            );
            config.UpdateProcedurePolicy(
                ProcedureConsultationPolicy.DiscountOnConsultation,
                discountFixedAmount: 200.00m
            );

            // Act
            var price = config.GetConsultationPriceWithProcedure(150.00m);

            // Assert
            price.Should().Be(0);
        }

        [Fact]
        public void GetConsultationPriceWithProcedure_WithBothDiscounts_PrioritizesFixedAmount()
        {
            // Arrange
            var config = new ClinicPricingConfiguration(
                clinicId: _clinicId,
                defaultConsultationPrice: 150.00m,
                tenantId: TenantId,
                defaultProcedurePolicy: ProcedureConsultationPolicy.DiscountOnConsultation
            );
            config.UpdateProcedurePolicy(
                ProcedureConsultationPolicy.DiscountOnConsultation,
                discountPercentage: 50,
                discountFixedAmount: 30.00m
            );

            // Act
            var price = config.GetConsultationPriceWithProcedure(150.00m);

            // Assert
            price.Should().Be(120.00m); // Fixed amount takes precedence
        }

        #endregion
    }
}
