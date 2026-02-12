using System;
using System.Text.Json;
using Xunit;
using MedicSoft.Application.JsonConverters;

namespace MedicSoft.Test.Application.JsonConverters
{
    public class DateOnlyJsonConverterTests
    {
        private readonly JsonSerializerOptions _options;

        public DateOnlyJsonConverterTests()
        {
            _options = new JsonSerializerOptions
            {
                Converters = { new DateOnlyJsonConverter() }
            };
        }

        [Theory]
        [InlineData(2025, 2, 17, "2025-02-17")]
        [InlineData(2025, 12, 31, "2025-12-31")]
        [InlineData(2025, 1, 1, "2025-01-01")]
        [InlineData(2024, 6, 15, "2024-06-15")]
        public void Write_SerializesDateTimeToYyyyMmDdFormat(int year, int month, int day, string expected)
        {
            // Arrange
            var date = new DateTime(year, month, day, 0, 0, 0, DateTimeKind.Utc);
            var testObj = new { ScheduledDate = date };

            // Act
            var json = JsonSerializer.Serialize(testObj, _options);

            // Assert
            Assert.Contains($"\"ScheduledDate\":\"{expected}\"", json);
        }

        [Theory]
        [InlineData("2025-02-17", 2025, 2, 17)]
        [InlineData("2025-12-31", 2025, 12, 31)]
        [InlineData("2025-01-01", 2025, 1, 1)]
        [InlineData("2024-06-15", 2024, 6, 15)]
        public void Read_DeserializesYyyyMmDdFormatToDateTime(string dateString, int expectedYear, int expectedMonth, int expectedDay)
        {
            // Arrange
            var json = $@"{{""ScheduledDate"":""{dateString}""}}";

            // Act
            var result = JsonSerializer.Deserialize<TestDto>(json, _options);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedYear, result.ScheduledDate.Year);
            Assert.Equal(expectedMonth, result.ScheduledDate.Month);
            Assert.Equal(expectedDay, result.ScheduledDate.Day);
        }

        [Fact]
        public void Write_StripTimeAndTimezoneInformation()
        {
            // Arrange - date with specific time component
            var date = new DateTime(2025, 2, 17, 14, 30, 45, DateTimeKind.Utc);
            var testObj = new { ScheduledDate = date };

            // Act
            var json = JsonSerializer.Serialize(testObj, _options);

            // Assert - should only contain date, no time
            Assert.Contains("\"ScheduledDate\":\"2025-02-17\"", json);
            Assert.DoesNotContain("14:30", json);
            Assert.DoesNotContain("T", json);
        }

        [Fact]
        public void Read_WithEmptyString_ThrowsJsonException()
        {
            // Arrange
            var json = @"{""ScheduledDate"":""""}";

            // Act & Assert
            Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<TestDto>(json, _options));
        }

        [Fact]
        public void Read_WithInvalidFormat_ThrowsJsonException()
        {
            // Arrange
            var json = @"{""ScheduledDate"":""17/02/2025""}";

            // Act & Assert
            Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<TestDto>(json, _options));
        }

        [Fact]
        public void Read_WithIsoFormatWithTime_ThrowsJsonException()
        {
            // Arrange - ISO format with time should be rejected
            var json = @"{""ScheduledDate"":""2025-02-17T14:30:00""}";

            // Act & Assert
            Assert.Throws<JsonException>(() =>
                JsonSerializer.Deserialize<TestDto>(json, _options));
        }

        [Fact]
        public void RoundTrip_PreservesDateValue()
        {
            // Arrange
            var original = new TestDto { ScheduledDate = new DateTime(2025, 2, 17, 0, 0, 0, DateTimeKind.Utc) };

            // Act
            var json = JsonSerializer.Serialize(original, _options);
            var deserialized = JsonSerializer.Deserialize<TestDto>(json, _options);

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal(original.ScheduledDate.Year, deserialized.ScheduledDate.Year);
            Assert.Equal(original.ScheduledDate.Month, deserialized.ScheduledDate.Month);
            Assert.Equal(original.ScheduledDate.Day, deserialized.ScheduledDate.Day);
        }

        [Fact]
        public void Read_ParsesDateAsUtc()
        {
            // Arrange
            var json = @"{""ScheduledDate"":""2025-02-17""}";

            // Act
            var result = JsonSerializer.Deserialize<TestDto>(json, _options);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(DateTimeKind.Utc, result.ScheduledDate.Kind);
        }

        private class TestDto
        {
            public DateTime ScheduledDate { get; set; }
        }
    }
}
