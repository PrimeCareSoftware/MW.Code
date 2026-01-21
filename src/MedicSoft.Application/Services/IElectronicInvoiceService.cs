using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Services
{
    public interface IElectronicInvoiceService
    {
        // Invoice Operations
        Task<ElectronicInvoiceDto> CreateInvoiceAsync(CreateElectronicInvoiceDto createDto, string tenantId);
        Task<ElectronicInvoiceDto> IssueInvoiceAsync(Guid invoiceId, string tenantId);
        Task<ElectronicInvoiceDto> CancelInvoiceAsync(Guid invoiceId, string reason, string tenantId);
        Task<ElectronicInvoiceDto> ReplaceInvoiceAsync(Guid invoiceId, string reason, string tenantId);
        
        // Query Operations
        Task<ElectronicInvoiceDto?> GetByIdAsync(Guid id, string tenantId);
        Task<IEnumerable<ElectronicInvoiceListDto>> GetByPeriodAsync(DateTime startDate, DateTime endDate, string tenantId);
        Task<IEnumerable<ElectronicInvoiceListDto>> GetByStatusAsync(string status, string tenantId);
        Task<IEnumerable<ElectronicInvoiceListDto>> GetByClientAsync(string cpfCnpj, string tenantId);
        Task<ElectronicInvoiceStatisticsDto> GetStatisticsAsync(DateTime startDate, DateTime endDate, string tenantId);
        
        // Document Operations
        Task<byte[]> GetPdfAsync(Guid invoiceId, string tenantId);
        Task<string> GetXmlAsync(Guid invoiceId, string tenantId);
        Task SendByEmailAsync(Guid invoiceId, string email, string tenantId);
        
        // Configuration Operations
        Task<InvoiceConfigurationDto?> GetConfigurationAsync(string tenantId);
        Task<InvoiceConfigurationDto> CreateConfigurationAsync(CreateInvoiceConfigurationDto createDto, string tenantId);
        Task<InvoiceConfigurationDto> UpdateConfigurationAsync(UpdateInvoiceConfigurationDto updateDto, string tenantId);
        Task UploadCertificateAsync(byte[] certificate, string password, string tenantId);
        Task ActivateConfigurationAsync(string tenantId);
        Task DeactivateConfigurationAsync(string tenantId);
    }
}
