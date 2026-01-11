using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing Clinical Examinations (CFM 1.821)
    /// </summary>
    [ApiController]
    [Route("api/clinical-examinations")]
    public class ClinicalExaminationsController : BaseController
    {
        private readonly IClinicalExaminationService _clinicalExaminationService;

        public ClinicalExaminationsController(
            IClinicalExaminationService clinicalExaminationService, 
            ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _clinicalExaminationService = clinicalExaminationService;
        }

        /// <summary>
        /// Create a new clinical examination for a medical record
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ClinicalExaminationDto>> Create([FromBody] CreateClinicalExaminationDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var examination = await _clinicalExaminationService.CreateClinicalExaminationAsync(createDto, GetTenantId());
                return CreatedAtAction(nameof(GetByMedicalRecord), 
                    new { medicalRecordId = examination.MedicalRecordId }, 
                    examination);
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
        /// Update an existing clinical examination
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ClinicalExaminationDto>> Update(Guid id, [FromBody] UpdateClinicalExaminationDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var examination = await _clinicalExaminationService.UpdateClinicalExaminationAsync(id, updateDto, GetTenantId());
                return Ok(examination);
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
        /// Get all clinical examinations for a medical record
        /// </summary>
        [HttpGet("medical-record/{medicalRecordId}")]
        public async Task<ActionResult<IEnumerable<ClinicalExaminationDto>>> GetByMedicalRecord(Guid medicalRecordId)
        {
            try
            {
                var examinations = await _clinicalExaminationService.GetByMedicalRecordIdAsync(medicalRecordId, GetTenantId());
                return Ok(examinations);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
