using System;
using System.ComponentModel.DataAnnotations;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO para marcar pagamento de um atendimento
    /// </summary>
    public class MarkAppointmentAsPaidDto
    {
        [Required(ErrorMessage = "Tipo de recebedor é obrigatório")]
        public string PaymentReceiverType { get; set; } = "Secretary"; // Doctor, Secretary, Other
        
        /// <summary>
        /// Valor pago pela consulta (opcional, validado na camada de domínio se fornecido)
        /// </summary>
        public decimal? PaymentAmount { get; set; }
        
        public string? PaymentMethod { get; set; } // Cash, CreditCard, DebitCard, Pix, BankTransfer, Check
    }

    /// <summary>
    /// DTO para finalizar atendimento pelo médico
    /// </summary>
    public class CompleteAppointmentDto
    {
        [StringLength(1000, ErrorMessage = "Notas devem ter no máximo 1000 caracteres")]
        public string? Notes { get; set; }
        
        /// <summary>
        /// Se true, registra que o médico recebeu o pagamento
        /// </summary>
        public bool RegisterPayment { get; set; }
        
        /// <summary>
        /// Valor pago pela consulta (opcional, validado na camada de domínio se fornecido)
        /// </summary>
        public decimal? PaymentAmount { get; set; }
        
        public string? PaymentMethod { get; set; } // Cash, CreditCard, DebitCard, Pix, BankTransfer, Check
    }
}
