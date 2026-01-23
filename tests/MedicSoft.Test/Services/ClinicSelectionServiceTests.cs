using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using Moq;
using Xunit;

namespace MedicSoft.Test.Services
{
    public class ClinicSelectionServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IUserClinicLinkRepository> _mockUserClinicLinkRepository;
        private readonly Mock<IClinicRepository> _mockClinicRepository;
        private readonly IClinicSelectionService _clinicSelectionService;
        private const string TenantId = "test-tenant";

        public ClinicSelectionServiceTests()
        {
            _mockUserRepository = new Mock<IUserRepository>();
            _mockUserClinicLinkRepository = new Mock<IUserClinicLinkRepository>();
            _mockClinicRepository = new Mock<IClinicRepository>();
            
            _clinicSelectionService = new ClinicSelectionService(
                _mockUserRepository.Object,
                _mockUserClinicLinkRepository.Object,
                _mockClinicRepository.Object
            );
        }

        [Fact]
        public async Task GetUserClinicsAsync_WithMultipleClinics_ShouldReturnAllActiveClinics()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinic1Id = Guid.NewGuid();
            var clinic2Id = Guid.NewGuid();
            var clinic3Id = Guid.NewGuid();

            var link1 = new UserClinicLink(userId, clinic1Id, TenantId, isPreferredClinic: true);
            var link2 = new UserClinicLink(userId, clinic2Id, TenantId, isPreferredClinic: false);
            var link3 = new UserClinicLink(userId, clinic3Id, TenantId, isPreferredClinic: false);
            link3.Deactivate("Inactive link"); // This one should be excluded

            var clinic1 = CreateTestClinic(clinic1Id, "Preferred Clinic", "Address 1");
            var clinic2 = CreateTestClinic(clinic2Id, "Regular Clinic", "Address 2");
            var clinic3 = CreateTestClinic(clinic3Id, "Inactive Clinic", "Address 3");

            _mockUserClinicLinkRepository.Setup(r => r.GetByUserIdAsync(userId, TenantId))
                .ReturnsAsync(new List<UserClinicLink> { link1, link2, link3 });

            _mockClinicRepository.Setup(r => r.GetByIdAsync(clinic1Id, TenantId))
                .ReturnsAsync(clinic1);
            _mockClinicRepository.Setup(r => r.GetByIdAsync(clinic2Id, TenantId))
                .ReturnsAsync(clinic2);

            // Act
            var result = await _clinicSelectionService.GetUserClinicsAsync(userId, TenantId);

