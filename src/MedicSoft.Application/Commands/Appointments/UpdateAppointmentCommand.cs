using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.Appointments
{
    public class UpdateAppointmentCommand : IRequest<AppointmentDto>
    {
        public Guid AppointmentId { get; }
        public UpdateAppointmentDto UpdateData { get; }
        public string TenantId { get; }

        public UpdateAppointmentCommand(Guid appointmentId, UpdateAppointmentDto updateData, string tenantId)
        {
            AppointmentId = appointmentId;
            UpdateData = updateData;
            TenantId = tenantId;
        }
    }
}
