using System;
using System.Text.RegularExpressions;

namespace MedicSoft.Domain.ValueObjects
{
    /// <summary>
    /// Value Object representing a CRM (Conselho Regional de Medicina)
    /// Format: CRM number + UF (e.g., "123456-SP")
    /// </summary>
    public record Crm
    {
        private static readonly Regex CrmRegex = new Regex(
            @"^\d{4,7}$",
            RegexOptions.Compiled);

        private static readonly string[] ValidUFs = 
        {
            "AC", "AL", "AP", "AM", "BA", "CE", "DF", "ES", "GO", "MA",
            "MT", "MS", "MG", "PA", "PB", "PR", "PE", "PI", "RJ", "RN",
            "RS", "RO", "RR", "SC", "SP", "SE", "TO"
        };

        public string Number { get; }
        public string State { get; }

        public Crm(string number, string state)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("CRM number cannot be empty", nameof(number));

            if (string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("CRM state cannot be empty", nameof(state));

            var cleanNumber = number.Trim();
            var cleanState = state.Trim().ToUpperInvariant();

            if (!CrmRegex.IsMatch(cleanNumber))
                throw new ArgumentException("CRM number must have between 4 and 7 digits", nameof(number));

            if (!IsValidState(cleanState))
                throw new ArgumentException($"Invalid state: {cleanState}. Must be a valid Brazilian UF.", nameof(state));

            Number = cleanNumber;
            State = cleanState;
        }

        private static bool IsValidState(string state)
        {
            return Array.Exists(ValidUFs, uf => uf == state);
        }

        public override string ToString() => $"{Number}-{State}";

        public static Crm Parse(string crmString)
        {
            if (string.IsNullOrWhiteSpace(crmString))
                throw new ArgumentException("CRM string cannot be empty", nameof(crmString));

            var parts = crmString.Split('-', '/');
            if (parts.Length != 2)
                throw new ArgumentException("CRM must be in format: NUMBER-UF or NUMBER/UF", nameof(crmString));

            return new Crm(parts[0].Trim(), parts[1].Trim());
        }
    }
}
