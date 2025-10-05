using System;
using System.Linq;

namespace MedicSoft.Domain.ValueObjects
{
    /// <summary>
    /// Value Object representing a Brazilian CPF (Cadastro de Pessoas Físicas)
    /// Validates CPF format and check digits
    /// </summary>
    public record Cpf
    {
        public string Value { get; }

        public Cpf(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("CPF cannot be empty", nameof(value));

            // Remove non-numeric characters
            var cleanCpf = new string(value.Where(char.IsDigit).ToArray());

            if (cleanCpf.Length != 11)
                throw new ArgumentException("CPF must have 11 digits", nameof(value));

            // Check for known invalid CPFs (all same digit)
            if (cleanCpf.Distinct().Count() == 1)
                throw new ArgumentException("Invalid CPF format", nameof(value));

            if (!IsValidCpf(cleanCpf))
                throw new ArgumentException("Invalid CPF check digits", nameof(value));

            Value = cleanCpf;
        }

        private static bool IsValidCpf(string cpf)
        {
            // Calculate first check digit
            var sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += int.Parse(cpf[i].ToString()) * (10 - i);
            }
            var remainder = sum % 11;
            var firstCheckDigit = remainder < 2 ? 0 : 11 - remainder;

            if (int.Parse(cpf[9].ToString()) != firstCheckDigit)
                return false;

            // Calculate second check digit
            sum = 0;
            for (int i = 0; i < 10; i++)
            {
                sum += int.Parse(cpf[i].ToString()) * (11 - i);
            }
            remainder = sum % 11;
            var secondCheckDigit = remainder < 2 ? 0 : 11 - remainder;

            return int.Parse(cpf[10].ToString()) == secondCheckDigit;
        }

        public string GetFormatted()
        {
            return $"{Value.Substring(0, 3)}.{Value.Substring(3, 3)}.{Value.Substring(6, 3)}-{Value.Substring(9, 2)}";
        }

        public override string ToString() => GetFormatted();

        public static implicit operator string(Cpf cpf) => cpf.Value;
    }
}
