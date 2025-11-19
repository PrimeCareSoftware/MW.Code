using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing owner-clinic relationships
    /// </summary>
    [Authorize]
    [ApiController]
    [Route("api/owner-clinic-links")]
    public class OwnerClinicLinksController : BaseController
    {
        private readonly IOwnerClinicLinkService _linkService;

        public OwnerClinicLinksController(
            IOwnerClinicLinkService linkService,
            ITenantContext tenantContext) : base(tenantContext)
        {
            _linkService = linkService;
        }

        /// <summary>
        /// Get all clinics for the authenticated owner
        /// </summary>
        [HttpGet("my-clinics")]
        [ProducesResponseType(typeof(IEnumerable<OwnerClinicSummaryDto>), 200)]
        public async Task<IActionResult> GetMyClinics()
        {
            var ownerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "owner_id");
            if (ownerIdClaim == null || !Guid.TryParse(ownerIdClaim.Value, out var ownerId))
                return Unauthorized("Owner ID not found in token");

            var links = await _linkService.GetClinicsWithSubscriptionsAsync(ownerId);
            
            var summaries = links.Select(l => new OwnerClinicSummaryDto
            {
                Id = l.Id,
                ClinicId = l.ClinicId,
                ClinicName = l.Clinic?.Name ?? "Unknown",
                ClinicCNPJ = l.Clinic?.Document ?? "Unknown",
                IsPrimaryOwner = l.IsPrimaryOwner,
                Role = l.Role,
                OwnershipPercentage = l.OwnershipPercentage,
                // Subscription details would be populated from related entities
            });

            return Ok(summaries);
        }

        /// <summary>
        /// Get all clinics for a specific owner (admin only)
        /// </summary>
        [HttpGet("owner/{ownerId}/clinics")]
        [ProducesResponseType(typeof(IEnumerable<OwnerClinicLinkDto>), 200)]
        public async Task<IActionResult> GetOwnerClinics(Guid ownerId)
        {
            var links = await _linkService.GetOwnerClinicsAsync(ownerId);
            
            var dtos = links.Select(l => new OwnerClinicLinkDto
            {
                Id = l.Id,
                OwnerId = l.OwnerId,
                ClinicId = l.ClinicId,
                LinkedDate = l.LinkedDate,
                IsActive = l.IsActive,
                IsPrimaryOwner = l.IsPrimaryOwner,
                Role = l.Role,
                OwnershipPercentage = l.OwnershipPercentage,
                InactivatedDate = l.InactivatedDate,
                InactivationReason = l.InactivationReason,
                ClinicName = l.Clinic?.Name,
                ClinicCNPJ = l.Clinic?.Document,
                OwnerFullName = l.Owner?.FullName,
                OwnerEmail = l.Owner?.Email
            });

            return Ok(dtos);
        }

        /// <summary>
        /// Get all owners for a specific clinic
        /// </summary>
        [HttpGet("clinic/{clinicId}/owners")]
        [ProducesResponseType(typeof(IEnumerable<OwnerClinicLinkDto>), 200)]
        public async Task<IActionResult> GetClinicOwners(Guid clinicId)
        {
            var links = await _linkService.GetClinicOwnersAsync(clinicId);
            
            var dtos = links.Select(l => new OwnerClinicLinkDto
            {
                Id = l.Id,
                OwnerId = l.OwnerId,
                ClinicId = l.ClinicId,
                LinkedDate = l.LinkedDate,
                IsActive = l.IsActive,
                IsPrimaryOwner = l.IsPrimaryOwner,
                Role = l.Role,
                OwnershipPercentage = l.OwnershipPercentage,
                InactivatedDate = l.InactivatedDate,
                InactivationReason = l.InactivationReason,
                ClinicName = l.Clinic?.Name,
                ClinicCNPJ = l.Clinic?.Document,
                OwnerFullName = l.Owner?.FullName,
                OwnerEmail = l.Owner?.Email
            });

            return Ok(dtos);
        }

        /// <summary>
        /// Get the primary owner of a clinic
        /// </summary>
        [HttpGet("clinic/{clinicId}/primary-owner")]
        [ProducesResponseType(typeof(OwnerClinicLinkDto), 200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPrimaryOwner(Guid clinicId)
        {
            var link = await _linkService.GetPrimaryOwnerAsync(clinicId);
            if (link == null)
                return NotFound("No primary owner found for this clinic");

            var dto = new OwnerClinicLinkDto
            {
                Id = link.Id,
                OwnerId = link.OwnerId,
                ClinicId = link.ClinicId,
                LinkedDate = link.LinkedDate,
                IsActive = link.IsActive,
                IsPrimaryOwner = link.IsPrimaryOwner,
                Role = link.Role,
                OwnershipPercentage = link.OwnershipPercentage,
                OwnerFullName = link.Owner?.FullName,
                OwnerEmail = link.Owner?.Email
            };

            return Ok(dto);
        }

        /// <summary>
        /// Link an owner to a clinic
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(OwnerClinicLinkDto), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> CreateLink([FromBody] CreateOwnerClinicLinkDto dto)
        {
            try
            {
                var tenantId = GetTenantId();
                var link = await _linkService.LinkOwnerToClinicAsync(
                    dto.OwnerId, 
                    dto.ClinicId, 
                    tenantId, 
                    dto.IsPrimaryOwner, 
                    dto.Role, 
                    dto.OwnershipPercentage);

                var resultDto = new OwnerClinicLinkDto
                {
                    Id = link.Id,
                    OwnerId = link.OwnerId,
                    ClinicId = link.ClinicId,
                    LinkedDate = link.LinkedDate,
                    IsActive = link.IsActive,
                    IsPrimaryOwner = link.IsPrimaryOwner,
                    Role = link.Role,
                    OwnershipPercentage = link.OwnershipPercentage
                };

                return CreatedAtAction(nameof(GetClinicOwners), new { clinicId = link.ClinicId }, resultDto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update an owner-clinic link
        /// </summary>
        [HttpPut("{linkId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateLink(Guid linkId, [FromBody] UpdateOwnerClinicLinkDto dto)
        {
            try
            {
                var tenantId = GetTenantId();
                await _linkService.UpdateLinkAsync(linkId, tenantId, dto.Role, dto.OwnershipPercentage);
                return Ok(new { message = "Link updated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Transfer primary ownership to another owner
        /// </summary>
        [HttpPost("clinic/{clinicId}/transfer-ownership")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> TransferOwnership(Guid clinicId, [FromBody] TransferPrimaryOwnershipDto dto)
        {
            try
            {
                // Get current owner from token
                var ownerIdClaim = User.Claims.FirstOrDefault(c => c.Type == "owner_id");
                if (ownerIdClaim == null || !Guid.TryParse(ownerIdClaim.Value, out var currentOwnerId))
                    return Unauthorized("Owner ID not found in token");

                // Verify current owner has access to clinic
                if (!await _linkService.HasAccessToClinicAsync(currentOwnerId, clinicId))
                    return Forbid("You don't have access to this clinic");

                await _linkService.TransferPrimaryOwnershipAsync(clinicId, currentOwnerId, dto.NewPrimaryOwnerId);
                return Ok(new { message = "Ownership transferred successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deactivate an owner-clinic link
        /// </summary>
        [HttpDelete("{linkId}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeactivateLink(Guid linkId, [FromBody] DeactivateLinkDto dto)
        {
            try
            {
                var tenantId = GetTenantId();
                await _linkService.DeactivateLinkAsync(linkId, tenantId, dto.Reason);
                return Ok(new { message = "Link deactivated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Reactivate an owner-clinic link
        /// </summary>
        [HttpPost("{linkId}/reactivate")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> ReactivateLink(Guid linkId)
        {
            try
            {
                var tenantId = GetTenantId();
                await _linkService.ReactivateLinkAsync(linkId, tenantId);
                return Ok(new { message = "Link reactivated successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Check if an owner has access to a clinic
        /// </summary>
        [HttpGet("check-access")]
        [ProducesResponseType(typeof(bool), 200)]
        public async Task<IActionResult> CheckAccess([FromQuery] Guid ownerId, [FromQuery] Guid clinicId)
        {
            var hasAccess = await _linkService.HasAccessToClinicAsync(ownerId, clinicId);
            return Ok(new { hasAccess });
        }
    }
}
