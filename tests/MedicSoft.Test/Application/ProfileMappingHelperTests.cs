using Xunit;
using MedicSoft.Application.Helpers;
using MedicSoft.Domain.Entities;
using Microsoft.Extensions.Logging;
using Moq;

namespace MedicSoft.Test.Application
{
    public class ProfileMappingHelperTests
    {
        private readonly Mock<ILogger> _loggerMock;

        public ProfileMappingHelperTests()
        {
            _loggerMock = new Mock<ILogger>();
        }

        [Theory]
        [InlineData("Psicólogo", UserRole.Psychologist)]
        [InlineData("psicologo", UserRole.Psychologist)]
        [InlineData("Psychologist", UserRole.Psychologist)]
        [InlineData("psychologist", UserRole.Psychologist)]
        public void MapProfileNameToRole_WithPsychologistVariants_ReturnsPsychologistRole(string profileName, UserRole expectedRole)
        {
            // Act
            var result = ProfileMappingHelper.MapProfileNameToRole(profileName, _loggerMock.Object);

            // Assert
            Assert.Equal(expectedRole, result);
        }

        [Theory]
        [InlineData("Médico", UserRole.Doctor)]
        [InlineData("medico", UserRole.Doctor)]
        [InlineData("Doctor", UserRole.Doctor)]
        [InlineData("doctor", UserRole.Doctor)]
        public void MapProfileNameToRole_WithDoctorVariants_ReturnsDoctorRole(string profileName, UserRole expectedRole)
        {
            // Act
            var result = ProfileMappingHelper.MapProfileNameToRole(profileName, _loggerMock.Object);

            // Assert
            Assert.Equal(expectedRole, result);
        }

        [Theory]
        [InlineData("Dentista", UserRole.Dentist)]
        [InlineData("dentista", UserRole.Dentist)]
        [InlineData("Dentist", UserRole.Dentist)]
        public void MapProfileNameToRole_WithDentistVariants_ReturnsDentistRole(string profileName, UserRole expectedRole)
        {
            // Act
            var result = ProfileMappingHelper.MapProfileNameToRole(profileName, _loggerMock.Object);

            // Assert
            Assert.Equal(expectedRole, result);
        }

        [Theory]
        [InlineData("Nutricionista", UserRole.Doctor)]
        [InlineData("Nutritionist", UserRole.Doctor)]
        [InlineData("Fisioterapeuta", UserRole.Doctor)]
        [InlineData("Physiotherapist", UserRole.Doctor)]
        [InlineData("Veterinário", UserRole.Doctor)]
        [InlineData("veterinario", UserRole.Doctor)]
        [InlineData("Veterinarian", UserRole.Doctor)]
        public void MapProfileNameToRole_WithOtherProfessionals_ReturnsDoctorRole(string profileName, UserRole expectedRole)
        {
            // Act
            var result = ProfileMappingHelper.MapProfileNameToRole(profileName, _loggerMock.Object);

            // Assert
            Assert.Equal(expectedRole, result);
        }

        [Theory]
        [InlineData("Proprietário", UserRole.ClinicOwner)]
        [InlineData("proprietario", UserRole.ClinicOwner)]
        [InlineData("Owner", UserRole.ClinicOwner)]
        public void MapProfileNameToRole_WithOwnerVariants_ReturnsClinicOwnerRole(string profileName, UserRole expectedRole)
        {
            // Act
            var result = ProfileMappingHelper.MapProfileNameToRole(profileName, _loggerMock.Object);

            // Assert
            Assert.Equal(expectedRole, result);
        }

        [Theory]
        [InlineData("Recepção", UserRole.Receptionist)]
        [InlineData("recepcao", UserRole.Receptionist)]
        [InlineData("Receptionist", UserRole.Receptionist)]
        [InlineData("Recepcionista", UserRole.Receptionist)]
        public void MapProfileNameToRole_WithReceptionistVariants_ReturnsReceptionistRole(string profileName, UserRole expectedRole)
        {
            // Act
            var result = ProfileMappingHelper.MapProfileNameToRole(profileName, _loggerMock.Object);

            // Assert
            Assert.Equal(expectedRole, result);
        }

        [Theory]
        [InlineData("Secretaria", UserRole.Secretary)]
        [InlineData("Secretário", UserRole.Secretary)]
        [InlineData("secretario", UserRole.Secretary)]
        [InlineData("Secretary", UserRole.Secretary)]
        [InlineData("Financeiro", UserRole.Secretary)]
        [InlineData("Financial", UserRole.Secretary)]
        public void MapProfileNameToRole_WithSecretaryVariants_ReturnsSecretaryRole(string profileName, UserRole expectedRole)
        {
            // Act
            var result = ProfileMappingHelper.MapProfileNameToRole(profileName, _loggerMock.Object);

            // Assert
            Assert.Equal(expectedRole, result);
        }

        [Theory]
        [InlineData("Enfermeiro", UserRole.Nurse)]
        [InlineData("Enfermeira", UserRole.Nurse)]
        [InlineData("Nurse", UserRole.Nurse)]
        public void MapProfileNameToRole_WithNurseVariants_ReturnsNurseRole(string profileName, UserRole expectedRole)
        {
            // Act
            var result = ProfileMappingHelper.MapProfileNameToRole(profileName, _loggerMock.Object);

            // Assert
            Assert.Equal(expectedRole, result);
        }

        [Theory]
        [InlineData("Psicólogo")]
        [InlineData("psicologo")]
        [InlineData("Psychologist")]
        [InlineData("Médico")]
        [InlineData("medico")]
        [InlineData("Doctor")]
        [InlineData("Dentista")]
        [InlineData("Nutricionista")]
        [InlineData("Proprietário")]
        [InlineData("Recepção")]
        public void IsRecognizedProfile_WithKnownProfileNames_ReturnsTrue(string profileName)
        {
            // Act
            var result = ProfileMappingHelper.IsRecognizedProfile(profileName);

            // Assert
            Assert.True(result);
        }

        [Theory]
        [InlineData("Unknown Profile")]
        [InlineData("Invalid")]
        [InlineData("")]
        [InlineData(null)]
        public void IsRecognizedProfile_WithUnknownProfileNames_ReturnsFalse(string profileName)
        {
            // Act
            var result = ProfileMappingHelper.IsRecognizedProfile(profileName);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void GetAllowedRolesForCreation_ExcludesSystemAdmin()
        {
            // Act
            var allowedRoles = ProfileMappingHelper.GetAllowedRolesForCreation();

            // Assert
            Assert.DoesNotContain("SystemAdmin", allowedRoles);
            Assert.Contains("Doctor", allowedRoles);
            Assert.Contains("Psychologist", allowedRoles);
            Assert.Contains("Dentist", allowedRoles);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public void MapProfileNameToRole_WithEmptyOrNullInput_ReturnsReceptionistAsDefault(string profileName)
        {
            // Act
            var result = ProfileMappingHelper.MapProfileNameToRole(profileName, _loggerMock.Object);

            // Assert
            Assert.Equal(UserRole.Receptionist, result);
        }
    }
}
