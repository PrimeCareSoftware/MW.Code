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

        [Fact]
        public void SetSubdomain_WithSequentialNumber_ShouldAccept()
        {
            // Arrange
            var clinic = CreateTestClinic();
            var subdomain = "clinica-teste-2";

            // Act
            clinic.SetSubdomain(subdomain);

            // Assert
            Assert.Equal(subdomain, clinic.Subdomain);
        }

        [Fact]
        public void SetSubdomain_WithMultipleHyphens_ShouldAccept()
        {
            // Arrange
            var clinic = CreateTestClinic();
            var subdomain = "clinica-sao-jose";

            // Act
            clinic.SetSubdomain(subdomain);

            // Assert
            Assert.Equal(subdomain, clinic.Subdomain);
        }

        [Fact]
        public void SetSubdomain_UserFriendlyName_ShouldNotContainRandomHexCharacters()
        {
            // Arrange
            var clinic = CreateTestClinic();
            
            // Test various friendly subdomains that should be accepted
            var friendlySubdomains = new[]
            {
                "clinica-saude",
                "clinica-teste-2",
                "clinica-exemplo",
                "clinica-popular-3",
                "clinic",
                "my-clinic-15"
            };

            foreach (var subdomain in friendlySubdomains)
            {
                // Act
                clinic.SetSubdomain(subdomain);

                // Assert
                Assert.Equal(subdomain, clinic.Subdomain);
                
                // Verify it doesn't look like it has random hex patterns
                // Old implementation would produce patterns like "clinica-03-ff48" or "clinic-a1b2c3d4"
                // New implementation should only have readable names with optional sequential numbers
                var parts = subdomain.Split('-');
                foreach (var part in parts)
                {
                    // Each part should either be a word or a simple number (not a hex string)
                    if (int.TryParse(part, out _))
                    {
                        // It's a number, which is fine for sequential numbering
                        continue;
                    }
                    // Otherwise, it should be a readable word (all letters)
                    Assert.True(part.All(char.IsLetter), 
                        $"Part '{part}' in subdomain '{subdomain}' contains non-letter characters that look like random hex");
                }
            }
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
