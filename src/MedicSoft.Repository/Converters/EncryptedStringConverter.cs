using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using MedicSoft.CrossCutting.Security;

namespace MedicSoft.Repository.Converters
{
    /// <summary>
    /// EF Core Value Converter for automatic encryption/decryption of sensitive fields.
    /// Transparently encrypts data before storing and decrypts when retrieving from database.
    /// </summary>
    public class EncryptedStringConverter : ValueConverter<string?, string?>
    {
        public EncryptedStringConverter(IDataEncryptionService encryptionService)
            : base(
                plainText => encryptionService.Encrypt(plainText),
                cipherText => encryptionService.Decrypt(cipherText))
        {
        }
    }
}
