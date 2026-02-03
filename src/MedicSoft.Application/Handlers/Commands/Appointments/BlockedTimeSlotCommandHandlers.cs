using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Appointments;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Appointments
{
    public class CreateBlockedTimeSlotCommandHandler : IRequestHandler<CreateBlockedTimeSlotCommand, BlockedTimeSlotDto>
    {
        private readonly IBlockedTimeSlotRepository _blockedTimeSlotRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IMapper _mapper;

        public CreateBlockedTimeSlotCommandHandler(
            IBlockedTimeSlotRepository blockedTimeSlotRepository,
            IClinicRepository clinicRepository,
            IMapper mapper)
        {
            _blockedTimeSlotRepository = blockedTimeSlotRepository;
            _clinicRepository = clinicRepository;
            _mapper = mapper;
        }

        public async Task<BlockedTimeSlotDto> Handle(CreateBlockedTimeSlotCommand request, CancellationToken cancellationToken)
        {
            // Validate clinic exists
            var clinic = await _clinicRepository.GetByIdAsync(request.BlockedTimeSlot.ClinicId, request.TenantId);
            if (clinic == null)
                throw new ArgumentException("Clinic not found");

            // Check for overlapping blocks
            var hasOverlap = await _blockedTimeSlotRepository.HasOverlappingBlockAsync(
                request.BlockedTimeSlot.ClinicId,
                request.BlockedTimeSlot.Date,
                request.BlockedTimeSlot.StartTime,
                request.BlockedTimeSlot.EndTime,
                request.BlockedTimeSlot.ProfessionalId,
                request.TenantId);

            if (hasOverlap)
                throw new InvalidOperationException("Time slot overlaps with an existing blocked time slot");

            // Create blocked time slot
            var blockedTimeSlot = new BlockedTimeSlot(
                request.BlockedTimeSlot.ClinicId,
                request.BlockedTimeSlot.Date,
                request.BlockedTimeSlot.StartTime,
                request.BlockedTimeSlot.EndTime,
                request.BlockedTimeSlot.Type,
                request.TenantId,
                request.BlockedTimeSlot.ProfessionalId,
                request.BlockedTimeSlot.Reason);

            var result = await _blockedTimeSlotRepository.AddAsync(blockedTimeSlot);
            
            // Reload to get navigation properties
            var blockedSlotWithDetails = await _blockedTimeSlotRepository.GetByIdAsync(result.Id, request.TenantId);

            return _mapper.Map<BlockedTimeSlotDto>(blockedSlotWithDetails);
        }
    }

    public class DeleteBlockedTimeSlotCommandHandler : IRequestHandler<DeleteBlockedTimeSlotCommand, bool>
    {
        private readonly IBlockedTimeSlotRepository _blockedTimeSlotRepository;

        public DeleteBlockedTimeSlotCommandHandler(IBlockedTimeSlotRepository blockedTimeSlotRepository)
        {
            _blockedTimeSlotRepository = blockedTimeSlotRepository;
        }

        public async Task<bool> Handle(DeleteBlockedTimeSlotCommand request, CancellationToken cancellationToken)
        {
            var blockedTimeSlot = await _blockedTimeSlotRepository.GetByIdAsync(request.Id, request.TenantId);
            if (blockedTimeSlot == null)
                return false;

            // If DeleteSeries is true and this is a recurring block, delete all instances
            if (request.DeleteSeries && blockedTimeSlot.IsRecurring && blockedTimeSlot.RecurringPatternId.HasValue)
            {
                await _blockedTimeSlotRepository.DeleteByRecurringPatternIdAsync(blockedTimeSlot.RecurringPatternId.Value, request.TenantId);
            }
            else
            {
                // Delete only this instance
                await _blockedTimeSlotRepository.DeleteAsync(request.Id, request.TenantId);
            }
            
            return true;
        }
    }

    public class UpdateBlockedTimeSlotCommandHandler : IRequestHandler<UpdateBlockedTimeSlotCommand, BlockedTimeSlotDto>
    {
        private readonly IBlockedTimeSlotRepository _blockedTimeSlotRepository;
        private readonly IMapper _mapper;

        public UpdateBlockedTimeSlotCommandHandler(
            IBlockedTimeSlotRepository blockedTimeSlotRepository,
            IMapper mapper)
        {
            _blockedTimeSlotRepository = blockedTimeSlotRepository;
            _mapper = mapper;
        }

        public async Task<BlockedTimeSlotDto> Handle(UpdateBlockedTimeSlotCommand request, CancellationToken cancellationToken)
        {
            var blockedTimeSlot = await _blockedTimeSlotRepository.GetByIdAsync(request.Id, request.TenantId);
            if (blockedTimeSlot == null)
                throw new ArgumentException("Blocked time slot not found");

            // If UpdateSeries is true and this is a recurring block, update all instances
            if (request.UpdateSeries && blockedTimeSlot.IsRecurring && blockedTimeSlot.RecurringPatternId.HasValue)
            {
                var seriesBlocks = await _blockedTimeSlotRepository.GetByRecurringPatternIdAsync(
                    blockedTimeSlot.RecurringPatternId.Value, 
                    request.TenantId);

                foreach (var block in seriesBlocks)
                {
                    block.UpdateTimeSlot(request.BlockedTimeSlot.StartTime, request.BlockedTimeSlot.EndTime);
                    block.UpdateType(request.BlockedTimeSlot.Type);
                    block.UpdateReason(request.BlockedTimeSlot.Reason);
                    await _blockedTimeSlotRepository.UpdateAsync(block);
                }
            }
            else
            {
                // Update only this instance
                // Check for overlapping blocks (excluding current one)
                var hasOverlap = await _blockedTimeSlotRepository.HasOverlappingBlockAsync(
                    blockedTimeSlot.ClinicId,
                    blockedTimeSlot.Date,
                    request.BlockedTimeSlot.StartTime,
                    request.BlockedTimeSlot.EndTime,
                    blockedTimeSlot.ProfessionalId,
                    request.TenantId,
                    request.Id);

                if (hasOverlap)
                    throw new InvalidOperationException("Updated time slot overlaps with an existing blocked time slot");

                // Update properties
                blockedTimeSlot.UpdateTimeSlot(request.BlockedTimeSlot.StartTime, request.BlockedTimeSlot.EndTime);
                blockedTimeSlot.UpdateType(request.BlockedTimeSlot.Type);
                blockedTimeSlot.UpdateReason(request.BlockedTimeSlot.Reason);

                await _blockedTimeSlotRepository.UpdateAsync(blockedTimeSlot);
            }

            return _mapper.Map<BlockedTimeSlotDto>(blockedTimeSlot);
        }
    }
}
