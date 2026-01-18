using System;
using FluentAssertions;
using MedicSoft.Domain.Entities;
using Xunit;

namespace MedicSoft.Test.Entities
{
    public class TissGuideProcedureTests
    {
        private const string TenantId = "test-tenant";
        private readonly Guid _tissGuideId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidData_CreatesTissGuideProcedure()
        {
            // Arrange
            var procedureCode = "40101012";
            var description = "Consulta médica";
            var quantity = 2;
            var unitPrice = 150.00m;

            // Act
            var procedure = new TissGuideProcedure(
                _tissGuideId, procedureCode, description, quantity, unitPrice, TenantId);

            // Assert
            procedure.Id.Should().NotBeEmpty();
            procedure.TissGuideId.Should().Be(_tissGuideId);
            procedure.ProcedureCode.Should().Be(procedureCode);
            procedure.ProcedureDescription.Should().Be(description);
            procedure.Quantity.Should().Be(quantity);
            procedure.UnitPrice.Should().Be(unitPrice);
            procedure.TotalPrice.Should().Be(quantity * unitPrice);
            procedure.ApprovedQuantity.Should().BeNull();
            procedure.ApprovedAmount.Should().BeNull();
            procedure.GlosedAmount.Should().BeNull();
            procedure.GlossReason.Should().BeNull();
        }

