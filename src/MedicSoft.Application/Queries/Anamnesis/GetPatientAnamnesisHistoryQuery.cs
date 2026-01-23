using MediatR;
using MedicSoft.Application.DTOs.Anamnesis;
using System;
using System.Collections.Generic;

namespace MedicSoft.Application.Queries.Anamnesis
{
    public class GetPatientAnamnesisHistoryQuery : IRequest<List<AnamnesisResponseDto>>
    {
        public Guid PatientId { get; }
        public string TenantId { get; }

        public GetPatientAnamnesisHistoryQuery(Guid patientId, string tenantId)
        {
            PatientId = patientId;
            TenantId = tenantId;
        }
    }
}
