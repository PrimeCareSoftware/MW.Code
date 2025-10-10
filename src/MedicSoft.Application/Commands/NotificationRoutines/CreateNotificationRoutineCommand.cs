using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.NotificationRoutines
{
    public class CreateNotificationRoutineCommand : IRequest<NotificationRoutineDto>
    {
        public CreateNotificationRoutineDto Routine { get; }
        public string TenantId { get; }

        public CreateNotificationRoutineCommand(CreateNotificationRoutineDto routine, string tenantId)
        {
            Routine = routine;
            TenantId = tenantId;
        }
    }
}
