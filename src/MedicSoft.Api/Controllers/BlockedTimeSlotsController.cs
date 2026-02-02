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
    [Route("api/blocked-time-slots")]
    [Authorize]
    public class BlockedTimeSlotsController : BaseController
    {
        private readonly IMediator _mediator;

        public BlockedTimeSlotsController(IMediator mediator, ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create a new blocked time slot (requires appointments.create permission)
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.AppointmentsCreate)]
        public async Task<ActionResult<BlockedTimeSlotDto>> Create([FromBody] CreateBlockedTimeSlotDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var command = new CreateBlockedTimeSlotCommand(createDto, GetTenantId());
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
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
        /// Get blocked time slots for a specific date and clinic
        /// </summary>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.AppointmentsView)]
        public async Task<ActionResult<IEnumerable<BlockedTimeSlotDto>>> GetByDate(
            [FromQuery] DateTime date, 
            [FromQuery] Guid clinicId,
            [FromQuery] Guid? professionalId = null)
        {
            var query = new GetBlockedTimeSlotsQuery(date, clinicId, GetTenantId(), professionalId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get blocked time slots for a date range
        /// </summary>
        [HttpGet("range")]
        [RequirePermissionKey(PermissionKeys.AppointmentsView)]
        public async Task<ActionResult<IEnumerable<BlockedTimeSlotDto>>> GetByDateRange(
            [FromQuery] DateTime startDate, 
            [FromQuery] DateTime endDate, 
            [FromQuery] Guid clinicId)
        {
            var query = new GetBlockedTimeSlotsRangeQuery(startDate, endDate, clinicId, GetTenantId());
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get a specific blocked time slot by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.AppointmentsView)]
        public async Task<ActionResult<BlockedTimeSlotDto>> GetById(Guid id)
        {
            var query = new GetBlockedTimeSlotByIdQuery(id, GetTenantId());
            var result = await _mediator.Send(query);
            
            if (result == null)
                return NotFound();

            return Ok(result);
        }

        /// <summary>
        /// Update an existing blocked time slot (requires appointments.edit permission)
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermissionKey(PermissionKeys.AppointmentsEdit)]
        public async Task<ActionResult<BlockedTimeSlotDto>> Update(Guid id, [FromBody] UpdateBlockedTimeSlotDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var command = new UpdateBlockedTimeSlotCommand(id, updateDto, GetTenantId());
                var result = await _mediator.Send(command);
                return Ok(result);
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
        /// Delete a blocked time slot (requires appointments.delete permission)
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermissionKey(PermissionKeys.AppointmentsDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var command = new DeleteBlockedTimeSlotCommand(id, GetTenantId());
            var result = await _mediator.Send(command);
            
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
