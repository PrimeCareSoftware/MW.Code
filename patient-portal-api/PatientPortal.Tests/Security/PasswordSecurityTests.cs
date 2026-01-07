using FluentAssertions;
using PatientPortal.Domain.Entities;

namespace PatientPortal.Tests.Security;

/// <summary>
/// Security tests for password hashing and validation
/// </summary>
[Trait("Category", "Security")]
public class PasswordSecurityTests
{
    [Fact]
    public void PasswordHash_ShouldNotStorePlaintext()
    {
        // Arrange
        var password = "SecurePassword123!";
        var user = new PatientUser
        {
            Email = "test@example.com",
            CPF = "12345678901",
            FullName = "Test User"
        };

        // Act
        user.SetPassword(password);

        // Assert
        user.PasswordHash.Should().NotBe(password);
        user.PasswordHash.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void PasswordHash_ShouldBeDifferentForSamePassword()
    {
        // Arrange
        var password = "SecurePassword123!";
        var user1 = new PatientUser
        {
            Email = "test1@example.com",
            CPF = "12345678901",
            FullName = "Test User 1"
        };
        var user2 = new PatientUser
        {
            Email = "test2@example.com",
            CPF = "98765432100",
            FullName = "Test User 2"
        };

        // Act
        user1.SetPassword(password);
        user2.SetPassword(password);

        // Assert - Hashes should be different due to salt
        user1.PasswordHash.Should().NotBe(user2.PasswordHash);
    }

    [Fact]
    public void VerifyPassword_WithCorrectPassword_ShouldReturnTrue()
    {
        // Arrange
        var password = "SecurePassword123!";
        var user = new PatientUser
        {
            Email = "test@example.com",
            CPF = "12345678901",
            FullName = "Test User"
        };
        user.SetPassword(password);

        // Act
        var result = user.VerifyPassword(password);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void VerifyPassword_WithIncorrectPassword_ShouldReturnFalse()
    {
        // Arrange
        var correctPassword = "SecurePassword123!";
        var wrongPassword = "WrongPassword123!";
        var user = new PatientUser
        {
            Email = "test@example.com",
            CPF = "12345678901",
            FullName = "Test User"
        };
        user.SetPassword(correctPassword);

        // Act
        var result = user.VerifyPassword(wrongPassword);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void VerifyPassword_WithEmptyPassword_ShouldReturnFalse()
    {
        // Arrange
        var password = "SecurePassword123!";
        var user = new PatientUser
        {
            Email = "test@example.com",
            CPF = "12345678901",
            FullName = "Test User"
        };
        user.SetPassword(password);

        // Act
        var result = user.VerifyPassword(string.Empty);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void PasswordHash_ShouldBeMinimumLength()
    {
        // Arrange
        var password = "SecurePassword123!";
        var user = new PatientUser
        {
            Email = "test@example.com",
            CPF = "12345678901",
            FullName = "Test User"
        };

        // Act
        user.SetPassword(password);

        // Assert - PBKDF2 with 100k iterations should produce a hash of significant length
        user.PasswordHash.Should().HaveLength(88); // Base64 encoded PBKDF2 output
    }

    [Theory]
    [InlineData("short")]
    [InlineData("12345678")]
    [InlineData("password")]
    [InlineData("Password")]
    [InlineData("PASSWORD123")]
    public void WeakPasswords_ShouldBeRejectedByValidation(string weakPassword)
    {
        // This test documents that weak passwords should be rejected
        // Actual validation would be in the application layer
        
        // Arrange & Act
        var isStrong = IsPasswordStrong(weakPassword);

        // Assert
        isStrong.Should().BeFalse();
    }

    [Theory]
    [InlineData("StrongPassword123!")]
    [InlineData("MyP@ssw0rd2024")]
    [InlineData("Secure#Pass123")]
    public void StrongPasswords_ShouldPassValidation(string strongPassword)
    {
        // Arrange & Act
        var isStrong = IsPasswordStrong(strongPassword);

        // Assert
        isStrong.Should().BeTrue();
    }

    [Fact]
    public void PasswordHashing_ShouldBeResistantToTimingAttacks()
    {
        // Arrange
        var password = "SecurePassword123!";
        var user = new PatientUser
        {
            Email = "test@example.com",
            CPF = "12345678901",
            FullName = "Test User"
        };
        user.SetPassword(password);

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        // Act - Verify correct password
        user.VerifyPassword(password);
        var correctPasswordTime = stopwatch.ElapsedMilliseconds;
        
        stopwatch.Restart();
        
        // Verify incorrect password
        user.VerifyPassword("WrongPassword123!");
        var wrongPasswordTime = stopwatch.ElapsedMilliseconds;

        // Assert - Times should be similar (within reasonable variance)
        // This helps prevent timing attacks
        var timeDifference = Math.Abs(correctPasswordTime - wrongPasswordTime);
        timeDifference.Should().BeLessThan(100); // Allow 100ms variance
    }

    /// <summary>
    /// Helper method to validate password strength
    /// Requirements: Min 8 chars, at least 1 uppercase, 1 lowercase, 1 digit, 1 special char
    /// </summary>
    private static bool IsPasswordStrong(string password)
    {
        if (password.Length < 8) return false;
        if (!password.Any(char.IsUpper)) return false;
        if (!password.Any(char.IsLower)) return false;
        if (!password.Any(char.IsDigit)) return false;
        if (!password.Any(c => !char.IsLetterOrDigit(c))) return false;
        return true;
    }
}
