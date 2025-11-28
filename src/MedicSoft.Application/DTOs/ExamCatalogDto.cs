using System;
using MedicSoft.Domain.Entities;

namespace MedicSoft.Application.DTOs
{
    public class ExamCatalogDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ExamType ExamType { get; set; }
        public string? Category { get; set; }
        public string? Preparation { get; set; }
        public string? Synonyms { get; set; }
        public string? TussCode { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ExamAutocompleteDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ExamType ExamType { get; set; }
        public string? Category { get; set; }
        public string? Preparation { get; set; }
        public string DisplayText => Name;
    }

    public class CreateExamCatalogDto
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public ExamType ExamType { get; set; }
        public string? Category { get; set; }
        public string? Preparation { get; set; }
        public string? Synonyms { get; set; }
        public string? TussCode { get; set; }
    }

    public class UpdateExamCatalogDto
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public ExamType? ExamType { get; set; }
        public string? Category { get; set; }
        public string? Preparation { get; set; }
        public string? Synonyms { get; set; }
        public string? TussCode { get; set; }
    }
}
