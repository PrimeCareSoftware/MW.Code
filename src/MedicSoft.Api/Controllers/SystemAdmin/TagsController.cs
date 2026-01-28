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
    /// Controller for managing tags and clinic categorization
    /// </summary>
    [ApiController]
    [Route("api/system-admin/tags")]
    [Authorize(Roles = "SystemAdmin")]
    public class TagsController : BaseController
    {
        private readonly ITagService _tagService;

        public TagsController(
            ITenantContext tenantContext,
            ITagService tagService) : base(tenantContext)
        {
            _tagService = tagService;
        }

        /// <summary>
        /// Get all tags
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<TagDto>>> GetAllTags()
        {
            var tags = await _tagService.GetAllTags();
            return Ok(tags);
        }

        /// <summary>
        /// Get a specific tag by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<TagDto>> GetTagById(Guid id)
        {
            var tag = await _tagService.GetTagById(id);
            
            if (tag == null)
                return NotFound(new { message = $"Tag with ID {id} not found" });

            return Ok(tag);
        }

        /// <summary>
        /// Create a new tag
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TagDto>> CreateTag([FromBody] CreateTagDto createDto)
        {
            // Use a constant for system tenant ID
            const string SystemTenantId = "system";
            
            var tag = await _tagService.CreateTag(createDto, SystemTenantId);
            return CreatedAtAction(nameof(GetTagById), new { id = tag.Id }, tag);
        }

        /// <summary>
        /// Update an existing tag
        /// </summary>
        [HttpPut("{id:guid}")]
        public async Task<ActionResult> UpdateTag(Guid id, [FromBody] UpdateTagDto updateDto)
        {
            var success = await _tagService.UpdateTag(id, updateDto);
            
            if (!success)
                return NotFound(new { message = $"Tag with ID {id} not found" });

            return Ok(new { message = "Tag updated successfully" });
        }

        /// <summary>
        /// Delete a tag
        /// </summary>
        [HttpDelete("{id:guid}")]
        public async Task<ActionResult> DeleteTag(Guid id)
        {
            var success = await _tagService.DeleteTag(id);
            
            if (!success)
                return NotFound(new { message = $"Tag with ID {id} not found" });

            return Ok(new { message = "Tag deleted successfully" });
        }

        /// <summary>
        /// Assign a tag to multiple clinics
        /// </summary>
        [HttpPost("assign")]
        public async Task<ActionResult> AssignTag([FromBody] AssignTagDto assignDto)
        {
            if (assignDto.ClinicIds == null || assignDto.ClinicIds.Count == 0)
            {
                return BadRequest(new { message = "At least one clinic ID is required" });
            }

            // Get username from claims if available
            var username = User?.Identity?.Name;

            var success = await _tagService.AssignTagsToClinics(
                assignDto.TagId, 
                assignDto.ClinicIds, 
                username);
            
            if (!success)
                return NotFound(new { message = $"Tag with ID {assignDto.TagId} not found" });

            return Ok(new { message = $"Tag assigned to {assignDto.ClinicIds.Count} clinic(s)" });
        }

        /// <summary>
        /// Remove a tag from multiple clinics
        /// </summary>
        [HttpPost("remove")]
        public async Task<ActionResult> RemoveTag([FromBody] AssignTagDto removeDto)
        {
            if (removeDto.ClinicIds == null || removeDto.ClinicIds.Count == 0)
            {
                return BadRequest(new { message = "At least one clinic ID is required" });
            }

            var success = await _tagService.RemoveTagFromClinics(
                removeDto.TagId, 
                removeDto.ClinicIds);
            
            if (!success)
                return NotFound(new { message = $"Tag with ID {removeDto.TagId} not found" });

            return Ok(new { message = $"Tag removed from {removeDto.ClinicIds.Count} clinic(s)" });
        }

        /// <summary>
        /// Get all tags for a specific clinic
        /// </summary>
        [HttpGet("clinic/{clinicId:guid}")]
        public async Task<ActionResult<List<TagDto>>> GetTagsByClinic(Guid clinicId)
        {
            var tags = await _tagService.GetTagsByClinic(clinicId);
            return Ok(tags);
        }

        /// <summary>
        /// Trigger automatic tag application
        /// </summary>
        [HttpPost("apply-automatic")]
        public async Task<ActionResult> ApplyAutomaticTags()
        {
            await _tagService.ApplyAutomaticTags();
            return Ok(new { message = "Automatic tags applied successfully" });
        }
    }
}
