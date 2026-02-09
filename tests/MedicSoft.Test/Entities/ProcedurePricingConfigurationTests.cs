using System;
using FluentAssertions;
using MedicSoft.Domain.Entities;
using Xunit;

namespace MedicSoft.Test.Entities
{
    public class ProcedurePricingConfigurationTests
    {
        private const string TenantId = "test-tenant";
        private readonly Guid _procedureId = Guid.NewGuid();
        private readonly Guid _clinicId = Guid.NewGuid();

        #region Constructor Tests

        [Fact]
        public void Constructor_WithValidData_CreatesInstance()
        {
            // Arrange & Act
            var config = new ProcedurePricingConfiguration(
                procedureId: _procedureId,
                clinicId: _clinicId,
                tenantId: TenantId
            );

            // Assert
            config.Should().NotBeNull();
            config.ProcedureId.Should().Be(_procedureId);
            config.ClinicId.Should().Be(_clinicId);
            config.TenantId.Should().Be(TenantId);
            config.ConsultationPolicy.Should().BeNull();
            config.CustomPrice.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithCustomPrice_CreatesInstance()
        {
            // Arrange & Act
            var config = new ProcedurePricingConfiguration(
                procedureId: _procedureId,
                clinicId: _clinicId,
                tenantId: TenantId,
                customPrice: 250.00m
            );

            // Assert
            config.CustomPrice.Should().Be(250.00m);
        }

        [Fact]
        public void Constructor_WithConsultationPolicy_CreatesInstance()
        {
            // Arrange & Act
            var config = new ProcedurePricingConfiguration(
                procedureId: _procedureId,
                clinicId: _clinicId,
                tenantId: TenantId,
                consultationPolicy: ProcedureConsultationPolicy.NoCharge
            );

            // Assert
            config.ConsultationPolicy.Should().Be(ProcedureConsultationPolicy.NoCharge);
        }

        [Fact]
        public void Constructor_WithEmptyProcedureId_ThrowsException()
        {
            // Arrange & Act
            Action act = () => new ProcedurePricingConfiguration(
                procedureId: Guid.Empty,
                clinicId: _clinicId,
                tenantId: TenantId
            );

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Procedure ID cannot be empty*");
        }

        [Fact]
        public void Constructor_WithEmptyClinicId_ThrowsException()
        {
            // Arrange & Act
            Action act = () => new ProcedurePricingConfiguration(
                procedureId: _procedureId,
                clinicId: Guid.Empty,
                tenantId: TenantId
            );

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Clinic ID cannot be empty*");
        }

        [Fact]
        public void Constructor_WithNegativeCustomPrice_ThrowsException()
        {
            // Arrange & Act
            Action act = () => new ProcedurePricingConfiguration(
                procedureId: _procedureId,
                clinicId: _clinicId,
                tenantId: TenantId,
                customPrice: -10.00m
            );

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Custom price cannot be negative*");
        }

        #endregion

        #region UpdateConsultationPolicy Tests

        [Fact]
        public void UpdateConsultationPolicy_WithChargeConsultation_UpdatesPolicy()
        {
            // Arrange
            var config = new ProcedurePricingConfiguration(
                procedureId: _procedureId,
                clinicId: _clinicId,
                tenantId: TenantId
            );

            // Act
            config.UpdateConsultationPolicy(ProcedureConsultationPolicy.ChargeConsultation);

            // Assert
            config.ConsultationPolicy.Should().Be(ProcedureConsultationPolicy.ChargeConsultation);
        }

        [Fact]
        public void UpdateConsultationPolicy_WithNoCharge_UpdatesPolicy()
        {
            // Arrange
            var config = new ProcedurePricingConfiguration(
                procedureId: _procedureId,
                clinicId: _clinicId,
                tenantId: TenantId
            );

            // Act
            config.UpdateConsultationPolicy(ProcedureConsultationPolicy.NoCharge);

            // Assert
            config.ConsultationPolicy.Should().Be(ProcedureConsultationPolicy.NoCharge);
        }

        [Fact]
        public void UpdateConsultationPolicy_WithDiscountPercentage_UpdatesPolicy()
        {
            // Arrange
            var config = new ProcedurePricingConfiguration(
                procedureId: _procedureId,
                clinicId: _clinicId,
                tenantId: TenantId
            );

            // Act
            config.UpdateConsultationPolicy(
                ProcedureConsultationPolicy.DiscountOnConsultation,
                discountPercentage: 30
            );

            // Assert
            config.ConsultationPolicy.Should().Be(ProcedureConsultationPolicy.DiscountOnConsultation);
            config.ConsultationDiscountPercentage.Should().Be(30);
        }

        [Fact]
        public void UpdateConsultationPolicy_WithDiscountFixedAmount_UpdatesPolicy()
        {
            // Arrange
            var config = new ProcedurePricingConfiguration(
                procedureId: _procedureId,
                clinicId: _clinicId,
                tenantId: TenantId
            );

            // Act
            config.UpdateConsultationPolicy(
                ProcedureConsultationPolicy.DiscountOnConsultation,
                discountFixedAmount: 40.00m
            );

            // Assert
            config.ConsultationPolicy.Should().Be(ProcedureConsultationPolicy.DiscountOnConsultation);
            config.ConsultationDiscountFixedAmount.Should().Be(40.00m);
        }

        [Fact]
        public void UpdateConsultationPolicy_WithDiscountPolicyButNoDiscount_ThrowsException()
        {
            // Arrange
            var config = new ProcedurePricingConfiguration(
                procedureId: _procedureId,
                clinicId: _clinicId,
                tenantId: TenantId
            );

            // Act
            Action act = () => config.UpdateConsultationPolicy(
                ProcedureConsultationPolicy.DiscountOnConsultation
            );

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Discount percentage or fixed amount is required*");
        }

        [Fact]
        public void UpdateConsultationPolicy_WithInvalidDiscountPercentage_ThrowsException()
        {
            // Arrange
            var config = new ProcedurePricingConfiguration(
                procedureId: _procedureId,
                clinicId: _clinicId,
                tenantId: TenantId
            );

            // Act
            Action act = () => config.UpdateConsultationPolicy(
                ProcedureConsultationPolicy.DiscountOnConsultation,
                discountPercentage: 150
            );

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Discount percentage must be between 0 and 100*");
        }

        [Fact]
        public void UpdateConsultationPolicy_WithNegativeDiscountAmount_ThrowsException()
        {
            // Arrange
            var config = new ProcedurePricingConfiguration(
                procedureId: _procedureId,
                clinicId: _clinicId,
                tenantId: TenantId
            );

            // Act
            Action act = () => config.UpdateConsultationPolicy(
                ProcedureConsultationPolicy.DiscountOnConsultation,
                discountFixedAmount: -10.00m
            );

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Discount fixed amount cannot be negative*");
        }

        [Fact]
        public void UpdateConsultationPolicy_ToNull_ClearsPolicy()
        {
            // Arrange
            var config = new ProcedurePricingConfiguration(
                procedureId: _procedureId,
                clinicId: _clinicId,
                tenantId: TenantId,
                consultationPolicy: ProcedureConsultationPolicy.NoCharge
            );

            // Act
            config.UpdateConsultationPolicy(null);

            // Assert
            config.ConsultationPolicy.Should().BeNull();
        }

        #endregion

        #region UpdateCustomPrice Tests

        [Fact]
        public void UpdateCustomPrice_WithValidPrice_UpdatesPrice()
        {
            // Arrange
            var config = new ProcedurePricingConfiguration(
                procedureId: _procedureId,
                clinicId: _clinicId,
                tenantId: TenantId
            );

            // Act
            config.UpdateCustomPrice(300.00m);

            // Assert
            config.CustomPrice.Should().Be(300.00m);
        }

        [Fact]
        public void UpdateCustomPrice_WithNull_ClearsPrice()
        {
            // Arrange
            var config = new ProcedurePricingConfiguration(
                procedureId: _procedureId,
                clinicId: _clinicId,
                tenantId: TenantId,
                customPrice: 250.00m
            );

            // Act
            config.UpdateCustomPrice(null);

            // Assert
            config.CustomPrice.Should().BeNull();
        }

        [Fact]
        public void UpdateCustomPrice_WithNegativePrice_ThrowsException()
        {
            // Arrange
            var config = new ProcedurePricingConfiguration(
                procedureId: _procedureId,
                clinicId: _clinicId,
                tenantId: TenantId
            );

            // Act
            Action act = () => config.UpdateCustomPrice(-10.00m);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Custom price cannot be negative*");
        }

        [Fact]
        public void UpdateCustomPrice_WithZero_UpdatesPrice()
        {
            // Arrange
            var config = new ProcedurePricingConfiguration(
                procedureId: _procedureId,
                clinicId: _clinicId,
                tenantId: TenantId
            );

            // Act
            config.UpdateCustomPrice(0);

            // Assert
            config.CustomPrice.Should().Be(0);
        }

        #endregion

        #region GetEffectivePrice Tests

        [Fact]
        public void GetEffectivePrice_WithCustomPrice_ReturnsCustomPrice()
        {
            // Arrange
            var config = new ProcedurePricingConfiguration(
                procedureId: _procedureId,
                clinicId: _clinicId,
                tenantId: TenantId,
                customPrice: 300.00m
            );

            // Act
            var price = config.GetEffectivePrice(defaultPrice: 200.00m);

            // Assert
            price.Should().Be(300.00m);
        }

        [Fact]
        public void GetEffectivePrice_WithoutCustomPrice_ReturnsDefaultPrice()
        {
            // Arrange
            var config = new ProcedurePricingConfiguration(
                procedureId: _procedureId,
                clinicId: _clinicId,
                tenantId: TenantId
            );

            // Act
            var price = config.GetEffectivePrice(defaultPrice: 200.00m);

            // Assert
            price.Should().Be(200.00m);
        }

        [Fact]
        public void GetEffectivePrice_WithZeroCustomPrice_ReturnsZero()
        {
            // Arrange
            var config = new ProcedurePricingConfiguration(
                procedureId: _procedureId,
                clinicId: _clinicId,
                tenantId: TenantId,
                customPrice: 0
            );

            // Act
            var price = config.GetEffectivePrice(defaultPrice: 200.00m);

            // Assert
            price.Should().Be(0);
        }

        #endregion
    }
}
