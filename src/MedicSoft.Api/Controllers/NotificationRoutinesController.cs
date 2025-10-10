using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.Commands.NotificationRoutines;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.NotificationRoutines;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing notification routines - automated notification scheduling
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class NotificationRoutinesController : BaseController
    {
        private readonly IMediator _mediator;

        public NotificationRoutinesController(ITenantContext tenantContext, IMediator mediator) 
            : base(tenantContext)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Get all notification routines for the current tenant
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<NotificationRoutineDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<NotificationRoutineDto>>> GetAll()
        {
            var tenantId = GetTenantId();
            var query = new GetAllNotificationRoutinesQuery(tenantId);
            var routines = await _mediator.Send(query);
            return Ok(routines);
        }

        /// <summary>
        /// Get active notification routines for the current tenant
        /// </summary>
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<NotificationRoutineDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<NotificationRoutineDto>>> GetActive()
        {
            var tenantId = GetTenantId();
            var query = new GetActiveNotificationRoutinesQuery(tenantId);
            var routines = await _mediator.Send(query);
            return Ok(routines);
        }

        /// <summary>
        /// Get a notification routine by ID
        /// </summary>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(NotificationRoutineDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NotificationRoutineDto>> GetById(Guid id)
        {
            var tenantId = GetTenantId();
            var query = new GetNotificationRoutineByIdQuery(id, tenantId);
            var routine = await _mediator.Send(query);
            
            if (routine == null)
                return NotFound(new { message = "Notification routine not found" });

            return Ok(routine);
        }

        /// <summary>
        /// Create a new notification routine
        /// </summary>
        /// <remarks>
        /// Sample request:
        /// 
        ///     POST /api/notificationroutines
        ///     {
        ///         "name": "Daily Appointment Reminder",
        ///         "description": "Send daily reminders for next-day appointments",
        ///         "channel": "WhatsApp",
        ///         "type": "AppointmentReminder",
        ///         "messageTemplate": "Hello {patientName}, you have an appointment tomorrow at {appointmentTime}",
        ///         "scheduleType": "Daily",
        ///         "scheduleConfiguration": "{\"time\":\"18:00\"}",
        ///         "scope": "Clinic",
        ///         "maxRetries": 3,
        ///         "recipientFilter": "{\"hasAppointmentNextDay\":true}"
        ///     }
        /// 
        /// Available channels: SMS, WhatsApp, Email, Push
        /// Available types: AppointmentReminder, AppointmentConfirmation, AppointmentCancellation, AppointmentRescheduled, PaymentReminder, PrescriptionReady, ExamResults, General
        /// Available scheduleTypes: Daily, Weekly, Monthly, Custom, BeforeAppointment, AfterAppointment
        /// Available scopes: Clinic, System (System requires admin privileges)
        /// </remarks>
        [HttpPost]
        [ProducesResponseType(typeof(NotificationRoutineDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<NotificationRoutineDto>> Create([FromBody] CreateNotificationRoutineDto dto)
        {
            try
            {
                var tenantId = GetTenantId();
                var command = new CreateNotificationRoutineCommand(dto, tenantId);
                var routine = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = routine.Id }, routine);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing notification routine
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(NotificationRoutineDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<NotificationRoutineDto>> Update(Guid id, [FromBody] UpdateNotificationRoutineDto dto)
        {
            try
            {
                var tenantId = GetTenantId();
                var command = new UpdateNotificationRoutineCommand(id, dto, tenantId);
                var routine = await _mediator.Send(command);
                return Ok(routine);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a notification routine
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Delete(Guid id)
        {
            var tenantId = GetTenantId();
            var command = new DeleteNotificationRoutineCommand(id, tenantId);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound(new { message = "Notification routine not found" });

            return NoContent();
        }

        /// <summary>
        /// Activate a notification routine
        /// </summary>
        [HttpPost("{id}/activate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Activate(Guid id)
        {
            var tenantId = GetTenantId();
            var command = new ActivateNotificationRoutineCommand(id, tenantId);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound(new { message = "Notification routine not found" });

            return Ok(new { message = "Notification routine activated successfully" });
        }

        /// <summary>
        /// Deactivate a notification routine
        /// </summary>
        [HttpPost("{id}/deactivate")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> Deactivate(Guid id)
        {
            var tenantId = GetTenantId();
            var command = new DeactivateNotificationRoutineCommand(id, tenantId);
            var result = await _mediator.Send(command);

            if (!result)
                return NotFound(new { message = "Notification routine not found" });

            return Ok(new { message = "Notification routine deactivated successfully" });
        }
    }
}
