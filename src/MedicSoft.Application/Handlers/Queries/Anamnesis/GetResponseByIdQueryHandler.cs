using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs.Anamnesis;
using MedicSoft.Application.Queries.Anamnesis;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Anamnesis
{
    public class GetResponseByIdQueryHandler : IRequestHandler<GetResponseByIdQuery, AnamnesisResponseDto?>
    {
        private readonly IAnamnesisResponseRepository _repository;
        private readonly IMapper _mapper;

        public GetResponseByIdQueryHandler(IAnamnesisResponseRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AnamnesisResponseDto?> Handle(GetResponseByIdQuery request, CancellationToken cancellationToken)
        {
            var response = await _repository.GetByIdAsync(request.ResponseId, request.TenantId);
            return response == null ? null : _mapper.Map<AnamnesisResponseDto>(response);
        }
    }
}
