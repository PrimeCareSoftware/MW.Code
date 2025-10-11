using System;
using MedicSoft.Domain.Entities;
using Xunit;

namespace MedicSoft.Test.Entities
{
    public class ProcedureTests
    {
        private const string TestTenantId = "test-tenant";

        [Fact]
        public void Constructor_WithValidData_CreatesProcedure()
        {
            // Arrange & Act
            var procedure = new Procedure(
                "Consulta Geral",
                "CONS-001",
                "Consulta médica geral",
                ProcedureCategory.Consultation,
                150.00m,
                30,
                TestTenantId,
                false
            );

            // Assert
            Assert.Equal("Consulta Geral", procedure.Name);
            Assert.Equal("CONS-001", procedure.Code);
            Assert.Equal("Consulta médica geral", procedure.Description);
            Assert.Equal(ProcedureCategory.Consultation, procedure.Category);
            Assert.Equal(150.00m, procedure.Price);
            Assert.Equal(30, procedure.DurationMinutes);
            Assert.False(procedure.RequiresMaterials);
            Assert.True(procedure.IsActive);
        }

        [Fact]
        public void Constructor_WithEmptyName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Procedure("", "CODE-001", "Description", ProcedureCategory.Consultation,
                    100.00m, 30, TestTenantId));
        }

        [Fact]
        public void Constructor_WithWhitespaceName_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Procedure("   ", "CODE-001", "Description", ProcedureCategory.Consultation,
                    100.00m, 30, TestTenantId));
        }

        [Fact]
        public void Constructor_WithEmptyCode_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Procedure("Procedure Name", "", "Description", ProcedureCategory.Consultation,
                    100.00m, 30, TestTenantId));
        }

        [Fact]
        public void Constructor_WithNegativePrice_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Procedure("Procedure Name", "CODE-001", "Description", ProcedureCategory.Consultation,
                    -10.00m, 30, TestTenantId));
        }

        [Fact]
        public void Constructor_WithZeroDuration_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Procedure("Procedure Name", "CODE-001", "Description", ProcedureCategory.Consultation,
                    100.00m, 0, TestTenantId));
        }

        [Fact]
        public void Constructor_WithNegativeDuration_ThrowsArgumentException()
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Procedure("Procedure Name", "CODE-001", "Description", ProcedureCategory.Consultation,
                    100.00m, -30, TestTenantId));
        }

        [Fact]
        public void Update_WithValidData_UpdatesProcedure()
        {
            // Arrange
            var procedure = CreateValidProcedure();

            // Act
            procedure.Update(
                "Updated Name",
                "Updated Description",
                ProcedureCategory.Surgery,
                250.00m,
                60,
                true
            );

            // Assert
            Assert.Equal("Updated Name", procedure.Name);
            Assert.Equal("Updated Description", procedure.Description);
            Assert.Equal(ProcedureCategory.Surgery, procedure.Category);
            Assert.Equal(250.00m, procedure.Price);
            Assert.Equal(60, procedure.DurationMinutes);
            Assert.True(procedure.RequiresMaterials);
            Assert.NotNull(procedure.UpdatedAt);
        }

        [Fact]
        public void Update_WithEmptyName_ThrowsArgumentException()
        {
            // Arrange
            var procedure = CreateValidProcedure();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                procedure.Update("", "Description", ProcedureCategory.Consultation, 100.00m, 30, false));
        }

        [Fact]
        public void Update_WithNegativePrice_ThrowsArgumentException()
        {
            // Arrange
            var procedure = CreateValidProcedure();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                procedure.Update("Name", "Description", ProcedureCategory.Consultation, -10.00m, 30, false));
        }

        [Fact]
        public void Update_WithInvalidDuration_ThrowsArgumentException()
        {
            // Arrange
            var procedure = CreateValidProcedure();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                procedure.Update("Name", "Description", ProcedureCategory.Consultation, 100.00m, 0, false));
        }

        [Fact]
        public void Activate_SetsIsActiveToTrue()
        {
            // Arrange
            var procedure = CreateValidProcedure();
            procedure.Deactivate();

            // Act
            procedure.Activate();

            // Assert
            Assert.True(procedure.IsActive);
            Assert.NotNull(procedure.UpdatedAt);
        }

        [Fact]
        public void Deactivate_SetsIsActiveToFalse()
        {
            // Arrange
            var procedure = CreateValidProcedure();

            // Act
            procedure.Deactivate();

            // Assert
            Assert.False(procedure.IsActive);
            Assert.NotNull(procedure.UpdatedAt);
        }

        private Procedure CreateValidProcedure()
        {
            return new Procedure(
                "Test Procedure",
                "TEST-001",
                "Test Description",
                ProcedureCategory.Consultation,
                100.00m,
                30,
                TestTenantId,
                false
            );
        }
    }
}
