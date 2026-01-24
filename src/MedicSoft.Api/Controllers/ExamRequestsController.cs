using Microsoft.AspNetCore.Mvc;
using MedicSoft.Application.DTOs;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using AutoMapper;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/exam-requests")]
    public class ExamRequestsController : BaseController
    {
        private readonly IExamRequestRepository _examRequestRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMapper _mapper;

        public ExamRequestsController(
            IExamRequestRepository examRequestRepository,
            IPatientRepository patientRepository,
            IAppointmentRepository appointmentRepository,
            IMapper mapper,
            ITenantContext tenantContext) 
            : base(tenantContext)
        {
            _examRequestRepository = examRequestRepository;
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Test endpoint to verify controller is accessible
        /// </summary>
        [HttpGet("test")]
        public ActionResult<string> Test()
        {
            return Ok("ExamRequestsController is working");
        }

        /// <summary>
        /// Create a new exam request for an appointment
        /// </summary>
        [HttpPost]
        [Route("")]
        public async Task<ActionResult<ExamRequestDto>> Create([FromBody] CreateExamRequestDto createDto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var tenantId = GetTenantId();

                // Validate patient exists
                var patient = await _patientRepository.GetByIdAsync(createDto.PatientId, tenantId);
                if (patient == null)
                {
                    return BadRequest("Patient not found");
                }

                // Validate appointment exists
                var appointment = await _appointmentRepository.GetByIdAsync(createDto.AppointmentId, tenantId);
                if (appointment == null)
                {
                    return BadRequest("Appointment not found");
                }

                var examRequest = new ExamRequest(
                    createDto.AppointmentId,
                    createDto.PatientId,
                    createDto.ExamType,
                    createDto.ExamName,
                    createDto.Description,
                    createDto.Urgency,
                    tenantId,
                    createDto.Notes
                );

                await _examRequestRepository.AddAsync(examRequest);

                var dto = _mapper.Map<ExamRequestDto>(examRequest);
                dto.PatientName = patient.Name;

                return CreatedAtAction(nameof(GetById), new { id = examRequest.Id }, dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Update an exam request
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<ExamRequestDto>> Update(Guid id, [FromBody] UpdateExamRequestDto updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var tenantId = GetTenantId();
                var examRequest = await _examRequestRepository.GetByIdAsync(id, tenantId);

                if (examRequest == null)
                    return NotFound($"Exam request with ID {id} not found");

                // Update basic information if provided
                var examName = updateDto.ExamName ?? examRequest.ExamName;
                var description = updateDto.Description ?? examRequest.Description;
                var urgency = updateDto.Urgency ?? examRequest.Urgency;
                
                examRequest.Update(examName, description, urgency, updateDto.Notes);

                // Update scheduled date if provided
                if (updateDto.ScheduledDate.HasValue)
                {
                    examRequest.Schedule(updateDto.ScheduledDate.Value);
                }

                await _examRequestRepository.UpdateAsync(examRequest);

                var dto = _mapper.Map<ExamRequestDto>(examRequest);
                return Ok(dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Complete an exam request with results
        /// </summary>
        [HttpPost("{id}/complete")]
        public async Task<ActionResult<ExamRequestDto>> Complete(Guid id, [FromBody] CompleteExamRequestDto completeDto)
        {
            if (!ModelState.IsValid)
                return BadRequestInvalidModel();

            try
            {
                var tenantId = GetTenantId();
                var examRequest = await _examRequestRepository.GetByIdAsync(id, tenantId);

                if (examRequest == null)
                    return NotFound($"Exam request with ID {id} not found");

                examRequest.Complete(completeDto.Results);
                await _examRequestRepository.UpdateAsync(examRequest);

                var dto = _mapper.Map<ExamRequestDto>(examRequest);
                return Ok(dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cancel an exam request
        /// </summary>
        [HttpPost("{id}/cancel")]
        public async Task<ActionResult> Cancel(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var examRequest = await _examRequestRepository.GetByIdAsync(id, tenantId);

                if (examRequest == null)
                    return NotFound($"Exam request with ID {id} not found");

                examRequest.Cancel();
                await _examRequestRepository.UpdateAsync(examRequest);

                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all pending exam requests
        /// </summary>
        [HttpGet("pending")]
        public async Task<ActionResult<IEnumerable<ExamRequestDto>>> GetPending()
        {
            try
            {
                var tenantId = GetTenantId();
                var examRequests = await _examRequestRepository.GetPendingExamsAsync(tenantId);

                var dtos = _mapper.Map<IEnumerable<ExamRequestDto>>(examRequests);
                return Ok(dtos);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all urgent exam requests
        /// </summary>
        [HttpGet("urgent")]
        public async Task<ActionResult<IEnumerable<ExamRequestDto>>> GetUrgent()
        {
            try
            {
                var tenantId = GetTenantId();
                var examRequests = await _examRequestRepository.GetUrgentExamsAsync(tenantId);

                var dtos = _mapper.Map<IEnumerable<ExamRequestDto>>(examRequests);
                return Ok(dtos);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all exam requests for an appointment
        /// </summary>
        [HttpGet("appointment/{appointmentId}")]
        public async Task<ActionResult<IEnumerable<ExamRequestDto>>> GetByAppointment(Guid appointmentId)
        {
            try
            {
                var tenantId = GetTenantId();
                var examRequests = await _examRequestRepository.GetByAppointmentIdAsync(appointmentId, tenantId);

                var dtos = _mapper.Map<IEnumerable<ExamRequestDto>>(examRequests);
                return Ok(dtos);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get all exam requests for a patient
        /// </summary>
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<ExamRequestDto>>> GetByPatient(Guid patientId)
        {
            try
            {
                var tenantId = GetTenantId();
                var examRequests = await _examRequestRepository.GetByPatientIdAsync(patientId, tenantId);

                var dtos = _mapper.Map<IEnumerable<ExamRequestDto>>(examRequests);
                return Ok(dtos);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get exam request by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ExamRequestDto>> GetById(Guid id)
        {
            try
            {
                var tenantId = GetTenantId();
                var examRequest = await _examRequestRepository.GetByIdAsync(id, tenantId);

                if (examRequest == null)
                    return NotFound($"Exam request with ID {id} not found");

                var dto = _mapper.Map<ExamRequestDto>(examRequest);
                return Ok(dto);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
