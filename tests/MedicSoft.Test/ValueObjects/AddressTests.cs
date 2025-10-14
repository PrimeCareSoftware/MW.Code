using System;
using Xunit;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Test.ValueObjects
{
    public class AddressTests
    {
        [Fact]
        public void Constructor_WithValidData_CreatesAddressObject()
        {
            // Arrange
            var street = "Main Street";
            var number = "123";
            var neighborhood = "Downtown";
            var city = "São Paulo";
            var state = "SP";
            var zipCode = "01234-567";
            var country = "Brazil";
            var complement = "Apt 45";

            // Act
            var address = new Address(street, number, neighborhood, city, state, zipCode, country, complement);

            // Assert
            Assert.NotNull(address);
            Assert.Equal(street, address.Street);
            Assert.Equal(number, address.Number);
            Assert.Equal(neighborhood, address.Neighborhood);
            Assert.Equal(city, address.City);
            Assert.Equal(state, address.State);
            Assert.Equal(zipCode, address.ZipCode);
            Assert.Equal(country, address.Country);
            Assert.Equal(complement, address.Complement);
        }

        [Fact]
        public void Constructor_WithoutComplement_CreatesAddressObject()
        {
            // Arrange & Act
            var address = new Address("Main Street", "123", "Downtown", "São Paulo", "SP", "01234-567", "Brazil");

            // Assert
            Assert.NotNull(address);
            Assert.Null(address.Complement);
        }

        [Theory]
        [InlineData("", "123", "Downtown", "São Paulo", "SP", "01234-567", "Brazil")]
        [InlineData("   ", "123", "Downtown", "São Paulo", "SP", "01234-567", "Brazil")]
        [InlineData(null, "123", "Downtown", "São Paulo", "SP", "01234-567", "Brazil")]
        public void Constructor_WithNullOrEmptyStreet_ThrowsArgumentException(
            string street, string number, string neighborhood, string city, string state, string zipCode, string country)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Address(street, number, neighborhood, city, state, zipCode, country));
            Assert.Equal("A rua não pode estar vazia (Parameter 'street')", exception.Message);
        }

        [Theory]
        [InlineData("Main Street", "", "Downtown", "São Paulo", "SP", "01234-567", "Brazil")]
        [InlineData("Main Street", "   ", "Downtown", "São Paulo", "SP", "01234-567", "Brazil")]
        [InlineData("Main Street", null, "Downtown", "São Paulo", "SP", "01234-567", "Brazil")]
        public void Constructor_WithNullOrEmptyNumber_ThrowsArgumentException(
            string street, string number, string neighborhood, string city, string state, string zipCode, string country)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Address(street, number, neighborhood, city, state, zipCode, country));
            Assert.Equal("O número não pode estar vazio (Parameter 'number')", exception.Message);
        }

        [Theory]
        [InlineData("Main Street", "123", "", "São Paulo", "SP", "01234-567", "Brazil")]
        [InlineData("Main Street", "123", "   ", "São Paulo", "SP", "01234-567", "Brazil")]
        [InlineData("Main Street", "123", null, "São Paulo", "SP", "01234-567", "Brazil")]
        public void Constructor_WithNullOrEmptyNeighborhood_ThrowsArgumentException(
            string street, string number, string neighborhood, string city, string state, string zipCode, string country)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Address(street, number, neighborhood, city, state, zipCode, country));
            Assert.Equal("O bairro não pode estar vazio (Parameter 'neighborhood')", exception.Message);
        }

        [Theory]
        [InlineData("Main Street", "123", "Downtown", "", "SP", "01234-567", "Brazil")]
        [InlineData("Main Street", "123", "Downtown", "   ", "SP", "01234-567", "Brazil")]
        [InlineData("Main Street", "123", "Downtown", null, "SP", "01234-567", "Brazil")]
        public void Constructor_WithNullOrEmptyCity_ThrowsArgumentException(
            string street, string number, string neighborhood, string city, string state, string zipCode, string country)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Address(street, number, neighborhood, city, state, zipCode, country));
            Assert.Equal("A cidade não pode estar vazia (Parameter 'city')", exception.Message);
        }

        [Theory]
        [InlineData("Main Street", "123", "Downtown", "São Paulo", "", "01234-567", "Brazil")]
        [InlineData("Main Street", "123", "Downtown", "São Paulo", "   ", "01234-567", "Brazil")]
        [InlineData("Main Street", "123", "Downtown", "São Paulo", null, "01234-567", "Brazil")]
        public void Constructor_WithNullOrEmptyState_ThrowsArgumentException(
            string street, string number, string neighborhood, string city, string state, string zipCode, string country)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Address(street, number, neighborhood, city, state, zipCode, country));
            Assert.Equal("O estado não pode estar vazio (Parameter 'state')", exception.Message);
        }

        [Theory]
        [InlineData("Main Street", "123", "Downtown", "São Paulo", "SP", "", "Brazil")]
        [InlineData("Main Street", "123", "Downtown", "São Paulo", "SP", "   ", "Brazil")]
        [InlineData("Main Street", "123", "Downtown", "São Paulo", "SP", null, "Brazil")]
        public void Constructor_WithNullOrEmptyZipCode_ThrowsArgumentException(
            string street, string number, string neighborhood, string city, string state, string zipCode, string country)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Address(street, number, neighborhood, city, state, zipCode, country));
            Assert.Equal("O CEP não pode estar vazio (Parameter 'zipCode')", exception.Message);
        }

        [Theory]
        [InlineData("Main Street", "123", "Downtown", "São Paulo", "SP", "01234-567", "")]
        [InlineData("Main Street", "123", "Downtown", "São Paulo", "SP", "01234-567", "   ")]
        [InlineData("Main Street", "123", "Downtown", "São Paulo", "SP", "01234-567", null)]
        public void Constructor_WithNullOrEmptyCountry_ThrowsArgumentException(
            string street, string number, string neighborhood, string city, string state, string zipCode, string country)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new Address(street, number, neighborhood, city, state, zipCode, country));
            Assert.Equal("O país não pode estar vazio (Parameter 'country')", exception.Message);
        }

        [Fact]
        public void Constructor_TrimsWhitespace()
        {
            // Arrange & Act
            var address = new Address(
                "  Main Street  ",
                "  123  ",
                "  Downtown  ",
                "  São Paulo  ",
                "  SP  ",
                "  01234-567  ",
                "  Brazil  ",
                "  Apt 45  "
            );

            // Assert
            Assert.Equal("Main Street", address.Street);
            Assert.Equal("123", address.Number);
            Assert.Equal("Downtown", address.Neighborhood);
            Assert.Equal("São Paulo", address.City);
            Assert.Equal("SP", address.State);
            Assert.Equal("01234-567", address.ZipCode);
            Assert.Equal("Brazil", address.Country);
            Assert.Equal("Apt 45", address.Complement);
        }

        [Fact]
        public void ToString_WithComplement_ReturnsFormattedAddress()
        {
            // Arrange
            var address = new Address("Main Street", "123", "Downtown", "São Paulo", "SP", "01234-567", "Brazil", "Apt 45");

            // Act
            var result = address.ToString();

            // Assert
            Assert.Equal("Main Street, 123, Apt 45, Downtown, São Paulo, SP, 01234-567, Brazil", result);
        }

        [Fact]
        public void ToString_WithoutComplement_ReturnsFormattedAddress()
        {
            // Arrange
            var address = new Address("Main Street", "123", "Downtown", "São Paulo", "SP", "01234-567", "Brazil");

            // Act
            var result = address.ToString();

            // Assert
            Assert.Equal("Main Street, 123, Downtown, São Paulo, SP, 01234-567, Brazil", result);
        }
    }
}
