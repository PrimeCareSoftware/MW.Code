using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.NotificationRoutines;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.NotificationRoutines
{
    public class GetNotificationRoutineByIdQueryHandler : IRequestHandler<GetNotificationRoutineByIdQuery, NotificationRoutineDto?>
    {
        private readonly INotificationRoutineRepository _repository;
        private readonly IMapper _mapper;

        public GetNotificationRoutineByIdQueryHandler(INotificationRoutineRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<NotificationRoutineDto?> Handle(GetNotificationRoutineByIdQuery request, CancellationToken cancellationToken)
        {
            var routine = await _repository.GetByIdAsync(request.Id, request.TenantId);
            return routine != null ? _mapper.Map<NotificationRoutineDto>(routine) : null;
        }
    }
}
