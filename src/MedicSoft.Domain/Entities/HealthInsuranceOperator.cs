using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Representa uma operadora de plano de saúde (ANS)
    /// </summary>
    public class HealthInsuranceOperator : BaseEntity
    {
        public string TradeName { get; private set; } // Nome comercial (ex: "Unimed São Paulo")
        public string CompanyName { get; private set; } // Razão social
        public string RegisterNumber { get; private set; } // Registro ANS
        public string Document { get; private set; } // CNPJ
        public string? Phone { get; private set; }
        public string? Email { get; private set; }
        public string? ContactPerson { get; private set; }
        public bool IsActive { get; private set; } = true;
        
        // Configurações de integração
        public OperatorIntegrationType IntegrationType { get; private set; }
        public string? WebsiteUrl { get; private set; }
        public string? ApiEndpoint { get; private set; }
        public string? ApiKey { get; private set; }
        public bool RequiresPriorAuthorization { get; private set; }
        
        // Configurações TISS
        public string? TissVersion { get; private set; } // Ex: "4.02.00"
        public bool SupportsTissXml { get; private set; }
        public string? BatchSubmissionEmail { get; private set; }
        
        private HealthInsuranceOperator() 
        { 
            // EF Constructor
            TradeName = null!;
            CompanyName = null!;
            RegisterNumber = null!;
            Document = null!;
        }

        public HealthInsuranceOperator(
            string tradeName, 
            string companyName, 
            string registerNumber, 
            string document,
            string tenantId,
            string? phone = null,
            string? email = null,
            string? contactPerson = null) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(tradeName))
                throw new ArgumentException("Trade name cannot be empty", nameof(tradeName));
            
            if (string.IsNullOrWhiteSpace(companyName))
                throw new ArgumentException("Company name cannot be empty", nameof(companyName));
            
            if (string.IsNullOrWhiteSpace(registerNumber))
                throw new ArgumentException("Register number (ANS) cannot be empty", nameof(registerNumber));
            
            if (string.IsNullOrWhiteSpace(document))
                throw new ArgumentException("Document (CNPJ) cannot be empty", nameof(document));

            TradeName = tradeName.Trim();
            CompanyName = companyName.Trim();
            RegisterNumber = registerNumber.Trim();
            Document = document.Trim();
            Phone = phone?.Trim();
            Email = email?.Trim();
            ContactPerson = contactPerson?.Trim();
            IntegrationType = OperatorIntegrationType.Manual; // Default
            SupportsTissXml = false; // Default
            RequiresPriorAuthorization = false; // Default
        }

        public void UpdateBasicInfo(string tradeName, string companyName, string? phone, string? email, string? contactPerson)
        {
            if (string.IsNullOrWhiteSpace(tradeName))
                throw new ArgumentException("Trade name cannot be empty", nameof(tradeName));
            
            if (string.IsNullOrWhiteSpace(companyName))
                throw new ArgumentException("Company name cannot be empty", nameof(companyName));

            TradeName = tradeName.Trim();
            CompanyName = companyName.Trim();
            Phone = phone?.Trim();
            Email = email?.Trim();
            ContactPerson = contactPerson?.Trim();
            UpdateTimestamp();
        }

        public void ConfigureIntegration(
            OperatorIntegrationType integrationType,
            string? websiteUrl = null,
            string? apiEndpoint = null,
            string? apiKey = null,
            bool requiresPriorAuthorization = false)
        {
            IntegrationType = integrationType;
            WebsiteUrl = websiteUrl?.Trim();
            ApiEndpoint = apiEndpoint?.Trim();
            ApiKey = apiKey?.Trim();
            RequiresPriorAuthorization = requiresPriorAuthorization;
            UpdateTimestamp();
        }

        public void ConfigureTiss(string tissVersion, bool supportsTissXml, string? batchSubmissionEmail = null)
        {
            if (string.IsNullOrWhiteSpace(tissVersion))
                throw new ArgumentException("TISS version cannot be empty", nameof(tissVersion));

            TissVersion = tissVersion.Trim();
            SupportsTissXml = supportsTissXml;
            BatchSubmissionEmail = batchSubmissionEmail?.Trim();
            UpdateTimestamp();
        }

        public void Activate()
        {
            IsActive = true;
            UpdateTimestamp();
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdateTimestamp();
        }
    }

    /// <summary>
    /// Tipo de integração com a operadora
    /// </summary>
    public enum OperatorIntegrationType
    {
        Manual = 1,      // Processo manual (papel/PDF)
        WebPortal = 2,   // Portal web da operadora
        TissXml = 3,     // XML TISS (arquivo)
        RestApi = 4      // API REST da operadora
    }
}
