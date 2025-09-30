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
                throw new ArgumentException("Country code cannot be empty", nameof(countryCode));
            
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Phone number cannot be empty", nameof(number));

            CountryCode = countryCode.Trim();
            Number = number.Trim();
        }

        public override string ToString() => $"{CountryCode} {Number}";
    }
}