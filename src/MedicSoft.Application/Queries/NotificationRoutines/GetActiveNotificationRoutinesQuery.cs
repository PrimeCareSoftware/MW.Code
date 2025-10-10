using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.NotificationRoutines
{
    public class GetActiveNotificationRoutinesQuery : IRequest<IEnumerable<NotificationRoutineDto>>
    {
        public string TenantId { get; }

        public GetActiveNotificationRoutinesQuery(string tenantId)
        {
            TenantId = tenantId;
        }
    }
}
