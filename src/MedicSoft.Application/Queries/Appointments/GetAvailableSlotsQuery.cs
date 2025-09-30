using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Appointments
{
    public class GetAvailableSlotsQuery : IRequest<IEnumerable<AvailableSlotDto>>
    {
        public DateTime Date { get; }
        public Guid ClinicId { get; }
        public int DurationMinutes { get; }
        public string TenantId { get; }

        public GetAvailableSlotsQuery(DateTime date, Guid clinicId, int durationMinutes, string tenantId)
        {
            Date = date;
            ClinicId = clinicId;
            DurationMinutes = durationMinutes;
            TenantId = tenantId;
        }
    }
}