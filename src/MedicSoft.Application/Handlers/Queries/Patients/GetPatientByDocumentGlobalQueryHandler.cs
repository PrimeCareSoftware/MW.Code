using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Patients;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Patients
{
    public class GetPatientByDocumentGlobalQueryHandler : IRequestHandler<GetPatientByDocumentGlobalQuery, PatientDto?>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;

        public GetPatientByDocumentGlobalQueryHandler(IPatientRepository patientRepository, IMapper mapper)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        public async Task<PatientDto?> Handle(GetPatientByDocumentGlobalQuery request, CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetByDocumentGlobalAsync(request.Document);
            return patient == null ? null : _mapper.Map<PatientDto>(patient);
        }
    }
}
