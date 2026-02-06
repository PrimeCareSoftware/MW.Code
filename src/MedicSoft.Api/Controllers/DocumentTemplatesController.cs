using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.Commands.DocumentTemplates;
using MedicSoft.Application.DTOs.DocumentTemplates;
using MedicSoft.Application.Queries.DocumentTemplates;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing document templates for printing
    /// Handles medical records, prescriptions, certificates, and other document types
    /// </summary>
    [ApiController]
    [Route("api/document-templates")]
    [Authorize]
    public class DocumentTemplatesController : BaseController
    {
        private readonly IMediator _mediator;

        public DocumentTemplatesController(IMediator mediator, ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }

        /// <summary>
        /// Get all document templates for the current tenant
        /// </summary>
        /// <param name="specialty">Filter by professional specialty (optional)</param>
        /// <param name="type">Filter by document type (optional)</param>
        /// <param name="isActive">Filter by active status (optional)</param>
        /// <param name="isSystem">Filter by system templates (optional)</param>
        /// <param name="clinicId">Filter by clinic ID (optional)</param>
        /// <returns>List of document templates</returns>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.FormConfigurationView)]
        [ProducesResponseType(typeof(List<DocumentTemplateDto>), 200)]
        public async Task<ActionResult<List<DocumentTemplateDto>>> GetAll(
            [FromQuery] ProfessionalSpecialty? specialty = null,
            [FromQuery] DocumentTemplateType? type = null,
            [FromQuery] bool? isActive = null,
            [FromQuery] bool? isSystem = null,
            [FromQuery] Guid? clinicId = null)
        {
            var filter = new DocumentTemplateFilterDto
            {
                Specialty = specialty,
                Type = type,
                IsActive = isActive,
                IsSystem = isSystem,
                ClinicId = clinicId
            };

            var query = new GetAllDocumentTemplatesQuery(GetTenantId(), filter);
            var templates = await _mediator.Send(query);
            return Ok(templates);
        }

        /// <summary>
        /// Get a specific document template by ID
        /// </summary>
        /// <param name="id">Template ID</param>
        /// <returns>Document template details</returns>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.FormConfigurationView)]
        [ProducesResponseType(typeof(DocumentTemplateDto), 200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<DocumentTemplateDto>> GetById(Guid id)
        {
            var query = new GetDocumentTemplateByIdQuery(id, GetTenantId());
            var template = await _mediator.Send(query);
            
            if (template == null)
            {
                return NotFound(new { message = $"Template com ID {id} não encontrado" });
            }
            
            return Ok(template);
        }

        /// <summary>
        /// Get templates by professional specialty
        /// </summary>
        /// <param name="specialty">Professional specialty</param>
        /// <param name="activeOnly">Return only active templates</param>
        /// <returns>List of templates for the specialty</returns>
        [HttpGet("by-specialty/{specialty}")]
        [RequirePermissionKey(PermissionKeys.FormConfigurationView)]
        [ProducesResponseType(typeof(List<DocumentTemplateDto>), 200)]
        public async Task<ActionResult<List<DocumentTemplateDto>>> GetBySpecialty(
            ProfessionalSpecialty specialty,
            [FromQuery] bool activeOnly = false)
        {
            var query = new GetTemplatesBySpecialtyQuery(specialty, GetTenantId(), activeOnly);
            var templates = await _mediator.Send(query);
            return Ok(templates);
        }

        /// <summary>
        /// Get templates by document type
        /// </summary>
        /// <param name="type">Document type</param>
        /// <returns>List of templates for the type</returns>
        [HttpGet("by-type/{type}")]
        [RequirePermissionKey(PermissionKeys.FormConfigurationView)]
        [ProducesResponseType(typeof(List<DocumentTemplateDto>), 200)]
        public async Task<ActionResult<List<DocumentTemplateDto>>> GetByType(DocumentTemplateType type)
        {
            var query = new GetTemplatesByTypeQuery(type, GetTenantId());
            var templates = await _mediator.Send(query);
            return Ok(templates);
        }

        /// <summary>
        /// Get templates by clinic
        /// </summary>
        /// <param name="clinicId">Clinic ID</param>
        /// <returns>List of templates for the clinic</returns>
        [HttpGet("by-clinic/{clinicId}")]
        [RequirePermissionKey(PermissionKeys.FormConfigurationView)]
        [ProducesResponseType(typeof(List<DocumentTemplateDto>), 200)]
        public async Task<ActionResult<List<DocumentTemplateDto>>> GetByClinic(Guid clinicId)
        {
            var query = new GetTemplatesByClinicQuery(clinicId, GetTenantId());
            var templates = await _mediator.Send(query);
            return Ok(templates);
        }

        /// <summary>
        /// Create a new document template
        /// </summary>
        /// <param name="dto">Template data</param>
        /// <returns>Created template</returns>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.FormConfigurationManage)]
        [ProducesResponseType(typeof(DocumentTemplateDto), 201)]
        [ProducesResponseType(400)]
        public async Task<ActionResult<DocumentTemplateDto>> Create([FromBody] CreateDocumentTemplateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestInvalidModel();
            }

            var command = new CreateDocumentTemplateCommand(dto, GetTenantId(), GetClinicId());
            var result = await _mediator.Send(command);
            
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update an existing document template
        /// </summary>
        /// <param name="id">Template ID</param>
        /// <param name="dto">Updated template data</param>
        /// <returns>Updated template</returns>
        [HttpPut("{id}")]
        [RequirePermissionKey(PermissionKeys.FormConfigurationManage)]
        [ProducesResponseType(typeof(DocumentTemplateDto), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<DocumentTemplateDto>> Update(Guid id, [FromBody] UpdateDocumentTemplateDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequestInvalidModel();
            }

            try
            {
                var command = new UpdateDocumentTemplateCommand(id, dto, GetTenantId());
                var result = await _mediator.Send(command);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a document template
        /// </summary>
        /// <param name="id">Template ID</param>
        /// <returns>No content on success</returns>
        [HttpDelete("{id}")]
        [RequirePermissionKey(PermissionKeys.FormConfigurationManage)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var command = new DeleteDocumentTemplateCommand(id, GetTenantId());
                await _mediator.Send(command);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Activate a document template
        /// </summary>
        /// <param name="id">Template ID</param>
        /// <returns>No content on success</returns>
        [HttpPost("{id}/activate")]
        [RequirePermissionKey(PermissionKeys.FormConfigurationManage)]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Activate(Guid id)
        {
            try
            {
                var command = new ActivateDocumentTemplateCommand(id, GetTenantId());
                await _mediator.Send(command);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate a document template
        /// </summary>
        /// <param name="id">Template ID</param>
        /// <returns>No content on success</returns>
        [HttpPost("{id}/deactivate")]
        [RequirePermissionKey(PermissionKeys.FormConfigurationManage)]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> Deactivate(Guid id)
        {
            try
            {
                var command = new DeactivateDocumentTemplateCommand(id, GetTenantId());
                await _mediator.Send(command);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
