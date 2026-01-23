using MediatR;
using MedicSoft.Application.DTOs.Anamnesis;

namespace MedicSoft.Application.Commands.Anamnesis
{
    public class CreateAnamnesisTemplateCommand : IRequest<AnamnesisTemplateDto>
    {
        public CreateAnamnesisTemplateDto Template { get; }
        public string TenantId { get; }
        public System.Guid CreatedBy { get; }

        public CreateAnamnesisTemplateCommand(CreateAnamnesisTemplateDto template, string tenantId, System.Guid createdBy)
        {
            Template = template;
            TenantId = tenantId;
            CreatedBy = createdBy;
        }
    }
}
