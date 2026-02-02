using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Appointments;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Appointments
{
    public class GetBlockedTimeSlotsQueryHandler : IRequestHandler<GetBlockedTimeSlotsQuery, IEnumerable<BlockedTimeSlotDto>>
    {
        private readonly IBlockedTimeSlotRepository _repository;
        private readonly IMapper _mapper;

        public GetBlockedTimeSlotsQueryHandler(IBlockedTimeSlotRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BlockedTimeSlotDto>> Handle(GetBlockedTimeSlotsQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Domain.Entities.BlockedTimeSlot> blockedSlots;
            
            if (request.ProfessionalId.HasValue)
            {
                blockedSlots = await _repository.GetByProfessionalAsync(request.ProfessionalId.Value, request.Date, request.TenantId);
            }
            else
            {
                blockedSlots = await _repository.GetByDateAsync(request.Date, request.ClinicId, request.TenantId);
            }

            return _mapper.Map<IEnumerable<BlockedTimeSlotDto>>(blockedSlots);
        }
    }

    public class GetBlockedTimeSlotsRangeQueryHandler : IRequestHandler<GetBlockedTimeSlotsRangeQuery, IEnumerable<BlockedTimeSlotDto>>
    {
        private readonly IBlockedTimeSlotRepository _repository;
        private readonly IMapper _mapper;

        public GetBlockedTimeSlotsRangeQueryHandler(IBlockedTimeSlotRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<BlockedTimeSlotDto>> Handle(GetBlockedTimeSlotsRangeQuery request, CancellationToken cancellationToken)
        {
            var blockedSlots = await _repository.GetByDateRangeAsync(request.StartDate, request.EndDate, request.ClinicId, request.TenantId);
            return _mapper.Map<IEnumerable<BlockedTimeSlotDto>>(blockedSlots);
        }
    }

    public class GetBlockedTimeSlotByIdQueryHandler : IRequestHandler<GetBlockedTimeSlotByIdQuery, BlockedTimeSlotDto?>
    {
        private readonly IBlockedTimeSlotRepository _repository;
        private readonly IMapper _mapper;

        public GetBlockedTimeSlotByIdQueryHandler(IBlockedTimeSlotRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<BlockedTimeSlotDto?> Handle(GetBlockedTimeSlotByIdQuery request, CancellationToken cancellationToken)
        {
            var blockedSlot = await _repository.GetByIdAsync(request.Id, request.TenantId);
            return blockedSlot != null ? _mapper.Map<BlockedTimeSlotDto>(blockedSlot) : null;
        }
    }
}
