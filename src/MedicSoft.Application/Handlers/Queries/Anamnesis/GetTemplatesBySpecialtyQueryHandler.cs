using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs.Anamnesis;
using MedicSoft.Application.Queries.Anamnesis;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Anamnesis
{
    public class GetTemplatesBySpecialtyQueryHandler : IRequestHandler<GetTemplatesBySpecialtyQuery, List<AnamnesisTemplateDto>>
    {
        private readonly IAnamnesisTemplateRepository _repository;
        private readonly IMapper _mapper;

        public GetTemplatesBySpecialtyQueryHandler(IAnamnesisTemplateRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<AnamnesisTemplateDto>> Handle(GetTemplatesBySpecialtyQuery request, CancellationToken cancellationToken)
        {
            var templates = await _repository.GetBySpecialtyAsync(request.Specialty, request.TenantId);
            return _mapper.Map<List<AnamnesisTemplateDto>>(templates);
        }
    }
}
