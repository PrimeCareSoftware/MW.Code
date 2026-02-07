using System;
using System.Collections.Generic;
using MediatR;
using MedicSoft.Application.DTOs.GlobalDocumentTemplates;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Application.Queries.GlobalDocumentTemplates
{
    /// <summary>
    /// Query to get all global document templates
    /// </summary>
    public class GetAllGlobalTemplatesQuery : IRequest<List<GlobalDocumentTemplateDto>>
    {
        public string TenantId { get; }
        public GlobalDocumentTemplateFilterDto? Filter { get; }

        public GetAllGlobalTemplatesQuery(string tenantId, GlobalDocumentTemplateFilterDto? filter = null)
        {
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
            Filter = filter;
        }
    }
    
    /// <summary>
    /// Query to get a global template by ID
    /// </summary>
    public class GetGlobalTemplateByIdQuery : IRequest<GlobalDocumentTemplateDto?>
    {
        public Guid Id { get; }
        public string TenantId { get; }

        public GetGlobalTemplateByIdQuery(Guid id, string tenantId)
        {
            Id = id;
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        }
    }
    
    /// <summary>
    /// Query to get global templates by type
    /// </summary>
    public class GetGlobalTemplatesByTypeQuery : IRequest<List<GlobalDocumentTemplateDto>>
    {
        public DocumentTemplateType Type { get; }
        public string TenantId { get; }

        public GetGlobalTemplatesByTypeQuery(DocumentTemplateType type, string tenantId)
        {
            Type = type;
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        }
    }
    
    /// <summary>
    /// Query to get global templates by specialty
    /// </summary>
    public class GetGlobalTemplatesBySpecialtyQuery : IRequest<List<GlobalDocumentTemplateDto>>
    {
        public ProfessionalSpecialty Specialty { get; }
        public string TenantId { get; }

        public GetGlobalTemplatesBySpecialtyQuery(ProfessionalSpecialty specialty, string tenantId)
        {
            Specialty = specialty;
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        }
    }
}
