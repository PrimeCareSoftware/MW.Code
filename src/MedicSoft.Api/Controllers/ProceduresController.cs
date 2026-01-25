using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.Commands.Procedures;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Procedures;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProceduresController : BaseController
    {
        private readonly IMediator _mediator;
        private readonly IOwnerRepository _ownerRepository;

        public ProceduresController(IMediator mediator, ITenantContext tenantContext, IOwnerRepository ownerRepository)
            : base(tenantContext)
        {
            _mediator = mediator;
            _ownerRepository = ownerRepository;
        }

        /// <summary>
        /// Get all procedures for the clinic (requires procedures.view permission)
        /// For clinic owners with procedures.manage permission, returns procedures from all owned clinics
        /// </summary>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.ProceduresView)]
        public async Task<ActionResult<IEnumerable<ProcedureDto>>> GetAll([FromQuery] bool activeOnly = true)
        {
            var userId = GetUserId();
            var tenantId = GetTenantId();
            Guid? ownerId = null;

            // Check if user has owner-level manage permission and is authenticated as an owner
            var isSystemOwnerClaim = User?.FindFirst("is_system_owner")?.Value;
            var roleClaim = User?.FindFirst("role")?.Value;
            
            if (roleClaim == RoleNames.ClinicOwner && userId != Guid.Empty)
            {
                // Verify the user is indeed an owner
                var owner = await _ownerRepository.GetByIdAsync(userId, tenantId);
                if (owner != null)
                {
                    ownerId = owner.Id;
                }
            }

            var query = new GetProceduresByClinicQuery(tenantId, activeOnly, ownerId);
            var procedures = await _mediator.Send(query);
            return Ok(procedures);
        }

        /// <summary>
        /// Get procedure by ID (requires procedures.view permission)
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.ProceduresView)]
        public async Task<ActionResult<ProcedureDto>> GetById(Guid id)
        {
            var query = new GetProcedureByIdQuery(id, GetTenantId());
            var procedure = await _mediator.Send(query);
            
            if (procedure == null)
                return NotFound($"Procedure with ID {id} not found");

            return Ok(procedure);
        }

        /// <summary>
        /// Create a new procedure (requires procedures.create permission)
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.ProceduresCreate)]
        public async Task<ActionResult<ProcedureDto>> Create([FromBody] CreateProcedureDto dto)
        {
            var command = new CreateProcedureCommand(dto, GetTenantId());
            var procedure = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = procedure.Id }, procedure);
        }

        /// <summary>
        /// Update an existing procedure (requires procedures.edit permission)
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermissionKey(PermissionKeys.ProceduresEdit)]
        public async Task<ActionResult<ProcedureDto>> Update(Guid id, [FromBody] UpdateProcedureDto dto)
        {
            var command = new UpdateProcedureCommand(id, dto, GetTenantId());
            var procedure = await _mediator.Send(command);
            return Ok(procedure);
        }

        /// <summary>
        /// Delete (deactivate) a procedure (requires procedures.delete permission)
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermissionKey(PermissionKeys.ProceduresDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var command = new DeleteProcedureCommand(id, GetTenantId());
            var result = await _mediator.Send(command);
            
            if (!result)
                return NotFound($"Procedure with ID {id} not found");

            return NoContent();
        }

        /// <summary>
        /// Add a procedure to an appointment (requires procedures.create permission)
        /// </summary>
        [HttpPost("appointments/{appointmentId}/procedures")]
        [RequirePermissionKey(PermissionKeys.ProceduresCreate)]
        public async Task<ActionResult<AppointmentProcedureDto>> AddProcedureToAppointment(
            Guid appointmentId,
            [FromBody] AddProcedureToAppointmentDto dto)
        {
            var command = new AddProcedureToAppointmentCommand(appointmentId, dto, GetTenantId());
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Get all procedures for an appointment (requires procedures.view permission)
        /// </summary>
        [HttpGet("appointments/{appointmentId}/procedures")]
        [RequirePermissionKey(PermissionKeys.ProceduresView)]
        public async Task<ActionResult<IEnumerable<AppointmentProcedureDto>>> GetAppointmentProcedures(Guid appointmentId)
        {
            var query = new GetAppointmentProceduresQuery(appointmentId, GetTenantId());
            var procedures = await _mediator.Send(query);
            return Ok(procedures);
        }

        /// <summary>
        /// Get billing summary for an appointment with all procedures and totals (requires procedures.view permission)
        /// </summary>
        [HttpGet("appointments/{appointmentId}/billing-summary")]
        [RequirePermissionKey(PermissionKeys.ProceduresView)]
        public async Task<ActionResult<AppointmentBillingSummaryDto>> GetAppointmentBillingSummary(Guid appointmentId)
        {
            var query = new GetAppointmentBillingSummaryQuery(appointmentId, GetTenantId());
            var summary = await _mediator.Send(query);
            
            if (summary == null)
                return NotFound($"Appointment with ID {appointmentId} not found");

            return Ok(summary);
        }
    }
}
