using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.DiagnosticHypotheses;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.DiagnosticHypotheses
{
    public class CreateDiagnosticHypothesisCommandHandler : IRequestHandler<CreateDiagnosticHypothesisCommand, DiagnosticHypothesisDto>
    {
        private readonly IDiagnosticHypothesisRepository _diagnosticHypothesisRepository;
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IMapper _mapper;

        public CreateDiagnosticHypothesisCommandHandler(
            IDiagnosticHypothesisRepository diagnosticHypothesisRepository,
            IMedicalRecordRepository medicalRecordRepository,
            IMapper mapper)
        {
            _diagnosticHypothesisRepository = diagnosticHypothesisRepository;
            _medicalRecordRepository = medicalRecordRepository;
            _mapper = mapper;
        }

        public async Task<DiagnosticHypothesisDto> Handle(CreateDiagnosticHypothesisCommand request, CancellationToken cancellationToken)
        {
            // Validate medical record exists
            var medicalRecord = await _medicalRecordRepository.GetByIdAsync(request.HypothesisDto.MedicalRecordId, request.TenantId);
            if (medicalRecord == null)
            {
                throw new InvalidOperationException("Medical record not found");
            }

            // Convert DTO enum to Domain enum
            var diagnosisType = request.HypothesisDto.Type == DiagnosisTypeDto.Principal 
                ? DiagnosisType.Principal 
                : DiagnosisType.Secondary;

            // Create diagnostic hypothesis
            var hypothesis = new DiagnosticHypothesis(
                request.HypothesisDto.MedicalRecordId,
                request.TenantId,
                request.HypothesisDto.Description,
                request.HypothesisDto.ICD10Code,
                diagnosisType
            );

            await _diagnosticHypothesisRepository.AddAsync(hypothesis);

            return _mapper.Map<DiagnosticHypothesisDto>(hypothesis);
        }
    }
}
