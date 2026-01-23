using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs.Anamnesis;
using MedicSoft.Application.Queries.Anamnesis;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Anamnesis
{
    public class GetResponseByAppointmentQueryHandler : IRequestHandler<GetResponseByAppointmentQuery, AnamnesisResponseDto?>
    {
        private readonly IAnamnesisResponseRepository _repository;
        private readonly IMapper _mapper;

        public GetResponseByAppointmentQueryHandler(IAnamnesisResponseRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AnamnesisResponseDto?> Handle(GetResponseByAppointmentQuery request, CancellationToken cancellationToken)
        {
            var response = await _repository.GetByAppointmentIdAsync(request.AppointmentId, request.TenantId);
            return response == null ? null : _mapper.Map<AnamnesisResponseDto>(response);
        }
    }
}
