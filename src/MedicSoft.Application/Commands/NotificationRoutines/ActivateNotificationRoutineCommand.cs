using MediatR;

namespace MedicSoft.Application.Commands.NotificationRoutines
{
    public class ActivateNotificationRoutineCommand : IRequest<bool>
    {
        public Guid Id { get; }
        public string TenantId { get; }

        public ActivateNotificationRoutineCommand(Guid id, string tenantId)
        {
            Id = id;
            TenantId = tenantId;
        }
    }
}
