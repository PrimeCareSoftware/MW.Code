using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Payments
{
    public class GetAppointmentPaymentsQuery : IRequest<List<PaymentDto>>
    {
        public Guid AppointmentId { get; }
        public string TenantId { get; }

        public GetAppointmentPaymentsQuery(Guid appointmentId, string tenantId)
        {
            AppointmentId = appointmentId;
            TenantId = tenantId;
        }
    }
}
