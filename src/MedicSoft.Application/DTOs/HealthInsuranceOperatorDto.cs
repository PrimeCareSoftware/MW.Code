using System;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO for Health Insurance Operator
    /// </summary>
    public class HealthInsuranceOperatorDto
    {
        public Guid Id { get; set; }
        public string TradeName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string RegisterNumber { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? ContactPerson { get; set; }
        public bool IsActive { get; set; }
        public string IntegrationType { get; set; } = string.Empty;
        public string? WebsiteUrl { get; set; }
        public string? ApiEndpoint { get; set; }
        public bool RequiresPriorAuthorization { get; set; }
        public string? TissVersion { get; set; }
        public bool SupportsTissXml { get; set; }
        public string? BatchSubmissionEmail { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for creating a Health Insurance Operator
    /// </summary>
    public class CreateHealthInsuranceOperatorDto
    {
        public string TradeName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string RegisterNumber { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? ContactPerson { get; set; }
        public string? WebsiteUrl { get; set; }
        public bool RequiresPriorAuthorization { get; set; }
    }

    /// <summary>
    /// DTO for updating a Health Insurance Operator
    /// </summary>
    public class UpdateHealthInsuranceOperatorDto
    {
        public string TradeName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? ContactPerson { get; set; }
        public string? WebsiteUrl { get; set; }
        public bool RequiresPriorAuthorization { get; set; }
    }

    /// <summary>
    /// DTO for configuring operator integration
    /// </summary>
    public class ConfigureOperatorIntegrationDto
    {
        public string IntegrationType { get; set; } = string.Empty; // Manual, WebPortal, TissXml, RestApi
        public string? ApiEndpoint { get; set; }
        public string? ApiKey { get; set; }
        public string? WebsiteUrl { get; set; }
    }

    /// <summary>
    /// DTO for configuring TISS settings
    /// </summary>
    public class ConfigureOperatorTissDto
    {
        public string TissVersion { get; set; } = string.Empty;
        public bool SupportsTissXml { get; set; }
        public string? BatchSubmissionEmail { get; set; }
    }
}
