using MediatR;
using MedicSoft.Application.DTOs.Anamnesis;
using System;

namespace MedicSoft.Application.Commands.Anamnesis
{
    public class SaveAnamnesisAnswersCommand : IRequest<AnamnesisResponseDto>
    {
        public Guid ResponseId { get; }
        public SaveAnswersDto Answers { get; }
        public string TenantId { get; }

        public SaveAnamnesisAnswersCommand(Guid responseId, SaveAnswersDto answers, string tenantId)
        {
            ResponseId = responseId;
            Answers = answers;
            TenantId = tenantId;
        }
    }
}
