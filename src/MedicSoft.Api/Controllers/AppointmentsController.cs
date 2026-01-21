using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/appointments")]
    [Authorize]
    public class AppointmentsController : BaseController
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService, ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _appointmentService = appointmentService;
        }

        /// <summary>
        /// Create a new appointment (requires appointments.create permission)
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.AppointmentsCreate)]
        public async Task<ActionResult<AppointmentDto>> Create([FromBody] CreateAppointmentDto createAppointmentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var appointment = await _appointmentService.CreateAppointmentAsync(createAppointmentDto, GetTenantId());
                return CreatedAtAction(nameof(GetDailyAgenda), 
                    new { date = appointment.ScheduledDate, clinicId = appointment.ClinicId }, 
                    appointment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing appointment (requires appointments.edit permission)
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermissionKey(PermissionKeys.AppointmentsEdit)]
        public async Task<ActionResult<AppointmentDto>> Update(Guid id, [FromBody] UpdateAppointmentDto updateAppointmentDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var appointment = await _appointmentService.UpdateAppointmentAsync(id, updateAppointmentDto, GetTenantId());
                return Ok(appointment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cancel an appointment (requires appointments.edit permission)
        /// </summary>
        [HttpPut("{id}/cancel")]
        [RequirePermissionKey(PermissionKeys.AppointmentsEdit)]
        public async Task<ActionResult> Cancel(Guid id, [FromBody] CancelAppointmentRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _appointmentService.CancelAppointmentAsync(id, request.CancellationReason, GetTenantId());
                
                if (!result)
                    return NotFound($"Appointment with ID {id} not found");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get daily agenda for a clinic (requires appointments.view permission)
        /// </summary>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.AppointmentsView)]
        public async Task<ActionResult<DailyAgendaDto>> GetDailyAgenda([FromQuery] DateTime date, [FromQuery] Guid clinicId)
        {
            try
            {
                var agenda = await _appointmentService.GetDailyAgendaAsync(date, clinicId, GetTenantId());
                return Ok(agenda);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get daily agenda for a clinic - alias endpoint (requires appointments.view permission)
        /// </summary>
        [HttpGet("agenda")]
        [RequirePermissionKey(PermissionKeys.AppointmentsView)]
        public async Task<ActionResult<DailyAgendaDto>> GetDailyAgendaAlias([FromQuery] DateTime date, [FromQuery] Guid clinicId)
        {
            try
            {
                var agenda = await _appointmentService.GetDailyAgendaAsync(date, clinicId, GetTenantId());
                return Ok(agenda);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get appointment by ID (requires appointments.view permission)
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.AppointmentsView)]
        public async Task<ActionResult<AppointmentDto>> GetById(Guid id)
        {
            try
            {
                var appointment = await _appointmentService.GetByIdAsync(id, GetTenantId());
                
                if (appointment == null)
                    return NotFound($"Appointment with ID {id} not found");

                return Ok(appointment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get available time slots for a specific date and clinic
        /// </summary>
        [HttpGet("available-slots")]
        public async Task<ActionResult<IEnumerable<AvailableSlotDto>>> GetAvailableSlots(
            [FromQuery] DateTime date, 
            [FromQuery] Guid clinicId, 
            [FromQuery] int durationMinutes = 30)
        {
            try
            {
                var slots = await _appointmentService.GetAvailableSlotsAsync(date, clinicId, durationMinutes, GetTenantId());
                return Ok(slots);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Mark appointment as paid (requires appointments.edit permission)
        /// </summary>
        [HttpPost("{id}/mark-as-paid")]
        [RequirePermissionKey(PermissionKeys.AppointmentsEdit)]
        public async Task<ActionResult> MarkAsPaid(Guid id, [FromBody] MarkAppointmentAsPaidDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _appointmentService.MarkAppointmentAsPaidAsync(
                    id, GetUserId(), dto.PaymentReceiverType, GetTenantId());
                
                if (!result)
                    return NotFound($"Appointment with ID {id} not found");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Complete appointment (check-out by doctor) (requires appointments.edit permission)
        /// </summary>
        [HttpPost("{id}/complete")]
        [RequirePermissionKey(PermissionKeys.AppointmentsEdit)]
        public async Task<ActionResult> Complete(Guid id, [FromBody] CompleteAppointmentDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _appointmentService.CompleteAppointmentAsync(
                    id, GetUserId(), GetTenantId(), dto.Notes, dto.RegisterPayment);
                
                if (!result)
                    return NotFound($"Appointment with ID {id} not found");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }

    public class CancelAppointmentRequest
    {
        public string CancellationReason { get; set; } = string.Empty;
    }
}