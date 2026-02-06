using AutoMapper;
using MediatR;
using MedicSoft.Application.Queries.Appointments;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.Services;
using MedicSoft.Application.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace MedicSoft.Application.Handlers.Queries.Appointments
{
    public class GetDailyAgendaQueryHandler : IRequestHandler<GetDailyAgendaQuery, DailyAgendaDto>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly AppointmentSchedulingService _schedulingService;
        private readonly IMapper _mapper;
        private readonly ICacheService _cacheService;

        public GetDailyAgendaQueryHandler(
            IAppointmentRepository appointmentRepository,
            IClinicRepository clinicRepository,
            AppointmentSchedulingService schedulingService,
            IMapper mapper,
            ICacheService cacheService)
        {
            _appointmentRepository = appointmentRepository;
            _clinicRepository = clinicRepository;
            _schedulingService = schedulingService;
            _mapper = mapper;
            _cacheService = cacheService;
        }

        public async Task<DailyAgendaDto> Handle(GetDailyAgendaQuery request, CancellationToken cancellationToken)
        {
            // Cache clinic data (rarely changes)
            var cacheKey = $"clinic:{request.ClinicId}";
            var clinic = await _cacheService.GetOrCreateAsync(cacheKey, async () =>
            {
                return await _clinicRepository.GetByIdAsync(request.ClinicId, request.TenantId);
            }, TimeSpan.FromHours(1));

            if (clinic == null)
            {
                throw new InvalidOperationException("Clinic not found");
            }

            // Use optimized repository method with Include and filtering at database level
            var appointments = await _appointmentRepository.GetDailyAgendaWithIncludesAsync(
                request.Date, 
                request.ClinicId, 
                request.TenantId,
                request.ProfessionalId);
            
            // Calculate available slots
            var availableSlots = await _schedulingService.GetAvailableSlotsAsync(
                request.Date, request.ClinicId, clinic.AppointmentDurationMinutes, request.TenantId);

            return new DailyAgendaDto
            {
                Date = request.Date,
                ClinicId = request.ClinicId,
                ClinicName = clinic.Name,
                Appointments = _mapper.Map<List<AppointmentDto>>(appointments),
                AvailableSlots = availableSlots.ToList()
            };
        }
    }
}