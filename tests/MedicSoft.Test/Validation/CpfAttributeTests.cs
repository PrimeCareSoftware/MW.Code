using System.ComponentModel.DataAnnotations;
using MedicSoft.Application.Validation;
using Xunit;

namespace MedicSoft.Test.Validation
{
    public class CpfAttributeTests
    {
        private class TestModel
        {
            [Cpf]
            public string? Document { get; set; }
        }

        [Fact]
        public void CpfAttribute_ShouldAcceptFormattedCpf()
        {
            // Arrange
            var model = new TestModel { Document = "000.000.001-91" };
            var context = new ValidationContext(model) { MemberName = nameof(TestModel.Document) };
            var attribute = new CpfAttribute();

            // Act
            var result = attribute.GetValidationResult(model.Document, context);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
            Assert.Equal("00000000191", model.Document); // Should be cleaned
        }

        [Fact]
        public void CpfAttribute_ShouldAcceptUnformattedCpf()
        {
            // Arrange
            var model = new TestModel { Document = "00000000191" };
            var context = new ValidationContext(model) { MemberName = nameof(TestModel.Document) };
            var attribute = new CpfAttribute();

            // Act
            var result = attribute.GetValidationResult(model.Document, context);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
            Assert.Equal("00000000191", model.Document);
        }

        [Fact]
        public void CpfAttribute_ShouldRejectInvalidLength()
        {
            // Arrange
            var model = new TestModel { Document = "123456789" }; // Only 9 digits
            var context = new ValidationContext(model) { MemberName = nameof(TestModel.Document) };
            var attribute = new CpfAttribute();

            // Act
            var result = attribute.GetValidationResult(model.Document, context);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(ValidationResult.Success, result);
            Assert.Contains("11 dígitos", result.ErrorMessage);
        }

        [Fact]
        public void CpfAttribute_ShouldAcceptNullValue()
        {
            // Arrange
            var model = new TestModel { Document = null };
            var context = new ValidationContext(model) { MemberName = nameof(TestModel.Document) };
            var attribute = new CpfAttribute();

            // Act
            var result = attribute.GetValidationResult(model.Document, context);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void CpfAttribute_ShouldAcceptEmptyString()
        {
            // Arrange
            var model = new TestModel { Document = "" };
            var context = new ValidationContext(model) { MemberName = nameof(TestModel.Document) };
            var attribute = new CpfAttribute();

            // Act
            var result = attribute.GetValidationResult(model.Document, context);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }
    }
}
