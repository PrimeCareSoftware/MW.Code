using System;
using System.ComponentModel.DataAnnotations;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO for Health Insurance Operator
    /// </summary>
    public class HealthInsuranceOperatorDto
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Nome fantasia é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome fantasia deve ter entre 3 e 200 caracteres")]
        public string TradeName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Razão social é obrigatória")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Razão social deve ter entre 3 e 200 caracteres")]
        public string CompanyName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Número de registro ANS é obrigatório")]
        [StringLength(50, ErrorMessage = "Número de registro deve ter no máximo 50 caracteres")]
        public string RegisterNumber { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "CNPJ é obrigatório")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "CNPJ deve conter 14 dígitos")]
        public string Document { get; set; } = string.Empty;
        
        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Phone { get; set; }
        
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string? Email { get; set; }
        
        [StringLength(200, ErrorMessage = "Pessoa de contato deve ter no máximo 200 caracteres")]
        public string? ContactPerson { get; set; }
        
        public bool IsActive { get; set; }
        
        [Required(ErrorMessage = "Tipo de integração é obrigatório")]
        [StringLength(50, ErrorMessage = "Tipo de integração deve ter no máximo 50 caracteres")]
        public string IntegrationType { get; set; } = string.Empty;
        
        [Url(ErrorMessage = "URL do website inválida")]
        [StringLength(500, ErrorMessage = "URL do website deve ter no máximo 500 caracteres")]
        public string? WebsiteUrl { get; set; }
        
        [Url(ErrorMessage = "URL da API inválida")]
        [StringLength(500, ErrorMessage = "URL da API deve ter no máximo 500 caracteres")]
        public string? ApiEndpoint { get; set; }
        
        public bool RequiresPriorAuthorization { get; set; }
        
        [StringLength(20, ErrorMessage = "Versão TISS deve ter no máximo 20 caracteres")]
        public string? TissVersion { get; set; }
        
        public bool SupportsTissXml { get; set; }
        
        [EmailAddress(ErrorMessage = "Email de submissão de lote inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string? BatchSubmissionEmail { get; set; }
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for creating a Health Insurance Operator
    /// </summary>
    public class CreateHealthInsuranceOperatorDto
    {
        [Required(ErrorMessage = "Nome fantasia é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome fantasia deve ter entre 3 e 200 caracteres")]
        public string TradeName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Razão social é obrigatória")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Razão social deve ter entre 3 e 200 caracteres")]
        public string CompanyName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Número de registro ANS é obrigatório")]
        [StringLength(50, ErrorMessage = "Número de registro deve ter no máximo 50 caracteres")]
        public string RegisterNumber { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "CNPJ é obrigatório")]
        [RegularExpression(@"^\d{14}$", ErrorMessage = "CNPJ deve conter 14 dígitos")]
        public string Document { get; set; } = string.Empty;
        
        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Phone { get; set; }
        
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string? Email { get; set; }
        
        [StringLength(200, ErrorMessage = "Pessoa de contato deve ter no máximo 200 caracteres")]
        public string? ContactPerson { get; set; }
        
        [Url(ErrorMessage = "URL do website inválida")]
        [StringLength(500, ErrorMessage = "URL do website deve ter no máximo 500 caracteres")]
        public string? WebsiteUrl { get; set; }
        
        public bool RequiresPriorAuthorization { get; set; }
    }

    /// <summary>
    /// DTO for updating a Health Insurance Operator
    /// </summary>
    public class UpdateHealthInsuranceOperatorDto
    {
        [Required(ErrorMessage = "Nome fantasia é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome fantasia deve ter entre 3 e 200 caracteres")]
        public string TradeName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Razão social é obrigatória")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Razão social deve ter entre 3 e 200 caracteres")]
        public string CompanyName { get; set; } = string.Empty;
        
        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string? Phone { get; set; }
        
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string? Email { get; set; }
        
        [StringLength(200, ErrorMessage = "Pessoa de contato deve ter no máximo 200 caracteres")]
        public string? ContactPerson { get; set; }
        
        [Url(ErrorMessage = "URL do website inválida")]
        [StringLength(500, ErrorMessage = "URL do website deve ter no máximo 500 caracteres")]
        public string? WebsiteUrl { get; set; }
        
        public bool RequiresPriorAuthorization { get; set; }
    }

    /// <summary>
    /// DTO for configuring operator integration
    /// </summary>
    public class ConfigureOperatorIntegrationDto
    {
        [Required(ErrorMessage = "Tipo de integração é obrigatório")]
        [StringLength(50, ErrorMessage = "Tipo de integração deve ter no máximo 50 caracteres")]
        public string IntegrationType { get; set; } = string.Empty; // Manual, WebPortal, TissXml, RestApi
        
        [Url(ErrorMessage = "URL da API inválida")]
        [StringLength(500, ErrorMessage = "URL da API deve ter no máximo 500 caracteres")]
        public string? ApiEndpoint { get; set; }
        
        [StringLength(200, ErrorMessage = "Chave da API deve ter no máximo 200 caracteres")]
        public string? ApiKey { get; set; }
        
        [Url(ErrorMessage = "URL do website inválida")]
        [StringLength(500, ErrorMessage = "URL do website deve ter no máximo 500 caracteres")]
        public string? WebsiteUrl { get; set; }
    }

    /// <summary>
    /// DTO for configuring TISS settings
    /// </summary>
    public class ConfigureOperatorTissDto
    {
        [Required(ErrorMessage = "Versão TISS é obrigatória")]
        [StringLength(20, ErrorMessage = "Versão TISS deve ter no máximo 20 caracteres")]
        public string TissVersion { get; set; } = string.Empty;
        
        public bool SupportsTissXml { get; set; }
        
        [EmailAddress(ErrorMessage = "Email de submissão de lote inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string? BatchSubmissionEmail { get; set; }
    }
}
