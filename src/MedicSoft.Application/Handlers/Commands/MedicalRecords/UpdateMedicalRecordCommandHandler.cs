using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.MedicalRecords;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.MedicalRecords
{
    public class UpdateMedicalRecordCommandHandler : IRequestHandler<UpdateMedicalRecordCommand, MedicalRecordDto>
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IMapper _mapper;

        public UpdateMedicalRecordCommandHandler(
            IMedicalRecordRepository medicalRecordRepository,
            IMapper mapper)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _mapper = mapper;
        }

        public async Task<MedicalRecordDto> Handle(UpdateMedicalRecordCommand request, CancellationToken cancellationToken)
        {
            var medicalRecord = await _medicalRecordRepository.GetByIdAsync(request.Id, request.TenantId);
            if (medicalRecord == null)
            {
                throw new InvalidOperationException("Medical record not found");
            }

            if (request.UpdateDto.Diagnosis != null)
            {
                medicalRecord.UpdateDiagnosis(request.UpdateDto.Diagnosis);
            }

            if (request.UpdateDto.Prescription != null)
            {
                medicalRecord.UpdatePrescription(request.UpdateDto.Prescription);
            }

            if (request.UpdateDto.Notes != null)
            {
                medicalRecord.UpdateNotes(request.UpdateDto.Notes);
            }

            if (request.UpdateDto.ConsultationDurationMinutes.HasValue)
            {
                medicalRecord.UpdateConsultationTime(request.UpdateDto.ConsultationDurationMinutes.Value);
            }

            await _medicalRecordRepository.UpdateAsync(medicalRecord);

            return _mapper.Map<MedicalRecordDto>(medicalRecord);
        }
    }
}
