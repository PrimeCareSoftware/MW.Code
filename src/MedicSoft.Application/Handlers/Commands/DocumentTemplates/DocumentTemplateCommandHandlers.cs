using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.DocumentTemplates;
using MedicSoft.Application.DTOs.DocumentTemplates;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.DocumentTemplates
{
    /// <summary>
    /// Handler for creating a new document template
    /// </summary>
    public class CreateDocumentTemplateCommandHandler : IRequestHandler<CreateDocumentTemplateCommand, DocumentTemplateDto>
    {
        private readonly IDocumentTemplateRepository _repository;
        private readonly IMapper _mapper;

        public CreateDocumentTemplateCommandHandler(IDocumentTemplateRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<DocumentTemplateDto> Handle(CreateDocumentTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = new DocumentTemplate(
                name: request.Template.Name,
                description: request.Template.Description,
                specialty: request.Template.Specialty,
                type: request.Template.Type,
                content: request.Template.Content,
                variables: request.Template.Variables,
                tenantId: request.TenantId,
                clinicId: request.ClinicId ?? request.Template.ClinicId,
                isSystem: false
            );

            var created = await _repository.AddAsync(template);
            return _mapper.Map<DocumentTemplateDto>(created);
        }
    }
    
    /// <summary>
    /// Handler for updating an existing document template
    /// </summary>
    public class UpdateDocumentTemplateCommandHandler : IRequestHandler<UpdateDocumentTemplateCommand, DocumentTemplateDto>
    {
        private readonly IDocumentTemplateRepository _repository;
        private readonly IMapper _mapper;

        public UpdateDocumentTemplateCommandHandler(IDocumentTemplateRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<DocumentTemplateDto> Handle(UpdateDocumentTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetByIdAsync(request.Id, request.TenantId);
            
            if (template == null)
            {
                throw new InvalidOperationException($"Template with ID {request.Id} not found");
            }

            template.Update(
                name: request.Template.Name,
                description: request.Template.Description,
                content: request.Template.Content,
                variables: request.Template.Variables
            );

            await _repository.UpdateAsync(template);
            return _mapper.Map<DocumentTemplateDto>(template);
        }
    }
    
    /// <summary>
    /// Handler for deleting a document template
    /// </summary>
    public class DeleteDocumentTemplateCommandHandler : IRequestHandler<DeleteDocumentTemplateCommand, Unit>
    {
        private readonly IDocumentTemplateRepository _repository;

        public DeleteDocumentTemplateCommandHandler(IDocumentTemplateRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Unit> Handle(DeleteDocumentTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetByIdAsync(request.Id, request.TenantId);
            
            if (template == null)
            {
                throw new InvalidOperationException($"Template with ID {request.Id} not found");
            }

            if (template.IsSystem)
            {
                throw new InvalidOperationException("Cannot delete system templates");
            }

            await _repository.DeleteAsync(request.Id, request.TenantId);
            return Unit.Value;
        }
    }
    
    /// <summary>
    /// Handler for activating a document template
    /// </summary>
    public class ActivateDocumentTemplateCommandHandler : IRequestHandler<ActivateDocumentTemplateCommand, Unit>
    {
        private readonly IDocumentTemplateRepository _repository;

        public ActivateDocumentTemplateCommandHandler(IDocumentTemplateRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Unit> Handle(ActivateDocumentTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetByIdAsync(request.Id, request.TenantId);
            
            if (template == null)
            {
                throw new InvalidOperationException($"Template with ID {request.Id} not found");
            }

            template.Activate();
            await _repository.UpdateAsync(template);
            return Unit.Value;
        }
    }
    
    /// <summary>
    /// Handler for deactivating a document template
    /// </summary>
    public class DeactivateDocumentTemplateCommandHandler : IRequestHandler<DeactivateDocumentTemplateCommand, Unit>
    {
        private readonly IDocumentTemplateRepository _repository;

        public DeactivateDocumentTemplateCommandHandler(IDocumentTemplateRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public async Task<Unit> Handle(DeactivateDocumentTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetByIdAsync(request.Id, request.TenantId);
            
            if (template == null)
            {
                throw new InvalidOperationException($"Template with ID {request.Id} not found");
            }

            template.Deactivate();
            await _repository.UpdateAsync(template);
            return Unit.Value;
        }
    }
    
    /// <summary>
    /// Handler for creating a document template from a global template
    /// </summary>
    public class CreateDocumentTemplateFromGlobalCommandHandler : IRequestHandler<CreateDocumentTemplateFromGlobalCommand, DocumentTemplateDto>
    {
        private readonly IDocumentTemplateRepository _repository;
        private readonly IGlobalDocumentTemplateRepository _globalRepository;
        private readonly IMapper _mapper;

        public CreateDocumentTemplateFromGlobalCommandHandler(
            IDocumentTemplateRepository repository,
            IGlobalDocumentTemplateRepository globalRepository,
            IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _globalRepository = globalRepository ?? throw new ArgumentNullException(nameof(globalRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<DocumentTemplateDto> Handle(CreateDocumentTemplateFromGlobalCommand request, CancellationToken cancellationToken)
        {
            var globalTemplate = await _globalRepository.GetByIdAsync(request.GlobalTemplateId, request.TenantId);
            
            if (globalTemplate == null)
            {
                throw new InvalidOperationException($"Template global com ID {request.GlobalTemplateId} não foi encontrado");
            }
            
            if (!globalTemplate.IsActive)
            {
                throw new InvalidOperationException($"Não é possível criar um template a partir de um template global inativo");
            }

            var template = new DocumentTemplate(
                name: globalTemplate.Name,
                description: globalTemplate.Description,
                specialty: globalTemplate.Specialty,
                type: globalTemplate.Type,
                content: globalTemplate.Content,
                variables: globalTemplate.Variables,
                tenantId: request.TenantId,
                clinicId: request.ClinicId,
                isSystem: false,
                globalTemplateId: request.GlobalTemplateId
            );

            var created = await _repository.AddAsync(template);
            return _mapper.Map<DocumentTemplateDto>(created);
        }
    }
}
