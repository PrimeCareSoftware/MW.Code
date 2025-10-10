using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Patients;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Application.Handlers.Commands.Patients
{
    public class CreatePatientCommandHandler : IRequestHandler<CreatePatientCommand, PatientDto>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;

        public CreatePatientCommandHandler(IPatientRepository patientRepository, IMapper mapper)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        public async Task<PatientDto> Handle(CreatePatientCommand request, CancellationToken cancellationToken)
        {
            // Check if document already exists
            if (await _patientRepository.IsDocumentUniqueAsync(request.Patient.Document, request.TenantId) == false)
            {
                throw new InvalidOperationException("A patient with this document already exists");
            }

            // Check if email already exists
            if (await _patientRepository.IsEmailUniqueAsync(request.Patient.Email, request.TenantId) == false)
            {
                throw new InvalidOperationException("A patient with this email already exists");
            }

            // Create patient using mapper with tenant context
            var patient = _mapper.Map<Domain.Entities.Patient>(request.Patient, opt => 
                opt.Items["TenantId"] = request.TenantId);

            // If guardian is specified, validate and set
            if (request.Patient.GuardianId.HasValue)
            {
                var guardian = await _patientRepository.GetByIdAsync(request.Patient.GuardianId.Value, request.TenantId);
                if (guardian == null)
                    throw new InvalidOperationException("Guardian patient not found");

                patient.SetGuardian(request.Patient.GuardianId.Value);
            }

            var createdPatient = await _patientRepository.AddAsync(patient);
            return _mapper.Map<PatientDto>(createdPatient);
        }
    }
}