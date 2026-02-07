using System;
using MedicSoft.Domain.Common;
using MedicSoft.Domain.Enums;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a global document template available for all clinics in the system
    /// System admins can create and manage these templates
    /// </summary>
    public class GlobalDocumentTemplate : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public DocumentTemplateType Type { get; private set; }
        public ProfessionalSpecialty Specialty { get; private set; }
        public string Content { get; private set; } // HTML rico
        public string Variables { get; private set; } // JSON com variáveis disponíveis
        public bool IsActive { get; private set; }
        public string CreatedBy { get; private set; }
        
        private GlobalDocumentTemplate()
        {
            // EF Constructor
            Name = null!;
            Description = null!;
            Content = null!;
            Variables = null!;
            CreatedBy = null!;
        }
        
        public GlobalDocumentTemplate(
            string name,
            string description,
            DocumentTemplateType type,
            ProfessionalSpecialty specialty,
            string content,
            string variables,
            string tenantId,
            string createdBy) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));
            
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Description cannot be empty", nameof(description));
            
            if (string.IsNullOrWhiteSpace(content))
                throw new ArgumentException("Content cannot be empty", nameof(content));
            
            if (string.IsNullOrWhiteSpace(variables))
                throw new ArgumentException("Variables cannot be empty", nameof(variables));
            
            if (string.IsNullOrWhiteSpace(createdBy))
                throw new ArgumentException("CreatedBy cannot be empty", nameof(createdBy));
            
            Name = name.Trim();
            Description = description.Trim();
            Type = type;
            Specialty = specialty;
            Content = content.Trim();
            Variables = variables.Trim();
            CreatedBy = createdBy.Trim();
            IsActive = true;
        }
        
        public void Update(string name, string description, string content, string variables)
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
            Content = content.Trim();
            Variables = variables.Trim();
            UpdateTimestamp();
        }
        
        public void SetActiveStatus(bool isActive)
        {
            IsActive = isActive;
            UpdateTimestamp();
        }
    }
}
