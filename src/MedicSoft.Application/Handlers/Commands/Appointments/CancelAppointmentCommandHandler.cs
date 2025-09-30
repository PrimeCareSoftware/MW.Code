using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Appointments;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Appointments
{
    public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, bool>
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public CancelAppointmentCommandHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<bool> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, request.TenantId);
            if (appointment == null)
            {
                return false;
            }

            appointment.Cancel(request.CancellationReason);
            await _appointmentRepository.UpdateAsync(appointment);
            
            return true;
        }
    }
}