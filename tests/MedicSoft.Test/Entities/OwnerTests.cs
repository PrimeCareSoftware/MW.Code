using System;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class OwnerTests
    {
        private readonly string _tenantId = "test-tenant";
        private readonly Guid _clinicId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidData_CreatesOwner()
        {
            // Arrange
            var username = "john.doe";
            var email = "john@clinic.com";
            var passwordHash = "hashed_password_123";
            var fullName = "John Doe";
            var phone = "+5511999999999";

            // Act
            var owner = new Owner(username, email, passwordHash, fullName, phone, _tenantId, _clinicId);

            // Assert
            Assert.NotEqual(Guid.Empty, owner.Id);
            Assert.Equal(username, owner.Username);
            Assert.Equal(email, owner.Email);
            Assert.Equal(passwordHash, owner.PasswordHash);
            Assert.Equal(fullName, owner.FullName);
            Assert.Equal(phone, owner.Phone);
            Assert.Equal(_clinicId, owner.ClinicId);
            Assert.True(owner.IsActive);
            Assert.Null(owner.LastLoginAt);
        }

        [Fact]
        public void Constructor_WithProfessionalData_CreatesOwner()
        {
            // Arrange
            var professionalId = "CRM 12345";
            var specialty = "Cardiologia";

            // Act
            var owner = CreateValidOwner(professionalId, specialty);

            // Assert
            Assert.Equal(professionalId, owner.ProfessionalId);
            Assert.Equal(specialty, owner.Specialty);
        }

        [Fact]
        public void Constructor_WithNullUsername_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Owner(null!, "email@test.com", "hash", "Name", "Phone", _tenantId, _clinicId));
        }

        [Fact]
        public void Constructor_WithEmptyUsername_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Owner("", "email@test.com", "hash", "Name", "Phone", _tenantId, _clinicId));
        }

        [Fact]
        public void Constructor_WithNullEmail_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Owner("username", null!, "hash", "Name", "Phone", _tenantId, _clinicId));
        }

        [Fact]
        public void UpdateProfile_WithValidData_UpdatesOwner()
        {
            // Arrange
            var owner = CreateValidOwner();
            var newEmail = "newemail@clinic.com";
            var newFullName = "John Doe Updated";
            var newPhone = "+5511988888888";

            // Act
            owner.UpdateProfile(newEmail, newFullName, newPhone);

            // Assert
            Assert.Equal(newEmail, owner.Email);
            Assert.Equal(newFullName, owner.FullName);
            Assert.Equal(newPhone, owner.Phone);
        }

        [Fact]
        public void UpdateProfile_WithProfessionalData_UpdatesOwner()
        {
            // Arrange
            var owner = CreateValidOwner();
            var newProfessionalId = "CRM 67890";
            var newSpecialty = "Pediatria";

            // Act
            owner.UpdateProfile("email@test.com", "Name", "Phone", newProfessionalId, newSpecialty);

            // Assert
            Assert.Equal(newProfessionalId, owner.ProfessionalId);
            Assert.Equal(newSpecialty, owner.Specialty);
        }

        [Fact]
        public void UpdateProfile_WithNullEmail_ThrowsArgumentException()
        {
            // Arrange
            var owner = CreateValidOwner();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                owner.UpdateProfile(null!, "Name", "Phone"));
        }

        [Fact]
        public void UpdatePassword_WithValidHash_UpdatesPassword()
        {
            // Arrange
            var owner = CreateValidOwner();
            var newPasswordHash = "new_hashed_password_456";

            // Act
            owner.UpdatePassword(newPasswordHash);

            // Assert
            Assert.Equal(newPasswordHash, owner.PasswordHash);
        }

        [Fact]
        public void UpdatePassword_WithNullHash_ThrowsArgumentException()
        {
            // Arrange
            var owner = CreateValidOwner();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                owner.UpdatePassword(null!));
        }

        [Fact]
        public void Activate_SetsIsActiveToTrue()
        {
            // Arrange
            var owner = CreateValidOwner();
            owner.Deactivate(); // First deactivate

            // Act
            owner.Activate();

            // Assert
            Assert.True(owner.IsActive);
        }

        [Fact]
        public void Deactivate_SetsIsActiveToFalse()
        {
            // Arrange
            var owner = CreateValidOwner();

            // Act
            owner.Deactivate();

            // Assert
            Assert.False(owner.IsActive);
        }

        [Fact]
        public void RecordLogin_UpdatesLastLoginAt()
        {
            // Arrange
            var owner = CreateValidOwner();
            var beforeLogin = DateTime.UtcNow;

            // Act
            owner.RecordLogin();
            var afterLogin = DateTime.UtcNow;

            // Assert
            Assert.NotNull(owner.LastLoginAt);
            Assert.True(owner.LastLoginAt >= beforeLogin);
            Assert.True(owner.LastLoginAt <= afterLogin);
        }

        [Fact]
        public void Username_IsConvertedToLowerCase()
        {
            // Arrange & Act
            var owner = new Owner("UPPERCASE.USER", "email@test.com", "hash", "Name", "Phone", _tenantId, _clinicId);

            // Assert
            Assert.Equal("uppercase.user", owner.Username);
        }

        [Fact]
        public void Email_IsConvertedToLowerCase()
        {
            // Arrange & Act
            var owner = new Owner("username", "EMAIL@TEST.COM", "hash", "Name", "Phone", _tenantId, _clinicId);

            // Assert
            Assert.Equal("email@test.com", owner.Email);
        }

        [Fact]
        public void Constructor_WithNullClinicId_CreatesSystemOwner()
        {
            // Arrange
            var username = "igor";
            var email = "igor@medicwarehouse.com";
            var passwordHash = "hashed_password_123";
            var fullName = "Igor Leessa";
            var phone = "+5511999999999";

            // Act
            var owner = new Owner(username, email, passwordHash, fullName, phone, "system", null);

            // Assert
            Assert.NotEqual(Guid.Empty, owner.Id);
            Assert.Equal(username, owner.Username);
            Assert.Equal(email, owner.Email);
            Assert.Equal(passwordHash, owner.PasswordHash);
            Assert.Equal(fullName, owner.FullName);
            Assert.Equal(phone, owner.Phone);
            Assert.Null(owner.ClinicId);
            Assert.True(owner.IsSystemOwner);
            Assert.True(owner.IsActive);
            Assert.Null(owner.LastLoginAt);
        }

        [Fact]
        public void IsSystemOwner_WithClinicId_ReturnsFalse()
        {
            // Arrange & Act
            var owner = new Owner("username", "email@test.com", "hash", "Name", "Phone", _tenantId, _clinicId);

            // Assert
            Assert.False(owner.IsSystemOwner);
            Assert.Equal(_clinicId, owner.ClinicId);
        }

        [Fact]
        public void IsSystemOwner_WithNullClinicId_ReturnsTrue()
        {
            // Arrange & Act
            var owner = new Owner("username", "email@test.com", "hash", "Name", "Phone", "system", null);

            // Assert
            Assert.True(owner.IsSystemOwner);
            Assert.Null(owner.ClinicId);
        }

        [Fact]
        public void Constructor_WithEmptyGuidClinicId_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Owner("username", "email@test.com", "hash", "Name", "Phone", _tenantId, Guid.Empty));
        }

        // Helper methods
        private Owner CreateValidOwner(string? professionalId = null, string? specialty = null)
        {
            return new Owner(
                "test.owner",
                "owner@clinic.com",
                "hashed_password",
                "Test Owner",
                "+5511999999999",
                _tenantId,
                _clinicId,
                professionalId,
                specialty
            );
        }
    }
}
