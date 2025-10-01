using AutoMapper;
using MediatR;
using MedicSoft.Application.Queries.Patients;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Patients
{
    public class GetPatientByIdQueryHandler : IRequestHandler<GetPatientByIdQuery, PatientDto?>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;

        public GetPatientByIdQueryHandler(IPatientRepository patientRepository, IMapper mapper)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        public async Task<PatientDto?> Handle(GetPatientByIdQuery request, CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetByIdAsync(request.PatientId, request.TenantId);
            return patient != null ? _mapper.Map<PatientDto>(patient) : null;
        }
    }
}