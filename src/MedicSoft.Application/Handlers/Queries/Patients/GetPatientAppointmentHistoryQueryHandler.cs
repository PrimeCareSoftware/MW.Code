using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using MedicSoft.Application.DTOs;
using MedicSoft.Application.Queries.Patients;
using MedicSoft.Domain.Interfaces;
using MedicSoft.Repository.Context;

namespace MedicSoft.Application.Handlers.Queries.Patients
{
    public class GetPatientAppointmentHistoryQueryHandler 
        : IRequestHandler<GetPatientAppointmentHistoryQuery, PatientCompleteHistoryDto>
    {
        private readonly MedicSoftDbContext _context;
        private readonly IPatientRepository _patientRepository;

        public GetPatientAppointmentHistoryQueryHandler(
            MedicSoftDbContext context,
            IPatientRepository patientRepository)
        {
            _context = context;
            _patientRepository = patientRepository;
        }

        public async Task<PatientCompleteHistoryDto> Handle(
            GetPatientAppointmentHistoryQuery request, 
            CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetByIdAsync(request.PatientId, request.TenantId);
            
            if (patient == null)
            {
                throw new InvalidOperationException($"Patient with ID {request.PatientId} not found");
            }

            // Get appointments with related data
            var appointments = await _context.Appointments
                .Where(a => a.PatientId == request.PatientId && a.TenantId == request.TenantId)
                .Include(a => a.Clinic)
                .OrderByDescending(a => a.ScheduledDate)
                .ThenByDescending(a => a.ScheduledTime)
                .ToListAsync(cancellationToken);

            var appointmentIds = appointments.Select(a => a.Id).ToList();

            // Get payments for these appointments
            var payments = await _context.Payments
                .Where(p => appointmentIds.Contains(p.AppointmentId!.Value))
                .ToListAsync(cancellationToken);

            // Get medical records if requested
            Dictionary<Guid, Domain.Entities.MedicalRecord> medicalRecords = new();
            if (request.IncludeMedicalRecords)
            {
                var records = await _context.MedicalRecords
                    .Where(mr => appointmentIds.Contains(mr.AppointmentId))
                    .ToListAsync(cancellationToken);
                
                medicalRecords = records.ToDictionary(mr => mr.AppointmentId);
            }

            // Build history DTOs
            var appointmentHistory = appointments.Select(a =>
            {
                var payment = payments.FirstOrDefault(p => p.AppointmentId == a.Id);
                var medicalRecord = request.IncludeMedicalRecords && medicalRecords.ContainsKey(a.Id) 
                    ? medicalRecords[a.Id] 
                    : null;

                return new PatientAppointmentHistoryDto
                {
                    AppointmentId = a.Id,
                    ScheduledDate = a.ScheduledDate,
                    ScheduledTime = a.ScheduledTime,
                    Status = a.Status.ToString(),
                    Type = a.Type.ToString(),
                    DoctorName = null, // Will be populated from clinic staff if available
                    DoctorSpecialty = null,
                    DoctorProfessionalId = null,
                    CheckInTime = a.CheckInTime,
                    CheckOutTime = a.CheckOutTime,
                    Payment = payment != null ? new PaymentHistoryDto
                    {
                        PaymentId = payment.Id,
                        Amount = payment.Amount,
                        Method = payment.Method.ToString(),
                        Status = payment.Status.ToString(),
                        PaymentDate = payment.PaymentDate,
                        CardLastFourDigits = payment.CardLastFourDigits,
                        PixKey = payment.PixKey
                    } : null,
                    MedicalRecord = medicalRecord != null ? new MedicalRecordSummaryDto
                    {
                        MedicalRecordId = medicalRecord.Id,
                        Diagnosis = medicalRecord.Diagnosis,
                        ConsultationDurationMinutes = medicalRecord.ConsultationDurationMinutes,
                        CreatedAt = medicalRecord.CreatedAt
                    } : null
                };
            }).ToList();

            return new PatientCompleteHistoryDto
            {
                PatientId = patient.Id,
                PatientName = patient.Name,
                Appointments = appointmentHistory,
                Procedures = new List<PatientProcedureHistoryDto>() // Will be populated by separate query
            };
        }
    }
}
