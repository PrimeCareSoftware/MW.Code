using System;

namespace MedicSoft.Application.DTOs
{
    // Request DTOs
    public record CreateElectronicInvoiceDto(
        string Type,  // NFSe, NFe, NFCe
        string ClientCpfCnpj,
        string ClientName,
        string? ClientEmail,
        string? ClientPhone,
        string? ClientAddress,
        string? ClientCity,
        string? ClientState,
        string? ClientZipCode,
        string ServiceDescription,
        string? ServiceCode,
        decimal ServiceAmount,
        Guid? PaymentId,
        Guid? AppointmentId,
        bool AutoIssue = false
    );

    public record IssueElectronicInvoiceDto(
        Guid InvoiceId
    );

    public record CancelElectronicInvoiceDto(
        Guid InvoiceId,
        string Reason
    );

    public record SendElectronicInvoiceEmailDto(
        Guid InvoiceId,
        string Email
    );

    // Response DTOs
    public record ElectronicInvoiceDto
    {
        public Guid Id { get; init; }
        public string Type { get; init; } = null!;
        public string Number { get; init; } = null!;
        public string Series { get; init; } = null!;
        public DateTime IssueDate { get; init; }
        public string Status { get; init; } = null!;

        // Provider
        public string ProviderCnpj { get; init; } = null!;
        public string ProviderName { get; init; } = null!;
        public string? ProviderMunicipalRegistration { get; init; }

        // Client
        public string ClientCpfCnpj { get; init; } = null!;
        public string ClientName { get; init; } = null!;
        public string? ClientEmail { get; init; }
        public string? ClientPhone { get; init; }

        // Service
        public string ServiceDescription { get; init; } = null!;
        public string? ServiceCode { get; init; }
        public decimal ServiceAmount { get; init; }

        // Taxes
        public decimal IssRate { get; init; }
        public decimal IssAmount { get; init; }
        public decimal PisAmount { get; init; }
        public decimal CofinsAmount { get; init; }
        public decimal CsllAmount { get; init; }
        public decimal InssAmount { get; init; }
        public decimal IrAmount { get; init; }
        public decimal TotalTaxes { get; init; }
        public decimal NetAmount { get; init; }

        // SEFAZ
        public string? AuthorizationCode { get; init; }
        public string? AccessKey { get; init; }
        public string? VerificationCode { get; init; }
        public string? Protocol { get; init; }
        public DateTime? AuthorizationDate { get; init; }

        // Documents
        public string? PdfUrl { get; init; }
        public bool HasXml { get; init; }

        // Cancellation
        public DateTime? CancellationDate { get; init; }
        public string? CancellationReason { get; init; }

        // References
        public Guid? PaymentId { get; init; }
        public Guid? AppointmentId { get; init; }

        // Error
        public string? ErrorMessage { get; init; }
        public string? ErrorCode { get; init; }

        // Metadata
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }

    public record ElectronicInvoiceListDto
    {
        public Guid Id { get; init; }
        public string Type { get; init; } = null!;
        public string Number { get; init; } = null!;
        public DateTime IssueDate { get; init; }
        public string Status { get; init; } = null!;
        public string ClientName { get; init; } = null!;
        public decimal ServiceAmount { get; init; }
        public decimal NetAmount { get; init; }
        public string? AccessKey { get; init; }
        public string? PdfUrl { get; init; }
    }

    // Invoice Configuration DTOs
    public record CreateInvoiceConfigurationDto(
        string Cnpj,
        string CompanyName,
        string? MunicipalRegistration,
        string? StateRegistration,
        string? TradeName,
        string? Address,
        string? AddressNumber,
        string? AddressComplement,
        string? Neighborhood,
        string? City,
        string? State,
        string? ZipCode,
        string? CityCode,
        string? Phone,
        string? Email,
        string? ServiceCode,
        decimal DefaultIssRate,
        bool IssRetainedByDefault,
        bool IsSimplifiedTaxRegime,
        string? SimplifiedTaxRegimeCode,
        string Gateway,  // FocusNFe, ENotas, NFeCidades, Direct
        string? GatewayApiKey,
        string? GatewayEnvironment,
        bool AutoIssueAfterPayment,
        bool SendEmailAfterIssuance
    );

    public record UpdateInvoiceConfigurationDto(
        string? MunicipalRegistration,
        string? StateRegistration,
        string? TradeName,
        string? Address,
        string? AddressNumber,
        string? AddressComplement,
        string? Neighborhood,
        string? City,
        string? State,
        string? ZipCode,
        string? CityCode,
        string? Phone,
        string? Email,
        string? ServiceCode,
        decimal? DefaultIssRate,
        bool? IssRetainedByDefault,
        bool? IsSimplifiedTaxRegime,
        string? SimplifiedTaxRegimeCode,
        string? Gateway,
        string? GatewayApiKey,
        string? GatewayEnvironment,
        bool? AutoIssueAfterPayment,
        bool? SendEmailAfterIssuance
    );

    public record UploadCertificateDto(
        byte[] Certificate,
        string Password
    );

    public record InvoiceConfigurationDto
    {
        public Guid Id { get; init; }
        public string Cnpj { get; init; } = null!;
        public string CompanyName { get; init; } = null!;
        public string? MunicipalRegistration { get; init; }
        public string? StateRegistration { get; init; }
        public string? TradeName { get; init; }

        // Address
        public string? Address { get; init; }
        public string? AddressNumber { get; init; }
        public string? AddressComplement { get; init; }
        public string? Neighborhood { get; init; }
        public string? City { get; init; }
        public string? State { get; init; }
        public string? ZipCode { get; init; }
        public string? CityCode { get; init; }

        // Contact
        public string? Phone { get; init; }
        public string? Email { get; init; }

        // Tax
        public string? ServiceCode { get; init; }
        public decimal DefaultIssRate { get; init; }
        public bool IssRetainedByDefault { get; init; }
        public bool IsSimplifiedTaxRegime { get; init; }
        public string? SimplifiedTaxRegimeCode { get; init; }

        // Certificate
        public bool HasCertificate { get; init; }
        public DateTime? CertificateExpirationDate { get; init; }
        public bool IsCertificateExpired { get; init; }

        // Numbering
        public int CurrentInvoiceNumber { get; init; }
        public string DefaultInvoiceSeries { get; init; } = null!;
        public int CurrentRpsNumber { get; init; }

        // Gateway
        public string Gateway { get; init; } = null!;
        public bool HasGatewayApiKey { get; init; }
        public string? GatewayEnvironment { get; init; }
        public bool IsActive { get; init; }

        // Automation
        public bool AutoIssueAfterPayment { get; init; }
        public bool SendEmailAfterIssuance { get; init; }

        // Metadata
        public DateTime CreatedAt { get; init; }
        public DateTime? UpdatedAt { get; init; }
    }

    // Filter/Query DTOs
    public record ElectronicInvoiceFilterDto(
        DateTime? StartDate,
        DateTime? EndDate,
        string? Status,
        string? ClientCpfCnpj,
        string? Type
    );

    // Statistics DTOs
    public record ElectronicInvoiceStatisticsDto
    {
        public int TotalInvoices { get; init; }
        public int AuthorizedInvoices { get; init; }
        public int CancelledInvoices { get; init; }
        public int PendingInvoices { get; init; }
        public int ErrorInvoices { get; init; }
        public decimal TotalAmount { get; init; }
        public decimal TotalTaxes { get; init; }
        public decimal NetAmount { get; init; }
    }
}
