using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.InformedConsents;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.InformedConsents
{
    public class CreateInformedConsentCommandHandler : IRequestHandler<CreateInformedConsentCommand, InformedConsentDto>
    {
        private readonly IInformedConsentRepository _informedConsentRepository;
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IMapper _mapper;

        public CreateInformedConsentCommandHandler(
            IInformedConsentRepository informedConsentRepository,
            IMedicalRecordRepository medicalRecordRepository,
            IPatientRepository patientRepository,
            IMapper mapper)
        {
            _informedConsentRepository = informedConsentRepository;
            _medicalRecordRepository = medicalRecordRepository;
            _patientRepository = patientRepository;
            _mapper = mapper;
        }

        public async Task<InformedConsentDto> Handle(CreateInformedConsentCommand request, CancellationToken cancellationToken)
        {
            // Validate medical record exists
            var medicalRecord = await _medicalRecordRepository.GetByIdAsync(request.ConsentDto.MedicalRecordId, request.TenantId);
            if (medicalRecord == null)
            {
                throw new InvalidOperationException("Medical record not found");
            }

            // Validate patient exists
            var patient = await _patientRepository.GetByIdAsync(request.ConsentDto.PatientId, request.TenantId);
            if (patient == null)
            {
                throw new InvalidOperationException("Patient not found");
            }

            // Create informed consent
            var consent = new InformedConsent(
                request.ConsentDto.MedicalRecordId,
                request.ConsentDto.PatientId,
                request.TenantId,
                request.ConsentDto.ConsentText,
                request.ConsentDto.RegisteredByUserId
            );

            await _informedConsentRepository.AddAsync(consent);

            return _mapper.Map<InformedConsentDto>(consent);
        }
    }
}
