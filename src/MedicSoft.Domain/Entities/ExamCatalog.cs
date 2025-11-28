using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a catalog entry for medical exams available for autocomplete.
    /// Used for exam name suggestions during consultation.
    /// </summary>
    public class ExamCatalog : BaseEntity
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public ExamType ExamType { get; private set; }
        public string? Category { get; private set; }
        public string? Preparation { get; private set; } // Preparo para o exame
        public string? Synonyms { get; private set; } // Comma-separated synonyms for better search
        public string? TussCode { get; private set; } // Código TUSS (Terminologia Unificada da Saúde Suplementar)
        public bool IsActive { get; private set; }

        private ExamCatalog()
        {
            // EF Constructor
            Name = null!;
        }

        public ExamCatalog(string name, ExamType examType, string tenantId,
            string? description = null, string? category = null,
            string? preparation = null, string? synonyms = null,
            string? tussCode = null) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            Name = name.Trim();
            ExamType = examType;
            Description = description?.Trim();
            Category = category?.Trim();
            Preparation = preparation?.Trim();
            Synonyms = synonyms?.Trim();
            TussCode = tussCode?.Trim();
            IsActive = true;
        }

        public void Update(string name, ExamType examType,
            string? description = null, string? category = null,
            string? preparation = null, string? synonyms = null,
            string? tussCode = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            Name = name.Trim();
            ExamType = examType;
            Description = description?.Trim();
            Category = category?.Trim();
            Preparation = preparation?.Trim();
            Synonyms = synonyms?.Trim();
            TussCode = tussCode?.Trim();
            UpdateTimestamp();
        }

        public void Activate()
        {
            IsActive = true;
            UpdateTimestamp();
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdateTimestamp();
        }
    }
}
