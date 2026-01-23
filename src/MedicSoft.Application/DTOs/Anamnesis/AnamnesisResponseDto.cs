using System;
using System.Collections.Generic;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Application.DTOs.Anamnesis
{
    public class AnamnesisResponseDto
    {
        public Guid Id { get; set; }
        public Guid AppointmentId { get; set; }
        public Guid PatientId { get; set; }
        public Guid DoctorId { get; set; }
        public Guid TemplateId { get; set; }
        public string TemplateName { get; set; } = string.Empty;
        public DateTime ResponseDate { get; set; }
        public List<QuestionAnswer> Answers { get; set; } = new List<QuestionAnswer>();
        public bool IsComplete { get; set; }
    }

    public class CreateAnamnesisResponseDto
    {
        public Guid AppointmentId { get; set; }
        public Guid TemplateId { get; set; }
    }

    public class SaveAnswersDto
    {
        public List<QuestionAnswer> Answers { get; set; } = new List<QuestionAnswer>();
        public bool IsComplete { get; set; }
    }
}
