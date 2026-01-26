using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class DataPortabilityController : BaseController
    {
        private readonly IDataPortabilityService _portabilityService;
        private readonly IAuditService _auditService;
        private readonly ILogger<DataPortabilityController> _logger;

        public DataPortabilityController(
            ITenantContext tenantContext,
            IDataPortabilityService portabilityService,
            IAuditService auditService,
            ILogger<DataPortabilityController> logger) : base(tenantContext)
        {
            _portabilityService = portabilityService ?? throw new ArgumentNullException(nameof(portabilityService));
            _auditService = auditService ?? throw new ArgumentNullException(nameof(auditService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Exporta todos os dados de um paciente em formato JSON (LGPD Art. 18, V)
        /// </summary>
        [HttpGet("patient/{patientId}/export/json")]
        [Authorize(Roles = "SystemAdmin,ClinicOwner,Patient")]
        public async Task<IActionResult> ExportAsJson(Guid patientId)
        {
            try
            {
                var tenantId = GetTenantId();
                var userId = GetUserId();
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                // Log the data portability request
                await _portabilityService.LogPortabilityRequestAsync(
                    patientId,
                    "JSON",
                    ipAddress,
                    userAgent,
                    tenantId
                );

                var json = await _portabilityService.ExportPatientDataAsJsonAsync(patientId, tenantId);

                return File(
                    System.Text.Encoding.UTF8.GetBytes(json),
                    "application/json",
                    $"patient_{patientId}_data.json"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting patient {PatientId} data as JSON", patientId);
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Exporta todos os dados de um paciente em formato XML (LGPD Art. 18, V)
        /// </summary>
        [HttpGet("patient/{patientId}/export/xml")]
        [Authorize(Roles = "SystemAdmin,ClinicOwner,Patient")]
        public async Task<IActionResult> ExportAsXml(Guid patientId)
        {
            try
            {
                var tenantId = GetTenantId();
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                // Log the data portability request
                await _portabilityService.LogPortabilityRequestAsync(
                    patientId,
                    "XML",
                    ipAddress,
                    userAgent,
                    tenantId
                );

                var xml = await _portabilityService.ExportPatientDataAsXmlAsync(patientId, tenantId);

                return File(
                    System.Text.Encoding.UTF8.GetBytes(xml),
                    "application/xml",
                    $"patient_{patientId}_data.xml"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting patient {PatientId} data as XML", patientId);
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Exporta todos os dados de um paciente em formato PDF (LGPD Art. 18, V)
        /// </summary>
        [HttpGet("patient/{patientId}/export/pdf")]
        [Authorize(Roles = "SystemAdmin,ClinicOwner,Patient")]
        public async Task<IActionResult> ExportAsPdf(Guid patientId)
        {
            try
            {
                var tenantId = GetTenantId();
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                // Log the data portability request
                await _portabilityService.LogPortabilityRequestAsync(
                    patientId,
                    "PDF",
                    ipAddress,
                    userAgent,
                    tenantId
                );

                var pdf = await _portabilityService.ExportPatientDataAsPdfAsync(patientId, tenantId);

                return File(
                    pdf,
                    "application/pdf",
                    $"patient_{patientId}_data.pdf"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting patient {PatientId} data as PDF", patientId);
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Cria um pacote completo de dados do paciente (ZIP com múltiplos formatos)
        /// </summary>
        [HttpGet("patient/{patientId}/export/package")]
        [Authorize(Roles = "SystemAdmin,ClinicOwner,Patient")]
        public async Task<IActionResult> ExportPackage(Guid patientId)
        {
            try
            {
                var tenantId = GetTenantId();
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                // Log the data portability request
                await _portabilityService.LogPortabilityRequestAsync(
                    patientId,
                    "PACKAGE",
                    ipAddress,
                    userAgent,
                    tenantId
                );

                var package = await _portabilityService.CreatePatientDataPackageAsync(patientId, tenantId);

                return File(
                    package,
                    "application/zip",
                    $"patient_{patientId}_complete_data.zip"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating data package for patient {PatientId}", patientId);
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Endpoint de informações sobre portabilidade de dados (LGPD)
        /// </summary>
        [HttpGet("info")]
        [AllowAnonymous]
        public IActionResult GetPortabilityInfo()
        {
            var info = new
            {
                Title = "Direito à Portabilidade de Dados - LGPD",
                Article = "Art. 18, V da Lei 13.709/2018",
                Description = "Você tem o direito de solicitar a portabilidade dos seus dados pessoais a outro fornecedor de serviço ou produto.",
                AvailableFormats = new[] { "JSON", "XML", "PDF", "Package (ZIP)" },
                ProcessingTime = "Imediato",
                Instructions = new[]
                {
                    "1. Autentique-se no sistema",
                    "2. Acesse seu perfil de paciente",
                    "3. Escolha o formato desejado (JSON, XML, PDF ou pacote completo)",
                    "4. Faça o download dos seus dados",
                    "5. Os dados incluem: informações pessoais, histórico médico, consultas, exames, prescrições, etc."
                },
                LegalBasis = "LGPD Art. 18, V - portabilidade dos dados a outro fornecedor de serviço ou produto",
                Contact = "Para dúvidas, entre em contato com o Encarregado de Dados (DPO)"
            };

            return Ok(info);
        }
    }
}