            // Assert
            result.Should().HaveCount(2);
            result.First().ClinicName.Should().Be("Preferred Clinic");
            result.First().IsPreferred.Should().BeTrue();
            result.Last().ClinicName.Should().Be("Regular Clinic");
        }

        [Fact]
        public async Task GetUserClinicsAsync_WithNoLinks_ShouldUseLegacyClinicId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();

            var user = CreateTestUser(userId, clinicId);
            var clinic = CreateTestClinic(clinicId, "Legacy Clinic", "Legacy Address");

            _mockUserClinicLinkRepository.Setup(r => r.GetByUserIdAsync(userId, TenantId))
                .ReturnsAsync(new List<UserClinicLink>());

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, TenantId))
                .ReturnsAsync(user);

            _mockClinicRepository.Setup(r => r.GetByIdAsync(clinicId, TenantId))
                .ReturnsAsync(clinic);

            // Act
            var result = await _clinicSelectionService.GetUserClinicsAsync(userId, TenantId);

            // Assert
            result.Should().HaveCount(1);
            result.First().ClinicName.Should().Be("Legacy Clinic");
            result.First().IsPreferred.Should().BeTrue();
        }

        [Fact]
        public async Task GetUserClinicsAsync_ShouldExcludeInactiveClinics()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var activeClinicId = Guid.NewGuid();
            var inactiveClinicId = Guid.NewGuid();

            var link1 = new UserClinicLink(userId, activeClinicId, TenantId);
            var link2 = new UserClinicLink(userId, inactiveClinicId, TenantId);

            var activeClinic = CreateTestClinic(activeClinicId, "Active Clinic", "Address 1");
            var inactiveClinic = CreateTestClinic(inactiveClinicId, "Inactive Clinic", "Address 2");
            inactiveClinic.Deactivate();

            _mockUserClinicLinkRepository.Setup(r => r.GetByUserIdAsync(userId, TenantId))
                .ReturnsAsync(new List<UserClinicLink> { link1, link2 });

            _mockClinicRepository.Setup(r => r.GetByIdAsync(activeClinicId, TenantId))
                .ReturnsAsync(activeClinic);
            _mockClinicRepository.Setup(r => r.GetByIdAsync(inactiveClinicId, TenantId))
                .ReturnsAsync(inactiveClinic);

            // Act
            var result = await _clinicSelectionService.GetUserClinicsAsync(userId, TenantId);

            // Assert
            result.Should().HaveCount(1);
            result.First().ClinicName.Should().Be("Active Clinic");
        }

        [Fact]
        public async Task SwitchClinicAsync_WithValidAccess_ShouldSwitchSuccessfully()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();

            var user = CreateTestUser(userId, Guid.NewGuid());
            var clinic = CreateTestClinic(clinicId, "Target Clinic", "Target Address");

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, TenantId))
                .ReturnsAsync(user);

            _mockUserClinicLinkRepository.Setup(r => r.UserHasAccessToClinicAsync(userId, clinicId, TenantId))
                .ReturnsAsync(true);

            _mockClinicRepository.Setup(r => r.GetByIdAsync(clinicId, TenantId))
                .ReturnsAsync(clinic);

            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            _mockUserRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _clinicSelectionService.SwitchClinicAsync(userId, clinicId, TenantId);

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().Be("Clinic switched successfully");
            result.CurrentClinicId.Should().Be(clinicId);
            result.CurrentClinicName.Should().Be("Target Clinic");
            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
            _mockUserRepository.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SwitchClinicAsync_WithUserNotFound_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, TenantId))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _clinicSelectionService.SwitchClinicAsync(userId, clinicId, TenantId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("User not found");
        }

        [Fact]
        public async Task SwitchClinicAsync_WithNoAccess_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();

            var user = CreateTestUser(userId, Guid.NewGuid());

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, TenantId))
                .ReturnsAsync(user);

            _mockUserClinicLinkRepository.Setup(r => r.UserHasAccessToClinicAsync(userId, clinicId, TenantId))
                .ReturnsAsync(false);

            // Act
            var result = await _clinicSelectionService.SwitchClinicAsync(userId, clinicId, TenantId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("User does not have access to this clinic");
        }

        [Fact]
        public async Task SwitchClinicAsync_WithLegacyClinicId_ShouldAllowSwitch()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();

            var user = CreateTestUser(userId, clinicId); // User has legacy ClinicId
            var clinic = CreateTestClinic(clinicId, "Legacy Clinic", "Legacy Address");

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, TenantId))
                .ReturnsAsync(user);

            _mockUserClinicLinkRepository.Setup(r => r.UserHasAccessToClinicAsync(userId, clinicId, TenantId))
                .ReturnsAsync(false); // No explicit link, but legacy ClinicId matches

            _mockClinicRepository.Setup(r => r.GetByIdAsync(clinicId, TenantId))
                .ReturnsAsync(clinic);

            _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
                .Returns(Task.CompletedTask);

            _mockUserRepository.Setup(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _clinicSelectionService.SwitchClinicAsync(userId, clinicId, TenantId);

            // Assert
            result.Success.Should().BeTrue();
        }

        [Fact]
        public async Task SwitchClinicAsync_WithInactiveClinic_ShouldReturnFailure()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();

            var user = CreateTestUser(userId, Guid.NewGuid());
            var clinic = CreateTestClinic(clinicId, "Inactive Clinic", "Inactive Address");
            clinic.Deactivate();

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, TenantId))
                .ReturnsAsync(user);

            _mockUserClinicLinkRepository.Setup(r => r.UserHasAccessToClinicAsync(userId, clinicId, TenantId))
                .ReturnsAsync(true);

            _mockClinicRepository.Setup(r => r.GetByIdAsync(clinicId, TenantId))
                .ReturnsAsync(clinic);

            // Act
            var result = await _clinicSelectionService.SwitchClinicAsync(userId, clinicId, TenantId);

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Clinic not found or inactive");
        }

        [Fact]
        public async Task GetCurrentClinicAsync_WithCurrentClinicSet_ShouldReturnCurrentClinic()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();

            var user = CreateTestUser(userId, clinicId);
            user.SetCurrentClinic(clinicId);
            
            var clinic = CreateTestClinic(clinicId, "Current Clinic", "Current Address");
            var link = new UserClinicLink(userId, clinicId, TenantId, isPreferredClinic: true);

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, TenantId))
                .ReturnsAsync(user);

            _mockClinicRepository.Setup(r => r.GetByIdAsync(clinicId, TenantId))
                .ReturnsAsync(clinic);

            _mockUserClinicLinkRepository.Setup(r => r.GetByUserAndClinicAsync(userId, clinicId, TenantId))
                .ReturnsAsync(link);

            // Act
            var result = await _clinicSelectionService.GetCurrentClinicAsync(userId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result!.ClinicName.Should().Be("Current Clinic");
            result.IsPreferred.Should().BeTrue();
        }

        [Fact]
        public async Task GetCurrentClinicAsync_WithUserNotFound_ShouldReturnNull()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, TenantId))
                .ReturnsAsync((User?)null);

            // Act
            var result = await _clinicSelectionService.GetCurrentClinicAsync(userId, TenantId);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public async Task GetCurrentClinicAsync_WithNoCurrentClinicSet_ShouldUseLegacyClinicId()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var clinicId = Guid.NewGuid();

            var user = CreateTestUser(userId, clinicId);
            var clinic = CreateTestClinic(clinicId, "Legacy Clinic", "Legacy Address");

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId, TenantId))
                .ReturnsAsync(user);

            _mockClinicRepository.Setup(r => r.GetByIdAsync(clinicId, TenantId))
                .ReturnsAsync(clinic);

            _mockUserClinicLinkRepository.Setup(r => r.GetByUserAndClinicAsync(userId, clinicId, TenantId))
                .ReturnsAsync((UserClinicLink?)null);

            // Act
            var result = await _clinicSelectionService.GetCurrentClinicAsync(userId, TenantId);

            // Assert
            result.Should().NotBeNull();
            result!.ClinicName.Should().Be("Legacy Clinic");
            result.IsPreferred.Should().BeTrue(); // Should be preferred when using legacy ClinicId
        }

        // Helper methods
        private User CreateTestUser(Guid userId, Guid clinicId)
        {
            var user = new User(
                "testuser",
                "test@example.com",
                "hashed_password",
                "Test User",
                "1234567890",
                UserRole.Doctor,
                TenantId,
                clinicId
            );
            // Use reflection to set Id since it's likely protected/private
            typeof(Domain.Common.BaseEntity).GetProperty("Id")!.SetValue(user, userId);
            return user;
        }

        private Clinic CreateTestClinic(Guid clinicId, string name, string address)
        {
            var clinic = new Clinic(
                name,
                name, // tradeName
                "12345678901234", // CNPJ
                "(11) 98765-4321",
                "test@clinic.com",
                address,
                TimeSpan.FromHours(8), // openingTime
                TimeSpan.FromHours(18), // closingTime
                TenantId
            );
            // Use reflection to set Id
            typeof(Domain.Common.BaseEntity).GetProperty("Id")!.SetValue(clinic, clinicId);
            return clinic;
        }
    }
}
