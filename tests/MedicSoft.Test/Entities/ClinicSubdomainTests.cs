using System;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class ClinicSubdomainTests
    {
        [Fact]
        public void SetSubdomain_WithValidSubdomain_ShouldSetCorrectly()
        {
            // Arrange
            var clinic = CreateTestClinic();
            var validSubdomain = "clinic1";

            // Act
            clinic.SetSubdomain(validSubdomain);

            // Assert
            Assert.Equal(validSubdomain, clinic.Subdomain);
        }

        [Fact]
        public void SetSubdomain_WithUppercaseSubdomain_ShouldConvertToLowercase()
        {
            // Arrange
            var clinic = CreateTestClinic();
            var subdomain = "CLINIC1";

            // Act
            clinic.SetSubdomain(subdomain);

            // Assert
            Assert.Equal("clinic1", clinic.Subdomain);
        }

        [Fact]
        public void SetSubdomain_WithHyphens_ShouldAccept()
        {
            // Arrange
            var clinic = CreateTestClinic();
            var subdomain = "clinic-test-1";

            // Act
            clinic.SetSubdomain(subdomain);

            // Assert
            Assert.Equal(subdomain, clinic.Subdomain);
        }

        [Fact]
        public void SetSubdomain_WithInvalidCharacters_ShouldThrowException()
        {
            // Arrange
            var clinic = CreateTestClinic();
            var invalidSubdomain = "clinic@test";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => clinic.SetSubdomain(invalidSubdomain));
            Assert.Contains("lowercase letters, numbers, and hyphens", exception.Message);
        }

        [Fact]
        public void SetSubdomain_WithSpaces_ShouldThrowException()
        {
            // Arrange
            var clinic = CreateTestClinic();
            var invalidSubdomain = "clinic test";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => clinic.SetSubdomain(invalidSubdomain));
        }

        [Fact]
        public void SetSubdomain_WithTooShort_ShouldThrowException()
        {
            // Arrange
            var clinic = CreateTestClinic();
            var tooShort = "ab";

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => clinic.SetSubdomain(tooShort));
            Assert.Contains("between 3 and 63 characters", exception.Message);
        }

        [Fact]
        public void SetSubdomain_WithTooLong_ShouldThrowException()
        {
            // Arrange
            var clinic = CreateTestClinic();
            var tooLong = new string('a', 64);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => clinic.SetSubdomain(tooLong));
            Assert.Contains("between 3 and 63 characters", exception.Message);
        }

        [Fact]
        public void SetSubdomain_WithNull_ShouldSetToNull()
        {
            // Arrange
            var clinic = CreateTestClinic();
            clinic.SetSubdomain("clinic1");

            // Act
            clinic.SetSubdomain(null);

            // Assert
            Assert.Null(clinic.Subdomain);
        }

        [Fact]
        public void SetSubdomain_WithEmptyString_ShouldSetToNull()
        {
            // Arrange
            var clinic = CreateTestClinic();
            clinic.SetSubdomain("clinic1");

            // Act
            clinic.SetSubdomain("");

            // Assert
            Assert.Null(clinic.Subdomain);
        }

        [Fact]
        public void SetSubdomain_StartingWithHyphen_ShouldThrowException()
        {
            // Arrange
            var clinic = CreateTestClinic();
            var invalidSubdomain = "-clinic";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => clinic.SetSubdomain(invalidSubdomain));
        }

        [Fact]
        public void SetSubdomain_EndingWithHyphen_ShouldThrowException()
        {
            // Arrange
            var clinic = CreateTestClinic();
            var invalidSubdomain = "clinic-";

            // Act & Assert
            Assert.Throws<ArgumentException>(() => clinic.SetSubdomain(invalidSubdomain));
        }

        private Clinic CreateTestClinic()
        {
            return new Clinic(
                name: "Test Clinic",
                tradeName: "Test Trade Name",
                document: "11.222.333/0001-81", // Valid CNPJ format with check digits
                phone: "(11) 98765-4321",
                email: "test@clinic.com",
                address: "Test Address",
                openingTime: new TimeSpan(8, 0, 0),
                closingTime: new TimeSpan(18, 0, 0),
                tenantId: "test-tenant",
                appointmentDurationMinutes: 30
            );
        }
    }
}
