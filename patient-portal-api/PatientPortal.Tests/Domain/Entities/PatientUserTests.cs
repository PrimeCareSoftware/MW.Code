using PatientPortal.Domain.Entities;
using Xunit;
using FluentAssertions;

namespace PatientPortal.Tests.Domain.Entities;

/// <summary>
/// Unit tests for PatientUser entity
/// </summary>
public class PatientUserTests
{
    [Fact]
    public void PatientUser_Creation_ShouldSetDefaultValues()
    {
        // Arrange & Act
        var patientUser = new PatientUser
        {
            Id = Guid.NewGuid(),
            Email = "test@example.com",
            FullName = "Test User",
            CPF = "12345678901",
            DateOfBirth = new DateTime(1990, 1, 1),
            CreatedAt = DateTime.UtcNow
        };

        // Assert
        patientUser.Id.Should().NotBeEmpty();
        patientUser.Email.Should().Be("test@example.com");
        patientUser.FullName.Should().Be("Test User");
        patientUser.CPF.Should().Be("12345678901");
        patientUser.IsActive.Should().BeTrue(); // Default value
        patientUser.AccessFailedCount.Should().Be(0); // Default value
    }

    [Fact]
    public void PatientUser_WithLockout_ShouldBeLockedOut()
    {
        // Arrange
        var patientUser = new PatientUser
        {
            Id = Guid.NewGuid(),
            LockoutEnd = DateTime.UtcNow.AddMinutes(15)
        };

        // Act & Assert
        patientUser.LockoutEnd.Should().NotBeNull();
        patientUser.LockoutEnd.Should().BeAfter(DateTime.UtcNow);
    }

    [Fact]
    public void PatientUser_IncrementAccessFailedCount_ShouldIncrease()
    {
        // Arrange
        var patientUser = new PatientUser
        {
            Id = Guid.NewGuid(),
            AccessFailedCount = 0
        };

        // Act
        patientUser.AccessFailedCount++;
        patientUser.AccessFailedCount++;

        // Assert
        patientUser.AccessFailedCount.Should().Be(2);
    }

    [Theory]
    [InlineData("test@example.com")]
    [InlineData("user.name+tag@example.co.uk")]
    [InlineData("x@example.com")]
    public void PatientUser_WithValidEmail_ShouldAccept(string email)
    {
        // Arrange & Act
        var patientUser = new PatientUser
        {
            Id = Guid.NewGuid(),
            Email = email
        };

        // Assert
        patientUser.Email.Should().Be(email);
    }

    [Theory]
    [InlineData("12345678901")]
    [InlineData("98765432100")]
    public void PatientUser_WithValidCPF_ShouldAccept(string cpf)
    {
        // Arrange & Act
        var patientUser = new PatientUser
        {
            Id = Guid.NewGuid(),
            CPF = cpf
        };

        // Assert
        patientUser.CPF.Should().Be(cpf);
        patientUser.CPF.Should().HaveLength(11);
    }
}
