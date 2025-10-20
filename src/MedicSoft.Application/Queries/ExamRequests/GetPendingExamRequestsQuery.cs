using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.ExamRequests
{
    public record GetPendingExamRequestsQuery(string TenantId) 
        : IRequest<IEnumerable<ExamRequestDto>>;
}
