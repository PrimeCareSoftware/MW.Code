using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Appointments;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
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
        private readonly IPatientClinicLinkRepository _patientClinicLinkRepository;
        private readonly IUserRepository _userRepository;
        private readonly IInAppNotificationService _notificationService;
        private readonly AppointmentSchedulingService _schedulingService;
        private readonly IMapper _mapper;

        public CreateAppointmentCommandHandler(
            IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository,
            IClinicRepository clinicRepository,
            IMedicalRecordRepository medicalRecordRepository,
            IPatientClinicLinkRepository patientClinicLinkRepository,
            IUserRepository userRepository,
            IInAppNotificationService notificationService,
            AppointmentSchedulingService schedulingService,
            IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _clinicRepository = clinicRepository;
            _medicalRecordRepository = medicalRecordRepository;
            _patientClinicLinkRepository = patientClinicLinkRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
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
                request.Appointment.ProfessionalId,
                request.Appointment.Notes
            );

            // Set room number if provided (before creating medical record)
            if (!string.IsNullOrWhiteSpace(request.Appointment.RoomNumber))
            {
                appointment.UpdateRoomNumber(request.Appointment.RoomNumber);
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

            // Check if notification to primary doctor should be sent
            await NotifyPrimaryDoctorIfNeededAsync(appointment, patient, clinic, request.TenantId);

            return _mapper.Map<AppointmentDto>(appointment);
        }

        private async Task NotifyPrimaryDoctorIfNeededAsync(
            Appointment appointment, 
            Patient patient, 
            Clinic clinic, 
            string tenantId)
        {
            // Check if clinic has notification enabled
            if (!clinic.NotifyPrimaryDoctorOnOtherDoctorAppointment)
                return;

            // Check if appointment has a professional assigned
            if (!appointment.ProfessionalId.HasValue)
                return;

            // Get patient-clinic link to check primary doctor
            var patientClinicLink = await _patientClinicLinkRepository.GetLinkAsync(
                patient.Id, 
                clinic.Id, 
                tenantId);

            if (patientClinicLink == null || !patientClinicLink.PrimaryDoctorId.HasValue)
                return;

            // Check if the appointment professional is different from primary doctor
            if (patientClinicLink.PrimaryDoctorId == appointment.ProfessionalId)
                return;

            // Get user information for notification
            var appointmentDoctor = await _userRepository.GetByIdAsync(appointment.ProfessionalId.Value, tenantId);
            if (appointmentDoctor == null)
                return;

            // Send notification to primary doctor
            var message = $"O paciente {patient.Name} foi agendado para consulta com Dr(a). {appointmentDoctor.FullName} " +
                         $"em {appointment.ScheduledDate:dd/MM/yyyy} às {appointment.ScheduledTime:hh\\:mm}.";

            // Note: The current notification service implementation stores notifications per tenant.
            // In a production system, this should be enhanced to target specific users (the primary doctor).
            await _notificationService.CreateNotificationAsync(
                type: "DoctorAppointmentNotification",
                title: "Paciente Agendado com Outro Médico",
                message: message,
                data: new
                {
                    AppointmentId = appointment.Id,
                    PatientId = patient.Id,
                    PatientName = patient.Name,
                    AppointmentDoctorId = appointment.ProfessionalId,
                    AppointmentDoctorName = appointmentDoctor.FullName,
                    PrimaryDoctorId = patientClinicLink.PrimaryDoctorId,
                    TargetUserId = patientClinicLink.PrimaryDoctorId, // For future filtering on the client side
                    ScheduledDate = appointment.ScheduledDate,
                    ScheduledTime = appointment.ScheduledTime
                },
                tenantId: tenantId
            );
        }
    }
}