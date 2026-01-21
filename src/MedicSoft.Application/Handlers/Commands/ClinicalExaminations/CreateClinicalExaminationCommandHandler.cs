using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.ClinicalExaminations;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.ClinicalExaminations
{
    public class CreateClinicalExaminationCommandHandler : IRequestHandler<CreateClinicalExaminationCommand, ClinicalExaminationDto>
    {
        private readonly IClinicalExaminationRepository _clinicalExaminationRepository;
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IMapper _mapper;

        public CreateClinicalExaminationCommandHandler(
            IClinicalExaminationRepository clinicalExaminationRepository,
            IMedicalRecordRepository medicalRecordRepository,
            IMapper mapper)
        {
            _clinicalExaminationRepository = clinicalExaminationRepository;
            _medicalRecordRepository = medicalRecordRepository;
            _mapper = mapper;
        }

        public async Task<ClinicalExaminationDto> Handle(CreateClinicalExaminationCommand request, CancellationToken cancellationToken)
        {
            // Validate medical record exists
            var medicalRecord = await _medicalRecordRepository.GetByIdAsync(request.ExaminationDto.MedicalRecordId, request.TenantId);
            if (medicalRecord == null)
            {
                throw new InvalidOperationException("Medical record not found");
            }

            // Create clinical examination
            var examination = new ClinicalExamination(
                request.ExaminationDto.MedicalRecordId,
                request.TenantId,
                request.ExaminationDto.SystematicExamination,
                request.ExaminationDto.BloodPressureSystolic,
                request.ExaminationDto.BloodPressureDiastolic,
                request.ExaminationDto.HeartRate,
                request.ExaminationDto.RespiratoryRate,
                request.ExaminationDto.Temperature,
                request.ExaminationDto.OxygenSaturation,
                request.ExaminationDto.Weight,
                request.ExaminationDto.Height,
                request.ExaminationDto.GeneralState
            );

            await _clinicalExaminationRepository.AddAsync(examination);

            return _mapper.Map<ClinicalExaminationDto>(examination);
        }
    }
}
