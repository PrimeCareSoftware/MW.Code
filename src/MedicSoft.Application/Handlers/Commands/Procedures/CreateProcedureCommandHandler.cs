using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Procedures;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Procedures
{
    public class CreateProcedureCommandHandler : IRequestHandler<CreateProcedureCommand, ProcedureDto>
    {
        private readonly IProcedureRepository _procedureRepository;
        private readonly IMapper _mapper;

        public CreateProcedureCommandHandler(IProcedureRepository procedureRepository, IMapper mapper)
        {
            _procedureRepository = procedureRepository;
            _mapper = mapper;
        }

        public async Task<ProcedureDto> Handle(CreateProcedureCommand request, CancellationToken cancellationToken)
        {
            // Check if code already exists
            if (await _procedureRepository.IsCodeUniqueAsync(request.Procedure.Code, request.TenantId) == false)
            {
                throw new InvalidOperationException("A procedure with this code already exists");
            }

            var procedure = new Procedure(
                request.Procedure.Name,
                request.Procedure.Code,
                request.Procedure.Description,
                request.Procedure.Category,
                request.Procedure.Price,
                request.Procedure.DurationMinutes,
                request.TenantId,
                request.Procedure.RequiresMaterials
            );

            var createdProcedure = await _procedureRepository.AddAsync(procedure);
            return _mapper.Map<ProcedureDto>(createdProcedure);
        }
    }
}
