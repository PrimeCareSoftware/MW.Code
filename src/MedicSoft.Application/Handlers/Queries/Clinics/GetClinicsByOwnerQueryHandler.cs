using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Clinics;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Clinics
{
    public class GetClinicsByOwnerQueryHandler : IRequestHandler<GetClinicsByOwnerQuery, IEnumerable<ClinicDto>>
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IOwnerClinicLinkRepository _ownerClinicLinkRepository;
        private readonly IMapper _mapper;

        public GetClinicsByOwnerQueryHandler(
            IClinicRepository clinicRepository,
            IOwnerClinicLinkRepository ownerClinicLinkRepository,
            IMapper mapper)
        {
            _clinicRepository = clinicRepository;
            _ownerClinicLinkRepository = ownerClinicLinkRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ClinicDto>> Handle(GetClinicsByOwnerQuery request, CancellationToken cancellationToken)
        {
            // Get all clinic links for the owner
            var clinicLinks = await _ownerClinicLinkRepository.GetClinicsByOwnerIdAsync(request.OwnerId);
            
            var clinics = new List<ClinicDto>();
            
            foreach (var link in clinicLinks.Where(l => l.IsActive))
            {
                var clinic = await _clinicRepository.GetByIdAsync(link.ClinicId, request.TenantId);
                if (clinic != null)
                {
                    clinics.Add(_mapper.Map<ClinicDto>(clinic));
                }
            }

            return clinics;
        }
    }
}
