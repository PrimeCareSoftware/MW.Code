using System;
using System.Text.RegularExpressions;

namespace MedicSoft.Domain.ValueObjects
{
    /// <summary>
    /// Objeto de Valor representando um CRM (Conselho Regional de Medicina)
    /// Formato: número do CRM + UF (ex: "123456-SP")
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
                throw new ArgumentException("O número do CRM não pode estar vazio", nameof(number));

            if (string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("O estado do CRM não pode estar vazio", nameof(state));

            var cleanNumber = number.Trim();
            var cleanState = state.Trim().ToUpperInvariant();

            if (!CrmRegex.IsMatch(cleanNumber))
                throw new ArgumentException("O número do CRM deve ter entre 4 e 7 dígitos", nameof(number));

            if (!IsValidState(cleanState))
                throw new ArgumentException($"Estado inválido: {cleanState}. Deve ser uma UF brasileira válida.", nameof(state));

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
                throw new ArgumentException("A string do CRM não pode estar vazia", nameof(crmString));

            var parts = crmString.Split('-', '/');
            if (parts.Length != 2)
                throw new ArgumentException("O CRM deve estar no formato: NUMERO-UF ou NUMERO/UF", nameof(crmString));

            return new Crm(parts[0].Trim(), parts[1].Trim());
        }
    }
}
