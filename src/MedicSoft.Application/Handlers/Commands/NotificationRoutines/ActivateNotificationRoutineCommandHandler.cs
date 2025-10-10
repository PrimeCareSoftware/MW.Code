using MediatR;
using MedicSoft.Application.Commands.NotificationRoutines;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.NotificationRoutines
{
    public class ActivateNotificationRoutineCommandHandler : IRequestHandler<ActivateNotificationRoutineCommand, bool>
    {
        private readonly INotificationRoutineRepository _repository;

        public ActivateNotificationRoutineCommandHandler(INotificationRoutineRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(ActivateNotificationRoutineCommand request, CancellationToken cancellationToken)
        {
            var routine = await _repository.GetByIdAsync(request.Id, request.TenantId);
            if (routine == null)
                return false;

            routine.Activate();
            await _repository.UpdateAsync(routine);
            return true;
        }
    }
}
