using System;
using System.ComponentModel.DataAnnotations;
using MedicSoft.Application.Validation;

namespace MedicSoft.Application.DTOs
{
    public class SupplierDto
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 200 caracteres")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessage = "Nome fantasia deve ter no máximo 200 caracteres")]
        public string? TradeName { get; set; }
        
        [StringLength(20, ErrorMessage = "CPF/CNPJ deve ter no máximo 20 caracteres")]
        public string? DocumentNumber { get; set; }
        
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string? Email { get; set; }
        
        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Phone { get; set; }
        
        [StringLength(300, ErrorMessage = "Endereço deve ter no máximo 300 caracteres")]
        public string? Address { get; set; }
        
        [StringLength(100, ErrorMessage = "Cidade deve ter no máximo 100 caracteres")]
        public string? City { get; set; }
        
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Estado deve ter 2 caracteres (sigla)")]
        public string? State { get; set; }
        
        [Cep]
        public string? ZipCode { get; set; }
        
        [StringLength(100, ErrorMessage = "Nome do banco deve ter no máximo 100 caracteres")]
        public string? BankName { get; set; }
        
        [StringLength(50, ErrorMessage = "Conta bancária deve ter no máximo 50 caracteres")]
        public string? BankAccount { get; set; }
        
        [StringLength(100, ErrorMessage = "Chave PIX deve ter no máximo 100 caracteres")]
        public string? PixKey { get; set; }
        
        [StringLength(1000, ErrorMessage = "Observações devem ter no máximo 1000 caracteres")]
        public string? Notes { get; set; }
        
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateSupplierDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 200 caracteres")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessage = "Nome fantasia deve ter no máximo 200 caracteres")]
        public string? TradeName { get; set; }
        
        [StringLength(20, ErrorMessage = "CPF/CNPJ deve ter no máximo 20 caracteres")]
        public string? DocumentNumber { get; set; }
        
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string? Email { get; set; }
        
        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Phone { get; set; }
        
        [StringLength(300, ErrorMessage = "Endereço deve ter no máximo 300 caracteres")]
        public string? Address { get; set; }
        
        [StringLength(100, ErrorMessage = "Cidade deve ter no máximo 100 caracteres")]
        public string? City { get; set; }
        
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Estado deve ter 2 caracteres (sigla)")]
        public string? State { get; set; }
        
        [Cep]
        public string? ZipCode { get; set; }
        
        [StringLength(100, ErrorMessage = "Nome do banco deve ter no máximo 100 caracteres")]
        public string? BankName { get; set; }
        
        [StringLength(50, ErrorMessage = "Conta bancária deve ter no máximo 50 caracteres")]
        public string? BankAccount { get; set; }
        
        [StringLength(100, ErrorMessage = "Chave PIX deve ter no máximo 100 caracteres")]
        public string? PixKey { get; set; }
        
        [StringLength(1000, ErrorMessage = "Observações devem ter no máximo 1000 caracteres")]
        public string? Notes { get; set; }
    }

    public class UpdateSupplierDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 200 caracteres")]
        public string Name { get; set; } = string.Empty;
        
        [StringLength(200, ErrorMessage = "Nome fantasia deve ter no máximo 200 caracteres")]
        public string? TradeName { get; set; }
        
        [StringLength(20, ErrorMessage = "CPF/CNPJ deve ter no máximo 20 caracteres")]
        public string? DocumentNumber { get; set; }
        
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string? Email { get; set; }
        
        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Phone { get; set; }
        
        [StringLength(300, ErrorMessage = "Endereço deve ter no máximo 300 caracteres")]
        public string? Address { get; set; }
        
        [StringLength(100, ErrorMessage = "Cidade deve ter no máximo 100 caracteres")]
        public string? City { get; set; }
        
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Estado deve ter 2 caracteres (sigla)")]
        public string? State { get; set; }
        
        [Cep]
        public string? ZipCode { get; set; }
        
        [StringLength(100, ErrorMessage = "Nome do banco deve ter no máximo 100 caracteres")]
        public string? BankName { get; set; }
        
        [StringLength(50, ErrorMessage = "Conta bancária deve ter no máximo 50 caracteres")]
        public string? BankAccount { get; set; }
        
        [StringLength(100, ErrorMessage = "Chave PIX deve ter no máximo 100 caracteres")]
        public string? PixKey { get; set; }
        
        [StringLength(1000, ErrorMessage = "Observações devem ter no máximo 1000 caracteres")]
        public string? Notes { get; set; }
    }
}
