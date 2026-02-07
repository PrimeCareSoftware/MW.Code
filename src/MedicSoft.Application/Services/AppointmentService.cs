using MediatR;
using MedicSoft.Application.Commands.Appointments;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Appointments;

namespace MedicSoft.Application.Services
{
    public interface IAppointmentService
    {
        Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto, string tenantId);
        Task<AppointmentDto> UpdateAppointmentAsync(Guid appointmentId, UpdateAppointmentDto updateAppointmentDto, string tenantId);
        Task<bool> CancelAppointmentAsync(Guid appointmentId, string cancellationReason, string tenantId);
        Task<DailyAgendaDto> GetDailyAgendaAsync(DateTime date, Guid clinicId, string tenantId, Guid? professionalId = null);
        Task<WeekAgendaDto> GetWeekAgendaAsync(DateTime startDate, DateTime endDate, Guid clinicId, string tenantId, Guid? professionalId = null);
        Task<IEnumerable<AvailableSlotDto>> GetAvailableSlotsAsync(DateTime date, Guid clinicId, int durationMinutes, string tenantId);
        Task<AppointmentDto?> GetByIdAsync(Guid appointmentId, string tenantId);
        Task<bool> MarkAppointmentAsPaidAsync(Guid appointmentId, Guid paidByUserId, string paymentReceiverType, string tenantId, decimal? paymentAmount = null, string? paymentMethod = null);
        Task<bool> CompleteAppointmentAsync(Guid appointmentId, Guid completedByUserId, string tenantId, string? notes = null, bool registerPayment = false, decimal? paymentAmount = null, string? paymentMethod = null);
    }

    public class AppointmentService : IAppointmentService
    {
        private readonly IMediator _mediator;

        public AppointmentService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<AppointmentDto> CreateAppointmentAsync(CreateAppointmentDto createAppointmentDto, string tenantId)
        {
            var command = new CreateAppointmentCommand(createAppointmentDto, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<AppointmentDto> UpdateAppointmentAsync(Guid appointmentId, UpdateAppointmentDto updateAppointmentDto, string tenantId)
        {
            var command = new UpdateAppointmentCommand(appointmentId, updateAppointmentDto, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<bool> CancelAppointmentAsync(Guid appointmentId, string cancellationReason, string tenantId)
        {
            var command = new CancelAppointmentCommand(appointmentId, cancellationReason, tenantId);
            return await _mediator.Send(command);
        }

        public async Task<DailyAgendaDto> GetDailyAgendaAsync(DateTime date, Guid clinicId, string tenantId, Guid? professionalId = null)
        {
            var query = new GetDailyAgendaQuery(date, clinicId, tenantId, professionalId);
            return await _mediator.Send(query);
        }

        public async Task<WeekAgendaDto> GetWeekAgendaAsync(DateTime startDate, DateTime endDate, Guid clinicId, string tenantId, Guid? professionalId = null)
        {
            var query = new GetWeekAgendaQuery(startDate, endDate, clinicId, tenantId, professionalId);
            return await _mediator.Send(query);
        }

        public async Task<IEnumerable<AvailableSlotDto>> GetAvailableSlotsAsync(DateTime date, Guid clinicId, int durationMinutes, string tenantId)
        {
            var query = new GetAvailableSlotsQuery(date, clinicId, durationMinutes, tenantId);
            return await _mediator.Send(query);
        }

        public async Task<AppointmentDto?> GetByIdAsync(Guid appointmentId, string tenantId)
        {
            var query = new GetAppointmentByIdQuery(appointmentId, tenantId);
            return await _mediator.Send(query);
        }

        public async Task<bool> MarkAppointmentAsPaidAsync(Guid appointmentId, Guid paidByUserId, string paymentReceiverType, string tenantId, decimal? paymentAmount = null, string? paymentMethod = null)
        {
            var command = new MarkAppointmentAsPaidCommand(appointmentId, paidByUserId, paymentReceiverType, tenantId, paymentAmount, paymentMethod);
            return await _mediator.Send(command);
        }

        public async Task<bool> CompleteAppointmentAsync(Guid appointmentId, Guid completedByUserId, string tenantId, string? notes = null, bool registerPayment = false, decimal? paymentAmount = null, string? paymentMethod = null)
        {
            var command = new CompleteAppointmentCommand(appointmentId, completedByUserId, tenantId, notes, registerPayment, paymentAmount, paymentMethod);
            return await _mediator.Send(command);
        }
    }
}