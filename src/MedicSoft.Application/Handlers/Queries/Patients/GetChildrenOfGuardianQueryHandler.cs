using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Patients;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Patients
{
    public class GetChildrenOfGuardianQueryHandler : IRequestHandler<GetChildrenOfGuardianQuery, IEnumerable<PatientDto>>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;

        public GetChildrenOfGuardianQueryHandler(IPatientRepository patientRepository, IMapper mapper)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PatientDto>> Handle(GetChildrenOfGuardianQuery request, CancellationToken cancellationToken)
        {
            // Get guardian patient
            var guardian = await _patientRepository.GetByIdAsync(request.GuardianId, request.TenantId);
            if (guardian == null)
                return Enumerable.Empty<PatientDto>();

            // Get children
            var children = await _patientRepository.GetChildrenOfGuardianAsync(request.GuardianId, request.TenantId);

            return _mapper.Map<IEnumerable<PatientDto>>(children);
        }
    }
}
