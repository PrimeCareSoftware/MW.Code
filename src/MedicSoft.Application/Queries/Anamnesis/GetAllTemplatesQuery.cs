using MediatR;
using MedicSoft.Application.DTOs.Anamnesis;
using System.Collections.Generic;

namespace MedicSoft.Application.Queries.Anamnesis
{
    public class GetAllTemplatesQuery : IRequest<List<AnamnesisTemplateDto>>
    {
        public string TenantId { get; }

        public GetAllTemplatesQuery(string tenantId)
        {
            TenantId = tenantId;
        }
    }
}
