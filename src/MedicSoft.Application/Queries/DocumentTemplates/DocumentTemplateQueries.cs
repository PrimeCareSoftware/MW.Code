using System;
using System.Collections.Generic;
using MediatR;
using MedicSoft.Application.DTOs.DocumentTemplates;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Application.Queries.DocumentTemplates
{
    /// <summary>
    /// Query to get all document templates
    /// </summary>
    public class GetAllDocumentTemplatesQuery : IRequest<List<DocumentTemplateDto>>
    {
        public string TenantId { get; }
        public DocumentTemplateFilterDto? Filter { get; }

        public GetAllDocumentTemplatesQuery(string tenantId, DocumentTemplateFilterDto? filter = null)
        {
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
            Filter = filter;
        }
    }
    
    /// <summary>
    /// Query to get a document template by ID
    /// </summary>
    public class GetDocumentTemplateByIdQuery : IRequest<DocumentTemplateDto?>
    {
        public Guid Id { get; }
        public string TenantId { get; }

        public GetDocumentTemplateByIdQuery(Guid id, string tenantId)
        {
            Id = id;
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        }
    }
    
    /// <summary>
    /// Query to get templates by specialty
    /// </summary>
    public class GetTemplatesBySpecialtyQuery : IRequest<List<DocumentTemplateDto>>
    {
        public ProfessionalSpecialty Specialty { get; }
        public string TenantId { get; }
        public bool ActiveOnly { get; }

        public GetTemplatesBySpecialtyQuery(ProfessionalSpecialty specialty, string tenantId, bool activeOnly = false)
        {
            Specialty = specialty;
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
            ActiveOnly = activeOnly;
        }
    }
    
    /// <summary>
    /// Query to get templates by type
    /// </summary>
    public class GetTemplatesByTypeQuery : IRequest<List<DocumentTemplateDto>>
    {
        public DocumentTemplateType Type { get; }
        public string TenantId { get; }

        public GetTemplatesByTypeQuery(DocumentTemplateType type, string tenantId)
        {
            Type = type;
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        }
    }
    
    /// <summary>
    /// Query to get templates by clinic
    /// </summary>
    public class GetTemplatesByClinicQuery : IRequest<List<DocumentTemplateDto>>
    {
        public Guid ClinicId { get; }
        public string TenantId { get; }

        public GetTemplatesByClinicQuery(Guid clinicId, string tenantId)
        {
            ClinicId = clinicId;
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        }
    }
}
