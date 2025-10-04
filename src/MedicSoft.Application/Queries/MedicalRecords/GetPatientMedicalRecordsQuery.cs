using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.MedicalRecords
{
    public record GetPatientMedicalRecordsQuery(Guid PatientId, string TenantId) 
        : IRequest<IEnumerable<MedicalRecordDto>>;
}
