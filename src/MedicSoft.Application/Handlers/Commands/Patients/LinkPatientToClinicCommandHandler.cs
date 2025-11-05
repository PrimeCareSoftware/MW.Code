using MediatR;
using MedicSoft.Application.Commands.Patients;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Patients
{
    public class LinkPatientToClinicCommandHandler : IRequestHandler<LinkPatientToClinicCommand, bool>
    {
        private readonly IPatientClinicLinkRepository _linkRepository;
        private readonly IPatientRepository _patientRepository;

        public LinkPatientToClinicCommandHandler(
            IPatientClinicLinkRepository linkRepository,
            IPatientRepository patientRepository)
        {
            _linkRepository = linkRepository;
            _patientRepository = patientRepository;
        }

        public async Task<bool> Handle(LinkPatientToClinicCommand request, CancellationToken cancellationToken)
        {
            return await _linkRepository.ExecuteInTransactionAsync(async () =>
            {
                // Check if patient exists
                var patient = await _patientRepository.GetByIdAsync(request.PatientId, request.TenantId);
                if (patient == null)
                    throw new InvalidOperationException("Patient not found");

                // Check if link already exists
                var existingLink = await _linkRepository.GetLinkAsync(request.PatientId, request.ClinicId, request.TenantId);
                if (existingLink != null)
                {
                    if (!existingLink.IsActive)
                    {
                        existingLink.Activate();
                        await _linkRepository.UpdateAsync(existingLink);
                    }
                    return true;
                }

                // Create new link
                var link = new PatientClinicLink(request.PatientId, request.ClinicId, request.TenantId);
                await _linkRepository.AddAsync(link);
                return true;
            });
        }
    }
}
