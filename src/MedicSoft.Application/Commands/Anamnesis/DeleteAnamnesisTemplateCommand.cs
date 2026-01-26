using MediatR;
using System;

namespace MedicSoft.Application.Commands.Anamnesis
{
    public class DeleteAnamnesisTemplateCommand : IRequest<bool>
    {
        public Guid TemplateId { get; }
        public string TenantId { get; }

        public DeleteAnamnesisTemplateCommand(Guid templateId, string tenantId)
        {
            TemplateId = templateId;
            TenantId = tenantId;
        }
    }
}
