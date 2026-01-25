using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.Commands.Clinics;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Clinics;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for owners to manage their clinics
    /// </summary>
    [Route("api/owner-clinics")]
    [ApiController]
    [Authorize]
    public class OwnerClinicsController : BaseController
    {
        private readonly IMediator _mediator;

        public OwnerClinicsController(IMediator mediator, ITenantContext tenantContext)
            : base(tenantContext)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all clinics for the authenticated owner
        /// </summary>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.ClinicView)]
        public async Task<ActionResult<IEnumerable<ClinicDto>>> GetMyClinics()
        {
            var ownerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "owner_id");
            if (ownerIdClaim == null || !Guid.TryParse(ownerIdClaim.Value, out var ownerId))
                return Unauthorized("Owner ID not found in token");

            var query = new GetClinicsByOwnerQuery(ownerId, GetTenantId());
            var clinics = await _mediator.Send(query);
            return Ok(clinics);
        }

        /// <summary>
        /// Get a specific clinic by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.ClinicView)]
        public async Task<ActionResult<ClinicDto>> GetById(Guid id)
        {
            var query = new GetClinicByIdQuery(id, GetTenantId());
            var clinic = await _mediator.Send(query);
            
            if (clinic == null)
                return NotFound($"Clínica com ID {id} não encontrada");

            return Ok(clinic);
        }

        /// <summary>
        /// Create a new clinic (owner will be automatically linked as primary owner)
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.ClinicEdit)]
        public async Task<ActionResult<ClinicDto>> Create([FromBody] CreateClinicDto dto)
        {
            var ownerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "owner_id");
            if (ownerIdClaim == null || !Guid.TryParse(ownerIdClaim.Value, out var ownerId))
                return Unauthorized("Owner ID not found in token");

            try
            {
                var command = new CreateClinicCommand(dto, GetTenantId(), ownerId);
                var clinic = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = clinic.Id }, clinic);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing clinic
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermissionKey(PermissionKeys.ClinicEdit)]
        public async Task<ActionResult<ClinicDto>> Update(Guid id, [FromBody] UpdateClinicDto dto)
        {
            try
            {
                var command = new UpdateClinicCommand(id, dto, GetTenantId());
                var clinic = await _mediator.Send(command);
                return Ok(clinic);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Note: Deletion of clinics is not allowed per requirements.
        /// To deactivate a clinic, use the deactivate endpoint in ClinicAdminController
        /// </summary>
    }
}
