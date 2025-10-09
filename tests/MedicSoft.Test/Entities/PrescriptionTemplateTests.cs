using System;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class PrescriptionTemplateTests
    {
        private readonly string _tenantId = "test-tenant";

        [Fact]
        public void Constructor_WithValidData_CreatesTemplate()
        {
            // Arrange
            var name = "Antibiotic Prescription";
            var description = "Standard antibiotic prescription template";
            var content = "Medication: {{medication}}\nDosage: {{dosage}}";
            var category = "Antibiotics";

            // Act
            var template = new PrescriptionTemplate(name, description, content, category, _tenantId);

            // Assert
            Assert.NotEqual(Guid.Empty, template.Id);
            Assert.Equal(name, template.Name);
            Assert.Equal(description, template.Description);
            Assert.Equal(content, template.TemplateContent);
            Assert.Equal(category, template.Category);
            Assert.True(template.IsActive);
            Assert.Equal(_tenantId, template.TenantId);
        }

        [Fact]
        public void Constructor_WithEmptyName_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new PrescriptionTemplate("", "Description", "Content", "Category", _tenantId));

            Assert.Equal("Name cannot be empty (Parameter 'name')", exception.Message);
        }

        [Fact]
        public void Constructor_WithWhitespaceName_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new PrescriptionTemplate("   ", "Description", "Content", "Category", _tenantId));

            Assert.Equal("Name cannot be empty (Parameter 'name')", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyTemplateContent_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new PrescriptionTemplate("Name", "Description", "", "Category", _tenantId));

            Assert.Equal("Template content cannot be empty (Parameter 'templateContent')", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyCategory_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new PrescriptionTemplate("Name", "Description", "Content", "", _tenantId));

            Assert.Equal("Category cannot be empty (Parameter 'category')", exception.Message);
        }

        [Fact]
        public void Constructor_TrimsWhitespace()
        {
            // Arrange
            var name = "  Template Name  ";
            var description = "  Description  ";
            var content = "  Content  ";
            var category = "  Category  ";

            // Act
            var template = new PrescriptionTemplate(name, description, content, category, _tenantId);

            // Assert
            Assert.Equal("Template Name", template.Name);
            Assert.Equal("Description", template.Description);
            Assert.Equal("Content", template.TemplateContent);
            Assert.Equal("Category", template.Category);
        }

        [Fact]
        public void Constructor_WithNullDescription_SetsEmptyString()
        {
            // Act
            var template = new PrescriptionTemplate("Name", null!, "Content", "Category", _tenantId);

            // Assert
            Assert.Equal(string.Empty, template.Description);
        }

        [Fact]
        public void Update_WithValidData_UpdatesTemplate()
        {
            // Arrange
            var template = CreateValidTemplate();
            var newName = "Updated Template";
            var newDescription = "Updated description";
            var newContent = "Updated content";
            var newCategory = "Updated";

            // Act
            template.Update(newName, newDescription, newContent, newCategory);

            // Assert
            Assert.Equal(newName, template.Name);
            Assert.Equal(newDescription, template.Description);
            Assert.Equal(newContent, template.TemplateContent);
            Assert.Equal(newCategory, template.Category);
            Assert.NotNull(template.UpdatedAt);
        }

        [Fact]
        public void Update_WithEmptyName_ThrowsArgumentException()
        {
            // Arrange
            var template = CreateValidTemplate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                template.Update("", "Description", "Content", "Category"));

            Assert.Equal("Name cannot be empty (Parameter 'name')", exception.Message);
        }

        [Fact]
        public void Update_WithEmptyContent_ThrowsArgumentException()
        {
            // Arrange
            var template = CreateValidTemplate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                template.Update("Name", "Description", "", "Category"));

            Assert.Equal("Template content cannot be empty (Parameter 'templateContent')", exception.Message);
        }

        [Fact]
        public void Update_WithEmptyCategory_ThrowsArgumentException()
        {
            // Arrange
            var template = CreateValidTemplate();

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                template.Update("Name", "Description", "Content", ""));

            Assert.Equal("Category cannot be empty (Parameter 'category')", exception.Message);
        }

        [Fact]
        public void Deactivate_SetsTemplateToInactive()
        {
            // Arrange
            var template = CreateValidTemplate();
            Assert.True(template.IsActive);

            // Act
            template.Deactivate();

            // Assert
            Assert.False(template.IsActive);
            Assert.NotNull(template.UpdatedAt);
        }

        [Fact]
        public void Activate_SetsTemplateToActive()
        {
            // Arrange
            var template = CreateValidTemplate();
            template.Deactivate();
            Assert.False(template.IsActive);

            // Act
            template.Activate();

            // Assert
            Assert.True(template.IsActive);
            Assert.NotNull(template.UpdatedAt);
        }

        [Fact]
        public void Constructor_NewTemplateIsActiveByDefault()
        {
            // Act
            var template = CreateValidTemplate();

            // Assert
            Assert.True(template.IsActive);
        }

        private PrescriptionTemplate CreateValidTemplate()
        {
            return new PrescriptionTemplate(
                "Test Template",
                "Test Description",
                "Test Content",
                "Test Category",
                _tenantId
            );
        }
    }
}
