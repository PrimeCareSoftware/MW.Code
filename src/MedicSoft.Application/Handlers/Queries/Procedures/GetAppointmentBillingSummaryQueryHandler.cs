using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Procedures;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Procedures
{
    public class GetAppointmentBillingSummaryQueryHandler : IRequestHandler<GetAppointmentBillingSummaryQuery, AppointmentBillingSummaryDto?>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IAppointmentProcedureRepository _appointmentProcedureRepository;
        private readonly IPaymentRepository _paymentRepository;

        public GetAppointmentBillingSummaryQueryHandler(
            IAppointmentRepository appointmentRepository,
            IAppointmentProcedureRepository appointmentProcedureRepository,
            IPaymentRepository paymentRepository)
        {
            _appointmentRepository = appointmentRepository;
            _appointmentProcedureRepository = appointmentProcedureRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<AppointmentBillingSummaryDto?> Handle(GetAppointmentBillingSummaryQuery request, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, request.TenantId);
            if (appointment == null)
            {
                return null;
            }

            var procedures = await _appointmentProcedureRepository.GetByAppointmentIdAsync(request.AppointmentId, request.TenantId);
            var procedureDtos = procedures.Select(ap => new AppointmentProcedureDto
            {
                Id = ap.Id,
                AppointmentId = ap.AppointmentId,
                ProcedureId = ap.ProcedureId,
                PatientId = ap.PatientId,
                ProcedureName = ap.Procedure?.Name ?? string.Empty,
                ProcedureCode = ap.Procedure?.Code ?? string.Empty,
                PriceCharged = ap.PriceCharged,
                Notes = ap.Notes,
                PerformedAt = ap.PerformedAt
            }).ToList();

            var subTotal = procedureDtos.Sum(p => p.PriceCharged);
            var taxAmount = 0m; // Tax calculation can be customized per clinic
            var total = subTotal + taxAmount;

            // Check if there's a payment for this appointment
            var payments = await _paymentRepository.GetByAppointmentIdAsync(request.AppointmentId);
            var payment = payments.FirstOrDefault();

            return new AppointmentBillingSummaryDto
            {
                AppointmentId = appointment.Id,
                PatientId = appointment.PatientId,
                PatientName = appointment.Patient?.Name ?? string.Empty,
                AppointmentDate = appointment.GetScheduledDateTime(),
                Procedures = procedureDtos,
                SubTotal = subTotal,
                TaxAmount = taxAmount,
                Total = total,
                PaymentStatus = payment?.Status
            };
        }
    }
}
