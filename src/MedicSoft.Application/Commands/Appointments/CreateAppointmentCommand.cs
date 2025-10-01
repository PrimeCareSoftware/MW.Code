using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.Appointments
{
    public class CreateAppointmentCommand : IRequest<AppointmentDto>
    {
        public CreateAppointmentDto Appointment { get; }
        public string TenantId { get; }

        public CreateAppointmentCommand(CreateAppointmentDto appointment, string tenantId)
        {
            Appointment = appointment;
            TenantId = tenantId;
        }
    }
}