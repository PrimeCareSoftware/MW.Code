using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.DiagnosticHypotheses;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.DiagnosticHypotheses
{
    public class UpdateDiagnosticHypothesisCommandHandler : IRequestHandler<UpdateDiagnosticHypothesisCommand, DiagnosticHypothesisDto>
    {
        private readonly IDiagnosticHypothesisRepository _diagnosticHypothesisRepository;
        private readonly IMapper _mapper;

        public UpdateDiagnosticHypothesisCommandHandler(
            IDiagnosticHypothesisRepository diagnosticHypothesisRepository,
            IMapper mapper)
        {
            _diagnosticHypothesisRepository = diagnosticHypothesisRepository;
            _mapper = mapper;
        }

        public async Task<DiagnosticHypothesisDto> Handle(UpdateDiagnosticHypothesisCommand request, CancellationToken cancellationToken)
        {
            var hypothesis = await _diagnosticHypothesisRepository.GetByIdAsync(request.Id, request.TenantId);
            if (hypothesis == null)
            {
                throw new InvalidOperationException("Diagnostic hypothesis not found");
            }

            // Update description if provided
            if (!string.IsNullOrWhiteSpace(request.UpdateDto.Description))
            {
                hypothesis.UpdateDescription(request.UpdateDto.Description);
            }

            // Update ICD-10 code if provided
            if (!string.IsNullOrWhiteSpace(request.UpdateDto.ICD10Code))
            {
                hypothesis.UpdateICD10Code(request.UpdateDto.ICD10Code);
            }

            // Update type if provided
            if (request.UpdateDto.Type.HasValue)
            {
                var diagnosisType = request.UpdateDto.Type.Value == DiagnosisTypeDto.Principal 
                    ? DiagnosisType.Principal 
                    : DiagnosisType.Secondary;
                hypothesis.UpdateType(diagnosisType);
            }

            await _diagnosticHypothesisRepository.UpdateAsync(hypothesis);

            return _mapper.Map<DiagnosticHypothesisDto>(hypothesis);
        }
    }
}
