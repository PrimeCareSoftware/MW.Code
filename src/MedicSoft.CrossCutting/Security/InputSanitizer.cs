using System.Text.RegularExpressions;
using System.Web;

namespace MedicSoft.CrossCutting.Security
{
    /// <summary>
    /// Service for sanitizing user inputs to prevent XSS and injection attacks
    /// </summary>
    public static class InputSanitizer
    {
        /// <summary>
        /// Sanitizes HTML content by removing dangerous tags and attributes
        /// </summary>
        public static string SanitizeHtml(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // HTML encode the input to prevent XSS
            var sanitized = HttpUtility.HtmlEncode(input);
            
            return sanitized;
        }

        /// <summary>
        /// Removes all HTML tags from input
        /// </summary>
        public static string StripHtml(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Remove HTML tags
            var stripped = Regex.Replace(input, @"<[^>]*>", string.Empty);
            
            // Decode HTML entities
            stripped = HttpUtility.HtmlDecode(stripped);
            
            return stripped.Trim();
        }

        /// <summary>
        /// Sanitizes SQL input by removing dangerous characters
        /// Note: This should be used in addition to parameterized queries, not as a replacement
        /// </summary>
        public static string SanitizeSqlInput(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            // Remove SQL injection patterns (defense in depth, use parameterized queries as primary defense)
            var sanitized = input
                .Replace("'", "''")  // Escape single quotes
                .Replace("--", "")    // Remove SQL comments
                .Replace(";", "")     // Remove statement terminators
                .Replace("/*", "")    // Remove multi-line comment start
                .Replace("*/", "");   // Remove multi-line comment end

            return sanitized;
        }

        /// <summary>
        /// Sanitizes input for use in filenames
        /// </summary>
        public static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return string.Empty;

            // Remove invalid filename characters
            var invalidChars = Path.GetInvalidFileNameChars();
            var sanitized = new string(fileName.Where(c => !invalidChars.Contains(c)).ToArray());

            // Remove potentially dangerous patterns
            sanitized = Regex.Replace(sanitized, @"\.\.+", "");  // Remove directory traversal
            sanitized = sanitized.Replace("../", "").Replace("..\\", "");

            return sanitized.Trim();
        }

        /// <summary>
        /// Validates and sanitizes email addresses
        /// </summary>
        public static (bool IsValid, string Sanitized) SanitizeEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return (false, string.Empty);

            var trimmed = email.Trim().ToLowerInvariant();

            // Basic email validation regex
            var emailPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            var isValid = Regex.IsMatch(trimmed, emailPattern);

            return isValid ? (true, trimmed) : (false, string.Empty);
        }

        /// <summary>
        /// Sanitizes phone numbers by removing non-numeric characters
        /// </summary>
        public static string SanitizePhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrWhiteSpace(phoneNumber))
                return string.Empty;

            // Keep only digits, +, -, (, ), and spaces
            var sanitized = Regex.Replace(phoneNumber, @"[^\d\+\-\(\)\s]", "");
            
            return sanitized.Trim();
        }

        /// <summary>
        /// Sanitizes URLs by validating and encoding
        /// </summary>
        public static (bool IsValid, string Sanitized) SanitizeUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return (false, string.Empty);

            try
            {
                var uri = new Uri(url, UriKind.Absolute);
                
                // Only allow HTTP and HTTPS
                if (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)
                    return (false, string.Empty);

                return (true, uri.AbsoluteUri);
            }
            catch
            {
                return (false, string.Empty);
            }
        }

        /// <summary>
        /// Trims and limits string length
        /// </summary>
        public static string TrimAndLimit(string input, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(input))
                return string.Empty;

            var trimmed = input.Trim();
            
            return trimmed.Length > maxLength 
                ? trimmed.Substring(0, maxLength) 
                : trimmed;
        }
    }
}
