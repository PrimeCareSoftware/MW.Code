using System;
using Xunit;
using MedicSoft.Domain.Services;

namespace MedicSoft.Test.Services
{
    public class DocumentValidatorTests
    {
        [Theory]
        [InlineData("111.444.777-35")]
        [InlineData("11144477735")]
        [InlineData("123.456.789-09")]
        [InlineData("12345678909")]
        public void IsValidCpf_WithValidCpf_ReturnsTrue(string cpf)
        {
            // Act
            var result = DocumentValidator.IsValidCpf(cpf);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        [InlineData("12345678901")] // Invalid check digit
        [InlineData("000.000.000-00")] // All zeros
        [InlineData("111.111.111-11")] // All ones
        [InlineData("12345")] // Too short
        public void IsValidCpf_WithInvalidCpf_ReturnsFalse(string cpf)
        {
            // Act
            var result = DocumentValidator.IsValidCpf(cpf);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("11.222.333/0001-81")]
        [InlineData("11222333000181")]
        [InlineData("06.990.590/0001-23")]
        [InlineData("06990590000123")]
        public void IsValidCnpj_WithValidCnpj_ReturnsTrue(string cnpj)
        {
            // Act
            var result = DocumentValidator.IsValidCnpj(cnpj);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        [InlineData("11222333000182")] // Invalid check digit
        [InlineData("00.000.000/0000-00")] // All zeros
        [InlineData("11.111.111/1111-11")] // All ones
        [InlineData("123456")] // Too short
        public void IsValidCnpj_WithInvalidCnpj_ReturnsFalse(string cnpj)
        {
            // Act
            var result = DocumentValidator.IsValidCnpj(cnpj);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData("123456-SP")]
        [InlineData("12345/RJ")]
        [InlineData("1234567-MG")]
        public void IsValidCrm_WithValidCrm_ReturnsTrue(string crm)
        {
            // Act
            var result = DocumentValidator.IsValidCrm(crm);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        [InlineData("123456")] // Missing state
        [InlineData("123456-XX")] // Invalid state
        [InlineData("123-SP")] // Too short
        public void IsValidCrm_WithInvalidCrm_ReturnsFalse(string crm)
        {
            // Act
            var result = DocumentValidator.IsValidCrm(crm);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidateCpf_WithValidCpf_ReturnsCpfObject()
        {
            // Arrange
            var cpf = "11144477735";

            // Act
            var cpfObj = DocumentValidator.ValidateCpf(cpf);

            // Assert
            Assert.NotNull(cpfObj);
            Assert.Equal("11144477735", cpfObj.Value);
        }

        [Fact]
        public void ValidateCpf_WithInvalidCpf_ThrowsArgumentException()
        {
            // Arrange
            var invalidCpf = "12345678901";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => DocumentValidator.ValidateCpf(invalidCpf));
        }

        [Fact]
        public void ValidateCnpj_WithValidCnpj_ReturnsCnpjObject()
        {
            // Arrange
            var cnpj = "11222333000181";

            // Act
            var cnpjObj = DocumentValidator.ValidateCnpj(cnpj);

            // Assert
            Assert.NotNull(cnpjObj);
            Assert.Equal("11222333000181", cnpjObj.Value);
        }

        [Fact]
        public void ValidateCnpj_WithInvalidCnpj_ThrowsArgumentException()
        {
            // Arrange
            var invalidCnpj = "11222333000182";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => DocumentValidator.ValidateCnpj(invalidCnpj));
        }

        [Fact]
        public void ValidateCrm_WithValidCrm_ReturnsCrmObject()
        {
            // Arrange
            var crm = "123456-SP";

            // Act
            var crmObj = DocumentValidator.ValidateCrm(crm);

            // Assert
            Assert.NotNull(crmObj);
            Assert.Equal("123456", crmObj.Number);
            Assert.Equal("SP", crmObj.State);
        }

        [Fact]
        public void ValidateCrm_WithInvalidCrm_ThrowsArgumentException()
        {
            // Arrange
            var invalidCrm = "123-XX";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => DocumentValidator.ValidateCrm(invalidCrm));
        }
    }
}
