using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public enum InvoiceGateway
    {
        FocusNFe = 1,
        ENotas = 2,
        NFeCidades = 3,
        Direct = 4  // Integração direta com SEFAZ
    }

    /// <summary>
    /// Configuration for electronic invoice (NF-e/NFS-e) issuance for a tenant.
    /// Stores fiscal data, certificates, and gateway settings.
    /// </summary>
    public class InvoiceConfiguration : BaseEntity
    {
        // Company Fiscal Data
        public string Cnpj { get; private set; } = null!;
        public string CompanyName { get; private set; } = null!;
        public string? MunicipalRegistration { get; private set; }
        public string? StateRegistration { get; private set; }
        public string? TradeName { get; private set; }

        // Address
        public string? Address { get; private set; }
        public string? AddressNumber { get; private set; }
        public string? AddressComplement { get; private set; }
        public string? Neighborhood { get; private set; }
        public string? City { get; private set; }
        public string? State { get; private set; }
        public string? ZipCode { get; private set; }
        public string? CityCode { get; private set; }  // IBGE City Code

        // Contact
        public string? Phone { get; private set; }
        public string? Email { get; private set; }

        // Tax Configuration
        public string? ServiceCode { get; private set; }  // CNAE or Municipal Service Code
        public decimal DefaultIssRate { get; private set; } // Default ISS Rate (%)
        public bool IssRetainedByDefault { get; private set; }

        // Simplified Tax Regime
        public bool IsSimplifiedTaxRegime { get; private set; }  // Simples Nacional
        public string? SimplifiedTaxRegimeCode { get; private set; }

        // Digital Certificate (A1 - stored in database)
        public byte[]? DigitalCertificate { get; private set; }
        public string? CertificatePassword { get; private set; }
        public DateTime? CertificateExpirationDate { get; private set; }
        public string? CertificateThumbprint { get; private set; }

        // Invoice Numbering
        public int CurrentInvoiceNumber { get; private set; }
        public string DefaultInvoiceSeries { get; private set; } = "1";
        public int CurrentRpsNumber { get; private set; }  // RPS = Recibo Provisório de Serviços

        // Gateway Configuration
        public InvoiceGateway Gateway { get; private set; }
        public string? GatewayApiKey { get; private set; }
        public string? GatewayEnvironment { get; private set; }  // production, homologation
        public bool IsActive { get; private set; }

        // Additional Settings
        public bool AutoIssueAfterPayment { get; private set; }
        public bool SendEmailAfterIssuance { get; private set; }
        public string? EmailTemplate { get; private set; }

        private InvoiceConfiguration()
        {
            // EF Constructor
        }

        public InvoiceConfiguration(
            string cnpj,
            string companyName,
            InvoiceGateway gateway,
            string tenantId,
            string? municipalRegistration = null,
            string? cityCode = null,
            decimal defaultIssRate = 0) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(cnpj))
                throw new ArgumentException("CNPJ is required", nameof(cnpj));

            if (string.IsNullOrWhiteSpace(companyName))
                throw new ArgumentException("Company name is required", nameof(companyName));

            Cnpj = cnpj.Trim();
            CompanyName = companyName.Trim();
            Gateway = gateway;
            MunicipalRegistration = municipalRegistration?.Trim();
            CityCode = cityCode?.Trim();
            DefaultIssRate = defaultIssRate;

            CurrentInvoiceNumber = 0;
            CurrentRpsNumber = 0;
            DefaultInvoiceSeries = "1";
            IsActive = false;  // Must be activated after configuration is complete
            GatewayEnvironment = "homologation";  // Start in test mode
        }

        public void UpdateFiscalData(
            string? municipalRegistration = null,
            string? stateRegistration = null,
            string? tradeName = null,
            string? serviceCode = null,
            decimal? defaultIssRate = null,
            bool? issRetainedByDefault = null)
        {
            if (!string.IsNullOrWhiteSpace(municipalRegistration))
                MunicipalRegistration = municipalRegistration.Trim();

            if (!string.IsNullOrWhiteSpace(stateRegistration))
                StateRegistration = stateRegistration.Trim();

            if (!string.IsNullOrWhiteSpace(tradeName))
                TradeName = tradeName.Trim();

            if (!string.IsNullOrWhiteSpace(serviceCode))
                ServiceCode = serviceCode.Trim();

            if (defaultIssRate.HasValue && defaultIssRate.Value >= 0)
                DefaultIssRate = defaultIssRate.Value;

            if (issRetainedByDefault.HasValue)
                IssRetainedByDefault = issRetainedByDefault.Value;

            UpdateTimestamp();
        }

        public void UpdateAddress(
            string? address = null,
            string? addressNumber = null,
            string? addressComplement = null,
            string? neighborhood = null,
            string? city = null,
            string? state = null,
            string? zipCode = null,
            string? cityCode = null)
        {
            if (!string.IsNullOrWhiteSpace(address))
                Address = address.Trim();

            if (!string.IsNullOrWhiteSpace(addressNumber))
                AddressNumber = addressNumber.Trim();

            if (!string.IsNullOrWhiteSpace(addressComplement))
                AddressComplement = addressComplement.Trim();

            if (!string.IsNullOrWhiteSpace(neighborhood))
                Neighborhood = neighborhood.Trim();

            if (!string.IsNullOrWhiteSpace(city))
                City = city.Trim();

            if (!string.IsNullOrWhiteSpace(state))
                State = state.Trim();

            if (!string.IsNullOrWhiteSpace(zipCode))
                ZipCode = zipCode.Trim();

            if (!string.IsNullOrWhiteSpace(cityCode))
                CityCode = cityCode.Trim();

            UpdateTimestamp();
        }

        public void UpdateContact(string? phone = null, string? email = null)
        {
            if (!string.IsNullOrWhiteSpace(phone))
                Phone = phone.Trim();

            if (!string.IsNullOrWhiteSpace(email))
                Email = email.Trim();

            UpdateTimestamp();
        }

        public void SetDigitalCertificate(
            byte[] certificate,
            string password,
            DateTime expirationDate,
            string? thumbprint = null)
        {
            if (certificate == null || certificate.Length == 0)
                throw new ArgumentException("Certificate cannot be empty", nameof(certificate));

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException("Certificate password is required", nameof(password));

            if (expirationDate <= DateTime.UtcNow)
                throw new ArgumentException("Certificate has already expired", nameof(expirationDate));

            DigitalCertificate = certificate;
            CertificatePassword = password;
            CertificateExpirationDate = expirationDate;
            CertificateThumbprint = thumbprint?.Trim();

            UpdateTimestamp();
        }

        public void RemoveDigitalCertificate()
        {
            DigitalCertificate = null;
            CertificatePassword = null;
            CertificateExpirationDate = null;
            CertificateThumbprint = null;

            UpdateTimestamp();
        }

        public bool IsCertificateExpired()
        {
            return CertificateExpirationDate.HasValue &&
                   CertificateExpirationDate.Value <= DateTime.UtcNow;
        }

        public bool HasValidCertificate()
        {
            return DigitalCertificate != null &&
                   !string.IsNullOrWhiteSpace(CertificatePassword) &&
                   !IsCertificateExpired();
        }

        public void UpdateGatewaySettings(
            InvoiceGateway? gateway = null,
            string? apiKey = null,
            string? environment = null)
        {
            if (gateway.HasValue)
                Gateway = gateway.Value;

            if (!string.IsNullOrWhiteSpace(apiKey))
                GatewayApiKey = apiKey.Trim();

            if (!string.IsNullOrWhiteSpace(environment))
            {
                if (environment != "production" && environment != "homologation")
                    throw new ArgumentException("Environment must be 'production' or 'homologation'", nameof(environment));

                GatewayEnvironment = environment;
            }

            UpdateTimestamp();
        }

        public void UpdateAutomationSettings(
            bool? autoIssueAfterPayment = null,
            bool? sendEmailAfterIssuance = null,
            string? emailTemplate = null)
        {
            if (autoIssueAfterPayment.HasValue)
                AutoIssueAfterPayment = autoIssueAfterPayment.Value;

            if (sendEmailAfterIssuance.HasValue)
                SendEmailAfterIssuance = sendEmailAfterIssuance.Value;

            if (!string.IsNullOrWhiteSpace(emailTemplate))
                EmailTemplate = emailTemplate.Trim();

            UpdateTimestamp();
        }

        public void SetSimplifiedTaxRegime(bool isSimplified, string? code = null)
        {
            IsSimplifiedTaxRegime = isSimplified;
            SimplifiedTaxRegimeCode = code?.Trim();
            UpdateTimestamp();
        }

        public int GetNextInvoiceNumber()
        {
            CurrentInvoiceNumber++;
            UpdateTimestamp();
            return CurrentInvoiceNumber;
        }

        public int GetNextRpsNumber()
        {
            CurrentRpsNumber++;
            UpdateTimestamp();
            return CurrentRpsNumber;
        }

        public void SetInvoiceSeries(string series)
        {
            if (string.IsNullOrWhiteSpace(series))
                throw new ArgumentException("Series cannot be empty", nameof(series));

            DefaultInvoiceSeries = series.Trim();
            UpdateTimestamp();
        }

        public void Activate()
        {
            if (string.IsNullOrWhiteSpace(Cnpj))
                throw new InvalidOperationException("CNPJ is required");

            if (string.IsNullOrWhiteSpace(CompanyName))
                throw new InvalidOperationException("Company name is required");

            if (string.IsNullOrWhiteSpace(GatewayApiKey) && Gateway != InvoiceGateway.Direct)
                throw new InvalidOperationException("Gateway API key is required");

            // Certificate not required for some gateways (they manage it)
            // if (!HasValidCertificate() && Gateway == InvoiceGateway.Direct)
            //     throw new InvalidOperationException("Valid digital certificate is required for direct integration");

            IsActive = true;
            UpdateTimestamp();
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdateTimestamp();
        }

        public bool CanIssueInvoices()
        {
            return IsActive &&
                   !string.IsNullOrWhiteSpace(Cnpj) &&
                   !string.IsNullOrWhiteSpace(GatewayApiKey);
        }
    }
}
