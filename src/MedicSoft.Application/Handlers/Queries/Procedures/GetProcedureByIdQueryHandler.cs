using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Procedures;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Procedures
{
    public class GetProcedureByIdQueryHandler : IRequestHandler<GetProcedureByIdQuery, ProcedureDto?>
    {
        private readonly IProcedureRepository _procedureRepository;
        private readonly IMapper _mapper;

        public GetProcedureByIdQueryHandler(IProcedureRepository procedureRepository, IMapper mapper)
        {
            _procedureRepository = procedureRepository;
            _mapper = mapper;
        }

        public async Task<ProcedureDto?> Handle(GetProcedureByIdQuery request, CancellationToken cancellationToken)
        {
            var procedure = await _procedureRepository.GetByIdAsync(request.ProcedureId, request.TenantId);
            return procedure == null ? null : _mapper.Map<ProcedureDto>(procedure);
        }
    }
}
