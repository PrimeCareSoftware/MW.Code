using System.Text.Json;
using System.Text.Json.Serialization;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Api.JsonConverters
{
    /// <summary>
    /// Custom JSON converter for ProcedureCategory enum that accepts both string names and numeric values.
    /// This allows API consumers to send category as either "Aesthetic" or 9.
    /// </summary>
    public class ProcedureCategoryJsonConverter : JsonConverter<ProcedureCategory>
    {
        public override ProcedureCategory Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            switch (reader.TokenType)
            {
                case JsonTokenType.String:
                    var stringValue = reader.GetString();
                    if (string.IsNullOrWhiteSpace(stringValue))
                    {
                        throw new JsonException("ProcedureCategory value cannot be empty");
                    }

                    // Try to parse as enum name (case-insensitive)
                    if (Enum.TryParse<ProcedureCategory>(stringValue, ignoreCase: true, out var enumValue))
                    {
                        return enumValue;
                    }

                    throw new JsonException($"Invalid ProcedureCategory value: '{stringValue}'. Valid values are: {string.Join(", ", Enum.GetNames(typeof(ProcedureCategory)))}");

                case JsonTokenType.Number:
                    var numericValue = reader.GetInt32();
                    if (Enum.IsDefined(typeof(ProcedureCategory), numericValue))
                    {
                        return (ProcedureCategory)numericValue;
                    }

                    throw new JsonException($"Invalid ProcedureCategory numeric value: {numericValue}. Valid values are 0-{Enum.GetValues(typeof(ProcedureCategory)).Length - 1}");

                default:
                    throw new JsonException($"Unexpected token type for ProcedureCategory: {reader.TokenType}");
            }
        }

        public override void Write(Utf8JsonWriter writer, ProcedureCategory value, JsonSerializerOptions options)
        {
            // Serialize as numeric value for consistency with existing API consumers
            writer.WriteNumberValue((int)value);
        }
    }
}
