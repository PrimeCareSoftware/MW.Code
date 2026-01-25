using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Clinics;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Clinics
{
    public class UpdateClinicCommandHandler : IRequestHandler<UpdateClinicCommand, ClinicDto>
    {
        private readonly IClinicRepository _clinicRepository;
        private readonly IMapper _mapper;

        public UpdateClinicCommandHandler(IClinicRepository clinicRepository, IMapper mapper)
        {
            _clinicRepository = clinicRepository;
            _mapper = mapper;
        }

        public async Task<ClinicDto> Handle(UpdateClinicCommand request, CancellationToken cancellationToken)
        {
            var clinic = await _clinicRepository.GetByIdAsync(request.ClinicId, request.TenantId);
            
            if (clinic == null)
                throw new InvalidOperationException($"Clínica com ID {request.ClinicId} não encontrada.");

            // Update clinic information
            clinic.UpdateInfo(
                request.Clinic.Name,
                request.Clinic.TradeName,
                request.Clinic.Phone,
                request.Clinic.Email,
                request.Clinic.Address
            );

            // Update schedule settings
            clinic.UpdateScheduleSettings(
                request.Clinic.OpeningTime,
                request.Clinic.ClosingTime,
                request.Clinic.AppointmentDurationMinutes,
                request.Clinic.AllowEmergencySlots
            );

            // Update optional fields if provided
            if (request.Clinic.NumberOfRooms.HasValue)
            {
                clinic.UpdateNumberOfRooms(request.Clinic.NumberOfRooms.Value);
            }

            if (request.Clinic.NotifyPrimaryDoctorOnOtherDoctorAppointment.HasValue)
            {
                clinic.UpdateNotifyPrimaryDoctorSetting(request.Clinic.NotifyPrimaryDoctorOnOtherDoctorAppointment.Value);
            }

            await _clinicRepository.UpdateAsync(clinic);

            return _mapper.Map<ClinicDto>(clinic);
        }
    }
}
