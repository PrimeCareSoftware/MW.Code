using System;
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
    public class GetPatientAppointmentHistoryQueryHandler 
        : IRequestHandler<GetPatientAppointmentHistoryQuery, PatientCompleteHistoryDto>
    {
        private readonly IPatientRepository _patientRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMedicalRecordRepository _medicalRecordRepository;

        public GetPatientAppointmentHistoryQueryHandler(
            IPatientRepository patientRepository,
            IAppointmentRepository appointmentRepository,
            IPaymentRepository paymentRepository,
            IMedicalRecordRepository medicalRecordRepository)
        {
            _patientRepository = patientRepository;
            _appointmentRepository = appointmentRepository;
            _paymentRepository = paymentRepository;
            _medicalRecordRepository = medicalRecordRepository;
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

            // Get appointments for this patient
            var appointments = await _appointmentRepository.GetByPatientIdAsync(request.PatientId, request.TenantId);
            var appointmentList = appointments.ToList();

            // Build history DTOs
            var appointmentHistory = new List<PatientAppointmentHistoryDto>();
            
            foreach (var appointment in appointmentList)
            {
                // Get payment for this appointment
                var payments = await _paymentRepository.GetByAppointmentIdAsync(appointment.Id);
                var payment = payments.FirstOrDefault();

                // Get medical record if requested
                Domain.Entities.MedicalRecord? medicalRecord = null;
                if (request.IncludeMedicalRecords)
                {
                    medicalRecord = await _medicalRecordRepository.GetByAppointmentIdAsync(appointment.Id, request.TenantId);
                }

                var historyEntry = new PatientAppointmentHistoryDto
                {
                    AppointmentId = appointment.Id,
                    ScheduledDate = appointment.ScheduledDate,
                    ScheduledTime = appointment.ScheduledTime,
                    Status = appointment.Status.ToString(),
                    Type = appointment.Type.ToString(),
                    DoctorName = null, // Will be populated from clinic staff if available
                    DoctorSpecialty = null,
                    DoctorProfessionalId = null,
                    CheckInTime = appointment.CheckInTime,
                    CheckOutTime = appointment.CheckOutTime,
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

                appointmentHistory.Add(historyEntry);
            }

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
