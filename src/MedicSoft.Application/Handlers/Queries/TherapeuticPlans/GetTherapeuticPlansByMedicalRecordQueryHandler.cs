using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.TherapeuticPlans;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.TherapeuticPlans
{
    public class GetTherapeuticPlansByMedicalRecordQueryHandler 
        : IRequestHandler<GetTherapeuticPlansByMedicalRecordQuery, IEnumerable<TherapeuticPlanDto>>
    {
        private readonly ITherapeuticPlanRepository _therapeuticPlanRepository;
        private readonly IMapper _mapper;

        public GetTherapeuticPlansByMedicalRecordQueryHandler(
            ITherapeuticPlanRepository therapeuticPlanRepository,
            IMapper mapper)
        {
            _therapeuticPlanRepository = therapeuticPlanRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<TherapeuticPlanDto>> Handle(
            GetTherapeuticPlansByMedicalRecordQuery request, 
            CancellationToken cancellationToken)
        {
            var plans = await _therapeuticPlanRepository
                .GetByMedicalRecordIdAsync(request.MedicalRecordId, request.TenantId);
            
            return _mapper.Map<IEnumerable<TherapeuticPlanDto>>(plans);
        }
    }
}
