using System;
using FluentAssertions;
using MedicSoft.Domain.Entities;
using Xunit;

namespace MedicSoft.Test.Entities
{
    public class TussProcedureTests
    {
        private const string TenantId = "test-tenant";

        [Fact]
        public void Constructor_WithValidData_CreatesTussProcedure()
        {
            // Arrange
            var code = "40101012";
            var description = "Consulta médica em consultório";
            var category = "Consultas";
            var referencePrice = 150.00m;

            // Act
            var procedure = new TussProcedure(code, description, category, referencePrice, TenantId);

            // Assert
            procedure.Id.Should().NotBeEmpty();
            procedure.Code.Should().Be(code);
            procedure.Description.Should().Be(description);
            procedure.Category.Should().Be(category);
            procedure.ReferencePrice.Should().Be(referencePrice);
            procedure.RequiresAuthorization.Should().BeFalse();
            procedure.IsActive.Should().BeTrue();
            procedure.LastUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void Constructor_WithRequiresAuthorization_CreatesTussProcedure()
        {
            // Arrange
            var requiresAuth = true;

            // Act
            var procedure = new TussProcedure(
                "20101040", "Ressonância magnética", "Exames", 800.00m, TenantId, requiresAuth);

            // Assert
            procedure.RequiresAuthorization.Should().BeTrue();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidCode_ThrowsArgumentException(string? code)
        {
            // Act
            var act = () => new TussProcedure(code!, "Description", "Category", 100m, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Code*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidDescription_ThrowsArgumentException(string? description)
        {
            // Act
            var act = () => new TussProcedure("40101012", description!, "Category", 100m, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Description*");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_WithInvalidCategory_ThrowsArgumentException(string? category)
        {
            // Act
            var act = () => new TussProcedure("40101012", "Description", category!, 100m, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Category*");
        }

        [Fact]
        public void Constructor_WithNegativeReferencePrice_ThrowsArgumentException()
        {
            // Act
            var act = () => new TussProcedure("40101012", "Description", "Category", -100m, TenantId);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Reference price*");
        }

        [Fact]
        public void Constructor_WithZeroPrice_CreatesValidProcedure()
        {
            // Act
            var procedure = new TussProcedure("40101012", "Description", "Category", 0m, TenantId);

            // Assert
            procedure.ReferencePrice.Should().Be(0m);
        }

        [Fact]
        public void UpdateInfo_WithValidData_UpdatesProcedure()
        {
            // Arrange
            var procedure = CreateValidProcedure();
            var newDescription = "Consulta médica especializada";
            var newCategory = "Consultas Especializadas";
            var newPrice = 250.00m;
            var newRequiresAuth = true;

            // Act
            procedure.UpdateInfo(newDescription, newCategory, newPrice, newRequiresAuth);

            // Assert
            procedure.Description.Should().Be(newDescription);
            procedure.Category.Should().Be(newCategory);
            procedure.ReferencePrice.Should().Be(newPrice);
            procedure.RequiresAuthorization.Should().BeTrue();
            procedure.LastUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            procedure.UpdatedAt.Should().NotBeNull();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateInfo_WithInvalidDescription_ThrowsArgumentException(string? description)
        {
            // Arrange
            var procedure = CreateValidProcedure();

            // Act
            var act = () => procedure.UpdateInfo(description!, "Category", 100m, false);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void UpdateInfo_WithInvalidCategory_ThrowsArgumentException(string? category)
        {
            // Arrange
            var procedure = CreateValidProcedure();

            // Act
            var act = () => procedure.UpdateInfo("Description", category!, 100m, false);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateInfo_WithNegativePrice_ThrowsArgumentException()
        {
            // Arrange
            var procedure = CreateValidProcedure();

            // Act
            var act = () => procedure.UpdateInfo("Description", "Category", -100m, false);

            // Assert
            act.Should().Throw<ArgumentException>();
        }

        [Fact]
        public void UpdateReferencePrice_WithValidPrice_UpdatesPrice()
        {
            // Arrange
            var procedure = CreateValidProcedure();
            var newPrice = 200.00m;

            // Act
            procedure.UpdateReferencePrice(newPrice);

            // Assert
            procedure.ReferencePrice.Should().Be(newPrice);
            procedure.LastUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
            procedure.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void UpdateReferencePrice_WithNegativePrice_ThrowsArgumentException()
        {
            // Arrange
            var procedure = CreateValidProcedure();

            // Act
            var act = () => procedure.UpdateReferencePrice(-100m);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Reference price*");
        }

        [Fact]
        public void Activate_SetsIsActiveToTrue()
        {
            // Arrange
            var procedure = CreateValidProcedure();
            procedure.Deactivate();

            // Act
            procedure.Activate();

            // Assert
            procedure.IsActive.Should().BeTrue();
            procedure.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Deactivate_SetsIsActiveToFalse()
        {
            // Arrange
            var procedure = CreateValidProcedure();

            // Act
            procedure.Deactivate();

            // Assert
            procedure.IsActive.Should().BeFalse();
            procedure.UpdatedAt.Should().NotBeNull();
        }

        [Fact]
        public void Constructor_TrimsWhitespace_FromStringProperties()
        {
            // Arrange & Act
            var procedure = new TussProcedure(
                "  40101012  ",
                "  Consulta médica  ",
                "  Consultas  ",
                150.00m,
                TenantId
            );

            // Assert
            procedure.Code.Should().Be("40101012");
            procedure.Description.Should().Be("Consulta médica");
            procedure.Category.Should().Be("Consultas");
        }

        [Fact]
        public void LastUpdated_IsSetOnCreation()
        {
            // Arrange & Act
            var procedure = CreateValidProcedure();

            // Assert
            procedure.LastUpdated.Should().NotBeNull();
            procedure.LastUpdated.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public void LastUpdated_IsUpdatedOnInfoChange()
        {
            // Arrange
            var procedure = CreateValidProcedure();
            var originalLastUpdated = procedure.LastUpdated;
            System.Threading.Thread.Sleep(100); // Ensure time difference

            // Act
            procedure.UpdateInfo("New Description", "New Category", 200m, true);

            // Assert
            procedure.LastUpdated.Should().NotBe(originalLastUpdated);
            procedure.LastUpdated.Should().BeAfter(originalLastUpdated!.Value);
        }

        [Fact]
        public void LastUpdated_IsUpdatedOnPriceChange()
        {
            // Arrange
            var procedure = CreateValidProcedure();
            var originalLastUpdated = procedure.LastUpdated;
            System.Threading.Thread.Sleep(100); // Ensure time difference

            // Act
            procedure.UpdateReferencePrice(200m);

            // Assert
            procedure.LastUpdated.Should().NotBe(originalLastUpdated);
            procedure.LastUpdated.Should().BeAfter(originalLastUpdated!.Value);
        }

        [Fact]
        public void ActivateDeactivate_CanBeToggledMultipleTimes()
        {
            // Arrange
            var procedure = CreateValidProcedure();

            // Act & Assert
            procedure.IsActive.Should().BeTrue();
            
            procedure.Deactivate();
            procedure.IsActive.Should().BeFalse();
            
            procedure.Activate();
            procedure.IsActive.Should().BeTrue();
            
            procedure.Deactivate();
            procedure.IsActive.Should().BeFalse();
        }

        private TussProcedure CreateValidProcedure()
        {
            return new TussProcedure(
                "40101012",
                "Consulta médica em consultório",
                "Consultas",
                150.00m,
                TenantId
            );
        }
    }
}
