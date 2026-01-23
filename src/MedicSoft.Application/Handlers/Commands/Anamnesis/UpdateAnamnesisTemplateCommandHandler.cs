using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Anamnesis;
using MedicSoft.Application.DTOs.Anamnesis;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Anamnesis
{
    public class UpdateAnamnesisTemplateCommandHandler : IRequestHandler<UpdateAnamnesisTemplateCommand, AnamnesisTemplateDto>
    {
        private readonly IAnamnesisTemplateRepository _repository;
        private readonly IMapper _mapper;

        public UpdateAnamnesisTemplateCommandHandler(IAnamnesisTemplateRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AnamnesisTemplateDto> Handle(UpdateAnamnesisTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetByIdAsync(request.TemplateId, request.TenantId);
            
            if (template == null)
                throw new InvalidOperationException("Template não encontrado");

            template.Update(
                request.Template.Name,
                request.Template.Description,
                request.Template.Sections
            );

            await _repository.UpdateAsync(template);
            return _mapper.Map<AnamnesisTemplateDto>(template);
        }
    }
}
