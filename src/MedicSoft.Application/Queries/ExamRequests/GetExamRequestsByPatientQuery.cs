using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.ExamRequests
{
    public record GetExamRequestsByPatientQuery(Guid PatientId, string TenantId) 
        : IRequest<IEnumerable<ExamRequestDto>>;
}
