using System.Text.Json;
using System.Text.Json.Serialization;

namespace MedicSoft.Api.JsonConverters
{
    /// <summary>
    /// Custom JSON converter for TimeSpan that serializes to and from "HH:mm" format
    /// This ensures compatibility with frontend calendar components that expect simple time strings
    /// </summary>
    public class TimeSpanJsonConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value))
            {
                return TimeSpan.Zero;
            }

            // Try to parse as "HH:mm" format first (24-hour format)
            if (TimeSpan.TryParseExact(value, @"hh\:mm", null, out var result))
            {
                return result;
            }

            // Fall back to default TimeSpan parsing for backwards compatibility
            if (TimeSpan.TryParse(value, out var fallbackResult))
            {
                return fallbackResult;
            }

            throw new JsonException($"Unable to parse TimeSpan value: {value}");
        }

        public override void Write(Utf8JsonWriter writer, TimeSpan value, JsonSerializerOptions options)
        {
            // Serialize as "HH:mm" format in 24-hour notation (e.g., "09:00", "14:30")
            // For TimeSpan, we use the total hours component
            var hours = (int)value.TotalHours;
            var minutes = value.Minutes;
            writer.WriteStringValue($"{hours:D2}:{minutes:D2}");
        }
    }
}
