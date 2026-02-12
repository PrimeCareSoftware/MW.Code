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
            [FromQuery] string date, 
            [FromQuery] Guid clinicId,
            [FromQuery] Guid? professionalId = null)
        {
            // Parse date string explicitly to avoid timezone issues
            if (!TryParseDateParameter(date, out var parsedDate))
            {
                return BadRequest("Data inválida. Use o formato yyyy-MM-dd");
            }

            var query = new GetBlockedTimeSlotsQuery(parsedDate, clinicId, GetTenantId(), professionalId);
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// Get blocked time slots for a date range
        /// </summary>
        [HttpGet("range")]
        [RequirePermissionKey(PermissionKeys.AppointmentsView)]
        public async Task<ActionResult<IEnumerable<BlockedTimeSlotDto>>> GetByDateRange(
            [FromQuery] string startDate, 
            [FromQuery] string endDate, 
            [FromQuery] Guid clinicId)
        {
            // Parse date strings explicitly to avoid timezone issues
            if (!TryParseDateParameter(startDate, out var parsedStartDate))
            {
                return BadRequest("Data inicial inválida. Use o formato yyyy-MM-dd");
            }

            if (!TryParseDateParameter(endDate, out var parsedEndDate))
            {
                return BadRequest("Data final inválida. Use o formato yyyy-MM-dd");
            }

            var query = new GetBlockedTimeSlotsRangeQuery(parsedStartDate, parsedEndDate, clinicId, GetTenantId());
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
        public async Task<ActionResult<BlockedTimeSlotDto>> Update(
            Guid id, 
            [FromBody] UpdateBlockedTimeSlotDto updateDto,
            [FromQuery] bool updateSeries = false)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var command = new UpdateBlockedTimeSlotCommand(id, updateDto, GetTenantId(), updateSeries);
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
        /// Delete a blocked time slot with recurrence scope options (requires appointments.delete permission)
        /// </summary>
        /// <param name="id">Blocked slot ID</param>
        /// <param name="scope">Delete scope: 1=ThisOccurrence (default), 2=ThisAndFuture, 3=AllInSeries</param>
        /// <param name="reason">Optional deletion reason for audit trail</param>
        /// <param name="deleteSeries">Deprecated - use 'scope' parameter instead</param>
        [HttpDelete("{id}")]
        [RequirePermissionKey(PermissionKeys.AppointmentsDelete)]
        public async Task<ActionResult> Delete(
            Guid id, 
            [FromQuery] RecurringDeleteScope scope = RecurringDeleteScope.ThisOccurrence,
            [FromQuery] string? reason = null,
            [FromQuery] bool? deleteSeries = null)
        {
            // Handle backward compatibility: if deleteSeries is true, treat as AllInSeries
            if (deleteSeries.HasValue && deleteSeries.Value)
            {
                scope = RecurringDeleteScope.AllInSeries;
            }

            var command = new DeleteRecurringScopeCommand(id, scope, GetTenantId(), reason);
            var result = await _mediator.Send(command);
            
            if (!result)
                return NotFound();

            return NoContent();
        }
    }
}
