using MediatR;

namespace MedicSoft.Application.Commands.NotificationRoutines
{
    public class DeleteNotificationRoutineCommand : IRequest<bool>
    {
        public Guid Id { get; }
        public string TenantId { get; }

        public DeleteNotificationRoutineCommand(Guid id, string tenantId)
        {
            Id = id;
            TenantId = tenantId;
        }
    }
}
