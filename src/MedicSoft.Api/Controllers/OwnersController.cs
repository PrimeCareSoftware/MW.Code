using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for owner management - create, update, activate/deactivate owners
    /// SECURITY: This controller is restricted to SystemAdmin role only.
    /// Clinic owners (who contract the medicwarehouse-app service) should NOT have access.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = RoleNames.SystemAdmin)]
    [RequireSystemOwner]
    public class OwnersController : BaseController
    {
        private readonly IOwnerService _ownerService;

        public OwnersController(IOwnerService ownerService, ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _ownerService = ownerService;
        }

        /// <summary>
        /// Get all owners (SystemAdmin only)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OwnerDto>>> GetAllOwners()
        {
            var tenantId = GetTenantId();
            var owners = await _ownerService.GetAllOwnersAsync(tenantId);

            return Ok(owners.Select(o => new OwnerDto
            {
                Id = o.Id,
                Username = o.Username,
                Email = o.Email,
                FullName = o.FullName,
                Phone = o.Phone,
                ClinicId = o.ClinicId,
                IsActive = o.IsActive,
                LastLoginAt = o.LastLoginAt,
                ProfessionalId = o.ProfessionalId,
                Specialty = o.Specialty,
                IsSystemOwner = o.IsSystemOwner
            }));
        }

        /// <summary>
        /// Get owner by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<OwnerDto>> GetOwner(Guid id)
        {
            var tenantId = GetTenantId();
            var owner = await _ownerService.GetOwnerByIdAsync(id, tenantId);

            if (owner == null)
                return NotFound(new { message = "Owner not found" });

            return Ok(new OwnerDto
            {
                Id = owner.Id,
                Username = owner.Username,
                Email = owner.Email,
                FullName = owner.FullName,
                Phone = owner.Phone,
                ClinicId = owner.ClinicId,
                IsActive = owner.IsActive,
                LastLoginAt = owner.LastLoginAt,
                ProfessionalId = owner.ProfessionalId,
                Specialty = owner.Specialty,
                IsSystemOwner = owner.IsSystemOwner
            });
        }

        /// <summary>
        /// Get owner by clinic ID
        /// </summary>
        [HttpGet("by-clinic/{clinicId}")]
        public async Task<ActionResult<OwnerDto>> GetOwnerByClinic(Guid clinicId)
        {
            var tenantId = GetTenantId();
            var owner = await _ownerService.GetOwnerByClinicIdAsync(clinicId, tenantId);

            if (owner == null)
                return NotFound(new { message = "Owner not found for this clinic" });

            return Ok(new OwnerDto
            {
                Id = owner.Id,
                Username = owner.Username,
                Email = owner.Email,
                FullName = owner.FullName,
                Phone = owner.Phone,
                ClinicId = owner.ClinicId,
                IsActive = owner.IsActive,
                LastLoginAt = owner.LastLoginAt,
                ProfessionalId = owner.ProfessionalId,
                Specialty = owner.Specialty,
                IsSystemOwner = owner.IsSystemOwner
            });
        }

        /// <summary>
        /// Create new owner (SystemAdmin only)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<OwnerDto>> CreateOwner([FromBody] CreateOwnerRequest request)
        {
            try
            {
                var tenantId = GetTenantId();
                var owner = await _ownerService.CreateOwnerAsync(
                    request.Username,
                    request.Email,
                    request.Password,
                    request.FullName,
                    request.Phone,
                    tenantId,
                    request.ClinicId,
                    request.ProfessionalId,
                    request.Specialty
                );

                return CreatedAtAction(nameof(GetOwner), new { id = owner.Id }, new OwnerDto
                {
                    Id = owner.Id,
                    Username = owner.Username,
                    Email = owner.Email,
                    FullName = owner.FullName,
                    Phone = owner.Phone,
                    ClinicId = owner.ClinicId,
                    IsActive = owner.IsActive,
                    ProfessionalId = owner.ProfessionalId,
                    Specialty = owner.Specialty,
                    IsSystemOwner = owner.IsSystemOwner
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update owner profile
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateOwner(Guid id, [FromBody] UpdateOwnerRequest request)
        {
            try
            {
                var tenantId = GetTenantId();
                await _ownerService.UpdateOwnerProfileAsync(
                    id,
                    request.Email,
                    request.FullName,
                    request.Phone,
                    tenantId,
                    request.ProfessionalId,
                    request.Specialty
                );

                return Ok(new { message = "Owner updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Activate owner (SystemAdmin only)
        /// </summary>
        [HttpPost("{id}/activate")]
        public async Task<ActionResult> ActivateOwner(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                await _ownerService.ActivateOwnerAsync(id, tenantId);
                return Ok(new { message = "Owner activated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate owner (SystemAdmin only)
        /// </summary>
        [HttpPost("{id}/deactivate")]
        public async Task<ActionResult> DeactivateOwner(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                await _ownerService.DeactivateOwnerAsync(id, tenantId);
                return Ok(new { message = "Owner deactivated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }

    public class CreateOwnerRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public Guid? ClinicId { get; set; }
        public string? ProfessionalId { get; set; }
        public string? Specialty { get; set; }
    }

    public class UpdateOwnerRequest
    {
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? ProfessionalId { get; set; }
        public string? Specialty { get; set; }
    }

    public class OwnerDto
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public Guid? ClinicId { get; set; }
        public bool IsActive { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public string? ProfessionalId { get; set; }
        public string? Specialty { get; set; }
        public bool IsSystemOwner { get; set; }
    }
}
