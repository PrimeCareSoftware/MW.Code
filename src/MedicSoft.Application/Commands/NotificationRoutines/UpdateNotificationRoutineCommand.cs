using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.NotificationRoutines
{
    public class UpdateNotificationRoutineCommand : IRequest<NotificationRoutineDto>
    {
        public Guid Id { get; }
        public UpdateNotificationRoutineDto Routine { get; }
        public string TenantId { get; }

        public UpdateNotificationRoutineCommand(Guid id, UpdateNotificationRoutineDto routine, string tenantId)
        {
            Id = id;
            Routine = routine;
            TenantId = tenantId;
        }
    }
}
