using System;
using System.Linq;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.ValueObjects
{
    /// <summary>
    /// Objeto de Valor representando um CNPJ brasileiro (Cadastro Nacional da Pessoa Jurídica)
    /// Valida o formato do CNPJ e os dígitos verificadores
    /// </summary>
    public record Cnpj
    {
        public string Value { get; }

        public Cnpj(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("O CNPJ não pode estar vazio", nameof(value));

            // Remove caracteres não numéricos
            var cleanCnpj = new string(value.Where(char.IsDigit).ToArray());

            if (cleanCnpj.Length != DocumentConstants.CnpjLength)
                throw new ArgumentException($"O CNPJ deve ter {DocumentConstants.CnpjLength} dígitos", nameof(value));

            // Verifica CNPJs inválidos conhecidos (todos os dígitos iguais)
            if (cleanCnpj.Distinct().Count() == 1)
                throw new ArgumentException("Formato de CNPJ inválido", nameof(value));

            if (!IsValidCnpj(cleanCnpj))
                throw new ArgumentException("Dígitos verificadores do CNPJ inválidos", nameof(value));

            Value = cleanCnpj;
        }

        private static bool IsValidCnpj(string cnpj)
        {
            // Calcula o primeiro dígito verificador
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

            // Calcula o segundo dígito verificador
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
