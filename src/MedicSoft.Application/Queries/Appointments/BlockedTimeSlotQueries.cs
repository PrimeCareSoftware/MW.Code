using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Appointments
{
    public class GetBlockedTimeSlotsQuery : IRequest<IEnumerable<BlockedTimeSlotDto>>
    {
        public DateTime Date { get; }
        public Guid ClinicId { get; }
        public Guid? ProfessionalId { get; }
        public string TenantId { get; }

        public GetBlockedTimeSlotsQuery(DateTime date, Guid clinicId, string tenantId, Guid? professionalId = null)
        {
            Date = date;
            ClinicId = clinicId;
            ProfessionalId = professionalId;
            TenantId = tenantId;
        }
    }

    public class GetBlockedTimeSlotsRangeQuery : IRequest<IEnumerable<BlockedTimeSlotDto>>
    {
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public Guid ClinicId { get; }
        public string TenantId { get; }

        public GetBlockedTimeSlotsRangeQuery(DateTime startDate, DateTime endDate, Guid clinicId, string tenantId)
        {
            StartDate = startDate;
            EndDate = endDate;
            ClinicId = clinicId;
            TenantId = tenantId;
        }
    }

    public class GetBlockedTimeSlotByIdQuery : IRequest<BlockedTimeSlotDto?>
    {
        public Guid Id { get; }
        public string TenantId { get; }

        public GetBlockedTimeSlotByIdQuery(Guid id, string tenantId)
        {
            Id = id;
            TenantId = tenantId;
        }
    }
}
