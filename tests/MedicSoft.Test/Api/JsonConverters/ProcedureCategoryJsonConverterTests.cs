using System;
using System.Text.Json;
using Xunit;
using MedicSoft.Api.JsonConverters;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Test.Api.JsonConverters
{
    public class ProcedureCategoryJsonConverterTests
    {
        private readonly JsonSerializerOptions _options;

        public ProcedureCategoryJsonConverterTests()
        {
            _options = new JsonSerializerOptions
            {
                Converters = { new ProcedureCategoryJsonConverter() }
            };
        }

        [Theory]
        [InlineData(ProcedureCategory.Consultation, 0)]
        [InlineData(ProcedureCategory.Exam, 1)]
        [InlineData(ProcedureCategory.Surgery, 2)]
        [InlineData(ProcedureCategory.Aesthetic, 9)]
        [InlineData(ProcedureCategory.Other, 11)]
        public void Write_SerializesProcedureCategoryToNumericValue(ProcedureCategory category, int expected)
        {
            // Arrange
            var testObj = new TestDto { Category = category };

            // Act
            var json = JsonSerializer.Serialize(testObj, _options);

            // Assert
            Assert.Contains($"\"Category\":{expected}", json);
        }

        [Theory]
        [InlineData(0, ProcedureCategory.Consultation)]
        [InlineData(1, ProcedureCategory.Exam)]
        [InlineData(2, ProcedureCategory.Surgery)]
        [InlineData(3, ProcedureCategory.Therapy)]
        [InlineData(4, ProcedureCategory.Vaccination)]
        [InlineData(9, ProcedureCategory.Aesthetic)]
        [InlineData(11, ProcedureCategory.Other)]
        public void Read_DeserializesNumericValueToProcedureCategory(int numericValue, ProcedureCategory expected)
        {
            // Arrange
            var json = $@"{{""Category"":{numericValue}}}";

            // Act
            var result = JsonSerializer.Deserialize<TestDto>(json, _options);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected, result.Category);
        }

        [Theory]
        [InlineData("Consultation", ProcedureCategory.Consultation)]
        [InlineData("consultation", ProcedureCategory.Consultation)]
        [InlineData("CONSULTATION", ProcedureCategory.Consultation)]
        [InlineData("Exam", ProcedureCategory.Exam)]
        [InlineData("Surgery", ProcedureCategory.Surgery)]
        [InlineData("Therapy", ProcedureCategory.Therapy)]
        [InlineData("Vaccination", ProcedureCategory.Vaccination)]
        [InlineData("Diagnostic", ProcedureCategory.Diagnostic)]
        [InlineData("Treatment", ProcedureCategory.Treatment)]
        [InlineData("Emergency", ProcedureCategory.Emergency)]
        [InlineData("Prevention", ProcedureCategory.Prevention)]
        [InlineData("Aesthetic", ProcedureCategory.Aesthetic)]
        [InlineData("FollowUp", ProcedureCategory.FollowUp)]
        [InlineData("Other", ProcedureCategory.Other)]
        public void Read_DeserializesStringNameToProcedureCategory(string stringValue, ProcedureCategory expected)
        {
            // Arrange
            var json = $@"{{""Category"":""{stringValue}""}}";

            // Act
            var result = JsonSerializer.Deserialize<TestDto>(json, _options);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expected, result.Category);
        }

        [Fact]
        public void Read_WithEmptyString_ThrowsJsonException()
        {
            // Arrange
            var json = @"{""Category"":""""}";

            // Act & Assert
            var exception = Assert.Throws<JsonException>(() => 
                JsonSerializer.Deserialize<TestDto>(json, _options));
            Assert.Contains("cannot be empty", exception.Message);
        }

        [Fact]
        public void Read_WithInvalidStringValue_ThrowsJsonException()
        {
            // Arrange
            var json = @"{""Category"":""InvalidCategory""}";

            // Act & Assert
            var exception = Assert.Throws<JsonException>(() => 
                JsonSerializer.Deserialize<TestDto>(json, _options));
            Assert.Contains("Invalid ProcedureCategory value", exception.Message);
        }

        [Fact]
        public void Read_WithInvalidNumericValue_ThrowsJsonException()
        {
            // Arrange
            var json = @"{""Category"":999}";

            // Act & Assert
            var exception = Assert.Throws<JsonException>(() => 
                JsonSerializer.Deserialize<TestDto>(json, _options));
            Assert.Contains("Invalid ProcedureCategory numeric value", exception.Message);
        }

        [Fact]
        public void RoundTrip_PreservesEnumValue()
        {
            // Arrange
            var original = new TestDto { Category = ProcedureCategory.Aesthetic };

            // Act
            var json = JsonSerializer.Serialize(original, _options);
            var deserialized = JsonSerializer.Deserialize<TestDto>(json, _options);

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal(original.Category, deserialized.Category);
        }

        [Fact]
        public void Deserialize_StringToNumericRoundTrip_PreservesValue()
        {
            // Arrange - Start with string input
            var jsonWithString = @"{""Category"":""Aesthetic""}";

            // Act - Deserialize string, then serialize (which produces numeric), then deserialize again
            var firstDeserialization = JsonSerializer.Deserialize<TestDto>(jsonWithString, _options);
            var jsonWithNumeric = JsonSerializer.Serialize(firstDeserialization, _options);
            var secondDeserialization = JsonSerializer.Deserialize<TestDto>(jsonWithNumeric, _options);

            // Assert - Both should result in the same enum value
            Assert.NotNull(firstDeserialization);
            Assert.NotNull(secondDeserialization);
            Assert.Equal(ProcedureCategory.Aesthetic, firstDeserialization.Category);
            Assert.Equal(ProcedureCategory.Aesthetic, secondDeserialization.Category);
        }

        private class TestDto
        {
            public ProcedureCategory Category { get; set; }
        }
    }
}
