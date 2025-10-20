using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.ExamRequests
{
    public record CompleteExamRequestCommand(Guid Id, CompleteExamRequestDto CompleteDto, string TenantId) 
        : IRequest<ExamRequestDto>;
}
