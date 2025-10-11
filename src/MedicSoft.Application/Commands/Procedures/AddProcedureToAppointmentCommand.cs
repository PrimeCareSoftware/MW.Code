using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.Procedures
{
    public class AddProcedureToAppointmentCommand : IRequest<AppointmentProcedureDto>
    {
        public Guid AppointmentId { get; }
        public AddProcedureToAppointmentDto ProcedureInfo { get; }
        public string TenantId { get; }

        public AddProcedureToAppointmentCommand(Guid appointmentId, AddProcedureToAppointmentDto procedureInfo, string tenantId)
        {
            AppointmentId = appointmentId;
            ProcedureInfo = procedureInfo;
            TenantId = tenantId;
        }
    }
}
