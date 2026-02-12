using AutoMapper;
using MediatR;
using MedicSoft.Application.Queries.Appointments;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Application.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Appointments
{
    public class GetWeekAgendaQueryHandler : IRequestHandler<GetWeekAgendaQuery, WeekAgendaDto>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public GetWeekAgendaQueryHandler(
            IAppointmentRepository appointmentRepository,
            IClinicRepository clinicRepository,
            IMapper mapper,
            ICacheService cacheService)
        {
            _appointmentRepository = appointmentRepository;
            _clinicRepository = clinicRepository;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<WeekAgendaDto> Handle(GetWeekAgendaQuery request, CancellationToken cancellationToken)
        {
            // Cache only clinic name (string) instead of full entity to avoid deserialization issues
            var cacheKey = $"clinic:name:{request.ClinicId}";
            var clinicName = await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                var clinic = await _clinicRepository.GetByIdAsync(request.ClinicId, request.TenantId);
                if (clinic == null)
                    throw new InvalidOperationException("Clinic not found");
                
                // Return only string (easy to serialize/deserialize)
                return clinic.Name;
            }, TimeSpan.FromHours(1));

            // Use optimized repository method to get appointments for the entire week in one query
            var appointments = await _appointmentRepository.GetWeekAgendaWithIncludesAsync(
                request.StartDate, 
                request.EndDate,
                request.ClinicId, 
                request.TenantId,
                request.ProfessionalId);

            return new WeekAgendaDto
            {
                StartDate = request.StartDate,
                EndDate = request.EndDate,
                ClinicId = request.ClinicId,
                ClinicName = clinicName,
                Appointments = _mapper.Map<List<AppointmentDto>>(appointments)
            };
        }
    }
}
