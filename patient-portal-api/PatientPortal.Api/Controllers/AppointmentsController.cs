using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientPortal.Application.DTOs.Appointments;
using PatientPortal.Application.Interfaces;
using PatientPortal.Domain.Enums;

namespace PatientPortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(IAppointmentService appointmentService, ILogger<AppointmentsController> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    /// <summary>
    /// Get all appointments for the authenticated patient
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<AppointmentDto>>> GetMyAppointments([FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        try
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var appointments = await _appointmentService.GetMyAppointmentsAsync(userId.Value, skip, take);
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointments");
            return StatusCode(500, new { message = "An error occurred while retrieving appointments" });
        }
    }

    /// <summary>
    /// Get upcoming appointments for the authenticated patient
    /// </summary>
    [HttpGet("upcoming")]
    public async Task<ActionResult<List<AppointmentDto>>> GetUpcomingAppointments([FromQuery] int take = 10)
    {
        try
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var appointments = await _appointmentService.GetUpcomingAppointmentsAsync(userId.Value, take);
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting upcoming appointments");
            return StatusCode(500, new { message = "An error occurred while retrieving upcoming appointments" });
        }
    }

    /// <summary>
    /// Get appointment by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<AppointmentDto>> GetAppointmentById(Guid id)
    {
        try
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var appointment = await _appointmentService.GetByIdAsync(id, userId.Value);
            
            if (appointment == null)
                return NotFound(new { message = "Appointment not found" });

            return Ok(appointment);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointment {AppointmentId}", id);
            return StatusCode(500, new { message = "An error occurred while retrieving the appointment" });
        }
    }

    /// <summary>
    /// Get appointments by status
    /// </summary>
    [HttpGet("status/{status}")]
    public async Task<ActionResult<List<AppointmentDto>>> GetAppointmentsByStatus(AppointmentStatus status, [FromQuery] int skip = 0, [FromQuery] int take = 50)
    {
        try
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var appointments = await _appointmentService.GetByStatusAsync(userId.Value, status, skip, take);
            return Ok(appointments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointments by status {Status}", status);
            return StatusCode(500, new { message = "An error occurred while retrieving appointments" });
        }
    }

    /// <summary>
    /// Get total count of appointments
    /// </summary>
    [HttpGet("count")]
    public async Task<ActionResult<int>> GetAppointmentsCount()
    {
        try
        {
            var userId = GetUserId();
            if (userId == null)
                return Unauthorized();

            var count = await _appointmentService.GetCountAsync(userId.Value);
            return Ok(new { count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting appointments count");
            return StatusCode(500, new { message = "An error occurred while counting appointments" });
        }
    }

    private Guid? GetUserId()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            return null;
        return userId;
    }
}
