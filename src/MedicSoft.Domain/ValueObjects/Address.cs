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
                throw new ArgumentException("Street cannot be empty", nameof(street));
            
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Number cannot be empty", nameof(number));
            
            if (string.IsNullOrWhiteSpace(neighborhood))
                throw new ArgumentException("Neighborhood cannot be empty", nameof(neighborhood));
            
            if (string.IsNullOrWhiteSpace(city))
                throw new ArgumentException("City cannot be empty", nameof(city));
            
            if (string.IsNullOrWhiteSpace(state))
                throw new ArgumentException("State cannot be empty", nameof(state));
            
            if (string.IsNullOrWhiteSpace(zipCode))
                throw new ArgumentException("ZipCode cannot be empty", nameof(zipCode));
            
            if (string.IsNullOrWhiteSpace(country))
                throw new ArgumentException("Country cannot be empty", nameof(country));

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