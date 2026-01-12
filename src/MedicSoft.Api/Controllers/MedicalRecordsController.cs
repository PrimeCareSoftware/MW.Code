using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/medical-records")]
    [Authorize]
    public class MedicalRecordsController : BaseController
    {
        private readonly IMedicalRecordService _medicalRecordService;

        public MedicalRecordsController(IMedicalRecordService medicalRecordService, ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _medicalRecordService = medicalRecordService;
        }

        /// <summary>
        /// Create a new medical record for an appointment (requires medical-records.create permission)
        /// Only Doctor, Dentist, Nurse have this permission
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsCreate)]
        public async Task<ActionResult<MedicalRecordDto>> Create([FromBody] CreateMedicalRecordDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var medicalRecord = await _medicalRecordService.CreateMedicalRecordAsync(createDto, GetTenantId());
                return CreatedAtAction(nameof(GetByAppointment), 
                    new { appointmentId = medicalRecord.AppointmentId }, 
                    medicalRecord);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update a medical record (requires medical-records.edit permission)
        /// Secretary does NOT have this permission
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsEdit)]
        public async Task<ActionResult<MedicalRecordDto>> Update(Guid id, [FromBody] UpdateMedicalRecordDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var medicalRecord = await _medicalRecordService.UpdateMedicalRecordAsync(id, updateDto, GetTenantId());
                return Ok(medicalRecord);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Complete a medical record and finish consultation (requires medical-records.edit permission)
        /// </summary>
        [HttpPost("{id}/complete")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsEdit)]
        public async Task<ActionResult<MedicalRecordDto>> Complete(Guid id, [FromBody] CompleteMedicalRecordDto completeDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var medicalRecord = await _medicalRecordService.CompleteMedicalRecordAsync(id, completeDto, GetTenantId());
                return Ok(medicalRecord);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get medical record by appointment ID (requires medical-records.view permission)
        /// </summary>
        [HttpGet("appointment/{appointmentId}")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsView)]
        public async Task<ActionResult<MedicalRecordDto>> GetByAppointment(Guid appointmentId)
        {
            try
            {
                var medicalRecord = await _medicalRecordService.GetByAppointmentIdAsync(appointmentId, GetTenantId());
                
                if (medicalRecord == null)
                    return NotFound($"Medical record not found for appointment {appointmentId}");

                return Ok(medicalRecord);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all medical records for a patient (requires medical-records.view permission)
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsView)]
        public async Task<ActionResult<IEnumerable<MedicalRecordDto>>> GetByPatient(Guid patientId)
        {
            try
            {
                var medicalRecords = await _medicalRecordService.GetPatientMedicalRecordsAsync(patientId, GetTenantId());
                return Ok(medicalRecords);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
