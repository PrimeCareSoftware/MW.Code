using System;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class PrescriptionItemTests
    {
        private readonly string _tenantId = "test-tenant";
        private readonly Guid _medicalRecordId = Guid.NewGuid();
        private readonly Guid _medicationId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidData_CreatesPrescriptionItem()
        {
            // Arrange
            var dosage = "1 comprimido";
            var frequency = "3x ao dia";
            var durationDays = 7;
            var quantity = 21;
            var instructions = "Tomar após as refeições";

            // Act
            var item = new PrescriptionItem(
                _medicalRecordId, _medicationId, dosage, frequency,
                durationDays, quantity, _tenantId, instructions);

            // Assert
            Assert.NotEqual(Guid.Empty, item.Id);
            Assert.Equal(_medicalRecordId, item.MedicalRecordId);
            Assert.Equal(_medicationId, item.MedicationId);
            Assert.Equal(dosage, item.Dosage);
            Assert.Equal(frequency, item.Frequency);
            Assert.Equal(durationDays, item.DurationDays);
            Assert.Equal(quantity, item.Quantity);
            Assert.Equal(instructions, item.Instructions);
        }

        [Fact]
        public void Constructor_WithoutInstructions_CreatesPrescriptionItem()
        {
            // Arrange & Act
            var item = new PrescriptionItem(
                _medicalRecordId, _medicationId, "1 comprimido", "2x ao dia",
                10, 20, _tenantId);

            // Assert
            Assert.NotEqual(Guid.Empty, item.Id);
            Assert.Null(item.Instructions);
        }

        [Fact]
        public void Constructor_WithEmptyMedicalRecordId_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new PrescriptionItem(Guid.Empty, _medicationId, "1 comp", "2x dia", 7, 14, _tenantId));
        }

        [Fact]
        public void Constructor_WithEmptyMedicationId_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new PrescriptionItem(_medicalRecordId, Guid.Empty, "1 comp", "2x dia", 7, 14, _tenantId));
        }

        [Fact]
        public void Constructor_WithEmptyDosage_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new PrescriptionItem(_medicalRecordId, _medicationId, "", "2x dia", 7, 14, _tenantId));
        }

        [Fact]
        public void Constructor_WithEmptyFrequency_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new PrescriptionItem(_medicalRecordId, _medicationId, "1 comp", "", 7, 14, _tenantId));
        }

        [Fact]
        public void Constructor_WithZeroDuration_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new PrescriptionItem(_medicalRecordId, _medicationId, "1 comp", "2x dia", 0, 14, _tenantId));
        }

        [Fact]
        public void Constructor_WithNegativeDuration_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new PrescriptionItem(_medicalRecordId, _medicationId, "1 comp", "2x dia", -5, 14, _tenantId));
        }

        [Fact]
        public void Constructor_WithZeroQuantity_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new PrescriptionItem(_medicalRecordId, _medicationId, "1 comp", "2x dia", 7, 0, _tenantId));
        }

        [Fact]
        public void Constructor_WithNegativeQuantity_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new PrescriptionItem(_medicalRecordId, _medicationId, "1 comp", "2x dia", 7, -10, _tenantId));
        }

        [Fact]
        public void Update_WithValidData_UpdatesPrescriptionItem()
        {
            // Arrange
            var item = CreateValidPrescriptionItem();
            var newDosage = "2 comprimidos";
            var newFrequency = "4x ao dia";
            var newDuration = 14;
            var newQuantity = 56;
            var newInstructions = "Tomar com água";

            // Act
            item.Update(newDosage, newFrequency, newDuration, newQuantity, newInstructions);

            // Assert
            Assert.Equal(newDosage, item.Dosage);
            Assert.Equal(newFrequency, item.Frequency);
            Assert.Equal(newDuration, item.DurationDays);
            Assert.Equal(newQuantity, item.Quantity);
            Assert.Equal(newInstructions, item.Instructions);
            Assert.NotNull(item.UpdatedAt);
        }

        [Fact]
        public void Update_WithEmptyDosage_ThrowsArgumentException()
        {
            // Arrange
            var item = CreateValidPrescriptionItem();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                item.Update("", "2x dia", 7, 14));
        }

        [Fact]
        public void Update_WithZeroDuration_ThrowsArgumentException()
        {
            // Arrange
            var item = CreateValidPrescriptionItem();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                item.Update("1 comp", "2x dia", 0, 14));
        }

        [Fact]
        public void Constructor_TrimsWhitespaceFromStrings()
        {
            // Arrange & Act
            var item = new PrescriptionItem(
                _medicalRecordId, _medicationId,
                "  1 comprimido  ", "  2x ao dia  ",
                7, 14, _tenantId, "  Tomar com água  ");

            // Assert
            Assert.Equal("1 comprimido", item.Dosage);
            Assert.Equal("2x ao dia", item.Frequency);
            Assert.Equal("Tomar com água", item.Instructions);
        }

        [Fact]
        public void Update_TrimsWhitespaceFromStrings()
        {
            // Arrange
            var item = CreateValidPrescriptionItem();

            // Act
            item.Update("  2 comp  ", "  3x dia  ", 10, 30, "  Nova instrução  ");

            // Assert
            Assert.Equal("2 comp", item.Dosage);
            Assert.Equal("3x dia", item.Frequency);
            Assert.Equal("Nova instrução", item.Instructions);
        }

        private PrescriptionItem CreateValidPrescriptionItem()
        {
            return new PrescriptionItem(
                _medicalRecordId, _medicationId,
                "1 comprimido", "2x ao dia",
                7, 14, _tenantId);
        }
    }
}
