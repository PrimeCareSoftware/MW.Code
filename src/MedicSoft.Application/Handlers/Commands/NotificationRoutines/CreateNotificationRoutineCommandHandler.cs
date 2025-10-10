using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.NotificationRoutines;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.NotificationRoutines
{
    public class CreateNotificationRoutineCommandHandler : IRequestHandler<CreateNotificationRoutineCommand, NotificationRoutineDto>
    {
        private readonly INotificationRoutineRepository _repository;
        private readonly IMapper _mapper;

        public CreateNotificationRoutineCommandHandler(INotificationRoutineRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<NotificationRoutineDto> Handle(CreateNotificationRoutineCommand request, CancellationToken cancellationToken)
        {
            // Parse enums from string
            if (!Enum.TryParse<NotificationChannel>(request.Routine.Channel, true, out var channel))
                throw new ArgumentException($"Invalid notification channel: {request.Routine.Channel}");

            if (!Enum.TryParse<NotificationType>(request.Routine.Type, true, out var type))
                throw new ArgumentException($"Invalid notification type: {request.Routine.Type}");

            if (!Enum.TryParse<RoutineScheduleType>(request.Routine.ScheduleType, true, out var scheduleType))
                throw new ArgumentException($"Invalid schedule type: {request.Routine.ScheduleType}");

            if (!Enum.TryParse<RoutineScope>(request.Routine.Scope, true, out var scope))
                throw new ArgumentException($"Invalid scope: {request.Routine.Scope}");

            // Validate system scope for admin only
            if (scope == RoutineScope.System && request.TenantId != "system-admin")
                throw new UnauthorizedAccessException("Only system administrators can create system-wide routines");

            // Create the routine
            var routine = new NotificationRoutine(
                request.Routine.Name,
                request.Routine.Description,
                channel,
                type,
                request.Routine.MessageTemplate,
                scheduleType,
                request.Routine.ScheduleConfiguration,
                scope,
                request.TenantId,
                request.Routine.MaxRetries,
                request.Routine.RecipientFilter
            );

            var created = await _repository.AddAsync(routine);
            return _mapper.Map<NotificationRoutineDto>(created);
        }
    }
}
