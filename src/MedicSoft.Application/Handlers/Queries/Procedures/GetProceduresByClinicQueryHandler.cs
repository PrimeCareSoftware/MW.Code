using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Procedures;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Procedures
{
    public class GetProceduresByClinicQueryHandler : IRequestHandler<GetProceduresByClinicQuery, IEnumerable<ProcedureDto>>
    {
        private readonly IProcedureRepository _procedureRepository;
        private readonly IMapper _mapper;

        public GetProceduresByClinicQueryHandler(IProcedureRepository procedureRepository, IMapper mapper)
        {
            _procedureRepository = procedureRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<ProcedureDto>> Handle(GetProceduresByClinicQuery request, CancellationToken cancellationToken)
        {
            var procedures = await _procedureRepository.GetByClinicAsync(request.TenantId, request.ActiveOnly);
            return _mapper.Map<IEnumerable<ProcedureDto>>(procedures);
        }
    }
}
