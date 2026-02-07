using System;
using System.ComponentModel.DataAnnotations;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Application.DTOs.GlobalDocumentTemplates
{
    /// <summary>
    /// DTO for reading global document template data
    /// </summary>
    public class GlobalDocumentTemplateDto
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public DocumentTemplateType Type { get; set; }
        
        public ProfessionalSpecialty Specialty { get; set; }
        
        public string Content { get; set; } = string.Empty;
        
        /// <summary>
        /// JSON string containing available variables
        /// </summary>
        public string Variables { get; set; } = string.Empty;
        
        public bool IsActive { get; set; }
        
        public string CreatedBy { get; set; } = string.Empty;
        
        public string TenantId { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
    }
    
    /// <summary>
    /// DTO for creating a new global document template
    /// </summary>
    public class CreateGlobalTemplateDto
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [MaxLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "A descrição é obrigatória")]
        [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "O tipo de documento é obrigatório")]
        public DocumentTemplateType Type { get; set; }
        
        [Required(ErrorMessage = "A especialidade é obrigatória")]
        public ProfessionalSpecialty Specialty { get; set; }
        
        [Required(ErrorMessage = "O conteúdo é obrigatório")]
        public string Content { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "As variáveis são obrigatórias")]
        public string Variables { get; set; } = "[]"; // Default empty JSON array
    }
    
    /// <summary>
    /// DTO for updating an existing global document template
    /// </summary>
    public class UpdateGlobalTemplateDto
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [MaxLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "A descrição é obrigatória")]
        [MaxLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres")]
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "O conteúdo é obrigatório")]
        public string Content { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "As variáveis são obrigatórias")]
        public string Variables { get; set; } = "[]";
        
        public bool IsActive { get; set; }
    }
    
    /// <summary>
    /// DTO for filtering global document templates
    /// </summary>
    public class GlobalDocumentTemplateFilterDto
    {
        public DocumentTemplateType? Type { get; set; }
        
        public ProfessionalSpecialty? Specialty { get; set; }
        
        public bool? IsActive { get; set; }
        
        public string? SearchTerm { get; set; }
    }
}
