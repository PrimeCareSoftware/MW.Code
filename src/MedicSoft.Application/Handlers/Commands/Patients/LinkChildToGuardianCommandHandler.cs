using MediatR;
using MedicSoft.Application.Commands.Patients;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Patients
{
    public class LinkChildToGuardianCommandHandler : IRequestHandler<LinkChildToGuardianCommand, bool>
    {
        private readonly IPatientRepository _patientRepository;

        public LinkChildToGuardianCommandHandler(IPatientRepository patientRepository)
        {
            _patientRepository = patientRepository;
        }

        public async Task<bool> Handle(LinkChildToGuardianCommand request, CancellationToken cancellationToken)
        {
            return await _patientRepository.ExecuteInTransactionAsync(async () =>
            {
                // Get child patient
                var child = await _patientRepository.GetByIdAsync(request.ChildId, request.TenantId);
                if (child == null)
                    throw new InvalidOperationException("Child patient not found");

                // Get guardian patient
                var guardian = await _patientRepository.GetByIdAsync(request.GuardianId, request.TenantId);
                if (guardian == null)
                    throw new InvalidOperationException("Guardian patient not found");

                // Validate child is under 18
                if (!child.IsChild())
                    throw new InvalidOperationException("Only children (under 18) can have a guardian");

                // Link child to guardian
                child.SetGuardian(request.GuardianId);
                guardian.AddChild(child);

                await _patientRepository.UpdateAsync(child);
                await _patientRepository.UpdateAsync(guardian);

                return true;
            });
        }
    }
}
