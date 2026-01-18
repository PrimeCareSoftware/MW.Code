using System;
using FluentAssertions;
using MedicSoft.Domain.Entities;
using Xunit;

namespace MedicSoft.Test.Entities
{
    public class PatientHealthInsuranceTests
    {
        private const string TenantId = "test-tenant";
        private readonly Guid _patientId = Guid.NewGuid();
        private readonly Guid _planId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidData_CreatesPatientHealthInsurance()
        {
            // Arrange
            var cardNumber = "123456789012";
            var validFrom = DateTime.UtcNow;

            // Act
            var phi = new PatientHealthInsurance(_patientId, _planId, cardNumber, validFrom, TenantId);

            // Assert
            phi.Id.Should().NotBeEmpty();
            phi.PatientId.Should().Be(_patientId);
            phi.HealthInsurancePlanId.Should().Be(_planId);
            phi.CardNumber.Should().Be(cardNumber);
            phi.ValidFrom.Should().Be(validFrom);
            phi.IsActive.Should().BeTrue();
            phi.IsHolder.Should().BeTrue();
            phi.ValidUntil.Should().BeNull();
            phi.CardValidationCode.Should().BeNull();
            phi.HolderName.Should().BeNull();
            phi.HolderDocument.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithDependentData_CreatesPatientHealthInsurance()
        {
            // Arrange
            var cardNumber = "123456789012";
            var validFrom = DateTime.UtcNow;
            var validUntil = DateTime.UtcNow.AddYears(1);
            var holderName = "João Silva";
            var holderDocument = "12345678900";

            // Act
            var phi = new PatientHealthInsurance(
                _patientId, _planId, cardNumber, validFrom, TenantId,
                isHolder: false, cardValidationCode: "123", validUntil: validUntil,
                holderName: holderName, holderDocument: holderDocument);

            // Assert
            phi.IsHolder.Should().BeFalse();
            phi.HolderName.Should().Be(holderName);
            phi.HolderDocument.Should().Be(holderDocument);
            phi.ValidUntil.Should().Be(validUntil);
            phi.CardValidationCode.Should().Be("123");
        }

        [Fact]
        public void Constructor_WithEmptyPatientId_ThrowsArgumentException()
        {
            // Act
            var act = () => new PatientHealthInsurance(Guid.Empty, _planId, "123456", DateTime.UtcNow, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Patient ID*");
        }

        [Fact]
        public void Constructor_WithEmptyPlanId_ThrowsArgumentException()
        {
            // Act
            var act = () => new PatientHealthInsurance(_patientId, Guid.Empty, "123456", DateTime.UtcNow, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Health Insurance Plan ID*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidCardNumber_ThrowsArgumentException(string? cardNumber)
        {
            // Act
            var act = () => new PatientHealthInsurance(_patientId, _planId, cardNumber!, DateTime.UtcNow, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Card number*");
        }

        [Fact]
        public void Constructor_WithValidFromTooFarInFuture_ThrowsArgumentException()
        {
            // Arrange
            var validFrom = DateTime.UtcNow.AddYears(2);

            // Act
            var act = () => new PatientHealthInsurance(_patientId, _planId, "123456", validFrom, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Valid from date*");
        }

        [Fact]
        public void Constructor_WithValidUntilBeforeValidFrom_ThrowsArgumentException()
        {
            // Arrange
            var validFrom = DateTime.UtcNow;
            var validUntil = validFrom.AddDays(-1);

            // Act
            var act = () => new PatientHealthInsurance(_patientId, _planId, "123456", validFrom, TenantId, validUntil: validUntil);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Valid until date*");
        }

        [Fact]
        public void Constructor_WithDependentMissingHolderName_ThrowsArgumentException()
        {
            // Act
            var act = () => new PatientHealthInsurance(
                _patientId, _planId, "123456", DateTime.UtcNow, TenantId,
                isHolder: false, holderDocument: "12345678900");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Holder name*");
        }

        [Fact]
        public void Constructor_WithDependentMissingHolderDocument_ThrowsArgumentException()
        {
            // Act
            var act = () => new PatientHealthInsurance(
                _patientId, _planId, "123456", DateTime.UtcNow, TenantId,
                isHolder: false, holderName: "João Silva");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Holder document*");
        }

        [Fact]
        public void UpdateCardInfo_WithValidData_UpdatesCard()
        {
            // Arrange
            var phi = CreateValidPatientHealthInsurance();
            var newCardNumber = "999888777666";
            var newValidationCode = "456";

            // Act
            phi.UpdateCardInfo(newCardNumber, newValidationCode);

            // Assert
            phi.CardNumber.Should().Be(newCardNumber);
            phi.CardValidationCode.Should().Be(newValidationCode);
            phi.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void UpdateCardInfo_WithEmptyCardNumber_ThrowsArgumentException()
        {
            // Arrange
            var phi = CreateValidPatientHealthInsurance();

            // Act
            var act = () => phi.UpdateCardInfo("", null);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateValidityPeriod_WithValidDates_UpdatesPeriod()
        {
            // Arrange
            var phi = CreateValidPatientHealthInsurance();
            var newValidFrom = DateTime.UtcNow.AddDays(30);
            var newValidUntil = newValidFrom.AddYears(1);

            // Act
            phi.UpdateValidityPeriod(newValidFrom, newValidUntil);

            // Assert
            phi.ValidFrom.Should().Be(newValidFrom);
            phi.ValidUntil.Should().Be(newValidUntil);
            phi.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void UpdateValidityPeriod_WithInvalidDates_ThrowsArgumentException()
        {
            // Arrange
            var phi = CreateValidPatientHealthInsurance();
            var validFrom = DateTime.UtcNow;
            var validUntil = validFrom.AddDays(-1);

            // Act
            var act = () => phi.UpdateValidityPeriod(validFrom, validUntil);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateHolderInfo_ChangingToDependent_UpdatesHolderInfo()
        {
            // Arrange
            var phi = CreateValidPatientHealthInsurance();
            var holderName = "Maria Santos";
            var holderDocument = "98765432100";

            // Act
            phi.UpdateHolderInfo(false, holderName, holderDocument);

            // Assert
            phi.IsHolder.Should().BeFalse();
            phi.HolderName.Should().Be(holderName);
            phi.HolderDocument.Should().Be(holderDocument);
            phi.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void UpdateHolderInfo_ChangingToHolder_ClearsHolderData()
        {
            // Arrange
            var phi = new PatientHealthInsurance(
                _patientId, _planId, "123456", DateTime.UtcNow, TenantId,
                isHolder: false, holderName: "João", holderDocument: "12345678900");

            // Act
            phi.UpdateHolderInfo(true);

            // Assert
            phi.IsHolder.Should().BeTrue();
        }

        [Fact]
        public void UpdateHolderInfo_DependentWithoutName_ThrowsArgumentException()
        {
            // Arrange
            var phi = CreateValidPatientHealthInsurance();

            // Act
            var act = () => phi.UpdateHolderInfo(false, null, "12345678900");

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void Activate_SetsIsActiveToTrue()
        {
            // Arrange
            var phi = CreateValidPatientHealthInsurance();
            phi.Deactivate();

            // Act
            phi.Activate();

            // Assert
            phi.IsActive.Should().BeTrue();
            phi.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Deactivate_SetsIsActiveToFalse()
        {
            // Arrange
            var phi = CreateValidPatientHealthInsurance();

            // Act
            phi.Deactivate();

            // Assert
            phi.IsActive.Should().BeFalse();
            phi.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void IsValid_WithActiveAndValidDates_ReturnsTrue()
        {
            // Arrange
            var validFrom = DateTime.UtcNow.AddDays(-30);
            var validUntil = DateTime.UtcNow.AddDays(30);
            var phi = new PatientHealthInsurance(
                _patientId, _planId, "123456", validFrom, TenantId, validUntil: validUntil);

            // Act
            var isValid = phi.IsValid();

            // Assert
            isValid.Should().BeTrue();
        }

        [Fact]
        public void IsValid_WithInactiveInsurance_ReturnsFalse()
        {
            // Arrange
            var phi = CreateValidPatientHealthInsurance();
            phi.Deactivate();

            // Act
            var isValid = phi.IsValid();

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void IsValid_WithExpiredInsurance_ReturnsFalse()
        {
            // Arrange
            var validFrom = DateTime.UtcNow.AddDays(-60);
            var validUntil = DateTime.UtcNow.AddDays(-1);
            var phi = new PatientHealthInsurance(
                _patientId, _planId, "123456", validFrom, TenantId, validUntil: validUntil);

            // Act
            var isValid = phi.IsValid();

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void IsValid_WithFutureValidFrom_ReturnsFalse()
        {
            // Arrange
            var validFrom = DateTime.UtcNow.AddDays(30);
            var phi = new PatientHealthInsurance(
                _patientId, _planId, "123456", validFrom, TenantId);

            // Act
            var isValid = phi.IsValid();

            // Assert
            isValid.Should().BeFalse();
        }

        [Fact]
        public void IsValid_WithCustomDate_ValidatesCorrectly()
        {
            // Arrange
            var validFrom = DateTime.UtcNow.AddDays(-60);
            var validUntil = DateTime.UtcNow.AddDays(60);
            var phi = new PatientHealthInsurance(
                _patientId, _planId, "123456", validFrom, TenantId, validUntil: validUntil);
            var checkDate = DateTime.UtcNow.AddDays(90); // After expiration

            // Act
            var isValid = phi.IsValid(checkDate);

            // Assert
            isValid.Should().BeFalse();
        }

        private PatientHealthInsurance CreateValidPatientHealthInsurance()
        {
            return new PatientHealthInsurance(
                _patientId,
                _planId,
                "123456789012",
                DateTime.UtcNow.AddDays(-30),
                TenantId
            );
        }
    }
}
