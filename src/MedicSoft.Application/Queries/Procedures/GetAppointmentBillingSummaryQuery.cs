using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Procedures
{
    public class GetAppointmentBillingSummaryQuery : IRequest<AppointmentBillingSummaryDto?>
    {
        public Guid AppointmentId { get; }
        public string TenantId { get; }

        public GetAppointmentBillingSummaryQuery(Guid appointmentId, string tenantId)
        {
            AppointmentId = appointmentId;
            TenantId = tenantId;
        }
    }
}
