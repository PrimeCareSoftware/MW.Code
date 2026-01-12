using System;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    public abstract class BaseController : ControllerBase
    {
        protected readonly ITenantContext _tenantContext;

        protected BaseController(ITenantContext tenantContext)
        {
            _tenantContext = tenantContext;
        }

        protected string GetTenantId()
        {
            // Try to get tenant from JWT claims first
            var tenantClaim = User?.FindFirst("tenant_id");
            if (tenantClaim != null && !string.IsNullOrEmpty(tenantClaim.Value))
            {
                return tenantClaim.Value;
            }

            // Try to get tenant from X-Tenant-Id header (set by TenantResolutionMiddleware)
            var tenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            if (!string.IsNullOrEmpty(tenantId))
            {
                return tenantId;
            }

            // Try to get tenant from HttpContext items (set by TenantResolutionMiddleware)
            if (HttpContext.Items.TryGetValue("TenantId", out var tenantIdObj) && tenantIdObj is string contextTenantId)
            {
                return contextTenantId;
            }

            // Fall back to default tenant only if truly unavailable
            return "default-tenant";
        }

        protected Guid GetUserId()
        {
            var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim != null && Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            return Guid.Empty;
        }

        protected Guid? GetClinicId()
        {
            // Extract clinicId from JWT claims
            var clinicIdClaim = User?.FindFirst("clinic_id");
            if (clinicIdClaim != null && Guid.TryParse(clinicIdClaim.Value, out var clinicId))
            {
                return clinicId;
            }
            return null;
        }
    }
}