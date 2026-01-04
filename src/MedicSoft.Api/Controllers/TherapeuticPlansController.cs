using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Identity;

namespace MedicSoft.Api.Controllers
{
    /// <summary>
    /// Controller for managing Therapeutic Plans (CFM 1.821)
    /// </summary>
    public class TherapeuticPlansController : BaseController
    {
        private readonly ITherapeuticPlanService _therapeuticPlanService;

        public TherapeuticPlansController(
            ITherapeuticPlanService therapeuticPlanService, 
            ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _therapeuticPlanService = therapeuticPlanService;
        }

        /// <summary>
        /// Create a new therapeutic plan for a medical record
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<TherapeuticPlanDto>> Create([FromBody] CreateTherapeuticPlanDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var plan = await _therapeuticPlanService.CreateTherapeuticPlanAsync(createDto, GetTenantId());
                return CreatedAtAction(nameof(GetByMedicalRecord), 
                    new { medicalRecordId = plan.MedicalRecordId }, 
                    plan);
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
        /// Update an existing therapeutic plan
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<TherapeuticPlanDto>> Update(Guid id, [FromBody] UpdateTherapeuticPlanDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var plan = await _therapeuticPlanService.UpdateTherapeuticPlanAsync(id, updateDto, GetTenantId());
                return Ok(plan);
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
        /// Get all therapeutic plans for a medical record
        /// </summary>
        [HttpGet("medical-record/{medicalRecordId}")]
        public async Task<ActionResult<IEnumerable<TherapeuticPlanDto>>> GetByMedicalRecord(Guid medicalRecordId)
        {
            try
            {
                var plans = await _therapeuticPlanService.GetByMedicalRecordIdAsync(medicalRecordId, GetTenantId());
                return Ok(plans);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
