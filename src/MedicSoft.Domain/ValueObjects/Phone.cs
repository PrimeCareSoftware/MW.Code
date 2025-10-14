using System;

namespace MedicSoft.Domain.ValueObjects
{
    public record Phone
    {
        public string CountryCode { get; }
        public string Number { get; }

        public Phone(string countryCode, string number)
        {
            if (string.IsNullOrWhiteSpace(countryCode))
                throw new ArgumentException("O código do país não pode estar vazio", nameof(countryCode));
            
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("O número de telefone não pode estar vazio", nameof(number));

            CountryCode = countryCode.Trim();
            Number = number.Trim();
        }

        public override string ToString() => $"{CountryCode} {Number}";
    }
}