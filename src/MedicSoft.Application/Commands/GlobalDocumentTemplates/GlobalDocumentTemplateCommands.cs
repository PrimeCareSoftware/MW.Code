using System;
using MediatR;
using MedicSoft.Application.DTOs.GlobalDocumentTemplates;

namespace MedicSoft.Application.Commands.GlobalDocumentTemplates
{
    /// <summary>
    /// Command to create a new global document template
    /// </summary>
    public class CreateGlobalTemplateCommand : IRequest<GlobalDocumentTemplateDto>
    {
        public CreateGlobalTemplateDto Template { get; }
        public string TenantId { get; }
        public string CreatedBy { get; }

        public CreateGlobalTemplateCommand(CreateGlobalTemplateDto template, string tenantId, string createdBy)
        {
            Template = template ?? throw new ArgumentNullException(nameof(template));
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
            CreatedBy = createdBy ?? throw new ArgumentNullException(nameof(createdBy));
        }
    }
    
    /// <summary>
    /// Command to update an existing global document template
    /// </summary>
    public class UpdateGlobalTemplateCommand : IRequest<GlobalDocumentTemplateDto>
    {
        public Guid Id { get; }
        public UpdateGlobalTemplateDto Template { get; }
        public string TenantId { get; }

        public UpdateGlobalTemplateCommand(Guid id, UpdateGlobalTemplateDto template, string tenantId)
        {
            Id = id;
            Template = template ?? throw new ArgumentNullException(nameof(template));
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        }
    }
    
    /// <summary>
    /// Command to delete a global document template
    /// </summary>
    public class DeleteGlobalTemplateCommand : IRequest<Unit>
    {
        public Guid Id { get; }
        public string TenantId { get; }

        public DeleteGlobalTemplateCommand(Guid id, string tenantId)
        {
            Id = id;
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        }
    }
    
    /// <summary>
    /// Command to set active status of a global document template
    /// </summary>
    public class SetGlobalTemplateActiveStatusCommand : IRequest<Unit>
    {
        public Guid Id { get; }
        public bool IsActive { get; }
        public string TenantId { get; }

        public SetGlobalTemplateActiveStatusCommand(Guid id, bool isActive, string tenantId)
        {
            Id = id;
            IsActive = isActive;
            TenantId = tenantId ?? throw new ArgumentNullException(nameof(tenantId));
        }
    }
}
