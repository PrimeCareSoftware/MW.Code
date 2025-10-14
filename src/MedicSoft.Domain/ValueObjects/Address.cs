using System;

namespace MedicSoft.Domain.ValueObjects
{
    public record Address
    {
        public string Street { get; }
        public string Number { get; }
        public string? Complement { get; }
        public string Neighborhood { get; }
        public string City { get; }
        public string State { get; }
        public string ZipCode { get; }
        public string Country { get; }

        public Address(string street, string number, string neighborhood, 
            string city, string state, string zipCode, string country, string? complement = null)
        {
            if (string.IsNullOrWhiteSpace(street))
                throw new ArgumentException("A rua não pode estar vazia", nameof(street));
            
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("O número não pode estar vazio", nameof(number));
            
            if (string.IsNullOrWhiteSpace(neighborhood))
                throw new ArgumentException("O bairro não pode estar vazio", nameof(neighborhood));
            
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("A cidade não pode estar vazia", nameof(city));
            
            if (string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("O estado não pode estar vazio", nameof(state));
            
            if (string.IsNullOrWhiteSpace(zipCode))
                throw new ArgumentException("O CEP não pode estar vazio", nameof(zipCode));
            
            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("O país não pode estar vazio", nameof(country));

            Street = street.Trim();
            Number = number.Trim();
            Complement = complement?.Trim();
            Neighborhood = neighborhood.Trim();
            City = city.Trim();
            State = state.Trim();
            ZipCode = zipCode.Trim();
            Country = country.Trim();
        }

        public override string ToString()
        {
            var complementText = !string.IsNullOrEmpty(Complement) ? $", {Complement}" : "";
            return $"{Street}, {Number}{complementText}, {Neighborhood}, {City}, {State}, {ZipCode}, {Country}";
        }
    }
}