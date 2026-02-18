using System;
using Xunit;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Test.Entities
{
    public class AccessProfileTests
    {
        private readonly string _tenantId = "test-tenant";
        private readonly Guid _clinicId = Guid.NewGuid();

        [Theory]
        [InlineData("Médico", ProfessionalSpecialty.Medico)]
        [InlineData("Dentista", ProfessionalSpecialty.Dentista)]
        [InlineData("Nutricionista", ProfessionalSpecialty.Nutricionista)]
        [InlineData("Psicólogo", ProfessionalSpecialty.Psicologo)]
        [InlineData("Fisioterapeuta", ProfessionalSpecialty.Fisioterapeuta)]
        [InlineData("Veterinário", ProfessionalSpecialty.Veterinario)]
        [InlineData("Enfermeiro", ProfessionalSpecialty.Enfermeiro)]
        [InlineData("Terapeuta Ocupacional", ProfessionalSpecialty.TerapeutaOcupacional)]
        [InlineData("Fonoaudiólogo", ProfessionalSpecialty.Fonoaudiologo)]
        public void GetProfessionalSpecialtyForProfileName_WithProfessionalProfile_ReturnsCorrectSpecialty(
            string profileName, ProfessionalSpecialty expectedSpecialty)
        {
            // Act
            var result = AccessProfile.GetProfessionalSpecialtyForProfileName(profileName);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedSpecialty, result.Value);
        }

        [Theory]
        [InlineData("Proprietário")]
        [InlineData("Recepção/Secretaria")]
        [InlineData("Financeiro")]
        [InlineData("Unknown Profile")]
        [InlineData("")]
        public void GetProfessionalSpecialtyForProfileName_WithNonProfessionalProfile_ReturnsNull(string profileName)
        {
            // Act
            var result = AccessProfile.GetProfessionalSpecialtyForProfileName(profileName);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public void GetDefaultProfilesForClinicType_ReturnsAllTwelveProfiles()
        {
            // Arrange
            var clinicType = ClinicType.Medical;

            // Act
            var profiles = AccessProfile.GetDefaultProfilesForClinicType(_tenantId, _clinicId, clinicType);

            // Assert
            Assert.Equal(12, profiles.Count);
            
            // Verify common profiles
            Assert.Contains(profiles, p => p.Name == "Proprietário");
            Assert.Contains(profiles, p => p.Name == "Recepção/Secretaria");
            Assert.Contains(profiles, p => p.Name == "Financeiro");
            
            // Verify professional profiles
            Assert.Contains(profiles, p => p.Name == "Médico");
            Assert.Contains(profiles, p => p.Name == "Dentista");
            Assert.Contains(profiles, p => p.Name == "Nutricionista");
            Assert.Contains(profiles, p => p.Name == "Psicólogo");
            Assert.Contains(profiles, p => p.Name == "Fisioterapeuta");
            Assert.Contains(profiles, p => p.Name == "Veterinário");
            Assert.Contains(profiles, p => p.Name == "Enfermeiro");
            Assert.Contains(profiles, p => p.Name == "Terapeuta Ocupacional");
            Assert.Contains(profiles, p => p.Name == "Fonoaudiólogo");
        }

        [Fact]
        public void GetDefaultProfilesForClinicType_AllProfilesHaveCorrectClinicId()
        {
            // Arrange
            var clinicType = ClinicType.Dental;

            // Act
            var profiles = AccessProfile.GetDefaultProfilesForClinicType(_tenantId, _clinicId, clinicType);

            // Assert
            Assert.All(profiles, p => Assert.Equal(_clinicId, p.ClinicId));
        }

        [Fact]
        public void GetDefaultProfilesForClinicType_AllProfilesHaveCorrectTenantId()
        {
            // Arrange
            var clinicType = ClinicType.Nutritionist;

            // Act
            var profiles = AccessProfile.GetDefaultProfilesForClinicType(_tenantId, _clinicId, clinicType);

            // Assert
            Assert.All(profiles, p => Assert.Equal(_tenantId, p.TenantId));
        }

        [Fact]
        public void GetDefaultProfilesForClinicType_AllProfilesAreMarkedAsDefault()
        {
            // Arrange
            var clinicType = ClinicType.Psychology;

            // Act
            var profiles = AccessProfile.GetDefaultProfilesForClinicType(_tenantId, _clinicId, clinicType);

            // Assert
            Assert.All(profiles, p => Assert.True(p.IsDefault));
        }

        [Fact]
        public void IsProfessionalProfile_IdentifiesProfessionalProfilesCorrectly()
        {
            // Arrange
            var medicalProfile = AccessProfile.CreateDefaultMedicalProfile(_tenantId, _clinicId);
            var dentistProfile = AccessProfile.CreateDefaultDentistProfile(_tenantId, _clinicId);
            var ownerProfile = AccessProfile.CreateDefaultOwnerProfile(_tenantId, _clinicId);
            var receptionProfile = AccessProfile.CreateDefaultReceptionProfile(_tenantId, _clinicId);
            var financialProfile = AccessProfile.CreateDefaultFinancialProfile(_tenantId, _clinicId);

            // Act & Assert
            Assert.True(medicalProfile.IsProfessionalProfile());
            Assert.True(dentistProfile.IsProfessionalProfile());
            Assert.False(ownerProfile.IsProfessionalProfile());
            Assert.False(receptionProfile.IsProfessionalProfile());
            Assert.False(financialProfile.IsProfessionalProfile());
        }
    }
}
