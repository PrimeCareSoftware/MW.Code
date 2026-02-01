using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.MedicalRecords;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.MedicalRecords
{
    public class CompleteMedicalRecordCommandHandler : IRequestHandler<CompleteMedicalRecordCommand, MedicalRecordDto>
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ICfm1821ValidationService _cfm1821ValidationService;
        private readonly IMapper _mapper;

        public CompleteMedicalRecordCommandHandler(
            IMedicalRecordRepository medicalRecordRepository,
            IAppointmentRepository appointmentRepository,
            ICfm1821ValidationService cfm1821ValidationService,
            IMapper mapper)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _appointmentRepository = appointmentRepository;
            _cfm1821ValidationService = cfm1821ValidationService;
            _mapper = mapper;
        }

        public async Task<MedicalRecordDto> Handle(CompleteMedicalRecordCommand request, CancellationToken cancellationToken)
        {
            return await _medicalRecordRepository.ExecuteInTransactionAsync(async () =>
            {
                var medicalRecord = await _medicalRecordRepository.GetByIdAsync(request.Id, request.TenantId);
                if (medicalRecord == null)
                {
                    throw new InvalidOperationException("Medical record not found");
                }

                // CFM 1.821 validation removed - allow completion without all required fields
                // Complete the medical record
                medicalRecord.CompleteConsultation(
                    request.CompleteDto.Diagnosis,
                    request.CompleteDto.Prescription,
                    request.CompleteDto.Notes
                );

                await _medicalRecordRepository.UpdateAsync(medicalRecord);

                // Check out the appointment
                var appointment = await _appointmentRepository.GetByIdAsync(medicalRecord.AppointmentId, request.TenantId);
                if (appointment != null)
                {
                    appointment.CheckOut(medicalRecord.Notes);
                    await _appointmentRepository.UpdateAsync(appointment);
                }

                return _mapper.Map<MedicalRecordDto>(medicalRecord);
            });
        }
    }
}
