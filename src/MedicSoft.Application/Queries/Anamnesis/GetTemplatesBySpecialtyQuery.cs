using MediatR;
using MedicSoft.Application.DTOs.Anamnesis;
using MedicSoft.Domain.Enums;
using System.Collections.Generic;

namespace MedicSoft.Application.Queries.Anamnesis
{
    public class GetTemplatesBySpecialtyQuery : IRequest<List<AnamnesisTemplateDto>>
    {
        public MedicalSpecialty Specialty { get; }
        public string TenantId { get; }

        public GetTemplatesBySpecialtyQuery(MedicalSpecialty specialty, string tenantId)
        {
            Specialty = specialty;
            TenantId = tenantId;
        }
    }
}
