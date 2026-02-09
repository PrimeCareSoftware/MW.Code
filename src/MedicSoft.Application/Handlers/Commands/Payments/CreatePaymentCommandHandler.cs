using AutoMapper;
using MediatR;
using MedicSoft.Application.Commands.Payments;
using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Interfaces;

namespace MedicSoft.Application.Handlers.Commands.Payments
{
    public class CreatePaymentCommandHandler : IRequestHandler<CreatePaymentCommand, PaymentDto>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IMapper _mapper;

        public CreatePaymentCommandHandler(
            IPaymentRepository paymentRepository,
            IAppointmentRepository appointmentRepository,
            IMapper mapper)
        {
            _paymentRepository = paymentRepository;
            _appointmentRepository = appointmentRepository;
            _mapper = mapper;
        }

        public async Task<PaymentDto> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            // Validate appointment or subscription exists
            if (request.Payment.AppointmentId.HasValue)
            {
                var appointment = await _appointmentRepository.GetByIdAsync(
                    request.Payment.AppointmentId.Value, 
                    request.TenantId);
                
                if (appointment == null)
                {
                    throw new InvalidOperationException("Appointment not found");
                }
            }

            // Parse payment method
            if (!Enum.TryParse<PaymentMethod>(request.Payment.Method, true, out var paymentMethod))
            {
                paymentMethod = PaymentMethod.Cash;
            }

            // Create payment
            var payment = new Payment(
                request.Payment.Amount,
                paymentMethod,
                request.TenantId,
                request.Payment.AppointmentId,
                request.Payment.ClinicSubscriptionId,
                null, // appointmentProcedureId
                request.Payment.Notes
            );

            // Set payment method specific details
            if ((paymentMethod == PaymentMethod.CreditCard || paymentMethod == PaymentMethod.DebitCard) 
                && !string.IsNullOrWhiteSpace(request.Payment.CardLastFourDigits))
            {
                payment.SetCardDetails(request.Payment.CardLastFourDigits);
            }
            else if (paymentMethod == PaymentMethod.Pix && !string.IsNullOrWhiteSpace(request.Payment.PixKey))
            {
                payment.SetPixDetails(request.Payment.PixKey, string.Empty);
            }

            await _paymentRepository.AddAsync(payment);

            return _mapper.Map<PaymentDto>(payment);
        }
    }
}
