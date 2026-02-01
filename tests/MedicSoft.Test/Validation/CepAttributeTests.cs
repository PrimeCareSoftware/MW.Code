using System.ComponentModel.DataAnnotations;
using MedicSoft.Application.Validation;
using Xunit;

namespace MedicSoft.Test.Validation
{
    public class CepAttributeTests
    {
        private class TestModel
        {
            [Cep]
            public string? ZipCode { get; set; }
        }

        [Fact]
        public void CepAttribute_ShouldAcceptFormattedCep()
        {
            // Arrange
            var model = new TestModel { ZipCode = "26510-361" };
            var context = new ValidationContext(model) { MemberName = nameof(TestModel.ZipCode) };
            var attribute = new CepAttribute();

            // Act
            var result = attribute.GetValidationResult(model.ZipCode, context);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
            Assert.Equal("26510361", model.ZipCode); // Should be cleaned
        }

        [Fact]
        public void CepAttribute_ShouldAcceptUnformattedCep()
        {
            // Arrange
            var model = new TestModel { ZipCode = "26510361" };
            var context = new ValidationContext(model) { MemberName = nameof(TestModel.ZipCode) };
            var attribute = new CepAttribute();

            // Act
            var result = attribute.GetValidationResult(model.ZipCode, context);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
            Assert.Equal("26510361", model.ZipCode);
        }

        [Fact]
        public void CepAttribute_ShouldRejectInvalidLength()
        {
            // Arrange
            var model = new TestModel { ZipCode = "12345" }; // Only 5 digits
            var context = new ValidationContext(model) { MemberName = nameof(TestModel.ZipCode) };
            var attribute = new CepAttribute();

            // Act
            var result = attribute.GetValidationResult(model.ZipCode, context);

            // Assert
            Assert.NotNull(result);
            Assert.NotEqual(ValidationResult.Success, result);
            Assert.Contains("8 dígitos", result.ErrorMessage);
        }

        [Fact]
        public void CepAttribute_ShouldAcceptNullValue()
        {
            // Arrange
            var model = new TestModel { ZipCode = null };
            var context = new ValidationContext(model) { MemberName = nameof(TestModel.ZipCode) };
            var attribute = new CepAttribute();

            // Act
            var result = attribute.GetValidationResult(model.ZipCode, context);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }

        [Fact]
        public void CepAttribute_ShouldAcceptEmptyString()
        {
            // Arrange
            var model = new TestModel { ZipCode = "" };
            var context = new ValidationContext(model) { MemberName = nameof(TestModel.ZipCode) };
            var attribute = new CepAttribute();

            // Act
            var result = attribute.GetValidationResult(model.ZipCode, context);

            // Assert
            Assert.Equal(ValidationResult.Success, result);
        }
    }
}
