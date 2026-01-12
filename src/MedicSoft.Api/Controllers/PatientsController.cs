using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.Application.Queries.Patients;
using MedicSoft.CrossCutting.Authorization;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Common;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PatientsController : BaseController
    {
        private readonly IPatientService _patientService;
        private readonly IMediator _mediator;

        public PatientsController(
            IPatientService patientService, 
            IMediator mediator,
            ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _patientService = patientService;
            _mediator = mediator;
        }

        /// <summary>
        /// Get all patients (requires patients.view permission)
        /// </summary>
        [HttpGet]
        [RequirePermissionKey(PermissionKeys.PatientsView)]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetAll()
        {
            // Get clinicId from JWT token
            var clinicId = GetClinicId();
            
            // If user has a clinic ID, filter patients by clinic
            // Otherwise, return all patients in the tenant (for system admins)
            IEnumerable<PatientDto> patients;
            if (clinicId.HasValue)
            {
                patients = await _patientService.GetPatientsByClinicIdAsync(clinicId.Value, GetTenantId());
            }
            else
            {
                patients = await _patientService.GetAllPatientsAsync(GetTenantId());
            }
            
            return Ok(patients);
        }

        /// <summary>
        /// Search patients by CPF, Name or Phone (requires patients.view permission)
        /// </summary>
        [HttpGet("search")]
        [RequirePermissionKey(PermissionKeys.PatientsView)]
        public async Task<ActionResult<IEnumerable<PatientDto>>> Search([FromQuery] string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return BadRequest("Search term cannot be empty");

            if (searchTerm.Length < 3)
                return BadRequest("Search term must be at least 3 characters long");

            var patients = await _patientService.SearchPatientsAsync(searchTerm, GetTenantId());
            return Ok(patients);
        }

        /// <summary>
        /// Get patient by document (CPF) across all clinics (requires patients.view permission)
        /// </summary>
        [HttpGet("by-document/{document}")]
        [RequirePermissionKey(PermissionKeys.PatientsView)]
        public async Task<ActionResult<PatientDto>> GetByDocument(string document)
        {
            var patient = await _patientService.GetPatientByDocumentGlobalAsync(document);
            
            if (patient == null)
                return NotFound($"Patient with document {document} not found");

            return Ok(patient);
        }

        /// <summary>
        /// Get patient by ID (requires patients.view permission)
        /// </summary>
        [HttpGet("{id}")]
        [RequirePermissionKey(PermissionKeys.PatientsView)]
        public async Task<ActionResult<PatientDto>> GetById(Guid id)
        {
            var patient = await _patientService.GetPatientByIdAsync(id, GetTenantId());
            
            if (patient == null)
                return NotFound($"Patient with ID {id} not found");

            return Ok(patient);
        }

        /// <summary>
        /// Create a new patient (requires patients.create permission)
        /// </summary>
        [HttpPost]
        [RequirePermissionKey(PermissionKeys.PatientsCreate)]
        public async Task<ActionResult<PatientDto>> Create([FromBody] CreatePatientDto createPatientDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var patient = await _patientService.CreatePatientAsync(createPatientDto, GetTenantId());
                return CreatedAtAction(nameof(GetById), new { id = patient.Id }, patient);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update an existing patient (requires patients.edit permission)
        /// </summary>
        [HttpPut("{id}")]
        [RequirePermissionKey(PermissionKeys.PatientsEdit)]
        public async Task<ActionResult<PatientDto>> Update(Guid id, [FromBody] UpdatePatientDto updatePatientDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var patient = await _patientService.UpdatePatientAsync(id, updatePatientDto, GetTenantId());
                return Ok(patient);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Delete a patient (requires patients.delete permission)
        /// </summary>
        [HttpDelete("{id}")]
        [RequirePermissionKey(PermissionKeys.PatientsDelete)]
        public async Task<ActionResult> Delete(Guid id)
        {
            try
            {
                var result = await _patientService.DeletePatientAsync(id, GetTenantId());
                
                if (!result)
                    return NotFound($"Patient with ID {id} not found");

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Link patient to a clinic (requires patients.edit permission)
        /// </summary>
        [HttpPost("{patientId}/link-clinic/{clinicId}")]
        [RequirePermissionKey(PermissionKeys.PatientsEdit)]
        public async Task<ActionResult> LinkToClinic(Guid patientId, Guid clinicId)
        {
            try
            {
                var result = await _patientService.LinkPatientToClinicAsync(patientId, clinicId, GetTenantId());
                return Ok(new { success = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Link child to guardian (requires patients.edit permission)
        /// </summary>
        [HttpPost("{childId}/link-guardian/{guardianId}")]
        [RequirePermissionKey(PermissionKeys.PatientsEdit)]
        public async Task<ActionResult> LinkChildToGuardian(Guid childId, Guid guardianId)
        {
            try
            {
                var result = await _patientService.LinkChildToGuardianAsync(childId, guardianId, GetTenantId());
                return Ok(new { success = result });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get children of a guardian (requires patients.view permission)
        /// </summary>
        [HttpGet("{guardianId}/children")]
        [RequirePermissionKey(PermissionKeys.PatientsView)]
        public async Task<ActionResult<IEnumerable<PatientDto>>> GetChildren(Guid guardianId)
        {
            var children = await _patientService.GetChildrenOfGuardianAsync(guardianId, GetTenantId());
            return Ok(children);
        }

        /// <summary>
        /// Get appointment history for a patient (requires patients.view permission)
        /// Includes payment information for all appointments
        /// Medical records are only included if user has 'medical-records.view' permission
        /// </summary>
        [HttpGet("{patientId}/appointment-history")]
        [RequirePermissionKey(PermissionKeys.PatientsView)]
        public async Task<ActionResult<PatientCompleteHistoryDto>> GetAppointmentHistory(
            Guid patientId,
            [FromQuery] bool includeMedicalRecords = false)
        {
            try
            {
                // Check if user has permission to view medical records when requested
                if (includeMedicalRecords)
                {
                    var userId = GetUserId();
                    var userRepository = HttpContext.RequestServices.GetService<MedicSoft.Domain.Interfaces.IUserRepository>();
                    if (userRepository != null)
                    {
                        var user = await userRepository.GetByIdAsync(userId, GetTenantId());
                        if (user != null && !user.HasPermissionKey(PermissionKeys.MedicalRecordsView))
                        {
                            includeMedicalRecords = false; // Silently exclude medical records if no permission
                        }
                    }
                }
                
                var query = new GetPatientAppointmentHistoryQuery(
                    patientId, 
                    GetTenantId(), 
                    includeMedicalRecords);
                
                var history = await _mediator.Send(query);
                return Ok(history);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Get procedure history for a patient (requires patients.view permission)
        /// Includes payment information for all procedures
        /// </summary>
        [HttpGet("{patientId}/procedure-history")]
        [RequirePermissionKey(PermissionKeys.PatientsView)]
        public async Task<ActionResult<IEnumerable<PatientProcedureHistoryDto>>> GetProcedureHistory(Guid patientId)
        {
            try
            {
                var query = new GetPatientProcedureHistoryQuery(patientId, GetTenantId());
                var history = await _mediator.Send(query);
                return Ok(history);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}