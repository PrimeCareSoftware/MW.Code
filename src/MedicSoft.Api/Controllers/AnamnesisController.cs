using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.Commands.Anamnesis;
using MedicSoft.Application.DTOs.Anamnesis;
using MedicSoft.Application.Queries.Anamnesis;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/anamnesis")]
    [Authorize]
    public class AnamnesisController : BaseController
    {
        private readonly IMediator _mediator;

        public AnamnesisController(IMediator mediator, ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _mediator = mediator;
        }

        #region Templates

        /// <summary>
        /// Get templates by medical specialty
        /// </summary>
        [HttpGet("templates")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsView)]
        public async Task<ActionResult<List<AnamnesisTemplateDto>>> GetTemplatesBySpecialty([FromQuery] MedicalSpecialty specialty)
        {
            try
            {
                var query = new GetTemplatesBySpecialtyQuery(specialty, GetTenantId());
                var templates = await _mediator.Send(query);
                return Ok(templates);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get template by ID
        /// </summary>
        [HttpGet("templates/{id}")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsView)]
        public async Task<ActionResult<AnamnesisTemplateDto>> GetTemplateById(Guid id)
        {
            try
            {
                var query = new GetTemplateByIdQuery(id, GetTenantId());
                var template = await _mediator.Send(query);
                
                if (template == null)
                    return NotFound($"Template com ID {id} não encontrado");
                
                return Ok(template);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Create a new anamnesis template
        /// </summary>
        [HttpPost("templates")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsEdit)]
        public async Task<ActionResult<AnamnesisTemplateDto>> CreateTemplate([FromBody] CreateAnamnesisTemplateDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var command = new CreateAnamnesisTemplateCommand(createDto, GetTenantId(), GetUserId());
                var template = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetTemplateById), new { id = template.Id }, template);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing anamnesis template
        /// </summary>
        [HttpPut("templates/{id}")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsEdit)]
        public async Task<ActionResult<AnamnesisTemplateDto>> UpdateTemplate(Guid id, [FromBody] UpdateAnamnesisTemplateDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var command = new UpdateAnamnesisTemplateCommand(id, updateDto, GetTenantId());
                var template = await _mediator.Send(command);
                return Ok(template);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region Responses

        /// <summary>
        /// Create a new anamnesis response for an appointment
        /// </summary>
        [HttpPost("responses")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsCreate)]
        public async Task<ActionResult<AnamnesisResponseDto>> CreateResponse([FromBody] CreateAnamnesisResponseDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                // Get patient and doctor from appointment
                // For now, we'll use the current user as doctor and get patient from DTO
                // In a real scenario, you'd fetch these from the appointment
                var command = new CreateAnamnesisResponseCommand(
                    createDto.AppointmentId,
                    createDto.TemplateId,
                    Guid.Empty, // PatientId - should be fetched from appointment
                    GetUserId(), // DoctorId - current user
                    GetTenantId()
                );
                var response = await _mediator.Send(command);
                return CreatedAtAction(nameof(GetResponseById), new { id = response.Id }, response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Save answers to an anamnesis response
        /// </summary>
        [HttpPut("responses/{id}/answers")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsEdit)]
        public async Task<ActionResult<AnamnesisResponseDto>> SaveAnswers(Guid id, [FromBody] SaveAnswersDto saveDto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var command = new SaveAnamnesisAnswersCommand(id, saveDto, GetTenantId());
                var response = await _mediator.Send(command);
                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get anamnesis response by ID
        /// </summary>
        [HttpGet("responses/{id}")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsView)]
        public async Task<ActionResult<AnamnesisResponseDto>> GetResponseById(Guid id)
        {
            try
            {
                var query = new GetResponseByIdQuery(id, GetTenantId());
                var response = await _mediator.Send(query);
                
                if (response == null)
                    return NotFound($"Resposta com ID {id} não encontrada");
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get anamnesis response by appointment ID
        /// </summary>
        [HttpGet("responses/by-appointment/{appointmentId}")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsView)]
        public async Task<ActionResult<AnamnesisResponseDto>> GetResponseByAppointment(Guid appointmentId)
        {
            try
            {
                var query = new GetResponseByAppointmentQuery(appointmentId, GetTenantId());
                var response = await _mediator.Send(query);
                
                if (response == null)
                    return NotFound($"Nenhuma anamnese encontrada para o atendimento {appointmentId}");
                
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get patient's anamnesis history
        /// </summary>
        [HttpGet("responses/patient/{patientId}")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsView)]
        public async Task<ActionResult<List<AnamnesisResponseDto>>> GetPatientHistory(Guid patientId)
        {
            try
            {
                var query = new GetPatientAnamnesisHistoryQuery(patientId, GetTenantId());
                var responses = await _mediator.Send(query);
                return Ok(responses);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        #endregion
    }
}
