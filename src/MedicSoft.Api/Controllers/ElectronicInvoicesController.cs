using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ElectronicInvoicesController : BaseController
    {
        private readonly IElectronicInvoiceService _invoiceService;

        public ElectronicInvoicesController(
            IElectronicInvoiceService invoiceService,
            ITenantContext tenantContext)
            : base(tenantContext)
        {
            _invoiceService = invoiceService;
        }

        /// <summary>
        /// Create a new electronic invoice
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ElectronicInvoiceDto>> CreateInvoice([FromBody] CreateElectronicInvoiceDto dto)
        {
            try
            {
                var invoice = await _invoiceService.CreateInvoiceAsync(dto, GetTenantId());
                return CreatedAtAction(nameof(GetInvoice), new { id = invoice.Id }, invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Issue an electronic invoice (send to SEFAZ)
        /// </summary>
        [HttpPost("{id}/issue")]
        public async Task<ActionResult<ElectronicInvoiceDto>> IssueInvoice(Guid id)
        {
            try
            {
                var invoice = await _invoiceService.IssueInvoiceAsync(id, GetTenantId());
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Cancel an electronic invoice
        /// </summary>
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult<ElectronicInvoiceDto>> CancelInvoice(Guid id, [FromBody] CancelElectronicInvoiceDto dto)
        {
            try
            {
                var invoice = await _invoiceService.CancelInvoiceAsync(id, dto.Reason, GetTenantId());
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Replace a cancelled invoice with a new one
        /// </summary>
        [HttpPost("{id}/replace")]
        public async Task<ActionResult<ElectronicInvoiceDto>> ReplaceInvoice(Guid id, [FromBody] CancelElectronicInvoiceDto dto)
        {
            try
            {
                var invoice = await _invoiceService.ReplaceInvoiceAsync(id, dto.Reason, GetTenantId());
                return Ok(invoice);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get invoice by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ElectronicInvoiceDto>> GetInvoice(Guid id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id, GetTenantId());
            
            if (invoice == null)
                return NotFound(new { error = $"Invoice {id} not found" });
            
            return Ok(invoice);
        }

        /// <summary>
        /// Get invoices by period
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ElectronicInvoiceListDto>>> GetInvoices(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? status,
            [FromQuery] string? clientCpfCnpj)
        {
            try
            {
                var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
                var end = endDate ?? DateTime.UtcNow;

                IEnumerable<ElectronicInvoiceListDto> invoices;

                if (!string.IsNullOrWhiteSpace(status))
                {
                    invoices = await _invoiceService.GetByStatusAsync(status, GetTenantId());
                }
                else if (!string.IsNullOrWhiteSpace(clientCpfCnpj))
                {
                    invoices = await _invoiceService.GetByClientAsync(clientCpfCnpj, GetTenantId());
                }
                else
                {
                    invoices = await _invoiceService.GetByPeriodAsync(start, end, GetTenantId());
                }

                return Ok(invoices);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Get invoice statistics for a period
        /// </summary>
        [HttpGet("statistics")]
        public async Task<ActionResult<ElectronicInvoiceStatisticsDto>> GetStatistics(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
                var end = endDate ?? DateTime.UtcNow;

                var statistics = await _invoiceService.GetStatisticsAsync(start, end, GetTenantId());
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Download invoice PDF
        /// </summary>
        [HttpGet("{id}/pdf")]
        public async Task<ActionResult> DownloadPdf(Guid id)
        {
            try
            {
                var pdf = await _invoiceService.GetPdfAsync(id, GetTenantId());
                return File(pdf, "application/pdf", $"nfse-{id}.pdf");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Download invoice XML
        /// </summary>
        [HttpGet("{id}/xml")]
        public async Task<ActionResult> DownloadXml(Guid id)
        {
            try
            {
                var xml = await _invoiceService.GetXmlAsync(id, GetTenantId());
                return File(System.Text.Encoding.UTF8.GetBytes(xml), "application/xml", $"nfse-{id}.xml");
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Send invoice by email
        /// </summary>
        [HttpPost("{id}/send-email")]
        public async Task<ActionResult> SendByEmail(Guid id, [FromBody] SendElectronicInvoiceEmailDto dto)
        {
            try
            {
                await _invoiceService.SendByEmailAsync(id, dto.Email, GetTenantId());
                return Ok(new { message = "Invoice sent successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // Configuration Endpoints

        /// <summary>
        /// Get invoice configuration for current tenant
        /// </summary>
        [HttpGet("configuration")]
        public async Task<ActionResult<InvoiceConfigurationDto>> GetConfiguration()
        {
            var config = await _invoiceService.GetConfigurationAsync(GetTenantId());
            
            if (config == null)
                return NotFound(new { error = "Configuration not found" });
            
            return Ok(config);
        }

        /// <summary>
        /// Create invoice configuration
        /// </summary>
        [HttpPost("configuration")]
        public async Task<ActionResult<InvoiceConfigurationDto>> CreateConfiguration([FromBody] CreateInvoiceConfigurationDto dto)
        {
            try
            {
                var config = await _invoiceService.CreateConfigurationAsync(dto, GetTenantId());
                return CreatedAtAction(nameof(GetConfiguration), null, config);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Update invoice configuration
        /// </summary>
        [HttpPut("configuration")]
        public async Task<ActionResult<InvoiceConfigurationDto>> UpdateConfiguration([FromBody] UpdateInvoiceConfigurationDto dto)
        {
            try
            {
                var config = await _invoiceService.UpdateConfigurationAsync(dto, GetTenantId());
                return Ok(config);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Upload digital certificate
        /// </summary>
        [HttpPost("configuration/certificate")]
        public async Task<ActionResult> UploadCertificate([FromBody] UploadCertificateDto dto)
        {
            try
            {
                await _invoiceService.UploadCertificateAsync(dto.Certificate, dto.Password, GetTenantId());
                return Ok(new { message = "Certificate uploaded successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Activate configuration
        /// </summary>
        [HttpPost("configuration/activate")]
        public async Task<ActionResult> ActivateConfiguration()
        {
            try
            {
                await _invoiceService.ActivateConfigurationAsync(GetTenantId());
                return Ok(new { message = "Configuration activated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate configuration
        /// </summary>
        [HttpPost("configuration/deactivate")]
        public async Task<ActionResult> DeactivateConfiguration()
        {
            try
            {
                await _invoiceService.DeactivateConfigurationAsync(GetTenantId());
                return Ok(new { message = "Configuration deactivated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}
