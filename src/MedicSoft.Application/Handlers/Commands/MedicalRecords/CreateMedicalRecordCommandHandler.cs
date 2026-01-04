using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.MedicalRecords;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.MedicalRecords
{
    public class CreateMedicalRecordCommandHandler : IRequestHandler<CreateMedicalRecordCommand, MedicalRecordDto>
    {
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMapper _mapper;

        public CreateMedicalRecordCommandHandler(
            IMedicalRecordRepository medicalRecordRepository,
            IPatientRepository patientRepository,
            IAppointmentRepository appointmentRepository,
            IMapper mapper)
        {
            _medicalRecordRepository = medicalRecordRepository;
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
        }

        public async Task<MedicalRecordDto> Handle(CreateMedicalRecordCommand request, CancellationToken cancellationToken)
        {
            return await _medicalRecordRepository.ExecuteInTransactionAsync(async () =>
            {
                // Validate patient exists
                var patient = await _patientRepository.GetByIdAsync(request.MedicalRecordDto.PatientId, request.TenantId);
                if (patient == null)
                {
                    throw new InvalidOperationException("Patient not found");
                }

                // Validate appointment exists
                var appointment = await _appointmentRepository.GetByIdAsync(request.MedicalRecordDto.AppointmentId, request.TenantId);
                if (appointment == null)
                {
                    throw new InvalidOperationException("Appointment not found");
                }

                // Check if medical record already exists for this appointment
                var existingRecord = await _medicalRecordRepository.GetByAppointmentIdAsync(request.MedicalRecordDto.AppointmentId, request.TenantId);
                if (existingRecord != null)
                {
                    throw new InvalidOperationException("Medical record already exists for this appointment");
                }

                // Check in the appointment
                appointment.CheckIn();
                await _appointmentRepository.UpdateAsync(appointment);

                // Create medical record with CFM 1.821 fields
                var medicalRecord = new MedicalRecord(
                    request.MedicalRecordDto.AppointmentId,
                    request.MedicalRecordDto.PatientId,
                    request.TenantId,
                    request.MedicalRecordDto.ConsultationStartTime,
                    chiefComplaint: request.MedicalRecordDto.ChiefComplaint,
                    historyOfPresentIllness: request.MedicalRecordDto.HistoryOfPresentIllness,
                    request.MedicalRecordDto.Diagnosis,
                    request.MedicalRecordDto.Prescription,
                    request.MedicalRecordDto.Notes,
                    request.MedicalRecordDto.PastMedicalHistory,
                    request.MedicalRecordDto.FamilyHistory,
                    request.MedicalRecordDto.LifestyleHabits,
                    request.MedicalRecordDto.CurrentMedications
                );

                await _medicalRecordRepository.AddAsync(medicalRecord);

                var dto = _mapper.Map<MedicalRecordDto>(medicalRecord);
                dto.PatientName = patient.Name;
                
                return dto;
            });
        }
    }
}
