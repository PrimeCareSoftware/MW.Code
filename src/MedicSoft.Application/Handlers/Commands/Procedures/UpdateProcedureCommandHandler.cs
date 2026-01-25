using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Procedures;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Procedures
{
    public class UpdateProcedureCommandHandler : IRequestHandler<UpdateProcedureCommand, ProcedureDto>
    {
        private readonly IProcedureRepository _procedureRepository;
        private readonly IMapper _mapper;

        public UpdateProcedureCommandHandler(IProcedureRepository procedureRepository, IMapper mapper)
        {
            _procedureRepository = procedureRepository;
            _mapper = mapper;
        }

        public async Task<ProcedureDto> Handle(UpdateProcedureCommand request, CancellationToken cancellationToken)
        {
            var procedure = await _procedureRepository.GetByIdAsync(request.ProcedureId, request.TenantId);
            if (procedure == null)
            {
                throw new InvalidOperationException("Procedure not found");
            }

            procedure.Update(
                request.Procedure.Name,
                request.Procedure.Description,
                request.Procedure.Category,
                request.Procedure.Price,
                request.Procedure.DurationMinutes,
                request.Procedure.RequiresMaterials,
                request.Procedure.ClinicId,
                request.Procedure.AcceptedHealthInsurances,
                request.Procedure.AllowInMedicalAttendance,
                request.Procedure.AllowInExclusiveProcedureAttendance
            );

            await _procedureRepository.UpdateAsync(procedure);
            return _mapper.Map<ProcedureDto>(procedure);
        }
    }
}
