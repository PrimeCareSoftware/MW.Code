using System;
using System.ComponentModel.DataAnnotations;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.DTOs
{
    public class ProcedureDto
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 200 caracteres")]
        public string Name { get; set; } = null!;
        
        [Required(ErrorMessage = "Código é obrigatório")]
        [StringLength(50, ErrorMessage = "Código deve ter no máximo 50 caracteres")]
        public string Code { get; set; } = null!;
        
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Descrição deve ter entre 10 e 1000 caracteres")]
        public string Description { get; set; } = null!;
        
        [Required(ErrorMessage = "Categoria é obrigatória")]
        public ProcedureCategory Category { get; set; }
        
        [Required(ErrorMessage = "Preço é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Duração é obrigatória")]
        [Range(5, 480, ErrorMessage = "Duração deve estar entre 5 e 480 minutos")]
        public int DurationMinutes { get; set; }
        
        public bool RequiresMaterials { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateProcedureDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 200 caracteres")]
        public string Name { get; set; } = null!;
        
        [Required(ErrorMessage = "Código é obrigatório")]
        [StringLength(50, ErrorMessage = "Código deve ter no máximo 50 caracteres")]
        public string Code { get; set; } = null!;
        
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Descrição deve ter entre 10 e 1000 caracteres")]
        public string Description { get; set; } = null!;
        
        [Required(ErrorMessage = "Categoria é obrigatória")]
        public ProcedureCategory Category { get; set; }
        
        [Required(ErrorMessage = "Preço é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Duração é obrigatória")]
        [Range(5, 480, ErrorMessage = "Duração deve estar entre 5 e 480 minutos")]
        public int DurationMinutes { get; set; }
        
        public bool RequiresMaterials { get; set; }
    }

    public class UpdateProcedureDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 200 caracteres")]
        public string Name { get; set; } = null!;
        
        [Required(ErrorMessage = "Código é obrigatório")]
        [StringLength(50, ErrorMessage = "Código deve ter no máximo 50 caracteres")]
        public string Code { get; set; } = null!;
        
        [Required(ErrorMessage = "Descrição é obrigatória")]
        [StringLength(1000, MinimumLength = 10, ErrorMessage = "Descrição deve ter entre 10 e 1000 caracteres")]
        public string Description { get; set; } = null!;
        
        [Required(ErrorMessage = "Categoria é obrigatória")]
        public ProcedureCategory Category { get; set; }
        
        [Required(ErrorMessage = "Preço é obrigatório")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço deve ser maior que zero")]
        public decimal Price { get; set; }
        
        [Required(ErrorMessage = "Duração é obrigatória")]
        [Range(5, 480, ErrorMessage = "Duração deve estar entre 5 e 480 minutos")]
        public int DurationMinutes { get; set; }
        
        public bool RequiresMaterials { get; set; }
    }

    public class AppointmentProcedureDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid ProcedureId { get; set; }
        public Guid PatientId { get; set; }
        public string ProcedureName { get; set; } = null!;
        public string ProcedureCode { get; set; } = null!;
        public decimal PriceCharged { get; set; }
        public string? Notes { get; set; }
        public DateTime PerformedAt { get; set; }
    }

    public class AddProcedureToAppointmentDto
    {
        [Required(ErrorMessage = "ID do procedimento é obrigatório")]
        public Guid ProcedureId { get; set; }
        
        [Range(0.01, double.MaxValue, ErrorMessage = "Preço customizado deve ser maior que zero")]
        public decimal? CustomPrice { get; set; }
        
        [StringLength(500, ErrorMessage = "Notas devem ter no máximo 500 caracteres")]
        public string? Notes { get; set; }
    }

    public class AppointmentBillingSummaryDto
    {
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = null!;
        public DateTime AppointmentDate { get; set; }
        public List<AppointmentProcedureDto> Procedures { get; set; } = new();
        public decimal SubTotal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal Total { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
    }
}
