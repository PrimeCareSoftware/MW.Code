using System;
using Xunit;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Test.ValueObjects
{
    public class CnpjTests
    {
        [Theory]
        [InlineData("11.222.333/0001-81")] // Valid CNPJ
        [InlineData("11222333000181")] // Valid CNPJ without formatting
        [InlineData("06.990.590/0001-23")] // Valid CNPJ
        [InlineData("06990590000123")] // Valid CNPJ without formatting
        public void Constructor_WithValidCnpj_CreatesCnpjObject(string cnpj)
        {
            // Act
            var cnpjObj = new Cnpj(cnpj);

            // Assert
            Assert.NotNull(cnpjObj);
            Assert.NotEmpty(cnpjObj.Value);
            Assert.Equal(14, cnpjObj.Value.Length);
        }

        [Theory]
        [InlineData("")] // Empty
        [InlineData("   ")] // Whitespace
        [InlineData(null)] // Null
        public void Constructor_WithNullOrEmpty_ThrowsArgumentException(string cnpj)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Cnpj(cnpj));
            Assert.Equal("CNPJ cannot be empty (Parameter 'value')", exception.Message);
        }

        [Theory]
        [InlineData("11.222.333/0001-8")] // Too short
        [InlineData("11.222.333/0001-811")] // Too long
        [InlineData("112223330001")] // Too short without formatting
        public void Constructor_WithInvalidLength_ThrowsArgumentException(string cnpj)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Cnpj(cnpj));
            Assert.Equal("CNPJ must have 14 digits (Parameter 'value')", exception.Message);
        }

        [Theory]
        [InlineData("00.000.000/0000-00")] // All zeros
        [InlineData("11.111.111/1111-11")] // All ones
        [InlineData("99.999.999/9999-99")] // All nines
        public void Constructor_WithRepeatedDigits_ThrowsArgumentException(string cnpj)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Cnpj(cnpj));
            Assert.Equal("Invalid CNPJ format (Parameter 'value')", exception.Message);
        }

        [Theory]
        [InlineData("11.222.333/0001-82")] // Invalid check digit
        [InlineData("06.990.590/0001-24")] // Invalid check digit
        public void Constructor_WithInvalidCheckDigit_ThrowsArgumentException(string cnpj)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Cnpj(cnpj));
            Assert.Equal("Invalid CNPJ check digits (Parameter 'value')", exception.Message);
        }

        [Fact]
        public void GetFormatted_ReturnsFormattedCnpj()
        {
            // Arrange
            var cnpj = new Cnpj("11222333000181");

            // Act
            var formatted = cnpj.GetFormatted();

            // Assert
            Assert.Equal("11.222.333/0001-81", formatted);
        }

        [Fact]
        public void ToString_ReturnsFormattedCnpj()
        {
            // Arrange
            var cnpj = new Cnpj("11222333000181");

            // Act
            var result = cnpj.ToString();

            // Assert
            Assert.Equal("11.222.333/0001-81", result);
        }

        [Fact]
        public void ImplicitConversion_ToStringReturnsValue()
        {
            // Arrange
            var cnpj = new Cnpj("11222333000181");

            // Act
            string result = cnpj;

            // Assert
            Assert.Equal("11222333000181", result);
        }
    }
}
