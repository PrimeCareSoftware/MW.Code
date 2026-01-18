using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing patient health insurance links
    /// </summary>
    [ApiController]
    [Route("api/patient-health-insurance")]
    [Authorize]
    public class PatientHealthInsuranceController : BaseController
    {
        private readonly IPatientHealthInsuranceService _patientHealthInsuranceService;

        public PatientHealthInsuranceController(
            IPatientHealthInsuranceService patientHealthInsuranceService,
            ITenantContext tenantContext) : base(tenantContext)
        {
            _patientHealthInsuranceService = patientHealthInsuranceService;
        }

        /// <summary>
        /// Get patient health insurance by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.PatientsView)]
        public async Task<ActionResult<PatientHealthInsuranceDto>> GetById(Guid id)
        {
            var insurance = await _patientHealthInsuranceService.GetByIdAsync(id, GetTenantId());
            if (insurance == null)
                return NotFound(new { message = $"Convênio do paciente não encontrado." });

            return Ok(insurance);
        }

        /// <summary>
        /// Get all health insurance plans for a patient
        /// </summary>
        [HttpGet("by-patient/{patientId}")]
        [RequirePermissionKey(PermissionKeys.PatientsView)]
        public async Task<ActionResult<IEnumerable<PatientHealthInsuranceDto>>> GetByPatientId(Guid patientId)
        {
            var insurances = await _patientHealthInsuranceService.GetByPatientIdAsync(patientId, GetTenantId());
            return Ok(insurances);
        }

        /// <summary>
        /// Get active health insurance plans for a patient
        /// </summary>
        [HttpGet("by-patient/{patientId}/active")]
        [RequirePermissionKey(PermissionKeys.PatientsView)]
        public async Task<ActionResult<IEnumerable<PatientHealthInsuranceDto>>> GetActiveByPatientId(Guid patientId)
        {
            var insurances = await _patientHealthInsuranceService.GetActiveByPatientIdAsync(patientId, GetTenantId());
            return Ok(insurances);
        }

        /// <summary>
        /// Get patient health insurance by card number
        /// </summary>
        [HttpGet("by-card/{cardNumber}")]
        [RequirePermissionKey(PermissionKeys.PatientsView)]
        public async Task<ActionResult<PatientHealthInsuranceDto>> GetByCardNumber(string cardNumber)
        {
            var insurance = await _patientHealthInsuranceService.GetByCardNumberAsync(cardNumber, GetTenantId());
            if (insurance == null)
                return NotFound(new { message = $"Carteirinha {cardNumber} não encontrada." });

            return Ok(insurance);
        }

        /// <summary>
        /// Link a patient to a health insurance plan
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.PatientsEdit)]
        public async Task<ActionResult<PatientHealthInsuranceDto>> Create([FromBody] CreatePatientHealthInsuranceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var insurance = await _patientHealthInsuranceService.CreateAsync(dto, GetTenantId());
                return CreatedAtAction(nameof(GetById), new { id = insurance.Id }, insurance);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update patient health insurance information
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermissionKey(PermissionKeys.PatientsEdit)]
        public async Task<ActionResult<PatientHealthInsuranceDto>> Update(Guid id, [FromBody] UpdatePatientHealthInsuranceDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var insurance = await _patientHealthInsuranceService.UpdateAsync(id, dto, GetTenantId());
                return Ok(insurance);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Validate a health insurance card
        /// </summary>
        [HttpPost("validate-card")]
        [RequirePermissionKey(PermissionKeys.PatientsView)]
        public async Task<ActionResult<CardValidationResultDto>> ValidateCard([FromBody] ValidateCardRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.CardNumber))
                return BadRequest(new { message = "Número da carteirinha é obrigatório." });

            try
            {
                var result = await _patientHealthInsuranceService.ValidateCardAsync(request.CardNumber, GetTenantId());
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Activate a patient health insurance
        /// </summary>
        [HttpPost("{id}/activate")]
        [RequirePermissionKey(PermissionKeys.PatientsEdit)]
        public async Task<ActionResult<PatientHealthInsuranceDto>> Activate(Guid id)
        {
            try
            {
                var insurance = await _patientHealthInsuranceService.ActivateAsync(id, GetTenantId());
                return Ok(insurance);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Deactivate a patient health insurance
        /// </summary>
        [HttpPost("{id}/deactivate")]
        [RequirePermissionKey(PermissionKeys.PatientsEdit)]
        public async Task<ActionResult<PatientHealthInsuranceDto>> Deactivate(Guid id)
        {
            try
            {
                var insurance = await _patientHealthInsuranceService.DeactivateAsync(id, GetTenantId());
                return Ok(insurance);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a patient health insurance (soft delete)
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermissionKey(PermissionKeys.PatientsEdit)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _patientHealthInsuranceService.DeleteAsync(id, GetTenantId());
                if (!result)
                    return NotFound(new { message = "Convênio do paciente não encontrado." });

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }

    public class ValidateCardRequest
    {
        public string CardNumber { get; set; } = string.Empty;
    }
}
