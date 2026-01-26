namespace PatientPortal.Application.Configuration;

/// <summary>
/// Configuration settings for email notifications
/// </summary>
public class EmailSettings
{
    public const string SectionName = "Email";

    /// <summary>
    /// SMTP server address
    /// </summary>
    public string SmtpServer { get; set; } = string.Empty;

    /// <summary>
    /// SMTP server port
    /// </summary>
    public int SmtpPort { get; set; } = 587;

    /// <summary>
    /// Use SSL/TLS for SMTP connection
    /// </summary>
    public bool UseSsl { get; set; } = true;

    /// <summary>
    /// SMTP username (if authentication required)
    /// </summary>
    public string? Username { get; set; }

    /// <summary>
    /// SMTP password (if authentication required)
    /// </summary>
    public string? Password { get; set; }

    /// <summary>
    /// From email address
    /// </summary>
    public string From { get; set; } = string.Empty;

    /// <summary>
    /// From name (display name)
    /// </summary>
    public string FromName { get; set; } = string.Empty;

    /// <summary>
    /// SendGrid API key (alternative to SMTP)
    /// </summary>
    public string? SendGridApiKey { get; set; }
}
