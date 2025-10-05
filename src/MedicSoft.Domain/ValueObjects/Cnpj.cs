using System;
using System.Linq;

namespace MedicSoft.Domain.ValueObjects
{
    /// <summary>
    /// Value Object representing a Brazilian CNPJ (Cadastro Nacional da Pessoa Jurídica)
    /// Validates CNPJ format and check digits
    /// </summary>
    public record Cnpj
    {
        public string Value { get; }

        public Cnpj(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("CNPJ cannot be empty", nameof(value));

            // Remove non-numeric characters
            var cleanCnpj = new string(value.Where(char.IsDigit).ToArray());

            if (cleanCnpj.Length != 14)
                throw new ArgumentException("CNPJ must have 14 digits", nameof(value));

            // Check for known invalid CNPJs (all same digit)
            if (cleanCnpj.Distinct().Count() == 1)
                throw new ArgumentException("Invalid CNPJ format", nameof(value));

            if (!IsValidCnpj(cleanCnpj))
                throw new ArgumentException("Invalid CNPJ check digits", nameof(value));

            Value = cleanCnpj;
        }

        private static bool IsValidCnpj(string cnpj)
        {
            // Calculate first check digit
            int[] multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            var sum = 0;
            for (int i = 0; i < 12; i++)
            {
                sum += int.Parse(cnpj[i].ToString()) * multiplier1[i];
            }
            var remainder = sum % 11;
            var firstCheckDigit = remainder < 2 ? 0 : 11 - remainder;

            if (int.Parse(cnpj[12].ToString()) != firstCheckDigit)
                return false;

            // Calculate second check digit
            int[] multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
            sum = 0;
            for (int i = 0; i < 13; i++)
            {
                sum += int.Parse(cnpj[i].ToString()) * multiplier2[i];
            }
            remainder = sum % 11;
            var secondCheckDigit = remainder < 2 ? 0 : 11 - remainder;

            return int.Parse(cnpj[13].ToString()) == secondCheckDigit;
        }

        public string GetFormatted()
        {
            return $"{Value.Substring(0, 2)}.{Value.Substring(2, 3)}.{Value.Substring(5, 3)}/{Value.Substring(8, 4)}-{Value.Substring(12, 2)}";
        }

        public override string ToString() => GetFormatted();

        public static implicit operator string(Cnpj cnpj) => cnpj.Value;
    }
}
