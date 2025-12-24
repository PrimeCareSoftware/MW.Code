using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Patients;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Queries.Patients
{
    public class GetPatientProcedureHistoryQueryHandler 
        : IRequestHandler<GetPatientProcedureHistoryQuery, IEnumerable<PatientProcedureHistoryDto>>
    {
        private readonly IAppointmentProcedureRepository _appointmentProcedureRepository;
        private readonly IPaymentRepository _paymentRepository;

        public GetPatientProcedureHistoryQueryHandler(
            IAppointmentProcedureRepository appointmentProcedureRepository,
            IPaymentRepository paymentRepository)
        {
            _appointmentProcedureRepository = appointmentProcedureRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<IEnumerable<PatientProcedureHistoryDto>> Handle(
            GetPatientProcedureHistoryQuery request, 
            CancellationToken cancellationToken)
        {
            // Get all appointment procedures for this patient
            var appointmentProcedures = await _appointmentProcedureRepository.GetByPatientIdAsync(request.PatientId, request.TenantId);
            var procedureList = appointmentProcedures.ToList();

            // Build procedure history DTOs
            var procedureHistory = new List<PatientProcedureHistoryDto>();

            foreach (var ap in procedureList)
            {
                // Get payment for this appointment
                var payments = await _paymentRepository.GetByAppointmentIdAsync(ap.AppointmentId);
                var payment = payments.FirstOrDefault();

                var historyEntry = new PatientProcedureHistoryDto
                {
                    ProcedureId = ap.Id,
                    AppointmentId = ap.AppointmentId,
                    ProcedureName = ap.Procedure?.Name ?? "N/A",
                    ProcedureCode = ap.Procedure?.Code ?? "N/A",
                    ProcedureCategory = ap.Procedure?.Category.ToString() ?? "N/A",
                    PriceCharged = ap.PriceCharged,
                    PerformedAt = ap.PerformedAt,
                    Notes = ap.Notes,
                    DoctorName = null, // Will be populated if available
                    DoctorSpecialty = null,
                    Payment = payment != null ? new PaymentHistoryDto
                    {
                        PaymentId = payment.Id,
                        Amount = payment.Amount,
                        Method = payment.Method.ToString(),
                        Status = payment.Status.ToString(),
                        PaymentDate = payment.PaymentDate,
                        CardLastFourDigits = payment.CardLastFourDigits,
                        PixKey = payment.PixKey
                    } : null
                };

                procedureHistory.Add(historyEntry);
            }

            return procedureHistory;
        }
    }
}
