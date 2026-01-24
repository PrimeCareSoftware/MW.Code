using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing Diagnostic Hypotheses with ICD-10 codes (CFM 1.821)
    /// </summary>
    [ApiController]
    [Route("api/diagnostic-hypotheses")]
    public class DiagnosticHypothesesController : BaseController
    {
        private readonly IDiagnosticHypothesisService _diagnosticHypothesisService;

        public DiagnosticHypothesesController(
            IDiagnosticHypothesisService diagnosticHypothesisService, 
            ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _diagnosticHypothesisService = diagnosticHypothesisService;
        }

        /// <summary>
        /// Create a new diagnostic hypothesis for a medical record
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DiagnosticHypothesisDto>> Create([FromBody] CreateDiagnosticHypothesisDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var hypothesis = await _diagnosticHypothesisService.CreateDiagnosticHypothesisAsync(createDto, GetTenantId());
                return CreatedAtAction(nameof(GetByMedicalRecord), 
                    new { medicalRecordId = hypothesis.MedicalRecordId }, 
                    hypothesis);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing diagnostic hypothesis
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<DiagnosticHypothesisDto>> Update(Guid id, [FromBody] UpdateDiagnosticHypothesisDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var hypothesis = await _diagnosticHypothesisService.UpdateDiagnosticHypothesisAsync(id, updateDto, GetTenantId());
                return Ok(hypothesis);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete a diagnostic hypothesis
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                await _diagnosticHypothesisService.DeleteDiagnosticHypothesisAsync(id, GetTenantId());
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Get all diagnostic hypotheses for a medical record
        /// </summary>
        [HttpGet("medical-record/{medicalRecordId}")]
        public async Task<ActionResult<IEnumerable<DiagnosticHypothesisDto>>> GetByMedicalRecord(Guid medicalRecordId)
        {
            try
            {
                var hypotheses = await _diagnosticHypothesisService.GetByMedicalRecordIdAsync(medicalRecordId, GetTenantId());
                return Ok(hypotheses);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
