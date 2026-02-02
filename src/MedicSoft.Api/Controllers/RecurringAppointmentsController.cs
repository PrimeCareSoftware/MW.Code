using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Commands.Appointments;
using MedicSoft.Application.Queries.Appointments;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/recurring-appointments")]
    [Authorize]
    public class RecurringAppointmentsController : BaseController
    {
        private readonly IMediator _mediator;

        public RecurringAppointmentsController(IMediator mediator, ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create recurring appointments pattern and generate appointments (requires appointments.create permission)
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.AppointmentsCreate)]
        public async Task<ActionResult<RecurringAppointmentPatternDto>> CreateRecurringAppointments([FromBody] CreateRecurringAppointmentsDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var command = new CreateRecurringAppointmentsCommand(createDto, GetTenantId());
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetPatternById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create recurring blocked time slots pattern and generate blocks (requires appointments.create permission)
        /// </summary>
        [HttpPost("blocked-slots")]
        [RequirePermissionKey(PermissionKeys.AppointmentsCreate)]
        public async Task<ActionResult<RecurringAppointmentPatternDto>> CreateRecurringBlockedSlots([FromBody] CreateRecurringAppointmentPatternDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var command = new CreateRecurringBlockedSlotsCommand(createDto, GetTenantId());
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetPatternById), new { id = result.Id }, result);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all recurring patterns for a clinic
        /// </summary>
        [HttpGet("clinic/{clinicId}")]
        [RequirePermissionKey(PermissionKeys.AppointmentsView)]
        public async Task<ActionResult<IEnumerable<RecurringAppointmentPatternDto>>> GetByClinic(Guid clinicId)
        {
            var query = new GetRecurringPatternsQuery(clinicId, GetTenantId());
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get all recurring patterns for a professional
        /// </summary>
        [HttpGet("professional/{professionalId}")]
        [RequirePermissionKey(PermissionKeys.AppointmentsView)]
        public async Task<ActionResult<IEnumerable<RecurringAppointmentPatternDto>>> GetByProfessional(Guid professionalId)
        {
            var query = new GetRecurringPatternsByProfessionalQuery(professionalId, GetTenantId());
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get all recurring patterns for a patient
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [RequirePermissionKey(PermissionKeys.PatientsView)]
        public async Task<ActionResult<IEnumerable<RecurringAppointmentPatternDto>>> GetByPatient(Guid patientId)
        {
            var query = new GetRecurringPatternsByPatientQuery(patientId, GetTenantId());
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get a specific recurring pattern by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.AppointmentsView)]
        public async Task<ActionResult<RecurringAppointmentPatternDto>> GetPatternById(Guid id)
        {
            var query = new GetRecurringPatternByIdQuery(id, GetTenantId());
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Deactivate a recurring pattern (stops generating future occurrences) (requires appointments.delete permission)
        /// </summary>
        [HttpPost("{id}/deactivate")]
        [RequirePermissionKey(PermissionKeys.AppointmentsDelete)]
        public async Task<ActionResult> Deactivate(Guid id)
        {
            var command = new DeactivateRecurringPatternCommand(id, GetTenantId());
            var result = await _mediator.Send(command);
            
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
