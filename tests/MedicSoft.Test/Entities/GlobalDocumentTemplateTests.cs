using System;
using Xunit;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Test.Entities
{
    public class GlobalDocumentTemplateTests
    {
        private readonly string _tenantId = "test-tenant";
        private readonly string _createdBy = "admin-user-id";

        [Fact]
        public void Constructor_WithValidData_CreatesTemplate()
        {
            // Arrange
            var name = "Medical Certificate Template";
            var description = "Standard medical certificate template";
            var type = DocumentTemplateType.MedicalCertificate;
            var specialty = ProfessionalSpecialty.Medico;
            var content = "<p>Medical Certificate Content</p>";
            var variables = "{\"patient\":[\"name\",\"cpf\"],\"professional\":[\"name\",\"registration\"]}";

            // Act
            var template = new GlobalDocumentTemplate(
                name, 
                description, 
                type, 
                specialty, 
                content, 
                variables, 
                _tenantId, 
                _createdBy);

            // Assert
            Assert.NotEqual(Guid.Empty, template.Id);
            Assert.Equal(name, template.Name);
            Assert.Equal(description, template.Description);
            Assert.Equal(type, template.Type);
            Assert.Equal(specialty, template.Specialty);
            Assert.Equal(content, template.Content);
            Assert.Equal(variables, template.Variables);
            Assert.Equal(_createdBy, template.CreatedBy);
            Assert.True(template.IsActive);
            Assert.Equal(_tenantId, template.TenantId);
        }

        [Fact]
        public void Constructor_WithEmptyName_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new GlobalDocumentTemplate(
                    "", 
                    "Description", 
                    DocumentTemplateType.MedicalCertificate, 
                    ProfessionalSpecialty.Medico, 
                    "Content", 
                    "{}", 
                    _tenantId, 
                    _createdBy));

            Assert.Equal("Name cannot be empty (Parameter 'name')", exception.Message);
        }

        [Fact]
        public void Constructor_WithWhitespaceName_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new GlobalDocumentTemplate(
                    "   ", 
                    "Description", 
                    DocumentTemplateType.Prescription, 
                    ProfessionalSpecialty.Medico, 
                    "Content", 
                    "{}", 
                    _tenantId, 
                    _createdBy));

            Assert.Equal("Name cannot be empty (Parameter 'name')", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyDescription_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new GlobalDocumentTemplate(
                    "Name", 
                    "", 
                    DocumentTemplateType.MedicalCertificate, 
                    ProfessionalSpecialty.Medico, 
                    "Content", 
                    "{}", 
                    _tenantId, 
                    _createdBy));

            Assert.Equal("Description cannot be empty (Parameter 'description')", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyContent_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new GlobalDocumentTemplate(
                    "Name", 
                    "Description", 
                    DocumentTemplateType.MedicalCertificate, 
                    ProfessionalSpecialty.Medico, 
                    "", 
                    "{}", 
                    _tenantId, 
                    _createdBy));

            Assert.Equal("Content cannot be empty (Parameter 'content')", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyVariables_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new GlobalDocumentTemplate(
                    "Name", 
                    "Description", 
                    DocumentTemplateType.MedicalCertificate, 
                    ProfessionalSpecialty.Medico, 
                    "Content", 
                    "", 
                    _tenantId, 
                    _createdBy));

            Assert.Equal("Variables cannot be empty (Parameter 'variables')", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyCreatedBy_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new GlobalDocumentTemplate(
                    "Name", 
                    "Description", 
                    DocumentTemplateType.MedicalCertificate, 
                    ProfessionalSpecialty.Medico, 
                    "Content", 
                    "{}", 
                    _tenantId, 
                    ""));

            Assert.Equal("CreatedBy cannot be empty (Parameter 'createdBy')", exception.Message);
        }

        [Fact]
        public void Constructor_TrimsWhitespace()
        {
            // Arrange
            var name = "  Template Name  ";
            var description = "  Description  ";
            var content = "  Content  ";
            var variables = "  {}  ";
            var createdBy = "  admin-id  ";

            // Act
            var template = new GlobalDocumentTemplate(
                name, 
                description, 
                DocumentTemplateType.MedicalCertificate, 
                ProfessionalSpecialty.Medico, 
                content, 
                variables, 
                _tenantId, 
                createdBy);

            // Assert
            Assert.Equal(name.Trim(), template.Name);
            Assert.Equal(description.Trim(), template.Description);
            Assert.Equal(content.Trim(), template.Content);
            Assert.Equal(variables.Trim(), template.Variables);
            Assert.Equal(createdBy.Trim(), template.CreatedBy);
        }

        [Fact]
        public void Constructor_SetsIsActiveToTrue()
        {
            // Act
            var template = new GlobalDocumentTemplate(
                "Name", 
                "Description", 
                DocumentTemplateType.MedicalCertificate, 
                ProfessionalSpecialty.Medico, 
                "Content", 
                "{}", 
                _tenantId, 
                _createdBy);

            // Assert
            Assert.True(template.IsActive);
        }

        [Fact]
        public void Update_WithValidData_UpdatesTemplate()
        {
            // Arrange
            var template = new GlobalDocumentTemplate(
                "Original Name", 
                "Original Description", 
                DocumentTemplateType.MedicalCertificate, 
                ProfessionalSpecialty.Medico, 
                "Original Content", 
                "{\"old\":true}", 
                _tenantId, 
                _createdBy);

            var newName = "Updated Name";
            var newDescription = "Updated Description";
            var newContent = "Updated Content";
            var newVariables = "{\"new\":true}";

            // Act
            template.Update(newName, newDescription, newContent, newVariables);

            // Assert
            Assert.Equal(newName, template.Name);
            Assert.Equal(newDescription, template.Description);
            Assert.Equal(newContent, template.Content);
            Assert.Equal(newVariables, template.Variables);
        }

        [Fact]
        public void Update_WithEmptyName_ThrowsArgumentException()
        {
            // Arrange
            var template = new GlobalDocumentTemplate(
                "Name", 
                "Description", 
                DocumentTemplateType.MedicalCertificate, 
                ProfessionalSpecialty.Medico, 
                "Content", 
                "{}", 
                _tenantId, 
                _createdBy);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                template.Update("", "Description", "Content", "{}"));

            Assert.Equal("Name cannot be empty (Parameter 'name')", exception.Message);
        }

        [Fact]
        public void Update_WithEmptyDescription_ThrowsArgumentException()
        {
            // Arrange
            var template = new GlobalDocumentTemplate(
                "Name", 
                "Description", 
                DocumentTemplateType.MedicalCertificate, 
                ProfessionalSpecialty.Medico, 
                "Content", 
                "{}", 
                _tenantId, 
                _createdBy);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                template.Update("Name", "", "Content", "{}"));

            Assert.Equal("Description cannot be empty (Parameter 'description')", exception.Message);
        }

        [Fact]
        public void Update_WithEmptyContent_ThrowsArgumentException()
        {
            // Arrange
            var template = new GlobalDocumentTemplate(
                "Name", 
                "Description", 
                DocumentTemplateType.MedicalCertificate, 
                ProfessionalSpecialty.Medico, 
                "Content", 
                "{}", 
                _tenantId, 
                _createdBy);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                template.Update("Name", "Description", "", "{}"));

            Assert.Equal("Content cannot be empty (Parameter 'content')", exception.Message);
        }

        [Fact]
        public void Update_WithEmptyVariables_ThrowsArgumentException()
        {
            // Arrange
            var template = new GlobalDocumentTemplate(
                "Name", 
                "Description", 
                DocumentTemplateType.MedicalCertificate, 
                ProfessionalSpecialty.Medico, 
                "Content", 
                "{}", 
                _tenantId, 
                _createdBy);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                template.Update("Name", "Description", "Content", ""));

            Assert.Equal("Variables cannot be empty (Parameter 'variables')", exception.Message);
        }

        [Fact]
        public void Update_TrimsWhitespace()
        {
            // Arrange
            var template = new GlobalDocumentTemplate(
                "Name", 
                "Description", 
                DocumentTemplateType.MedicalCertificate, 
                ProfessionalSpecialty.Medico, 
                "Content", 
                "{}", 
                _tenantId, 
                _createdBy);

            var newName = "  Updated Name  ";
            var newDescription = "  Updated Description  ";
            var newContent = "  Updated Content  ";
            var newVariables = "  {\"updated\":true}  ";

            // Act
            template.Update(newName, newDescription, newContent, newVariables);

            // Assert
            Assert.Equal(newName.Trim(), template.Name);
            Assert.Equal(newDescription.Trim(), template.Description);
            Assert.Equal(newContent.Trim(), template.Content);
            Assert.Equal(newVariables.Trim(), template.Variables);
        }

        [Fact]
        public void SetActiveStatus_FromFalseToTrue_UpdatesIsActive()
        {
            // Arrange
            var template = new GlobalDocumentTemplate(
                "Name", 
                "Description", 
                DocumentTemplateType.MedicalCertificate, 
                ProfessionalSpecialty.Medico, 
                "Content", 
                "{}", 
                _tenantId, 
                _createdBy);

            template.SetActiveStatus(false);
            Assert.False(template.IsActive);

            // Act
            template.SetActiveStatus(true);

            // Assert
            Assert.True(template.IsActive);
        }

        [Fact]
        public void SetActiveStatus_ToFalse_UpdatesIsActive()
        {
            // Arrange
            var template = new GlobalDocumentTemplate(
                "Name", 
                "Description", 
                DocumentTemplateType.MedicalCertificate, 
                ProfessionalSpecialty.Medico, 
                "Content", 
                "{}", 
                _tenantId, 
                _createdBy);

            // Initially true
            Assert.True(template.IsActive);

            // Act
            template.SetActiveStatus(false);

            // Assert
            Assert.False(template.IsActive);
        }

        [Fact]
        public void Constructor_WithDifferentTypes_CreatesCorrectTemplate()
        {
            // Arrange & Act
            var prescription = new GlobalDocumentTemplate(
                "Prescription", 
                "Description", 
                DocumentTemplateType.Prescription, 
                ProfessionalSpecialty.Medico, 
                "Content", 
                "{}", 
                _tenantId, 
                _createdBy);

            var certificate = new GlobalDocumentTemplate(
                "Certificate", 
                "Description", 
                DocumentTemplateType.MedicalCertificate, 
                ProfessionalSpecialty.Medico, 
                "Content", 
                "{}", 
                _tenantId, 
                _createdBy);

            // Assert
            Assert.Equal(DocumentTemplateType.Prescription, prescription.Type);
            Assert.Equal(DocumentTemplateType.MedicalCertificate, certificate.Type);
        }

        [Fact]
        public void Constructor_WithDifferentSpecialties_CreatesCorrectTemplate()
        {
            // Arrange & Act
            var medico = new GlobalDocumentTemplate(
                "Template", 
                "Description", 
                DocumentTemplateType.MedicalCertificate, 
                ProfessionalSpecialty.Medico, 
                "Content", 
                "{}", 
                _tenantId, 
                _createdBy);

            var psicologo = new GlobalDocumentTemplate(
                "Template", 
                "Description", 
                DocumentTemplateType.MedicalCertificate, 
                ProfessionalSpecialty.Psicologo, 
                "Content", 
                "{}", 
                _tenantId, 
                _createdBy);

            // Assert
            Assert.Equal(ProfessionalSpecialty.Medico, medico.Specialty);
            Assert.Equal(ProfessionalSpecialty.Psicologo, psicologo.Specialty);
        }
    }
}
