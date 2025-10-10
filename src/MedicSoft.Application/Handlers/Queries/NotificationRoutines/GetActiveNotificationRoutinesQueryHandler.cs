using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.NotificationRoutines;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.NotificationRoutines
{
    public class GetActiveNotificationRoutinesQueryHandler : IRequestHandler<GetActiveNotificationRoutinesQuery, IEnumerable<NotificationRoutineDto>>
    {
        private readonly INotificationRoutineRepository _repository;
        private readonly IMapper _mapper;

        public GetActiveNotificationRoutinesQueryHandler(INotificationRoutineRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NotificationRoutineDto>> Handle(GetActiveNotificationRoutinesQuery request, CancellationToken cancellationToken)
        {
            var routines = await _repository.GetActiveRoutinesByTenantAsync(request.TenantId);
            return _mapper.Map<IEnumerable<NotificationRoutineDto>>(routines);
        }
    }
}
