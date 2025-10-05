using System;
using Xunit;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Test.ValueObjects
{
    public class EmailTests
    {
        [Theory]
        [InlineData("test@example.com")]
        [InlineData("user.name@example.com")]
        [InlineData("user+tag@example.co.uk")]
        [InlineData("user_name123@test-domain.com")]
        public void Constructor_WithValidEmail_CreatesEmailObject(string email)
        {
            // Act
            var emailObj = new Email(email);

            // Assert
            Assert.NotNull(emailObj);
            Assert.Equal(email.ToLowerInvariant(), emailObj.Value);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void Constructor_WithNullOrEmpty_ThrowsArgumentException(string email)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(email));
            Assert.Equal("Email cannot be empty (Parameter 'value')", exception.Message);
        }

        [Theory]
        [InlineData("invalid")]
        [InlineData("@example.com")]
        [InlineData("user@")]
        [InlineData("user@domain")]
        [InlineData("user domain@example.com")]
        public void Constructor_WithInvalidFormat_ThrowsArgumentException(string email)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => new Email(email));
            Assert.Equal("Invalid email format (Parameter 'value')", exception.Message);
        }

        [Fact]
        public void Constructor_ConvertsEmailToLowerCase()
        {
            // Arrange
            var email = "USER@EXAMPLE.COM";

            // Act
            var emailObj = new Email(email);

            // Assert
            Assert.Equal("user@example.com", emailObj.Value);
        }

        [Fact]
        public void ToString_ReturnsEmailValue()
        {
            // Arrange
            var email = new Email("test@example.com");

            // Act
            var result = email.ToString();

            // Assert
            Assert.Equal("test@example.com", result);
        }

        [Fact]
        public void ImplicitConversion_ToStringReturnsValue()
        {
            // Arrange
            var email = new Email("test@example.com");

            // Act
            string result = email;

            // Assert
            Assert.Equal("test@example.com", result);
        }

        [Fact]
        public void ImplicitConversion_FromStringCreatesEmail()
        {
            // Arrange
            string emailString = "test@example.com";

            // Act
            Email email = emailString;

            // Assert
            Assert.Equal("test@example.com", email.Value);
        }
    }
}
