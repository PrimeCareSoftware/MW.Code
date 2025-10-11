using MediatR;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.Commands.Procedures;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Procedures;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProceduresController : BaseController
    {
        private readonly IMediator _mediator;

        public ProceduresController(IMediator mediator, ITenantContext tenantContext)
            : base(tenantContext)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all procedures for the clinic
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProcedureDto>>> GetAll([FromQuery] bool activeOnly = true)
        {
            var query = new GetProceduresByClinicQuery(GetTenantId(), activeOnly);
            var procedures = await _mediator.Send(query);
            return Ok(procedures);
        }

        /// <summary>
        /// Get procedure by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProcedureDto>> GetById(Guid id)
        {
            var query = new GetProcedureByIdQuery(id, GetTenantId());
            var procedure = await _mediator.Send(query);
            
            if (procedure == null)
                return NotFound($"Procedure with ID {id} not found");

            return Ok(procedure);
        }

        /// <summary>
        /// Create a new procedure
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ProcedureDto>> Create([FromBody] CreateProcedureDto dto)
        {
            var command = new CreateProcedureCommand(dto, GetTenantId());
            var procedure = await _mediator.Send(command);
            return CreatedAtAction(nameof(GetById), new { id = procedure.Id }, procedure);
        }

        /// <summary>
        /// Update an existing procedure
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ProcedureDto>> Update(Guid id, [FromBody] UpdateProcedureDto dto)
        {
            var command = new UpdateProcedureCommand(id, dto, GetTenantId());
            var procedure = await _mediator.Send(command);
            return Ok(procedure);
        }

        /// <summary>
        /// Delete (deactivate) a procedure
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            var command = new DeleteProcedureCommand(id, GetTenantId());
            var result = await _mediator.Send(command);
            
            if (!result)
                return NotFound($"Procedure with ID {id} not found");

            return NoContent();
        }

        /// <summary>
        /// Add a procedure to an appointment
        /// </summary>
        [HttpPost("appointments/{appointmentId}/procedures")]
        public async Task<ActionResult<AppointmentProcedureDto>> AddProcedureToAppointment(
            Guid appointmentId,
            [FromBody] AddProcedureToAppointmentDto dto)
        {
            var command = new AddProcedureToAppointmentCommand(appointmentId, dto, GetTenantId());
            var result = await _mediator.Send(command);
            return Ok(result);
        }

        /// <summary>
        /// Get all procedures for an appointment
        /// </summary>
        [HttpGet("appointments/{appointmentId}/procedures")]
        public async Task<ActionResult<IEnumerable<AppointmentProcedureDto>>> GetAppointmentProcedures(Guid appointmentId)
        {
            var query = new GetAppointmentProceduresQuery(appointmentId, GetTenantId());
            var procedures = await _mediator.Send(query);
            return Ok(procedures);
        }

        /// <summary>
        /// Get billing summary for an appointment with all procedures and totals
        /// </summary>
        [HttpGet("appointments/{appointmentId}/billing-summary")]
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
