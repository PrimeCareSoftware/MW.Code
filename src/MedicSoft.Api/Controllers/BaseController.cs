using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected readonly ITenantContext _tenantContext;

        protected BaseController(ITenantContext tenantContext)
        {
            _tenantContext = tenantContext;
        }

        protected string GetTenantId()
        {
            // In a real implementation, this would extract tenant from JWT claims
            // For demo purposes, we'll use a default tenant
            var tenantId = HttpContext.Request.Headers["X-Tenant-Id"].FirstOrDefault();
            return !string.IsNullOrEmpty(tenantId) ? tenantId : "default-tenant";
        }

        protected string? GetUserId()
        {
            return User?.Identity?.Name;
        }

        protected Guid? GetClinicId()
        {
            // Extract clinicId from JWT claims
            var clinicIdClaim = User?.Claims?.FirstOrDefault(c => c.Type == "clinic_id");
            if (clinicIdClaim != null && Guid.TryParse(clinicIdClaim.Value, out var clinicId))
            {
                return clinicId;
            }
            return null;
        }
    }
}