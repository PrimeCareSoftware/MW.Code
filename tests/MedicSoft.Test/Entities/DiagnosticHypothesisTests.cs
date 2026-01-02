using System;
using Xunit;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Entities
{
    public class DiagnosticHypothesisTests
    {
        private readonly string _tenantId = "test-tenant";
        private readonly Guid _medicalRecordId = Guid.NewGuid();

        [Fact]
        public void Constructor_WithValidData_CreatesDiagnosticHypothesis()
        {
            // Arrange
            var description = "Acute bronchitis";
            var icd10Code = "J20.9";

            // Act
            var diagnosis = new DiagnosticHypothesis(_medicalRecordId, _tenantId, description, icd10Code);

            // Assert
            Assert.NotEqual(Guid.Empty, diagnosis.Id);
            Assert.Equal(_medicalRecordId, diagnosis.MedicalRecordId);
            Assert.Equal(description, diagnosis.Description);
            Assert.Equal("J20.9", diagnosis.ICD10Code);
            Assert.Equal(DiagnosisType.Principal, diagnosis.Type);
            Assert.True(diagnosis.DiagnosedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void Constructor_WithSecondaryType_CreatesSecondaryDiagnosis()
        {
            // Arrange
            var description = "Hypertension";
            var icd10Code = "I10";

            // Act
            var diagnosis = new DiagnosticHypothesis(_medicalRecordId, _tenantId, description, icd10Code, DiagnosisType.Secondary);

            // Assert
            Assert.Equal(DiagnosisType.Secondary, diagnosis.Type);
        }

        [Fact]
        public void Constructor_WithEmptyMedicalRecordId_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new DiagnosticHypothesis(Guid.Empty, _tenantId, "Description", "A00"));

            Assert.Equal("Medical record ID cannot be empty (Parameter 'medicalRecordId')", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyDescription_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new DiagnosticHypothesis(_medicalRecordId, _tenantId, "", "A00"));

            Assert.Equal("Description is required (Parameter 'description')", exception.Message);
        }

        [Fact]
        public void Constructor_WithEmptyICD10Code_ThrowsArgumentException()
        {
            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() =>
                new DiagnosticHypothesis(_medicalRecordId, _tenantId, "Description", ""));

            Assert.Equal("ICD-10 code is required (Parameter 'icd10Code')", exception.Message);
        }

        [Theory]
        [InlineData("A00")]
        [InlineData("Z99")]
        [InlineData("A00.0")]
        [InlineData("A00.01")]
        [InlineData("J20.9")]
        [InlineData("I10")]
        public void Constructor_WithValidICD10Codes_CreatesHypothesis(string icd10Code)
        {
            // Act
            var diagnosis = new DiagnosticHypothesis(_medicalRecordId, _tenantId, "Test", icd10Code);

            // Assert
            Assert.Equal(icd10Code.ToUpperInvariant(), diagnosis.ICD10Code);
        }

        [Theory]
        [InlineData("a00")]
        [InlineData("z99.9")]
        [InlineData("j20.01")]
        public void Constructor_WithLowercaseICD10Code_ConvertsToUppercase(string icd10Code)
        {
            // Act
            var diagnosis = new DiagnosticHypothesis(_medicalRecordId, _tenantId, "Test", icd10Code);

            // Assert
            Assert.Equal(icd10Code.ToUpperInvariant(), diagnosis.ICD10Code);
        }

        [Theory]
        [InlineData("00")]  // Too short
        [InlineData("AAA")]  // Three letters
        [InlineData("1A0")]  // Starts with number
        [InlineData("A0A")]  // Letter in position 3
        [InlineData("AA0")]  // Letter in position 2
        [InlineData("A00..0")]  // Double dot
        [InlineData("A00.")]  // Dot without suffix
        [InlineData("A00.ABC")]  // Letters after dot
        [InlineData("A00.001")]  // Three digits after dot
        public void Constructor_WithInvalidICD10Code_ThrowsArgumentException(string icd10Code)
        {
            // Act & Assert
            Assert.Throws<ArgumentException>(() =>
                new DiagnosticHypothesis(_medicalRecordId, _tenantId, "Test", icd10Code));
        }

        [Fact]
        public void UpdateDescription_UpdatesDescriptionField()
        {
            // Arrange
            var diagnosis = CreateValidDiagnosis();
            var newDescription = "Chronic bronchitis with acute exacerbation";

            // Act
            diagnosis.UpdateDescription(newDescription);

            // Assert
            Assert.Equal(newDescription, diagnosis.Description);
            Assert.NotNull(diagnosis.UpdatedAt);
        }

        [Fact]
        public void UpdateDescription_WithEmpty_ThrowsArgumentException()
        {
            // Arrange
            var diagnosis = CreateValidDiagnosis();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => diagnosis.UpdateDescription(""));
        }

        [Fact]
        public void UpdateICD10Code_UpdatesCodeField()
        {
            // Arrange
            var diagnosis = CreateValidDiagnosis();
            var newCode = "J21.0";

            // Act
            diagnosis.UpdateICD10Code(newCode);

            // Assert
            Assert.Equal("J21.0", diagnosis.ICD10Code);
            Assert.NotNull(diagnosis.UpdatedAt);
        }

        [Fact]
        public void UpdateICD10Code_WithInvalidCode_ThrowsArgumentException()
        {
            // Arrange
            var diagnosis = CreateValidDiagnosis();

            // Act & Assert
            Assert.Throws<ArgumentException>(() => diagnosis.UpdateICD10Code("INVALID"));
        }

        [Fact]
        public void UpdateType_ChangesType()
        {
            // Arrange
            var diagnosis = CreateValidDiagnosis();

            // Act
            diagnosis.UpdateType(DiagnosisType.Secondary);

            // Assert
            Assert.Equal(DiagnosisType.Secondary, diagnosis.Type);
            Assert.NotNull(diagnosis.UpdatedAt);
        }

        [Fact]
        public void Constructor_TrimsWhitespace()
        {
            // Arrange
            var description = "  Test Description  ";
            var icd10Code = "  A00.0  ";

            // Act
            var diagnosis = new DiagnosticHypothesis(_medicalRecordId, _tenantId, description, icd10Code);

            // Assert
            Assert.Equal("Test Description", diagnosis.Description);
            Assert.Equal("A00.0", diagnosis.ICD10Code);
        }

        private DiagnosticHypothesis CreateValidDiagnosis()
        {
            return new DiagnosticHypothesis(_medicalRecordId, _tenantId, "Acute bronchitis", "J20.9");
        }
    }
}
