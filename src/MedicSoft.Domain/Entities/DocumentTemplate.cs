using System;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a document template for a specific professional specialty
    /// </summary>
    public class DocumentTemplate : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public ProfessionalSpecialty Specialty { get; private set; }
        public DocumentTemplateType Type { get; private set; }
        public string Content { get; private set; }
        public string Variables { get; private set; } // JSON string with available variables
        public bool IsActive { get; private set; }
        public bool IsSystem { get; private set; } // System templates cannot be deleted
        public Guid? ClinicId { get; private set; } // Null for system templates, set for custom clinic templates
        
        // Navigation properties
        public Clinic? Clinic { get; private set; }
        
        private DocumentTemplate()
        {
            // EF Constructor
            Name = null!;
            Description = null!;
            Content = null!;
            Variables = null!;
        }
        
        public DocumentTemplate(
            string name,
            string description,
            ProfessionalSpecialty specialty,
            DocumentTemplateType type,
            string content,
            string variables,
            string tenantId,
            Guid? clinicId = null,
            bool isSystem = false) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));
            
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));
            
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be empty", nameof(content));
            
            if (string.IsNullOrWhiteSpace(variables))
                throw new ArgumentException("Variables cannot be empty", nameof(variables));
            
            Name = name.Trim();
            Description = description.Trim();
            Specialty = specialty;
            Type = type;
            Content = content.Trim();
            Variables = variables.Trim();
            ClinicId = clinicId;
            IsSystem = isSystem;
            IsActive = true;
        }
        
        public void Update(string name, string description, string content, string variables)
        {
            if (IsSystem)
                throw new InvalidOperationException("Cannot modify system templates");
            
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));
            
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));
            
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be empty", nameof(content));
            
            if (string.IsNullOrWhiteSpace(variables))
                throw new ArgumentException("Variables cannot be empty", nameof(variables));
            
            Name = name.Trim();
            Description = description.Trim();
            Content = content.Trim();
            Variables = variables.Trim();
            UpdateTimestamp();
        }
        
        public void Activate()
        {
            IsActive = true;
            UpdateTimestamp();
        }
        
        public void Deactivate()
        {
            if (IsSystem)
                throw new InvalidOperationException("Cannot deactivate system templates");
            
            IsActive = false;
            UpdateTimestamp();
        }
    }
}
