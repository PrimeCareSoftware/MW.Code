using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.NotificationRoutines;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.NotificationRoutines
{
    public class GetAllNotificationRoutinesQueryHandler : IRequestHandler<GetAllNotificationRoutinesQuery, IEnumerable<NotificationRoutineDto>>
    {
        private readonly INotificationRoutineRepository _repository;
        private readonly IMapper _mapper;

        public GetAllNotificationRoutinesQueryHandler(INotificationRoutineRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<NotificationRoutineDto>> Handle(GetAllNotificationRoutinesQuery request, CancellationToken cancellationToken)
        {
            var routines = await _repository.GetByTenantAsync(request.TenantId);
            return _mapper.Map<IEnumerable<NotificationRoutineDto>>(routines);
        }
    }
}
