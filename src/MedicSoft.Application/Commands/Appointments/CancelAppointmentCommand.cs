using MediatR;

namespace MedicSoft.Application.Commands.Appointments
{
    public class CancelAppointmentCommand : IRequest<bool>
    {
        public Guid AppointmentId { get; }
        public string CancellationReason { get; }
        public string TenantId { get; }

        public CancelAppointmentCommand(Guid appointmentId, string cancellationReason, string tenantId)
        {
            AppointmentId = appointmentId;
            CancellationReason = cancellationReason;
            TenantId = tenantId;
        }
    }
}