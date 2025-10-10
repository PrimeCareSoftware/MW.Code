namespace MedicSoft.Domain.Interfaces
{
    /// <summary>
    /// Service for password hashing and verification using BCrypt
    /// </summary>
    public interface IPasswordHasher
    {
        /// <summary>
        /// Hashes a password using BCrypt
        /// </summary>
        string HashPassword(string password);

        /// <summary>
        /// Verifies a password against a hash
        /// </summary>
        bool VerifyPassword(string password, string hashedPassword);

        /// <summary>
        /// Validates password strength based on security requirements
        /// </summary>
        (bool IsValid, string ErrorMessage) ValidatePasswordStrength(string password, int minLength = 8);
    }
}
