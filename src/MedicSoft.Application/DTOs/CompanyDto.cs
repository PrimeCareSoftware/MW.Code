using System;
using System.ComponentModel.DataAnnotations;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Application.DTOs
{
    public class CompanyDto
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 200 caracteres")]
        public string Name { get; set; } = null!;
        
        [Required(ErrorMessage = "Nome fantasia é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome fantasia deve ter entre 3 e 200 caracteres")]
        public string TradeName { get; set; } = null!;
        
        [Required(ErrorMessage = "Documento é obrigatório")]
        public string Document { get; set; } = null!;
        
        [Required(ErrorMessage = "Tipo de documento é obrigatório")]
        public DocumentType DocumentType { get; set; }
        
        [Required(ErrorMessage = "Telefone é obrigatório")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string Phone { get; set; } = null!;
        
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = null!;
        
        public bool IsActive { get; set; }
        
        [StringLength(63, MinimumLength = 3, ErrorMessage = "Subdomínio deve ter entre 3 e 63 caracteres")]
        public string? Subdomain { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateCompanyDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 200 caracteres")]
        public string Name { get; set; } = null!;
        
        [Required(ErrorMessage = "Nome fantasia é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome fantasia deve ter entre 3 e 200 caracteres")]
        public string TradeName { get; set; } = null!;
        
        [Required(ErrorMessage = "Documento é obrigatório")]
        public string Document { get; set; } = null!;
        
        [Required(ErrorMessage = "Tipo de documento é obrigatório")]
        public DocumentType DocumentType { get; set; }
        
        [Required(ErrorMessage = "Telefone é obrigatório")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string Phone { get; set; } = null!;
        
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = null!;
        
        [StringLength(63, MinimumLength = 3, ErrorMessage = "Subdomínio deve ter entre 3 e 63 caracteres")]
        public string? Subdomain { get; set; }
    }

    public class UpdateCompanyDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 200 caracteres")]
        public string Name { get; set; } = null!;
        
        [Required(ErrorMessage = "Nome fantasia é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome fantasia deve ter entre 3 e 200 caracteres")]
        public string TradeName { get; set; } = null!;
        
        [Required(ErrorMessage = "Telefone é obrigatório")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string Phone { get; set; } = null!;
        
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = null!;
        
        [StringLength(63, MinimumLength = 3, ErrorMessage = "Subdomínio deve ter entre 3 e 63 caracteres")]
        public string? Subdomain { get; set; }
    }
}
