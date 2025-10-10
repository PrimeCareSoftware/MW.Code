using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.NotificationRoutines
{
    public class GetNotificationRoutineByIdQuery : IRequest<NotificationRoutineDto?>
    {
        public Guid Id { get; }
        public string TenantId { get; }

        public GetNotificationRoutineByIdQuery(Guid id, string tenantId)
        {
            Id = id;
            TenantId = tenantId;
        }
    }
}
