using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Patients;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Patients
{
    public class SearchPatientsByClinicQueryHandler : IRequestHandler<SearchPatientsByClinicQuery, IEnumerable<PatientDto>>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;

        public SearchPatientsByClinicQueryHandler(IPatientRepository patientRepository, IMapper mapper)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<PatientDto>> Handle(SearchPatientsByClinicQuery request, CancellationToken cancellationToken)
        {
            var patients = await _patientRepository.SearchAsync(request.SearchTerm, request.TenantId, request.ClinicId);
            return _mapper.Map<IEnumerable<PatientDto>>(patients);
        }
    }
}
