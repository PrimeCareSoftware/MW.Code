using System;
using System.Threading.Tasks;
using Moq;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using Xunit;

namespace MedicSoft.Test.Services
{
    public class UserServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IPasswordHasher> _mockPasswordHasher;
        private readonly Mock<IModuleConfigurationRepository> _mockModuleConfigRepository;
        private readonly Mock<IUserClinicLinkRepository> _mockUserClinicLinkRepository;
        private readonly Mock<IClinicRepository> _mockClinicRepository;
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockModuleConfigRepository = new Mock<IModuleConfigurationRepository>();
            _mockUserClinicLinkRepository = new Mock<IUserClinicLinkRepository>();
            _mockClinicRepository = new Mock<IClinicRepository>();
            _userService = new UserService(
                _mockUserRepository.Object, 
                _mockPasswordHasher.Object,
                _mockModuleConfigRepository.Object,
                _mockUserClinicLinkRepository.Object,
                _mockClinicRepository.Object);
        }

        [Fact]
        public async Task ChangeUserPasswordAsync_ShouldUpdatePasswordSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var tenantId = "test-tenant";
            var newPassword = "NewSecurePassword123!";
            var hashedPassword = "hashed_password_123";

            var user = new User(
                "testuser",
                "test@example.com",
                "old_hash",
                "Test User",
                "1234567890",
                UserRole.Doctor,
                tenantId,
                Guid.NewGuid()
            );

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, tenantId))
                .ReturnsAsync(user);
            
            _mockPasswordHasher.Setup(h => h.HashPassword(newPassword))
                .Returns(hashedPassword);

            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            await _userService.ChangeUserPasswordAsync(userId, newPassword, tenantId);

            // Assert
            _mockPasswordHasher.Verify(h => h.HashPassword(newPassword), Times.Once);
            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task ChangeUserPasswordAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var tenantId = "test-tenant";
            var newPassword = "NewSecurePassword123!";

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, tenantId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _userService.ChangeUserPasswordAsync(userId, newPassword, tenantId)
            );
            
            Assert.Equal("User not found", exception.Message);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldCreateUserSuccessfully()
        {
            // Arrange
            var username = "newuser";
            var email = "newuser@example.com";
            var password = "SecurePassword123!";
            var fullName = "New User";
            var phone = "1234567890";
            var role = UserRole.Receptionist;
            var tenantId = "test-tenant";
            var clinicId = Guid.NewGuid();
            var hashedPassword = "hashed_password";

            _mockUserRepository.Setup(r => r.GetUserByUsernameAsync(username, tenantId))
                .ReturnsAsync((User?)null);

            _mockPasswordHasher.Setup(h => h.HashPassword(password))
                .Returns(hashedPassword);

            _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            var user = await _userService.CreateUserAsync(
                username,
                email,
                password,
                fullName,
                phone,
                role,
                tenantId,
                clinicId
            );

            // Assert
            Assert.NotNull(user);
            Assert.Equal(username.ToLowerInvariant(), user.Username);
            Assert.Equal(email.ToLowerInvariant(), user.Email);
            Assert.Equal(fullName, user.FullName);
            Assert.Equal(role, user.Role);
            Assert.True(user.IsActive);
            _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_ShouldThrowException_WhenUsernameExists()
        {
            // Arrange
            var username = "existinguser";
            var tenantId = "test-tenant";
            var existingUser = new User(
                username,
                "existing@example.com",
                "hash",
                "Existing User",
                "1234567890",
                UserRole.Doctor,
                tenantId,
                Guid.NewGuid()
            );

            _mockUserRepository.Setup(r => r.GetUserByUsernameAsync(username, tenantId))
                .ReturnsAsync(existingUser);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _userService.CreateUserAsync(
                    username,
                    "new@example.com",
                    "password",
                    "New User",
                    "1234567890",
                    UserRole.Receptionist,
                    tenantId,
                    Guid.NewGuid()
                )
            );

            Assert.Equal("Username already exists", exception.Message);
        }

        [Fact]
        public async Task ActivateUserAsync_ShouldActivateUserSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var tenantId = "test-tenant";
            var user = new User(
                "testuser",
                "test@example.com",
                "hash",
                "Test User",
                "1234567890",
                UserRole.Doctor,
                tenantId,
                Guid.NewGuid()
            );
            
            // Deactivate first
            user.Deactivate();

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, tenantId))
                .ReturnsAsync(user);

            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            await _userService.ActivateUserAsync(userId, tenantId);

            // Assert
            Assert.True(user.IsActive);
            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task DeactivateUserAsync_ShouldDeactivateUserSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var tenantId = "test-tenant";
            var user = new User(
                "testuser",
                "test@example.com",
                "hash",
                "Test User",
                "1234567890",
                UserRole.Doctor,
                tenantId,
                Guid.NewGuid()
            );

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, tenantId))
                .ReturnsAsync(user);

            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            await _userService.DeactivateUserAsync(userId, tenantId);

            // Assert
            Assert.False(user.IsActive);
            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task ChangeUserRoleAsync_ShouldChangeRoleSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var tenantId = "test-tenant";
            var newRole = UserRole.Nurse;
            var user = new User(
                "testuser",
                "test@example.com",
                "hash",
                "Test User",
                "1234567890",
                UserRole.Doctor,
                tenantId,
                Guid.NewGuid()
            );

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, tenantId))
                .ReturnsAsync(user);

            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            await _userService.ChangeUserRoleAsync(userId, newRole, tenantId);

            // Assert
            Assert.Equal(newRole, user.Role);
            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_Doctor_ShouldValidateProfessionalId_WhenRequired()
        {
            // Arrange
            var tenantId = "test-tenant";
            var clinicId = Guid.NewGuid();
            var configJson = "{\"ProfessionalIdRequired\":true,\"SpecialtyRequired\":false}";
            
            var moduleConfig = new ModuleConfiguration(
                clinicId,
                SystemModules.DoctorFieldsConfig,
                tenantId,
                true,
                configJson
            );

            _mockUserRepository.Setup(r => r.GetUserByUsernameAsync(It.IsAny<string>(), tenantId))
                .ReturnsAsync((User?)null);

            _mockModuleConfigRepository.Setup(r => r.GetByClinicAndModuleAsync(clinicId, SystemModules.DoctorFieldsConfig, tenantId))
                .ReturnsAsync(moduleConfig);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _userService.CreateUserAsync(
                    "testdoctor",
                    "doctor@test.com",
                    "password123",
                    "Dr. Test",
                    "1234567890",
                    UserRole.Doctor,
                    tenantId,
                    clinicId,
                    null, // No professional ID provided
                    "Cardiology"
                )
            );
            
            Assert.Equal("Professional ID (CRM) is required for doctors in this clinic", exception.Message);
        }

        [Fact]
        public async Task CreateUserAsync_Doctor_ShouldValidateSpecialty_WhenRequired()
        {
            // Arrange
            var tenantId = "test-tenant";
            var clinicId = Guid.NewGuid();
            var configJson = "{\"ProfessionalIdRequired\":false,\"SpecialtyRequired\":true}";
            
            var moduleConfig = new ModuleConfiguration(
                clinicId,
                SystemModules.DoctorFieldsConfig,
                tenantId,
                true,
                configJson
            );

            _mockUserRepository.Setup(r => r.GetUserByUsernameAsync(It.IsAny<string>(), tenantId))
                .ReturnsAsync((User?)null);

            _mockModuleConfigRepository.Setup(r => r.GetByClinicAndModuleAsync(clinicId, SystemModules.DoctorFieldsConfig, tenantId))
                .ReturnsAsync(moduleConfig);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _userService.CreateUserAsync(
                    "testdoctor",
                    "doctor@test.com",
                    "password123",
                    "Dr. Test",
                    "1234567890",
                    UserRole.Doctor,
                    tenantId,
                    clinicId,
                    "CRM-123456",
                    null // No specialty provided
                )
            );
            
            Assert.Equal("Specialty is required for doctors in this clinic", exception.Message);
        }

        [Fact]
        public async Task CreateUserAsync_Doctor_ShouldSucceed_WhenFieldsOptional()
        {
            // Arrange
            var tenantId = "test-tenant";
            var clinicId = Guid.NewGuid();
            var configJson = "{\"ProfessionalIdRequired\":false,\"SpecialtyRequired\":false}";
            
            var moduleConfig = new ModuleConfiguration(
                clinicId,
                SystemModules.DoctorFieldsConfig,
                tenantId,
                true,
                configJson
            );

            _mockUserRepository.Setup(r => r.GetUserByUsernameAsync(It.IsAny<string>(), tenantId))
                .ReturnsAsync((User?)null);

            _mockModuleConfigRepository.Setup(r => r.GetByClinicAndModuleAsync(clinicId, SystemModules.DoctorFieldsConfig, tenantId))
                .ReturnsAsync(moduleConfig);

            _mockPasswordHasher.Setup(h => h.HashPassword(It.IsAny<string>()))
                .Returns("hashed_password");

            _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            // Act - should not throw
            var user = await _userService.CreateUserAsync(
                "testdoctor",
                "doctor@test.com",
                "password123",
                "Dr. Test",
                "1234567890",
                UserRole.Doctor,
                tenantId,
                clinicId,
                null, // No professional ID
                null  // No specialty
            );

            // Assert
            Assert.NotNull(user);
            Assert.Equal(UserRole.Doctor, user.Role);
            _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task CreateUserAsync_NonDoctor_ShouldNotValidateDoctorFields()
        {
            // Arrange
            var tenantId = "test-tenant";
            var clinicId = Guid.NewGuid();

            _mockUserRepository.Setup(r => r.GetUserByUsernameAsync(It.IsAny<string>(), tenantId))
                .ReturnsAsync((User?)null);

            _mockPasswordHasher.Setup(h => h.HashPassword(It.IsAny<string>()))
                .Returns("hashed_password");

            _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<User>()))
                .ReturnsAsync((User u) => u);

            // Act - should not throw even without doctor fields
            var user = await _userService.CreateUserAsync(
                "testnurse",
                "nurse@test.com",
                "password123",
                "Nurse Test",
                "1234567890",
                UserRole.Nurse,
                tenantId,
                clinicId,
                null,
                null
            );

            // Assert
            Assert.NotNull(user);
            Assert.Equal(UserRole.Nurse, user.Role);
            _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
            // Should not check module configuration for non-doctors
            _mockModuleConfigRepository.Verify(
                r => r.GetByClinicAndModuleAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>()), 
                Times.Never);
        }

        [Fact]
        public async Task LinkUserToClinicAsync_ShouldCreateLinkSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var tenantId = "test-tenant";

            var user = new User("testuser", "test@example.com", "hash", "Test User", "1234567890", UserRole.Doctor, tenantId, Guid.NewGuid());
            var clinic = new Clinic("Test Clinic", "123 Main St", "1234567890", "test@clinic.com", Guid.NewGuid(), tenantId);

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, tenantId))
                .ReturnsAsync(user);
            
            _mockClinicRepository.Setup(r => r.GetByIdAsync(clinicId, tenantId))
                .ReturnsAsync(clinic);

            _mockUserClinicLinkRepository.Setup(r => r.GetByUserAndClinicAsync(userId, clinicId, tenantId))
                .ReturnsAsync((UserClinicLink?)null);

            _mockUserClinicLinkRepository.Setup(r => r.AddAsync(It.IsAny<UserClinicLink>()))
                .Returns(Task.CompletedTask);

            // Act
            var link = await _userService.LinkUserToClinicAsync(userId, clinicId, tenantId, false);

            // Assert
            Assert.NotNull(link);
            Assert.Equal(userId, link.UserId);
            Assert.Equal(clinicId, link.ClinicId);
            Assert.True(link.IsActive);
            _mockUserClinicLinkRepository.Verify(r => r.AddAsync(It.IsAny<UserClinicLink>()), Times.Once);
        }

        [Fact]
        public async Task LinkUserToClinicAsync_ShouldThrowException_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var tenantId = "test-tenant";

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, tenantId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _userService.LinkUserToClinicAsync(userId, clinicId, tenantId, false)
            );

            Assert.Contains("not found", exception.Message);
        }

        [Fact]
        public async Task LinkUserToClinicAsync_ShouldThrowException_WhenClinicNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var tenantId = "test-tenant";

            var user = new User("testuser", "test@example.com", "hash", "Test User", "1234567890", UserRole.Doctor, tenantId, Guid.NewGuid());

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, tenantId))
                .ReturnsAsync(user);
            
            _mockClinicRepository.Setup(r => r.GetByIdAsync(clinicId, tenantId))
                .ReturnsAsync((Clinic?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _userService.LinkUserToClinicAsync(userId, clinicId, tenantId, false)
            );

            Assert.Contains("not found", exception.Message);
        }

        [Fact]
        public async Task RemoveUserClinicLinkAsync_ShouldDeactivateLinkSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var tenantId = "test-tenant";

            var link = new UserClinicLink(userId, clinicId, tenantId, false);
            var user = new User("testuser", "test@example.com", "hash", "Test User", "1234567890", UserRole.Doctor, tenantId, clinicId);

            _mockUserClinicLinkRepository.Setup(r => r.GetByUserAndClinicAsync(userId, clinicId, tenantId))
                .ReturnsAsync(link);

            _mockUserClinicLinkRepository.Setup(r => r.UpdateAsync(It.IsAny<UserClinicLink>()))
                .Returns(Task.CompletedTask);

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, tenantId))
                .ReturnsAsync(user);

            _mockUserClinicLinkRepository.Setup(r => r.GetByUserIdAsync(userId, tenantId))
                .ReturnsAsync(new List<UserClinicLink> { link });

            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            // Act
            await _userService.RemoveUserClinicLinkAsync(userId, clinicId, tenantId);

            // Assert
            Assert.False(link.IsActive);
            _mockUserClinicLinkRepository.Verify(r => r.UpdateAsync(It.IsAny<UserClinicLink>()), Times.Once);
        }

        [Fact]
        public async Task RemoveUserClinicLinkAsync_ShouldThrowException_WhenLinkNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var tenantId = "test-tenant";

            _mockUserClinicLinkRepository.Setup(r => r.GetByUserAndClinicAsync(userId, clinicId, tenantId))
                .ReturnsAsync((UserClinicLink?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _userService.RemoveUserClinicLinkAsync(userId, clinicId, tenantId)
            );

            Assert.Contains("No link found", exception.Message);
        }

        [Fact]
        public async Task SetPreferredClinicAsync_ShouldSetPreferredSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var tenantId = "test-tenant";

            var targetLink = new UserClinicLink(userId, clinicId, tenantId, false);
            var otherLink = new UserClinicLink(userId, Guid.NewGuid(), tenantId, true);

            _mockUserClinicLinkRepository.Setup(r => r.GetByUserAndClinicAsync(userId, clinicId, tenantId))
                .ReturnsAsync(targetLink);

            _mockUserClinicLinkRepository.Setup(r => r.GetByUserIdAsync(userId, tenantId))
                .ReturnsAsync(new List<UserClinicLink> { targetLink, otherLink });

            _mockUserClinicLinkRepository.Setup(r => r.UpdateAsync(It.IsAny<UserClinicLink>()))
                .Returns(Task.CompletedTask);

            // Act
            await _userService.SetPreferredClinicAsync(userId, clinicId, tenantId);

            // Assert
            Assert.True(targetLink.IsPreferredClinic);
            Assert.False(otherLink.IsPreferredClinic);
            _mockUserClinicLinkRepository.Verify(r => r.UpdateAsync(It.IsAny<UserClinicLink>()), Times.Exactly(2));
        }

        [Fact]
        public async Task SetPreferredClinicAsync_ShouldThrowException_WhenUserHasNoAccessToClinic()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();
            var tenantId = "test-tenant";

            _mockUserClinicLinkRepository.Setup(r => r.GetByUserAndClinicAsync(userId, clinicId, tenantId))
                .ReturnsAsync((UserClinicLink?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _userService.SetPreferredClinicAsync(userId, clinicId, tenantId)
            );

            Assert.Contains("does not have access", exception.Message);
        }
    }
}
