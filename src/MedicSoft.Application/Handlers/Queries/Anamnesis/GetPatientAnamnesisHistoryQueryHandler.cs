using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs.Anamnesis;
using MedicSoft.Application.Queries.Anamnesis;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Anamnesis
{
    public class GetPatientAnamnesisHistoryQueryHandler : IRequestHandler<GetPatientAnamnesisHistoryQuery, List<AnamnesisResponseDto>>
    {
        private readonly IAnamnesisResponseRepository _repository;
        private readonly IMapper _mapper;

        public GetPatientAnamnesisHistoryQueryHandler(IAnamnesisResponseRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<AnamnesisResponseDto>> Handle(GetPatientAnamnesisHistoryQuery request, CancellationToken cancellationToken)
        {
            var responses = await _repository.GetByPatientIdAsync(request.PatientId, request.TenantId);
            return _mapper.Map<List<AnamnesisResponseDto>>(responses);
        }
    }
}
