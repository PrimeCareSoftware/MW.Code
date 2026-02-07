using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.SystemAdmin;
using MedicSoft.Application.Services.SystemAdmin;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers.SystemAdmin
{
    /// <summary>
    /// Controller for managing users across all tenants
    /// </summary>
    [ApiController]
    [Route("api/system-admin/cross-tenant-users")]
    [Authorize(Roles = "SystemAdmin")]
    public class CrossTenantUsersController : BaseController
    {
        private readonly ICrossTenantUserService _userService;

        public CrossTenantUsersController(
            ITenantContext tenantContext,
            ICrossTenantUserService userService) : base(tenantContext)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get users across all tenants with filtering
        /// </summary>
        [HttpPost("filter")]
        public async Task<ActionResult> GetUsers([FromBody] CrossTenantUserFilterDto filters)
        {
            var (users, totalCount) = await _userService.GetUsers(filters);
            
            return Ok(new
            {
                data = users,
                totalCount,
                page = filters.Page,
                pageSize = filters.PageSize,
                totalPages = (int)Math.Ceiling(totalCount / (double)filters.PageSize)
            });
        }

        /// <summary>
        /// Get a specific user by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<CrossTenantUserDto>> GetUserById(Guid id)
        {
            var user = await _userService.GetUserById(id);
            
            if (user == null)
                return NotFound(new { message = $"User with ID {id} not found" });

            return Ok(user);
        }

        /// <summary>
        /// Reset user password
        /// </summary>
        [HttpPost("{id:guid}/reset-password")]
        public async Task<ActionResult> ResetPassword(Guid id, [FromBody] ResetPasswordDto dto)
        {
            // Improved password validation
            if (string.IsNullOrWhiteSpace(dto.NewPassword))
            {
                return BadRequest(new { message = "Password is required" });
            }

            if (dto.NewPassword.Length < 8)
            {
                return BadRequest(new { message = "Password must be at least 8 characters long" });
            }

            var success = await _userService.ResetPassword(id, dto.NewPassword);
            
            if (!success)
                return NotFound(new { message = $"User with ID {id} not found" });

            return Ok(new { message = "Password reset successfully" });
        }

        /// <summary>
        /// Toggle user activation status
        /// </summary>
        [HttpPost("{id:guid}/toggle-status")]
        public async Task<ActionResult> ToggleStatus(Guid id)
        {
            var success = await _userService.ToggleUserActivation(id);
            
            if (!success)
                return NotFound(new { message = $"User with ID {id} not found" });

            return Ok(new { message = "User activation toggled successfully" });
        }

        /// <summary>
        /// Transfer ownership from one user to another within the same clinic
        /// </summary>
        [HttpPost("transfer-ownership")]
        public async Task<ActionResult> TransferOwnership([FromBody] TransferOwnershipDto dto)
        {
            try
            {
                var success = await _userService.TransferOwnership(dto.CurrentOwnerId, dto.NewOwnerId);
                
                if (!success)
                    return NotFound(new { message = "One or both users not found" });

                return Ok(new { message = "Ownership transferred successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
