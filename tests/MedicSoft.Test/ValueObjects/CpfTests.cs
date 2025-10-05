using System;
using Xunit;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Test.ValueObjects
{
    public class CpfTests
    {
        [Theory]
        [InlineData("111.444.777-35")] // Valid CPF
        [InlineData("11144477735")] // Valid CPF without formatting
        [InlineData("123.456.789-09")] // Valid CPF
        [InlineData("12345678909")] // Valid CPF without formatting
        public void Constructor_WithValidCpf_CreatesCpfObject(string cpf)
        {
            // Act
            var cpfObj = new Cpf(cpf);

            // Assert
            Assert.NotNull(cpfObj);
            Assert.NotEmpty(cpfObj.Value);
            Assert.Equal(11, cpfObj.Value.Length);
        }

        [Theory]
        [InlineData("")] // Empty
        [InlineData("   ")] // Whitespace
        [InlineData(null)] // Null
        public void Constructor_WithNullOrEmpty_ThrowsArgumentException(string cpf)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Cpf(cpf));
            Assert.Equal("CPF cannot be empty (Parameter 'value')", exception.Message);
        }

        [Theory]
        [InlineData("123.456.789-0")] // Too short
        [InlineData("123.456.789-099")] // Too long
        [InlineData("12345678")] // Too short without formatting
        public void Constructor_WithInvalidLength_ThrowsArgumentException(string cpf)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Cpf(cpf));
            Assert.Equal("CPF must have 11 digits (Parameter 'value')", exception.Message);
        }

        [Theory]
        [InlineData("000.000.000-00")] // All zeros
        [InlineData("111.111.111-11")] // All ones
        [InlineData("999.999.999-99")] // All nines
        public void Constructor_WithRepeatedDigits_ThrowsArgumentException(string cpf)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Cpf(cpf));
            Assert.Equal("Invalid CPF format (Parameter 'value')", exception.Message);
        }

        [Theory]
        [InlineData("111.444.777-36")] // Invalid check digit
        [InlineData("123.456.789-10")] // Invalid check digit
        public void Constructor_WithInvalidCheckDigit_ThrowsArgumentException(string cpf)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Cpf(cpf));
            Assert.Equal("Invalid CPF check digits (Parameter 'value')", exception.Message);
        }

        [Fact]
        public void GetFormatted_ReturnsFormattedCpf()
        {
            // Arrange
            var cpf = new Cpf("11144477735");

            // Act
            var formatted = cpf.GetFormatted();

            // Assert
            Assert.Equal("111.444.777-35", formatted);
        }

        [Fact]
        public void ToString_ReturnsFormattedCpf()
        {
            // Arrange
            var cpf = new Cpf("11144477735");

            // Act
            var result = cpf.ToString();

            // Assert
            Assert.Equal("111.444.777-35", result);
        }

        [Fact]
        public void ImplicitConversion_ToStringReturnsValue()
        {
            // Arrange
            var cpf = new Cpf("11144477735");

            // Act
            string result = cpf;

            // Assert
            Assert.Equal("11144477735", result);
        }
    }
}
