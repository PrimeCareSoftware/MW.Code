using MediatR;

namespace MedicSoft.Application.Commands.NotificationRoutines
{
    public class DeactivateNotificationRoutineCommand : IRequest<bool>
    {
        public Guid Id { get; }
        public string TenantId { get; }

        public DeactivateNotificationRoutineCommand(Guid id, string tenantId)
        {
            Id = id;
            TenantId = tenantId;
        }
    }
}
