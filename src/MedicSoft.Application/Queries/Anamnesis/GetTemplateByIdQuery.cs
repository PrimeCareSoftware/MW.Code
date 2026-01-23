using MediatR;
using MedicSoft.Application.DTOs.Anamnesis;
using System;

namespace MedicSoft.Application.Queries.Anamnesis
{
    public class GetTemplateByIdQuery : IRequest<AnamnesisTemplateDto?>
    {
        public Guid TemplateId { get; }
        public string TenantId { get; }

        public GetTemplateByIdQuery(Guid templateId, string tenantId)
        {
            TemplateId = templateId;
            TenantId = tenantId;
        }
    }
}
