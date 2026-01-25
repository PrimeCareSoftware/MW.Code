using MediatR;
using MedicSoft.Application.DTOs;

namespace MedicSoft.Application.Commands.Clinics
{
    public class UpdateClinicCommand : IRequest<ClinicDto>
    {
        public Guid ClinicId { get; }
        public UpdateClinicDto Clinic { get; }
        public string TenantId { get; }

        public UpdateClinicCommand(Guid clinicId, UpdateClinicDto clinic, string tenantId)
        {
            ClinicId = clinicId;
            Clinic = clinic;
            TenantId = tenantId;
        }
    }
}
