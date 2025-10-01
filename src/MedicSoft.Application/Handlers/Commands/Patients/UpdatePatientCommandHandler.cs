using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Patients;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Application.Handlers.Commands.Patients
{
    public class UpdatePatientCommandHandler : IRequestHandler<UpdatePatientCommand, PatientDto>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;

        public UpdatePatientCommandHandler(IPatientRepository patientRepository, IMapper mapper)
        {
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        public async Task<PatientDto> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetByIdAsync(request.PatientId, request.TenantId);
            if (patient == null)
            {
                throw new InvalidOperationException("Patient not found");
            }

            // Check if email is unique (excluding current patient)
            if (await _patientRepository.IsEmailUniqueAsync(request.Patient.Email, request.TenantId, request.PatientId) == false)
            {
                throw new InvalidOperationException("A patient with this email already exists");
            }

            // Update patient information
            var email = new Email(request.Patient.Email);
            var phone = new Phone(request.Patient.PhoneCountryCode, request.Patient.PhoneNumber);
            var address = new Address(
                request.Patient.Address.Street,
                request.Patient.Address.Number,
                request.Patient.Address.Neighborhood,
                request.Patient.Address.City,
                request.Patient.Address.State,
                request.Patient.Address.ZipCode,
                request.Patient.Address.Country,
                request.Patient.Address.Complement
            );

            patient.UpdatePersonalInfo(request.Patient.Name, email, phone, address);
            patient.UpdateMedicalInfo(request.Patient.MedicalHistory, request.Patient.Allergies);

            await _patientRepository.UpdateAsync(patient);
            return _mapper.Map<PatientDto>(patient);
        }
    }
}