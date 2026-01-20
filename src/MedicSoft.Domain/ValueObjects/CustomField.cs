using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.ValueObjects
{
    /// <summary>
    /// Value object representing a custom field definition in a consultation form
    /// </summary>
    public record CustomField
    {
        private static readonly Regex FieldKeyRegex = new Regex(
            @"^[a-zA-Z_][a-zA-Z0-9_]*$",
            RegexOptions.Compiled);

        public string FieldKey { get; init; }
        public string Label { get; init; }
        public CustomFieldType FieldType { get; init; }
        public bool IsRequired { get; init; }
        public int DisplayOrder { get; init; }
        public string? Placeholder { get; init; }
        public string? DefaultValue { get; init; }
        public string? HelpText { get; init; }
        public List<string>? Options { get; init; } // For SelecaoUnica and SelecaoMultipla

        public CustomField(
            string fieldKey,
            string label,
            CustomFieldType fieldType,
            bool isRequired = false,
            int displayOrder = 0,
            string? placeholder = null,
            string? defaultValue = null,
            string? helpText = null,
            List<string>? options = null)
        {
            if (string.IsNullOrWhiteSpace(fieldKey))
                throw new ArgumentException("Field key cannot be empty", nameof(fieldKey));

            if (string.IsNullOrWhiteSpace(label))
                throw new ArgumentException("Label cannot be empty", nameof(label));

            // Validate field key format (alphanumeric and underscores only)
            if (!FieldKeyRegex.IsMatch(fieldKey))
                throw new ArgumentException("Field key must start with a letter or underscore and contain only alphanumeric characters and underscores", nameof(fieldKey));

            // Validate options for selection types
            if ((fieldType == CustomFieldType.SelecaoUnica || fieldType == CustomFieldType.SelecaoMultipla) && 
                (options == null || !options.Any()))
                throw new ArgumentException($"Options are required for field type {fieldType}", nameof(options));

            FieldKey = fieldKey.Trim();
            Label = label.Trim();
            FieldType = fieldType;
            IsRequired = isRequired;
            DisplayOrder = displayOrder;
            Placeholder = placeholder?.Trim();
            DefaultValue = defaultValue?.Trim();
            HelpText = helpText?.Trim();
            Options = options?.Select(o => o.Trim()).ToList();
        }
    }
}
