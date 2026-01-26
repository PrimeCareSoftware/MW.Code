using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MedicSoft.Application.Commands.Anamnesis;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Anamnesis
{
    public class DeleteAnamnesisTemplateCommandHandler : IRequestHandler<DeleteAnamnesisTemplateCommand, bool>
    {
        private readonly IAnamnesisTemplateRepository _repository;

        public DeleteAnamnesisTemplateCommandHandler(IAnamnesisTemplateRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteAnamnesisTemplateCommand request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetByIdAsync(request.TemplateId, request.TenantId);
            
            if (template == null)
                throw new InvalidOperationException("Template não encontrado");

            await _repository.DeleteAsync(request.TemplateId, request.TenantId);
            return true;
        }
    }
}
