using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MedicSoft.Application.Commands.Appointments;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Appointments
{
    /// <summary>
    /// Handler para marcar atendimento como pago
    /// </summary>
    public class MarkAppointmentAsPaidCommandHandler : IRequestHandler<MarkAppointmentAsPaidCommand, bool>
    {
        private readonly IAppointmentRepository _appointmentRepository;

        public MarkAppointmentAsPaidCommandHandler(IAppointmentRepository appointmentRepository)
        {
            _appointmentRepository = appointmentRepository;
        }

        public async Task<bool> Handle(MarkAppointmentAsPaidCommand request, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId, request.TenantId);
            if (appointment == null)
            {
                return false;
            }

            // Parse PaymentReceiverType
            if (!Enum.TryParse<PaymentReceiverType>(request.PaymentReceiverType, out var receiverType))
            {
                throw new ArgumentException($"Invalid PaymentReceiverType: {request.PaymentReceiverType}");
            }

            // Parse PaymentMethod if provided
            PaymentMethod? paymentMethod = null;
            if (!string.IsNullOrWhiteSpace(request.PaymentMethod))
            {
                if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, out var parsedMethod))
                {
                    throw new ArgumentException($"Invalid PaymentMethod: {request.PaymentMethod}");
                }
                paymentMethod = parsedMethod;
            }

            appointment.MarkAsPaid(request.PaidByUserId, receiverType, request.PaymentAmount, paymentMethod);
            await _appointmentRepository.UpdateAsync(appointment);
            
            return true;
        }
    }

    /// <summary>
    /// Handler para finalizar atendimento (check-out pelo médico)
    /// </summary>
    public class CompleteAppointmentCommandHandler : IRequestHandler<CompleteAppointmentCommand, bool>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IClinicRepository _clinicRepository;

        public CompleteAppointmentCommandHandler(
            IAppointmentRepository appointmentRepository,
            IClinicRepository clinicRepository)
        {
            _appointmentRepository = appointmentRepository;
            _clinicRepository = clinicRepository;
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

            // Se deve registrar pagamento e ainda não foi pago
            if (request.RegisterPayment && !appointment.IsPaid)
            {
                // Busca a configuração da clínica para saber o tipo padrão
                var clinic = await _clinicRepository.GetByIdAsync(appointment.ClinicId, request.TenantId);
                var receiverType = clinic?.DefaultPaymentReceiverType ?? PaymentReceiverType.Doctor;
                
                // Parse PaymentMethod if provided
                PaymentMethod? paymentMethod = null;
                if (!string.IsNullOrWhiteSpace(request.PaymentMethod))
                {
                    if (!Enum.TryParse<PaymentMethod>(request.PaymentMethod, out var parsedMethod))
                    {
                        throw new ArgumentException($"Invalid PaymentMethod: {request.PaymentMethod}");
                    }
                    paymentMethod = parsedMethod;
                }
                
                appointment.MarkAsPaid(request.CompletedByUserId, receiverType, request.PaymentAmount, paymentMethod);
            }

            await _appointmentRepository.UpdateAsync(appointment);
            
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
