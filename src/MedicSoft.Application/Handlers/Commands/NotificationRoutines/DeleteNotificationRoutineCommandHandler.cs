using MediatR;
using MedicSoft.Application.Commands.NotificationRoutines;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.NotificationRoutines
{
    public class DeleteNotificationRoutineCommandHandler : IRequestHandler<DeleteNotificationRoutineCommand, bool>
    {
        private readonly INotificationRoutineRepository _repository;

        public DeleteNotificationRoutineCommandHandler(INotificationRoutineRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteNotificationRoutineCommand request, CancellationToken cancellationToken)
        {
            var routine = await _repository.GetByIdAsync(request.Id, request.TenantId);
            if (routine == null)
                return false;

            await _repository.DeleteAsync(request.Id, request.TenantId);
            return true;
        }
    }
}
