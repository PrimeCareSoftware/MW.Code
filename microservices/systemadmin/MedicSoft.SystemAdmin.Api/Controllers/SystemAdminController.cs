using Microsoft.AspNetCore.Mvc;
using MedicSoft.SystemAdmin.Api.Models;
using MedicSoft.SystemAdmin.Api.Services;
using MedicSoft.Shared.Authentication;

namespace MedicSoft.SystemAdmin.Api.Controllers;

[Route("api/system-admin")]
public class SystemAdminController : MicroserviceBaseController
{
    private readonly ISystemAdminService _systemAdminService;

    public SystemAdminController(ISystemAdminService systemAdminService)
    {
        _systemAdminService = systemAdminService;
    }

    /// <summary>
    /// Get all clinics in the system (cross-tenant)
    /// </summary>
    [HttpGet("clinics")]
    public async Task<ActionResult> GetAllClinics(
        [FromQuery] string? status = null,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20)
    {
        // Verify user is system owner
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        var (clinics, totalCount) = await _systemAdminService.GetAllClinicsAsync(status, page, pageSize);

        return Ok(new
        {
            totalCount,
            page,
            pageSize,
            totalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
            clinics
        });
    }

    /// <summary>
    /// Get detailed information about a specific clinic
    /// </summary>
    [HttpGet("clinics/{id}")]
    public async Task<ActionResult<ClinicDetailDto>> GetClinic(Guid id)
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        var clinic = await _systemAdminService.GetClinicDetailsAsync(id);

        if (clinic == null)
            return NotFound(new { message = "Clínica não encontrada" });

        return Ok(clinic);
    }

    /// <summary>
    /// Create a new clinic with owner
    /// </summary>
    [HttpPost("clinics")]
    public async Task<ActionResult> CreateClinic([FromBody] CreateClinicRequest request)
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        try
        {
            var (clinicId, tenantId) = await _systemAdminService.CreateClinicAsync(request);

            return Ok(new
            {
                message = "Clínica criada com sucesso",
                clinicId,
                tenantId
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Activate or deactivate a clinic
    /// </summary>
    [HttpPost("clinics/{id}/toggle-status")]
    public async Task<ActionResult> ToggleClinicStatus(Guid id)
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        var result = await _systemAdminService.ToggleClinicStatusAsync(id);

        if (!result)
            return NotFound(new { message = "Clínica não encontrada" });

        return Ok(new { message = "Status da clínica alterado com sucesso" });
    }

    /// <summary>
    /// Get system-wide analytics
    /// </summary>
    [HttpGet("analytics")]
    public async Task<ActionResult<SystemAnalyticsDto>> GetSystemAnalytics()
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        var analytics = await _systemAdminService.GetSystemAnalyticsAsync();
        return Ok(analytics);
    }

    /// <summary>
    /// Create a new System Owner
    /// </summary>
    [HttpPost("system-owners")]
    public async Task<ActionResult> CreateSystemOwner([FromBody] CreateSystemOwnerRequest request)
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        try
        {
            var ownerId = await _systemAdminService.CreateSystemOwnerAsync(request);

            return Ok(new
            {
                message = "System owner criado com sucesso",
                ownerId,
                username = request.Username,
                isSystemOwner = true
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Get all system owners
    /// </summary>
    [HttpGet("system-owners")]
    public async Task<ActionResult> GetSystemOwners()
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        var owners = await _systemAdminService.GetSystemOwnersAsync();
        return Ok(owners);
    }

    /// <summary>
    /// Health check endpoint
    /// </summary>
    [HttpGet("health")]
    public IActionResult Health()
    {
        return Ok(new { status = "healthy", service = "SystemAdmin.Microservice" });
    }
}
