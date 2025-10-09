using System;
using MedicSoft.Domain.Common;

namespace MedicSoft.Domain.Entities
{
    /// <summary>
    /// Represents a reusable template for prescriptions.
    /// Allows clinics to standardize their prescription format.
    /// </summary>
    public class PrescriptionTemplate : BaseEntity
    {
        public string Name { get; private set; }
        public string Description { get; private set; }
        public string TemplateContent { get; private set; }
        public string Category { get; private set; }
        public bool IsActive { get; private set; }

        private PrescriptionTemplate()
        {
            // EF Constructor
            Name = null!;
            Description = null!;
            TemplateContent = null!;
            Category = null!;
        }

        public PrescriptionTemplate(string name, string description, string templateContent, 
            string category, string tenantId) : base(tenantId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(templateContent))
                throw new ArgumentException("Template content cannot be empty", nameof(templateContent));

            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category cannot be empty", nameof(category));

            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            TemplateContent = templateContent.Trim();
            Category = category.Trim();
            IsActive = true;
        }

        public void Update(string name, string description, string templateContent, string category)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Name cannot be empty", nameof(name));

            if (string.IsNullOrWhiteSpace(templateContent))
                throw new ArgumentException("Template content cannot be empty", nameof(templateContent));

            if (string.IsNullOrWhiteSpace(category))
                throw new ArgumentException("Category cannot be empty", nameof(category));

            Name = name.Trim();
            Description = description?.Trim() ?? string.Empty;
            TemplateContent = templateContent.Trim();
            Category = category.Trim();
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
