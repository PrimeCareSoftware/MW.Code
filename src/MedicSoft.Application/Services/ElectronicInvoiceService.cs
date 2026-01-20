using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    public class ElectronicInvoiceService : IElectronicInvoiceService
    {
        private readonly IElectronicInvoiceRepository _invoiceRepository;
        private readonly IInvoiceConfigurationRepository _configRepository;
        private readonly IMapper _mapper;

        public ElectronicInvoiceService(
            IElectronicInvoiceRepository invoiceRepository,
            IInvoiceConfigurationRepository configRepository,
            IMapper mapper)
        {
            _invoiceRepository = invoiceRepository;
            _configRepository = configRepository;
            _mapper = mapper;
        }

        public async Task<ElectronicInvoiceDto> CreateInvoiceAsync(CreateElectronicInvoiceDto createDto, string tenantId)
        {
            // Get configuration
            var config = await _configRepository.GetByTenantIdAsync(tenantId);
            if (config == null)
                throw new InvalidOperationException("Invoice configuration not found. Please configure electronic invoices first.");

            if (!config.IsActive)
                throw new InvalidOperationException("Invoice configuration is not active.");

            // Parse invoice type
            if (!Enum.TryParse<ElectronicInvoiceType>(createDto.Type, true, out var invoiceType))
                throw new ArgumentException($"Invalid invoice type: {createDto.Type}");

            // Create invoice entity
            var invoice = new ElectronicInvoice(
                invoiceType,
                config.DefaultInvoiceSeries,
                config.Cnpj,
                config.CompanyName,
                createDto.ClientCpfCnpj,
                createDto.ClientName,
                createDto.ServiceDescription,
                createDto.ServiceAmount,
                tenantId,
                config.MunicipalRegistration,
                createDto.ClientEmail,
                createDto.ServiceCode ?? config.ServiceCode,
                createDto.PaymentId,
                createDto.AppointmentId
            );

            // Set provider details
            invoice.SetProviderDetails(
                config.MunicipalRegistration,
                config.StateRegistration,
                config.Address,
                config.City,
                config.State,
                config.ZipCode
            );

            // Set client details
            if (!string.IsNullOrWhiteSpace(createDto.ClientAddress))
            {
                invoice.SetClientDetails(
                    createDto.ClientEmail,
                    createDto.ClientPhone,
                    createDto.ClientAddress,
                    createDto.ClientCity,
                    createDto.ClientState,
                    createDto.ClientZipCode
                );
            }

            // Calculate taxes
            invoice.CalculateTaxes(config.DefaultIssRate, issRetained: config.IssRetainedByDefault);

            // Save invoice
            var created = await _invoiceRepository.AddAsync(invoice);

            // Auto-issue if requested
            if (createDto.AutoIssue)
            {
                return await IssueInvoiceAsync(created.Id, tenantId);
            }

            return _mapper.Map<ElectronicInvoiceDto>(created);
        }

        public async Task<ElectronicInvoiceDto> IssueInvoiceAsync(Guid invoiceId, string tenantId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId, tenantId);
            if (invoice == null)
                throw new InvalidOperationException("Invoice not found");

            if (!invoice.CanBeIssued())
                throw new InvalidOperationException($"Invoice cannot be issued. Current status: {invoice.Status}");

            var config = await _configRepository.GetByTenantIdAsync(tenantId);
            if (config == null)
                throw new InvalidOperationException("Invoice configuration not found");

            // Get next invoice number
            var invoiceNumber = await _configRepository.GetNextInvoiceNumberAsync(tenantId);
            invoice.SetInvoiceNumber(invoiceNumber.ToString());

            // Mark as pending
            invoice.MarkAsPending();
            invoice.MarkAsPendingAuthorization();

            // TODO: Integrate with gateway to issue invoice
            // For now, we'll simulate authorization
            // In real implementation, this would call the gateway service
            
            try
            {
                // Simulate gateway call
                var accessKey = GenerateAccessKey(invoice);
                var authorizationCode = Guid.NewGuid().ToString("N").Substring(0, 15);
                var protocol = DateTime.UtcNow.Ticks.ToString();

                invoice.Authorize(
                    authorizationCode,
                    accessKey,
                    protocol,
                    DateTime.UtcNow,
                    Guid.NewGuid().ToString("N").Substring(0, 10),
                    null, // XML content would come from gateway
                    null  // PDF URL would come from gateway
                );

                await _invoiceRepository.UpdateAsync(invoice);

                // TODO: Send email if configured
                if (config.SendEmailAfterIssuance && !string.IsNullOrWhiteSpace(invoice.ClientEmail))
                {
                    // Implement email sending
                }

                return _mapper.Map<ElectronicInvoiceDto>(invoice);
            }
            catch (Exception ex)
            {
                invoice.SetError("GATEWAY_ERROR", ex.Message);
                await _invoiceRepository.UpdateAsync(invoice);
                throw;
            }
        }

        public async Task<ElectronicInvoiceDto> CancelInvoiceAsync(Guid invoiceId, string reason, string tenantId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId, tenantId);
            if (invoice == null)
                throw new InvalidOperationException("Invoice not found");

            if (!invoice.CanBeCancelled())
                throw new InvalidOperationException($"Invoice cannot be cancelled. Current status: {invoice.Status}");

            // TODO: Call gateway to cancel invoice
            // For now, we'll simulate cancellation
            
            invoice.Cancel(reason);
            await _invoiceRepository.UpdateAsync(invoice);

            return _mapper.Map<ElectronicInvoiceDto>(invoice);
        }

        public async Task<ElectronicInvoiceDto> ReplaceInvoiceAsync(Guid invoiceId, string reason, string tenantId)
        {
            // First cancel the original invoice
            await CancelInvoiceAsync(invoiceId, reason, tenantId);

            var originalInvoice = await _invoiceRepository.GetByIdAsync(invoiceId, tenantId);
            if (originalInvoice == null)
                throw new InvalidOperationException("Original invoice not found");

            // Create replacement invoice
            var createDto = new CreateElectronicInvoiceDto(
                originalInvoice.Type.ToString(),
                originalInvoice.ClientCpfCnpj,
                originalInvoice.ClientName,
                originalInvoice.ClientEmail,
                originalInvoice.ClientPhone,
                originalInvoice.ClientAddress,
                originalInvoice.ClientCity,
                originalInvoice.ClientState,
                originalInvoice.ClientZipCode,
                originalInvoice.ServiceDescription,
                originalInvoice.ServiceCode,
                originalInvoice.ServiceAmount,
                originalInvoice.PaymentId,
                originalInvoice.AppointmentId,
                AutoIssue: false
            );

            var replacementInvoice = await CreateInvoiceAsync(createDto, tenantId);
            
            // Link invoices
            var replacement = await _invoiceRepository.GetByIdAsync(replacementInvoice.Id, tenantId);
            if (replacement != null)
            {
                replacement.SetAsReplacementFor(invoiceId);
                originalInvoice.SetReplacement(replacement.Id);
                
                await _invoiceRepository.UpdateAsync(replacement);
                await _invoiceRepository.UpdateAsync(originalInvoice);
            }

            return replacementInvoice;
        }

        public async Task<ElectronicInvoiceDto?> GetByIdAsync(Guid id, string tenantId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(id, tenantId);
            return invoice != null ? _mapper.Map<ElectronicInvoiceDto>(invoice) : null;
        }

        public async Task<IEnumerable<ElectronicInvoiceListDto>> GetByPeriodAsync(DateTime startDate, DateTime endDate, string tenantId)
        {
            var invoices = await _invoiceRepository.GetByPeriodAsync(startDate, endDate, tenantId);
            return _mapper.Map<IEnumerable<ElectronicInvoiceListDto>>(invoices);
        }

        public async Task<IEnumerable<ElectronicInvoiceListDto>> GetByStatusAsync(string status, string tenantId)
        {
            if (!Enum.TryParse<ElectronicInvoiceStatus>(status, true, out var invoiceStatus))
                throw new ArgumentException($"Invalid status: {status}");

            var invoices = await _invoiceRepository.GetByStatusAsync(invoiceStatus, tenantId);
            return _mapper.Map<IEnumerable<ElectronicInvoiceListDto>>(invoices);
        }

        public async Task<IEnumerable<ElectronicInvoiceListDto>> GetByClientAsync(string cpfCnpj, string tenantId)
        {
            var invoices = await _invoiceRepository.GetByClientCpfCnpjAsync(cpfCnpj, tenantId);
            return _mapper.Map<IEnumerable<ElectronicInvoiceListDto>>(invoices);
        }

        public async Task<ElectronicInvoiceStatisticsDto> GetStatisticsAsync(DateTime startDate, DateTime endDate, string tenantId)
        {
            var invoices = await _invoiceRepository.GetByPeriodAsync(startDate, endDate, tenantId);
            var invoiceList = invoices.ToList();

            return new ElectronicInvoiceStatisticsDto
            {
                TotalInvoices = invoiceList.Count,
                AuthorizedInvoices = invoiceList.Count(i => i.Status == ElectronicInvoiceStatus.Authorized),
                CancelledInvoices = invoiceList.Count(i => i.Status == ElectronicInvoiceStatus.Cancelled),
                PendingInvoices = invoiceList.Count(i => i.Status == ElectronicInvoiceStatus.Pending || i.Status == ElectronicInvoiceStatus.PendingAuthorization),
                ErrorInvoices = invoiceList.Count(i => i.Status == ElectronicInvoiceStatus.Error || i.Status == ElectronicInvoiceStatus.Denied),
                TotalAmount = invoiceList.Where(i => i.Status == ElectronicInvoiceStatus.Authorized).Sum(i => i.ServiceAmount),
                TotalTaxes = invoiceList.Where(i => i.Status == ElectronicInvoiceStatus.Authorized).Sum(i => i.TotalTaxes),
                NetAmount = invoiceList.Where(i => i.Status == ElectronicInvoiceStatus.Authorized).Sum(i => i.NetAmount)
            };
        }

        public async Task<byte[]> GetPdfAsync(Guid invoiceId, string tenantId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId, tenantId);
            if (invoice == null)
                throw new InvalidOperationException("Invoice not found");

            if (string.IsNullOrWhiteSpace(invoice.PdfUrl))
                throw new InvalidOperationException("PDF not available for this invoice");

            // TODO: Download PDF from gateway or storage
            // For now, return empty array
            return Array.Empty<byte>();
        }

        public async Task<string> GetXmlAsync(Guid invoiceId, string tenantId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId, tenantId);
            if (invoice == null)
                throw new InvalidOperationException("Invoice not found");

            if (string.IsNullOrWhiteSpace(invoice.XmlContent))
                throw new InvalidOperationException("XML not available for this invoice");

            return invoice.XmlContent;
        }

        public async Task SendByEmailAsync(Guid invoiceId, string email, string tenantId)
        {
            var invoice = await _invoiceRepository.GetByIdAsync(invoiceId, tenantId);
            if (invoice == null)
                throw new InvalidOperationException("Invoice not found");

            if (invoice.Status != ElectronicInvoiceStatus.Authorized)
                throw new InvalidOperationException("Can only send authorized invoices");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email is required");

            // TODO: Implement email sending
            await Task.CompletedTask;
        }

        // Configuration Methods
        public async Task<InvoiceConfigurationDto?> GetConfigurationAsync(string tenantId)
        {
            var config = await _configRepository.GetByTenantIdAsync(tenantId);
            return config != null ? _mapper.Map<InvoiceConfigurationDto>(config) : null;
        }

        public async Task<InvoiceConfigurationDto> CreateConfigurationAsync(CreateInvoiceConfigurationDto createDto, string tenantId)
        {
            var exists = await _configRepository.ExistsForTenantAsync(tenantId);
            if (exists)
                throw new InvalidOperationException("Configuration already exists for this tenant");

            if (!Enum.TryParse<InvoiceGateway>(createDto.Gateway, true, out var gateway))
                throw new ArgumentException($"Invalid gateway: {createDto.Gateway}");

            var config = new InvoiceConfiguration(
                createDto.Cnpj,
                createDto.CompanyName,
                gateway,
                tenantId,
                createDto.MunicipalRegistration,
                createDto.CityCode,
                createDto.DefaultIssRate
            );

            // Update all details
            config.UpdateFiscalData(
                createDto.MunicipalRegistration,
                createDto.StateRegistration,
                createDto.TradeName,
                createDto.ServiceCode,
                createDto.DefaultIssRate,
                createDto.IssRetainedByDefault
            );

            config.UpdateAddress(
                createDto.Address,
                createDto.AddressNumber,
                createDto.AddressComplement,
                createDto.Neighborhood,
                createDto.City,
                createDto.State,
                createDto.ZipCode,
                createDto.CityCode
            );

            config.UpdateContact(createDto.Phone, createDto.Email);

            config.UpdateGatewaySettings(
                gateway,
                createDto.GatewayApiKey,
                createDto.GatewayEnvironment ?? "homologation"
            );

            config.UpdateAutomationSettings(
                createDto.AutoIssueAfterPayment,
                createDto.SendEmailAfterIssuance
            );

            config.SetSimplifiedTaxRegime(
                createDto.IsSimplifiedTaxRegime,
                createDto.SimplifiedTaxRegimeCode
            );

            var created = await _configRepository.AddAsync(config);
            return _mapper.Map<InvoiceConfigurationDto>(created);
        }

        public async Task<InvoiceConfigurationDto> UpdateConfigurationAsync(UpdateInvoiceConfigurationDto updateDto, string tenantId)
        {
            var config = await _configRepository.GetByTenantIdAsync(tenantId);
            if (config == null)
                throw new InvalidOperationException("Configuration not found");

            // Update fiscal data
            config.UpdateFiscalData(
                updateDto.MunicipalRegistration,
                updateDto.StateRegistration,
                updateDto.TradeName,
                updateDto.ServiceCode,
                updateDto.DefaultIssRate,
                updateDto.IssRetainedByDefault
            );

            // Update address
            config.UpdateAddress(
                updateDto.Address,
                updateDto.AddressNumber,
                updateDto.AddressComplement,
                updateDto.Neighborhood,
                updateDto.City,
                updateDto.State,
                updateDto.ZipCode,
                updateDto.CityCode
            );

            // Update contact
            config.UpdateContact(updateDto.Phone, updateDto.Email);

            // Update gateway
            if (!string.IsNullOrWhiteSpace(updateDto.Gateway))
            {
                if (!Enum.TryParse<InvoiceGateway>(updateDto.Gateway, true, out var gateway))
                    throw new ArgumentException($"Invalid gateway: {updateDto.Gateway}");

                config.UpdateGatewaySettings(gateway, updateDto.GatewayApiKey, updateDto.GatewayEnvironment);
            }

            // Update automation
            config.UpdateAutomationSettings(
                updateDto.AutoIssueAfterPayment,
                updateDto.SendEmailAfterIssuance
            );

            // Update tax regime
            if (updateDto.IsSimplifiedTaxRegime.HasValue)
            {
                config.SetSimplifiedTaxRegime(
                    updateDto.IsSimplifiedTaxRegime.Value,
                    updateDto.SimplifiedTaxRegimeCode
                );
            }

            await _configRepository.UpdateAsync(config);
            return _mapper.Map<InvoiceConfigurationDto>(config);
        }

        public async Task UploadCertificateAsync(byte[] certificate, string password, string tenantId)
        {
            var config = await _configRepository.GetByTenantIdAsync(tenantId);
            if (config == null)
                throw new InvalidOperationException("Configuration not found");

            // TODO: Extract certificate expiration date and thumbprint from the certificate
            var expirationDate = DateTime.UtcNow.AddYears(1); // Placeholder

            config.SetDigitalCertificate(certificate, password, expirationDate);
            await _configRepository.UpdateAsync(config);
        }

        public async Task ActivateConfigurationAsync(string tenantId)
        {
            var config = await _configRepository.GetByTenantIdAsync(tenantId);
            if (config == null)
                throw new InvalidOperationException("Configuration not found");

            config.Activate();
            await _configRepository.UpdateAsync(config);
        }

        public async Task DeactivateConfigurationAsync(string tenantId)
        {
            var config = await _configRepository.GetByTenantIdAsync(tenantId);
            if (config == null)
                throw new InvalidOperationException("Configuration not found");

            config.Deactivate();
            await _configRepository.UpdateAsync(config);
        }

        // Helper methods
        private string GenerateAccessKey(ElectronicInvoice invoice)
        {
            // Simplified access key generation (44 digits)
            // Real implementation should follow SEFAZ standards
            var random = new Random();
            return string.Concat(Enumerable.Range(0, 44).Select(_ => random.Next(0, 10)));
        }
    }
}
