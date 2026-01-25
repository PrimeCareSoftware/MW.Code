using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.Clinics
{
    public class CreateClinicCommand : IRequest<ClinicDto>
    {
        public CreateClinicDto Clinic { get; }
        public string TenantId { get; }
        public Guid OwnerId { get; }

        public CreateClinicCommand(CreateClinicDto clinic, string tenantId, Guid ownerId)
        {
            Clinic = clinic;
            TenantId = tenantId;
            OwnerId = ownerId;
        }
    }
}
