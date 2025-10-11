using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Procedures;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Procedures
{
    public class GetAppointmentProceduresQueryHandler : IRequestHandler<GetAppointmentProceduresQuery, IEnumerable<AppointmentProcedureDto>>
    {
        private readonly IAppointmentProcedureRepository _appointmentProcedureRepository;
        private readonly IMapper _mapper;

        public GetAppointmentProceduresQueryHandler(IAppointmentProcedureRepository appointmentProcedureRepository, IMapper mapper)
        {
            _appointmentProcedureRepository = appointmentProcedureRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<AppointmentProcedureDto>> Handle(GetAppointmentProceduresQuery request, CancellationToken cancellationToken)
        {
            var procedures = await _appointmentProcedureRepository.GetByAppointmentIdAsync(request.AppointmentId, request.TenantId);
            
            var dtos = procedures.Select(ap => new AppointmentProcedureDto
            {
                Id = ap.Id,
                AppointmentId = ap.AppointmentId,
                ProcedureId = ap.ProcedureId,
                PatientId = ap.PatientId,
                ProcedureName = ap.Procedure?.Name ?? string.Empty,
                ProcedureCode = ap.Procedure?.Code ?? string.Empty,
                PriceCharged = ap.PriceCharged,
                Notes = ap.Notes,
                PerformedAt = ap.PerformedAt
            });

            return dtos;
        }
    }
}
