using MediatR;
using MedicSoft.Application.Commands.NotificationRoutines;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.NotificationRoutines
{
    public class DeactivateNotificationRoutineCommandHandler : IRequestHandler<DeactivateNotificationRoutineCommand, bool>
    {
        private readonly INotificationRoutineRepository _repository;

        public DeactivateNotificationRoutineCommandHandler(INotificationRoutineRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeactivateNotificationRoutineCommand request, CancellationToken cancellationToken)
        {
            var routine = await _repository.GetByIdAsync(request.Id, request.TenantId);
            if (routine == null)
                return false;

            routine.Deactivate();
            await _repository.UpdateAsync(routine);
            return true;
        }
    }
}
