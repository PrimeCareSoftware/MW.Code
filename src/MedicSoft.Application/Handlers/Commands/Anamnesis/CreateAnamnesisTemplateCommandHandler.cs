using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Anamnesis;
using MedicSoft.Application.DTOs.Anamnesis;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Anamnesis
{
    public class CreateAnamnesisTemplateCommandHandler : IRequestHandler<CreateAnamnesisTemplateCommand, AnamnesisTemplateDto>
    {
        private readonly IAnamnesisTemplateRepository _repository;
        private readonly IMapper _mapper;

        public CreateAnamnesisTemplateCommandHandler(IAnamnesisTemplateRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AnamnesisTemplateDto> Handle(CreateAnamnesisTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = new AnamnesisTemplate(
                request.Template.Name,
                request.Template.Specialty,
                request.Template.Sections,
                request.TenantId,
                request.CreatedBy,
                request.Template.Description,
                request.Template.IsDefault
            );

            // If this is set as default, unset other defaults for this specialty
            if (request.Template.IsDefault)
            {
                var existingDefault = await _repository.GetDefaultTemplateAsync(request.Template.Specialty, request.TenantId);
                if (existingDefault != null)
                {
                    existingDefault.RemoveAsDefault();
                    await _repository.UpdateAsync(existingDefault);
                }
            }

            var created = await _repository.AddAsync(template);
            return _mapper.Map<AnamnesisTemplateDto>(created);
        }
    }
}
