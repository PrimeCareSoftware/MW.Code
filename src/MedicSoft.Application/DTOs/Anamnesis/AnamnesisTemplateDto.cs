using System;
using System.Collections.Generic;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.ValueObjects;

namespace MedicSoft.Application.DTOs.Anamnesis
{
    public class AnamnesisTemplateDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public MedicalSpecialty Specialty { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsDefault { get; set; }
        public List<QuestionSection> Sections { get; set; } = new List<QuestionSection>();
        public DateTime CreatedAt { get; set; }
        public Guid CreatedBy { get; set; }
    }

    public class CreateAnamnesisTemplateDto
    {
        public string Name { get; set; } = string.Empty;
        public MedicalSpecialty Specialty { get; set; }
        public string? Description { get; set; }
        public bool IsDefault { get; set; }
        public List<QuestionSection> Sections { get; set; } = new List<QuestionSection>();
    }

    public class UpdateAnamnesisTemplateDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<QuestionSection> Sections { get; set; } = new List<QuestionSection>();
    }
}
