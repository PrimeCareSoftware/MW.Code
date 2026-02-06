using System;
using System.ComponentModel.DataAnnotations;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Application.DTOs.DocumentTemplates
{
    /// <summary>
    /// DTO for reading document template data
    /// </summary>
    public class DocumentTemplateDto
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = string.Empty;
        
        public string Description { get; set; } = string.Empty;
        
        public ProfessionalSpecialty Specialty { get; set; }
        
        public DocumentTemplateType Type { get; set; }
        
        public string Content { get; set; } = string.Empty;
        
        /// <summary>
        /// JSON string containing available variables
        /// Example: [{"key":"patientName","label":"Nome do Paciente","type":"text"},...]
        /// </summary>
        public string Variables { get; set; } = string.Empty;
        
        public bool IsActive { get; set; }
        
        public bool IsSystem { get; set; }
        
        public Guid? ClinicId { get; set; }
        
        public string TenantId { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
    }
    
    /// <summary>
    /// DTO for creating a new document template
    /// </summary>
    public class CreateDocumentTemplateDto
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [MaxLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres")]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres")]
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "A especialidade é obrigatória")]
        public ProfessionalSpecialty Specialty { get; set; }
        
        [Required(ErrorMessage = "O tipo de documento é obrigatório")]
        public DocumentTemplateType Type { get; set; }
        
        [Required(ErrorMessage = "O conteúdo é obrigatório")]
        public string Content { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "As variáveis são obrigatórias")]
        public string Variables { get; set; } = "[]"; // Default empty JSON array
        
        public Guid? ClinicId { get; set; }
    }
    
    /// <summary>
    /// DTO for updating an existing document template
    /// </summary>
    public class UpdateDocumentTemplateDto
    {
        [Required(ErrorMessage = "O nome é obrigatório")]
        [MaxLength(200, ErrorMessage = "O nome deve ter no máximo 200 caracteres")]
        public string Name { get; set; } = string.Empty;
        
        [MaxLength(1000, ErrorMessage = "A descrição deve ter no máximo 1000 caracteres")]
        public string Description { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "O conteúdo é obrigatório")]
        public string Content { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "As variáveis são obrigatórias")]
        public string Variables { get; set; } = "[]";
    }
    
    /// <summary>
    /// DTO for listing document templates with filters
    /// </summary>
    public class DocumentTemplateFilterDto
    {
        public ProfessionalSpecialty? Specialty { get; set; }
        
        public DocumentTemplateType? Type { get; set; }
        
        public bool? IsActive { get; set; }
        
        public bool? IsSystem { get; set; }
        
        public Guid? ClinicId { get; set; }
    }
    
    /// <summary>
    /// DTO for template variable definition
    /// Used in the Variables JSON field
    /// </summary>
    public class TemplateVariableDto
    {
        [Required]
        public string Key { get; set; } = string.Empty;
        
        [Required]
        public string Label { get; set; } = string.Empty;
        
        [Required]
        public string Type { get; set; } = "text"; // text, date, number, boolean
        
        public string? Description { get; set; }
        
        public string? DefaultValue { get; set; }
        
        public bool IsRequired { get; set; }
        
        public int DisplayOrder { get; set; }
    }
}
