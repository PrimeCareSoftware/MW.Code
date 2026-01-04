using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.ClinicalExaminations
{
    public record GetClinicalExaminationsByMedicalRecordQuery(Guid MedicalRecordId, string TenantId) 
        : IRequest<IEnumerable<ClinicalExaminationDto>>;
}
