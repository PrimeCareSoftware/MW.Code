using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs.SoapRecords;
using MedicSoft.Application.Services;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/soap-records")]
    [Authorize]
    public class SoapRecordsController : BaseController
    {
        private readonly ISoapRecordService _soapRecordService;

        public SoapRecordsController(
            ISoapRecordService soapRecordService, 
            ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _soapRecordService = soapRecordService;
        }

        /// <summary>
        /// Create a new SOAP record for an appointment
        /// </summary>
        [HttpPost("appointment/{appointmentId}")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsCreate)]
        public async Task<ActionResult<SoapRecordDto>> CreateSoapRecord(Guid appointmentId)
        {
            try
            {
                var soapRecord = await _soapRecordService.CreateSoapRecord(appointmentId, GetTenantId());
                return CreatedAtAction(nameof(GetSoapRecord), new { id = soapRecord.Id }, soapRecord);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update the Subjective section of a SOAP record
        /// </summary>
        [HttpPut("{id}/subjective")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsEdit)]
        public async Task<ActionResult<SoapRecordDto>> UpdateSubjective(
            Guid id,
            [FromBody] UpdateSubjectiveDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var soapRecord = await _soapRecordService.UpdateSubjective(id, updateDto, GetTenantId());
                return Ok(soapRecord);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update the Objective section of a SOAP record
        /// </summary>
        [HttpPut("{id}/objective")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsEdit)]
        public async Task<ActionResult<SoapRecordDto>> UpdateObjective(
            Guid id,
            [FromBody] UpdateObjectiveDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var soapRecord = await _soapRecordService.UpdateObjective(id, updateDto, GetTenantId());
                return Ok(soapRecord);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update the Assessment section of a SOAP record
        /// </summary>
        [HttpPut("{id}/assessment")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsEdit)]
        public async Task<ActionResult<SoapRecordDto>> UpdateAssessment(
            Guid id,
            [FromBody] UpdateAssessmentDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var soapRecord = await _soapRecordService.UpdateAssessment(id, updateDto, GetTenantId());
                return Ok(soapRecord);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update the Plan section of a SOAP record
        /// </summary>
        [HttpPut("{id}/plan")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsEdit)]
        public async Task<ActionResult<SoapRecordDto>> UpdatePlan(
            Guid id,
            [FromBody] UpdatePlanDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var soapRecord = await _soapRecordService.UpdatePlan(id, updateDto, GetTenantId());
                return Ok(soapRecord);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Complete and lock a SOAP record
        /// </summary>
        [HttpPost("{id}/complete")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsEdit)]
        public async Task<ActionResult<SoapRecordDto>> CompleteSoapRecord(Guid id)
        {
            try
            {
                var validation = await _soapRecordService.ValidateCompleteness(id, GetTenantId());

                if (!validation.IsValid)
                {
                    return BadRequest(new
                    {
                        message = "Prontuário SOAP incompleto",
                        missingFields = validation.MissingFields,
                        warnings = validation.Warnings
                    });
                }

                var soapRecord = await _soapRecordService.CompleteSoapRecord(id, GetTenantId());
                return Ok(soapRecord);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Unlock a completed SOAP record (admin only)
        /// </summary>
        [HttpPost("{id}/unlock")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsEdit)]
        public async Task<IActionResult> UnlockSoapRecord(Guid id)
        {
            try
            {
                await _soapRecordService.UnlockSoapRecord(id, GetTenantId());
                return Ok(new { message = "SOAP record unlocked successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Get a SOAP record by ID
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsView)]
        public async Task<ActionResult<SoapRecordDto>> GetSoapRecord(Guid id)
        {
            var soapRecord = await _soapRecordService.GetBySoapId(id, GetTenantId());
            
            if (soapRecord == null)
                return NotFound(new { message = "SOAP record not found" });

            return Ok(soapRecord);
        }

        /// <summary>
        /// Get SOAP record by appointment ID
        /// </summary>
        [HttpGet("appointment/{appointmentId}")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsView)]
        public async Task<ActionResult<SoapRecordDto>> GetByAppointment(Guid appointmentId)
        {
            var soapRecord = await _soapRecordService.GetByAppointmentId(appointmentId, GetTenantId());
            
            if (soapRecord == null)
                return NotFound(new { message = "SOAP record not found for this appointment" });

            return Ok(soapRecord);
        }

        /// <summary>
        /// Get all SOAP records for a patient
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsView)]
        public async Task<ActionResult<IEnumerable<SoapRecordDto>>> GetPatientSoapRecords(Guid patientId)
        {
            var records = await _soapRecordService.GetByPatientId(patientId, GetTenantId());
            return Ok(records);
        }

        /// <summary>
        /// Get all SOAP records created by a doctor
        /// </summary>
        [HttpGet("doctor/{doctorId}")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsView)]
        public async Task<ActionResult<IEnumerable<SoapRecordDto>>> GetDoctorSoapRecords(Guid doctorId)
        {
            var records = await _soapRecordService.GetByDoctorId(doctorId, GetTenantId());
            return Ok(records);
        }

        /// <summary>
        /// Validate completeness of a SOAP record
        /// </summary>
        [HttpGet("{id}/validate")]
        [RequirePermissionKey(PermissionKeys.MedicalRecordsView)]
        public async Task<ActionResult<SoapRecordValidationDto>> ValidateSoapRecord(Guid id)
        {
            try
            {
                var validation = await _soapRecordService.ValidateCompleteness(id, GetTenantId());
                return Ok(validation);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }
    }
}
