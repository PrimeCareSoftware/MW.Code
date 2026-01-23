using MediatR;
using MedicSoft.Application.DTOs.Anamnesis;
using System;

namespace MedicSoft.Application.Commands.Anamnesis
{
    public class UpdateAnamnesisTemplateCommand : IRequest<AnamnesisTemplateDto>
    {
        public Guid TemplateId { get; }
        public UpdateAnamnesisTemplateDto Template { get; }
        public string TenantId { get; }

        public UpdateAnamnesisTemplateCommand(Guid templateId, UpdateAnamnesisTemplateDto template, string tenantId)
        {
            TemplateId = templateId;
            Template = template;
            TenantId = tenantId;
        }
    }
}
