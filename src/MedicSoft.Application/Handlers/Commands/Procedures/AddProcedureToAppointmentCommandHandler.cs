using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Procedures;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Procedures
{
    public class AddProcedureToAppointmentCommandHandler : IRequestHandler<AddProcedureToAppointmentCommand, AppointmentProcedureDto>
    {
        private readonly IAppointmentProcedureRepository _appointmentProcedureRepository;
        private readonly IProcedureRepository _procedureRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMapper _mapper;

        public AddProcedureToAppointmentCommandHandler(
            IAppointmentProcedureRepository appointmentProcedureRepository,
            IProcedureRepository procedureRepository,
            IAppointmentRepository appointmentRepository,
            IMapper mapper)
        {
            _appointmentProcedureRepository = appointmentProcedureRepository;
            _procedureRepository = procedureRepository;
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
        }

        public async Task<AppointmentProcedureDto> Handle(AddProcedureToAppointmentCommand request, CancellationToken cancellationToken)
        {
            // Validate appointment exists
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, request.TenantId);
            if (appointment == null)
            {
                throw new InvalidOperationException("Appointment not found");
            }

            // Validate procedure exists
            var procedure = await _procedureRepository.GetByIdAsync(request.ProcedureInfo.ProcedureId, request.TenantId);
            if (procedure == null)
            {
                throw new InvalidOperationException("Procedure not found");
            }

            // Use custom price if provided, otherwise use procedure's default price
            var priceCharged = request.ProcedureInfo.CustomPrice ?? procedure.Price;

            var appointmentProcedure = new AppointmentProcedure(
                request.AppointmentId,
                request.ProcedureInfo.ProcedureId,
                appointment.PatientId,
                priceCharged,
                DateTime.UtcNow,
                request.TenantId,
                request.ProcedureInfo.Notes
            );

            var created = await _appointmentProcedureRepository.AddAsync(appointmentProcedure);
            
            var dto = _mapper.Map<AppointmentProcedureDto>(created);
            dto.ProcedureName = procedure.Name;
            dto.ProcedureCode = procedure.Code;
            
            return dto;
        }
    }
}
