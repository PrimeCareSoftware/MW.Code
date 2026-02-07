using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Appointments
{
    public class GetWeekAgendaQuery : IRequest<WeekAgendaDto>
    {
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public Guid ClinicId { get; }
        public string TenantId { get; }
        public Guid? ProfessionalId { get; }

        public GetWeekAgendaQuery(DateTime startDate, DateTime endDate, Guid clinicId, string tenantId, Guid? professionalId = null)
        {
            StartDate = startDate;
            EndDate = endDate;
            ClinicId = clinicId;
            TenantId = tenantId;
            ProfessionalId = professionalId;
        }
    }
}
