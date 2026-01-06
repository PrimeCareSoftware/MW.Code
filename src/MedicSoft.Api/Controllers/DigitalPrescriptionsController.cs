using Microsoft.AspNetCore.Mvc;
using AutoMapper;
using MedicSoft.Application.DTOs;
using MedicSoft.CrossCutting.Identity;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DigitalPrescriptionsController : BaseController
    {
        private readonly IDigitalPrescriptionRepository _prescriptionRepository;
        private readonly IDigitalPrescriptionItemRepository _itemRepository;
        private readonly IPrescriptionSequenceControlRepository _sequenceRepository;
        private readonly IMapper _mapper;

        public DigitalPrescriptionsController(
            IDigitalPrescriptionRepository prescriptionRepository,
            IDigitalPrescriptionItemRepository itemRepository,
            IPrescriptionSequenceControlRepository sequenceRepository,
            IMapper mapper,
            ITenantContext tenantContext)
            : base(tenantContext)
        {
            _prescriptionRepository = prescriptionRepository;
            _itemRepository = itemRepository;
            _sequenceRepository = sequenceRepository;
            _mapper = mapper;
        }

        /// <summary>
        /// Create a new digital prescription
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<DigitalPrescriptionDto>> Create([FromBody] CreateDigitalPrescriptionDto dto)
        {
            try
            {
                // Parse prescription type
                if (!Enum.TryParse<PrescriptionType>(dto.Type, true, out var prescriptionType))
                    return BadRequest($"Invalid prescription type: {dto.Type}");

                // Generate sequence number for controlled prescriptions
                string? sequenceNumber = null;
                if (prescriptionType == PrescriptionType.SpecialControlA ||
                    prescriptionType == PrescriptionType.SpecialControlB ||
                    prescriptionType == PrescriptionType.SpecialControlC1)
                {
                    sequenceNumber = await _sequenceRepository.GenerateNextSequenceAsync(
                        prescriptionType, GetTenantId());
                }

                // Create prescription entity
                var prescription = new DigitalPrescription(
                    dto.MedicalRecordId,
                    dto.PatientId,
                    dto.DoctorId,
                    prescriptionType,
                    dto.DoctorName,
                    dto.DoctorCRM,
                    dto.DoctorCRMState,
                    dto.PatientName,
                    dto.PatientDocument,
                    GetTenantId(),
                    sequenceNumber,
                    dto.Notes
                );

                // Add items
                foreach (var itemDto in dto.Items)
                {
                    ControlledSubstanceList? controlledList = null;
                    if (itemDto.IsControlledSubstance && !string.IsNullOrWhiteSpace(itemDto.ControlledList))
                    {
                        if (Enum.TryParse<ControlledSubstanceList>(itemDto.ControlledList, true, out var parsed))
                            controlledList = parsed;
                    }

                    var item = new DigitalPrescriptionItem(
                        prescription.Id,
                        itemDto.MedicationId,
                        itemDto.MedicationName,
                        itemDto.Dosage,
                        itemDto.PharmaceuticalForm,
                        itemDto.Frequency,
                        itemDto.DurationDays,
                        itemDto.Quantity,
                        GetTenantId(),
                        itemDto.GenericName,
                        itemDto.ActiveIngredient,
                        itemDto.IsControlledSubstance,
                        controlledList,
                        itemDto.AnvisaRegistration,
                        itemDto.AdministrationRoute,
                        itemDto.Instructions
                    );

                    prescription.AddItem(item);
                }

                var created = await _prescriptionRepository.AddAsync(prescription);
                var result = _mapper.Map<DigitalPrescriptionDto>(created);

                return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get prescription by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<DigitalPrescriptionDto>> GetById(Guid id)
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(id, GetTenantId());

            if (prescription == null)
                return NotFound($"Prescription {id} not found");

            return Ok(_mapper.Map<DigitalPrescriptionDto>(prescription));
        }

        /// <summary>
        /// Get prescriptions by patient ID
        /// </summary>
        [HttpGet("patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<DigitalPrescriptionDto>>> GetByPatient(Guid patientId)
        {
            var prescriptions = await _prescriptionRepository.GetByPatientIdAsync(patientId, GetTenantId());
            return Ok(_mapper.Map<IEnumerable<DigitalPrescriptionDto>>(prescriptions));
        }

        /// <summary>
        /// Get prescriptions by medical record ID
        /// </summary>
        [HttpGet("medical-record/{medicalRecordId}")]
        public async Task<ActionResult<IEnumerable<DigitalPrescriptionDto>>> GetByMedicalRecord(Guid medicalRecordId)
        {
            var prescriptions = await _prescriptionRepository.GetByMedicalRecordIdAsync(medicalRecordId, GetTenantId());
            return Ok(_mapper.Map<IEnumerable<DigitalPrescriptionDto>>(prescriptions));
        }

        /// <summary>
        /// Get prescriptions by doctor ID
        /// </summary>
        [HttpGet("doctor/{doctorId}")]
        public async Task<ActionResult<IEnumerable<DigitalPrescriptionDto>>> GetByDoctor(Guid doctorId)
        {
            var prescriptions = await _prescriptionRepository.GetByDoctorIdAsync(doctorId, GetTenantId());
            return Ok(_mapper.Map<IEnumerable<DigitalPrescriptionDto>>(prescriptions));
        }

        /// <summary>
        /// Get active prescriptions for a patient
        /// </summary>
        [HttpGet("patient/{patientId}/active")]
        public async Task<ActionResult<IEnumerable<DigitalPrescriptionDto>>> GetActiveForPatient(Guid patientId)
        {
            var prescriptions = await _prescriptionRepository.GetActivePrescriptionsForPatientAsync(patientId, GetTenantId());
            return Ok(_mapper.Map<IEnumerable<DigitalPrescriptionDto>>(prescriptions));
        }

        /// <summary>
        /// Get prescription by verification code (QR code lookup)
        /// </summary>
        [HttpGet("verify/{verificationCode}")]
        public async Task<ActionResult<DigitalPrescriptionDto>> GetByVerificationCode(string verificationCode)
        {
            var prescription = await _prescriptionRepository.GetByVerificationCodeAsync(verificationCode);

            if (prescription == null)
                return NotFound($"Prescription with verification code {verificationCode} not found");

            return Ok(_mapper.Map<DigitalPrescriptionDto>(prescription));
        }

        /// <summary>
        /// Sign a prescription with digital signature
        /// </summary>
        [HttpPost("{id}/sign")]
        public async Task<ActionResult> SignPrescription(Guid id, [FromBody] SignPrescriptionDto dto)
        {
            try
            {
                var prescription = await _prescriptionRepository.GetByIdAsync(id, GetTenantId());

                if (prescription == null)
                    return NotFound($"Prescription {id} not found");

                prescription.SignPrescription(dto.DigitalSignature, dto.CertificateThumbprint);
                await _prescriptionRepository.UpdateAsync(prescription);

                return Ok(new { message = "Prescription signed successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Deactivate a prescription
        /// </summary>
        [HttpPost("{id}/deactivate")]
        public async Task<ActionResult> Deactivate(Guid id)
        {
            try
            {
                var prescription = await _prescriptionRepository.GetByIdAsync(id, GetTenantId());

                if (prescription == null)
                    return NotFound($"Prescription {id} not found");

                prescription.Deactivate();
                await _prescriptionRepository.UpdateAsync(prescription);

                return Ok(new { message = "Prescription deactivated successfully" });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Get prescriptions requiring SNGPC report
        /// </summary>
        [HttpGet("sngpc/unreported")]
        public async Task<ActionResult<IEnumerable<DigitalPrescriptionDto>>> GetUnreportedToSNGPC(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
            var end = endDate ?? DateTime.UtcNow;

            var prescriptions = await _prescriptionRepository.GetUnreportedToSNGPCAsync(start, end, GetTenantId());
            return Ok(_mapper.Map<IEnumerable<DigitalPrescriptionDto>>(prescriptions));
        }
    }

    // Additional DTO for signing
    public class SignPrescriptionDto
    {
        public string DigitalSignature { get; set; } = null!;
        public string CertificateThumbprint { get; set; } = null!;
    }
}
