using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.Appointments
{
    public class CreateRecurringAppointmentsCommand : IRequest<RecurringAppointmentPatternDto>
    {
        public CreateRecurringAppointmentsDto RecurringAppointments { get; }
        public string TenantId { get; }

        public CreateRecurringAppointmentsCommand(CreateRecurringAppointmentsDto recurringAppointments, string tenantId)
        {
            RecurringAppointments = recurringAppointments;
            TenantId = tenantId;
        }
    }

    public class CreateRecurringBlockedSlotsCommand : IRequest<RecurringAppointmentPatternDto>
    {
        public CreateRecurringAppointmentPatternDto Pattern { get; }
        public string TenantId { get; }

        public CreateRecurringBlockedSlotsCommand(CreateRecurringAppointmentPatternDto pattern, string tenantId)
        {
            Pattern = pattern;
            TenantId = tenantId;
        }
    }

    public class DeactivateRecurringPatternCommand : IRequest<bool>
    {
        public Guid PatternId { get; }
        public string TenantId { get; }

        public DeactivateRecurringPatternCommand(Guid patternId, string tenantId)
        {
            PatternId = patternId;
            TenantId = tenantId;
        }
    }
}
