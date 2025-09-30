using AutoMapper;
using MediatR;
using MedicSoft.Application.Queries.Appointments;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.Services;

namespace MedicSoft.Application.Handlers.Queries.Appointments
{
    public class GetAvailableSlotsQueryHandler : IRequestHandler<GetAvailableSlotsQuery, IEnumerable<AvailableSlotDto>>
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly AppointmentSchedulingService _schedulingService;

        public GetAvailableSlotsQueryHandler(
            IClinicRepository clinicRepository,
            AppointmentSchedulingService schedulingService)
        {
            _clinicRepository = clinicRepository;
            _schedulingService = schedulingService;
        }

        public async Task<IEnumerable<AvailableSlotDto>> Handle(GetAvailableSlotsQuery request, CancellationToken cancellationToken)
        {
            var clinic = await _clinicRepository.GetByIdAsync(request.ClinicId, request.TenantId);
            if (clinic == null)
            {
                throw new InvalidOperationException("Clinic not found");
            }

            var availableSlots = await _schedulingService.GetAvailableSlotsAsync(
                request.Date, request.ClinicId, request.DurationMinutes, request.TenantId);

            return availableSlots.Select(slot => new AvailableSlotDto
            {
                Date = request.Date,
                Time = slot,
                DurationMinutes = request.DurationMinutes,
                IsAvailable = true
            });
        }
    }
}