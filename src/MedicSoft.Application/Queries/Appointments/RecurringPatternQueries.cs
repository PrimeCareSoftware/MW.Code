using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Queries.Appointments
{
    public class GetRecurringPatternsQuery : IRequest<IEnumerable<RecurringAppointmentPatternDto>>
    {
        public Guid ClinicId { get; }
        public string TenantId { get; }

        public GetRecurringPatternsQuery(Guid clinicId, string tenantId)
        {
            ClinicId = clinicId;
            TenantId = tenantId;
        }
    }

    public class GetRecurringPatternsByProfessionalQuery : IRequest<IEnumerable<RecurringAppointmentPatternDto>>
    {
        public Guid ProfessionalId { get; }
        public string TenantId { get; }

        public GetRecurringPatternsByProfessionalQuery(Guid professionalId, string tenantId)
        {
            ProfessionalId = professionalId;
            TenantId = tenantId;
        }
    }

    public class GetRecurringPatternsByPatientQuery : IRequest<IEnumerable<RecurringAppointmentPatternDto>>
    {
        public Guid PatientId { get; }
        public string TenantId { get; }

        public GetRecurringPatternsByPatientQuery(Guid patientId, string tenantId)
        {
            PatientId = patientId;
            TenantId = tenantId;
        }
    }

    public class GetRecurringPatternByIdQuery : IRequest<RecurringAppointmentPatternDto?>
    {
        public Guid Id { get; }
        public string TenantId { get; }

        public GetRecurringPatternByIdQuery(Guid id, string tenantId)
        {
            Id = id;
            TenantId = tenantId;
        }
    }
}
