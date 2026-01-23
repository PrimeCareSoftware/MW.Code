using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Anamnesis;
using MedicSoft.Application.DTOs.Anamnesis;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Anamnesis
{
    public class CreateAnamnesisResponseCommandHandler : IRequestHandler<CreateAnamnesisResponseCommand, AnamnesisResponseDto>
    {
        private readonly IAnamnesisResponseRepository _repository;
        private readonly IMapper _mapper;

        public CreateAnamnesisResponseCommandHandler(IAnamnesisResponseRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<AnamnesisResponseDto> Handle(CreateAnamnesisResponseCommand request, CancellationToken cancellationToken)
        {
            // Check if response already exists for this appointment
            var existing = await _repository.GetByAppointmentIdAsync(request.AppointmentId, request.TenantId);
            if (existing != null)
                throw new InvalidOperationException("Já existe uma anamnese para este atendimento");

            var response = new AnamnesisResponse(
                request.AppointmentId,
                request.PatientId,
                request.DoctorId,
                request.TemplateId,
                request.TenantId
            );

            var created = await _repository.AddAsync(response);
            return _mapper.Map<AnamnesisResponseDto>(created);
        }
    }
}
