using System.Text.Json;
using System.Text.Json.Serialization;

namespace MedicSoft.Application.JsonConverters
{
    /// <summary>
    /// Custom JSON converter for DateTime properties that represent date-only values.
    /// Serializes dates in "yyyy-MM-dd" format without time or timezone information.
    /// This prevents timezone conversion issues where dates can shift by ±1 day.
    /// Apply this converter with [JsonConverter(typeof(DateOnlyJsonConverter))] on date-only properties.
    /// </summary>
    public class DateOnlyJsonConverter : JsonConverter<DateTime>
    {
        private const string DateFormat = "yyyy-MM-dd";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (string.IsNullOrEmpty(value))
            {
                throw new JsonException("Date value cannot be null or empty");
            }

            // Parse the date string in yyyy-MM-dd format as UTC to avoid timezone issues
            if (DateTime.TryParseExact(
                value,
                DateFormat,
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal,
                out var date))
            {
                return date;
            }

            throw new JsonException($"Unable to parse date value: {value}. Expected format: {DateFormat}");
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            // Serialize as "yyyy-MM-dd" format only, stripping time and timezone information
            writer.WriteStringValue(value.ToString(DateFormat, System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
