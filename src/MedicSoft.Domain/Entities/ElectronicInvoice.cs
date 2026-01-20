using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    public enum ElectronicInvoiceType
    {
        NFSe = 1,    // Serviços (Municipal)
        NFe = 2,     // Produtos (Estadual)
        NFCe = 3,    // Consumidor (Estadual)
        RPS = 4      // Recibo Provisório de Serviços
    }

    public enum ElectronicInvoiceStatus
    {
        Draft = 1,              // Rascunho
        Pending = 2,            // Aguardando processamento
        PendingAuthorization = 3, // Enviada, aguardando SEFAZ
        Authorized = 4,         // Autorizada pela SEFAZ
        Cancelled = 5,          // Cancelada
        Denied = 6,             // Denegada pela SEFAZ
        Error = 7               // Erro no processamento
    }

    /// <summary>
    /// Represents an electronic invoice (NF-e/NFS-e) for Brazilian tax compliance.
    /// This is separate from the Invoice entity which handles internal billing.
    /// </summary>
    public class ElectronicInvoice : BaseEntity
    {
        // Basic Info
        public ElectronicInvoiceType Type { get; private set; }
        public string Number { get; private set; } = null!;
        public string Series { get; private set; } = null!;
        public DateTime IssueDate { get; private set; }
        public ElectronicInvoiceStatus Status { get; private set; }

        // Provider (Clinic) - Prestador
        public string ProviderCnpj { get; private set; } = null!;
        public string ProviderName { get; private set; } = null!;
        public string? ProviderMunicipalRegistration { get; private set; }
        public string? ProviderStateRegistration { get; private set; }
        public string? ProviderAddress { get; private set; }
        public string? ProviderCity { get; private set; }
        public string? ProviderState { get; private set; }
        public string? ProviderZipCode { get; private set; }

        // Client (Patient/Customer) - Tomador
        public string ClientCpfCnpj { get; private set; } = null!;
        public string ClientName { get; private set; } = null!;
        public string? ClientEmail { get; private set; }
        public string? ClientPhone { get; private set; }
        public string? ClientAddress { get; private set; }
        public string? ClientCity { get; private set; }
        public string? ClientState { get; private set; }
        public string? ClientZipCode { get; private set; }

        // Service/Product Description - Serviço
        public string ServiceDescription { get; private set; } = null!;
        public string? ServiceCode { get; private set; }  // CNAE
        public string? ItemCode { get; private set; }     // Item List Code
        public decimal ServiceAmount { get; private set; }

        // Taxes - Impostos
        public decimal IssRate { get; private set; }      // Alíquota ISS (%)
        public decimal IssAmount { get; private set; }    // ISS Value
        public bool IssRetained { get; private set; }     // ISS Retido
        public decimal IrAmount { get; private set; }     // IR Retido
        public decimal PisAmount { get; private set; }    // PIS
        public decimal CofinsAmount { get; private set; } // COFINS
        public decimal CsllAmount { get; private set; }   // CSLL
        public decimal InssAmount { get; private set; }   // INSS
        public decimal TotalTaxes { get; private set; }
        public decimal NetAmount { get; private set; }    // Valor Líquido

        // SEFAZ Response - Resposta SEFAZ
        public string? AuthorizationCode { get; private set; }
        public string? AccessKey { get; private set; }    // Chave de acesso (44 dígitos)
        public string? VerificationCode { get; private set; }  // Código de verificação
        public string? Protocol { get; private set; }
        public DateTime? AuthorizationDate { get; private set; }

        // Documents - Documentos
        public string? XmlContent { get; private set; }
        public string? PdfUrl { get; private set; }
        public string? RpsNumber { get; private set; }    // RPS Number if applicable

        // Cancellation - Cancelamento
        public DateTime? CancellationDate { get; private set; }
        public string? CancellationReason { get; private set; }
        public string? CancellationProtocol { get; private set; }

        // Replacement - Substituição
        public Guid? ReplacedInvoiceId { get; private set; }
        public Guid? ReplacementInvoiceId { get; private set; }

        // Error Handling - Tratamento de Erros
        public string? ErrorMessage { get; private set; }
        public string? ErrorCode { get; private set; }

        // References - Referências
        public Guid? PaymentId { get; private set; }
        public Guid? AppointmentId { get; private set; }

        // Navigation Properties
        public Payment? Payment { get; private set; }
        public Appointment? Appointment { get; private set; }

        private ElectronicInvoice()
        {
            // EF Constructor
        }

        public ElectronicInvoice(
            ElectronicInvoiceType type,
            string series,
            string providerCnpj,
            string providerName,
            string clientCpfCnpj,
            string clientName,
            string serviceDescription,
            decimal serviceAmount,
            string tenantId,
            string? providerMunicipalRegistration = null,
            string? clientEmail = null,
            string? serviceCode = null,
            Guid? paymentId = null,
            Guid? appointmentId = null) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(providerCnpj))
                throw new ArgumentException("Provider CNPJ is required", nameof(providerCnpj));

            if (string.IsNullOrWhiteSpace(providerName))
                throw new ArgumentException("Provider name is required", nameof(providerName));

            if (string.IsNullOrWhiteSpace(clientCpfCnpj))
                throw new ArgumentException("Client CPF/CNPJ is required", nameof(clientCpfCnpj));

            if (string.IsNullOrWhiteSpace(clientName))
                throw new ArgumentException("Client name is required", nameof(clientName));

            if (string.IsNullOrWhiteSpace(serviceDescription))
                throw new ArgumentException("Service description is required", nameof(serviceDescription));

            if (serviceAmount <= 0)
                throw new ArgumentException("Service amount must be greater than zero", nameof(serviceAmount));

            Type = type;
            Series = series?.Trim() ?? "1";
            Number = string.Empty; // Will be set when issued
            Status = ElectronicInvoiceStatus.Draft;
            IssueDate = DateTime.UtcNow;

            ProviderCnpj = providerCnpj.Trim();
            ProviderName = providerName.Trim();
            ProviderMunicipalRegistration = providerMunicipalRegistration?.Trim();

            ClientCpfCnpj = clientCpfCnpj.Trim();
            ClientName = clientName.Trim();
            ClientEmail = clientEmail?.Trim();

            ServiceDescription = serviceDescription.Trim();
            ServiceCode = serviceCode?.Trim();
            ServiceAmount = serviceAmount;

            PaymentId = paymentId;
            AppointmentId = appointmentId;

            // Initialize tax amounts (will be calculated before issuing)
            IssRate = 0;
            IssAmount = 0;
            IrAmount = 0;
            PisAmount = 0;
            CofinsAmount = 0;
            CsllAmount = 0;
            InssAmount = 0;
            TotalTaxes = 0;
            NetAmount = serviceAmount;
        }

        public void SetInvoiceNumber(string number)
        {
            if (string.IsNullOrWhiteSpace(number))
                throw new ArgumentException("Invoice number cannot be empty", nameof(number));

            if (Status != ElectronicInvoiceStatus.Draft && Status != ElectronicInvoiceStatus.Pending)
                throw new InvalidOperationException("Can only set number for draft or pending invoices");

            Number = number.Trim();
            UpdateTimestamp();
        }

        public void CalculateTaxes(
            decimal issRate,
            decimal pisRate = 0.0065m,    // 0.65%
            decimal cofinsRate = 0.03m,    // 3.00%
            decimal csllRate = 0.01m,      // 1.00%
            decimal inssRate = 0m,         // Variable
            decimal irRate = 0m,           // Variable
            bool issRetained = false)
        {
            if (Status != ElectronicInvoiceStatus.Draft)
                throw new InvalidOperationException("Can only calculate taxes for draft invoices");

            IssRate = issRate;
            IssAmount = Math.Round(ServiceAmount * issRate / 100, 2);
            IssRetained = issRetained;

            PisAmount = Math.Round(ServiceAmount * pisRate, 2);
            CofinsAmount = Math.Round(ServiceAmount * cofinsRate, 2);
            CsllAmount = Math.Round(ServiceAmount * csllRate, 2);
            InssAmount = Math.Round(ServiceAmount * inssRate, 2);
            IrAmount = Math.Round(ServiceAmount * irRate, 2);

            TotalTaxes = IssAmount + PisAmount + CofinsAmount + CsllAmount + InssAmount + IrAmount;
            NetAmount = ServiceAmount - TotalTaxes;

            UpdateTimestamp();
        }

        public void SetProviderDetails(
            string? municipalRegistration = null,
            string? stateRegistration = null,
            string? address = null,
            string? city = null,
            string? state = null,
            string? zipCode = null)
        {
            ProviderMunicipalRegistration = municipalRegistration?.Trim();
            ProviderStateRegistration = stateRegistration?.Trim();
            ProviderAddress = address?.Trim();
            ProviderCity = city?.Trim();
            ProviderState = state?.Trim();
            ProviderZipCode = zipCode?.Trim();
            UpdateTimestamp();
        }

        public void SetClientDetails(
            string? email = null,
            string? phone = null,
            string? address = null,
            string? city = null,
            string? state = null,
            string? zipCode = null)
        {
            ClientEmail = email?.Trim();
            ClientPhone = phone?.Trim();
            ClientAddress = address?.Trim();
            ClientCity = city?.Trim();
            ClientState = state?.Trim();
            ClientZipCode = zipCode?.Trim();
            UpdateTimestamp();
        }

        public void MarkAsPending()
        {
            if (Status != ElectronicInvoiceStatus.Draft)
                throw new InvalidOperationException("Only draft invoices can be marked as pending");

            Status = ElectronicInvoiceStatus.Pending;
            UpdateTimestamp();
        }

        public void MarkAsPendingAuthorization()
        {
            if (Status != ElectronicInvoiceStatus.Pending)
                throw new InvalidOperationException("Only pending invoices can be marked as pending authorization");

            Status = ElectronicInvoiceStatus.PendingAuthorization;
            UpdateTimestamp();
        }

        public void Authorize(
            string authorizationCode,
            string accessKey,
            string protocol,
            DateTime authorizationDate,
            string? verificationCode = null,
            string? xmlContent = null,
            string? pdfUrl = null)
        {
            if (Status != ElectronicInvoiceStatus.PendingAuthorization)
                throw new InvalidOperationException("Only pending authorization invoices can be authorized");

            if (string.IsNullOrWhiteSpace(authorizationCode))
                throw new ArgumentException("Authorization code is required", nameof(authorizationCode));

            if (string.IsNullOrWhiteSpace(accessKey))
                throw new ArgumentException("Access key is required", nameof(accessKey));

            Status = ElectronicInvoiceStatus.Authorized;
            AuthorizationCode = authorizationCode.Trim();
            AccessKey = accessKey.Trim();
            Protocol = protocol?.Trim();
            AuthorizationDate = authorizationDate;
            VerificationCode = verificationCode?.Trim();
            XmlContent = xmlContent;
            PdfUrl = pdfUrl?.Trim();
            ErrorMessage = null;
            ErrorCode = null;

            UpdateTimestamp();
        }

        public void Deny(string errorCode, string errorMessage)
        {
            if (Status == ElectronicInvoiceStatus.Authorized)
                throw new InvalidOperationException("Cannot deny an authorized invoice");

            if (Status == ElectronicInvoiceStatus.Cancelled)
                throw new InvalidOperationException("Cannot deny a cancelled invoice");

            Status = ElectronicInvoiceStatus.Denied;
            ErrorCode = errorCode?.Trim();
            ErrorMessage = errorMessage?.Trim();
            UpdateTimestamp();
        }

        public void SetError(string errorCode, string errorMessage)
        {
            if (Status == ElectronicInvoiceStatus.Authorized)
                throw new InvalidOperationException("Cannot set error for authorized invoice");

            Status = ElectronicInvoiceStatus.Error;
            ErrorCode = errorCode?.Trim();
            ErrorMessage = errorMessage?.Trim();
            UpdateTimestamp();
        }

        public void Cancel(string reason, string? protocol = null)
        {
            if (Status != ElectronicInvoiceStatus.Authorized)
                throw new InvalidOperationException("Only authorized invoices can be cancelled");

            if (string.IsNullOrWhiteSpace(reason))
                throw new ArgumentException("Cancellation reason is required", nameof(reason));

            Status = ElectronicInvoiceStatus.Cancelled;
            CancellationReason = reason.Trim();
            CancellationDate = DateTime.UtcNow;
            CancellationProtocol = protocol?.Trim();
            UpdateTimestamp();
        }

        public void SetReplacement(Guid replacementInvoiceId)
        {
            if (Status != ElectronicInvoiceStatus.Cancelled)
                throw new InvalidOperationException("Only cancelled invoices can be replaced");

            ReplacementInvoiceId = replacementInvoiceId;
            UpdateTimestamp();
        }

        public void SetAsReplacementFor(Guid replacedInvoiceId)
        {
            if (Status != ElectronicInvoiceStatus.Draft)
                throw new InvalidOperationException("Only draft invoices can be set as replacement");

            ReplacedInvoiceId = replacedInvoiceId;
            UpdateTimestamp();
        }

        public void UpdateDocuments(string? xmlContent = null, string? pdfUrl = null)
        {
            if (!string.IsNullOrWhiteSpace(xmlContent))
                XmlContent = xmlContent;

            if (!string.IsNullOrWhiteSpace(pdfUrl))
                PdfUrl = pdfUrl.Trim();

            UpdateTimestamp();
        }

        public bool CanBeCancelled()
        {
            return Status == ElectronicInvoiceStatus.Authorized;
        }

        public bool CanBeIssued()
        {
            return Status == ElectronicInvoiceStatus.Draft ||
                   Status == ElectronicInvoiceStatus.Error ||
                   Status == ElectronicInvoiceStatus.Denied;
        }
    }
}
