using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ConsentController : BaseController
    {
        private readonly IConsentManagementService _consentService;
        private readonly ILogger<ConsentController> _logger;

        public ConsentController(
            ITenantContext tenantContext,
            IConsentManagementService consentService,
            ILogger<ConsentController> logger) : base(tenantContext)
        {
            _consentService = consentService ?? throw new ArgumentNullException(nameof(consentService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Registra um novo consentimento do paciente
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "SystemAdmin,ClinicOwner,Doctor,Nurse,Patient")]
        public async Task<IActionResult> RecordConsent([FromBody] RecordConsentRequest request)
        {
            try
            {
                var tenantId = GetTenantId();
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                var consentId = await _consentService.RecordConsentAsync(
                    request.PatientId,
                    request.PatientName,
                    request.Type,
                    request.Purpose,
                    request.Description,
                    request.ExpirationDate,
                    ipAddress,
                    request.ConsentText,
                    request.ConsentVersion,
                    request.ConsentMethod,
                    userAgent,
                    tenantId
                );

                return Ok(new { ConsentId = consentId, Message = "Consentimento registrado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording consent");
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Revoga um consentimento existente
        /// </summary>
        [HttpPost("{consentId}/revoke")]
        [Authorize(Roles = "SystemAdmin,ClinicOwner,Doctor,Patient")]
        public async Task<IActionResult> RevokeConsent(Guid consentId, [FromBody] RevokeConsentRequest request)
        {
            try
            {
                var tenantId = GetTenantId();
                await _consentService.RevokeConsentAsync(consentId, request.Reason, tenantId);

                return Ok(new { Message = "Consentimento revogado com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error revoking consent {ConsentId}", consentId);
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém todos os consentimentos de um paciente
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "SystemAdmin,ClinicOwner,Doctor,Nurse,Patient")]
        public async Task<IActionResult> GetPatientConsents(Guid patientId)
        {
            try
            {
                var tenantId = GetTenantId();
                var consents = await _consentService.GetPatientConsentsAsync(patientId, tenantId);

                return Ok(consents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting consents for patient {PatientId}", patientId);
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém consentimentos ativos de um paciente
        /// </summary>
        [HttpGet("patient/{patientId}/active")]
        [Authorize(Roles = "SystemAdmin,ClinicOwner,Doctor,Nurse")]
        public async Task<IActionResult> GetActivePatientConsents(Guid patientId)
        {
            try
            {
                var tenantId = GetTenantId();
                var consents = await _consentService.GetActivePatientConsentsAsync(patientId, tenantId);

                return Ok(consents);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting active consents for patient {PatientId}", patientId);
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Verifica se o paciente tem consentimento ativo para determinada finalidade
        /// </summary>
        [HttpGet("patient/{patientId}/has-consent")]
        [Authorize(Roles = "SystemAdmin,ClinicOwner,Doctor,Nurse")]
        public async Task<IActionResult> HasActiveConsent(Guid patientId, [FromQuery] ConsentPurpose purpose)
        {
            try
            {
                var tenantId = GetTenantId();
                var hasConsent = await _consentService.HasActiveConsentAsync(patientId, purpose, tenantId);

                return Ok(new { HasConsent = hasConsent, Purpose = purpose.ToString() });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking consent for patient {PatientId}", patientId);
                return BadRequest(new { Error = ex.Message });
            }
        }
    }

    public class RecordConsentRequest
    {
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public ConsentType Type { get; set; }
        public ConsentPurpose Purpose { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime? ExpirationDate { get; set; }
        public string ConsentText { get; set; } = string.Empty;
        public string ConsentVersion { get; set; } = "1.0";
        public string ConsentMethod { get; set; } = "WEB";
    }

    public class RevokeConsentRequest
    {
        public string Reason { get; set; } = string.Empty;
    }
}
