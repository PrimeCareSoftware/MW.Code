using System;
using MediatR;

namespace MedicSoft.Application.Commands.Appointments
{
    /// <summary>
    /// Command para marcar um atendimento como pago
    /// </summary>
    public class MarkAppointmentAsPaidCommand : IRequest<bool>
    {
        public Guid AppointmentId { get; }
        public Guid PaidByUserId { get; }
        public string PaymentReceiverType { get; } // Doctor, Secretary, Other
        public decimal? PaymentAmount { get; }
        public string? PaymentMethod { get; }
        public string TenantId { get; }

        public MarkAppointmentAsPaidCommand(Guid appointmentId, Guid paidByUserId, 
            string paymentReceiverType, string tenantId, decimal? paymentAmount = null, string? paymentMethod = null)
        {
            AppointmentId = appointmentId;
            PaidByUserId = paidByUserId;
            PaymentReceiverType = paymentReceiverType;
            PaymentAmount = paymentAmount;
            PaymentMethod = paymentMethod;
            TenantId = tenantId;
        }
    }

    /// <summary>
    /// Command para finalizar um atendimento (check-out pelo médico)
    /// </summary>
    public class CompleteAppointmentCommand : IRequest<bool>
    {
        public Guid AppointmentId { get; }
        public Guid CompletedByUserId { get; }
        public string? Notes { get; }
        public bool RegisterPayment { get; }
        public decimal? PaymentAmount { get; }
        public string? PaymentMethod { get; }
        public string TenantId { get; }

        public CompleteAppointmentCommand(Guid appointmentId, Guid completedByUserId, 
            string tenantId, string? notes = null, bool registerPayment = false, 
            decimal? paymentAmount = null, string? paymentMethod = null)
        {
            AppointmentId = appointmentId;
            CompletedByUserId = completedByUserId;
            Notes = notes;
            RegisterPayment = registerPayment;
            PaymentAmount = paymentAmount;
            PaymentMethod = paymentMethod;
            TenantId = tenantId;
        }
    }

    /// <summary>
    /// Command para atualizar a configuração de tipo de recebedor de pagamento da clínica
    /// </summary>
    public class UpdateClinicPaymentReceiverCommand : IRequest<bool>
    {
        public Guid ClinicId { get; }
        public string PaymentReceiverType { get; } // Doctor, Secretary, Other
        public string TenantId { get; }

        public UpdateClinicPaymentReceiverCommand(Guid clinicId, string paymentReceiverType, string tenantId)
        {
            ClinicId = clinicId;
            PaymentReceiverType = paymentReceiverType;
            TenantId = tenantId;
        }
    }
}
