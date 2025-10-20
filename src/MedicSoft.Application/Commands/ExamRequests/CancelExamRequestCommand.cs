using MediatR;

namespace MedicSoft.Application.Commands.ExamRequests
{
    public record CancelExamRequestCommand(Guid Id, string TenantId) 
        : IRequest<bool>;
}
