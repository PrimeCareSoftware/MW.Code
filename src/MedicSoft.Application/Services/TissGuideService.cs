using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Service implementation for managing TISS guides
    /// </summary>
    public class TissGuideService : ITissGuideService
    {
        private readonly ITissGuideRepository _guideRepository;
        private readonly ITissGuideProcedureRepository _procedureRepository;
        private readonly ITissBatchRepository _batchRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientHealthInsuranceRepository _insuranceRepository;
        private readonly IMapper _mapper;

        public TissGuideService(
            ITissGuideRepository guideRepository,
            ITissGuideProcedureRepository procedureRepository,
            ITissBatchRepository batchRepository,
            IAppointmentRepository appointmentRepository,
            IPatientHealthInsuranceRepository insuranceRepository,
            IMapper mapper)
        {
            _guideRepository = guideRepository;
            _procedureRepository = procedureRepository;
            _batchRepository = batchRepository;
            _appointmentRepository = appointmentRepository;
            _insuranceRepository = insuranceRepository;
            _mapper = mapper;
        }

        public async Task<TissGuideDto> CreateAsync(CreateTissGuideDto dto, string tenantId)
        {
            // Validate batch exists
            var batch = await _batchRepository.GetByIdAsync(dto.TissBatchId, tenantId);
            if (batch == null)
            {
                throw new InvalidOperationException($"TISS batch with ID {dto.TissBatchId} not found");
            }

            // Validate appointment exists
            var appointment = await _appointmentRepository.GetByIdAsync(dto.AppointmentId, tenantId);
            if (appointment == null)
            {
                throw new InvalidOperationException($"Appointment with ID {dto.AppointmentId} not found");
            }

            // Validate patient health insurance exists
            var insurance = await _insuranceRepository.GetByIdAsync(dto.PatientHealthInsuranceId, tenantId);
            if (insurance == null)
            {
                throw new InvalidOperationException($"Patient health insurance with ID {dto.PatientHealthInsuranceId} not found");
            }

            if (!insurance.IsValid())
            {
                throw new InvalidOperationException("Patient health insurance is not valid");
            }

            // Parse guide type
            if (!Enum.TryParse<TissGuideType>(dto.GuideType, true, out var guideType))
            {
                throw new ArgumentException($"Invalid guide type: {dto.GuideType}");
            }

            // Generate guide number
            var guideNumber = $"GUIDE-{DateTime.UtcNow:yyyyMMddHHmmss}-{Guid.NewGuid().ToString().Substring(0, 8).ToUpper()}";

            var guide = new TissGuide(
                dto.TissBatchId,
                dto.AppointmentId,
                dto.PatientHealthInsuranceId,
                guideNumber,
                guideType,
                dto.ServiceDate,
                tenantId,
                dto.AuthorizationNumber
            );

            await _guideRepository.AddAsync(guide);
            await _guideRepository.SaveChangesAsync();

            // Reload with navigation properties
            var result = await _guideRepository.GetWithProceduresAsync(guide.Id, tenantId);
            return _mapper.Map<TissGuideDto>(result);
        }

        public async Task<TissGuideDto> AddProcedureAsync(Guid guideId, AddProcedureToGuideDto dto, string tenantId)
        {
            var guide = await _guideRepository.GetWithProceduresAsync(guideId, tenantId);
            if (guide == null)
            {
                throw new InvalidOperationException($"TISS guide with ID {guideId} not found");
            }

            var procedure = new TissGuideProcedure(
                guideId,
                dto.ProcedureCode,
                dto.ProcedureDescription,
                dto.Quantity,
                dto.UnitPrice,
                tenantId
            );

            guide.AddProcedure(procedure);
            
            await _procedureRepository.AddAsync(procedure);
            await _guideRepository.UpdateAsync(guide);
            await _guideRepository.SaveChangesAsync();

            // Reload with updated procedures
            var result = await _guideRepository.GetWithProceduresAsync(guideId, tenantId);
            return _mapper.Map<TissGuideDto>(result);
        }

        public async Task<TissGuideDto> RemoveProcedureAsync(Guid guideId, Guid procedureId, string tenantId)
        {
            var guide = await _guideRepository.GetWithProceduresAsync(guideId, tenantId);
            if (guide == null)
            {
                throw new InvalidOperationException($"TISS guide with ID {guideId} not found");
            }

            guide.RemoveProcedure(procedureId);
            
            // Delete the procedure from database
            await _procedureRepository.DeleteAsync(procedureId, tenantId);

            await _guideRepository.UpdateAsync(guide);
            await _guideRepository.SaveChangesAsync();

            // Reload with updated procedures
            var result = await _guideRepository.GetWithProceduresAsync(guideId, tenantId);
            return _mapper.Map<TissGuideDto>(result);
        }

        public async Task<TissGuideDto> FinalizeAsync(Guid guideId, string tenantId)
        {
            var guide = await _guideRepository.GetWithProceduresAsync(guideId, tenantId);
            if (guide == null)
            {
                throw new InvalidOperationException($"TISS guide with ID {guideId} not found");
            }

            guide.MarkAsSent();
            
            await _guideRepository.UpdateAsync(guide);
            await _guideRepository.SaveChangesAsync();

            var result = await _guideRepository.GetWithProceduresAsync(guideId, tenantId);
            return _mapper.Map<TissGuideDto>(result);
        }

        public async Task<IEnumerable<TissGuideDto>> GetAllAsync(string tenantId)
        {
            var guides = await _guideRepository.GetAllAsync(tenantId);
            return _mapper.Map<IEnumerable<TissGuideDto>>(guides);
        }

        public async Task<TissGuideDto?> GetByIdAsync(Guid id, string tenantId)
        {
            var guide = await _guideRepository.GetWithProceduresAsync(id, tenantId);
            return guide != null ? _mapper.Map<TissGuideDto>(guide) : null;
        }

        public async Task<IEnumerable<TissGuideDto>> GetByBatchIdAsync(Guid batchId, string tenantId)
        {
            var guides = await _guideRepository.GetByBatchIdAsync(batchId, tenantId);
            return _mapper.Map<IEnumerable<TissGuideDto>>(guides);
        }

        public async Task<IEnumerable<TissGuideDto>> GetByAppointmentIdAsync(Guid appointmentId, string tenantId)
        {
            var guides = await _guideRepository.GetByAppointmentIdAsync(appointmentId, tenantId);
            return _mapper.Map<IEnumerable<TissGuideDto>>(guides);
        }

        public async Task<IEnumerable<TissGuideDto>> GetByStatusAsync(string status, string tenantId)
        {
            if (!Enum.TryParse<GuideStatus>(status, true, out var statusEnum))
            {
                throw new ArgumentException($"Invalid status: {status}");
            }

            var guides = await _guideRepository.GetByStatusAsync(statusEnum, tenantId);
            return _mapper.Map<IEnumerable<TissGuideDto>>(guides);
        }

        public async Task<TissGuideDto?> GetByGuideNumberAsync(string guideNumber, string tenantId)
        {
            var guide = await _guideRepository.GetByGuideNumberAsync(guideNumber, tenantId);
            return guide != null ? _mapper.Map<TissGuideDto>(guide) : null;
        }

        public async Task<TissGuideDto> ProcessResponseAsync(Guid guideId, ProcessGuideResponseDto dto, string tenantId)
        {
            var guide = await _guideRepository.GetWithProceduresAsync(guideId, tenantId);
            if (guide == null)
            {
                throw new InvalidOperationException($"TISS guide with ID {guideId} not found");
            }

            // Process guide-level response
            if (dto.ApprovedAmount.HasValue)
            {
                if (dto.GlosedAmount.HasValue && dto.GlosedAmount.Value > 0)
                {
                    // Partially approved
                    guide.Approve(dto.ApprovedAmount.Value);
                    if (!string.IsNullOrWhiteSpace(dto.GlossReason))
                    {
                        // Note: Guide entity doesn't expose GlossReason setter, so we use reflection or accept the limitation
                    }
                }
                else
                {
                    // Fully approved
                    guide.Approve(dto.ApprovedAmount.Value);
                }
            }
            else if (!string.IsNullOrWhiteSpace(dto.GlossReason))
            {
                // Rejected
                guide.Reject(dto.GlossReason);
            }

            // Process procedure-level responses
            foreach (var procResponse in dto.ProcedureResponses)
            {
                var procedure = guide.Procedures.FirstOrDefault(p => p.Id == procResponse.ProcedureId);
                if (procedure != null)
                {
                    procedure.ProcessOperatorResponse(
                        null, // approvedQuantity not provided in DTO
                        procResponse.ApprovedAmount,
                        procResponse.GlossReason
                    );
                    await _procedureRepository.UpdateAsync(procedure);
                }
            }

            await _guideRepository.UpdateAsync(guide);
            await _guideRepository.SaveChangesAsync();

            var result = await _guideRepository.GetWithProceduresAsync(guideId, tenantId);
            return _mapper.Map<TissGuideDto>(result);
        }

        public async Task<TissGuideDto> MarkAsPaidAsync(Guid guideId, string tenantId)
        {
            var guide = await _guideRepository.GetWithProceduresAsync(guideId, tenantId);
            if (guide == null)
            {
                throw new InvalidOperationException($"TISS guide with ID {guideId} not found");
            }

            guide.MarkAsPaid();
            
            await _guideRepository.UpdateAsync(guide);
            await _guideRepository.SaveChangesAsync();

            var result = await _guideRepository.GetWithProceduresAsync(guideId, tenantId);
            return _mapper.Map<TissGuideDto>(result);
        }
    }
}
