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
    /// Update clinic information
    /// </summary>
    [HttpPut("clinics/{id}")]
    public async Task<ActionResult> UpdateClinic(Guid id, [FromBody] UpdateClinicRequest request)
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        var result = await _systemAdminService.UpdateClinicAsync(id, request);

        if (!result)
            return NotFound(new { message = "Clínica não encontrada" });

        return Ok(new { message = "Clínica atualizada com sucesso" });
    }

    /// <summary>
    /// Update clinic subscription
    /// </summary>
    [HttpPut("clinics/{id}/subscription")]
    public async Task<ActionResult> UpdateSubscription(Guid id, [FromBody] UpdateSubscriptionRequest request)
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        var result = await _systemAdminService.UpdateClinicSubscriptionAsync(id, request);

        if (!result)
            return NotFound(new { message = "Assinatura não encontrada" });

        return Ok(new { message = "Assinatura atualizada com sucesso" });
    }

    /// <summary>
    /// Enable manual override for a clinic subscription
    /// </summary>
    [HttpPost("clinics/{id}/subscription/manual-override")]
    public async Task<ActionResult> EnableManualOverride(Guid id, [FromBody] EnableManualOverrideRequest request)
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        var result = await _systemAdminService.EnableManualOverrideAsync(id, request);

        if (!result)
            return NotFound(new { message = "Assinatura não encontrada" });

        return Ok(new { message = "Override manual ativado com sucesso" });
    }

    /// <summary>
    /// Disable manual override for a clinic subscription
    /// </summary>
    [HttpDelete("clinics/{id}/subscription/manual-override")]
    public async Task<ActionResult> DisableManualOverride(Guid id)
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        var result = await _systemAdminService.DisableManualOverrideAsync(id);

        if (!result)
            return NotFound(new { message = "Assinatura não encontrada" });

        return Ok(new { message = "Override manual desativado com sucesso" });
    }

    /// <summary>
    /// Get all subscription plans
    /// </summary>
    [HttpGet("subscription-plans")]
    public async Task<ActionResult> GetSubscriptionPlans([FromQuery] bool? activeOnly = null)
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        var plans = await _systemAdminService.GetSubscriptionPlansAsync(activeOnly);
        return Ok(plans);
    }

    /// <summary>
    /// Get a specific subscription plan
    /// </summary>
    [HttpGet("subscription-plans/{id}")]
    public async Task<ActionResult> GetSubscriptionPlan(Guid id)
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        var plan = await _systemAdminService.GetSubscriptionPlanAsync(id);

        if (plan == null)
            return NotFound(new { message = "Plano não encontrado" });

        return Ok(plan);
    }

    /// <summary>
    /// Create a new subscription plan
    /// </summary>
    [HttpPost("subscription-plans")]
    public async Task<ActionResult> CreateSubscriptionPlan([FromBody] CreateSubscriptionPlanRequest request)
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        var planId = await _systemAdminService.CreateSubscriptionPlanAsync(request);

        return Ok(new
        {
            message = "Plano criado com sucesso",
            planId
        });
    }

    /// <summary>
    /// Update a subscription plan
    /// </summary>
    [HttpPut("subscription-plans/{id}")]
    public async Task<ActionResult> UpdateSubscriptionPlan(Guid id, [FromBody] UpdateSubscriptionPlanRequest request)
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        var result = await _systemAdminService.UpdateSubscriptionPlanAsync(id, request);

        if (!result)
            return NotFound(new { message = "Plano não encontrado" });

        return Ok(new { message = "Plano atualizado com sucesso" });
    }

    /// <summary>
    /// Delete a subscription plan
    /// </summary>
    [HttpDelete("subscription-plans/{id}")]
    public async Task<ActionResult> DeleteSubscriptionPlan(Guid id)
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        try
        {
            var result = await _systemAdminService.DeleteSubscriptionPlanAsync(id);

            if (!result)
                return NotFound(new { message = "Plano não encontrado" });

            return Ok(new { message = "Plano excluído com sucesso" });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Toggle subscription plan status
    /// </summary>
    [HttpPost("subscription-plans/{id}/toggle-status")]
    public async Task<ActionResult> ToggleSubscriptionPlanStatus(Guid id)
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        var result = await _systemAdminService.ToggleSubscriptionPlanStatusAsync(id);

        if (!result)
            return NotFound(new { message = "Plano não encontrado" });

        return Ok(new { message = "Status do plano alterado com sucesso" });
    }

    /// <summary>
    /// Get all clinic owners
    /// </summary>
    [HttpGet("clinic-owners")]
    public async Task<ActionResult> GetClinicOwners([FromQuery] Guid? clinicId = null)
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        var owners = await _systemAdminService.GetClinicOwnersAsync(clinicId);
        return Ok(owners);
    }

    /// <summary>
    /// Reset clinic owner password
    /// </summary>
    [HttpPost("clinic-owners/{id}/reset-password")]
    public async Task<ActionResult> ResetOwnerPassword(Guid id, [FromBody] ResetPasswordRequest request)
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        var result = await _systemAdminService.ResetOwnerPasswordAsync(id, request.NewPassword);

        if (!result)
            return NotFound(new { message = "Proprietário não encontrado" });

        return Ok(new { message = "Senha redefinida com sucesso" });
    }

    /// <summary>
    /// Toggle clinic owner status
    /// </summary>
    [HttpPost("clinic-owners/{id}/toggle-status")]
    public async Task<ActionResult> ToggleOwnerStatus(Guid id)
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        var result = await _systemAdminService.ToggleOwnerStatusAsync(id);

        if (!result)
            return NotFound(new { message = "Proprietário não encontrado" });

        return Ok(new { message = "Status do proprietário alterado com sucesso" });
    }

    /// <summary>
    /// Get all subdomains
    /// </summary>
    [HttpGet("subdomains")]
    public async Task<ActionResult> GetSubdomains()
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        var subdomains = await _systemAdminService.GetSubdomainsAsync();
        return Ok(subdomains);
    }

    /// <summary>
    /// Get subdomain by name (public endpoint for tenant resolution)
    /// </summary>
    [HttpGet("subdomains/resolve/{subdomain}")]
    public async Task<ActionResult> ResolveSubdomain(string subdomain)
    {
        var result = await _systemAdminService.GetSubdomainByNameAsync(subdomain);

        if (result == null)
            return NotFound(new { message = "Subdomínio não encontrado" });

        return Ok(result);
    }

    /// <summary>
    /// Create a new subdomain
    /// </summary>
    [HttpPost("subdomains")]
    public async Task<ActionResult> CreateSubdomain([FromBody] CreateSubdomainRequest request)
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        try
        {
            var subdomainId = await _systemAdminService.CreateSubdomainAsync(request);

            return Ok(new
            {
                message = "Subdomínio criado com sucesso",
                subdomainId
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Delete a subdomain
    /// </summary>
    [HttpDelete("subdomains/{id}")]
    public async Task<ActionResult> DeleteSubdomain(Guid id)
    {
        if (!IsSystemOwner())
        {
            return Forbid();
        }

        var result = await _systemAdminService.DeleteSubdomainAsync(id);

        if (!result)
            return NotFound(new { message = "Subdomínio não encontrado" });

        return Ok(new { message = "Subdomínio excluído com sucesso" });
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
