using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.DiagnosticHypotheses;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.DiagnosticHypotheses
{
    public class GetDiagnosticHypothesesByMedicalRecordQueryHandler 
        : IRequestHandler<GetDiagnosticHypothesesByMedicalRecordQuery, IEnumerable<DiagnosticHypothesisDto>>
    {
        private readonly IDiagnosticHypothesisRepository _diagnosticHypothesisRepository;
        private readonly IMapper _mapper;

        public GetDiagnosticHypothesesByMedicalRecordQueryHandler(
            IDiagnosticHypothesisRepository diagnosticHypothesisRepository,
            IMapper mapper)
        {
            _diagnosticHypothesisRepository = diagnosticHypothesisRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<DiagnosticHypothesisDto>> Handle(
            GetDiagnosticHypothesesByMedicalRecordQuery request, 
            CancellationToken cancellationToken)
        {
            var hypotheses = await _diagnosticHypothesisRepository
                .GetByMedicalRecordIdAsync(request.MedicalRecordId, request.TenantId);
            
            return _mapper.Map<IEnumerable<DiagnosticHypothesisDto>>(hypotheses);
        }
    }
}
