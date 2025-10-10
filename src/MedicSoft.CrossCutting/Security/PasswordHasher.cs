using System.Text.RegularExpressions;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.CrossCutting.Security
{
    /// <summary>
    /// Implementation of password hashing using BCrypt
    /// </summary>
    public class PasswordHasher : IPasswordHasher
    {
        private const int WorkFactor = 12;

        public string HashPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Password cannot be null or empty", nameof(password));

            return BCrypt.Net.BCrypt.HashPassword(password, WorkFactor);
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            if (string.IsNullOrWhiteSpace(password))
                return false;

            if (string.IsNullOrWhiteSpace(hashedPassword))
                return false;

            try
            {
                return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
            }
            catch
            {
                return false;
            }
        }

        public (bool IsValid, string ErrorMessage) ValidatePasswordStrength(string password, int minLength = 8)
        {
            if (string.IsNullOrWhiteSpace(password))
                return (false, "Password cannot be empty");

            if (password.Length < minLength)
                return (false, $"Password must be at least {minLength} characters long");

            if (!Regex.IsMatch(password, @"[a-z]"))
                return (false, "Password must contain at least one lowercase letter");

            if (!Regex.IsMatch(password, @"[A-Z]"))
                return (false, "Password must contain at least one uppercase letter");

            if (!Regex.IsMatch(password, @"[0-9]"))
                return (false, "Password must contain at least one digit");

            if (!Regex.IsMatch(password, @"[^a-zA-Z0-9]"))
                return (false, "Password must contain at least one special character");

            // Check for common weak passwords
            var weakPasswords = new[] { "Password", "password", "12345678", "qwerty" };
            if (weakPasswords.Any(weak => password.Contains(weak, StringComparison.OrdinalIgnoreCase)))
                return (false, "Password contains common weak patterns");

            return (true, string.Empty);
        }
    }
}
