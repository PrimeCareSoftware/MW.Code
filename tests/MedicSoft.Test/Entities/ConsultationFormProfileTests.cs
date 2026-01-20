using System;
using System.Collections.Generic;
using Xunit;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Test.Entities
{
    public class ConsultationFormProfileTests
    {
        private readonly string _tenantId = "test-tenant";

        [Fact]
        public void Constructor_WithValidData_CreatesProfile()
        {
            // Arrange
            var name = "Perfil Médico Geral";
            var description = "Perfil para médicos generalistas";
            var specialty = ProfessionalSpecialty.Medico;

            // Act
            var profile = new ConsultationFormProfile(name, description, specialty, _tenantId);

            // Assert
            Assert.NotEqual(Guid.Empty, profile.Id);
            Assert.Equal(name, profile.Name);
            Assert.Equal(description, profile.Description);
            Assert.Equal(specialty, profile.Specialty);
            Assert.True(profile.IsActive);
            Assert.False(profile.IsSystemDefault);
            Assert.True(profile.ShowChiefComplaint);
            Assert.True(profile.ShowHistoryOfPresentIllness);
        }

        [Fact]
        public void Constructor_WithCustomFields_CreatesProfileWithFields()
        {
            // Arrange
            var customFields = new List<CustomField>
            {
                new CustomField("peso", "Peso", CustomFieldType.Numero, true, 1),
                new CustomField("altura", "Altura", CustomFieldType.Numero, true, 2)
            };

            // Act
            var profile = new ConsultationFormProfile(
                "Perfil Nutricionista",
                "Perfil para nutricionistas",
                ProfessionalSpecialty.Nutricionista,
                _tenantId,
                customFields: customFields
            );

            // Assert
            var fields = profile.GetCustomFields();
            Assert.Equal(2, fields.Count);
            Assert.Equal("peso", fields[0].FieldKey);
            Assert.Equal("altura", fields[1].FieldKey);
        }

        [Fact]
        public void Update_WithValidData_UpdatesProfile()
        {
            // Arrange
            var profile = new ConsultationFormProfile(
                "Perfil Original",
                "Descrição original",
                ProfessionalSpecialty.Medico,
                _tenantId
            );
            var newName = "Perfil Atualizado";
            var newDescription = "Nova descrição";

            // Act
            profile.Update(newName, newDescription, true, true, false, false, false, false);

            // Assert
            Assert.Equal(newName, profile.Name);
            Assert.Equal(newDescription, profile.Description);
            Assert.True(profile.ShowChiefComplaint);
            Assert.False(profile.ShowPastMedicalHistory);
            Assert.NotNull(profile.UpdatedAt);
        }

        [Fact]
        public void Update_OnSystemDefaultProfile_ThrowsInvalidOperationException()
        {
            // Arrange
            var profile = new ConsultationFormProfile(
                "Perfil Sistema",
                "Perfil do sistema",
                ProfessionalSpecialty.Medico,
                _tenantId,
                isSystemDefault: true
            );

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                profile.Update("Novo Nome", "Nova Descrição", true, true, true, true, true, true));

            Assert.Contains("System default profiles cannot be modified", exception.Message);
        }

        [Fact]
        public void Activate_SetsIsActiveToTrue()
        {
            // Arrange
            var profile = new ConsultationFormProfile(
                "Perfil Teste",
                "Descrição",
                ProfessionalSpecialty.Psicologo,
                _tenantId
            );
            profile.Deactivate();

            // Act
            profile.Activate();

            // Assert
            Assert.True(profile.IsActive);
            Assert.NotNull(profile.UpdatedAt);
        }

        [Fact]
        public void Deactivate_OnSystemDefaultProfile_ThrowsInvalidOperationException()
        {
            // Arrange
            var profile = new ConsultationFormProfile(
                "Perfil Sistema",
                "Perfil do sistema",
                ProfessionalSpecialty.Medico,
                _tenantId,
                isSystemDefault: true
            );

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() => profile.Deactivate());
            Assert.Contains("System default profiles cannot be deactivated", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyName_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new ConsultationFormProfile("", "Descrição", ProfessionalSpecialty.Medico, _tenantId));

            Assert.Equal("Name cannot be empty (Parameter 'name')", exception.Message);
        }

        [Fact]
        public void GetCustomFields_ReturnsDeserializedFields()
        {
            // Arrange
            var customFields = new List<CustomField>
            {
                new CustomField("imc", "IMC", CustomFieldType.Numero, false, 1, helpText: "Índice de Massa Corporal")
            };
            var profile = new ConsultationFormProfile(
                "Perfil Teste",
                "Descrição",
                ProfessionalSpecialty.Nutricionista,
                _tenantId,
                customFields: customFields
            );

            // Act
            var retrievedFields = profile.GetCustomFields();

            // Assert
            Assert.Single(retrievedFields);
            Assert.Equal("imc", retrievedFields[0].FieldKey);
            Assert.Equal("IMC", retrievedFields[0].Label);
            Assert.Equal("Índice de Massa Corporal", retrievedFields[0].HelpText);
        }
    }
}
