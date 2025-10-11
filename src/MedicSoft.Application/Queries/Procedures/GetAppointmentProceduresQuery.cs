using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Procedures
{
    public class GetAppointmentProceduresQuery : IRequest<IEnumerable<AppointmentProcedureDto>>
    {
        public Guid AppointmentId { get; }
        public string TenantId { get; }

        public GetAppointmentProceduresQuery(Guid appointmentId, string tenantId)
        {
            AppointmentId = appointmentId;
            TenantId = tenantId;
        }
    }
}
