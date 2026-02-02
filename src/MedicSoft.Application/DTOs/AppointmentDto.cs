using System;
using System.ComponentModel.DataAnnotations;

namespace MedicSoft.Application.DTOs
{
    public class AppointmentDto
    {
        public Guid Id { get; set; }
        
        [Required(ErrorMessage = "ID do paciente é obrigatório")]
        public Guid PatientId { get; set; }
        
        public string PatientName { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "ID da clínica é obrigatório")]
        public Guid ClinicId { get; set; }
        
        public string ClinicName { get; set; } = string.Empty;

        public Guid? ProfessionalId { get; set; }

        public string? ProfessionalName { get; set; }
        
        public string? ProfessionalSpecialty { get; set; }
        
        [Required(ErrorMessage = "Data agendada é obrigatória")]
        public DateTime ScheduledDate { get; set; }
        
        [Required(ErrorMessage = "Horário agendado é obrigatório")]
        public TimeSpan ScheduledTime { get; set; }
        
        [Required(ErrorMessage = "Duração é obrigatória")]
        [Range(5, 480, ErrorMessage = "Duração deve estar entre 5 e 480 minutos")]
        public int DurationMinutes { get; set; }
        
        [Required(ErrorMessage = "Tipo é obrigatório")]
        [StringLength(50, ErrorMessage = "Tipo deve ter no máximo 50 caracteres")]
        public string Type { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Status é obrigatório")]
        [StringLength(50, ErrorMessage = "Status deve ter no máximo 50 caracteres")]
        public string Status { get; set; } = string.Empty;
        
        [StringLength(1000, ErrorMessage = "Notas devem ter no máximo 1000 caracteres")]
        public string? Notes { get; set; }
        
        [StringLength(500, ErrorMessage = "Motivo de cancelamento deve ter no máximo 500 caracteres")]
        public string? CancellationReason { get; set; }
        
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        
        // Room number
        [StringLength(50, ErrorMessage = "Número da sala deve ter no máximo 50 caracteres")]
        public string? RoomNumber { get; set; }
        
        // Payment tracking
        public bool IsPaid { get; set; }
        public DateTime? PaidAt { get; set; }
        public Guid? PaidByUserId { get; set; }
        public string? PaidByUserName { get; set; }
        public string? PaymentReceivedBy { get; set; } // Doctor, Secretary, Other
        public decimal? PaymentAmount { get; set; }
        public string? PaymentMethod { get; set; } // Cash, CreditCard, DebitCard, Pix, BankTransfer, Check
        
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateAppointmentDto
    {
        [Required(ErrorMessage = "ID do paciente é obrigatório")]
        public Guid PatientId { get; set; }
        
        [Required(ErrorMessage = "ID da clínica é obrigatório")]
        public Guid ClinicId { get; set; }

        public Guid? ProfessionalId { get; set; }
        
        [Required(ErrorMessage = "Data agendada é obrigatória")]
        public DateTime ScheduledDate { get; set; }
        
        [Required(ErrorMessage = "Horário agendado é obrigatório")]
        public TimeSpan ScheduledTime { get; set; }
        
        [Required(ErrorMessage = "Duração é obrigatória")]
        [Range(5, 480, ErrorMessage = "Duração deve estar entre 5 e 480 minutos")]
        public int DurationMinutes { get; set; }
        
        [Required(ErrorMessage = "Tipo é obrigatório")]
        [StringLength(50, ErrorMessage = "Tipo deve ter no máximo 50 caracteres")]
        public string Type { get; set; } = "Regular";
        
        [StringLength(50, ErrorMessage = "Número da sala deve ter no máximo 50 caracteres")]
        public string? RoomNumber { get; set; }
        
        [StringLength(1000, ErrorMessage = "Notas devem ter no máximo 1000 caracteres")]
        public string? Notes { get; set; }
    }

    public class UpdateAppointmentDto
    {
        public Guid? ProfessionalId { get; set; }

        [Required(ErrorMessage = "Data agendada é obrigatória")]
        public DateTime ScheduledDate { get; set; }
        
        [Required(ErrorMessage = "Horário agendado é obrigatório")]
        public TimeSpan ScheduledTime { get; set; }
        
        [Required(ErrorMessage = "Duração é obrigatória")]
        [Range(5, 480, ErrorMessage = "Duração deve estar entre 5 e 480 minutos")]
        public int DurationMinutes { get; set; }
        
        [Required(ErrorMessage = "Tipo é obrigatório")]
        [StringLength(50, ErrorMessage = "Tipo deve ter no máximo 50 caracteres")]
        public string Type { get; set; } = "Regular";
        
        [StringLength(50, ErrorMessage = "Número da sala deve ter no máximo 50 caracteres")]
        public string? RoomNumber { get; set; }
        
        [StringLength(1000, ErrorMessage = "Notas devem ter no máximo 1000 caracteres")]
        public string? Notes { get; set; }
    }

    public class DailyAgendaDto
    {
        public DateTime Date { get; set; }
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public List<AppointmentDto> Appointments { get; set; } = new();
        public List<TimeSpan> AvailableSlots { get; set; } = new();
    }

    public class AvailableSlotDto
    {
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public int DurationMinutes { get; set; }
        public bool IsAvailable { get; set; }
    }
}