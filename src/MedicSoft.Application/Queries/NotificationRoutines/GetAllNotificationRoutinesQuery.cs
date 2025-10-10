using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.NotificationRoutines
{
    public class GetAllNotificationRoutinesQuery : IRequest<IEnumerable<NotificationRoutineDto>>
    {
        public string TenantId { get; }

        public GetAllNotificationRoutinesQuery(string tenantId)
        {
            TenantId = tenantId;
        }
    }
}
