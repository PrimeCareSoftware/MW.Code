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
        private readonly IUserService _userService;

        public UserServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockPasswordHasher = new Mock<IPasswordHasher>();
            _mockModuleConfigRepository = new Mock<IModuleConfigurationRepository>();
            _userService = new UserService(
                _mockUserRepository.Object, 
                _mockPasswordHasher.Object,
                _mockModuleConfigRepository.Object);
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
    }
}
