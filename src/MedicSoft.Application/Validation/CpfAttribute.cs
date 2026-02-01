using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace MedicSoft.Application.Validation
{
    /// <summary>
    /// Validates that a CPF (Brazilian tax ID) is valid.
    /// Accepts both formatted (000.000.000-00) and unformatted (00000000000) CPF.
    /// </summary>
    public class CpfAttribute : ValidationAttribute
    {
        private static readonly Regex CleanupRegex = new Regex(@"[^\d]", RegexOptions.Compiled);

        public CpfAttribute() : base("CPF deve conter 11 dígitos")
        {
        }

        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value == null)
            {
                return ValidationResult.Success;
            }

            var cpf = value.ToString();
            if (string.IsNullOrWhiteSpace(cpf))
            {
                return ValidationResult.Success;
            }

            // Remove formatting characters (dots, dashes, spaces)
            var cleanCpf = CleanupRegex.Replace(cpf, "");

            // Check if it has exactly 11 digits
            if (cleanCpf.Length != 11)
            {
                return new ValidationResult(ErrorMessage ?? "CPF deve conter 11 dígitos");
            }

            // Store the cleaned CPF back to the property
            var property = validationContext.ObjectType.GetProperty(validationContext.MemberName!);
            if (property != null && property.CanWrite)
            {
                property.SetValue(validationContext.ObjectInstance, cleanCpf);
            }

            return ValidationResult.Success;
        }
    }
}
