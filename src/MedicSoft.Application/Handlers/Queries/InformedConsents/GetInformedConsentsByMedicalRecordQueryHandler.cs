using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.InformedConsents;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.InformedConsents
{
    public class GetInformedConsentsByMedicalRecordQueryHandler 
        : IRequestHandler<GetInformedConsentsByMedicalRecordQuery, IEnumerable<InformedConsentDto>>
    {
        private readonly IInformedConsentRepository _informedConsentRepository;
        private readonly IMapper _mapper;

        public GetInformedConsentsByMedicalRecordQueryHandler(
            IInformedConsentRepository informedConsentRepository,
            IMapper mapper)
        {
            _informedConsentRepository = informedConsentRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<InformedConsentDto>> Handle(
            GetInformedConsentsByMedicalRecordQuery request, 
            CancellationToken cancellationToken)
        {
            var consents = await _informedConsentRepository
                .GetByMedicalRecordIdAsync(request.MedicalRecordId, request.TenantId);
            
            return _mapper.Map<IEnumerable<InformedConsentDto>>(consents);
        }
    }
}
