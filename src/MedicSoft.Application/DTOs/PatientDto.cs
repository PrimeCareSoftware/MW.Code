using System;
using System.ComponentModel.DataAnnotations;

namespace MedicSoft.Application.DTOs
{
    public class PatientDto
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 200 caracteres")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "CPF é obrigatório")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter 11 dígitos")]
        public string Document { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Data de nascimento é obrigatória")]
        public DateTime DateOfBirth { get; set; }
        
        [Required(ErrorMessage = "Gênero é obrigatório")]
        [StringLength(50, ErrorMessage = "Gênero deve ter no máximo 50 caracteres")]
        public string Gender { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Telefone é obrigatório")]
        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string Phone { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Endereço é obrigatório")]
        public AddressDto Address { get; set; } = new();
        
        [StringLength(5000, ErrorMessage = "Histórico médico deve ter no máximo 5000 caracteres")]
        public string? MedicalHistory { get; set; }
        
        [StringLength(2000, ErrorMessage = "Alergias devem ter no máximo 2000 caracteres")]
        public string? Allergies { get; set; }
        
        public bool IsActive { get; set; } = true;
        public int Age { get; set; }
        public bool IsChild { get; set; }
        public Guid? GuardianId { get; set; }
        public string? GuardianName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class AddressDto
    {
        [Required(ErrorMessage = "Rua é obrigatória")]
        [StringLength(200, ErrorMessage = "Rua deve ter no máximo 200 caracteres")]
        public string Street { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Número é obrigatório")]
        [StringLength(10, ErrorMessage = "Número deve ter no máximo 10 caracteres")]
        public string Number { get; set; } = string.Empty;
        
        [StringLength(100, ErrorMessage = "Complemento deve ter no máximo 100 caracteres")]
        public string? Complement { get; set; }
        
        [Required(ErrorMessage = "Bairro é obrigatório")]
        [StringLength(100, ErrorMessage = "Bairro deve ter no máximo 100 caracteres")]
        public string Neighborhood { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Cidade é obrigatória")]
        [StringLength(100, ErrorMessage = "Cidade deve ter no máximo 100 caracteres")]
        public string City { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Estado é obrigatório")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "Estado deve ter 2 caracteres (sigla)")]
        public string State { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "CEP é obrigatório")]
        [RegularExpression(@"^\d{8}$", ErrorMessage = "CEP deve conter 8 dígitos")]
        public string ZipCode { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "País é obrigatório")]
        [StringLength(50, ErrorMessage = "País deve ter no máximo 50 caracteres")]
        public string Country { get; set; } = string.Empty;
    }

    public class CreatePatientDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 200 caracteres")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "CPF é obrigatório")]
        [RegularExpression(@"^\d{11}$", ErrorMessage = "CPF deve conter 11 dígitos")]
        public string Document { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Data de nascimento é obrigatória")]
        public DateTime DateOfBirth { get; set; }
        
        [Required(ErrorMessage = "Gênero é obrigatório")]
        [StringLength(50, ErrorMessage = "Gênero deve ter no máximo 50 caracteres")]
        public string Gender { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Código do país é obrigatório")]
        [RegularExpression(@"^\+?\d{1,4}$", ErrorMessage = "Código do país deve ter entre 1 e 4 dígitos, opcionalmente com +")]
        public string PhoneCountryCode { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Número de telefone é obrigatório")]
        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Endereço é obrigatório")]
        public AddressDto Address { get; set; } = new();
        
        [StringLength(5000, ErrorMessage = "Histórico médico deve ter no máximo 5000 caracteres")]
        public string? MedicalHistory { get; set; }
        
        [StringLength(2000, ErrorMessage = "Alergias devem ter no máximo 2000 caracteres")]
        public string? Allergies { get; set; }
        
        public Guid? GuardianId { get; set; }
    }

    public class UpdatePatientDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Nome deve ter entre 3 e 200 caracteres")]
        public string Name { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email inválido")]
        [StringLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string Email { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Código do país é obrigatório")]
        [RegularExpression(@"^\+?\d{1,4}$", ErrorMessage = "Código do país deve ter entre 1 e 4 dígitos, opcionalmente com +")]
        public string PhoneCountryCode { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Número de telefone é obrigatório")]
        [Phone(ErrorMessage = "Telefone inválido")]
        [StringLength(20, ErrorMessage = "Telefone deve ter no máximo 20 caracteres")]
        public string PhoneNumber { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Endereço é obrigatório")]
        public AddressDto Address { get; set; } = new();
        
        [StringLength(5000, ErrorMessage = "Histórico médico deve ter no máximo 5000 caracteres")]
        public string? MedicalHistory { get; set; }
        
        [StringLength(2000, ErrorMessage = "Alergias devem ter no máximo 2000 caracteres")]
        public string? Allergies { get; set; }
    }
}