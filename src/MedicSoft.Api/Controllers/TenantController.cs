using Microsoft.AspNetCore.Mvc;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for tenant resolution and information
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TenantController : ControllerBase
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly ILogger<TenantController> _logger;

        public TenantController(IClinicRepository clinicRepository, ILogger<TenantController> logger)
        {
            _clinicRepository = clinicRepository;
            _logger = logger;
        }

        /// <summary>
        /// Get tenant information by subdomain
        /// </summary>
        /// <param name="subdomain">The clinic subdomain</param>
        /// <returns>Tenant information including tenantId and clinic name</returns>
        [HttpGet("resolve/{subdomain}")]
        public async Task<ActionResult<TenantInfoResponse>> ResolveTenantBySubdomain(string subdomain)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subdomain))
                {
                    return BadRequest(new { message = "Subdomain is required" });
                }

                var clinic = await _clinicRepository.GetBySubdomainAsync(subdomain);
                
                if (clinic == null)
                {
                    _logger.LogWarning("Tenant resolution failed for subdomain: {Subdomain}", subdomain);
                    return NotFound(new { message = "Clinic not found for the specified subdomain" });
                }

                _logger.LogInformation("Successfully resolved subdomain {Subdomain} to clinic: {ClinicName}", subdomain, clinic.Name);

                return Ok(new TenantInfoResponse
                {
                    TenantId = clinic.TenantId,
                    Subdomain = clinic.Subdomain,
                    ClinicName = clinic.Name,
                    ClinicId = clinic.Id,
                    IsActive = clinic.IsActive
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error resolving tenant for subdomain: {Subdomain}", subdomain);
                return StatusCode(500, new { message = "An error occurred while resolving tenant information" });
            }
        }

        /// <summary>
        /// Get current tenant information from context (resolved by middleware)
        /// </summary>
        [HttpGet("current")]
        public ActionResult<TenantInfoResponse> GetCurrentTenant()
        {
            var tenantId = HttpContext.Items["TenantId"] as string;
            var subdomain = HttpContext.Items["Subdomain"] as string;

            if (string.IsNullOrEmpty(tenantId))
            {
                return NotFound(new { message = "No tenant context found. Please access via subdomain or provide tenant information." });
            }

            return Ok(new TenantInfoResponse
            {
                TenantId = tenantId,
                Subdomain = subdomain
            });
        }
    }

    public class TenantInfoResponse
    {
        public string TenantId { get; set; } = string.Empty;
        public string? Subdomain { get; set; }
        public string? ClinicName { get; set; }
        public Guid? ClinicId { get; set; }
        public bool IsActive { get; set; }
    }
}
