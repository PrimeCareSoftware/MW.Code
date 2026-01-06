using MediatR;

namespace MedicSoft.Application.Commands.DigitalPrescriptions
{
    public record SignDigitalPrescriptionCommand(Guid PrescriptionId, string DigitalSignature, string CertificateThumbprint, string TenantId) 
        : IRequest<bool>;
}
