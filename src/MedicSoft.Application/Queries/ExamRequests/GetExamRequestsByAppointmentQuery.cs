using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.ExamRequests
{
    public record GetExamRequestsByAppointmentQuery(Guid AppointmentId, string TenantId) 
        : IRequest<IEnumerable<ExamRequestDto>>;
}
