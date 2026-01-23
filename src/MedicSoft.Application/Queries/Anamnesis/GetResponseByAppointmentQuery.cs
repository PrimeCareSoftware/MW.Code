using MediatR;
using MedicSoft.Application.DTOs.Anamnesis;
using System;

namespace MedicSoft.Application.Queries.Anamnesis
{
    public class GetResponseByAppointmentQuery : IRequest<AnamnesisResponseDto?>
    {
        public Guid AppointmentId { get; }
        public string TenantId { get; }

        public GetResponseByAppointmentQuery(Guid appointmentId, string tenantId)
        {
            AppointmentId = appointmentId;
            TenantId = tenantId;
        }
    }
}
