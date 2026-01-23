using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.MedicalRecords
{
    public class GetMedicalRecordVersionsQuery : IRequest<List<MedicalRecordVersionDto>>
    {
        public Guid MedicalRecordId { get; }
        public string TenantId { get; }

        public GetMedicalRecordVersionsQuery(Guid medicalRecordId, string tenantId)
        {
            MedicalRecordId = medicalRecordId;
            TenantId = tenantId;
        }
    }
}
