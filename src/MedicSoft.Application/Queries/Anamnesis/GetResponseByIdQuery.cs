using MediatR;
using MedicSoft.Application.DTOs.Anamnesis;
using System;

namespace MedicSoft.Application.Queries.Anamnesis
{
    public class GetResponseByIdQuery : IRequest<AnamnesisResponseDto?>
    {
        public Guid ResponseId { get; }
        public string TenantId { get; }

        public GetResponseByIdQuery(Guid responseId, string tenantId)
        {
            ResponseId = responseId;
            TenantId = tenantId;
        }
    }
}
