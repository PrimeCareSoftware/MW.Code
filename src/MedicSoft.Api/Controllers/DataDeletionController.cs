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
    public class DataDeletionController : BaseController
    {
        private readonly IDataDeletionService _deletionService;
        private readonly ILogger<DataDeletionController> _logger;

        public DataDeletionController(
            ITenantContext tenantContext,
            IDataDeletionService deletionService,
            ILogger<DataDeletionController> logger) : base(tenantContext)
        {
            _deletionService = deletionService ?? throw new ArgumentNullException(nameof(deletionService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Cria uma requisição de exclusão/anonimização de dados (direito ao esquecimento - LGPD Art. 18)
        /// </summary>
        [HttpPost("request")]
        [Authorize(Roles = "SystemAdmin,ClinicOwner,Patient")]
        public async Task<IActionResult> RequestDataDeletion([FromBody] DataDeletionRequestDto request)
        {
            try
            {
                var tenantId = GetTenantId();
                var ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
                var userAgent = HttpContext.Request.Headers["User-Agent"].ToString();

                var requestId = await _deletionService.RequestDataDeletionAsync(
                    request.PatientId,
                    request.PatientName,
                    request.PatientEmail,
                    request.Reason,
                    request.RequestType,
                    ipAddress,
                    userAgent,
                    request.RequiresLegalApproval,
                    tenantId
                );

                return Ok(new 
                { 
                    RequestId = requestId, 
                    Message = "Requisição de exclusão criada com sucesso. Será processada em até 48 horas.",
                    LgpdInfo = "Conforme Art. 18, VI da LGPD, você tem direito à eliminação dos dados tratados com consentimento."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating data deletion request");
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Processa uma requisição de exclusão pendente (Admin only)
        /// </summary>
        [HttpPost("{requestId}/process")]
        [Authorize(Roles = "SystemAdmin,ClinicOwner")]
        public async Task<IActionResult> ProcessRequest(Guid requestId, [FromBody] ProcessRequestDto dto)
        {
            try
            {
                var tenantId = GetTenantId();
                var userId = GetUserId().ToString();
                var userName = GetUserName();

                await _deletionService.ProcessDataDeletionRequestAsync(
                    requestId,
                    userId,
                    userName,
                    dto.Notes,
                    tenantId
                );

                return Ok(new { Message = "Requisição em processamento" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing data deletion request {RequestId}", requestId);
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Completa uma requisição de exclusão (executa a anonimização/exclusão)
        /// </summary>
        [HttpPost("{requestId}/complete")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> CompleteRequest(Guid requestId)
        {
            try
            {
                var tenantId = GetTenantId();
                await _deletionService.CompleteDataDeletionRequestAsync(requestId, tenantId);

                return Ok(new { Message = "Requisição concluída com sucesso" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing data deletion request {RequestId}", requestId);
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Rejeita uma requisição de exclusão
        /// </summary>
        [HttpPost("{requestId}/reject")]
        [Authorize(Roles = "SystemAdmin,ClinicOwner")]
        public async Task<IActionResult> RejectRequest(Guid requestId, [FromBody] RejectRequestDto dto)
        {
            try
            {
                var tenantId = GetTenantId();
                await _deletionService.RejectDataDeletionRequestAsync(requestId, dto.Reason, tenantId);

                return Ok(new { Message = "Requisição rejeitada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rejecting data deletion request {RequestId}", requestId);
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Aprova legalmente uma requisição
        /// </summary>
        [HttpPost("{requestId}/legal-approval")]
        [Authorize(Roles = "SystemAdmin")]
        public async Task<IActionResult> ApproveLegal(Guid requestId, [FromBody] LegalApprovalDto dto)
        {
            try
            {
                var tenantId = GetTenantId();
                await _deletionService.ApproveLegalAsync(requestId, dto.Approver, tenantId);

                return Ok(new { Message = "Aprovação legal registrada" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error approving legal for request {RequestId}", requestId);
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém requisições pendentes (Admin only)
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Roles = "SystemAdmin,ClinicOwner")]
        public async Task<IActionResult> GetPendingRequests()
        {
            try
            {
                var tenantId = GetTenantId();
                var requests = await _deletionService.GetPendingRequestsAsync(tenantId);

                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting pending deletion requests");
                return BadRequest(new { Error = ex.Message });
            }
        }

        /// <summary>
        /// Obtém requisições de um paciente
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "SystemAdmin,ClinicOwner,Patient")]
        public async Task<IActionResult> GetPatientRequests(Guid patientId)
        {
            try
            {
                var tenantId = GetTenantId();
                var requests = await _deletionService.GetPatientRequestsAsync(patientId, tenantId);

                return Ok(requests);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting deletion requests for patient {PatientId}", patientId);
                return BadRequest(new { Error = ex.Message });
            }
        }
    }

    public class DataDeletionRequestDto
    {
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;
        public string Reason { get; set; } = string.Empty;
        public DeletionRequestType RequestType { get; set; }
        public bool RequiresLegalApproval { get; set; } = false;
    }

    public class ProcessRequestDto
    {
        public string Notes { get; set; } = string.Empty;
    }

    public class RejectRequestDto
    {
        public string Reason { get; set; } = string.Empty;
    }

    public class LegalApprovalDto
    {
        public string Approver { get; set; } = string.Empty;
    }
}
