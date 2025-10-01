using AutoMapper;
using MediatR;
using MedicSoft.Application.Queries.Appointments;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.Services;

namespace MedicSoft.Application.Handlers.Queries.Appointments
{
    public class GetDailyAgendaQueryHandler : IRequestHandler<GetDailyAgendaQuery, DailyAgendaDto>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly AppointmentSchedulingService _schedulingService;
        private readonly IMapper _mapper;

        public GetDailyAgendaQueryHandler(
            IAppointmentRepository appointmentRepository,
            IClinicRepository clinicRepository,
            AppointmentSchedulingService schedulingService,
            IMapper mapper)
        {
            _appointmentRepository = appointmentRepository;
            _clinicRepository = clinicRepository;
            _schedulingService = schedulingService;
            _mapper = mapper;
        }

        public async Task<DailyAgendaDto> Handle(GetDailyAgendaQuery request, CancellationToken cancellationToken)
        {
            var clinic = await _clinicRepository.GetByIdAsync(request.ClinicId, request.TenantId);
            if (clinic == null)
            {
                throw new InvalidOperationException("Clinic not found");
            }

            var appointments = await _appointmentRepository.GetDailyAgendaAsync(request.Date, request.ClinicId, request.TenantId);
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