using MediatR;
using MedicSoft.Application.Commands.ExamRequests;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.ExamRequests;

namespace MedicSoft.Application.Services
{
    public interface IExamRequestService
    {
        Task<ExamRequestDto> CreateExamRequestAsync(CreateExamRequestDto createDto, string tenantId);
        Task<ExamRequestDto> UpdateExamRequestAsync(Guid id, UpdateExamRequestDto updateDto, string tenantId);
        Task<ExamRequestDto> CompleteExamRequestAsync(Guid id, CompleteExamRequestDto completeDto, string tenantId);
        Task<bool> CancelExamRequestAsync(Guid id, string tenantId);
        Task<ExamRequestDto?> GetByIdAsync(Guid id, string tenantId);
        Task<IEnumerable<ExamRequestDto>> GetByAppointmentIdAsync(Guid appointmentId, string tenantId);
        Task<IEnumerable<ExamRequestDto>> GetByPatientIdAsync(Guid patientId, string tenantId);
        Task<IEnumerable<ExamRequestDto>> GetPendingExamsAsync(string tenantId);
        Task<IEnumerable<ExamRequestDto>> GetUrgentExamsAsync(string tenantId);
    }

    public class ExamRequestService : IExamRequestService
    {
        private readonly IMediator _mediator;

        public ExamRequestService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<ExamRequestDto> CreateExamRequestAsync(CreateExamRequestDto createDto, string tenantId)
        {
            var command = new CreateExamRequestCommand(createDto, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<ExamRequestDto> UpdateExamRequestAsync(Guid id, UpdateExamRequestDto updateDto, string tenantId)
        {
            var command = new UpdateExamRequestCommand(id, updateDto, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<ExamRequestDto> CompleteExamRequestAsync(Guid id, CompleteExamRequestDto completeDto, string tenantId)
        {
            var command = new CompleteExamRequestCommand(id, completeDto, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<bool> CancelExamRequestAsync(Guid id, string tenantId)
        {
            var command = new CancelExamRequestCommand(id, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<ExamRequestDto?> GetByIdAsync(Guid id, string tenantId)
        {
            var query = new GetExamRequestByIdQuery(id, tenantId);
            return await _mediator.Send(query);
        }

        public async Task<IEnumerable<ExamRequestDto>> GetByAppointmentIdAsync(Guid appointmentId, string tenantId)
        {
            var query = new GetExamRequestsByAppointmentQuery(appointmentId, tenantId);
            return await _mediator.Send(query);
        }

        public async Task<IEnumerable<ExamRequestDto>> GetByPatientIdAsync(Guid patientId, string tenantId)
        {
            var query = new GetExamRequestsByPatientQuery(patientId, tenantId);
            return await _mediator.Send(query);
        }

        public async Task<IEnumerable<ExamRequestDto>> GetPendingExamsAsync(string tenantId)
        {
            var query = new GetPendingExamRequestsQuery(tenantId);
            return await _mediator.Send(query);
        }

        public async Task<IEnumerable<ExamRequestDto>> GetUrgentExamsAsync(string tenantId)
        {
            var query = new GetUrgentExamRequestsQuery(tenantId);
            return await _mediator.Send(query);
        }
    }
}
