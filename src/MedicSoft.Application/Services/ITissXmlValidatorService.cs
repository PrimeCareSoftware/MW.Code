using System.Collections.Generic;
using System.Threading.Tasks;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service for validating TISS XML files against ANS schemas
    /// TISS Standard Version: 4.02.00
    /// </summary>
    public interface ITissXmlValidatorService
    {
        /// <summary>
        /// Validates a guide XML against TISS schema
        /// </summary>
        /// <param name="xml">XML content as string</param>
        /// <returns>Validation result with detailed errors if any</returns>
        Task<ValidationResult> ValidateGuideXmlAsync(string xml);

        /// <summary>
        /// Validates a batch XML against TISS schema
        /// </summary>
        /// <param name="xml">XML content as string</param>
        /// <returns>Validation result with detailed errors if any</returns>
        Task<ValidationResult> ValidateBatchXmlAsync(string xml);

        /// <summary>
        /// Validates XML well-formedness and basic structure
        /// </summary>
        /// <param name="xml">XML content as string</param>
        /// <returns>Validation result</returns>
        Task<ValidationResult> ValidateXmlStructureAsync(string xml);

        /// <summary>
        /// Gets the TISS version being validated
        /// </summary>
        string GetTissVersion();
    }

    /// <summary>
    /// Result of XML validation
    /// </summary>
    public class ValidationResult
    {
        public bool IsValid { get; set; }
        public string? ErrorMessage { get; set; }
        public List<ValidationError> Errors { get; set; } = new();
        public List<ValidationWarning> Warnings { get; set; } = new();

        /// <summary>
        /// Total number of errors found
        /// </summary>
        public int ErrorCount => Errors.Count;

        /// <summary>
        /// Total number of warnings found
        /// </summary>
        public int WarningCount => Warnings.Count;

        /// <summary>
        /// Adds an error to the validation result
        /// </summary>
        public void AddError(string message, string? lineNumber = null, string? column = null)
        {
            Errors.Add(new ValidationError
            {
                Message = message,
                LineNumber = lineNumber,
                Column = column
            });
            IsValid = false;
        }

        /// <summary>
        /// Adds a warning to the validation result
        /// </summary>
        public void AddWarning(string message, string? lineNumber = null)
        {
            Warnings.Add(new ValidationWarning
            {
                Message = message,
                LineNumber = lineNumber
            });
        }
    }

    /// <summary>
    /// Represents a validation error
    /// </summary>
    public class ValidationError
    {
        public string Message { get; set; } = string.Empty;
        public string? LineNumber { get; set; }
        public string? Column { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(LineNumber))
                return $"Line {LineNumber}: {Message}";
            return Message;
        }
    }

    /// <summary>
    /// Represents a validation warning
    /// </summary>
    public class ValidationWarning
    {
        public string Message { get; set; } = string.Empty;
        public string? LineNumber { get; set; }

        public override string ToString()
        {
            if (!string.IsNullOrEmpty(LineNumber))
                return $"Line {LineNumber}: {Message}";
            return Message;
        }
    }
}
