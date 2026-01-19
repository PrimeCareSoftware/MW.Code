using System;

namespace MedicSoft.Application.DTOs
{
    /// <summary>
    /// DTO público para listagem de clínicas no site.
    /// Contém apenas informações essenciais e públicas, respeitando a LGPD.
    /// NÃO inclui dados sensíveis como CNPJ completo ou informações internas.
    /// </summary>
    public class PublicClinicDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string TradeName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public TimeSpan OpeningTime { get; set; }
        public TimeSpan ClosingTime { get; set; }
        public int AppointmentDurationMinutes { get; set; }
        public bool IsAcceptingNewPatients { get; set; }
    }

    /// <summary>
    /// Filtros para busca de clínicas públicas
    /// </summary>
    public class SearchClinicsRequestDto
    {
        public string? Name { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// Resultado paginado da busca de clínicas
    /// </summary>
    public class SearchClinicsResultDto
    {
        public List<PublicClinicDto> Clinics { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
    }

    /// <summary>
    /// DTO para agendamento público (sem autenticação).
    /// Requer dados básicos do paciente para criar o agendamento.
    /// </summary>
    public class PublicAppointmentRequestDto
    {
        // Dados da consulta
        public Guid ClinicId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public TimeSpan ScheduledTime { get; set; }
        public int DurationMinutes { get; set; } = 30;
        
        // Dados do paciente (necessários para criar o agendamento)
        public string PatientName { get; set; } = string.Empty;
        public string PatientCpf { get; set; } = string.Empty;
        public DateTime PatientBirthDate { get; set; }
        public string PatientPhone { get; set; } = string.Empty;
        public string PatientEmail { get; set; } = string.Empty;
        
        // Observações opcionais
        public string? Notes { get; set; }
    }

    /// <summary>
    /// Resposta do agendamento público
    /// </summary>
    public class PublicAppointmentResponseDto
    {
        public Guid AppointmentId { get; set; }
        public Guid ClinicId { get; set; }
        public string ClinicName { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public TimeSpan ScheduledTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = "Agendamento realizado com sucesso! Você receberá uma confirmação por e-mail.";
    }

    /// <summary>
    /// DTO para consultar horários disponíveis (público)
    /// </summary>
    public class PublicAvailableSlotsRequestDto
    {
        public Guid ClinicId { get; set; }
        public DateTime Date { get; set; }
        public int DurationMinutes { get; set; } = 30;
    }
}
