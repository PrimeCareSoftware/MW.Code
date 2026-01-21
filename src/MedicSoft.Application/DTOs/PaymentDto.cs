using System;
using System.ComponentModel.DataAnnotations;

namespace MedicSoft.Application.DTOs
{
    public class PaymentDto
    {
        public Guid Id { get; set; }
        public Guid? AppointmentId { get; set; }
        public Guid? ClinicSubscriptionId { get; set; }
        
        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
        public decimal Amount { get; set; }
        
        [Required(ErrorMessage = "Método de pagamento é obrigatório")]
        [StringLength(50, ErrorMessage = "Método de pagamento deve ter no máximo 50 caracteres")]
        public string Method { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Status é obrigatório")]
        [StringLength(50, ErrorMessage = "Status deve ter no máximo 50 caracteres")]
        public string Status { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Data de pagamento é obrigatória")]
        public DateTime PaymentDate { get; set; }
        
        public DateTime? ProcessedDate { get; set; }
        
        [StringLength(100, ErrorMessage = "ID da transação deve ter no máximo 100 caracteres")]
        public string? TransactionId { get; set; }
        
        [StringLength(1000, ErrorMessage = "Notas devem ter no máximo 1000 caracteres")]
        public string? Notes { get; set; }
        
        [StringLength(500, ErrorMessage = "Motivo de cancelamento deve ter no máximo 500 caracteres")]
        public string? CancellationReason { get; set; }
        
        public DateTime? CancellationDate { get; set; }
        
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Últimos 4 dígitos do cartão devem ter 4 caracteres")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Últimos 4 dígitos do cartão devem ser numéricos")]
        public string? CardLastFourDigits { get; set; }
        
        [StringLength(100, ErrorMessage = "Chave PIX deve ter no máximo 100 caracteres")]
        public string? PixKey { get; set; }
        
        [StringLength(100, ErrorMessage = "ID da transação PIX deve ter no máximo 100 caracteres")]
        public string? PixTransactionId { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreatePaymentDto
    {
        public Guid? AppointmentId { get; set; }
        public Guid? ClinicSubscriptionId { get; set; }
        
        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
        public decimal Amount { get; set; }
        
        [Required(ErrorMessage = "Método de pagamento é obrigatório")]
        [StringLength(50, ErrorMessage = "Método de pagamento deve ter no máximo 50 caracteres")]
        public string Method { get; set; } = "Cash";
        
        [StringLength(1000, ErrorMessage = "Notas devem ter no máximo 1000 caracteres")]
        public string? Notes { get; set; }
        
        [StringLength(4, MinimumLength = 4, ErrorMessage = "Últimos 4 dígitos do cartão devem ter 4 caracteres")]
        [RegularExpression(@"^\d{4}$", ErrorMessage = "Últimos 4 dígitos do cartão devem ser numéricos")]
        public string? CardLastFourDigits { get; set; }
        
        [StringLength(100, ErrorMessage = "Chave PIX deve ter no máximo 100 caracteres")]
        public string? PixKey { get; set; }
    }

    public class ProcessPaymentDto
    {
        [Required(ErrorMessage = "ID do pagamento é obrigatório")]
        public Guid PaymentId { get; set; }
        
        [Required(ErrorMessage = "ID da transação é obrigatório")]
        [StringLength(100, ErrorMessage = "ID da transação deve ter no máximo 100 caracteres")]
        public string TransactionId { get; set; } = string.Empty;
    }

    public class RefundPaymentDto
    {
        [Required(ErrorMessage = "ID do pagamento é obrigatório")]
        public Guid PaymentId { get; set; }
        
        [Required(ErrorMessage = "Motivo é obrigatório")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Motivo deve ter entre 10 e 500 caracteres")]
        public string Reason { get; set; } = string.Empty;
    }

    public class CancelPaymentDto
    {
        [Required(ErrorMessage = "ID do pagamento é obrigatório")]
        public Guid PaymentId { get; set; }
        
        [Required(ErrorMessage = "Motivo é obrigatório")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Motivo deve ter entre 10 e 500 caracteres")]
        public string Reason { get; set; } = string.Empty;
    }
}
