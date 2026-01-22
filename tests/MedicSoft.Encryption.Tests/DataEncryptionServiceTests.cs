using System;
using System.Security.Cryptography;
using FluentAssertions;
using MedicSoft.CrossCutting.Security;
using Xunit;

namespace MedicSoft.Encryption.Tests
{
    public class DataEncryptionServiceTests
    {
        private readonly DataEncryptionService _encryptionService;
        private readonly string _testKey;

        public DataEncryptionServiceTests()
        {
            // Generate a valid test key
            _testKey = DataEncryptionService.GenerateKey();
            _encryptionService = new DataEncryptionService(_testKey);
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenKeyIsNull()
        {
            // Arrange & Act
            Action act = () => new DataEncryptionService(null!);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Encryption key cannot be null or empty*");
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenKeyIsEmpty()
        {
            // Arrange & Act
            Action act = () => new DataEncryptionService(string.Empty);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Encryption key cannot be null or empty*");
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenKeyIsNotBase64()
        {
            // Arrange & Act
            Action act = () => new DataEncryptionService("not-a-valid-base64-key!");

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Encryption key must be a valid Base64 string*");
        }

        [Fact]
        public void Constructor_ShouldThrowException_WhenKeyIsWrongSize()
        {
            // Arrange - Create a 128-bit key instead of 256-bit
            var wrongSizeKey = Convert.ToBase64String(new byte[16]);

            // Act
            Action act = () => new DataEncryptionService(wrongSizeKey);

            // Assert
            act.Should().Throw<ArgumentException>()
                .WithMessage("*Encryption key must be 256 bits*");
        }

        [Fact]
        public void Encrypt_ShouldReturnNull_WhenInputIsNull()
        {
            // Arrange
            string? input = null;

            // Act
            var result = _encryptionService.Encrypt(input);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Encrypt_ShouldReturnWhitespace_WhenInputIsWhitespace()
        {
            // Arrange
            var input = "   ";

            // Act
            var result = _encryptionService.Encrypt(input);

            // Assert
            result.Should().Be(input);
        }

        [Fact]
        public void Encrypt_ShouldReturnDifferentValue_ThanOriginalText()
        {
            // Arrange
            var plainText = "Sensitive medical information";

            // Act
            var encrypted = _encryptionService.Encrypt(plainText);

            // Assert
            encrypted.Should().NotBeNullOrWhiteSpace();
            encrypted.Should().NotBe(plainText);
        }

        [Fact]
        public void Encrypt_ShouldReturnBase64String()
        {
            // Arrange
            var plainText = "Patient has diabetes and hypertension";

            // Act
            var encrypted = _encryptionService.Encrypt(plainText);

            // Assert
            encrypted.Should().NotBeNullOrWhiteSpace();
            
            // Should be able to decode from Base64
            Action act = () => Convert.FromBase64String(encrypted!);
            act.Should().NotThrow();
        }

        [Fact]
        public void Encrypt_ShouldReturnDifferentCipherText_ForSameInput()
        {
            // Arrange
            var plainText = "Patient allergic to penicillin";

            // Act
            var encrypted1 = _encryptionService.Encrypt(plainText);
            var encrypted2 = _encryptionService.Encrypt(plainText);

            // Assert - Different because of random nonce
            encrypted1.Should().NotBe(encrypted2);
        }

        [Fact]
        public void Decrypt_ShouldReturnNull_WhenInputIsNull()
        {
            // Arrange
            string? input = null;

            // Act
            var result = _encryptionService.Decrypt(input);

            // Assert
            result.Should().BeNull();
        }

        [Fact]
        public void Decrypt_ShouldReturnWhitespace_WhenInputIsWhitespace()
        {
            // Arrange
            var input = "   ";

            // Act
            var result = _encryptionService.Decrypt(input);

            // Assert
            result.Should().Be(input);
        }

        [Fact]
        public void Decrypt_ShouldRestoreOriginalText_AfterEncryption()
        {
            // Arrange
            var originalText = "Patient diagnosis: Type 2 Diabetes Mellitus";

            // Act
            var encrypted = _encryptionService.Encrypt(originalText);
            var decrypted = _encryptionService.Decrypt(encrypted);

            // Assert
            decrypted.Should().Be(originalText);
        }

        [Fact]
        public void Decrypt_ShouldHandleLongText()
        {
            // Arrange
            var longText = new string('A', 10000) + " medical history with detailed information";

            // Act
            var encrypted = _encryptionService.Encrypt(longText);
            var decrypted = _encryptionService.Decrypt(encrypted);

            // Assert
            decrypted.Should().Be(longText);
        }

        [Fact]
        public void Decrypt_ShouldHandleSpecialCharacters()
        {
            // Arrange
            var textWithSpecialChars = "Patient: João Silva - Symptoms: ♥ palpitations, 中文字符, emoji 😊";

            // Act
            var encrypted = _encryptionService.Encrypt(textWithSpecialChars);
            var decrypted = _encryptionService.Decrypt(encrypted);

            // Assert
            decrypted.Should().Be(textWithSpecialChars);
        }

        [Fact]
        public void Decrypt_ShouldThrowException_WhenCipherTextIsInvalid()
        {
            // Arrange
            var invalidCipherText = "InvalidBase64Data!!!";

            // Act
            Action act = () => _encryptionService.Decrypt(invalidCipherText);

            // Assert
            act.Should().Throw<CryptographicException>()
                .WithMessage("*Invalid encrypted data format*");
        }

        [Fact]
        public void Decrypt_ShouldThrowException_WhenCipherTextIsTooShort()
        {
            // Arrange - Base64 string that's too short (less than nonce + tag size)
            var shortCipherText = Convert.ToBase64String(new byte[10]);

            // Act
            Action act = () => _encryptionService.Decrypt(shortCipherText);

            // Assert
            act.Should().Throw<CryptographicException>()
                .WithMessage("*Invalid encrypted data format*");
        }

        [Fact]
        public void Decrypt_ShouldThrowException_WhenUsingWrongKey()
        {
            // Arrange
            var plainText = "Confidential patient information";
            var encrypted = _encryptionService.Encrypt(plainText);
            
            // Create a different encryption service with a different key
            var differentKey = DataEncryptionService.GenerateKey();
            var differentService = new DataEncryptionService(differentKey);

            // Act
            Action act = () => differentService.Decrypt(encrypted);

            // Assert
            act.Should().Throw<CryptographicException>();
        }

        [Fact]
        public void Decrypt_ShouldThrowException_WhenDataIsCorrupted()
        {
            // Arrange
            var plainText = "Patient medical record";
            var encrypted = _encryptionService.Encrypt(plainText);
            
            // Corrupt the encrypted data by changing a character
            var encryptedBytes = Convert.FromBase64String(encrypted!);
            encryptedBytes[encryptedBytes.Length - 1] ^= 0xFF; // Flip all bits of last byte
            var corruptedEncrypted = Convert.ToBase64String(encryptedBytes);

            // Act
            Action act = () => _encryptionService.Decrypt(corruptedEncrypted);

            // Assert
            act.Should().Throw<CryptographicException>();
        }

        [Fact]
        public void GenerateKey_ShouldReturnValidBase64String()
        {
            // Act
            var key = DataEncryptionService.GenerateKey();

            // Assert
            key.Should().NotBeNullOrWhiteSpace();
            
            // Should be able to decode from Base64
            Action act = () => Convert.FromBase64String(key);
            act.Should().NotThrow();
        }

        [Fact]
        public void GenerateKey_ShouldReturn256BitKey()
        {
            // Act
            var key = DataEncryptionService.GenerateKey();
            var keyBytes = Convert.FromBase64String(key);

            // Assert
            keyBytes.Length.Should().Be(32); // 256 bits = 32 bytes
        }

        [Fact]
        public void GenerateKey_ShouldReturnDifferentKeys()
        {
            // Act
            var key1 = DataEncryptionService.GenerateKey();
            var key2 = DataEncryptionService.GenerateKey();

            // Assert
            key1.Should().NotBe(key2);
        }

        [Fact]
        public void GeneratedKey_ShouldBeUsableForEncryption()
        {
            // Arrange
            var key = DataEncryptionService.GenerateKey();
            var service = new DataEncryptionService(key);
            var plainText = "Test medical data";

            // Act
            var encrypted = service.Encrypt(plainText);
            var decrypted = service.Decrypt(encrypted);

            // Assert
            decrypted.Should().Be(plainText);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Short")]
        [InlineData("This is a medium length medical record")]
        [InlineData("This is a very long medical record with lots of detailed information about the patient's medical history, including diagnoses, treatments, medications, allergies, and other relevant clinical data that needs to be kept confidential and secure.")]
        public void EncryptDecrypt_ShouldWorkForVariousTextLengths(string plainText)
        {
            // Act
            var encrypted = _encryptionService.Encrypt(plainText);
            var decrypted = _encryptionService.Decrypt(encrypted);

            // Assert
            if (string.IsNullOrWhiteSpace(plainText))
            {
                decrypted.Should().Be(plainText);
            }
            else
            {
                decrypted.Should().Be(plainText);
            }
        }

        [Fact]
        public void EncryptDecrypt_ShouldPreserveNewlines()
        {
            // Arrange
            var textWithNewlines = "Line 1\nLine 2\r\nLine 3";

            // Act
            var encrypted = _encryptionService.Encrypt(textWithNewlines);
            var decrypted = _encryptionService.Decrypt(encrypted);

            // Assert
            decrypted.Should().Be(textWithNewlines);
        }
    }
}
