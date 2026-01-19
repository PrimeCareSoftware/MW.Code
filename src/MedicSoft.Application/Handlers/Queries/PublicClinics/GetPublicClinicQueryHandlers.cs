using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.PublicClinics;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.PublicClinics
{
    /// <summary>
    /// Handler para obter detalhes públicos de uma clínica específica.
    /// Retorna apenas informações públicas, sem dados sensíveis.
    /// </summary>
    public class GetPublicClinicByIdQueryHandler : IRequestHandler<GetPublicClinicByIdQuery, PublicClinicDto?>
    {
        private readonly IClinicRepository _clinicRepository;

        public GetPublicClinicByIdQueryHandler(IClinicRepository clinicRepository)
        {
            _clinicRepository = clinicRepository;
        }

        public async Task<PublicClinicDto?> Handle(GetPublicClinicByIdQuery request, CancellationToken cancellationToken)
        {
            // Busca a clínica sem filtro de tenant (é público)
            var clinic = await _clinicRepository.GetByIdWithoutTenantAsync(request.ClinicId);

            if (clinic == null || !clinic.IsActive)
                return null;

            // Retorna apenas dados públicos
            return new PublicClinicDto
            {
                Id = clinic.Id,
                Name = clinic.Name,
                TradeName = clinic.TradeName,
                Phone = clinic.Phone,
                Email = clinic.Email,
                Address = clinic.Address,
                City = ExtractCity(clinic.Address),
                State = ExtractState(clinic.Address),
                OpeningTime = clinic.OpeningTime,
                ClosingTime = clinic.ClosingTime,
                AppointmentDurationMinutes = clinic.AppointmentDurationMinutes,
                IsAcceptingNewPatients = clinic.IsActive && clinic.AllowEmergencySlots
            };
        }

        private string ExtractCity(string address)
        {
            try
            {
                var parts = address.Split(',');
                if (parts.Length >= 3)
                {
                    var cityStatePart = parts[^2].Trim();
                    var cityParts = cityStatePart.Split('-');
                    if (cityParts.Length >= 1)
                    {
                        return cityParts[0].Trim();
                    }
                }
            }
            catch { }
            return string.Empty;
        }

        private string ExtractState(string address)
        {
            try
            {
                var parts = address.Split(',');
                if (parts.Length >= 3)
                {
                    var cityStatePart = parts[^2].Trim();
                    var cityParts = cityStatePart.Split('-');
                    if (cityParts.Length >= 2)
                    {
                        return cityParts[1].Trim();
                    }
                }
            }
            catch { }
            return string.Empty;
        }
    }

    /// <summary>
    /// Handler para obter horários disponíveis públicos de uma clínica.
    /// Não requer autenticação.
    /// </summary>
    public class GetPublicAvailableSlotsQueryHandler : IRequestHandler<GetPublicAvailableSlotsQuery, List<AvailableSlotDto>>
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IAppointmentRepository _appointmentRepository;

        public GetPublicAvailableSlotsQueryHandler(
            IClinicRepository clinicRepository,
            IAppointmentRepository appointmentRepository)
        {
            _clinicRepository = clinicRepository;
            _appointmentRepository = appointmentRepository;
        }

        public async Task<List<AvailableSlotDto>> Handle(GetPublicAvailableSlotsQuery request, CancellationToken cancellationToken)
        {
            // Busca a clínica sem filtro de tenant
            var clinic = await _clinicRepository.GetByIdWithoutTenantAsync(request.ClinicId);

            if (clinic == null || !clinic.IsActive)
                return new List<AvailableSlotDto>();

            // Busca horários disponíveis usando o tenantId da clínica
            var availableTimeSlots = await _appointmentRepository.GetAvailableSlotsAsync(
                request.Date,
                request.ClinicId,
                request.DurationMinutes,
                clinic.TenantId
            );

            // Converte para DTO
            return availableTimeSlots.Select(time => new AvailableSlotDto
            {
                Date = request.Date,
                Time = time,
                DurationMinutes = request.DurationMinutes,
                IsAvailable = true
            }).ToList();
        }
    }
}
