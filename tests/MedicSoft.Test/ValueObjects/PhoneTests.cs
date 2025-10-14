using System;
using Xunit;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Test.ValueObjects
{
    public class PhoneTests
    {
        [Theory]
        [InlineData("+55", "11999999999")]
        [InlineData("+1", "5551234567")]
        [InlineData("+44", "2012345678")]
        public void Constructor_WithValidPhone_CreatesPhoneObject(string countryCode, string number)
        {
            // Act
            var phone = new Phone(countryCode, number);

            // Assert
            Assert.NotNull(phone);
            Assert.Equal(countryCode, phone.CountryCode);
            Assert.Equal(number, phone.Number);
        }

        [Theory]
        [InlineData("", "11999999999")]
        [InlineData("   ", "11999999999")]
        [InlineData(null, "11999999999")]
        public void Constructor_WithNullOrEmptyCountryCode_ThrowsArgumentException(string countryCode, string number)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Phone(countryCode, number));
            Assert.Equal("O código do país não pode estar vazio (Parameter 'countryCode')", exception.Message);
        }

        [Theory]
        [InlineData("+55", "")]
        [InlineData("+55", "   ")]
        [InlineData("+55", null)]
        public void Constructor_WithNullOrEmptyNumber_ThrowsArgumentException(string countryCode, string number)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Phone(countryCode, number));
            Assert.Equal("O número de telefone não pode estar vazio (Parameter 'number')", exception.Message);
        }

        [Fact]
        public void Constructor_TrimsWhitespace()
        {
            // Arrange
            var countryCode = "  +55  ";
            var number = "  11999999999  ";

            // Act
            var phone = new Phone(countryCode, number);

            // Assert
            Assert.Equal("+55", phone.CountryCode);
            Assert.Equal("11999999999", phone.Number);
        }

        [Fact]
        public void ToString_ReturnsFormattedPhone()
        {
            // Arrange
            var phone = new Phone("+55", "11999999999");

            // Act
            var result = phone.ToString();

            // Assert
            Assert.Equal("+55 11999999999", result);
        }
    }
}
