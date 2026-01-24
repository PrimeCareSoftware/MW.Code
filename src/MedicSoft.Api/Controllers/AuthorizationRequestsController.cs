using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing authorization requests
    /// </summary>
    [ApiController]
    [Route("api/authorization-requests")]
    [Authorize]
    public class AuthorizationRequestsController : BaseController
    {
        private readonly IAuthorizationRequestService _authorizationRequestService;

        public AuthorizationRequestsController(
            IAuthorizationRequestService authorizationRequestService,
            ITenantContext tenantContext) : base(tenantContext)
        {
            _authorizationRequestService = authorizationRequestService;
        }

        /// <summary>
        /// Get all authorization requests
        /// </summary>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.AuthorizationsView)]
        public async Task<ActionResult<IEnumerable<AuthorizationRequestDto>>> GetAll()
        {
            var requests = await _authorizationRequestService.GetAllAsync(GetTenantId());
            return Ok(requests);
        }

        /// <summary>
        /// Get authorization request by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.AuthorizationsView)]
        public async Task<ActionResult<AuthorizationRequestDto>> GetById(Guid id)
        {
            var request = await _authorizationRequestService.GetByIdAsync(id, GetTenantId());
            if (request == null)
                return NotFound(new { message = $"Solicitação de autorização não encontrada." });

            return Ok(request);
        }

        /// <summary>
        /// Get authorization requests by patient ID
        /// </summary>
        [HttpGet("by-patient/{patientId}")]
        [RequirePermissionKey(PermissionKeys.AuthorizationsView)]
        public async Task<ActionResult<IEnumerable<AuthorizationRequestDto>>> GetByPatientId(Guid patientId)
        {
            var requests = await _authorizationRequestService.GetByPatientIdAsync(patientId, GetTenantId());
            return Ok(requests);
        }

        /// <summary>
        /// Get authorization requests by status
        /// </summary>
        [HttpGet("by-status/{status}")]
        [RequirePermissionKey(PermissionKeys.AuthorizationsView)]
        public async Task<ActionResult<IEnumerable<AuthorizationRequestDto>>> GetByStatus(string status)
        {
            var requests = await _authorizationRequestService.GetByStatusAsync(status, GetTenantId());
            return Ok(requests);
        }

        /// <summary>
        /// Get pending authorization requests
        /// </summary>
        [HttpGet("pending")]
        [RequirePermissionKey(PermissionKeys.AuthorizationsView)]
        public async Task<ActionResult<IEnumerable<AuthorizationRequestDto>>> GetPending()
        {
            var requests = await _authorizationRequestService.GetPendingAsync(GetTenantId());
            return Ok(requests);
        }

        /// <summary>
        /// Get authorization request by authorization number
        /// </summary>
        [HttpGet("by-number/{authorizationNumber}")]
        [RequirePermissionKey(PermissionKeys.AuthorizationsView)]
        public async Task<ActionResult<AuthorizationRequestDto>> GetByAuthorizationNumber(string authorizationNumber)
        {
            var request = await _authorizationRequestService.GetByAuthorizationNumberAsync(authorizationNumber, GetTenantId());
            if (request == null)
                return NotFound(new { message = $"Autorização {authorizationNumber} não encontrada." });

            return Ok(request);
        }

        /// <summary>
        /// Create a new authorization request
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.AuthorizationsCreate)]
        public async Task<ActionResult<AuthorizationRequestDto>> Create([FromBody] CreateAuthorizationRequestDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var request = await _authorizationRequestService.CreateAsync(dto, GetTenantId());
                return CreatedAtAction(nameof(GetById), new { id = request.Id }, request);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Approve an authorization request
        /// </summary>
        [HttpPost("{id}/approve")]
        [RequirePermissionKey(PermissionKeys.AuthorizationsEdit)]
        public async Task<ActionResult<AuthorizationRequestDto>> Approve(Guid id, [FromBody] ApproveAuthorizationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var request = await _authorizationRequestService.ApproveAsync(id, dto, GetTenantId());
                return Ok(request);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deny an authorization request
        /// </summary>
        [HttpPost("{id}/deny")]
        [RequirePermissionKey(PermissionKeys.AuthorizationsEdit)]
        public async Task<ActionResult<AuthorizationRequestDto>> Deny(Guid id, [FromBody] DenyAuthorizationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var request = await _authorizationRequestService.DenyAsync(id, dto, GetTenantId());
                return Ok(request);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Cancel an authorization request
        /// </summary>
        [HttpPost("{id}/cancel")]
        [RequirePermissionKey(PermissionKeys.AuthorizationsEdit)]
        public async Task<ActionResult<AuthorizationRequestDto>> Cancel(Guid id)
        {
            try
            {
                var request = await _authorizationRequestService.CancelAsync(id, GetTenantId());
                return Ok(request);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Mark expired authorizations
        /// </summary>
        [HttpPost("mark-expired")]
        [RequirePermissionKey(PermissionKeys.AuthorizationsEdit)]
        public async Task<ActionResult<int>> MarkExpired()
        {
            var count = await _authorizationRequestService.MarkExpiredAuthorizationsAsync(GetTenantId());
            return Ok(new { message = $"{count} autorizações expiradas marcadas.", count });
        }
    }
}
