using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MedicSoft.Application.Commands.Appointments;
using MedicSoft.Application.Services;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Appointments
{
    /// <summary>
    /// Handler para marcar atendimento como pago
    /// Now integrates with PaymentFlowService to create Payment entity and Invoice automatically
    /// </summary>
    public class MarkAppointmentAsPaidCommandHandler : IRequestHandler<MarkAppointmentAsPaidCommand, bool>
    {
        private readonly IPaymentFlowService _paymentFlowService;

        public MarkAppointmentAsPaidCommandHandler(IPaymentFlowService paymentFlowService)
        {
            _paymentFlowService = paymentFlowService;
        }

        public async Task<bool> Handle(MarkAppointmentAsPaidCommand request, CancellationToken cancellationToken)
        {
            // Validate that payment amount and method are provided
            if (!request.PaymentAmount.HasValue || request.PaymentAmount.Value <= 0)
            {
                throw new ArgumentException("Payment amount is required and must be greater than zero");
            }

            if (string.IsNullOrWhiteSpace(request.PaymentMethod))
            {
                throw new ArgumentException("Payment method is required");
            }

            // Use PaymentFlowService to orchestrate complete payment flow
            // This will: 1) Mark appointment as paid, 2) Create Payment entity, 3) Generate Invoice
            var result = await _paymentFlowService.RegisterAppointmentPaymentAsync(
                request.AppointmentId,
                request.PaidByUserId,
                request.PaymentReceiverType,
                request.PaymentAmount.Value,
                request.PaymentMethod,
                request.TenantId
            );

            if (!result.Success)
            {
                throw new InvalidOperationException(result.ErrorMessage ?? "Failed to process payment");
            }

            return true;
        }
    }

    /// <summary>
    /// Handler para finalizar atendimento (check-out pelo médico)
    /// Now integrates with PaymentFlowService for complete payment integration
    /// </summary>
    public class CompleteAppointmentCommandHandler : IRequestHandler<CompleteAppointmentCommand, bool>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPaymentFlowService _paymentFlowService;

        public CompleteAppointmentCommandHandler(
            IAppointmentRepository appointmentRepository,
            IPaymentFlowService paymentFlowService)
        {
            _appointmentRepository = appointmentRepository;
            _paymentFlowService = paymentFlowService;
        }

        public async Task<bool> Handle(CompleteAppointmentCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, request.TenantId);
            if (appointment == null)
            {
                return false;
            }

            // Finaliza o atendimento (check-out)
            appointment.CheckOut(request.Notes);
            await _appointmentRepository.UpdateAsync(appointment);

            // Se deve registrar pagamento e ainda não foi pago
            if (request.RegisterPayment && !appointment.IsPaid)
            {
                // Validate payment data
                if (!request.PaymentAmount.HasValue || request.PaymentAmount.Value <= 0)
                {
                    throw new ArgumentException("Payment amount is required and must be greater than zero when registering payment");
                }

                if (string.IsNullOrWhiteSpace(request.PaymentMethod))
                {
                    throw new ArgumentException("Payment method is required when registering payment");
                }

                // Use PaymentFlowService to orchestrate complete payment flow
                var result = await _paymentFlowService.RegisterPaymentOnCompletionAsync(
                    request.AppointmentId,
                    request.CompletedByUserId,
                    request.PaymentAmount.Value,
                    request.PaymentMethod,
                    request.TenantId,
                    request.Notes
                );

                if (!result.Success)
                {
                    throw new InvalidOperationException(result.ErrorMessage ?? "Failed to process payment on completion");
                }
            }
            
            return true;
        }
    }

    /// <summary>
    /// Handler para atualizar configuração de pagamento da clínica
    /// </summary>
    public class UpdateClinicPaymentReceiverCommandHandler : IRequestHandler<UpdateClinicPaymentReceiverCommand, bool>
    {
        private readonly IClinicRepository _clinicRepository;

        public UpdateClinicPaymentReceiverCommandHandler(IClinicRepository clinicRepository)
        {
            _clinicRepository = clinicRepository;
        }

        public async Task<bool> Handle(UpdateClinicPaymentReceiverCommand request, CancellationToken cancellationToken)
        {
            var clinic = await _clinicRepository.GetByIdAsync(request.ClinicId, request.TenantId);
            if (clinic == null)
            {
                return false;
            }

            // Parse PaymentReceiverType
            if (!Enum.TryParse<PaymentReceiverType>(request.PaymentReceiverType, out var receiverType))
            {
                throw new ArgumentException($"Invalid PaymentReceiverType: {request.PaymentReceiverType}");
            }

            clinic.UpdatePaymentReceiverType(receiverType);
            await _clinicRepository.UpdateAsync(clinic);
            
            return true;
        }
    }
}
