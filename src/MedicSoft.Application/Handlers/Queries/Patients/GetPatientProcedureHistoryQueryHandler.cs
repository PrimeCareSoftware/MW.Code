using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Patients;
using MedicSoft.Repository.Context;

namespace MedicSoft.Application.Handlers.Queries.Patients
{
    public class GetPatientProcedureHistoryQueryHandler 
        : IRequestHandler<GetPatientProcedureHistoryQuery, IEnumerable<PatientProcedureHistoryDto>>
    {
        private readonly MedicSoftDbContext _context;

        public GetPatientProcedureHistoryQueryHandler(MedicSoftDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<PatientProcedureHistoryDto>> Handle(
            GetPatientProcedureHistoryQuery request, 
            CancellationToken cancellationToken)
        {
            // Get all appointment procedures for this patient
            var appointmentProcedures = await _context.AppointmentProcedures
                .Where(ap => ap.PatientId == request.PatientId && ap.TenantId == request.TenantId)
                .Include(ap => ap.Procedure)
                .Include(ap => ap.Appointment)
                .OrderByDescending(ap => ap.PerformedAt)
                .ToListAsync(cancellationToken);

            var appointmentIds = appointmentProcedures
                .Select(ap => ap.AppointmentId)
                .Distinct()
                .ToList();

            // Get payments for these appointments
            var payments = await _context.Payments
                .Where(p => appointmentIds.Contains(p.AppointmentId!.Value))
                .ToListAsync(cancellationToken);

            // Build procedure history DTOs
            var procedureHistory = appointmentProcedures.Select(ap =>
            {
                var payment = payments.FirstOrDefault(p => p.AppointmentId == ap.AppointmentId);

                return new PatientProcedureHistoryDto
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
            }).ToList();

            return procedureHistory;
        }
    }
}
