using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PatientPortal.Application.DTOs.Appointments;
using PatientPortal.Application.Interfaces;
using PatientPortal.Domain.Enums;

namespace PatientPortal.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : BaseController
{
    private readonly IAppointmentService _appointmentService;
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(IAppointmentService appointmentService, ILogger<AppointmentsController> logger)
    {
        _appointmentService = appointmentService;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all appointments for the authenticated patient with pagination
    /// </summary>
    /// <param name="skip">Number of records to skip for pagination (default: 0)</param>
    /// <param name="take">Number of records to return (default: 50, max: 100)</param>
    /// <returns>List of appointments with details</returns>
    /// <response code="200">Returns the list of appointments</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="500">Internal server error</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/appointments?skip=0&amp;take=20
    ///     Authorization: Bearer {access-token}
    /// 
    /// Returns all appointments (past, present, and future) for the authenticated patient.
    /// Use query parameters for pagination to handle large result sets efficiently.
    /// 
    /// **Response includes:**
    /// - Appointment ID, date, and time
    /// - Doctor information
    /// - Clinic/location details
    /// - Appointment status (Scheduled, Confirmed, Completed, Cancelled)
    /// - Appointment type/specialty
    /// </remarks>
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
    /// Retrieves upcoming appointments for the authenticated patient
    /// </summary>
    /// <param name="take">Maximum number of upcoming appointments to return (default: 10)</param>
    /// <returns>List of future appointments sorted by date</returns>
    /// <response code="200">Returns the list of upcoming appointments</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="500">Internal server error</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/appointments/upcoming?take=5
    ///     Authorization: Bearer {access-token}
    /// 
    /// Returns only future appointments (scheduled date is after current date/time).
    /// Results are sorted by appointment date in ascending order (nearest first).
    /// Useful for dashboard widgets showing next appointments.
    /// </remarks>
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
    /// Retrieves a specific appointment by its ID
    /// </summary>
    /// <param name="id">Unique identifier of the appointment</param>
    /// <returns>Detailed information about the appointment</returns>
    /// <response code="200">Returns the appointment details</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="404">Appointment not found or doesn't belong to the authenticated patient</response>
    /// <response code="500">Internal server error</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/appointments/3fa85f64-5717-4562-b3fc-2c963f66afa6
    ///     Authorization: Bearer {access-token}
    /// 
    /// Security: Users can only access their own appointments. Attempting to access
    /// another patient's appointment will return 404 Not Found.
    /// </remarks>
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
    /// Retrieves appointments filtered by status with pagination
    /// </summary>
    /// <param name="status">Status to filter by (Scheduled=0, Confirmed=1, InProgress=2, Completed=3, Cancelled=4, NoShow=5)</param>
    /// <param name="skip">Number of records to skip for pagination (default: 0)</param>
    /// <param name="take">Number of records to return (default: 50, max: 100)</param>
    /// <returns>List of appointments matching the specified status</returns>
    /// <response code="200">Returns the filtered list of appointments</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="500">Internal server error</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/appointments/status/1?skip=0&amp;take=20
    ///     Authorization: Bearer {access-token}
    /// 
    /// **Appointment Status Values:**
    /// - 0 = Scheduled (appointment booked but not confirmed)
    /// - 1 = Confirmed (appointment confirmed by clinic)
    /// - 2 = InProgress (patient is currently being attended)
    /// - 3 = Completed (appointment finished successfully)
    /// - 4 = Cancelled (appointment was cancelled)
    /// - 5 = NoShow (patient did not attend)
    /// 
    /// Useful for filtering appointments by their current state.
    /// </remarks>
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
    /// Gets the total count of appointments for the authenticated patient
    /// </summary>
    /// <returns>Total number of appointments</returns>
    /// <response code="200">Returns the count of appointments</response>
    /// <response code="401">User is not authenticated</response>
    /// <response code="500">Internal server error</response>
    /// <remarks>
    /// Sample request:
    /// 
    ///     GET /api/appointments/count
    ///     Authorization: Bearer {access-token}
    /// 
    /// Sample response:
    /// 
    ///     {
    ///         "count": 25
    ///     }
    /// 
    /// Returns the total count of all appointments (past, present, and future)
    /// for the authenticated patient. Useful for dashboard statistics and pagination.
    /// </remarks>
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
}
