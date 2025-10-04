using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Appointments
{
    public record GetAppointmentByIdQuery(Guid AppointmentId, string TenantId) 
        : IRequest<AppointmentDto?>;
}
