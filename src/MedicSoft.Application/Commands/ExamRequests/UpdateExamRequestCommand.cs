using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.ExamRequests
{
    public record UpdateExamRequestCommand(Guid Id, UpdateExamRequestDto ExamRequestDto, string TenantId) 
        : IRequest<ExamRequestDto>;
}
