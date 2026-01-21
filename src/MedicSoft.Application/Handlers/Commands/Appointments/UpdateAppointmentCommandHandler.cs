using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Appointments;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.Handlers.Commands.Appointments
{
    public class UpdateAppointmentCommandHandler : IRequestHandler<UpdateAppointmentCommand, AppointmentDto>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMapper _mapper;

        public UpdateAppointmentCommandHandler(
            IAppointmentRepository appointmentRepository,
            IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
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

            // Only allow editing of Scheduled or Confirmed appointments
            if (appointment.Status != AppointmentStatus.Scheduled && appointment.Status != AppointmentStatus.Confirmed)
            {
                throw new InvalidOperationException("Only scheduled or confirmed appointments can be edited");
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

            // Update the appointment in the repository
            await _appointmentRepository.UpdateAsync(appointment);

            return _mapper.Map<AppointmentDto>(appointment);
        }
    }
}
