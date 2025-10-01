using System;
using System.Text.RegularExpressions;

namespace MedicSoft.Domain.ValueObjects
{
    public record Email
    {
        private static readonly Regex EmailRegex = new Regex(
            @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public string Value { get; }

        public Email(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Email cannot be empty", nameof(value));

            if (!EmailRegex.IsMatch(value))
                throw new ArgumentException("Invalid email format", nameof(value));

            Value = value.Trim().ToLowerInvariant();
        }

        public override string ToString() => Value;

        public static implicit operator string(Email email) => email.Value;
        public static implicit operator Email(string email) => new(email);
    }
}