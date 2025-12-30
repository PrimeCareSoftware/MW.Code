using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MedicSoft.Application.Queries.Patients;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Patients
{
    public class GetPatientsByClinicIdQueryHandler : IRequestHandler<GetPatientsByClinicIdQuery, IEnumerable<PatientDto>>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;

        public GetPatientsByClinicIdQueryHandler(IPatientRepository patientRepository, IMapper mapper)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PatientDto>> Handle(GetPatientsByClinicIdQuery request, CancellationToken cancellationToken)
        {
            var patients = await _patientRepository.GetByClinicIdAsync(request.ClinicId, request.TenantId);
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }
    }
}
