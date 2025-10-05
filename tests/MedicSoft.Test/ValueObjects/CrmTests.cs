using System;
using Xunit;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Test.ValueObjects
{
    public class CrmTests
    {
        [Theory]
        [InlineData("123456", "SP")]
        [InlineData("12345", "RJ")]
        [InlineData("1234567", "MG")]
        [InlineData("1234", "RS")]
        public void Constructor_WithValidCrm_CreatesCrmObject(string number, string state)
        {
            // Act
            var crm = new Crm(number, state);

            // Assert
            Assert.NotNull(crm);
            Assert.Equal(number, crm.Number);
            Assert.Equal(state.ToUpperInvariant(), crm.State);
        }

        [Theory]
        [InlineData("", "SP")]
        [InlineData("   ", "SP")]
        [InlineData(null, "SP")]
        public void Constructor_WithNullOrEmptyNumber_ThrowsArgumentException(string number, string state)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Crm(number, state));
            Assert.Equal("CRM number cannot be empty (Parameter 'number')", exception.Message);
        }

        [Theory]
        [InlineData("123456", "")]
        [InlineData("123456", "   ")]
        [InlineData("123456", null)]
        public void Constructor_WithNullOrEmptyState_ThrowsArgumentException(string number, string state)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Crm(number, state));
            Assert.Equal("CRM state cannot be empty (Parameter 'state')", exception.Message);
        }

        [Theory]
        [InlineData("123")] // Too short
        [InlineData("12345678")] // Too long
        [InlineData("ABC123")] // Contains letters
        public void Constructor_WithInvalidNumber_ThrowsArgumentException(string number)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Crm(number, "SP"));
            Assert.Equal("CRM number must have between 4 and 7 digits (Parameter 'number')", exception.Message);
        }

        [Theory]
        [InlineData("XX")] // Invalid state
        [InlineData("ZZ")] // Invalid state
        [InlineData("ABC")] // Invalid state
        public void Constructor_WithInvalidState_ThrowsArgumentException(string state)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Crm("123456", state));
            Assert.Contains("Invalid state", exception.Message);
        }

        [Theory]
        [InlineData("sp")] // Lowercase
        [InlineData("Sp")] // Mixed case
        [InlineData("sP")] // Mixed case
        public void Constructor_NormalizesStateToUpperCase(string state)
        {
            // Act
            var crm = new Crm("123456", state);

            // Assert
            Assert.Equal("SP", crm.State);
        }

        [Fact]
        public void ToString_ReturnsFormattedCrm()
        {
            // Arrange
            var crm = new Crm("123456", "SP");

            // Act
            var result = crm.ToString();

            // Assert
            Assert.Equal("123456-SP", result);
        }

        [Theory]
        [InlineData("123456-SP", "123456", "SP")]
        [InlineData("12345/RJ", "12345", "RJ")]
        [InlineData("1234567-MG", "1234567", "MG")]
        public void Parse_WithValidString_CreatesCrmObject(string crmString, string expectedNumber, string expectedState)
        {
            // Act
            var crm = Crm.Parse(crmString);

            // Assert
            Assert.Equal(expectedNumber, crm.Number);
            Assert.Equal(expectedState.ToUpperInvariant(), crm.State);
        }

        [Theory]
        [InlineData("")] // Empty
        [InlineData("   ")] // Whitespace
        [InlineData(null)] // Null
        public void Parse_WithNullOrEmpty_ThrowsArgumentException(string crmString)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Crm.Parse(crmString));
            Assert.Equal("CRM string cannot be empty (Parameter 'crmString')", exception.Message);
        }

        [Theory]
        [InlineData("123456")] // Missing separator and state
        [InlineData("123456SP")] // Missing separator
        [InlineData("123456-SP-RJ")] // Too many parts
        public void Parse_WithInvalidFormat_ThrowsArgumentException(string crmString)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => Crm.Parse(crmString));
            Assert.Equal("CRM must be in format: NUMBER-UF or NUMBER/UF (Parameter 'crmString')", exception.Message);
        }

        [Theory]
        [InlineData("AC")]
        [InlineData("AL")]
        [InlineData("AP")]
        [InlineData("AM")]
        [InlineData("BA")]
        [InlineData("CE")]
        [InlineData("DF")]
        [InlineData("ES")]
        [InlineData("GO")]
        [InlineData("MA")]
        [InlineData("MT")]
        [InlineData("MS")]
        [InlineData("MG")]
        [InlineData("PA")]
        [InlineData("PB")]
        [InlineData("PR")]
        [InlineData("PE")]
        [InlineData("PI")]
        [InlineData("RJ")]
        [InlineData("RN")]
        [InlineData("RS")]
        [InlineData("RO")]
        [InlineData("RR")]
        [InlineData("SC")]
        [InlineData("SP")]
        [InlineData("SE")]
        [InlineData("TO")]
        public void Constructor_WithAllValidBrazilianStates_CreatesCrmObject(string state)
        {
            // Act
            var crm = new Crm("123456", state);

            // Assert
            Assert.Equal(state.ToUpperInvariant(), crm.State);
        }
    }
}
