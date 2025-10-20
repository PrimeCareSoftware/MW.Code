using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.ExamRequests
{
    public record CreateExamRequestCommand(CreateExamRequestDto ExamRequestDto, string TenantId) 
        : IRequest<ExamRequestDto>;
}
