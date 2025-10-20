using System;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.DTOs
{
    public class ExamRequestDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public string PatientName { get; set; } = string.Empty;
        public ExamType ExamType { get; set; }
        public string ExamName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ExamUrgency Urgency { get; set; }
        public ExamRequestStatus Status { get; set; }
        public DateTime RequestedDate { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CompletedDate { get; set; }
        public string? Results { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class CreateExamRequestDto
    {
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public ExamType ExamType { get; set; }
        public string ExamName { get; set; } = null!;
        public string Description { get; set; } = null!;
        public ExamUrgency Urgency { get; set; }
        public string? Notes { get; set; }
    }

    public class UpdateExamRequestDto
    {
        public string? ExamName { get; set; }
        public string? Description { get; set; }
        public ExamUrgency? Urgency { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public string? Notes { get; set; }
    }

    public class CompleteExamRequestDto
    {
        public string Results { get; set; } = null!;
    }
}
