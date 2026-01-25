using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Clinics;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Clinics
{
    public class GetClinicByIdQueryHandler : IRequestHandler<GetClinicByIdQuery, ClinicDto?>
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IMapper _mapper;

        public GetClinicByIdQueryHandler(IClinicRepository clinicRepository, IMapper mapper)
        {
            _clinicRepository = clinicRepository;
            _mapper = mapper;
        }

        public async Task<ClinicDto?> Handle(GetClinicByIdQuery request, CancellationToken cancellationToken)
        {
            var clinic = await _clinicRepository.GetByIdAsync(request.ClinicId, request.TenantId);
            return clinic != null ? _mapper.Map<ClinicDto>(clinic) : null;
        }
    }
}
