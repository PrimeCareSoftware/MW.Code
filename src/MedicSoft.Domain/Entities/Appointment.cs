using System;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        public Guid PatientId { get; private set; }
        public Guid ClinicId { get; private set; }
        public Guid? ProfessionalId { get; private set; }  // ID do médico/profissional
        public DateTime ScheduledDate { get; private set; }
        public TimeSpan ScheduledTime { get; private set; }
        public int DurationMinutes { get; private set; }
        public AppointmentType Type { get; private set; }
        public AppointmentMode Mode { get; private set; }  // Presencial ou Telemedicina
        public PaymentType PaymentType { get; private set; }  // Particular ou Convênio
        public Guid? HealthInsurancePlanId { get; private set; }  // Plano de saúde (se convênio)
        public AppointmentStatus Status { get; private set; }
        public string? Notes { get; private set; }
        public string? CancellationReason { get; private set; }
        public DateTime? CheckInTime { get; private set; }
        public DateTime? CheckOutTime { get; private set; }
        
        // Controle de pagamento
        public bool IsPaid { get; private set; }
        public DateTime? PaidAt { get; private set; }
        public Guid? PaidByUserId { get; private set; }
        public PaymentReceiverType? PaymentReceivedBy { get; private set; }

        // Propriedades de navegação
        public Patient Patient { get; private set; } = null!;
        public Clinic Clinic { get; private set; } = null!;
        public User? Professional { get; private set; }
        public HealthInsurancePlan? HealthInsurancePlan { get; private set; }

        private Appointment() 
        { 
            // Construtor do EF - avisos de nulabilidade suprimidos pois o EF Core define via reflection
        }

        public Appointment(Guid patientId, Guid clinicId, DateTime scheduledDate, 
            TimeSpan scheduledTime, int durationMinutes, AppointmentType type, 
            string tenantId, AppointmentMode mode = AppointmentMode.InPerson, 
            PaymentType paymentType = PaymentType.Private, 
            Guid? professionalId = null, Guid? healthInsurancePlanId = null,
            string? notes = null, bool allowHistoricalData = false) : base(tenantId)
        {
            if (patientId == Guid.Empty)
                throw new ArgumentException("O ID do paciente não pode estar vazio", nameof(patientId));
            
            if (clinicId == Guid.Empty)
                throw new ArgumentException("O ID da clínica não pode estar vazio", nameof(clinicId));

            if (!allowHistoricalData && scheduledDate < DateTime.Today)
                throw new ArgumentException("A data agendada não pode estar no passado", nameof(scheduledDate));

            if (durationMinutes <= 0)
                throw new ArgumentException("A duração deve ser positiva", nameof(durationMinutes));

            if (paymentType == PaymentType.HealthInsurance && !healthInsurancePlanId.HasValue)
                throw new ArgumentException("ID do plano de saúde é obrigatório quando o tipo de pagamento é convênio", nameof(healthInsurancePlanId));

            PatientId = patientId;
            ClinicId = clinicId;
            ProfessionalId = professionalId;
            ScheduledDate = scheduledDate;
            ScheduledTime = scheduledTime;
            DurationMinutes = durationMinutes;
            Type = type;
            Mode = mode;
            PaymentType = paymentType;
            HealthInsurancePlanId = healthInsurancePlanId;
            Status = AppointmentStatus.Scheduled;
            Notes = notes?.Trim();
        }

        public void Confirm()
        {
            if (Status != AppointmentStatus.Scheduled)
                throw new InvalidOperationException("Apenas agendamentos marcados podem ser confirmados");

            Status = AppointmentStatus.Confirmed;
            UpdateTimestamp();
        }

        public void Cancel(string reason)
        {
            if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.Cancelled)
                throw new InvalidOperationException("Não é possível cancelar agendamentos concluídos ou já cancelados");

            Status = AppointmentStatus.Cancelled;
            CancellationReason = reason?.Trim();
            UpdateTimestamp();
        }

        public void MarkAsNoShow()
        {
            if (Status != AppointmentStatus.Scheduled && Status != AppointmentStatus.Confirmed)
                throw new InvalidOperationException("Apenas agendamentos marcados ou confirmados podem ser marcados como ausência");

            Status = AppointmentStatus.NoShow;
            UpdateTimestamp();
        }

        public void CheckIn()
        {
            if (Status != AppointmentStatus.Confirmed && Status != AppointmentStatus.Scheduled)
                throw new InvalidOperationException("Apenas agendamentos confirmados ou marcados podem fazer check-in");

            Status = AppointmentStatus.InProgress;
            CheckInTime = DateTime.UtcNow;
            UpdateTimestamp();
        }

        public void CheckOut(string? notes = null)
        {
            if (Status != AppointmentStatus.InProgress)
                throw new InvalidOperationException("Apenas agendamentos em andamento podem fazer check-out");

            Status = AppointmentStatus.Completed;
            CheckOutTime = DateTime.UtcNow;
            if (!string.IsNullOrWhiteSpace(notes))
                Notes = notes.Trim();
            UpdateTimestamp();
        }

        public void Reschedule(DateTime newDate, TimeSpan newTime)
        {
            if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.Cancelled)
                throw new InvalidOperationException("Não é possível reagendar consultas concluídas ou canceladas");

            if (newDate < DateTime.Today)
                throw new ArgumentException("A nova data não pode estar no passado", nameof(newDate));

            ScheduledDate = newDate;
            ScheduledTime = newTime;
            Status = AppointmentStatus.Scheduled; // Reseta para agendado após reagendamento
            UpdateTimestamp();
        }

        public void UpdateDuration(int durationMinutes)
        {
            if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.Cancelled)
                throw new InvalidOperationException("Não é possível alterar a duração de consultas concluídas ou canceladas");

            if (durationMinutes <= 0)
                throw new ArgumentException("A duração deve ser positiva", nameof(durationMinutes));

            DurationMinutes = durationMinutes;
            UpdateTimestamp();
        }

        public void UpdateType(AppointmentType type)
        {
            if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.Cancelled)
                throw new InvalidOperationException("Não é possível alterar o tipo de consultas concluídas ou canceladas");

            Type = type;
            UpdateTimestamp();
        }

        public void UpdateProfessional(Guid? professionalId)
        {
            if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.Cancelled)
                throw new InvalidOperationException("Não é possível alterar o profissional de consultas concluídas ou canceladas");

            ProfessionalId = professionalId;
            UpdateTimestamp();
        }

        public void UpdateMode(AppointmentMode mode)
        {
            if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.Cancelled)
                throw new InvalidOperationException("Não é possível alterar o modo de consultas concluídas ou canceladas");

            Mode = mode;
            UpdateTimestamp();
        }

        public void UpdatePaymentInfo(PaymentType paymentType, Guid? healthInsurancePlanId = null)
        {
            if (Status == AppointmentStatus.Completed || Status == AppointmentStatus.Cancelled)
                throw new InvalidOperationException("Não é possível alterar as informações de pagamento de consultas concluídas ou canceladas");

            if (paymentType == PaymentType.HealthInsurance && !healthInsurancePlanId.HasValue)
                throw new ArgumentException("ID do plano de saúde é obrigatório quando o tipo de pagamento é convênio", nameof(healthInsurancePlanId));

            PaymentType = paymentType;
            HealthInsurancePlanId = healthInsurancePlanId;
            UpdateTimestamp();
        }

        public void UpdateNotes(string notes)
        {
            Notes = notes?.Trim();
            UpdateTimestamp();
        }

        public DateTime GetScheduledDateTime()
        {
            return ScheduledDate.Add(ScheduledTime);
        }

        public DateTime GetEndDateTime()
        {
            return GetScheduledDateTime().AddMinutes(DurationMinutes);
        }

        public bool IsOverlapping(DateTime start, DateTime end)
        {
            var appointmentStart = GetScheduledDateTime();
            var appointmentEnd = GetEndDateTime();
            
            return appointmentStart < end && appointmentEnd > start;
        }

        public void MarkAsPaid(Guid paidByUserId, PaymentReceiverType receiverType)
        {
            if (IsPaid)
                throw new InvalidOperationException("Payment has already been registered for this appointment");

            IsPaid = true;
            PaidAt = DateTime.UtcNow;
            PaidByUserId = paidByUserId;
            PaymentReceivedBy = receiverType;
            UpdateTimestamp();
        }

        public void UnmarkAsPaid()
        {
            if (!IsPaid)
                throw new InvalidOperationException("Appointment is not marked as paid");

            IsPaid = false;
            PaidAt = null;
            PaidByUserId = null;
            PaymentReceivedBy = null;
            UpdateTimestamp();
        }
    }
}