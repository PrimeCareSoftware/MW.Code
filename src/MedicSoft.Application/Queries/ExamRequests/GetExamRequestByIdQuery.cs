using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.ExamRequests
{
    public record GetExamRequestByIdQuery(Guid Id, string TenantId) 
        : IRequest<ExamRequestDto?>;
}
