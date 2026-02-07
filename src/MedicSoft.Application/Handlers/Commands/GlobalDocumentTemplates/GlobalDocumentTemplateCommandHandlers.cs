using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.GlobalDocumentTemplates;
using MedicSoft.Application.DTOs.GlobalDocumentTemplates;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.GlobalDocumentTemplates
{
    /// <summary>
    /// Handler for creating a new global document template
    /// </summary>
    public class CreateGlobalTemplateCommandHandler : IRequestHandler<CreateGlobalTemplateCommand, GlobalDocumentTemplateDto>
    {
        private readonly IGlobalDocumentTemplateRepository _repository;
        private readonly IMapper _mapper;

        public CreateGlobalTemplateCommandHandler(IGlobalDocumentTemplateRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<GlobalDocumentTemplateDto> Handle(CreateGlobalTemplateCommand request, CancellationToken cancellationToken)
        {
            // Check if template with same name and type already exists
            var exists = await _repository.ExistsByNameAndTypeAsync(
                request.Template.Name, 
                request.Template.Type, 
                request.TenantId
            );
            
            if (exists)
            {
                throw new InvalidOperationException(
                    $"Um template global com o nome '{request.Template.Name}' e tipo '{request.Template.Type}' já existe"
                );
            }

            var template = new GlobalDocumentTemplate(
                name: request.Template.Name,
                description: request.Template.Description,
                type: request.Template.Type,
                specialty: request.Template.Specialty,
                content: request.Template.Content,
                variables: request.Template.Variables,
                tenantId: request.TenantId,
                createdBy: request.CreatedBy
            );

            var created = await _repository.AddAsync(template);
            return _mapper.Map<GlobalDocumentTemplateDto>(created);
        }
    }
    
    /// <summary>
    /// Handler for updating an existing global document template
    /// </summary>
    public class UpdateGlobalTemplateCommandHandler : IRequestHandler<UpdateGlobalTemplateCommand, GlobalDocumentTemplateDto>
    {
        private readonly IGlobalDocumentTemplateRepository _repository;
        private readonly IMapper _mapper;

        public UpdateGlobalTemplateCommandHandler(IGlobalDocumentTemplateRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<GlobalDocumentTemplateDto> Handle(UpdateGlobalTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetByIdAsync(request.Id, request.TenantId);
            
            if (template == null)
            {
                throw new InvalidOperationException($"Template global com ID {request.Id} não foi encontrado");
            }

            template.Update(
                name: request.Template.Name,
                description: request.Template.Description,
                content: request.Template.Content,
                variables: request.Template.Variables
            );
            
            template.SetActiveStatus(request.Template.IsActive);

            await _repository.UpdateAsync(template);
            return _mapper.Map<GlobalDocumentTemplateDto>(template);
        }
    }
    
    /// <summary>
    /// Handler for deleting a global document template
    /// </summary>
    public class DeleteGlobalTemplateCommandHandler : IRequestHandler<DeleteGlobalTemplateCommand, Unit>
    {
        private readonly IGlobalDocumentTemplateRepository _repository;
        private readonly IDocumentTemplateRepository _documentTemplateRepository;

        public DeleteGlobalTemplateCommandHandler(
            IGlobalDocumentTemplateRepository repository,
            IDocumentTemplateRepository documentTemplateRepository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _documentTemplateRepository = documentTemplateRepository ?? throw new ArgumentNullException(nameof(documentTemplateRepository));
        }

        public async Task<Unit> Handle(DeleteGlobalTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetByIdAsync(request.Id, request.TenantId);
            
            if (template == null)
            {
                throw new InvalidOperationException($"Template global com ID {request.Id} não foi encontrado");
            }

            // Check if any document templates are using this global template
            var usedTemplates = await _documentTemplateRepository.FindAsync(
                dt => dt.GlobalTemplateId == request.Id,
                request.TenantId
            );
            
            if (usedTemplates.Any())
            {
                throw new InvalidOperationException(
                    $"Não é possível excluir este template global porque {usedTemplates.Count()} template(s) da clínica estão usando ele como base"
                );
            }

            await _repository.DeleteAsync(request.Id, request.TenantId);
            return Unit.Value;
        }
    }
    
    /// <summary>
    /// Handler for setting active status of a global document template
    /// </summary>
    public class SetGlobalTemplateActiveStatusCommandHandler : IRequestHandler<SetGlobalTemplateActiveStatusCommand, Unit>
    {
        private readonly IGlobalDocumentTemplateRepository _repository;

        public SetGlobalTemplateActiveStatusCommandHandler(IGlobalDocumentTemplateRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Unit> Handle(SetGlobalTemplateActiveStatusCommand request, CancellationToken cancellationToken)
        {
            await _repository.SetActiveStatusAsync(request.Id, request.IsActive, request.TenantId);
            return Unit.Value;
        }
    }
}
