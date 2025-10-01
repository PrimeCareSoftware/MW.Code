using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Appointments
{
    public class GetDailyAgendaQuery : IRequest<DailyAgendaDto>
    {
        public DateTime Date { get; }
        public Guid ClinicId { get; }
        public string TenantId { get; }

        public GetDailyAgendaQuery(DateTime date, Guid clinicId, string tenantId)
        {
            Date = date;
            ClinicId = clinicId;
            TenantId = tenantId;
        }
    }
}