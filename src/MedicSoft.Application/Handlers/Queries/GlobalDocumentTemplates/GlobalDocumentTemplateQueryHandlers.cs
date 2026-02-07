using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs.GlobalDocumentTemplates;
using MedicSoft.Application.Queries.GlobalDocumentTemplates;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.GlobalDocumentTemplates
{
    /// <summary>
    /// Handler for getting all global document templates
    /// </summary>
    public class GetAllGlobalTemplatesQueryHandler : IRequestHandler<GetAllGlobalTemplatesQuery, List<GlobalDocumentTemplateDto>>
    {
        private readonly IGlobalDocumentTemplateRepository _repository;
        private readonly IMapper _mapper;

        public GetAllGlobalTemplatesQueryHandler(IGlobalDocumentTemplateRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<GlobalDocumentTemplateDto>> Handle(GetAllGlobalTemplatesQuery request, CancellationToken cancellationToken)
        {
            var templates = await _repository.GetAllAsync(request.TenantId);
            var templateList = templates.ToList();
            
            // Apply filters if provided
            if (request.Filter != null)
            {
                if (request.Filter.Type.HasValue)
                {
                    templateList = templateList.Where(t => t.Type == request.Filter.Type.Value).ToList();
                }
                
                if (request.Filter.Specialty.HasValue)
                {
                    templateList = templateList.Where(t => t.Specialty == request.Filter.Specialty.Value).ToList();
                }
                
                if (request.Filter.IsActive.HasValue)
                {
                    templateList = templateList.Where(t => t.IsActive == request.Filter.IsActive.Value).ToList();
                }
                
                if (!string.IsNullOrWhiteSpace(request.Filter.SearchTerm))
                {
                    var searchTerm = request.Filter.SearchTerm.ToLower();
                    templateList = templateList.Where(t => 
                        t.Name.ToLower().Contains(searchTerm) || 
                        t.Description.ToLower().Contains(searchTerm)
                    ).ToList();
                }
            }
            
            return _mapper.Map<List<GlobalDocumentTemplateDto>>(templateList);
        }
    }
    
    /// <summary>
    /// Handler for getting a global template by ID
    /// </summary>
    public class GetGlobalTemplateByIdQueryHandler : IRequestHandler<GetGlobalTemplateByIdQuery, GlobalDocumentTemplateDto?>
    {
        private readonly IGlobalDocumentTemplateRepository _repository;
        private readonly IMapper _mapper;

        public GetGlobalTemplateByIdQueryHandler(IGlobalDocumentTemplateRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<GlobalDocumentTemplateDto?> Handle(GetGlobalTemplateByIdQuery request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetByIdAsync(request.Id, request.TenantId);
            return template == null ? null : _mapper.Map<GlobalDocumentTemplateDto>(template);
        }
    }
    
    /// <summary>
    /// Handler for getting global templates by type
    /// </summary>
    public class GetGlobalTemplatesByTypeQueryHandler : IRequestHandler<GetGlobalTemplatesByTypeQuery, List<GlobalDocumentTemplateDto>>
    {
        private readonly IGlobalDocumentTemplateRepository _repository;
        private readonly IMapper _mapper;

        public GetGlobalTemplatesByTypeQueryHandler(IGlobalDocumentTemplateRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<GlobalDocumentTemplateDto>> Handle(GetGlobalTemplatesByTypeQuery request, CancellationToken cancellationToken)
        {
            var templates = await _repository.GetByTypeAsync(request.Type, request.TenantId);
            return _mapper.Map<List<GlobalDocumentTemplateDto>>(templates.ToList());
        }
    }
    
    /// <summary>
    /// Handler for getting global templates by specialty
    /// </summary>
    public class GetGlobalTemplatesBySpecialtyQueryHandler : IRequestHandler<GetGlobalTemplatesBySpecialtyQuery, List<GlobalDocumentTemplateDto>>
    {
        private readonly IGlobalDocumentTemplateRepository _repository;
        private readonly IMapper _mapper;

        public GetGlobalTemplatesBySpecialtyQueryHandler(IGlobalDocumentTemplateRepository repository, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<List<GlobalDocumentTemplateDto>> Handle(GetGlobalTemplatesBySpecialtyQuery request, CancellationToken cancellationToken)
        {
            var templates = await _repository.GetBySpecialtyAsync(request.Specialty, request.TenantId);
            return _mapper.Map<List<GlobalDocumentTemplateDto>>(templates.ToList());
        }
    }
}
