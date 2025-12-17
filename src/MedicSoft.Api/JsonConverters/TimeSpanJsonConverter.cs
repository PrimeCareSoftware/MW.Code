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
            // Split and parse manually since TimeSpan.ParseExact doesn't support HH format directly
            if (value.Contains(':'))
            {
                var parts = value.Split(':');
                if (parts.Length == 2 && 
                    int.TryParse(parts[0], out var hours) && 
                    int.TryParse(parts[1], out var minutes) &&
                    hours >= 0 && hours <= 23 &&
                    minutes >= 0 && minutes <= 59)
                {
                    return new TimeSpan(hours, minutes, 0);
                }
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
            // Use Hours (not TotalHours) to get the hour component within a 24-hour period
            var hours = value.Hours;
            var minutes = value.Minutes;
            writer.WriteStringValue($"{hours:D2}:{minutes:D2}");
        }
    }
}
