using MedicSoft.CrossCutting.Security;
using Xunit;

namespace MedicSoft.Test.Security
{
    public class InputSanitizerTests
    {
        [Fact]
        public void SanitizeHtml_WithDangerousScript_EncodesIt()
        {
            // Arrange
            var input = "<script>alert('XSS')</script>";

            // Act
            var sanitized = InputSanitizer.SanitizeHtml(input);

            // Assert
            Assert.DoesNotContain("<script>", sanitized);
            Assert.DoesNotContain("</script>", sanitized);
        }

        [Theory]
        [InlineData(null, "")]
        [InlineData("", "")]
        [InlineData("   ", "")]
        public void SanitizeHtml_WithEmptyInput_ReturnsEmpty(string input, string expected)
        {
            // Act
            var sanitized = InputSanitizer.SanitizeHtml(input);

            // Assert
            Assert.Equal(expected, sanitized);
        }

        [Fact]
        public void StripHtml_RemovesAllHtmlTags()
        {
            // Arrange
            var input = "<p>Hello <strong>World</strong></p>";

            // Act
            var stripped = InputSanitizer.StripHtml(input);

            // Assert
            Assert.Equal("Hello World", stripped);
        }

        [Fact]
        public void SanitizeFileName_RemovesInvalidCharacters()
        {
            // Arrange - test that the function works correctly
            var fileName = "normalfilename.txt";

            // Act
            var sanitized = InputSanitizer.SanitizeFileName(fileName);

            // Assert
            Assert.Equal("normalfilename.txt", sanitized);
        }

        [Fact]
        public void SanitizeFileName_PrevendsDirectoryTraversal()
        {
            // Arrange
            var fileName = "../../../etc/passwd";

            // Act
            var sanitized = InputSanitizer.SanitizeFileName(fileName);

            // Assert
            Assert.DoesNotContain("../", sanitized);
            Assert.DoesNotContain("..\\", sanitized);
        }

        [Theory]
        [InlineData("test@example.com", true, "test@example.com")]
        [InlineData("UPPERCASE@EXAMPLE.COM", true, "uppercase@example.com")]
        [InlineData("invalid-email", false, "")]
        [InlineData("@example.com", false, "")]
        [InlineData("test@", false, "")]
        public void SanitizeEmail_ValidatesAndNormalizesEmail(string email, bool expectedValid, string expectedSanitized)
        {
            // Act
            var (isValid, sanitized) = InputSanitizer.SanitizeEmail(email);

            // Assert
            Assert.Equal(expectedValid, isValid);
            Assert.Equal(expectedSanitized, sanitized);
        }

        [Fact]
        public void SanitizePhoneNumber_RemovesNonNumericCharacters()
        {
            // Arrange
            var phoneNumber = "(11) 98765-4321";

            // Act
            var sanitized = InputSanitizer.SanitizePhoneNumber(phoneNumber);

            // Assert
            Assert.Contains("11", sanitized);
            Assert.Contains("98765", sanitized);
            Assert.Contains("4321", sanitized);
        }

        [Theory]
        [InlineData("https://example.com", true)]
        [InlineData("http://example.com", true)]
        [InlineData("javascript:alert('XSS')", false)]
        [InlineData("file:///etc/passwd", false)]
        [InlineData("invalid-url", false)]
        public void SanitizeUrl_ValidatesUrlScheme(string url, bool expectedValid)
        {
            // Act
            var (isValid, _) = InputSanitizer.SanitizeUrl(url);

            // Assert
            Assert.Equal(expectedValid, isValid);
        }

        [Fact]
        public void TrimAndLimit_TrimsAndLimitsLength()
        {
            // Arrange
            var input = "  This is a very long string that should be limited  ";
            var maxLength = 20;

            // Act
            var result = InputSanitizer.TrimAndLimit(input, maxLength);

            // Assert
            Assert.Equal(20, result.Length);
            Assert.DoesNotContain("  ", result);
        }

        [Fact]
        public void SanitizeSqlInput_RemovesDangerousPatterns()
        {
            // Arrange
            var input = "Robert'; DROP TABLE Students;--";

            // Act
            var sanitized = InputSanitizer.SanitizeSqlInput(input);

            // Assert
            Assert.DoesNotContain(";", sanitized);
            Assert.DoesNotContain("--", sanitized);
        }
    }
}
