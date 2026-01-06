using System;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class MedicationTests
    {
        private readonly string _tenantId = "test-tenant";

        [Fact]
        public void Constructor_WithValidData_CreatesMedication()
        {
            // Arrange
            var name = "Paracetamol";
            var dosage = "500mg";
            var pharmaceuticalForm = "Comprimido";
            var category = MedicationCategory.Analgesic;
            var requiresPrescription = false;

            // Act
            var medication = new Medication(name, dosage, pharmaceuticalForm, category, requiresPrescription, _tenantId);

            // Assert
            Assert.NotEqual(Guid.Empty, medication.Id);
            Assert.Equal(name, medication.Name);
            Assert.Equal(dosage, medication.Dosage);
            Assert.Equal(pharmaceuticalForm, medication.PharmaceuticalForm);
            Assert.Equal(category, medication.Category);
            Assert.Equal(requiresPrescription, medication.RequiresPrescription);
            Assert.True(medication.IsActive);
            Assert.False(medication.IsControlled);
        }

        [Fact]
        public void Constructor_WithAllOptionalData_CreatesMedication()
        {
            // Arrange
            var name = "Losartana Potássica";
            var dosage = "50mg";
            var pharmaceuticalForm = "Comprimido";
            var category = MedicationCategory.Antihypertensive;
            var requiresPrescription = true;
            var genericName = "Losartana";
            var manufacturer = "EMS";
            var activeIngredient = "Losartana Potássica";
            var concentration = "50mg";
            var administrationRoute = "Oral";
            var isControlled = false;
            var anvisaRegistration = "1234567890123";
            var barcode = "7891234567890";
            var description = "Anti-hipertensivo para tratamento de hipertensão";

            // Act
            var controlledList = (ControlledSubstanceList?)null;  // Not a controlled substance
            var medication = new Medication(
                name, dosage, pharmaceuticalForm, category, requiresPrescription, _tenantId,
                genericName, manufacturer, activeIngredient, concentration, administrationRoute,
                isControlled, controlledList, anvisaRegistration, barcode, description);

            // Assert
            Assert.Equal(name, medication.Name);
            Assert.Equal(genericName, medication.GenericName);
            Assert.Equal(manufacturer, medication.Manufacturer);
            Assert.Equal(activeIngredient, medication.ActiveIngredient);
            Assert.Equal(concentration, medication.Concentration);
            Assert.Equal(administrationRoute, medication.AdministrationRoute);
            Assert.Equal(isControlled, medication.IsControlled);
            Assert.Equal(anvisaRegistration, medication.AnvisaRegistration);
            Assert.Equal(barcode, medication.Barcode);
            Assert.Equal(description, medication.Description);
        }

        [Fact]
        public void Constructor_WithEmptyName_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Medication("", "500mg", "Comprimido", MedicationCategory.Analgesic, false, _tenantId));
        }

        [Fact]
        public void Constructor_WithEmptyDosage_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Medication("Paracetamol", "", "Comprimido", MedicationCategory.Analgesic, false, _tenantId));
        }

        [Fact]
        public void Constructor_WithEmptyPharmaceuticalForm_ThrowsArgumentException()
        {
            // Arrange & Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new Medication("Paracetamol", "500mg", "", MedicationCategory.Analgesic, false, _tenantId));
        }

        [Fact]
        public void Update_WithValidData_UpdatesMedication()
        {
            // Arrange
            var medication = CreateValidMedication();
            var newName = "Paracetamol Genérico";
            var newDosage = "750mg";
            var newPharmaceuticalForm = "Comprimido Revestido";
            var newCategory = MedicationCategory.AntiInflammatory;

            // Act
            medication.Update(newName, newDosage, newPharmaceuticalForm, newCategory, true);

            // Assert
            Assert.Equal(newName, medication.Name);
            Assert.Equal(newDosage, medication.Dosage);
            Assert.Equal(newPharmaceuticalForm, medication.PharmaceuticalForm);
            Assert.Equal(newCategory, medication.Category);
            Assert.True(medication.RequiresPrescription);
            Assert.NotNull(medication.UpdatedAt);
        }

        [Fact]
        public void Update_WithOptionalFields_UpdatesMedication()
        {
            // Arrange
            var medication = CreateValidMedication();
            var newManufacturer = "Novo Laboratório";
            var newDescription = "Nova descrição do medicamento";

            // Act
            medication.Update(
                "Paracetamol", "500mg", "Comprimido", MedicationCategory.Analgesic, false,
                manufacturer: newManufacturer, description: newDescription);

            // Assert
            Assert.Equal(newManufacturer, medication.Manufacturer);
            Assert.Equal(newDescription, medication.Description);
        }

        [Fact]
        public void Update_WithEmptyName_ThrowsArgumentException()
        {
            // Arrange
            var medication = CreateValidMedication();

            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                medication.Update("", "500mg", "Comprimido", MedicationCategory.Analgesic, false));
        }

        [Fact]
        public void Activate_SetsIsActiveToTrue()
        {
            // Arrange
            var medication = CreateValidMedication();
            medication.Deactivate();

            // Act
            medication.Activate();

            // Assert
            Assert.True(medication.IsActive);
            Assert.NotNull(medication.UpdatedAt);
        }

        [Fact]
        public void Deactivate_SetsIsActiveToFalse()
        {
            // Arrange
            var medication = CreateValidMedication();

            // Act
            medication.Deactivate();

            // Assert
            Assert.False(medication.IsActive);
            Assert.NotNull(medication.UpdatedAt);
        }

        [Fact]
        public void Constructor_WithControlledSubstance_SetsIsControlledCorrectly()
        {
            // Arrange & Act
            var medication = new Medication(
                "Rivotril", "2mg", "Comprimido", MedicationCategory.Anxiolytic,
                true, _tenantId, isControlled: true, controlledList: ControlledSubstanceList.B1_Psychotropics);

            // Assert
            Assert.True(medication.IsControlled);
            Assert.True(medication.RequiresPrescription);
            Assert.Equal(ControlledSubstanceList.B1_Psychotropics, medication.ControlledList);
        }

        [Fact]
        public void Constructor_TrimsWhitespaceFromStrings()
        {
            // Arrange & Act
            var medication = new Medication(
                "  Paracetamol  ", "  500mg  ", "  Comprimido  ",
                MedicationCategory.Analgesic, false, _tenantId,
                genericName: "  Para  ", manufacturer: "  EMS  ");

            // Assert
            Assert.Equal("Paracetamol", medication.Name);
            Assert.Equal("500mg", medication.Dosage);
            Assert.Equal("Comprimido", medication.PharmaceuticalForm);
            Assert.Equal("Para", medication.GenericName);
            Assert.Equal("EMS", medication.Manufacturer);
        }

        [Fact]
        public void MedicationCategory_HasAllExpectedValues()
        {
            // Assert - verify all expected categories exist
            Assert.True(Enum.IsDefined(typeof(MedicationCategory), MedicationCategory.Analgesic));
            Assert.True(Enum.IsDefined(typeof(MedicationCategory), MedicationCategory.Antibiotic));
            Assert.True(Enum.IsDefined(typeof(MedicationCategory), MedicationCategory.AntiInflammatory));
            Assert.True(Enum.IsDefined(typeof(MedicationCategory), MedicationCategory.Antihypertensive));
            Assert.True(Enum.IsDefined(typeof(MedicationCategory), MedicationCategory.Antidiabetic));
            Assert.True(Enum.IsDefined(typeof(MedicationCategory), MedicationCategory.Vitamin));
            Assert.True(Enum.IsDefined(typeof(MedicationCategory), MedicationCategory.Other));
        }

        private Medication CreateValidMedication()
        {
            return new Medication(
                "Paracetamol",
                "500mg",
                "Comprimido",
                MedicationCategory.Analgesic,
                false,
                _tenantId);
        }
    }
}
