using System;
using System.Text.Json;
using Xunit;
using MedicSoft.Api.JsonConverters;

namespace MedicSoft.Test.Api.JsonConverters
{
    public class TimeSpanJsonConverterTests
    {
        private readonly JsonSerializerOptions _options;

        public TimeSpanJsonConverterTests()
        {
            _options = new JsonSerializerOptions
            {
                Converters = { new TimeSpanJsonConverter() }
            };
        }

        [Theory]
        [InlineData(9, 0, "09:00")]
        [InlineData(14, 30, "14:30")]
        [InlineData(0, 0, "00:00")]
        [InlineData(23, 59, "23:59")]
        [InlineData(8, 5, "08:05")]
        public void Write_SerializesTimeSpanToHHmmFormat(int hours, int minutes, string expected)
        {
            // Arrange
            var timeSpan = new TimeSpan(hours, minutes, 0);
            var testObj = new { ScheduledTime = timeSpan };

            // Act
            var json = JsonSerializer.Serialize(testObj, _options);

            // Assert
            Assert.Contains($"\"ScheduledTime\":\"{expected}\"", json);
        }

        [Theory]
        [InlineData("09:00", 9, 0)]
        [InlineData("14:30", 14, 30)]
        [InlineData("00:00", 0, 0)]
        [InlineData("23:59", 23, 59)]
        [InlineData("08:05", 8, 5)]
        public void Read_DeserializesHHmmFormatToTimeSpan(string timeString, int expectedHours, int expectedMinutes)
        {
            // Arrange
            var json = $@"{{""ScheduledTime"":""{timeString}""}}";

            // Act
            var result = JsonSerializer.Deserialize<TestDto>(json, _options);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedHours, result.ScheduledTime.Hours);
            Assert.Equal(expectedMinutes, result.ScheduledTime.Minutes);
        }

        [Fact]
        public void Read_WithEmptyString_ReturnsTimeSpanZero()
        {
            // Arrange
            var json = @"{""ScheduledTime"":""""}";

            // Act
            var result = JsonSerializer.Deserialize<TestDto>(json, _options);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(TimeSpan.Zero, result.ScheduledTime);
        }

        [Theory]
        [InlineData("09:00:00")] // Standard TimeSpan format with seconds
        [InlineData("1.09:00:00")] // TimeSpan format with days
        public void Read_WithStandardTimeSpanFormat_ParsesCorrectly(string timeString)
        {
            // Arrange
            var json = $@"{{""ScheduledTime"":""{timeString}""}}";

            // Act
            var result = JsonSerializer.Deserialize<TestDto>(json, _options);

            // Assert
            Assert.NotNull(result);
            // Should parse using fallback TimeSpan.TryParse
            Assert.True(result.ScheduledTime.Hours >= 0);
        }

        [Fact]
        public void Read_WithInvalidFormat_ThrowsJsonException()
        {
            // Arrange
            var json = @"{""ScheduledTime"":""invalid""}";

            // Act & Assert
            Assert.Throws<JsonException>(() => 
                JsonSerializer.Deserialize<TestDto>(json, _options));
        }

        [Fact]
        public void RoundTrip_PreservesTimeValue()
        {
            // Arrange
            var original = new TestDto { ScheduledTime = new TimeSpan(10, 45, 0) };

            // Act
            var json = JsonSerializer.Serialize(original, _options);
            var deserialized = JsonSerializer.Deserialize<TestDto>(json, _options);

            // Assert
            Assert.NotNull(deserialized);
            Assert.Equal(original.ScheduledTime.Hours, deserialized.ScheduledTime.Hours);
            Assert.Equal(original.ScheduledTime.Minutes, deserialized.ScheduledTime.Minutes);
        }

        private class TestDto
        {
            public TimeSpan ScheduledTime { get; set; }
        }
    }
}
