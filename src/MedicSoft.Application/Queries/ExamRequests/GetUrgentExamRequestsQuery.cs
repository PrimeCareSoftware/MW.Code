using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.ExamRequests
{
    public record GetUrgentExamRequestsQuery(string TenantId) 
        : IRequest<IEnumerable<ExamRequestDto>>;
}
