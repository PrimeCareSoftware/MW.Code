using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs.DocumentTemplates;
using MedicSoft.Application.Queries.DocumentTemplates;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.DocumentTemplates
{
    /// <summary>
    /// Handler for getting all document templates
    /// </summary>
    public class GetAllDocumentTemplatesQueryHandler : IRequestHandler<GetAllDocumentTemplatesQuery, List<DocumentTemplateDto>>
    {
        private readonly IDocumentTemplateRepository _repository;
        private readonly IMapper _mapper;

        public GetAllDocumentTemplatesQueryHandler(IDocumentTemplateRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<DocumentTemplateDto>> Handle(GetAllDocumentTemplatesQuery request, CancellationToken cancellationToken)
        {
            var templates = await _repository.GetAllAsync(request.TenantId);
            
            var templateList = templates.ToList();
            
            // Apply filters if provided
            if (request.Filter != null)
            {
                if (request.Filter.Specialty.HasValue)
                {
                    templateList = templateList.Where(t => t.Specialty == request.Filter.Specialty.Value).ToList();
                }
                
                if (request.Filter.Type.HasValue)
                {
                    templateList = templateList.Where(t => t.Type == request.Filter.Type.Value).ToList();
                }
                
                if (request.Filter.IsActive.HasValue)
                {
                    templateList = templateList.Where(t => t.IsActive == request.Filter.IsActive.Value).ToList();
                }
                
                if (request.Filter.IsSystem.HasValue)
                {
                    templateList = templateList.Where(t => t.IsSystem == request.Filter.IsSystem.Value).ToList();
                }
                
                if (request.Filter.ClinicId.HasValue)
                {
                    templateList = templateList.Where(t => t.ClinicId == request.Filter.ClinicId.Value).ToList();
                }
            }
            
            return _mapper.Map<List<DocumentTemplateDto>>(templateList);
        }
    }
    
    /// <summary>
    /// Handler for getting a document template by ID
    /// </summary>
    public class GetDocumentTemplateByIdQueryHandler : IRequestHandler<GetDocumentTemplateByIdQuery, DocumentTemplateDto?>
    {
        private readonly IDocumentTemplateRepository _repository;
        private readonly IMapper _mapper;

        public GetDocumentTemplateByIdQueryHandler(IDocumentTemplateRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<DocumentTemplateDto?> Handle(GetDocumentTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetByIdAsync(request.Id, request.TenantId);
            return template == null ? null : _mapper.Map<DocumentTemplateDto>(template);
        }
    }
    
    /// <summary>
    /// Handler for getting templates by specialty
    /// </summary>
    public class GetTemplatesBySpecialtyQueryHandler : IRequestHandler<GetTemplatesBySpecialtyQuery, List<DocumentTemplateDto>>
    {
        private readonly IDocumentTemplateRepository _repository;
        private readonly IMapper _mapper;

        public GetTemplatesBySpecialtyQueryHandler(IDocumentTemplateRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<DocumentTemplateDto>> Handle(GetTemplatesBySpecialtyQuery request, CancellationToken cancellationToken)
        {
            var templates = request.ActiveOnly
                ? await _repository.GetActiveTemplatesAsync(request.Specialty, request.TenantId)
                : await _repository.GetBySpecialtyAsync(request.Specialty, request.TenantId);
            
            return _mapper.Map<List<DocumentTemplateDto>>(templates);
        }
    }
    
    /// <summary>
    /// Handler for getting templates by type
    /// </summary>
    public class GetTemplatesByTypeQueryHandler : IRequestHandler<GetTemplatesByTypeQuery, List<DocumentTemplateDto>>
    {
        private readonly IDocumentTemplateRepository _repository;
        private readonly IMapper _mapper;

        public GetTemplatesByTypeQueryHandler(IDocumentTemplateRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<DocumentTemplateDto>> Handle(GetTemplatesByTypeQuery request, CancellationToken cancellationToken)
        {
            var templates = await _repository.GetByTypeAsync(request.Type, request.TenantId);
            return _mapper.Map<List<DocumentTemplateDto>>(templates);
        }
    }
    
    /// <summary>
    /// Handler for getting templates by clinic
    /// </summary>
    public class GetTemplatesByClinicQueryHandler : IRequestHandler<GetTemplatesByClinicQuery, List<DocumentTemplateDto>>
    {
        private readonly IDocumentTemplateRepository _repository;
        private readonly IMapper _mapper;

        public GetTemplatesByClinicQueryHandler(IDocumentTemplateRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<DocumentTemplateDto>> Handle(GetTemplatesByClinicQuery request, CancellationToken cancellationToken)
        {
            var templates = await _repository.GetByClinicIdAsync(request.ClinicId, request.TenantId);
            return _mapper.Map<List<DocumentTemplateDto>>(templates);
        }
    }
}