        [Fact]
        public void Constructor_WithEmptyTissGuideId_ThrowsArgumentException()
        {
            // Act
            var act = () => new TissGuideProcedure(
                Guid.Empty, "40101012", "Consulta", 1, 150m, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*TISS Guide ID*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidProcedureCode_ThrowsArgumentException(string? procedureCode)
        {
            // Act
            var act = () => new TissGuideProcedure(
                _tissGuideId, procedureCode!, "Consulta", 1, 150m, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Procedure code*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidDescription_ThrowsArgumentException(string? description)
        {
            // Act
            var act = () => new TissGuideProcedure(
                _tissGuideId, "40101012", description!, 1, 150m, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Procedure description*");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-10)]
        public void Constructor_WithInvalidQuantity_ThrowsArgumentException(int quantity)
        {
            // Act
            var act = () => new TissGuideProcedure(
                _tissGuideId, "40101012", "Consulta", quantity, 150m, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Quantity*");
        }

        [Fact]
        public void Constructor_WithNegativeUnitPrice_ThrowsArgumentException()
        {
            // Act
            var act = () => new TissGuideProcedure(
                _tissGuideId, "40101012", "Consulta", 1, -150m, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Unit price*");
        }

        [Fact]
        public void Constructor_CalculatesTotalPrice_Correctly()
        {
            // Arrange
            var quantity = 5;
            var unitPrice = 123.45m;
            var expectedTotal = quantity * unitPrice;

            // Act
            var procedure = new TissGuideProcedure(
                _tissGuideId, "40101012", "Consulta", quantity, unitPrice, TenantId);

            // Assert
            procedure.TotalPrice.Should().Be(expectedTotal);
        }

        [Fact]
        public void UpdateQuantity_WithValidQuantity_UpdatesQuantityAndRecalculatesTotal()
        {
            // Arrange
            var procedure = CreateValidProcedure();
            var originalUnitPrice = procedure.UnitPrice;
            var newQuantity = 5;

            // Act
            procedure.UpdateQuantity(newQuantity);

            // Assert
            procedure.Quantity.Should().Be(newQuantity);
            procedure.TotalPrice.Should().Be(newQuantity * originalUnitPrice);
            procedure.UpdatedAt.Should().NotBeNull();
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public void UpdateQuantity_WithInvalidQuantity_ThrowsArgumentException(int quantity)
        {
            // Arrange
            var procedure = CreateValidProcedure();

            // Act
            var act = () => procedure.UpdateQuantity(quantity);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Quantity*");
        }

        [Fact]
        public void UpdateUnitPrice_WithValidPrice_UpdatesPriceAndRecalculatesTotal()
        {
            // Arrange
            var procedure = CreateValidProcedure();
            var originalQuantity = procedure.Quantity;
            var newUnitPrice = 200.00m;

            // Act
            procedure.UpdateUnitPrice(newUnitPrice);

            // Assert
            procedure.UnitPrice.Should().Be(newUnitPrice);
            procedure.TotalPrice.Should().Be(originalQuantity * newUnitPrice);
            procedure.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void UpdateUnitPrice_WithNegativePrice_ThrowsArgumentException()
        {
            // Arrange
            var procedure = CreateValidProcedure();

            // Act
            var act = () => procedure.UpdateUnitPrice(-100m);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Unit price*");
        }

        [Fact]
        public void ProcessOperatorResponse_WithFullApproval_SetsApprovedValues()
        {
            // Arrange
            var procedure = CreateValidProcedure();
            var approvedQuantity = procedure.Quantity;
            var approvedAmount = procedure.TotalPrice;

            // Act
            procedure.ProcessOperatorResponse(approvedQuantity, approvedAmount);

            // Assert
            procedure.ApprovedQuantity.Should().Be(approvedQuantity);
            procedure.ApprovedAmount.Should().Be(approvedAmount);
            procedure.GlosedAmount.Should().BeNull();
            procedure.GlossReason.Should().BeNull();
            procedure.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void ProcessOperatorResponse_WithPartialApproval_SetsGlosedValues()
        {
            // Arrange
            var procedure = CreateValidProcedure();
            var totalPrice = procedure.TotalPrice;
            var approvedAmount = totalPrice * 0.8m;
            var expectedGloss = totalPrice - approvedAmount;
            var glossReason = "Glosa parcial por falta de documentação";

            // Act
            procedure.ProcessOperatorResponse(1, approvedAmount, glossReason);

            // Assert
            procedure.ApprovedAmount.Should().Be(approvedAmount);
            procedure.GlosedAmount.Should().Be(expectedGloss);
            procedure.GlossReason.Should().Be(glossReason);
        }

        [Fact]
        public void ProcessOperatorResponse_WithFullRejection_SetsZeroApproval()
        {
            // Arrange
            var procedure = CreateValidProcedure();
            var glossReason = "Procedimento não autorizado";

            // Act
            procedure.ProcessOperatorResponse(0, 0m, glossReason);

            // Assert
            procedure.ApprovedQuantity.Should().Be(0);
            procedure.ApprovedAmount.Should().Be(0m);
            procedure.GlosedAmount.Should().Be(procedure.TotalPrice);
            procedure.GlossReason.Should().Be(glossReason);
        }

        [Fact]
        public void IsFullyApproved_WithFullApproval_ReturnsTrue()
        {
            // Arrange
            var procedure = CreateValidProcedure();
            procedure.ProcessOperatorResponse(procedure.Quantity, procedure.TotalPrice);

            // Act
            var result = procedure.IsFullyApproved();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsFullyApproved_WithPartialApproval_ReturnsFalse()
        {
            // Arrange
            var procedure = CreateValidProcedure();
            procedure.ProcessOperatorResponse(1, procedure.TotalPrice * 0.8m);

            // Act
            var result = procedure.IsFullyApproved();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsFullyApproved_WithoutProcessing_ReturnsFalse()
        {
            // Arrange
            var procedure = CreateValidProcedure();

            // Act
            var result = procedure.IsFullyApproved();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsPartiallyApproved_WithPartialApproval_ReturnsTrue()
        {
            // Arrange
            var procedure = CreateValidProcedure();
            procedure.ProcessOperatorResponse(1, procedure.TotalPrice * 0.5m);

            // Act
            var result = procedure.IsPartiallyApproved();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsPartiallyApproved_WithFullApproval_ReturnsFalse()
        {
            // Arrange
            var procedure = CreateValidProcedure();
            procedure.ProcessOperatorResponse(procedure.Quantity, procedure.TotalPrice);

            // Act
            var result = procedure.IsPartiallyApproved();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsPartiallyApproved_WithFullRejection_ReturnsFalse()
        {
            // Arrange
            var procedure = CreateValidProcedure();
            procedure.ProcessOperatorResponse(0, 0m);

            // Act
            var result = procedure.IsPartiallyApproved();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void IsRejected_WithFullRejection_ReturnsTrue()
        {
            // Arrange
            var procedure = CreateValidProcedure();
            procedure.ProcessOperatorResponse(0, 0m, "Rejeitado");

            // Act
            var result = procedure.IsRejected();

            // Assert
            result.Should().BeTrue();
        }

        [Fact]
        public void IsRejected_WithPartialApproval_ReturnsFalse()
        {
            // Arrange
            var procedure = CreateValidProcedure();
            procedure.ProcessOperatorResponse(1, procedure.TotalPrice * 0.5m);

            // Act
            var result = procedure.IsRejected();

            // Assert
            result.Should().BeFalse();
        }

        [Fact]
        public void Constructor_TrimsWhitespace_FromStringProperties()
        {
            // Arrange & Act
            var procedure = new TissGuideProcedure(
                _tissGuideId,
                "  40101012  ",
                "  Consulta médica  ",
                1,
                150m,
                TenantId
            );

            // Assert
            procedure.ProcedureCode.Should().Be("40101012");
            procedure.ProcedureDescription.Should().Be("Consulta médica");
        }

        private TissGuideProcedure CreateValidProcedure()
        {
            return new TissGuideProcedure(
                _tissGuideId,
                "40101012",
                "Consulta médica em consultório",
                2,
                150.00m,
                TenantId
            );
        }
    }
}
