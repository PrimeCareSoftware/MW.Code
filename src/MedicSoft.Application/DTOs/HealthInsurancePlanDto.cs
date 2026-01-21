using System;
using System.ComponentModel.DataAnnotations;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO for Health Insurance Plan
    /// </summary>
    public class HealthInsurancePlanDto
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "ID da operadora é obrigatório")]
        public Guid OperatorId { get; set; }
        
        [Required(ErrorMessage = "Nome do plano é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome do plano deve ter entre 3 e 200 caracteres")]
        public string PlanName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Código do plano é obrigatório")]
        [StringLength(50, ErrorMessage = "Código do plano deve ter no máximo 50 caracteres")]
        public string PlanCode { get; set; } = string.Empty;
        
        [StringLength(50, ErrorMessage = "Número de registro deve ter no máximo 50 caracteres")]
        public string? RegisterNumber { get; set; }
        
        [Required(ErrorMessage = "Tipo é obrigatório")]
        [StringLength(50, ErrorMessage = "Tipo deve ter no máximo 50 caracteres")]
        public string Type { get; set; } = string.Empty;
        
        public bool IsActive { get; set; }
        public bool CoversConsultations { get; set; }
        public bool CoversExams { get; set; }
        public bool CoversProcedures { get; set; }
        public bool RequiresPriorAuthorization { get; set; }
        public bool TissEnabled { get; set; }
        public string? OperatorName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    /// <summary>
    /// DTO for creating a Health Insurance Plan
    /// </summary>
    public class CreateHealthInsurancePlanDto
    {
        [Required(ErrorMessage = "ID da operadora é obrigatório")]
        public Guid OperatorId { get; set; }
        
        [Required(ErrorMessage = "Nome do plano é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome do plano deve ter entre 3 e 200 caracteres")]
        public string PlanName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Código do plano é obrigatório")]
        [StringLength(50, ErrorMessage = "Código do plano deve ter no máximo 50 caracteres")]
        public string PlanCode { get; set; } = string.Empty;
        
        [StringLength(50, ErrorMessage = "Número de registro deve ter no máximo 50 caracteres")]
        public string? RegisterNumber { get; set; }
        
        [Required(ErrorMessage = "Tipo é obrigatório")]
        [StringLength(50, ErrorMessage = "Tipo deve ter no máximo 50 caracteres")]
        public string Type { get; set; } = "Individual"; // Individual, Enterprise, Collective
        
        public bool CoversConsultations { get; set; } = true;
        public bool CoversExams { get; set; } = true;
        public bool CoversProcedures { get; set; } = true;
        public bool RequiresPriorAuthorization { get; set; } = false;
        public bool TissEnabled { get; set; } = false;
    }

    /// <summary>
    /// DTO for updating a Health Insurance Plan
    /// </summary>
    public class UpdateHealthInsurancePlanDto
    {
        [Required(ErrorMessage = "Nome do plano é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome do plano deve ter entre 3 e 200 caracteres")]
        public string PlanName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Código do plano é obrigatório")]
        [StringLength(50, ErrorMessage = "Código do plano deve ter no máximo 50 caracteres")]
        public string PlanCode { get; set; } = string.Empty;
        
        [StringLength(50, ErrorMessage = "Número de registro deve ter no máximo 50 caracteres")]
        public string? RegisterNumber { get; set; }
        
        [Required(ErrorMessage = "Tipo é obrigatório")]
        [StringLength(50, ErrorMessage = "Tipo deve ter no máximo 50 caracteres")]
        public string Type { get; set; } = "Individual";
        
        public bool CoversConsultations { get; set; }
        public bool CoversExams { get; set; }
        public bool CoversProcedures { get; set; }
        public bool RequiresPriorAuthorization { get; set; }
    }
}
