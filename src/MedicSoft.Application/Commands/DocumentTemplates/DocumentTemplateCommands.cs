using System;
using MediatR;
using MedicSoft.Application.DTOs.DocumentTemplates;

namespace MedicSoft.Application.Commands.DocumentTemplates
{
    /// <summary>
    /// Command to create a new document template
    /// </summary>
    public class CreateDocumentTemplateCommand : IRequest<DocumentTemplateDto>
    {
        public CreateDocumentTemplateDto Template { get; }
        public string TenantId { get; }
        public Guid? ClinicId { get; }

        public CreateDocumentTemplateCommand(CreateDocumentTemplateDto template, string tenantId, Guid? clinicId = null)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
            ClinicId = clinicId;
        }
    }
    
    /// <summary>
    /// Command to update an existing document template
    /// </summary>
    public class UpdateDocumentTemplateCommand : IRequest<DocumentTemplateDto>
    {
        public Guid Id { get; }
        public UpdateDocumentTemplateDto Template { get; }
        public string TenantId { get; }

        public UpdateDocumentTemplateCommand(Guid id, UpdateDocumentTemplateDto template, string tenantId)
        {
            Id = id;
            Template = template ?? throw new ArgumentNullException(nameof(template));
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        }
    }
    
    /// <summary>
    /// Command to delete a document template
    /// </summary>
    public class DeleteDocumentTemplateCommand : IRequest<Unit>
    {
        public Guid Id { get; }
        public string TenantId { get; }

        public DeleteDocumentTemplateCommand(Guid id, string tenantId)
        {
            Id = id;
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        }
    }
    
    /// <summary>
    /// Command to activate a document template
    /// </summary>
    public class ActivateDocumentTemplateCommand : IRequest<Unit>
    {
        public Guid Id { get; }
        public string TenantId { get; }

        public ActivateDocumentTemplateCommand(Guid id, string tenantId)
        {
            Id = id;
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        }
    }
    
    /// <summary>
    /// Command to deactivate a document template
    /// </summary>
    public class DeactivateDocumentTemplateCommand : IRequest<Unit>
    {
        public Guid Id { get; }
        public string TenantId { get; }

        public DeactivateDocumentTemplateCommand(Guid id, string tenantId)
        {
            Id = id;
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        }
    }
}
