using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.Commands.GlobalDocumentTemplates;
using MedicSoft.Application.DTOs.GlobalDocumentTemplates;
using MedicSoft.Application.Queries.GlobalDocumentTemplates;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Api.Controllers.SystemAdmin
{
    /// <summary>
    /// Controller for managing global document templates
    /// System admins can create templates that will be available for all clinics
    /// </summary>
    [ApiController]
    [Route("api/system-admin/global-templates")]
    [Authorize(Roles = "SystemAdmin")]
    public class GlobalDocumentTemplateController : BaseController
    {
        private readonly IMediator _mediator;

        public GlobalDocumentTemplateController(IMediator mediator, ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Get all global document templates
        /// </summary>
        /// <param name="type">Filter by document type (optional)</param>
        /// <param name="specialty">Filter by professional specialty (optional)</param>
        /// <param name="isActive">Filter by active status (optional)</param>
        /// <param name="searchTerm">Search in name and description (optional)</param>
        /// <returns>List of global document templates</returns>
        [HttpGet]
        [ProducesResponseType(typeof(List<GlobalDocumentTemplateDto>), 200)]
        public async Task<ActionResult<List<GlobalDocumentTemplateDto>>> GetAll(
            [FromQuery] DocumentTemplateType? type = null,
            [FromQuery] ProfessionalSpecialty? specialty = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] string? searchTerm = null)
        {
            var filter = new GlobalDocumentTemplateFilterDto
            {
                Type = type,
                Specialty = specialty,
                IsActive = isActive,
                SearchTerm = searchTerm
            };

            var query = new GetAllGlobalTemplatesQuery(GetTenantId(), filter);
            var templates = await _mediator.Send(query);
            return Ok(templates);
        }

        /// <summary>
        /// Get a specific global template by ID
        /// </summary>
        /// <param name="id">Template ID</param>
        /// <returns>Global document template details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(GlobalDocumentTemplateDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<GlobalDocumentTemplateDto>> GetById(Guid id)
        {
            var query = new GetGlobalTemplateByIdQuery(id, GetTenantId());
            var template = await _mediator.Send(query);
            
            if (template == null)
            {
                return NotFound(new { message = $"Template global com ID {id} não encontrado" });
            }
            
            return Ok(template);
        }

        /// <summary>
        /// Get global templates by document type
        /// </summary>
        /// <param name="type">Document template type</param>
        /// <returns>List of global templates for the specified type</returns>
        [HttpGet("type/{type}")]
        [ProducesResponseType(typeof(List<GlobalDocumentTemplateDto>), 200)]
        public async Task<ActionResult<List<GlobalDocumentTemplateDto>>> GetByType(DocumentTemplateType type)
        {
            var query = new GetGlobalTemplatesByTypeQuery(type, GetTenantId());
            var templates = await _mediator.Send(query);
            return Ok(templates);
        }

        /// <summary>
        /// Get global templates by professional specialty
        /// </summary>
        /// <param name="specialty">Professional specialty</param>
        /// <returns>List of global templates for the specified specialty</returns>
        [HttpGet("specialty/{specialty}")]
        [ProducesResponseType(typeof(List<GlobalDocumentTemplateDto>), 200)]
        public async Task<ActionResult<List<GlobalDocumentTemplateDto>>> GetBySpecialty(ProfessionalSpecialty specialty)
        {
            var query = new GetGlobalTemplatesBySpecialtyQuery(specialty, GetTenantId());
            var templates = await _mediator.Send(query);
            return Ok(templates);
        }

        /// <summary>
        /// Create a new global document template
        /// </summary>
        /// <param name="dto">Template creation data</param>
        /// <returns>Created global template</returns>
        [HttpPost]
        [ProducesResponseType(typeof(GlobalDocumentTemplateDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<GlobalDocumentTemplateDto>> Create([FromBody] CreateGlobalTemplateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestInvalidModel();
            }

            try
            {
                var createdBy = GetUserId();
                var command = new CreateGlobalTemplateCommand(dto, GetTenantId(), createdBy);
                var result = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing global document template
        /// </summary>
        /// <param name="id">Template ID</param>
        /// <param name="dto">Template update data</param>
        /// <returns>Updated global template</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(GlobalDocumentTemplateDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<GlobalDocumentTemplateDto>> Update(Guid id, [FromBody] UpdateGlobalTemplateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestInvalidModel();
            }

            try
            {
                var command = new UpdateGlobalTemplateCommand(id, dto, GetTenantId());
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a global document template
        /// </summary>
        /// <param name="id">Template ID</param>
        /// <returns>No content on success</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var command = new DeleteGlobalTemplateCommand(id, GetTenantId());
                await _mediator.Send(command);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Set active status of a global template
        /// </summary>
        /// <param name="id">Template ID</param>
        /// <param name="isActive">Active status</param>
        /// <returns>No content on success</returns>
        [HttpPatch("{id}/active")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> SetActiveStatus(Guid id, [FromBody] bool isActive)
        {
            try
            {
                var command = new SetGlobalTemplateActiveStatusCommand(id, isActive, GetTenantId());
                await _mediator.Send(command);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
