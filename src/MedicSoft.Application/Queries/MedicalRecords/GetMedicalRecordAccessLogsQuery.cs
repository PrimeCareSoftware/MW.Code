using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.MedicalRecords
{
    public class GetMedicalRecordAccessLogsQuery : IRequest<List<MedicalRecordAccessLogDto>>
    {
        public Guid MedicalRecordId { get; }
        public string TenantId { get; }
        public DateTime? StartDate { get; }
        public DateTime? EndDate { get; }

        public GetMedicalRecordAccessLogsQuery(Guid medicalRecordId, string tenantId, DateTime? startDate = null, DateTime? endDate = null)
        {
            MedicalRecordId = medicalRecordId;
            TenantId = tenantId;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
