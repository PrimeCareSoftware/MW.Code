using MediatR;

namespace MedicSoft.Application.Commands.Patients
{
    public class LinkChildToGuardianCommand : IRequest<bool>
    {
        public Guid ChildId { get; }
        public Guid GuardianId { get; }
        public string TenantId { get; }

        public LinkChildToGuardianCommand(Guid childId, Guid guardianId, string tenantId)
        {
            ChildId = childId;
            GuardianId = guardianId;
            TenantId = tenantId;
        }
    }
}
