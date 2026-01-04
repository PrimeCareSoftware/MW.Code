using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.ClinicalExaminations;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.ClinicalExaminations
{
    public class UpdateClinicalExaminationCommandHandler : IRequestHandler<UpdateClinicalExaminationCommand, ClinicalExaminationDto>
    {
        private readonly IClinicalExaminationRepository _clinicalExaminationRepository;
        private readonly IMapper _mapper;

        public UpdateClinicalExaminationCommandHandler(
            IClinicalExaminationRepository clinicalExaminationRepository,
            IMapper mapper)
        {
            _clinicalExaminationRepository = clinicalExaminationRepository;
            _mapper = mapper;
        }

        public async Task<ClinicalExaminationDto> Handle(UpdateClinicalExaminationCommand request, CancellationToken cancellationToken)
        {
            var examination = await _clinicalExaminationRepository.GetByIdAsync(request.Id, request.TenantId);
            if (examination == null)
            {
                throw new InvalidOperationException("Clinical examination not found");
            }

            // Update vital signs if provided
            if (request.UpdateDto.BloodPressureSystolic.HasValue || 
                request.UpdateDto.BloodPressureDiastolic.HasValue ||
                request.UpdateDto.HeartRate.HasValue ||
                request.UpdateDto.RespiratoryRate.HasValue ||
                request.UpdateDto.Temperature.HasValue ||
                request.UpdateDto.OxygenSaturation.HasValue)
            {
                examination.UpdateVitalSigns(
                    request.UpdateDto.BloodPressureSystolic,
                    request.UpdateDto.BloodPressureDiastolic,
                    request.UpdateDto.HeartRate,
                    request.UpdateDto.RespiratoryRate,
                    request.UpdateDto.Temperature,
                    request.UpdateDto.OxygenSaturation
                );
            }

            // Update systematic examination if provided
            if (!string.IsNullOrWhiteSpace(request.UpdateDto.SystematicExamination))
            {
                examination.UpdateSystematicExamination(request.UpdateDto.SystematicExamination);
            }

            // Update general state if provided
            if (request.UpdateDto.GeneralState != null)
            {
                examination.UpdateGeneralState(request.UpdateDto.GeneralState);
            }

            await _clinicalExaminationRepository.UpdateAsync(examination);

            return _mapper.Map<ClinicalExaminationDto>(examination);
        }
    }
}
