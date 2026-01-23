using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Appointments;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.Handlers.Commands.Appointments
{
    public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, AppointmentDto>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IPatientClinicLinkRepository _patientClinicLinkRepository;
        private readonly IUserRepository _userRepository;
        private readonly IInAppNotificationService _notificationService;
        private readonly IMapper _mapper;

        public UpdateAppointmentCommandHandler(
            IAppointmentRepository appointmentRepository,
            IPatientRepository patientRepository,
            IClinicRepository clinicRepository,
            IPatientClinicLinkRepository patientClinicLinkRepository,
            IUserRepository userRepository,
            IInAppNotificationService notificationService,
            IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _clinicRepository = clinicRepository;
            _patientClinicLinkRepository = patientClinicLinkRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _mapper = mapper;
        }

        public async Task<AppointmentDto> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
        {
            // Get existing appointment
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, request.TenantId);
            if (appointment == null)
            {
                throw new InvalidOperationException("Appointment not found");
            }

            // Track if professional changed for notification
            var originalProfessionalId = appointment.ProfessionalId;

            // Only allow editing of Scheduled or Confirmed appointments
            if (appointment.Status != AppointmentStatus.Scheduled && appointment.Status != AppointmentStatus.Confirmed)
            {
                throw new InvalidOperationException("Only scheduled or confirmed appointments can be edited");
            }

            // Update professional if changed
            if (request.UpdateData.ProfessionalId != appointment.ProfessionalId)
            {
                appointment.UpdateProfessional(request.UpdateData.ProfessionalId);
            }

            // Update schedule if date or time changed
            if (appointment.ScheduledDate != request.UpdateData.ScheduledDate || 
                appointment.ScheduledTime != request.UpdateData.ScheduledTime)
            {
                appointment.Reschedule(request.UpdateData.ScheduledDate, request.UpdateData.ScheduledTime);
            }

            // Update duration if changed
            if (appointment.DurationMinutes != request.UpdateData.DurationMinutes)
            {
                appointment.UpdateDuration(request.UpdateData.DurationMinutes);
            }

            // Update type if changed
            if (!string.IsNullOrWhiteSpace(request.UpdateData.Type))
            {
                if (!Enum.TryParse<AppointmentType>(request.UpdateData.Type, true, out var appointmentType))
                {
                    throw new InvalidOperationException($"Invalid appointment type: {request.UpdateData.Type}");
                }
                
                if (appointment.Type != appointmentType)
                {
                    appointment.UpdateType(appointmentType);
                }
            }

            // Update notes if provided
            if (request.UpdateData.Notes != null)
            {
                appointment.UpdateNotes(request.UpdateData.Notes);
            }

            // Update room number if provided
            if (request.UpdateData.RoomNumber != null)
            {
                appointment.UpdateRoomNumber(request.UpdateData.RoomNumber);
            }

            // Update the appointment in the repository
            await _appointmentRepository.UpdateAsync(appointment);

            // Check if notification to primary doctor should be sent (if professional was changed or set)
            if (originalProfessionalId != appointment.ProfessionalId && appointment.ProfessionalId.HasValue)
            {
                await NotifyPrimaryDoctorIfNeededAsync(appointment, request.TenantId);
            }

            return _mapper.Map<AppointmentDto>(appointment);
        }

        private async Task NotifyPrimaryDoctorIfNeededAsync(Appointment appointment, string tenantId)
        {
            // Get clinic
            var clinic = await _clinicRepository.GetByIdAsync(appointment.ClinicId, tenantId);
            if (clinic == null || !clinic.NotifyPrimaryDoctorOnOtherDoctorAppointment)
                return;

            // Check if appointment has a professional assigned
            if (!appointment.ProfessionalId.HasValue)
                return;

            // Get patient
            var patient = await _patientRepository.GetByIdAsync(appointment.PatientId, tenantId);
            if (patient == null)
                return;

            // Get patient-clinic link to check primary doctor
            var patientClinicLink = await _patientClinicLinkRepository.GetLinkAsync(
                appointment.PatientId,
                appointment.ClinicId,
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
            var message = $"O paciente {patient.Name} foi agendado para consulta com Dr(a). {appointmentDoctor.Name} " +
                         $"em {appointment.ScheduledDate:dd/MM/yyyy} às {appointment.ScheduledTime:hh\\:mm}.";

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
                    AppointmentDoctorName = appointmentDoctor.Name,
                    PrimaryDoctorId = patientClinicLink.PrimaryDoctorId,
                    ScheduledDate = appointment.ScheduledDate,
                    ScheduledTime = appointment.ScheduledTime
                },
                tenantId: tenantId
            );
        }
    }
}
