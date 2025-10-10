using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.NotificationRoutines;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.NotificationRoutines
{
    public class UpdateNotificationRoutineCommandHandler : IRequestHandler<UpdateNotificationRoutineCommand, NotificationRoutineDto>
    {
        private readonly INotificationRoutineRepository _repository;
        private readonly IMapper _mapper;

        public UpdateNotificationRoutineCommandHandler(INotificationRoutineRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<NotificationRoutineDto> Handle(UpdateNotificationRoutineCommand request, CancellationToken cancellationToken)
        {
            var routine = await _repository.GetByIdAsync(request.Id, request.TenantId);
            if (routine == null)
                throw new InvalidOperationException("Notification routine not found");

            // Parse enums from string
            if (!Enum.TryParse<NotificationChannel>(request.Routine.Channel, true, out var channel))
                throw new ArgumentException($"Invalid notification channel: {request.Routine.Channel}");

            if (!Enum.TryParse<NotificationType>(request.Routine.Type, true, out var type))
                throw new ArgumentException($"Invalid notification type: {request.Routine.Type}");

            if (!Enum.TryParse<RoutineScheduleType>(request.Routine.ScheduleType, true, out var scheduleType))
                throw new ArgumentException($"Invalid schedule type: {request.Routine.ScheduleType}");

            // Update the routine
            routine.Update(
                request.Routine.Name,
                request.Routine.Description,
                channel,
                type,
                request.Routine.MessageTemplate,
                scheduleType,
                request.Routine.ScheduleConfiguration,
                request.Routine.MaxRetries,
                request.Routine.RecipientFilter
            );

            await _repository.UpdateAsync(routine);
            return _mapper.Map<NotificationRoutineDto>(routine);
        }
    }
}
