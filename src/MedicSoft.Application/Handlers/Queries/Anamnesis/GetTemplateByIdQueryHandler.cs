using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs.Anamnesis;
using MedicSoft.Application.Queries.Anamnesis;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Anamnesis
{
    public class GetTemplateByIdQueryHandler : IRequestHandler<GetTemplateByIdQuery, AnamnesisTemplateDto?>
    {
        private readonly IAnamnesisTemplateRepository _repository;
        private readonly IMapper _mapper;

        public GetTemplateByIdQueryHandler(IAnamnesisTemplateRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AnamnesisTemplateDto?> Handle(GetTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetByIdAsync(request.TemplateId, request.TenantId);
            return template == null ? null : _mapper.Map<AnamnesisTemplateDto>(template);
        }
    }
}
