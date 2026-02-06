using Microsoft.AspNetCore.Mvc;
using PatientPortal.Infrastructure.Services;

namespace PatientPortal.Api.Controllers;

/// <summary>
/// Controller for seeding demo data in the Patient Portal
/// WARNING: These endpoints should be disabled in production
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class DataSeederController : ControllerBase
{
    private readonly PatientPortalSeederService _seederService;
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;

    public DataSeederController(
        PatientPortalSeederService seederService,
        IWebHostEnvironment environment,
        IConfiguration configuration)
    {
        _seederService = seederService;
        _environment = environment;
        _configuration = configuration;
    }

    /// <summary>
    /// Seed demo data for testing purposes
    /// WARNING: This endpoint should be disabled in production
    /// </summary>
    /// <remarks>
    /// This endpoint creates demo patient portal users based on existing patients in the main database.
    /// All demo users will have the password: Patient@123
    /// 
    /// Prerequisites:
    /// - Main application database must be seeded first (POST /api/data-seeder/seed-demo on main API)
    /// - Demo clinic must exist with ID: demo-clinic-001
    /// </remarks>
    /// <response code="200">Demo data seeded successfully</response>
    /// <response code="400">Data already exists or prerequisites not met</response>
    /// <response code="403">Endpoint not available in production</response>
    [HttpPost("seed-demo")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 400)]
    [ProducesResponseType(typeof(object), 403)]
    public async Task<ActionResult> SeedDemoData()
    {
        // Check if development mode is enabled
        var devModeEnabled = _configuration.GetValue<bool>("Development:EnableDevEndpoints", false);
        
        if (!_environment.IsDevelopment() && !devModeEnabled)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                error = "This endpoint is only available in Development environment or when Development:EnableDevEndpoints is true"
            });
        }

        try
        {
            await _seederService.SeedDemoDataAsync();
            
            return Ok(new
            {
                message = "Demo data seeded successfully for Patient Portal",
                tenantId = "demo-clinic-001",
                credentials = new
                {
                    note = "Use these credentials to login to the patient portal",
                    password = "Patient@123",
                    loginEndpoint = "POST /api/auth/login",
                    users = "All patients from demo clinic can login using their email or CPF and the password above"
                },
                summary = new
                {
                    patientUsers = "Created from existing patients in main database",
                    emailConfirmed = true,
                    twoFactorEnabled = false
                },
                nextSteps = new[]
                {
                    "1. Use GET /api/data-seeder/demo-info to see all available patient emails",
                    "2. Login with any patient email or CPF using password: Patient@123",
                    "3. Test the patient portal features"
                }
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                error = "An error occurred while seeding data", 
                details = ex.Message,
                stackTrace = _environment.IsDevelopment() ? ex.StackTrace : null
            });
        }
    }

    /// <summary>
    /// Get information about demo data
    /// </summary>
    /// <remarks>
    /// Returns information about available demo patient portal users and how to use them
    /// </remarks>
    /// <response code="200">Demo data information</response>
    [HttpGet("demo-info")]
    [ProducesResponseType(typeof(object), 200)]
    public async Task<ActionResult> GetDemoInfo()
    {
        try
        {
            // Get patient users from database
            var patientUsers = await _seederService.GetPatientUsersAsync();
            
            return Ok(new
            {
                tenantId = "demo-clinic-001",
                totalUsers = patientUsers.Count,
                loginCredentials = new
                {
                    password = "Patient@123",
                    note = "Use any patient email or CPF with this password",
                    endpoint = "POST /api/auth/login"
                },
                patients = patientUsers.Select(u => new
                {
                    email = u.Email,
                    cpf = u.CPF,
                    fullName = u.FullName,
                    emailConfirmed = u.EmailConfirmed,
                    twoFactorEnabled = u.TwoFactorEnabled
                }).ToList(),
                availableEndpoints = new[]
                {
                    "POST /api/auth/login - Login with email/CPF and password",
                    "GET /api/appointments - View patient appointments",
                    "GET /api/documents - View patient documents",
                    "GET /api/profile - View patient profile",
                    "PUT /api/profile - Update patient profile"
                },
                note = "If no users are shown, run POST /api/data-seeder/seed-demo first"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new 
            { 
                error = "An error occurred while fetching demo info", 
                details = ex.Message 
            });
        }
    }

    /// <summary>
    /// Clear all data from the patient portal database
    /// WARNING: This will delete all patient portal data. Use with caution!
    /// This endpoint should only be available in Development environment
    /// </summary>
    /// <remarks>
    /// Deletes all data from patient portal tables:
    /// - TwoFactorTokens
    /// - PasswordResetTokens
    /// - EmailVerificationTokens
    /// - RefreshTokens
    /// - PatientUsers
    /// </remarks>
    /// <response code="200">Database cleared successfully</response>
    /// <response code="403">Endpoint not available in production</response>
    [HttpDelete("clear-database")]
    [ProducesResponseType(typeof(object), 200)]
    [ProducesResponseType(typeof(object), 403)]
    public async Task<ActionResult> ClearDatabase()
    {
        // Check if development mode is enabled
        var devModeEnabled = _configuration.GetValue<bool>("Development:EnableDevEndpoints", false);
        
        if (!_environment.IsDevelopment() && !devModeEnabled)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new
            {
                error = "This endpoint is only available in Development environment or when Development:EnableDevEndpoints is true"
            });
        }

        try
        {
            await _seederService.ClearDatabaseAsync();
            
            return Ok(new
            {
                message = "Patient Portal database cleared successfully",
                deletedTables = new[]
                {
                    "TwoFactorTokens",
                    "PasswordResetTokens", 
                    "EmailVerificationTokens",
                    "RefreshTokens",
                    "PatientUsers"
                },
                note = "All patient portal data has been removed. You can now re-seed the database using POST /api/data-seeder/seed-demo"
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                error = "An error occurred while clearing the database",
                details = ex.Message,
                stackTrace = _environment.IsDevelopment() ? ex.StackTrace : null
            });
        }
    }
}
