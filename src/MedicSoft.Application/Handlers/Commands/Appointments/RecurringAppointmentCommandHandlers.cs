using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Appointments;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Domain.Services;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Application.Handlers.Commands.Appointments
{
    public class CreateRecurringAppointmentsCommandHandler : IRequestHandler<CreateRecurringAppointmentsCommand, RecurringAppointmentPatternDto>
    {
        private readonly IRecurringAppointmentPatternRepository _patternRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly RecurringPatternExpansionService _expansionService;
        private readonly IMapper _mapper;

        public CreateRecurringAppointmentsCommandHandler(
            IRecurringAppointmentPatternRepository patternRepository,
            IAppointmentRepository appointmentRepository,
            IClinicRepository clinicRepository,
            IPatientRepository patientRepository,
            RecurringPatternExpansionService expansionService,
            IMapper mapper)
        {
            _patternRepository = patternRepository;
            _appointmentRepository = appointmentRepository;
            _clinicRepository = clinicRepository;
            _patientRepository = patientRepository;
            _expansionService = expansionService;
            _mapper = mapper;
        }

        public async Task<RecurringAppointmentPatternDto> Handle(CreateRecurringAppointmentsCommand request, CancellationToken cancellationToken)
        {
            // Validate clinic exists
            var clinic = await _clinicRepository.GetByIdAsync(request.RecurringAppointments.ClinicId, request.TenantId);
            if (clinic == null)
                throw new ArgumentException("Clinic not found");

            // Validate patient exists
            var patient = await _patientRepository.GetByIdAsync(request.RecurringAppointments.PatientId, request.TenantId);
            if (patient == null)
                throw new ArgumentException("Patient not found");

            // Calculate end time
            var endTime = request.RecurringAppointments.StartTime.Add(TimeSpan.FromMinutes(request.RecurringAppointments.DurationMinutes));

            // Create recurring pattern
            var pattern = new RecurringAppointmentPattern(
                clinicId: request.RecurringAppointments.ClinicId,
                frequency: request.RecurringAppointments.Frequency,
                startDate: request.RecurringAppointments.StartDate,
                startTime: request.RecurringAppointments.StartTime,
                endTime: endTime,
                tenantId: request.TenantId,
                professionalId: request.RecurringAppointments.ProfessionalId,
                patientId: request.RecurringAppointments.PatientId,
                daysOfWeek: request.RecurringAppointments.DaysOfWeek,
                endDate: request.RecurringAppointments.EndDate,
                occurrencesCount: request.RecurringAppointments.OccurrencesCount,
                durationMinutes: request.RecurringAppointments.DurationMinutes,
                appointmentType: request.RecurringAppointments.AppointmentType,
                notes: request.RecurringAppointments.Notes);

            var savedPattern = await _patternRepository.AddAsync(pattern);

            // Generate appointments from pattern
            var appointments = _expansionService.GenerateAppointments(savedPattern);

            // Save generated appointments
            foreach (var appointment in appointments)
            {
                await _appointmentRepository.AddAsync(appointment);
            }

            var patternWithDetails = await _patternRepository.GetByIdAsync(savedPattern.Id, request.TenantId);
            return _mapper.Map<RecurringAppointmentPatternDto>(patternWithDetails);
        }
    }

    public class CreateRecurringBlockedSlotsCommandHandler : IRequestHandler<CreateRecurringBlockedSlotsCommand, RecurringAppointmentPatternDto>
    {
        private readonly IRecurringAppointmentPatternRepository _patternRepository;
        private readonly IBlockedTimeSlotRepository _blockedTimeSlotRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly RecurringPatternExpansionService _expansionService;
        private readonly IMapper _mapper;

        public CreateRecurringBlockedSlotsCommandHandler(
            IRecurringAppointmentPatternRepository patternRepository,
            IBlockedTimeSlotRepository blockedTimeSlotRepository,
            IClinicRepository clinicRepository,
            RecurringPatternExpansionService expansionService,
            IMapper mapper)
        {
            _patternRepository = patternRepository;
            _blockedTimeSlotRepository = blockedTimeSlotRepository;
            _clinicRepository = clinicRepository;
            _expansionService = expansionService;
            _mapper = mapper;
        }

        public async Task<RecurringAppointmentPatternDto> Handle(CreateRecurringBlockedSlotsCommand request, CancellationToken cancellationToken)
        {
            // Validate clinic exists
            var clinic = await _clinicRepository.GetByIdAsync(request.Pattern.ClinicId, request.TenantId);
            if (clinic == null)
                throw new ArgumentException("Clinic not found");

            if (!request.Pattern.BlockedSlotType.HasValue)
                throw new ArgumentException("Blocked slot type is required");

            // Create recurring pattern
            var pattern = new RecurringAppointmentPattern(
                clinicId: request.Pattern.ClinicId,
                frequency: request.Pattern.Frequency,
                startDate: request.Pattern.StartDate,
                startTime: request.Pattern.StartTime,
                endTime: request.Pattern.EndTime,
                tenantId: request.TenantId,
                interval: request.Pattern.Interval,
                professionalId: request.Pattern.ProfessionalId,
                daysOfWeek: request.Pattern.DaysOfWeek,
                dayOfMonth: request.Pattern.DayOfMonth,
                endDate: request.Pattern.EndDate,
                occurrencesCount: request.Pattern.OccurrencesCount,
                blockedSlotType: request.Pattern.BlockedSlotType,
                notes: request.Pattern.Notes);

            var savedPattern = await _patternRepository.AddAsync(pattern);

            // Generate blocked time slots from pattern
            var blockedSlots = _expansionService.GenerateBlockedTimeSlots(savedPattern);

            // Save generated blocked slots
            foreach (var blockedSlot in blockedSlots)
            {
                await _blockedTimeSlotRepository.AddAsync(blockedSlot);
            }

            var patternWithDetails = await _patternRepository.GetByIdAsync(savedPattern.Id, request.TenantId);
            return _mapper.Map<RecurringAppointmentPatternDto>(patternWithDetails);
        }
    }

    public class DeactivateRecurringPatternCommandHandler : IRequestHandler<DeactivateRecurringPatternCommand, bool>
    {
        private readonly IRecurringAppointmentPatternRepository _patternRepository;

        public DeactivateRecurringPatternCommandHandler(IRecurringAppointmentPatternRepository patternRepository)
        {
            _patternRepository = patternRepository;
        }

        public async Task<bool> Handle(DeactivateRecurringPatternCommand request, CancellationToken cancellationToken)
        {
            var pattern = await _patternRepository.GetByIdAsync(request.PatternId, request.TenantId);
            if (pattern == null)
                return false;

            pattern.Deactivate();
            await _patternRepository.UpdateAsync(pattern);
            return true;
        }
    }
}
