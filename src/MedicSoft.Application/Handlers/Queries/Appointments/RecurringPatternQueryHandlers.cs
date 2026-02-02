using AutoMapper;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Appointments;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Appointments
{
    public class GetRecurringPatternsQueryHandler : IRequestHandler<GetRecurringPatternsQuery, IEnumerable<RecurringAppointmentPatternDto>>
    {
        private readonly IRecurringAppointmentPatternRepository _repository;
        private readonly IMapper _mapper;

        public GetRecurringPatternsQueryHandler(IRecurringAppointmentPatternRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RecurringAppointmentPatternDto>> Handle(GetRecurringPatternsQuery request, CancellationToken cancellationToken)
        {
            var patterns = await _repository.GetActivePatternsByClinicAsync(request.ClinicId, request.TenantId);
            return _mapper.Map<IEnumerable<RecurringAppointmentPatternDto>>(patterns);
        }
    }

    public class GetRecurringPatternsByProfessionalQueryHandler : IRequestHandler<GetRecurringPatternsByProfessionalQuery, IEnumerable<RecurringAppointmentPatternDto>>
    {
        private readonly IRecurringAppointmentPatternRepository _repository;
        private readonly IMapper _mapper;

        public GetRecurringPatternsByProfessionalQueryHandler(IRecurringAppointmentPatternRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RecurringAppointmentPatternDto>> Handle(GetRecurringPatternsByProfessionalQuery request, CancellationToken cancellationToken)
        {
            var patterns = await _repository.GetActivePatternsByProfessionalAsync(request.ProfessionalId, request.TenantId);
            return _mapper.Map<IEnumerable<RecurringAppointmentPatternDto>>(patterns);
        }
    }

    public class GetRecurringPatternsByPatientQueryHandler : IRequestHandler<GetRecurringPatternsByPatientQuery, IEnumerable<RecurringAppointmentPatternDto>>
    {
        private readonly IRecurringAppointmentPatternRepository _repository;
        private readonly IMapper _mapper;

        public GetRecurringPatternsByPatientQueryHandler(IRecurringAppointmentPatternRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RecurringAppointmentPatternDto>> Handle(GetRecurringPatternsByPatientQuery request, CancellationToken cancellationToken)
        {
            var patterns = await _repository.GetActivePatternsByPatientAsync(request.PatientId, request.TenantId);
            return _mapper.Map<IEnumerable<RecurringAppointmentPatternDto>>(patterns);
        }
    }

    public class GetRecurringPatternByIdQueryHandler : IRequestHandler<GetRecurringPatternByIdQuery, RecurringAppointmentPatternDto?>
    {
        private readonly IRecurringAppointmentPatternRepository _repository;
        private readonly IMapper _mapper;

        public GetRecurringPatternByIdQueryHandler(IRecurringAppointmentPatternRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<RecurringAppointmentPatternDto?> Handle(GetRecurringPatternByIdQuery request, CancellationToken cancellationToken)
        {
            var pattern = await _repository.GetByIdAsync(request.Id, request.TenantId);
            return pattern != null ? _mapper.Map<RecurringAppointmentPatternDto>(pattern) : null;
        }
    }
}
