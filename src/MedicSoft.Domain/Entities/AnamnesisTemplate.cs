using System;
using System.Collections.Generic;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;
using MedicSoft.Domain.ValueObjects;
using System.Text.Json;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents an anamnesis template for a specific medical specialty
    /// </summary>
    public class AnamnesisTemplate : BaseEntity
    {
        public string Name { get; private set; } = string.Empty;
        public MedicalSpecialty Specialty { get; private set; }
        public string? Description { get; private set; }
        public bool IsActive { get; private set; } = true;
        public bool IsDefault { get; private set; }
        public string SectionsJson { get; private set; } = string.Empty;  // JSON serialized sections
        public Guid CreatedBy { get; private set; }
        
        // Navigation property
        private List<QuestionSection>? _sections;
        
        /// <summary>
        /// Gets the sections of questions for this template
        /// </summary>
        public List<QuestionSection> Sections
        {
            get
            {
                if (_sections == null && !string.IsNullOrEmpty(SectionsJson))
                {
                    _sections = JsonSerializer.Deserialize<List<QuestionSection>>(SectionsJson) ?? new List<QuestionSection>();
                }
                return _sections ?? new List<QuestionSection>();
            }
            private set
            {
                _sections = value;
                SectionsJson = JsonSerializer.Serialize(value);
            }
        }

        private AnamnesisTemplate() 
        { 
            // EF Core constructor
        }

        public AnamnesisTemplate(
            string name, 
            MedicalSpecialty specialty, 
            List<QuestionSection> sections,
            string tenantId,
            Guid createdBy,
            string? description = null,
            bool isDefault = false) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("O nome do template não pode estar vazio", nameof(name));
            
            if (sections == null || sections.Count == 0)
                throw new ArgumentException("O template deve ter pelo menos uma seção", nameof(sections));
            
            if (createdBy == Guid.Empty)
                throw new ArgumentException("O criador do template deve ser informado", nameof(createdBy));

            Name = name;
            Specialty = specialty;
            Description = description;
            IsDefault = isDefault;
            CreatedBy = createdBy;
            Sections = sections;
        }

        public void Update(string name, string? description, List<QuestionSection> sections)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("O nome do template não pode estar vazio", nameof(name));
            
            if (sections == null || sections.Count == 0)
                throw new ArgumentException("O template deve ter pelo menos uma seção", nameof(sections));

            Name = name;
            Description = description;
            Sections = sections;
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

        public void SetAsDefault()
        {
            IsDefault = true;
            UpdateTimestamp();
        }

        public void RemoveAsDefault()
        {
            IsDefault = false;
            UpdateTimestamp();
        }
    }
}
