using MedicSoft.CrossCutting.Security;
using Xunit;

namespace MedicSoft.Test.Security
{
    public class PasswordHasherTests
    {
        private readonly PasswordHasher _passwordHasher;

        public PasswordHasherTests()
        {
            _passwordHasher = new PasswordHasher();
        }

        [Fact]
        public void HashPassword_WithValidPassword_ReturnsHashedPassword()
        {
            // Arrange
            var password = "MySecureP@ssw0rd!";

            // Act
            var hashedPassword = _passwordHasher.HashPassword(password);

            // Assert
            Assert.NotNull(hashedPassword);
            Assert.NotEqual(password, hashedPassword);
            Assert.True(hashedPassword.Length > 50); // BCrypt hashes are typically 60 characters
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void HashPassword_WithInvalidPassword_ThrowsArgumentException(string password)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() => _passwordHasher.HashPassword(password));
        }

        [Fact]
        public void VerifyPassword_WithCorrectPassword_ReturnsTrue()
        {
            // Arrange
            var password = "MySecureP@ssw0rd!";
            var hashedPassword = _passwordHasher.HashPassword(password);

            // Act
            var result = _passwordHasher.VerifyPassword(password, hashedPassword);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void VerifyPassword_WithIncorrectPassword_ReturnsFalse()
        {
            // Arrange
            var password = "MySecureP@ssw0rd!";
            var wrongPassword = "WrongPassword123!";
            var hashedPassword = _passwordHasher.HashPassword(password);

            // Act
            var result = _passwordHasher.VerifyPassword(wrongPassword, hashedPassword);

            // Assert
            Assert.False(result);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void VerifyPassword_WithInvalidPassword_ReturnsFalse(string password)
        {
            // Arrange
            var hashedPassword = "$2a$12$abcdefghijklmnopqrstuv";

            // Act
            var result = _passwordHasher.VerifyPassword(password, hashedPassword);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void ValidatePasswordStrength_WithStrongPassword_ReturnsValid()
        {
            // Arrange
            var password = "MySecureP@ssw0rd!";

            // Act
            var (isValid, errorMessage) = _passwordHasher.ValidatePasswordStrength(password, 8);

            // Assert
            Assert.True(isValid);
            Assert.Empty(errorMessage);
        }

        [Theory]
        [InlineData("short", "Password must be at least 8 characters long")]
        [InlineData("nouppercase1!", "Password must contain at least one uppercase letter")]
        [InlineData("NOLOWERCASE1!", "Password must contain at least one lowercase letter")]
        [InlineData("NoDigits!", "Password must contain at least one digit")]
        [InlineData("NoSpecial1", "Password must contain at least one special character")]
        [InlineData("Password123!", "Password contains common weak patterns")]
        public void ValidatePasswordStrength_WithWeakPassword_ReturnsInvalid(string password, string expectedError)
        {
            // Act
            var (isValid, errorMessage) = _passwordHasher.ValidatePasswordStrength(password, 8);

            // Assert
            Assert.False(isValid);
            Assert.Equal(expectedError, errorMessage);
        }

        [Fact]
        public void ValidatePasswordStrength_WithEmptyPassword_ReturnsInvalid()
        {
            // Arrange
            var password = "";

            // Act
            var (isValid, errorMessage) = _passwordHasher.ValidatePasswordStrength(password, 8);

            // Assert
            Assert.False(isValid);
            Assert.Equal("Password cannot be empty", errorMessage);
        }

        [Fact]
        public void ValidatePasswordStrength_WithCustomMinLength_ValidatesCorrectly()
        {
            // Arrange
            var password = "Sh0rt!";

            // Act
            var (isValid, errorMessage) = _passwordHasher.ValidatePasswordStrength(password, 12);

            // Assert
            Assert.False(isValid);
            Assert.Equal("Password must be at least 12 characters long", errorMessage);
        }

        [Fact]
        public void HashPassword_CreatesDifferentHashesForSamePassword()
        {
            // Arrange
            var password = "MySecureP@ssw0rd!";

            // Act
            var hash1 = _passwordHasher.HashPassword(password);
            var hash2 = _passwordHasher.HashPassword(password);

            // Assert
            Assert.NotEqual(hash1, hash2); // BCrypt uses salt, so hashes should be different
            Assert.True(_passwordHasher.VerifyPassword(password, hash1));
            Assert.True(_passwordHasher.VerifyPassword(password, hash2));
        }
    }
}
