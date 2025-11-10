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
            var sessionId = Guid.NewGuid().ToString();

            // Act
            owner.RecordLogin(sessionId);
            var afterLogin = DateTime.UtcNow;

            // Assert
            Assert.NotNull(owner.LastLoginAt);
            Assert.True(owner.LastLoginAt >= beforeLogin);
            Assert.True(owner.LastLoginAt <= afterLogin);
            Assert.Equal(sessionId, owner.CurrentSessionId);
        }

        [Fact]
        public void RecordLogin_ThrowsException_WhenSessionIdIsEmpty()
        {
            // Arrange
            var owner = CreateValidOwner();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => owner.RecordLogin(""));
            Assert.Throws<ArgumentException>(() => owner.RecordLogin(null!));
        }

        [Fact]
        public void IsSessionValid_ReturnsTrueForMatchingSession()
        {
            // Arrange
            var owner = CreateValidOwner();
            var sessionId = Guid.NewGuid().ToString();
            owner.RecordLogin(sessionId);

            // Act
            var isValid = owner.IsSessionValid(sessionId);

            // Assert
            Assert.True(isValid);
        }

        [Fact]
        public void IsSessionValid_ReturnsFalseForDifferentSession()
        {
            // Arrange
            var owner = CreateValidOwner();
            var sessionId1 = Guid.NewGuid().ToString();
            var sessionId2 = Guid.NewGuid().ToString();
            owner.RecordLogin(sessionId1);

            // Act
            var isValid = owner.IsSessionValid(sessionId2);

            // Assert
            Assert.False(isValid);
        }

        [Fact]
        public void RecordLogin_InvalidatesPreviousSession()
        {
            // Arrange
            var owner = CreateValidOwner();
            var sessionId1 = Guid.NewGuid().ToString();
            var sessionId2 = Guid.NewGuid().ToString();
            
            // Act - First login
            owner.RecordLogin(sessionId1);
            Assert.True(owner.IsSessionValid(sessionId1));
            
            // Act - Second login (should invalidate first session)
            owner.RecordLogin(sessionId2);
            
            // Assert
            Assert.False(owner.IsSessionValid(sessionId1));
            Assert.True(owner.IsSessionValid(sessionId2));
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

        [Fact]
        public void SystemOwner_CannotBeAssignedToClinic_ClinicIdIsReadonlyAfterCreation()
        {
            // Arrange - Create a system owner (no clinic)
            var systemOwner = new Owner("sysowner", "sys@medicwarehouse.com", "hash", "System Owner", "123456789", "system", null);
            
            // Assert - System owner should not have a clinic
            Assert.True(systemOwner.IsSystemOwner);
            Assert.Null(systemOwner.ClinicId);
            
            // Business Rule: There is NO method to assign a clinic to a system owner after creation
            // The ClinicId property is readonly and can only be set via constructor
            // UpdateProfile() does NOT allow changing ClinicId
            
            // Act - Try to update profile (this should NOT change ClinicId)
            systemOwner.UpdateProfile("newemail@test.com", "New Name", "987654321");
            
            // Assert - ClinicId should still be null after profile update
            Assert.Null(systemOwner.ClinicId);
            Assert.True(systemOwner.IsSystemOwner);
            
            // Note: This test documents the business rule that system owners cannot "join" a clinic
            // Once created as a system owner (ClinicId=null), they remain a system owner forever
        }

        [Fact]
        public void ClinicOwner_CannotBecomeSystemOwner_ClinicIdIsReadonlyAfterCreation()
        {
            // Arrange - Create a clinic owner
            var clinicOwner = new Owner("clinicowner", "owner@clinic.com", "hash", "Clinic Owner", "123456789", _tenantId, _clinicId);
            
            // Assert - Clinic owner should have a clinic
            Assert.False(clinicOwner.IsSystemOwner);
            Assert.Equal(_clinicId, clinicOwner.ClinicId);
            
            // Business Rule: There is NO method to remove a clinic from a clinic owner
            // The ClinicId property is readonly and can only be set via constructor
            // UpdateProfile() does NOT allow changing ClinicId
            
            // Act - Try to update profile (this should NOT change ClinicId)
            clinicOwner.UpdateProfile("newemail@test.com", "New Name", "987654321");
            
            // Assert - ClinicId should still be the same after profile update
            Assert.Equal(_clinicId, clinicOwner.ClinicId);
            Assert.False(clinicOwner.IsSystemOwner);
            
            // Note: This test documents the business rule that clinic owners cannot become system owners
            // Once created with a clinic, they remain tied to that clinic forever
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
