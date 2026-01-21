using System;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class ClinicalExaminationTests
    {
        private readonly string _tenantId = "test-tenant";
        private readonly Guid _medicalRecordId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidData_CreatesClinicalExamination()
        {
            // Arrange
            var examination = "Patient presents with normal cardiovascular examination. Lungs clear bilaterally.";

            // Act
            var clinical = new ClinicalExamination(_medicalRecordId, _tenantId, examination);

            // Assert
            Assert.NotEqual(Guid.Empty, clinical.Id);
            Assert.Equal(_medicalRecordId, clinical.MedicalRecordId);
            Assert.Equal(examination, clinical.SystematicExamination);
            Assert.Null(clinical.BloodPressureSystolic);
            Assert.Null(clinical.BloodPressureDiastolic);
            Assert.Null(clinical.HeartRate);
        }

        [Fact]
        public void Constructor_WithVitalSigns_StoresAllValues()
        {
            // Arrange
            var examination = "Patient presents with normal cardiovascular examination.";
            var systolic = 120m;
            var diastolic = 80m;
            var heartRate = 72;
            var respiratoryRate = 16;
            var temperature = 36.5m;
            var oxygenSaturation = 98.5m;

            // Act
            var clinical = new ClinicalExamination(
                _medicalRecordId, _tenantId, examination,
                systolic, diastolic, heartRate, 
                respiratoryRate, temperature, oxygenSaturation);

            // Assert
            Assert.Equal(systolic, clinical.BloodPressureSystolic);
            Assert.Equal(diastolic, clinical.BloodPressureDiastolic);
            Assert.Equal(heartRate, clinical.HeartRate);
            Assert.Equal(respiratoryRate, clinical.RespiratoryRate);
            Assert.Equal(temperature, clinical.Temperature);
            Assert.Equal(oxygenSaturation, clinical.OxygenSaturation);
        }

        [Fact]
        public void Constructor_WithEmptyMedicalRecordId_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new ClinicalExamination(Guid.Empty, _tenantId, "Valid examination text here"));

            Assert.Equal("Medical record ID cannot be empty (Parameter 'medicalRecordId')", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyExamination_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new ClinicalExamination(_medicalRecordId, _tenantId, ""));

            Assert.Equal("Systematic examination is required (Parameter 'systematicExamination')", exception.Message);
        }

        [Fact]
        public void Constructor_WithShortExamination_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new ClinicalExamination(_medicalRecordId, _tenantId, "Too short"));

            Assert.Contains("must have at least 20 characters", exception.Message);
        }

        [Theory]
        [InlineData(49)]  // Below min
        [InlineData(301)]  // Above max
        public void Constructor_WithInvalidSystolicBP_ThrowsArgumentException(decimal value)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new ClinicalExamination(_medicalRecordId, _tenantId, 
                    "Patient presents with normal examination findings.", value, 80m, 72));

            Assert.Contains("Systolic blood pressure", exception.Message);
        }

        [Theory]
        [InlineData(29)]  // Below min
        [InlineData(201)]  // Above max
        public void Constructor_WithInvalidDiastolicBP_ThrowsArgumentException(decimal value)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new ClinicalExamination(_medicalRecordId, _tenantId,
                    "Patient presents with normal examination findings.", 120m, value, 72));

            Assert.Contains("Diastolic blood pressure", exception.Message);
        }

        [Theory]
        [InlineData(29)]  // Below min
        [InlineData(221)]  // Above max
        public void Constructor_WithInvalidHeartRate_ThrowsArgumentException(int value)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new ClinicalExamination(_medicalRecordId, _tenantId,
                    "Patient presents with normal examination findings.", 120m, 80m, value));

            Assert.Contains("Heart rate", exception.Message);
        }

        [Theory]
        [InlineData(7)]  // Below min
        [InlineData(61)]  // Above max
        public void Constructor_WithInvalidRespiratoryRate_ThrowsArgumentException(int value)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new ClinicalExamination(_medicalRecordId, _tenantId,
                    "Patient presents with normal examination findings.", 
                    120m, 80m, 72, value));

            Assert.Contains("Respiratory rate", exception.Message);
        }

        [Theory]
        [InlineData(31.9)]  // Below min
        [InlineData(45.1)]  // Above max
        public void Constructor_WithInvalidTemperature_ThrowsArgumentException(decimal value)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new ClinicalExamination(_medicalRecordId, _tenantId,
                    "Patient presents with normal examination findings.",
                    120m, 80m, 72, 16, value));

            Assert.Contains("Temperature", exception.Message);
        }

        [Theory]
        [InlineData(-0.1)]  // Below min
        [InlineData(100.1)]  // Above max
        public void Constructor_WithInvalidOxygenSaturation_ThrowsArgumentException(decimal value)
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new ClinicalExamination(_medicalRecordId, _tenantId,
                    "Patient presents with normal examination findings.",
                    120m, 80m, 72, 16, 36.5m, value));

            Assert.Contains("Oxygen saturation", exception.Message);
        }

        [Fact]
        public void UpdateVitalSigns_UpdatesAllValues()
        {
            // Arrange
            var clinical = CreateValidExamination();

            // Act
            clinical.UpdateVitalSigns(130m, 85m, 80, 18, 37.0m, 96.0m);

            // Assert
            Assert.Equal(130m, clinical.BloodPressureSystolic);
            Assert.Equal(85m, clinical.BloodPressureDiastolic);
            Assert.Equal(80, clinical.HeartRate);
            Assert.Equal(18, clinical.RespiratoryRate);
            Assert.Equal(37.0m, clinical.Temperature);
            Assert.Equal(96.0m, clinical.OxygenSaturation);
            Assert.NotNull(clinical.UpdatedAt);
        }

        [Fact]
        public void UpdateSystematicExamination_UpdatesExamination()
        {
            // Arrange
            var clinical = CreateValidExamination();
            var newExamination = "Updated examination: Patient shows improvement in respiratory function.";

            // Act
            clinical.UpdateSystematicExamination(newExamination);

            // Assert
            Assert.Equal(newExamination, clinical.SystematicExamination);
            Assert.NotNull(clinical.UpdatedAt);
        }

        [Fact]
        public void UpdateSystematicExamination_WithEmpty_ThrowsArgumentException()
        {
            // Arrange
            var clinical = CreateValidExamination();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => clinical.UpdateSystematicExamination(""));
        }

        [Fact]
        public void UpdateGeneralState_UpdatesState()
        {
            // Arrange
            var clinical = CreateValidExamination();
            var generalState = "Patient in good general condition";

            // Act
            clinical.UpdateGeneralState(generalState);

            // Assert
            Assert.Equal(generalState, clinical.GeneralState);
            Assert.NotNull(clinical.UpdatedAt);
        }

        [Fact]
        public void Constructor_TrimsWhitespace()
        {
            // Arrange
            var examination = "  Patient examination text with normal findings  ";
            var generalState = "  Good condition  ";

            // Act
            var clinical = new ClinicalExamination(
                _medicalRecordId, _tenantId, examination, 
                generalState: generalState);

            // Assert
            Assert.Equal("Patient examination text with normal findings", clinical.SystematicExamination);
            Assert.Equal("Good condition", clinical.GeneralState);
        }

        [Fact]
        public void BMI_WithWeightAndHeight_CalculatesCorrectly()
        {
            // Arrange
            var examination = "Patient presents with normal cardiovascular examination.";
            var weight = 70.0m; // kg
            var height = 1.75m; // meters
            var expectedBMI = 22.86m; // 70 / (1.75 * 1.75) = 22.857... rounded to 22.86

            // Act
            var clinical = new ClinicalExamination(
                _medicalRecordId, _tenantId, examination,
                weight: weight, height: height);

            // Assert
            Assert.NotNull(clinical.BMI);
            Assert.Equal(expectedBMI, clinical.BMI.Value);
        }

        [Fact]
        public void BMI_WithoutWeight_ReturnsNull()
        {
            // Arrange
            var examination = "Patient presents with normal cardiovascular examination.";
            var height = 1.75m;

            // Act
            var clinical = new ClinicalExamination(
                _medicalRecordId, _tenantId, examination,
                height: height);

            // Assert
            Assert.Null(clinical.BMI);
        }

        [Fact]
        public void BMI_WithoutHeight_ReturnsNull()
        {
            // Arrange
            var examination = "Patient presents with normal cardiovascular examination.";
            var weight = 70.0m;

            // Act
            var clinical = new ClinicalExamination(
                _medicalRecordId, _tenantId, examination,
                weight: weight);

            // Assert
            Assert.Null(clinical.BMI);
        }

        [Fact]
        public void BMI_WithZeroHeight_ReturnsNull()
        {
            // Arrange
            var examination = "Patient presents with normal cardiovascular examination.";
            var weight = 70.0m;
            var height = 0m;

            // Act
            var clinical = new ClinicalExamination(
                _medicalRecordId, _tenantId, examination,
                weight: weight, height: height);

            // Assert
            Assert.Null(clinical.BMI);
        }

        [Theory]
        [InlineData(50, 1.60, 19.53)]  // Peso normal
        [InlineData(80, 1.80, 24.69)]  // Peso normal
        [InlineData(100, 1.75, 32.65)] // Obesidade
        [InlineData(55, 1.70, 19.03)]  // Peso normal
        public void BMI_VariousWeightHeight_CalculatesCorrectly(decimal weight, decimal height, decimal expectedBMI)
        {
            // Arrange
            var examination = "Patient presents with normal cardiovascular examination.";

            // Act
            var clinical = new ClinicalExamination(
                _medicalRecordId, _tenantId, examination,
                weight: weight, height: height);

            // Assert
            Assert.NotNull(clinical.BMI);
            Assert.Equal(expectedBMI, clinical.BMI.Value);
        }

        [Fact]
        public void Constructor_WithInvalidWeight_ThrowsArgumentException()
        {
            // Arrange
            var examination = "Patient presents with normal cardiovascular examination.";
            var invalidWeight = 600m; // Above 500 kg limit

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new ClinicalExamination(_medicalRecordId, _tenantId, examination,
                    weight: invalidWeight, height: 1.75m));

            Assert.Contains("Weight must be between 0.5 and 500 kg", exception.Message);
        }

        [Fact]
        public void Constructor_WithInvalidHeight_ThrowsArgumentException()
        {
            // Arrange
            var examination = "Patient presents with normal cardiovascular examination.";
            var invalidHeight = 4.0m; // Above 3.0 meters limit

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new ClinicalExamination(_medicalRecordId, _tenantId, examination,
                    weight: 70m, height: invalidHeight));

            Assert.Contains("Height must be between 0.3 and 3.0 meters", exception.Message);
        }

        private ClinicalExamination CreateValidExamination()
        {
            return new ClinicalExamination(
                _medicalRecordId, _tenantId,
                "Patient presents with normal cardiovascular examination. Lungs clear bilaterally.",
                120m, 80m, 72);
        }
    }
}
