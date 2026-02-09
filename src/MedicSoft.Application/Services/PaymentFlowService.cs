using MedicSoft.Application.DTOs;
using MedicSoft.Domain.Entities;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace MedicSoft.Application.Services
{
    /// <summary>
    /// Orchestrates the complete payment flow from appointment to invoice
    /// Ensures all related entities (Payment, Invoice, TISS) are created and linked properly
    /// </summary>
    public class PaymentFlowService : IPaymentFlowService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IInvoiceRepository _invoiceRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IClinicRepository _clinicRepository;
        private readonly IAppointmentProcedureRepository _appointmentProcedureRepository;
        private readonly IClinicPricingConfigurationRepository _clinicPricingConfigurationRepository;
        private readonly IProcedurePricingConfigurationRepository _procedurePricingConfigurationRepository;
        private readonly ITissGuideService _tissGuideService;
        private readonly ILogger<PaymentFlowService> _logger;

        public PaymentFlowService(
            IAppointmentRepository appointmentRepository,
            IPaymentRepository paymentRepository,
            IInvoiceRepository invoiceRepository,
            IPatientRepository patientRepository,
            IClinicRepository clinicRepository,
            IAppointmentProcedureRepository appointmentProcedureRepository,
            IClinicPricingConfigurationRepository clinicPricingConfigurationRepository,
            IProcedurePricingConfigurationRepository procedurePricingConfigurationRepository,
            ITissGuideService tissGuideService,
            ILogger<PaymentFlowService> logger)
        {
            _appointmentRepository = appointmentRepository;
            _paymentRepository = paymentRepository;
            _invoiceRepository = invoiceRepository;
            _patientRepository = patientRepository;
            _clinicRepository = clinicRepository;
            _appointmentProcedureRepository = appointmentProcedureRepository;
            _clinicPricingConfigurationRepository = clinicPricingConfigurationRepository;
            _procedurePricingConfigurationRepository = procedurePricingConfigurationRepository;
            _tissGuideService = tissGuideService;
            _logger = logger;
        }

        public async Task<PaymentFlowResultDto> RegisterAppointmentPaymentAsync(
            Guid appointmentId,
            Guid paidByUserId,
            string paymentReceiverType,
            decimal amount,
            string paymentMethod,
            string tenantId,
            string? notes = null)
        {
            var result = new PaymentFlowResultDto
            {
                AppointmentId = appointmentId,
                ProcessedAt = DateTime.UtcNow
            };

            try
            {
                // 1. Get appointment
                var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, tenantId);
                if (appointment == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "Appointment not found";
                    return result;
                }

                // 2. Parse enums
                if (!Enum.TryParse<PaymentReceiverType>(paymentReceiverType, out var receiverType))
                {
                    result.Success = false;
                    result.ErrorMessage = $"Invalid PaymentReceiverType: {paymentReceiverType}";
                    return result;
                }

                if (!Enum.TryParse<PaymentMethod>(paymentMethod, out var method))
                {
                    result.Success = false;
                    result.ErrorMessage = $"Invalid PaymentMethod: {paymentMethod}";
                    return result;
                }

                // 3. Mark appointment as paid
                appointment.MarkAsPaid(paidByUserId, receiverType, amount, method);
                await _appointmentRepository.UpdateAsync(appointment);

                // 4. Create Payment entity
                var payment = new Payment(
                    amount,
                    method,
                    tenantId,
                    appointmentId: appointmentId,
                    notes: notes
                );
                await _paymentRepository.AddAsync(payment);
                result.PaymentId = payment.Id;

                // 5. Mark payment as processed
                payment.MarkAsPaid(transactionId: $"APT-{appointmentId:N}");
                await _paymentRepository.UpdateAsync(payment);

                // 6. Create Invoice
                var invoice = await CreateInvoiceForPaymentAsync(payment, appointment, tenantId);
                if (invoice != null)
                {
                    result.InvoiceId = invoice.Id;
                }

                // 7. Create TISS guide if health insurance
                if (appointment.PaymentType == PaymentType.HealthInsurance && appointment.HealthInsurancePlanId.HasValue)
                {
                    try
                    {
                        result.TissGuideId = await CreateTissGuideForAppointmentAsync(appointment, payment, tenantId);
                        
                        // Link TISS guide to invoice if both exist
                        if (invoice != null && result.TissGuideId.HasValue)
                        {
                            invoice.LinkToTissGuide(result.TissGuideId.Value);
                            await _invoiceRepository.UpdateAsync(invoice);
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to create TISS guide for appointment {AppointmentId}", appointmentId);
                        // Don't fail the entire flow if TISS creation fails
                    }
                }

                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Error processing payment: {ex.Message}";
                return result;
            }
        }

        public async Task<PaymentFlowResultDto> RegisterPaymentOnCompletionAsync(
            Guid appointmentId,
            Guid completedByUserId,
            decimal amount,
            string paymentMethod,
            string tenantId,
            string? notes = null)
        {
            // Get clinic to determine default receiver type
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, tenantId);
            if (appointment == null)
            {
                return new PaymentFlowResultDto
                {
                    AppointmentId = appointmentId,
                    Success = false,
                    ErrorMessage = "Appointment not found",
                    ProcessedAt = DateTime.UtcNow
                };
            }

            var clinic = await _clinicRepository.GetByIdAsync(appointment.ClinicId, tenantId);
            var receiverType = clinic?.DefaultPaymentReceiverType ?? PaymentReceiverType.Doctor;

            // Use the main registration method
            return await RegisterAppointmentPaymentAsync(
                appointmentId,
                completedByUserId,
                receiverType.ToString(),
                amount,
                paymentMethod,
                tenantId,
                notes
            );
        }

        private async Task<Invoice?> CreateInvoiceForPaymentAsync(
            Payment payment,
            Appointment appointment,
            string tenantId)
        {
            try
            {
                // Get patient information
                var patient = await _patientRepository.GetByIdAsync(appointment.PatientId, tenantId);
                if (patient == null)
                {
                    return null;
                }

                // Generate invoice number
                var invoiceNumber = GenerateInvoiceNumber(appointment.ClinicId, tenantId);

                // Calculate tax (simplified - in production this should be configurable)
                var taxRate = 0.0m; // No tax for medical services in most cases
                var taxAmount = payment.Amount * taxRate;

                // Create invoice
                var invoice = new Invoice(
                    invoiceNumber: invoiceNumber,
                    paymentId: payment.Id,
                    type: InvoiceType.Appointment,
                    amount: payment.Amount,
                    taxAmount: taxAmount,
                    dueDate: DateTime.UtcNow, // Already paid
                    customerName: patient.Name,
                    tenantId: tenantId,
                    description: $"Consulta médica - {appointment.ScheduledDate:dd/MM/yyyy}",
                    customerDocument: patient.Document
                );

                await _invoiceRepository.AddAsync(invoice);

                // Issue the invoice immediately since payment is already received
                invoice.Issue();
                invoice.MarkAsPaid(DateTime.UtcNow);
                await _invoiceRepository.UpdateAsync(invoice);

                return invoice;
            }
            catch (Exception ex)
            {
                // Log error but don't fail the payment process
                // Invoice is optional - payment success is what matters
                _logger.LogError(ex, "Failed to create invoice for payment {PaymentId}", payment.Id);
                return null;
            }
        }

        private string GenerateInvoiceNumber(Guid clinicId, string tenantId)
        {
            // Generate a unique invoice number using timestamp + random suffix
            // Format: INV-YYYYMMDD-HHMMSS-XXXX
            // Example: INV-20260123-143022-A7F3
            var timestamp = DateTime.UtcNow;
            var datePart = timestamp.ToString("yyyyMMdd");
            var timePart = timestamp.ToString("HHmmss");
            var randomSuffix = Guid.NewGuid().ToString("N")[..4].ToUpper();
            return $"INV-{datePart}-{timePart}-{randomSuffix}";
        }
        
        /// <summary>
        /// Registers payment for a specific procedure performed during an appointment
        /// </summary>
        public async Task<PaymentFlowResultDto> RegisterProcedurePaymentAsync(
            Guid appointmentProcedureId,
            Guid paidByUserId,
            string paymentMethod,
            string tenantId,
            string? notes = null)
        {
            var result = new PaymentFlowResultDto
            {
                ProcessedAt = DateTime.UtcNow
            };

            try
            {
                // Get appointment procedure
                var appointmentProcedure = await _appointmentProcedureRepository.GetByIdAsync(appointmentProcedureId, tenantId);
                if (appointmentProcedure == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "Appointment procedure not found";
                    return result;
                }

                // Get appointment
                var appointment = await _appointmentRepository.GetByIdAsync(appointmentProcedure.AppointmentId, tenantId);
                if (appointment == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "Appointment not found";
                    return result;
                }

                if (!Enum.TryParse<PaymentMethod>(paymentMethod, out var method))
                {
                    result.Success = false;
                    result.ErrorMessage = $"Invalid PaymentMethod: {paymentMethod}";
                    return result;
                }

                // Create Payment entity for procedure
                var payment = new Payment(
                    appointmentProcedure.PriceCharged,
                    method,
                    tenantId,
                    appointmentProcedureId: appointmentProcedureId,
                    notes: notes
                );
                await _paymentRepository.AddAsync(payment);
                result.PaymentId = payment.Id;

                // Mark payment as processed
                payment.MarkAsPaid(transactionId: $"PROC-{appointmentProcedureId:N}");
                await _paymentRepository.UpdateAsync(payment);

                // Create Invoice
                var invoice = await CreateInvoiceForProcedurePaymentAsync(payment, appointmentProcedure, appointment, tenantId);
                if (invoice != null)
                {
                    result.InvoiceId = invoice.Id;
                }

                result.Success = true;
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Error processing procedure payment: {ex.Message}";
                return result;
            }
        }
        
        private async Task<Invoice?> CreateInvoiceForProcedurePaymentAsync(
            Payment payment,
            AppointmentProcedure appointmentProcedure,
            Appointment appointment,
            string tenantId)
        {
            try
            {
                var patient = await _patientRepository.GetByIdAsync(appointment.PatientId, tenantId);
                if (patient == null)
                {
                    return null;
                }

                var invoiceNumber = GenerateInvoiceNumber(appointment.ClinicId, tenantId);
                var taxRate = 0.0m;
                var taxAmount = payment.Amount * taxRate;

                var invoice = new Invoice(
                    invoiceNumber: invoiceNumber,
                    paymentId: payment.Id,
                    type: InvoiceType.Service,
                    amount: payment.Amount,
                    taxAmount: taxAmount,
                    dueDate: DateTime.UtcNow,
                    customerName: patient.Name,
                    tenantId: tenantId,
                    description: $"Procedimento - {appointment.ScheduledDate:dd/MM/yyyy}",
                    customerDocument: patient.Document
                );

                await _invoiceRepository.AddAsync(invoice);

                invoice.Issue();
                invoice.MarkAsPaid(DateTime.UtcNow);
                await _invoiceRepository.UpdateAsync(invoice);

                return invoice;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create invoice for procedure payment {PaymentId}", payment.Id);
                return null;
            }
        }
        
        /// <summary>
        /// Creates a TISS guide for a health insurance appointment
        /// </summary>
        private async Task<Guid?> CreateTissGuideForAppointmentAsync(
            Appointment appointment,
            Payment payment,
            string tenantId)
        {
            try
            {
                if (appointment.PaymentType != PaymentType.HealthInsurance || !appointment.HealthInsurancePlanId.HasValue)
                {
                    return null;
                }

                // Delegate to TISS Guide Service
                var tissGuide = await _tissGuideService.CreateGuideForAppointmentAsync(
                    appointment.Id,
                    appointment.PatientId,
                    appointment.HealthInsurancePlanId.Value,
                    tenantId);

                return tissGuide?.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create TISS guide for appointment {AppointmentId}", appointment.Id);
                return null;
            }
        }
        
        /// <summary>
        /// Calculates the total amount to charge based on consultation and procedures with pricing configuration
        /// </summary>
        public async Task<decimal> CalculateTotalAmountAsync(
            Guid appointmentId,
            List<Guid> procedureIds,
            string tenantId)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(appointmentId, tenantId);
            if (appointment == null)
            {
                throw new ArgumentException("Appointment not found", nameof(appointmentId));
            }

            // Get pricing configuration
            var pricingConfig = await _clinicPricingConfigurationRepository.GetByClinicIdAsync(appointment.ClinicId, tenantId);
            
            decimal totalAmount = 0;

            // Calculate consultation price
            if (pricingConfig != null)
            {
                var consultationPrice = pricingConfig.GetConsultationPrice(appointment.Type, appointment.Mode);
                
                // If procedures are being performed, apply the procedure policy
                if (procedureIds != null && procedureIds.Any())
                {
                    consultationPrice = pricingConfig.GetConsultationPriceWithProcedure(consultationPrice);
                }
                
                totalAmount += consultationPrice;
            }

            // Add procedure prices
            if (procedureIds != null && procedureIds.Any())
            {
                foreach (var procedureId in procedureIds)
                {
                    var procedureConfig = await _procedurePricingConfigurationRepository
                        .GetByProcedureAndClinicAsync(procedureId, appointment.ClinicId, tenantId);
                    
                    // Use configured price or default
                    // This would require loading the Procedure entity to get default price
                    // For now, simplified approach
                    totalAmount += procedureConfig?.CustomPrice ?? 0;
                }
            }

            return totalAmount;
        }
    }
}
