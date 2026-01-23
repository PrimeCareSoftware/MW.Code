using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Anamnesis;
using MedicSoft.Application.DTOs.Anamnesis;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Anamnesis
{
    public class SaveAnamnesisAnswersCommandHandler : IRequestHandler<SaveAnamnesisAnswersCommand, AnamnesisResponseDto>
    {
        private readonly IAnamnesisResponseRepository _repository;
        private readonly IMapper _mapper;

        public SaveAnamnesisAnswersCommandHandler(IAnamnesisResponseRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AnamnesisResponseDto> Handle(SaveAnamnesisAnswersCommand request, CancellationToken cancellationToken)
        {
            var response = await _repository.GetByIdAsync(request.ResponseId, request.TenantId);
            
            if (response == null)
                throw new InvalidOperationException("Resposta de anamnese não encontrada");

            response.SaveAnswers(request.Answers.Answers, request.Answers.IsComplete);
            
            await _repository.UpdateAsync(response);
            return _mapper.Map<AnamnesisResponseDto>(response);
        }
    }
}
