using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using Xunit;

namespace MedicSoft.Test.Services
{
    public class AccessProfileServiceTests
    {
        private readonly Mock<IAccessProfileRepository> _mockProfileRepository;
        private readonly Mock<IUserRepository> _mockUserRepository;
        private readonly Mock<IConsultationFormProfileRepository> _mockConsultationFormProfileRepository;
        private readonly Mock<IClinicRepository> _mockClinicRepository;
        private readonly IAccessProfileService _profileService;

        public AccessProfileServiceTests()
        {
            _mockProfileRepository = new Mock<IAccessProfileRepository>();
            _mockUserRepository = new Mock<IUserRepository>();
            _mockConsultationFormProfileRepository = new Mock<IConsultationFormProfileRepository>();
            _mockClinicRepository = new Mock<IClinicRepository>();
            _profileService = new AccessProfileService(
                _mockProfileRepository.Object,
                _mockUserRepository.Object,
                _mockConsultationFormProfileRepository.Object,
                _mockClinicRepository.Object);
        }

        [Fact]
        public async Task UpdateAsync_WithDefaultProfile_CreatesClinicSpecificCopy()
        {
            // Arrange
            var tenantId = "test-tenant";
            var clinicId = Guid.NewGuid();
            var profileId = Guid.NewGuid();
            
            var defaultProfile = AccessProfile.CreateDefaultMedicalProfile(tenantId, clinicId);
            
            var updateDto = new UpdateAccessProfileDto
            {
                Name = "Médico Customizado",
                Description = "Perfil médico customizado para esta clínica",
                Permissions = new List<string> { "patients.view", "appointments.view" }
            };

            _mockProfileRepository.Setup(r => r.GetByIdAsync(profileId, tenantId))
                .ReturnsAsync(defaultProfile);

            _mockProfileRepository.Setup(r => r.AddAsync(It.IsAny<AccessProfile>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _profileService.UpdateAsync(profileId, updateDto, tenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateDto.Name, result.Name);
            Assert.Equal(updateDto.Description, result.Description);
            Assert.False(result.IsDefault); // The new profile should NOT be marked as default
            Assert.Equal(clinicId, result.ClinicId);
            Assert.Equal(updateDto.Permissions.Count, result.Permissions.Count);
            
            // Verify that a new profile was created (AddAsync was called)
            _mockProfileRepository.Verify(r => r.AddAsync(It.Is<AccessProfile>(
                p => !p.IsDefault && 
                     p.ClinicId == clinicId && 
                     p.Name == updateDto.Name
            )), Times.Once);
            
            // Verify that UpdateAsync was NOT called (we created a new profile instead)
            _mockProfileRepository.Verify(r => r.UpdateAsync(It.IsAny<AccessProfile>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WithCustomProfile_UpdatesDirectly()
        {
            // Arrange
            var tenantId = "test-tenant";
            var clinicId = Guid.NewGuid();
            var profileId = Guid.NewGuid();
            
            var customProfile = new AccessProfile(
                "Perfil Personalizado",
                "Um perfil customizado",
                tenantId,
                clinicId,
                isDefault: false
            );
            
            var updateDto = new UpdateAccessProfileDto
            {
                Name = "Perfil Atualizado",
                Description = "Descrição atualizada",
                Permissions = new List<string> { "patients.view" }
            };

            _mockProfileRepository.Setup(r => r.GetByIdAsync(profileId, tenantId))
                .ReturnsAsync(customProfile);

            _mockProfileRepository.Setup(r => r.UpdateAsync(It.IsAny<AccessProfile>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _profileService.UpdateAsync(profileId, updateDto, tenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateDto.Name, result.Name);
            Assert.Equal(updateDto.Description, result.Description);
            Assert.False(result.IsDefault);
            
            // Verify that UpdateAsync was called (we modified the existing profile)
            _mockProfileRepository.Verify(r => r.UpdateAsync(It.IsAny<AccessProfile>()), Times.Once);
            
            // Verify that AddAsync was NOT called (we didn't create a new profile)
            _mockProfileRepository.Verify(r => r.AddAsync(It.IsAny<AccessProfile>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAsync_WithDefaultProfile_PreservesConsultationFormProfile()
        {
            // Arrange
            var tenantId = "test-tenant";
            var clinicId = Guid.NewGuid();
            var profileId = Guid.NewGuid();
            var consultationFormProfileId = Guid.NewGuid();
            
            var defaultProfile = AccessProfile.CreateDefaultMedicalProfile(tenantId, clinicId);
            defaultProfile.SetConsultationFormProfile(consultationFormProfileId);
            
            var updateDto = new UpdateAccessProfileDto
            {
                Name = "Médico Customizado",
                Description = "Perfil médico customizado",
                Permissions = new List<string> { "patients.view" }
            };

            _mockProfileRepository.Setup(r => r.GetByIdAsync(profileId, tenantId))
                .ReturnsAsync(defaultProfile);

            _mockProfileRepository.Setup(r => r.AddAsync(It.IsAny<AccessProfile>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _profileService.UpdateAsync(profileId, updateDto, tenantId);

            // Assert
            Assert.Equal(consultationFormProfileId, result.ConsultationFormProfileId);
            
            _mockProfileRepository.Verify(r => r.AddAsync(It.Is<AccessProfile>(
                p => p.ConsultationFormProfileId == consultationFormProfileId
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_WithDefaultProfile_CopiesPermissionsWhenNotProvided()
        {
            // Arrange
            var tenantId = "test-tenant";
            var clinicId = Guid.NewGuid();
            var profileId = Guid.NewGuid();
            
            var defaultProfile = AccessProfile.CreateDefaultMedicalProfile(tenantId, clinicId);
            var originalPermissions = defaultProfile.GetPermissionKeys().ToList();
            
            var updateDto = new UpdateAccessProfileDto
            {
                Name = "Médico Customizado",
                Description = "Perfil médico customizado",
                Permissions = null // No permissions provided, should copy from original
            };

            _mockProfileRepository.Setup(r => r.GetByIdAsync(profileId, tenantId))
                .ReturnsAsync(defaultProfile);

            _mockProfileRepository.Setup(r => r.AddAsync(It.IsAny<AccessProfile>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _profileService.UpdateAsync(profileId, updateDto, tenantId);

            // Assert
            Assert.NotEmpty(result.Permissions);
            // Should have same number of permissions as the original
            Assert.Equal(originalPermissions.Count, result.Permissions.Count);
        }

        [Fact]
        public async Task UpdateAsync_WithDefaultProfileWithoutClinic_ThrowsException()
        {
            // Arrange
            var tenantId = "test-tenant";
            var profileId = Guid.NewGuid();
            
            // Create a default profile without a clinic (system-wide default)
            var defaultProfile = new AccessProfile(
                "System Default",
                "A system-wide default profile",
                tenantId,
                clinicId: null, // No clinic
                isDefault: true
            );
            
            var updateDto = new UpdateAccessProfileDto
            {
                Name = "Updated Profile",
                Description = "Updated description",
                Permissions = new List<string> { "patients.view" }
            };

            _mockProfileRepository.Setup(r => r.GetByIdAsync(profileId, tenantId))
                .ReturnsAsync(defaultProfile);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _profileService.UpdateAsync(profileId, updateDto, tenantId)
            );
            
            Assert.Equal("Cannot modify default profiles without a clinic context", exception.Message);
        }

        [Fact]
        public async Task UpdateAsync_WithConcurrencyException_RetriesSuccessfully()
        {
            // Arrange
            var tenantId = "test-tenant";
            var clinicId = Guid.NewGuid();
            var profileId = Guid.NewGuid();
            
            var customProfile = new AccessProfile(
                "Perfil Personalizado",
                "Um perfil customizado",
                tenantId,
                clinicId,
                isDefault: false
            );
            
            var updateDto = new UpdateAccessProfileDto
            {
                Name = "Perfil Atualizado",
                Description = "Descrição atualizada",
                Permissions = new List<string> { "patients.view" }
            };

            var callCount = 0;
            _mockProfileRepository.Setup(r => r.GetByIdAsync(profileId, tenantId))
                .ReturnsAsync(customProfile);

            _mockProfileRepository.Setup(r => r.UpdateAsync(It.IsAny<AccessProfile>()))
                .Returns(() =>
                {
                    callCount++;
                    if (callCount == 1)
                    {
                        // First attempt fails with concurrency exception
                        throw new Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException("Concurrency conflict");
                    }
                    // Second attempt succeeds
                    return Task.CompletedTask;
                });

            // Act
            var result = await _profileService.UpdateAsync(profileId, updateDto, tenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(updateDto.Name, result.Name);
            
            // Verify that UpdateAsync was called twice (first failed, second succeeded)
            _mockProfileRepository.Verify(r => r.UpdateAsync(It.IsAny<AccessProfile>()), Times.Exactly(2));
            
            // Verify that GetByIdAsync was called twice (initial load + retry)
            _mockProfileRepository.Verify(r => r.GetByIdAsync(profileId, tenantId), Times.Exactly(2));
        }

        [Fact]
        public async Task UpdateAsync_WithConcurrencyException_FailsAfterMaxRetries()
        {
            // Arrange
            var tenantId = "test-tenant";
            var clinicId = Guid.NewGuid();
            var profileId = Guid.NewGuid();
            
            var customProfile = new AccessProfile(
                "Perfil Personalizado",
                "Um perfil customizado",
                tenantId,
                clinicId,
                isDefault: false
            );
            
            var updateDto = new UpdateAccessProfileDto
            {
                Name = "Perfil Atualizado",
                Description = "Descrição atualizada",
                Permissions = new List<string> { "patients.view" }
            };

            _mockProfileRepository.Setup(r => r.GetByIdAsync(profileId, tenantId))
                .ReturnsAsync(customProfile);

            // Always throw concurrency exception
            _mockProfileRepository.Setup(r => r.UpdateAsync(It.IsAny<AccessProfile>()))
                .ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException("Concurrency conflict"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _profileService.UpdateAsync(profileId, updateDto, tenantId)
            );
            
            Assert.Contains("Unable to save profile changes", exception.Message);
            Assert.Contains("modified by another user", exception.Message);
            
            // Verify that UpdateAsync was called 3 times (max retries)
            _mockProfileRepository.Verify(r => r.UpdateAsync(It.IsAny<AccessProfile>()), Times.Exactly(3));
            
            // Verify that GetByIdAsync was called 3 times (initial + 2 retries)
            _mockProfileRepository.Verify(r => r.GetByIdAsync(profileId, tenantId), Times.Exactly(3));
        }

        [Fact]
        public async Task SetConsultationFormProfileAsync_WithConcurrencyException_RetriesSuccessfully()
        {
            // Arrange
            var tenantId = "test-tenant";
            var clinicId = Guid.NewGuid();
            var profileId = Guid.NewGuid();
            var consultationFormProfileId = Guid.NewGuid();
            
            var customProfile = new AccessProfile(
                "Perfil Personalizado",
                "Um perfil customizado",
                tenantId,
                clinicId,
                isDefault: false
            );

            var formProfile = new ConsultationFormProfile(
                "Form Profile",
                "Test form profile",
                Domain.Enums.ProfessionalSpecialty.Medico,
                "system"
            );

            var callCount = 0;
            _mockProfileRepository.Setup(r => r.GetByIdAsync(profileId, tenantId))
                .ReturnsAsync(customProfile);

            _mockConsultationFormProfileRepository
                .Setup(r => r.GetAllQueryable())
                .Returns(new[] { formProfile }.AsQueryable());

            _mockProfileRepository.Setup(r => r.UpdateAsync(It.IsAny<AccessProfile>()))
                .Returns(() =>
                {
                    callCount++;
                    if (callCount == 1)
                    {
                        // First attempt fails with concurrency exception
                        throw new Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException("Concurrency conflict");
                    }
                    // Second attempt succeeds
                    return Task.CompletedTask;
                });

            // Act
            var result = await _profileService.SetConsultationFormProfileAsync(profileId, consultationFormProfileId, tenantId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(consultationFormProfileId, result.ConsultationFormProfileId);
            
            // Verify that UpdateAsync was called twice (first failed, second succeeded)
            _mockProfileRepository.Verify(r => r.UpdateAsync(It.IsAny<AccessProfile>()), Times.Exactly(2));
            
            // Verify that GetByIdAsync was called twice (initial load + retry)
            _mockProfileRepository.Verify(r => r.GetByIdAsync(profileId, tenantId), Times.Exactly(2));
        }

        [Fact]
        public async Task SetConsultationFormProfileAsync_WithConcurrencyException_FailsAfterMaxRetries()
        {
            // Arrange
            var tenantId = "test-tenant";
            var clinicId = Guid.NewGuid();
            var profileId = Guid.NewGuid();
            var consultationFormProfileId = Guid.NewGuid();
            
            var customProfile = new AccessProfile(
                "Perfil Personalizado",
                "Um perfil customizado",
                tenantId,
                clinicId,
                isDefault: false
            );

            var formProfile = new ConsultationFormProfile(
                "Form Profile",
                "Test form profile",
                Domain.Enums.ProfessionalSpecialty.Medico,
                "system"
            );

            _mockProfileRepository.Setup(r => r.GetByIdAsync(profileId, tenantId))
                .ReturnsAsync(customProfile);

            _mockConsultationFormProfileRepository
                .Setup(r => r.GetAllQueryable())
                .Returns(new[] { formProfile }.AsQueryable());

            // Always throw concurrency exception
            _mockProfileRepository.Setup(r => r.UpdateAsync(It.IsAny<AccessProfile>()))
                .ThrowsAsync(new Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException("Concurrency conflict"));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _profileService.SetConsultationFormProfileAsync(profileId, consultationFormProfileId, tenantId)
            );
            
            Assert.Contains("Unable to save profile changes", exception.Message);
            Assert.Contains("modified by another user", exception.Message);
            
            // Verify that UpdateAsync was called 3 times (max retries)
            _mockProfileRepository.Verify(r => r.UpdateAsync(It.IsAny<AccessProfile>()), Times.Exactly(3));
        }
    }
}
