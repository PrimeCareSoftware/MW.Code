using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.MedicalRecords
{
    public record GetMedicalRecordByAppointmentQuery(Guid AppointmentId, string TenantId) 
        : IRequest<MedicalRecordDto?>;
}
