using Microsoft.AspNetCore.Mvc;
using MedicSoft.Appointments.Api.Models;
using MedicSoft.Appointments.Api.Services;
using MedicSoft.Shared.Authentication;

namespace MedicSoft.Appointments.Api.Controllers;

[Route("api/[controller]")]
public class AppointmentsController : MicroserviceBaseController
{
    private readonly IAppointmentService _appointmentService;

    public AppointmentsController(IAppointmentService appointmentService)
    {
        _appointmentService = appointmentService;
    }

    /// <summary>
    /// Create a new appointment
    /// </summary>
    [HttpPost]
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
    /// Cancel an appointment
    /// </summary>
    [HttpPut("{id}/cancel")]
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
    /// Get daily agenda for a clinic
    /// </summary>
    [HttpGet("agenda")]
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
    /// Get appointment by ID
    /// </summary>
    [HttpGet("{id}")]
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
    /// Check in a patient for their appointment
    /// </summary>
    [HttpPost("{id}/check-in")]
    public async Task<ActionResult<AppointmentDto>> CheckIn(Guid id)
    {
        try
        {
            var appointment = await _appointmentService.CheckInPatientAsync(id, GetTenantId());

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
    /// Start consultation for an appointment
    /// </summary>
    [HttpPost("{id}/start")]
    public async Task<ActionResult<AppointmentDto>> StartConsultation(Guid id)
    {
        try
        {
            var appointment = await _appointmentService.StartConsultationAsync(id, GetTenantId());

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
    /// Complete consultation for an appointment
    /// </summary>
    [HttpPost("{id}/complete")]
    public async Task<ActionResult<AppointmentDto>> CompleteConsultation(Guid id)
    {
        try
        {
            var appointment = await _appointmentService.CompleteConsultationAsync(id, GetTenantId());

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
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "Appointments.Microservice" });
    }
}
