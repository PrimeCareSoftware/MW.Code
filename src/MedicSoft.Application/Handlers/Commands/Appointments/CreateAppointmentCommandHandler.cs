using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Appointments;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.Services;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.Handlers.Commands.Appointments
{
    public class CreateAppointmentCommandHandler : IRequestHandler<CreateAppointmentCommand, AppointmentDto>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IMedicalRecordRepository _medicalRecordRepository;
        private readonly AppointmentSchedulingService _schedulingService;
        private readonly IMapper _mapper;

        public CreateAppointmentCommandHandler(
            IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository,
            IClinicRepository clinicRepository,
            IMedicalRecordRepository medicalRecordRepository,
            AppointmentSchedulingService schedulingService,
            IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _clinicRepository = clinicRepository;
            _medicalRecordRepository = medicalRecordRepository;
            _schedulingService = schedulingService;
            _mapper = mapper;
        }

        public async Task<AppointmentDto> Handle(CreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            // Validate patient exists
            var patient = await _patientRepository.GetByIdAsync(request.Appointment.PatientId, request.TenantId);
            if (patient == null)
            {
                throw new InvalidOperationException("Patient not found");
            }

            // Validate clinic exists
            var clinic = await _clinicRepository.GetByIdAsync(request.Appointment.ClinicId, request.TenantId);
            if (clinic == null)
            {
                throw new InvalidOperationException("Clinic not found");
            }

            // Parse appointment type
            if (!Enum.TryParse<AppointmentType>(request.Appointment.Type, true, out var appointmentType))
            {
                appointmentType = AppointmentType.Regular;
            }

            // Schedule the appointment
            var appointment = await _schedulingService.ScheduleAppointmentAsync(
                request.Appointment.PatientId,
                request.Appointment.ClinicId,
                request.Appointment.ScheduledDate,
                request.Appointment.ScheduledTime,
                request.Appointment.DurationMinutes,
                appointmentType,
                request.TenantId,
                request.Appointment.Notes
            );

            // Set room number if provided
            if (!string.IsNullOrWhiteSpace(request.Appointment.RoomNumber))
            {
                appointment.UpdateRoomNumber(request.Appointment.RoomNumber);
                await _appointmentRepository.UpdateAsync(appointment);
            }

            // Automatically create an empty MedicalRecord for this appointment
            var medicalRecord = new MedicalRecord(
                appointmentId: appointment.Id,
                patientId: patient.Id,
                tenantId: request.TenantId,
                consultationStartTime: appointment.ScheduledDate,
                chiefComplaint: "Consulta agendada",
                historyOfPresentIllness: "Registro criado automaticamente no agendamento da consulta. Informações clínicas serão adicionadas pelo profissional de saúde durante o atendimento."
            );

            await _medicalRecordRepository.AddAsync(medicalRecord);

            return _mapper.Map<AppointmentDto>(appointment);
        }
    }
}