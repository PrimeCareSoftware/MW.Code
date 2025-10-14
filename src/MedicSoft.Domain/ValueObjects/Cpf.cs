using System;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.ValueObjects
{
    /// <summary>
    /// Objeto de Valor representando um CPF brasileiro (Cadastro de Pessoas Físicas)
    /// Valida o formato do CPF e os dígitos verificadores
    /// </summary>
    public record Cpf
    {
        public string Value { get; }

        public Cpf(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("O CPF não pode estar vazio", nameof(value));

            // Remove caracteres não numéricos
            var cleanCpf = new string(value.Where(char.IsDigit).ToArray());

            if (cleanCpf.Length != DocumentConstants.CpfLength)
                throw new ArgumentException($"O CPF deve ter {DocumentConstants.CpfLength} dígitos", nameof(value));

            // Verifica CPFs inválidos conhecidos (todos os dígitos iguais)
            if (cleanCpf.Distinct().Count() == 1)
                throw new ArgumentException("Formato de CPF inválido", nameof(value));

            if (!IsValidCpf(cleanCpf))
                throw new ArgumentException("Dígitos verificadores do CPF inválidos", nameof(value));

            Value = cleanCpf;
        }

        private static bool IsValidCpf(string cpf)
        {
            // Calcula o primeiro dígito verificador
            var sum = 0;
            for (int i = 0; i < 9; i++)
            {
                sum += int.Parse(cpf[i].ToString()) * (10 - i);
            }
            var remainder = sum % 11;
            var firstCheckDigit = remainder < 2 ? 0 : 11 - remainder;

            if (int.Parse(cpf[9].ToString()) != firstCheckDigit)
                return false;

            // Calcula o segundo dígito verificador
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
