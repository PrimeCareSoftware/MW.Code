using System;
using System.ComponentModel.DataAnnotations;

namespace MedicSoft.Application.DTOs
{
    public class ExpenseDto
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "ID da clínica é obrigatório")]
        public Guid ClinicId { get; set; }
        
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(500, MinimumLength = 3, ErrorMessage = "Descrição deve ter entre 3 e 500 caracteres")]
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Categoria é obrigatória")]
        [StringLength(100, ErrorMessage = "Categoria deve ter no máximo 100 caracteres")]
        public string Category { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
        public decimal Amount { get; set; }
        
        [Required(ErrorMessage = "Data de vencimento é obrigatória")]
        public DateTime DueDate { get; set; }
        
        public DateTime? PaidDate { get; set; }
        
        [Required(ErrorMessage = "Status é obrigatório")]
        [StringLength(50, ErrorMessage = "Status deve ter no máximo 50 caracteres")]
        public string Status { get; set; } = string.Empty;
        
        [StringLength(50, ErrorMessage = "Método de pagamento deve ter no máximo 50 caracteres")]
        public string? PaymentMethod { get; set; }
        
        [StringLength(100, ErrorMessage = "Referência de pagamento deve ter no máximo 100 caracteres")]
        public string? PaymentReference { get; set; }
        
        [StringLength(200, ErrorMessage = "Nome do fornecedor deve ter no máximo 200 caracteres")]
        public string? SupplierName { get; set; }
        
        [StringLength(20, ErrorMessage = "CPF/CNPJ do fornecedor deve ter no máximo 20 caracteres")]
        public string? SupplierDocument { get; set; }
        
        [StringLength(1000, ErrorMessage = "Observações devem ter no máximo 1000 caracteres")]
        public string? Notes { get; set; }
        
        [StringLength(500, ErrorMessage = "Motivo de cancelamento deve ter no máximo 500 caracteres")]
        public string? CancellationReason { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? DaysOverdue { get; set; }
    }

    public class CreateExpenseDto
    {
        [Required(ErrorMessage = "ID da clínica é obrigatório")]
        public Guid ClinicId { get; set; }
        
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(500, MinimumLength = 3, ErrorMessage = "Descrição deve ter entre 3 e 500 caracteres")]
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Categoria é obrigatória")]
        [StringLength(100, ErrorMessage = "Categoria deve ter no máximo 100 caracteres")]
        public string Category { get; set; } = "Other";
        
        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
        public decimal Amount { get; set; }
        
        [Required(ErrorMessage = "Data de vencimento é obrigatória")]
        public DateTime DueDate { get; set; }
        
        [StringLength(200, ErrorMessage = "Nome do fornecedor deve ter no máximo 200 caracteres")]
        public string? SupplierName { get; set; }
        
        [StringLength(20, ErrorMessage = "CPF/CNPJ do fornecedor deve ter no máximo 20 caracteres")]
        public string? SupplierDocument { get; set; }
        
        [StringLength(1000, ErrorMessage = "Observações devem ter no máximo 1000 caracteres")]
        public string? Notes { get; set; }
    }

    public class UpdateExpenseDto
    {
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(500, MinimumLength = 3, ErrorMessage = "Descrição deve ter entre 3 e 500 caracteres")]
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Categoria é obrigatória")]
        [StringLength(100, ErrorMessage = "Categoria deve ter no máximo 100 caracteres")]
        public string Category { get; set; } = "Other";
        
        [Required(ErrorMessage = "Valor é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Valor deve ser maior que zero")]
        public decimal Amount { get; set; }
        
        [Required(ErrorMessage = "Data de vencimento é obrigatória")]
        public DateTime DueDate { get; set; }
        
        [StringLength(200, ErrorMessage = "Nome do fornecedor deve ter no máximo 200 caracteres")]
        public string? SupplierName { get; set; }
        
        [StringLength(20, ErrorMessage = "CPF/CNPJ do fornecedor deve ter no máximo 20 caracteres")]
        public string? SupplierDocument { get; set; }
        
        [StringLength(1000, ErrorMessage = "Observações devem ter no máximo 1000 caracteres")]
        public string? Notes { get; set; }
    }

    public class PayExpenseDto
    {
        [Required(ErrorMessage = "Método de pagamento é obrigatório")]
        [StringLength(50, ErrorMessage = "Método de pagamento deve ter no máximo 50 caracteres")]
        public string PaymentMethod { get; set; } = "Cash";
        
        [StringLength(100, ErrorMessage = "Referência de pagamento deve ter no máximo 100 caracteres")]
        public string? PaymentReference { get; set; }
    }

    public class CancelExpenseDto
    {
        [Required(ErrorMessage = "Motivo de cancelamento é obrigatório")]
        [StringLength(500, MinimumLength = 10, ErrorMessage = "Motivo deve ter entre 10 e 500 caracteres")]
        public string Reason { get; set; } = string.Empty;
    }
}
