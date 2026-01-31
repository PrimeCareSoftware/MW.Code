using System;
using System.Collections.Generic;
using System.Text.Json;
using Xunit;
using FluentAssertions;
using MedicSoft.Application.DTOs.SystemAdmin;

namespace MedicSoft.Test.Integration
{
    /// <summary>
    /// Tests for JSON serialization and enum conversion
    /// Ensures proper handling of enum values in API requests
    /// </summary>
    public class JsonEnumConversionTests
    {
        private readonly JsonSerializerOptions _jsonOptions;

        public JsonEnumConversionTests()
        {
            // Configure JSON options to match the API configuration
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters =
                {
                    new System.Text.Json.Serialization.JsonStringEnumConverter()
                }
            };
        }

        [Fact]
        public void ClinicFilterDto_WithStringHealthStatus_ShouldDeserialize()
        {
            // Arrange
            var json = @"{
                ""page"": 1,
                ""pageSize"": 20,
                ""healthStatus"": ""AtRisk""
            }";

            // Act
            var result = JsonSerializer.Deserialize<ClinicFilterDto>(json, _jsonOptions);

            // Assert
            result.Should().NotBeNull();
            result!.Page.Should().Be(1);
            result.PageSize.Should().Be(20);
            result.HealthStatus.Should().Be(HealthStatus.AtRisk);
        }

        [Fact]
        public void ClinicFilterDto_WithNumericHealthStatus_ShouldDeserialize()
        {
            // Arrange
            var json = @"{
                ""page"": 1,
                ""pageSize"": 20,
                ""healthStatus"": 2
            }";

            // Act
            var result = JsonSerializer.Deserialize<ClinicFilterDto>(json, _jsonOptions);

            // Assert
            result.Should().NotBeNull();
            result!.HealthStatus.Should().Be(HealthStatus.AtRisk);
        }

        [Fact]
        public void ClinicFilterDto_WithNullHealthStatus_ShouldDeserialize()
        {
            // Arrange
            var json = @"{
                ""page"": 1,
                ""pageSize"": 20
            }";

            // Act
            var result = JsonSerializer.Deserialize<ClinicFilterDto>(json, _jsonOptions);

            // Assert
            result.Should().NotBeNull();
            result!.HealthStatus.Should().BeNull();
        }

        [Theory]
        [InlineData("Healthy", HealthStatus.Healthy)]
        [InlineData("NeedsAttention", HealthStatus.NeedsAttention)]
        [InlineData("AtRisk", HealthStatus.AtRisk)]
        public void HealthStatus_AllValues_ShouldSerializeAndDeserialize(string enumString, HealthStatus enumValue)
        {
            // Arrange
            var filter = new ClinicFilterDto
            {
                HealthStatus = enumValue
            };

            // Act - Serialize
            var json = JsonSerializer.Serialize(filter, _jsonOptions);
            
            // Assert - Contains the string representation
            json.Should().Contain(enumString);

            // Act - Deserialize
            var deserialized = JsonSerializer.Deserialize<ClinicFilterDto>(json, _jsonOptions);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized!.HealthStatus.Should().Be(enumValue);
        }

        [Fact]
        public void ClinicFilterDto_WithCaseInsensitiveProperties_ShouldDeserialize()
        {
            // Arrange - Mixed case property names
            var json = @"{
                ""PAGE"": 2,
                ""pagesize"": 50,
                ""HEALTHSTATUS"": ""Healthy"",
                ""searchterm"": ""Test Clinic"",
                ""isactive"": true
            }";

            // Act
            var result = JsonSerializer.Deserialize<ClinicFilterDto>(json, _jsonOptions);

            // Assert
            result.Should().NotBeNull();
            result!.Page.Should().Be(2);
            result.PageSize.Should().Be(50);
            result.HealthStatus.Should().Be(HealthStatus.Healthy);
            result.SearchTerm.Should().Be("Test Clinic");
            result.IsActive.Should().BeTrue();
        }

        [Fact]
        public void ClinicFilterDto_WithAllFilters_ShouldSerializeAndDeserialize()
        {
            // Arrange
            var filter = new ClinicFilterDto
            {
                SearchTerm = "Clínica Test",
                IsActive = true,
                Tags = new List<string> { "vip", "enterprise" },
                HealthStatus = HealthStatus.AtRisk,
                SubscriptionStatus = "Active",
                CreatedAfter = new DateTime(2024, 1, 1),
                CreatedBefore = new DateTime(2024, 12, 31),
                Page = 1,
                PageSize = 20,
                SortBy = "name",
                SortDescending = false
            };

            // Act
            var json = JsonSerializer.Serialize(filter, _jsonOptions);
            var deserialized = JsonSerializer.Deserialize<ClinicFilterDto>(json, _jsonOptions);

            // Assert
            deserialized.Should().NotBeNull();
            deserialized!.Should().BeEquivalentTo(filter);
        }

        [Fact]
        public void HealthStatus_RoundTrip_ShouldMaintainValue()
        {
            // Test that enum values survive serialization round-trip
            foreach (HealthStatus status in Enum.GetValues(typeof(HealthStatus)))
            {
                // Arrange
                var original = new ClinicFilterDto { HealthStatus = status };

                // Act
                var json = JsonSerializer.Serialize(original, _jsonOptions);
                var deserialized = JsonSerializer.Deserialize<ClinicFilterDto>(json, _jsonOptions);

                // Assert
                deserialized.Should().NotBeNull();
                deserialized!.HealthStatus.Should().Be(status);
            }
        }

        [Fact]
        public void ExportFormat_Enum_ShouldSerializeAsString()
        {
            // Arrange
            var dto = new ExportClinicsDto
            {
                ClinicIds = new List<Guid> { Guid.NewGuid() },
                Format = ExportFormat.Excel,
                IncludeHealthScore = true
            };

            // Act
            var json = JsonSerializer.Serialize(dto, _jsonOptions);

            // Assert
            json.Should().Contain("Excel");
            json.Should().NotContain("1"); // Should not contain numeric value
        }
    }
}
