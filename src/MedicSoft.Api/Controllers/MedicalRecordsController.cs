using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Api.Controllers
{
    public class MedicalRecordsController : BaseController
    {
        private readonly IMedicalRecordService _medicalRecordService;

        public MedicalRecordsController(IMedicalRecordService medicalRecordService, ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _medicalRecordService = medicalRecordService;
        }

        /// <summary>
        /// Create a new medical record for an appointment
        /// Requires ManageMedicalRecords permission (Doctor, Dentist, Nurse have this)
        /// </summary>
        [HttpPost]
        [RequirePermission(Permission.ManageMedicalRecords)]
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
        /// Update a medical record
        /// Requires ManageMedicalRecords permission (Secretary does NOT have this)
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermission(Permission.ManageMedicalRecords)]
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
        /// Complete a medical record and finish consultation
        /// </summary>
        [HttpPost("{id}/complete")]
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
        /// Get medical record by appointment ID
        /// </summary>
        [HttpGet("appointment/{appointmentId}")]
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
        /// Get all medical records for a patient
        /// </summary>
        [HttpGet("patient/{patientId}")]
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
