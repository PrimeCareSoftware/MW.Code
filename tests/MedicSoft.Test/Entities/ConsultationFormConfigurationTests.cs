using System;
using System.Collections.Generic;
using Xunit;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Test.Entities
{
    public class ConsultationFormConfigurationTests
    {
        private readonly string _tenantId = "test-tenant";
        private readonly Guid _clinicId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidData_CreatesConfiguration()
        {
            // Arrange
            var configurationName = "Configuração Clínica Principal";

            // Act
            var configuration = new ConsultationFormConfiguration(
                _clinicId,
                configurationName,
                _tenantId
            );

            // Assert
            Assert.NotEqual(Guid.Empty, configuration.Id);
            Assert.Equal(_clinicId, configuration.ClinicId);
            Assert.Equal(configurationName, configuration.ConfigurationName);
            Assert.Null(configuration.ProfileId);
            Assert.True(configuration.IsActive);
            Assert.True(configuration.ShowChiefComplaint);
        }

        [Fact]
        public void Constructor_WithProfileId_CreatesConfigurationLinkedToProfile()
        {
            // Arrange
            var profileId = Guid.NewGuid();
            var configurationName = "Configuração baseada em perfil";

            // Act
            var configuration = new ConsultationFormConfiguration(
                _clinicId,
                configurationName,
                _tenantId,
                profileId
            );

            // Assert
            Assert.Equal(profileId, configuration.ProfileId);
        }

        [Fact]
        public void Constructor_WithCustomFields_CreatesConfigurationWithFields()
        {
            // Arrange
            var customFields = new List<CustomField>
            {
                new CustomField("sintomas_psicologicos", "Sintomas Psicológicos", CustomFieldType.TextoLongo, true, 1),
                new CustomField("escala_ansiedade", "Escala de Ansiedade (1-10)", CustomFieldType.Numero, true, 2)
            };

            // Act
            var configuration = new ConsultationFormConfiguration(
                _clinicId,
                "Configuração Psicologia",
                _tenantId,
                customFields: customFields
            );

            // Assert
            var fields = configuration.GetCustomFields();
            Assert.Equal(2, fields.Count);
            Assert.Equal("sintomas_psicologicos", fields[0].FieldKey);
            Assert.Equal("escala_ansiedade", fields[1].FieldKey);
        }

        [Fact]
        public void Update_WithValidData_UpdatesConfiguration()
        {
            // Arrange
            var configuration = new ConsultationFormConfiguration(
                _clinicId,
                "Configuração Original",
                _tenantId
            );
            var newName = "Configuração Atualizada";

            // Act
            configuration.Update(newName, false, false, true, true, true, true);

            // Assert
            Assert.Equal(newName, configuration.ConfigurationName);
            Assert.False(configuration.ShowChiefComplaint);
            Assert.False(configuration.ShowHistoryOfPresentIllness);
            Assert.True(configuration.ShowPastMedicalHistory);
            Assert.NotNull(configuration.UpdatedAt);
        }

        [Fact]
        public void AddCustomField_WithNewField_AddsField()
        {
            // Arrange
            var configuration = new ConsultationFormConfiguration(
                _clinicId,
                "Configuração Teste",
                _tenantId
            );
            var newField = new CustomField("novo_campo", "Novo Campo", CustomFieldType.TextoSimples, false, 1);

            // Act
            configuration.AddCustomField(newField);

            // Assert
            var fields = configuration.GetCustomFields();
            Assert.Single(fields);
            Assert.Equal("novo_campo", fields[0].FieldKey);
            Assert.NotNull(configuration.UpdatedAt);
        }

        [Fact]
        public void AddCustomField_WithDuplicateFieldKey_ThrowsInvalidOperationException()
        {
            // Arrange
            var configuration = new ConsultationFormConfiguration(
                _clinicId,
                "Configuração Teste",
                _tenantId,
                customFields: new List<CustomField>
                {
                    new CustomField("campo_existente", "Campo Existente", CustomFieldType.TextoSimples, false, 1)
                }
            );
            var duplicateField = new CustomField("campo_existente", "Tentativa Duplicada", CustomFieldType.TextoSimples, false, 2);

            // Act & Assert
            var exception = Assert.Throws<InvalidOperationException>(() =>
                configuration.AddCustomField(duplicateField));

            Assert.Contains("A field with key 'campo_existente' already exists", exception.Message);
        }

        [Fact]
        public void RemoveCustomField_WithExistingField_RemovesField()
        {
            // Arrange
            var configuration = new ConsultationFormConfiguration(
                _clinicId,
                "Configuração Teste",
                _tenantId,
                customFields: new List<CustomField>
                {
                    new CustomField("campo1", "Campo 1", CustomFieldType.TextoSimples, false, 1),
                    new CustomField("campo2", "Campo 2", CustomFieldType.TextoSimples, false, 2)
                }
            );

            // Act
            configuration.RemoveCustomField("campo1");

            // Assert
            var fields = configuration.GetCustomFields();
            Assert.Single(fields);
            Assert.Equal("campo2", fields[0].FieldKey);
            Assert.NotNull(configuration.UpdatedAt);
        }

        [Fact]
        public void UpdateFromProfile_SetsProfileId()
        {
            // Arrange
            var configuration = new ConsultationFormConfiguration(
                _clinicId,
                "Configuração Teste",
                _tenantId
            );
            var profileId = Guid.NewGuid();

            // Act
            configuration.UpdateFromProfile(profileId);

            // Assert
            Assert.Equal(profileId, configuration.ProfileId);
            Assert.NotNull(configuration.UpdatedAt);
        }

        [Fact]
        public void Activate_SetsIsActiveToTrue()
        {
            // Arrange
            var configuration = new ConsultationFormConfiguration(
                _clinicId,
                "Configuração Teste",
                _tenantId
            );
            configuration.Deactivate();

            // Act
            configuration.Activate();

            // Assert
            Assert.True(configuration.IsActive);
        }

        [Fact]
        public void Deactivate_SetsIsActiveToFalse()
        {
            // Arrange
            var configuration = new ConsultationFormConfiguration(
                _clinicId,
                "Configuração Teste",
                _tenantId
            );

            // Act
            configuration.Deactivate();

            // Assert
            Assert.False(configuration.IsActive);
        }

        [Fact]
        public void Constructor_WithEmptyClinicId_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new ConsultationFormConfiguration(Guid.Empty, "Nome", _tenantId));

            Assert.Equal("Clinic ID cannot be empty (Parameter 'clinicId')", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyConfigurationName_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new ConsultationFormConfiguration(_clinicId, "", _tenantId));

            Assert.Equal("Configuration name cannot be empty (Parameter 'configurationName')", exception.Message);
        }
    }
}
