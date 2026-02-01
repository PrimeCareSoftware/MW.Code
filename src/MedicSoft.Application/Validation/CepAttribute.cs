using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MedicSoft.Application.Validation
{
    /// <summary>
    /// Validates that a CEP (Brazilian postal code) is valid.
    /// Accepts both formatted (00000-000) and unformatted (00000000) CEP.
    /// </summary>
    public class CepAttribute : ValidationAttribute
    {
        private static readonly Regex CleanupRegex = new Regex(@"[^\d]", RegexOptions.Compiled);

        public CepAttribute() : base("CEP deve conter 8 dígitos")
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var cep = value.ToString();
            if (string.IsNullOrWhiteSpace(cep))
            {
                return ValidationResult.Success;
            }

            // Remove formatting characters (dashes, spaces)
            var cleanCep = CleanupRegex.Replace(cep, "");

            // Check if it has exactly 8 digits
            if (cleanCep.Length != 8)
            {
                return new ValidationResult(ErrorMessage ?? "CEP deve conter 8 dígitos");
            }

            // Store the cleaned CEP back to the property
            var property = validationContext.ObjectType.GetProperty(validationContext.MemberName!);
            if (property != null && property.CanWrite)
            {
                property.SetValue(validationContext.ObjectInstance, cleanCep);
            }

            return ValidationResult.Success;
        }
    }
}
